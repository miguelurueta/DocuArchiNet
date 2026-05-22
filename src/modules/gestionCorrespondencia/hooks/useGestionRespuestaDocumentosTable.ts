import { useCallback, useMemo, useRef, useState } from "react";
import type {
  AppTreeTableLoadChildrenResult,
  AppTreeTableLoadResult,
  AppTreeTableRow,
} from "../../../app/Components/UI/AppTreeTable";
import { actionListaDocumentosRadicados, queryListaDocumentosRadicados, resolveDocumentoVisualizacion } from "../services/listaDocumentosRadicados.service";
import { getSolicitaGabinetePorTareaWorkflow } from "../services/solicitaGabineteRadicadoWorkflow.service";
import { buildListaDocumentosRadicadosActionRequest } from "../adapters/documentosWorkbenchActionMapper";
import {
  adaptListaDocumentosRadicadosToWorkbenchModel,
  resolveDocumentWorkbenchRowId,
} from "../adapters/documentosWorkbenchResponseAdapter";
import {
  buildListaDocumentosRadicadosChildrenQuery,
  buildListaDocumentosRadicadosRootQuery,
} from "../adapters/gestionRespuestaDocumentosRequestMapper";
import type { ListaDocumentosRadicadosRowDto } from "../types/listaDocumentosRadicados.types";

const DEFAULT_TABLE_ID = "InboxListaDocumentosRadicado";

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

const resolveFileUrlFromResolveResponse = (input: unknown): string | undefined => {
  if (!input || typeof input !== "object") return undefined;
  const obj = input as Record<string, unknown>;
  return (
    readString(obj, "fileUrl", "FileUrl", "url", "Url") ??
    readString(obj.data, "fileUrl", "FileUrl", "url", "Url") ??
    readString(obj.meta, "fileUrl", "FileUrl", "url", "Url")
  );
};

export type GestionRespuestaDocumentoActivo = {
  fileUrl: string;
  documentId?: number;
  nombreGabinete?: string;
  rowId: string;
};

export const useGestionRespuestaDocumentosTable = (idTareaWf?: number) => {
  const latestRowRef = useRef<Map<string, ListaDocumentosRadicadosRowDto>>(new Map());
  const gabineteRef = useRef<string | undefined>(undefined);
  const tableIdRef = useRef<string>(DEFAULT_TABLE_ID);
  const [tableColumns, setTableColumns] = useState<import("ag-grid-community").ColDef<Record<string, unknown>>[]>();
  const [columns, setColumns] = useState<string[]>();

  const load = useCallback(async (): Promise<AppTreeTableLoadResult> => {
    try {
      let gabinete: string | undefined;
      if (typeof idTareaWf === "number" && Number.isFinite(idTareaWf) && idTareaWf > 0) {
        const gabineteResponse = await getSolicitaGabinetePorTareaWorkflow(idTareaWf);
        if (!gabineteResponse.success) {
          const message =
            gabineteResponse.errors?.[0]?.errorMessage ??
            gabineteResponse.message ??
            "No fue posible resolver el gabinete del radicado.";
          return { ok: false, message };
        }

        gabinete = gabineteResponse.data?.NombreGabinete;
      }

      gabineteRef.current = gabinete;
      const response = await queryListaDocumentosRadicados(buildListaDocumentosRadicadosRootQuery({ idTareaWf, nombreGabinete: gabinete }));
      if (!response.success || !response.data) {
        const message =
          response.errors?.[0]?.errorMessage ?? response.message ?? "No fue posible cargar el listado.";
        return { ok: false, message };
      }

      const model = adaptListaDocumentosRadicadosToWorkbenchModel(response.data, { viewMode: "flatDocuments" });
      latestRowRef.current = new Map(
        (response.data.Rows ?? []).map((row, index) => [resolveDocumentWorkbenchRowId(row, index), row]),
      );
      tableIdRef.current = model.tableId || DEFAULT_TABLE_ID;
      const resolvedColumns = model.columns && model.columns.length > 0 ? model.columns : inferColumnsFromRows(model.rows);
      setTableColumns(model.tableColumns);
      setColumns(resolvedColumns);
      return { ok: true, rows: model.rows };
    } catch {
      return { ok: false, message: "No fue posible cargar el listado." };
    }
  }, [idTareaWf]);

  const loadChildren = useCallback(
    async (row: AppTreeTableRow): Promise<AppTreeTableLoadChildrenResult> => {
      const parentNodeType = String(row.meta?.NodeType ?? row.meta?.nodeType ?? "");
      const gabinete = gabineteRef.current;

      try {
        const response = await queryListaDocumentosRadicados(
          buildListaDocumentosRadicadosChildrenQuery({
            nombreGabinete: gabinete,
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
        return { ok: true, rows: model.rows };
      } catch {
        return { ok: false, message: "No fue posible cargar el listado." };
      }
    },
    [],
  );

  const resolveFromActionResponse = useCallback(async (actionResponse: unknown): Promise<GestionRespuestaDocumentoActivo | null> => {
    if (!actionResponse || typeof actionResponse !== "object") return null;
    const response = actionResponse as import("../types/listaDocumentosRadicados.types").ApiResponse<
      import("../types/listaDocumentosRadicados.types").ListaDocumentosRadicadosActionData
    >;

    const resolveRequest = response.data?.DocumentResolveRequest;
    if (!response.success || !resolveRequest) {
      return null;
    }

    const resolved = await resolveDocumentoVisualizacion(resolveRequest);
    const fileUrl = resolveFileUrlFromResolveResponse(resolved);
    if (!fileUrl) {
      return null;
    }

    return {
      fileUrl,
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
    async (input: { actionId: string; rowId: string }): Promise<GestionRespuestaDocumentoActivo | null> => {
      const { nodeType, idDocumento, documentId, gabinete } = buildActionContextFromRow(input.rowId);
      if (!gabinete) return null;
      const tableId = tableIdRef.current || DEFAULT_TABLE_ID;

      const actionResponse = await actionListaDocumentosRadicados(
        buildListaDocumentosRadicadosActionRequest({
          context: { tableId, viewMode: "flatDocuments" },
          actionId: input.actionId,
          rowId: input.rowId,
          nodeType,
          idDocumento,
          documentId,
          nombreGabinete: gabinete,
        }),
      );

      if (actionResponse.data?.RequiresReloadNode) {
        // Mejor esfuerzo: refrescar la lista raíz sin romper UX.
        await load();
      }

      const resolved = await resolveFromActionResponse(actionResponse);
      if (!resolved) return null;

      return { ...resolved, rowId: input.rowId };
    },
    [buildActionContextFromRow, load, resolveFromActionResponse],
  );

  const performVerDocumento = useCallback(
    async (rowId: string): Promise<GestionRespuestaDocumentoActivo | null> => {
      const result = await performAction({ actionId: "ver_documento", rowId });
      if (!result) return null;
      return { ...result, rowId };
    },
    [performAction],
  );

  const onSelectRow = useCallback(async (rowId: string) => performVerDocumento(rowId), [performVerDocumento]);

  const onActionTriggered = useCallback(
    async (params: { actionId: string; rowId: string }) => {
      return performAction(params);
    },
    [performAction],
  );

  const getTableColumns = useCallback(() => tableColumns, [tableColumns]);
  const getColumns = useCallback(() => columns, [columns]);

  return useMemo(
    () => ({
      load,
      loadChildren,
      onSelectRow,
      onActionTriggered,
      getTableColumns,
      getColumns,
    }),
    [getColumns, getTableColumns, load, loadChildren, onActionTriggered, onSelectRow],
  );
};
