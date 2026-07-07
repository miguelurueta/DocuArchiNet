import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type {
  AppTreeTableLoadChildrenResult,
  AppTreeTableLoadResult,
  AppTreeTableRow,
} from "../../../app/Components/UI/AppTreeTable";
import {
  actionListaDocumentosRadicados,
  queryListaDocumentosRadicados,
} from "../services/listaDocumentosRadicados.service";
import { eliminarDocumentoStorageEngine } from "../services/eliminarDocumentoStorageEngine.service";
import { buildListaDocumentosRadicadosActionRequest } from "../adapters/documentosWorkbenchActionMapper";
import {
  adaptListaDocumentosRadicadosToWorkbenchModel,
  resolveDocumentWorkbenchRowId,
  resolveListaDocumentosRadicadosTotal,
} from "../adapters/documentosWorkbenchResponseAdapter";
import {
  buildListaDocumentosRadicadosChildrenQuery,
  buildListaDocumentosRadicadosRootQuery,
} from "../adapters/gestionRespuestaDocumentosRequestMapper";
import type {
  ApiResponse,
  DocumentRelationScope,
  ListaDocumentosRadicadosQueryData,
  ListaDocumentosRadicadosRowDto,
} from "../types/listaDocumentosRadicados.types";
import { useGestionRespuestaDocumentos } from "./useGestionRespuestaDocumentos";
import type { AppTableQueryState } from "../../../app/Components/UI/AppTable/types/appTableQueryState.types";

const DEFAULT_TABLE_ID = "InboxListaDocumentosRadicado";
const DEFAULT_DOCUMENT_RELATION_SCOPE: DocumentRelationScope = "documentsOnly";
const DOCUMENTOS_ENABLE_PAGINATION = false;
const DEFAULT_DOCUMENTOS_QUERY_STATE: AppTableQueryState = {
  page: 1,
  pageSize: 25,
  search: "",
  structuredFilters: [],
  sortField: "ID",
  sortDir: "asc",
};
const RADICADO_REQUIRED_MESSAGE =
  "No fue posible cargar documentos: el radicado de la tarea es obligatorio.";
const RADICADO_NOT_FOUND_MESSAGE =
  "No fue posible cargar documentos: el radicado no existe para la tarea.";

const isEstadoExistenciaNo = (value: unknown): boolean =>
  typeof value === "string" && value.trim().toUpperCase() === "NO";

const normalizeSearchText = (value: unknown): string => {
  if (value === null || value === undefined) return "";
  return String(value)
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .toLocaleLowerCase("es-CO");
};

const rowMatchesSearch = (
  row: ListaDocumentosRadicadosRowDto,
  search: string,
): boolean => {
  const normalizedSearch = normalizeSearchText(search).trim();
  if (!normalizedSearch) return true;

  const tokens = normalizedSearch.split(/\s+/).filter(Boolean);
  const searchableText = [
    row.RowId,
    ...Object.values(row.Values ?? {}),
    ...Object.values(row.Meta ?? {}),
  ]
    .map(normalizeSearchText)
    .join(" ");

  return tokens.every((token) => searchableText.includes(token));
};

const filterRowsBySearch = (
  rows: ListaDocumentosRadicadosRowDto[],
  search: string,
): ListaDocumentosRadicadosRowDto[] => {
  const normalizedSearch = normalizeSearchText(search).trim();
  if (!normalizedSearch) return rows;
  return rows.filter((row) => rowMatchesSearch(row, normalizedSearch));
};

const readString = (record: unknown, ...keys: string[]): string | undefined => {
  if (!record || typeof record !== "object") return undefined;
  const source = record as Record<string, unknown>;
  for (const key of keys) {
    const value = source[key];
    if (typeof value === "string" && value.trim().length > 0)
      return value.trim();
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

const inferColumnsFromRows = (
  rows: AppTreeTableRow[],
): string[] | undefined => {
  const first = rows.find(
    (row) => row.values && Object.keys(row.values).length > 0,
  );
  if (!first?.values) return undefined;
  const keys = Object.keys(first.values);
  return keys.length > 0 ? keys : undefined;
};

const DEBUG_GESTION_RESPUESTA_DOCUMENTOS_TABLE =
  typeof import.meta !== "undefined" &&
  Boolean(import.meta.env?.DEV) &&
  import.meta.env?.MODE !== "test";

const resolveBackendTotal = (
  response: ApiResponse<ListaDocumentosRadicadosQueryData>,
  modelTotal?: number,
): number | undefined => {
  const metaTotal = response.meta?.total ?? response.meta?.Total;
  if (
    typeof metaTotal === "number" &&
    Number.isFinite(metaTotal) &&
    metaTotal >= 0
  ) {
    return metaTotal;
  }

  if (
    typeof modelTotal === "number" &&
    Number.isFinite(modelTotal) &&
    modelTotal >= 0
  ) {
    return modelTotal;
  }

  return resolveListaDocumentosRadicadosTotal(response.data ?? { Rows: [] });
};

export type GestionRespuestaDocumentoActivo = {
  documentResolveRequest: { IdDocumento: number; NombreGabinete: string };
  documentId?: number;
  nombreGabinete?: string;
  rowId: string;
};

export type GestionRespuestaDeleteActionResult = {
  success: boolean;
  message: string;
  severity: "success" | "warning" | "error";
  requestId?: string;
  httpStatus?: number;
  rawResponse?: unknown;
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
  const latestRowRef = useRef<Map<string, ListaDocumentosRadicadosRowDto>>(
    new Map(),
  );
  const lastSuccessfulRowsRef = useRef<AppTreeTableRow[]>([]);
  const gabineteRef = useRef<{
    nombreGabinete?: string;
    radicado?: string;
    estadoExistencia?: string;
  }>({});
  const tableIdRef = useRef<string>(DEFAULT_TABLE_ID);
  const loadSeqRef = useRef(0);
  const [queryState, setQueryState] = useState<AppTableQueryState>(
    DEFAULT_DOCUMENTOS_QUERY_STATE,
  );
  const [documentRelationScope, setDocumentRelationScopeState] =
    useState<DocumentRelationScope>(DEFAULT_DOCUMENT_RELATION_SCOPE);
  const [loading, setLoading] = useState(false);
  const [tableColumns, setTableColumns] =
    useState<import("ag-grid-community").ColDef<Record<string, unknown>>[]>();
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
    setSelectedRowIds([]);
    setTableColumns(undefined);
    setColumns(undefined);
    setQueryState(DEFAULT_DOCUMENTOS_QUERY_STATE);
    setDocumentRelationScopeState(DEFAULT_DOCUMENT_RELATION_SCOPE);
    setCountState({
      rowsCount: 0,
      backendTotal: undefined,
      runtimePreferred: false,
    });
  }, [idTareaWf]);

  const loadDocuments = useCallback(
    async (options?: {
      enablePagination?: boolean | null;
      page?: number;
      pageSize?: number;
      documentRelationScope?: DocumentRelationScope;
    }): Promise<AppTreeTableLoadResult> => {
      const seq = ++loadSeqRef.current;
      setLoading(true);
      try {
        const effectiveEnablePagination =
          options?.enablePagination ?? DOCUMENTOS_ENABLE_PAGINATION;
        const effectivePage = effectiveEnablePagination
          ? (options?.page ?? queryState.page)
          : 1;
        const effectivePageSize = options?.pageSize ?? queryState.pageSize;
        const effectiveScope =
          options?.documentRelationScope ?? documentRelationScope;
        const effectiveSearch = queryState.search.trim();
        debugGestionRespuestaDocumentosTable("load start", {
          seq,
          idTareaWf,
          page: effectivePage,
          pageSize: effectivePageSize,
          documentRelationScope: effectiveScope,
          search: effectiveSearch,
          contextNombreGabinete,
          contextRadicado,
          gabineteLoading,
          hasGabineteError: Boolean(gabineteError),
        });
        let nombreGabinete: string | undefined;
        let radicado: string | undefined;
        let estadoExistenciaRadicado: string | undefined;
        const hasValidTask =
          typeof idTareaWf === "number" &&
          Number.isFinite(idTareaWf) &&
          idTareaWf > 0;

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
            if (seq === loadSeqRef.current) setLoading(false);
            return { ok: false, message: RADICADO_REQUIRED_MESSAGE };
          }

          if (isEstadoExistenciaNo(estadoExistenciaRadicado)) {
            gabineteRef.current = {
              nombreGabinete,
              radicado: resolvedRadicado,
              estadoExistencia: estadoExistenciaRadicado,
            };
            if (seq === loadSeqRef.current) setLoading(false);
            return { ok: false, message: RADICADO_NOT_FOUND_MESSAGE };
          }
        }

        gabineteRef.current = {
          nombreGabinete,
          radicado: resolvedRadicado,
          estadoExistencia: estadoExistenciaRadicado,
        };
        const response = await queryListaDocumentosRadicados(
          buildListaDocumentosRadicadosRootQuery({
            idTareaWf,
            nombreGabinete,
            radicado: hasValidTask ? resolvedRadicado : undefined,
            documentRelationScope: effectiveScope,
            enablePagination: effectiveEnablePagination,
            page: effectivePage,
            pageSize: effectivePageSize,
            search: effectiveEnablePagination ? effectiveSearch : "",
            searchType: queryState.searchType,
          }),
        );
        debugGestionRespuestaDocumentosTable("load response", {
          seq,
          success: response.success,
          rows: response.data?.Rows?.length ?? 0,
          message: response.message,
        });
        if (seq !== loadSeqRef.current) {
          // La carga quedó obsoleta por cambio de tarea: no limpiar el UI ni mostrar error.
          return { ok: true, rows: lastSuccessfulRowsRef.current };
        }
        if (!response.success || !response.data) {
          const message =
            response.errors?.[0]?.errorMessage ??
            response.message ??
            "No fue posible cargar el listado.";
          return { ok: false, message };
        }

        const filteredRows = effectiveEnablePagination
          ? (response.data.Rows ?? [])
          : filterRowsBySearch(response.data.Rows ?? [], effectiveSearch);
        const filteredData = { ...response.data, Rows: filteredRows };
        const model = adaptListaDocumentosRadicadosToWorkbenchModel(filteredData, {
          viewMode: "flatDocuments",
        });
        latestRowRef.current = new Map(
          filteredRows.map((row, index) => [
            resolveDocumentWorkbenchRowId(row, index),
            row,
          ]),
        );
        const backendTotal = effectiveSearch
          ? model.rows.length
          : resolveBackendTotal(response, model.total);
        if (seq !== loadSeqRef.current) {
          // Evitar mostrar error si cambió la tarea durante la actualización de estado.
          return { ok: true, rows: lastSuccessfulRowsRef.current };
        }
        setCountState((prev) => ({
          rowsCount: model.rows.length,
          backendTotal,
          runtimePreferred: effectiveEnablePagination
            ? false
            : prev.runtimePreferred,
        }));
        tableIdRef.current = model.tableId || DEFAULT_TABLE_ID;
        const resolvedColumns =
          model.columns && model.columns.length > 0
            ? model.columns
            : inferColumnsFromRows(model.rows);
        setTableColumns(model.tableColumns);
        setColumns(resolvedColumns);
        lastSuccessfulRowsRef.current = model.rows;
        debugGestionRespuestaDocumentosTable("load success", {
          seq,
          rows: model.rows.length,
          backendTotal,
          tableId: tableIdRef.current,
        });
        return { ok: true, rows: model.rows };
      } catch (error) {
        debugGestionRespuestaDocumentosTable("load error", {
          seq,
          error: error instanceof Error ? error.message : String(error),
        });
        return { ok: false, message: "No fue posible cargar el listado." };
      } finally {
        if (seq === loadSeqRef.current) {
          setLoading(false);
        }
      }
    },
    [
      contextNombreGabinete,
      contextRadicado,
      documentRelationScope,
      gabineteError,
      gabineteLoading,
      idTareaWf,
      queryState.page,
      queryState.pageSize,
      queryState.search,
      queryState.searchType,
    ],
  );

  const loadChildren = useCallback(
    async (row: AppTreeTableRow): Promise<AppTreeTableLoadChildrenResult> => {
      const parentNodeType = String(
        row.meta?.NodeType ?? row.meta?.nodeType ?? "",
      );
      const { nombreGabinete, radicado } = gabineteRef.current;

      try {
        const response = await queryListaDocumentosRadicados(
          buildListaDocumentosRadicadosChildrenQuery({
            nombreGabinete,
            radicado,
            parentRowId: row.id,
            parentNodeType: parentNodeType || null,
            level: Number(row.meta?.Level ?? 2),
            documentRelationScope: "documentsOnly",
            enablePagination: DOCUMENTOS_ENABLE_PAGINATION,
          }),
        );
        if (!response.success || !response.data) {
          const message =
            response.errors?.[0]?.errorMessage ??
            response.message ??
            "No fue posible cargar el listado.";
          return { ok: false, message };
        }

        const model = adaptListaDocumentosRadicadosToWorkbenchModel(
          response.data,
          { viewMode: "hierarchical" },
        );
        tableIdRef.current =
          model.tableId || tableIdRef.current || DEFAULT_TABLE_ID;
        for (const [index, childRow] of (response.data.Rows ?? []).entries()) {
          latestRowRef.current.set(
            resolveDocumentWorkbenchRowId(childRow, index),
            childRow,
          );
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

  const extractResolveRequestFromActionResponse = useCallback(
    (actionResponse: unknown): GestionRespuestaDocumentoActivo | null => {
      if (!actionResponse || typeof actionResponse !== "object") return null;
      const response =
        actionResponse as import("../types/listaDocumentosRadicados.types").ApiResponse<
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
    },
    [],
  );

  const buildActionContextFromRow = useCallback((rowId: string) => {
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
    const idAlmacen =
      readNumber(values, "IdAlmacen", "ID_ALMACEN") ??
      readNumber(meta, "IdAlmacen", "idAlmacen") ??
      documentId ??
      idDocumento;
    const gabinete =
      readString(meta, "NombreGabinete", "nombreGabinete", "NOMBRE_GABINETE") ??
      readString(values, "NOMBRE_GABINETE", "NombreGabinete", "NOMBREGABINETE");

    return { nodeType, idDocumento, documentId, idAlmacen, gabinete };
  }, []);

  const performAction = useCallback(
    async (input: { actionId: string; rowId: string }): Promise<unknown> => {
      const { nodeType, idDocumento, documentId, idAlmacen, gabinete } =
        buildActionContextFromRow(input.rowId);
      const tableId = tableIdRef.current || DEFAULT_TABLE_ID;

      if (input.actionId === "eliminar_item") {
        if (!gabinete || !idAlmacen || idAlmacen <= 0) {
          return {
            success: false,
            message: "No fue posible identificar el documento para eliminar.",
            severity: "error" as const,
          } satisfies GestionRespuestaDeleteActionResult;
        }

        const deleteResult = await eliminarDocumentoStorageEngine({
          idAlmacen,
          nombreGabinete: gabinete,
          sourceModule: "WORKFLOW",
        });

        if (deleteResult.success) {
          latestRowRef.current.delete(input.rowId);
          setCountState((prev) => ({
            ...prev,
            rowsCount: Math.max(0, prev.rowsCount - 1),
            runtimePreferred: true,
          }));
        }

        return deleteResult;
      }

      if (!gabinete) return null;

      const actionRequest = buildListaDocumentosRadicadosActionRequest({
        context: { tableId, viewMode: "flatDocuments" },
        actionId: input.actionId,
        rowId: input.rowId,
        nodeType,
        idDocumento,
        documentId,
        nombreGabinete: gabinete,
      });

      const actionResponse =
        await actionListaDocumentosRadicados(actionRequest);

      if (
        input.actionId === "agregar_item" ||
        input.actionId === "eliminar_item"
      ) {
        setCountState((prev) => ({ ...prev, runtimePreferred: true }));
      }

      if (actionResponse.data?.RequiresReloadNode) {
        // Mejor esfuerzo: refrescar la lista raíz sin romper UX.
        await loadDocuments({ enablePagination: false });
      }

      return actionResponse;
    },
    [buildActionContextFromRow, loadDocuments],
  );

  const performVerDocumento = useCallback(
    async (rowId: string): Promise<GestionRespuestaDocumentoActivo | null> => {
      const actionResponse = await performAction({
        actionId: "ver_documento",
        rowId,
      });
      const extracted = extractResolveRequestFromActionResponse(actionResponse);
      if (!extracted) return null;
      return { ...extracted, rowId };
    },
    [extractResolveRequestFromActionResponse, performAction],
  );

  const onSelectRow = useCallback(
    async (rowId: string) => performVerDocumento(rowId),
    [performVerDocumento],
  );

  const onActionTriggered = useCallback(
    async (params: { actionId: string; rowId: string }) => {
      if (params.actionId === "ver_documento") {
        return performVerDocumento(params.rowId);
      }
      return performAction(params);
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
    if (typeof countState.backendTotal === "number")
      return countState.backendTotal;
    return countState.rowsCount;
  }, [
    countState.backendTotal,
    countState.rowsCount,
    countState.runtimePreferred,
  ]);

  const selectedDocumentsCount = useMemo(
    () => selectedRowIds.length,
    [selectedRowIds.length],
  );

  const getTableColumns = useCallback(() => tableColumns, [tableColumns]);
  const getColumns = useCallback(() => columns, [columns]);
  const getWorkbenchContext = useCallback(
    (): GestionRespuestaWorkbenchContext => ({ ...gabineteRef.current }),
    [],
  );
  const onQueryChange = useCallback((patch: Partial<AppTableQueryState>) => {
    setQueryState((prev) => {
      const pageSizeChanged =
        typeof patch.pageSize === "number" &&
        patch.pageSize > 0 &&
        patch.pageSize !== prev.pageSize;
      const searchChanged =
        typeof patch.search === "string" && patch.search !== prev.search;
      const filtersChanged =
        Array.isArray(patch.structuredFilters) &&
        patch.structuredFilters !== prev.structuredFilters;
      const sortChanged =
        (typeof patch.sortField === "string" &&
          patch.sortField !== prev.sortField) ||
        (typeof patch.sortDir === "string" && patch.sortDir !== prev.sortDir);
      const shouldResetPage =
        pageSizeChanged || searchChanged || filtersChanged || sortChanged;

      return {
        ...prev,
        ...patch,
        page: shouldResetPage ? 1 : (patch.page ?? prev.page),
        pageSize: pageSizeChanged
          ? patch.pageSize!
          : (patch.pageSize ?? prev.pageSize),
      };
    });
  }, []);
  const setDocumentRelationScope = useCallback(
    (nextScope: DocumentRelationScope) => {
      setDocumentRelationScopeState(nextScope);
      setQueryState((prev) => ({ ...prev, page: 1 }));
    },
    [],
  );
  const refresh = useCallback(
    () =>
      loadDocuments({
        enablePagination: DOCUMENTOS_ENABLE_PAGINATION,
        page: queryState.page,
        pageSize: queryState.pageSize,
        documentRelationScope,
      }),
    [
      documentRelationScope,
      loadDocuments,
      queryState.page,
      queryState.pageSize,
    ],
  );
  const load = useCallback(() => refresh(), [refresh]);

  return useMemo(
    () => ({
      load,
      refresh,
      loadChildren,
      onSelectRow,
      onActionTriggered,
      onSelectionChanged,
      queryState,
      onQueryChange,
      documentRelationScope,
      setDocumentRelationScope,
      loading,
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
      refresh,
      loadChildren,
      loading,
      documentRelationScope,
      onQueryChange,
      onActionTriggered,
      onSelectRow,
      onSelectionChanged,
      queryState,
      selectedDocumentsCount,
      setDocumentRelationScope,
      totalDocumentsCount,
    ],
  );
};

function debugGestionRespuestaDocumentosTable(
  message: string,
  payload?: Record<string, unknown>,
): void {
  if (!DEBUG_GESTION_RESPUESTA_DOCUMENTOS_TABLE) {
    return;
  }

  console.info(
    `[useGestionRespuestaDocumentosTable][debug] ${message}`,
    payload ?? {},
  );
}
