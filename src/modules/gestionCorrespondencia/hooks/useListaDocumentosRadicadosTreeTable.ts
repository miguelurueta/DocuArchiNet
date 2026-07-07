import { useCallback, useMemo, useRef } from "react";
import type { AppTreeTableLoadChildrenResult, AppTreeTableLoadResult, AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";
import {
  actionListaDocumentosRadicados,
  queryListaDocumentosRadicados,
  resolveDocumentoVisualizacion,
} from "../services/listaDocumentosRadicados.service";
import type {
  DocumentRelationScope,
  ListaDocumentosRadicadosQueryRequest,
  ListaDocumentosRadicadosRowDto,
} from "../types/listaDocumentosRadicados.types";
import { useGestionRespuestaDocumentos } from "./useGestionRespuestaDocumentos";

const TABLE_ID = "InboxListaRadicados";
const DEFAULT_APLICA_TRD = 0;

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

const buildInitialQuery = (options?: {
  nombreGabinete?: string;
  documentRelationScope?: DocumentRelationScope;
  enablePagination?: boolean | null;
}): ListaDocumentosRadicadosQueryRequest => {
  const base: ListaDocumentosRadicadosQueryRequest = {
    ViewMode: "flatDocuments",
    Page: 1,
    PageSize: 25,
    SortDir: "ASC",
    Search: "",
    StructuredFilters: [],
    IncludeConfig: true,
    EnablePagination: options?.enablePagination ?? true,
    EnableColumnFilters: false,
    ParentRowId: null,
    ParentNodeType: null,
    Level: 1,
    DocumentRelationScope: options?.documentRelationScope ?? "documentsOnly",

    TableId: TABLE_ID,
    CampoRadicado: "",
    Radicado: "",
    AplicaTrd: DEFAULT_APLICA_TRD,
  };

  const nombreGabinete = options?.nombreGabinete?.trim();
  return nombreGabinete ? { ...base, NombreGabinete: nombreGabinete } : base;
};

const mapRow = (row: ListaDocumentosRadicadosRowDto): AppTreeTableRow => {
  const columns = Object.keys(row.Values ?? {});
  const firstValue =
    columns.length > 0 ? row.Values[columns[0]] : undefined;
  const label = typeof firstValue === "string" && firstValue.trim().length > 0
    ? firstValue.trim()
    : String(firstValue ?? row.RowId);

  return {
    id: row.RowId,
    label,
    values: row.Values,
    meta: { ...(row.Meta ?? {}) },
    hasChildren: Boolean(row.Meta?.HasChildren),
    children: row.Meta?.HasChildren ? [] : undefined,
  };
};

export type ListaDocumentosRadicadosTreeTable = {
  columns: string[];
  load: () => Promise<AppTreeTableLoadResult>;
  loadChildren: (row: AppTreeTableRow) => Promise<AppTreeTableLoadChildrenResult>;
  onSelectRow: (rowId: string) => Promise<void>;
};

export const useListaDocumentosRadicadosTreeTable = (): ListaDocumentosRadicadosTreeTable => {
  const latestRowRef = useRef<Map<string, ListaDocumentosRadicadosRowDto>>(new Map());
  const {
    nombreGabinete: gabineteNombreContextual,
    gabineteLoading,
    gabineteError,
  } = useGestionRespuestaDocumentos();

  const load = useCallback(async (): Promise<AppTreeTableLoadResult> => {
    try {
      if (gabineteLoading) {
        return { ok: false, message: "Cargando informacion del gabinete. Intenta nuevamente." };
      }

      if (gabineteError) {
        return { ok: false, message: gabineteError };
      }

      if (!gabineteNombreContextual) {
        return { ok: false, message: "NombreGabinete requerido" };
      }

      const response = await queryListaDocumentosRadicados(
        buildInitialQuery({
          nombreGabinete: gabineteNombreContextual,
          documentRelationScope: "documentsOnly",
          enablePagination: true,
        }),
      );
      if (!response.success || !response.data) {
        const message =
          response.errors?.[0]?.errorMessage ?? response.message ?? "No fue posible cargar el listado.";
        return { ok: false, message };
      }
      const rows = response.data.Rows ?? [];
      latestRowRef.current = new Map(rows.map((row) => [row.RowId, row]));
      return { ok: true, rows: rows.map(mapRow) };
    } catch {
      return { ok: false, message: "No fue posible cargar el listado." };
    }
  }, [gabineteError, gabineteLoading, gabineteNombreContextual]);

  const loadChildren = useCallback(async (row: AppTreeTableRow): Promise<AppTreeTableLoadChildrenResult> => {
    if (gabineteLoading) {
      return { ok: false, message: "Cargando informacion del gabinete. Intenta nuevamente." };
    }

    if (gabineteError) {
      return { ok: false, message: gabineteError };
    }

    if (!gabineteNombreContextual) {
      return { ok: false, message: "NombreGabinete requerido" };
    }

    const parentNodeType = String(row.meta?.NodeType ?? row.meta?.nodeType ?? "");
    const request: ListaDocumentosRadicadosQueryRequest = {
      ...buildInitialQuery({
        nombreGabinete: gabineteNombreContextual,
        documentRelationScope: "documentsOnly",
        enablePagination: true,
      }),
      ViewMode: "hierarchical",
      ParentRowId: row.id,
      ParentNodeType: parentNodeType || null,
      Level: 2,
    };
    try {
      const response = await queryListaDocumentosRadicados(request);
      if (!response.success || !response.data) {
        const message =
          response.errors?.[0]?.errorMessage ?? response.message ?? "No fue posible cargar el listado.";
        return { ok: false, message };
      }
      const rows = response.data.Rows ?? [];
      for (const childRow of rows) {
        latestRowRef.current.set(childRow.RowId, childRow);
      }
      return { ok: true, rows: rows.map(mapRow) };
    } catch {
      return { ok: false, message: "No fue posible cargar el listado." };
    }
  }, [gabineteError, gabineteLoading, gabineteNombreContextual]);

  const onSelectRow = useCallback(async (rowId: string) => {
    const selected = latestRowRef.current.get(rowId);
    const meta = selected?.Meta;

    if (gabineteLoading) {
      throw new Error("Cargando informacion del gabinete. Intenta nuevamente.");
    }

    if (gabineteError) {
      throw new Error(gabineteError);
    }

    const nodeType = readString(meta, "NodeType", "nodeType") ?? "documento";
    const documentIdFromMeta = readNumber(meta, "DocumentId", "documentId");

    if (!gabineteNombreContextual) {
      throw new Error("NombreGabinete requerido");
    }

    const actionResponse = await actionListaDocumentosRadicados({
      TableId: TABLE_ID,
      ViewMode: "flatDocuments",
      ActionId: "ver_documento",
      RowId: rowId,
      ParentRowId: null,
      NodeType: nodeType,
      Payload: {
        IdDocumento: typeof documentIdFromMeta === "number" ? documentIdFromMeta : undefined,
        DocumentId: typeof documentIdFromMeta === "number" ? documentIdFromMeta : undefined,
        NombreGabinete: gabineteNombreContextual,
      },
    });

    const resolveRequest = actionResponse.data?.DocumentResolveRequest;
    if (actionResponse.success && resolveRequest) {
      await resolveDocumentoVisualizacion(resolveRequest);
    }
  }, [gabineteError, gabineteLoading, gabineteNombreContextual]);

  const columns = useMemo(() => {
    // Contracto: si no hay config en response, el orden se infiere de la primera fila.
    // Este hook devuelve columnas [] y el consumidor puede inferirlas desde los rows.
    return [] as string[];
  }, []);

  return {
    columns,
    load,
    loadChildren,
    onSelectRow,
  };
};
