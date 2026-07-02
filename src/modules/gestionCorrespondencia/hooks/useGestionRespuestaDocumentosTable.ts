import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type {
  AppTreeTableLoadChildrenResult,
  AppTreeTableLoadResult,
  AppTreeTableRow,
} from "../../../app/Components/UI/AppTreeTable";
import { actionListaDocumentosRadicados, queryListaDocumentosRadicados } from "../services/listaDocumentosRadicados.service";
import { buildListaDocumentosRadicadosActionRequest } from "../adapters/documentosWorkbenchActionMapper";
import {
  adaptListaDocumentosRadicadosToWorkbenchModel,
  resolveDocumentWorkbenchRowId,
} from "../adapters/documentosWorkbenchResponseAdapter";
import {
  buildListaDocumentosRadicadosChildrenQuery,
  buildListaDocumentosRadicadosRootQuery,
} from "../adapters/gestionRespuestaDocumentosRequestMapper";
import type {
  ApiResponse,
  ListaDocumentosRadicadosQueryData,
  ListaDocumentosRadicadosRowDto,
} from "../types/listaDocumentosRadicados.types";
import { useGestionRespuestaDocumentos } from "./useGestionRespuestaDocumentos";

const DEFAULT_TABLE_ID = "InboxListaDocumentosRadicado";
const RADICADO_REQUIRED_MESSAGE =
  "No fue posible cargar documentos: el radicado de la tarea es obligatorio.";
const RADICADO_NOT_FOUND_MESSAGE =
  "No fue posible cargar documentos: el radicado no existe para la tarea.";

const isEstadoExistenciaNo = (value: unknown): boolean =>
  typeof value === "string" && value.trim().toUpperCase() === "NO";

const readString = (record: unknown, ...keys: string[]): string | undefined => {
  if (!record || typeof record !== "object") return undefined;
  const source = record as Record<string, unknown>;
  for (const key of keys) {
    const value = source[key];
    if (typeof value === "string" && value.trim().length > 0) return value.trim();
  }
  return undefined;
};

const readNumber = (record: unknown, ...keys: string[]): number | undefined => {
  if (!record || typeof record !== "object") return undefined;
  const source = record as Record<string, unknown>;
  for (const key of keys) {
    const value = source[key];
    if (typeof value === "number" && Number.isFinite(value)) return value;
  }
  return undefined;
};

const inferColumnsFromRows = (rows: AppTreeTableRow[]): string[] | undefined => {
  const first = rows.find((row) => row.values && Object.keys(row.values).length > 0);
  if (!first?.values) return undefined;
  const keys = Object.keys(first.values);
  return keys.length > 0 ? keys : undefined;
};

const readTotalCandidate = (value: unknown): number | undefined => {
  if (!value || typeof value !== "object") return undefined;
  const source = value as Record<string, unknown>;
  const candidates = [
    source.Total,
    source.total,
    source.TotalRecords,
    source.totalRecords,
  ];

  for (const candidate of candidates) {
    if (typeof candidate === "number" && Number.isFinite(candidate) && candidate >= 0) {
      return candidate;
    }
  }

  return undefined;
};

const resolveBackendTotal = (
  response: ApiResponse<ListaDocumentosRadicadosQueryData>,
): number | undefined =>
  readTotalCandidate(response.data) ??
  readTotalCandidate(response.meta) ??
  readTotalCandidate(response);

export type GestionRespuestaDocumentoActivo = {
  documentResolveRequest: { IdDocumento: number; NombreGabinete: string };
  documentId?: number;
  nombreGabinete?: string;
  rowId: string;
};

export type GestionRespuestaWorkbenchContext = {
  nombreGabinete?: string;
  radicado?: string;
};

type DocumentosCountState = {
  rowsCount: number;
  backendTotal?: number;
  runtimePreferred: boolean;
};

export const useGestionRespuestaDocumentosTable = (idTareaWf?: number) => {
  const {
    nombreGabinete: contextNombreGabinete,
    radicado: contextRadicado,
    gabineteLoading,
    gabineteError,
  } = useGestionRespuestaDocumentos();
  const latestRowRef = useRef<Map<string, ListaDocumentosRadicadosRowDto>>(new Map());
  const lastSuccessfulRowsRef = useRef<AppTreeTableRow[]>([]);
  const gabineteRef = useRef<{ nombreGabinete?: string; radicado?: string; estadoExistencia?: string }>({});
  const tableIdRef = useRef<string>(DEFAULT_TABLE_ID);
  const loadSeqRef = useRef(0);
  const [tableColumns, setTableColumns] = useState<import("ag-grid-community").ColDef<Record<string, unknown>>[]>();
  const [columns, setColumns] = useState<string[]>();
  const [selectedRowIds, setSelectedRowIds] = useState<string[]>([]);
  const [countState, setCountState] = useState<DocumentosCountState>({
    rowsCount: 0,
    backendTotal: undefined,
    runtimePreferred: false,
  });

  useEffect(() => {
    // Reset visual state when task changes to avoid rendering stale rows.
    // Nota: no incrementamos `loadSeqRef` aquí porque `load()` puede ejecutarse antes de los effects,
    // lo que produciría cancelaciones falsas y un listado vacío.
    latestRowRef.current.clear();
    lastSuccessfulRowsRef.current = [];
    gabineteRef.current = {};
    tableIdRef.current = DEFAULT_TABLE_ID;
    // eslint-disable-next-line react-hooks/set-state-in-effect -- Limpieza deliberada al cambiar de tarea para evitar estado visual stale.
    setSelectedRowIds([]);
    setTableColumns(undefined);
    setColumns(undefined);
    setCountState({ rowsCount: 0, backendTotal: undefined, runtimePreferred: false });
  }, [idTareaWf]);

  const load = useCallback(async (): Promise<AppTreeTableLoadResult> => {
    const seq = ++loadSeqRef.current;
    try {
      let nombreGabinete: string | undefined;
      let radicado: string | undefined;
      let estadoExistenciaRadicado: string | undefined;
      const hasValidTask =
        typeof idTareaWf === "number" && Number.isFinite(idTareaWf) && idTareaWf > 0;

      if (hasValidTask) {
        if (gabineteLoading) {
          return { ok: true, rows: lastSuccessfulRowsRef.current };
        }

        if (gabineteError) {
          return { ok: false, message: gabineteError };
        }

        nombreGabinete = contextNombreGabinete;
        radicado = contextRadicado;
      }

      const resolvedRadicado = radicado?.trim();
      if (hasValidTask) {
        if (!resolvedRadicado) {
          gabineteRef.current = {
            nombreGabinete,
            radicado: resolvedRadicado,
            estadoExistencia: estadoExistenciaRadicado,
          };
          return { ok: false, message: RADICADO_REQUIRED_MESSAGE };
        }

        if (isEstadoExistenciaNo(estadoExistenciaRadicado)) {
          gabineteRef.current = {
            nombreGabinete,
            radicado: resolvedRadicado,
            estadoExistencia: estadoExistenciaRadicado,
          };
          return { ok: false, message: RADICADO_NOT_FOUND_MESSAGE };
        }
      }

      gabineteRef.current = { nombreGabinete, radicado: resolvedRadicado, estadoExistencia: estadoExistenciaRadicado };
      const response = await queryListaDocumentosRadicados(
        buildListaDocumentosRadicadosRootQuery({
          idTareaWf,
          nombreGabinete,
          radicado: hasValidTask ? resolvedRadicado : undefined,
        }),
      );
      if (seq !== loadSeqRef.current) {
        // La carga quedó obsoleta por cambio de tarea: no limpiar el UI ni mostrar error.
        return { ok: true, rows: lastSuccessfulRowsRef.current };
      }
      if (!response.success || !response.data) {
        const message =
          response.errors?.[0]?.errorMessage ?? response.message ?? "No fue posible cargar el listado.";
        return { ok: false, message };
      }

      const model = adaptListaDocumentosRadicadosToWorkbenchModel(response.data, { viewMode: "flatDocuments" });
      latestRowRef.current = new Map(
        (response.data.Rows ?? []).map((row, index) => [resolveDocumentWorkbenchRowId(row, index), row]),
      );
      const backendTotal = resolveBackendTotal(response);
      if (seq !== loadSeqRef.current) {
        // Evitar mostrar error si cambió la tarea durante la actualización de estado.
        return { ok: true, rows: lastSuccessfulRowsRef.current };
      }
      setCountState((prev) => ({
        rowsCount: model.rows.length,
        backendTotal,
        runtimePreferred: prev.runtimePreferred,
      }));
      tableIdRef.current = model.tableId || DEFAULT_TABLE_ID;
      const resolvedColumns = model.columns && model.columns.length > 0 ? model.columns : inferColumnsFromRows(model.rows);
      setTableColumns(model.tableColumns);
      setColumns(resolvedColumns);
      lastSuccessfulRowsRef.current = model.rows;
      return { ok: true, rows: model.rows };
    } catch {
      return { ok: false, message: "No fue posible cargar el listado." };
    }
  }, [contextNombreGabinete, contextRadicado, gabineteError, gabineteLoading, idTareaWf]);

  const loadChildren = useCallback(
    async (row: AppTreeTableRow): Promise<AppTreeTableLoadChildrenResult> => {
      const parentNodeType = String(row.meta?.NodeType ?? row.meta?.nodeType ?? "");
      const { nombreGabinete, radicado } = gabineteRef.current;

      try {
        const response = await queryListaDocumentosRadicados(
          buildListaDocumentosRadicadosChildrenQuery({
            nombreGabinete,
            radicado,
            parentRowId: row.id,
            parentNodeType: parentNodeType || null,
            level: Number(row.meta?.Level ?? 2),
          }),
        );
        if (!response.success || !response.data) {
          const message =
            response.errors?.[0]?.errorMessage ?? response.message ?? "No fue posible cargar el listado.";
          return { ok: false, message };
        }

        const model = adaptListaDocumentosRadicadosToWorkbenchModel(response.data, { viewMode: "hierarchical" });
        tableIdRef.current = model.tableId || tableIdRef.current || DEFAULT_TABLE_ID;
        for (const [index, childRow] of (response.data.Rows ?? []).entries()) {
          latestRowRef.current.set(resolveDocumentWorkbenchRowId(childRow, index), childRow);
        }
        setCountState((prev) => ({
          ...prev,
          rowsCount: latestRowRef.current.size,
        }));
        return { ok: true, rows: model.rows };
      } catch {
        return { ok: false, message: "No fue posible cargar el listado." };
      }
    },
    [],
  );

  const extractResolveRequestFromActionResponse = useCallback((actionResponse: unknown): GestionRespuestaDocumentoActivo | null => {
    if (!actionResponse || typeof actionResponse !== "object") return null;
    const response = actionResponse as import("../types/listaDocumentosRadicados.types").ApiResponse<
      import("../types/listaDocumentosRadicados.types").ListaDocumentosRadicadosActionData
    >;

    const resolveRequest = response.data?.DocumentResolveRequest;
    if (!response.success || !resolveRequest) {
      return null;
    }

    return {
      documentResolveRequest: resolveRequest,
      documentId: resolveRequest.IdDocumento,
      nombreGabinete: resolveRequest.NombreGabinete,
      rowId: "",
    };
  }, []);

  const buildActionContextFromRow = useCallback(
    (rowId: string) => {
      const selected = latestRowRef.current.get(rowId);
      const meta = selected?.Meta;
      const values = selected?.Values;

      const nodeType = readString(meta, "NodeType", "nodeType") ?? "documento";
      const idDocumento =
        readNumber(values, "IdDocumento", "ID_DOCUMENTO", "IDDOCUMENTO") ??
        readNumber(meta, "IdDocumento", "idDocumento", "ID_DOCUMENTO");
      const documentId =
        readNumber(values, "DocumentId", "DOCUMENTID") ??
        readNumber(meta, "DocumentId", "documentId");
      const gabinete =
        readString(meta, "NombreGabinete", "nombreGabinete", "NOMBRE_GABINETE") ??
        readString(values, "NOMBRE_GABINETE", "NombreGabinete", "NOMBREGABINETE");

      return { nodeType, idDocumento, documentId, gabinete };
    },
    [],
  );

  const performAction = useCallback(
    async (input: { actionId: string; rowId: string }): Promise<unknown> => {
      const { nodeType, idDocumento, documentId, gabinete } = buildActionContextFromRow(input.rowId);

      if (!gabinete) return null;
      const tableId = tableIdRef.current || DEFAULT_TABLE_ID;

      const actionRequest = buildListaDocumentosRadicadosActionRequest({
        context: { tableId, viewMode: "flatDocuments" },
        actionId: input.actionId,
        rowId: input.rowId,
        nodeType,
        idDocumento,
        documentId,
        nombreGabinete: gabinete,
      });

      const actionResponse = await actionListaDocumentosRadicados(actionRequest);

      if (input.actionId === "agregar_item" || input.actionId === "eliminar_item") {
        setCountState((prev) => ({ ...prev, runtimePreferred: true }));
      }

      if (actionResponse.data?.RequiresReloadNode) {
        // Mejor esfuerzo: refrescar la lista raíz sin romper UX.
        await load();
      }

      return actionResponse;
    },
    [buildActionContextFromRow, load],
  );

  const performVerDocumento = useCallback(
    async (rowId: string): Promise<GestionRespuestaDocumentoActivo | null> => {
      const actionResponse = await performAction({ actionId: "ver_documento", rowId });
      const extracted = extractResolveRequestFromActionResponse(actionResponse);
      if (!extracted) return null;
      return { ...extracted, rowId };
    },
    [extractResolveRequestFromActionResponse, performAction],
  );

  const onSelectRow = useCallback(async (rowId: string) => performVerDocumento(rowId), [performVerDocumento]);

  const onActionTriggered = useCallback(
    async (params: { actionId: string; rowId: string }) => {
      if (params.actionId === "ver_documento") {
        return performVerDocumento(params.rowId);
      }
      await performAction(params);
      return null;
    },
    [performAction, performVerDocumento],
  );

  const onSelectionChanged = useCallback((rowIds: string[]) => {
    setSelectedRowIds(Array.from(new Set(rowIds)));
  }, []);

  useEffect(() => {
    if (selectedRowIds.length === 0) return;
    setSelectedRowIds((prev) => {
      const filtered = prev.filter((rowId) => latestRowRef.current.has(rowId));
      return filtered.length === prev.length ? prev : filtered;
    });
  }, [countState.rowsCount, selectedRowIds]);

  const totalDocumentsCount = useMemo(() => {
    if (countState.runtimePreferred) return countState.rowsCount;
    if (typeof countState.backendTotal === "number") return countState.backendTotal;
    return countState.rowsCount;
  }, [countState.backendTotal, countState.rowsCount, countState.runtimePreferred]);

  const selectedDocumentsCount = useMemo(() => selectedRowIds.length, [selectedRowIds.length]);

  const getTableColumns = useCallback(() => tableColumns, [tableColumns]);
  const getColumns = useCallback(() => columns, [columns]);
  const getWorkbenchContext = useCallback(
    (): GestionRespuestaWorkbenchContext => ({ ...gabineteRef.current }),
    [],
  );

  return useMemo(
    () => ({
      load,
      loadChildren,
      onSelectRow,
      onActionTriggered,
      onSelectionChanged,
      getTableColumns,
      getColumns,
      getWorkbenchContext,
      totalDocumentsCount,
      selectedDocumentsCount,
    }),
    [
      getColumns,
      getTableColumns,
      getWorkbenchContext,
      load,
      loadChildren,
      onActionTriggered,
      onSelectRow,
      onSelectionChanged,
      selectedDocumentsCount,
      totalDocumentsCount,
    ],
  );
};
