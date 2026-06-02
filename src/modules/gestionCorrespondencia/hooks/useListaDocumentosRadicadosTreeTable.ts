import { useCallback, useMemo, useRef } from "react";
import type { AppTreeTableLoadChildrenResult, AppTreeTableLoadResult, AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";
import {
  actionListaDocumentosRadicados,
  queryListaDocumentosRadicados,
  resolveDocumentoVisualizacion,
} from "../services/listaDocumentosRadicados.service";
import { getSolicitaGabinetePorTareaWorkflow } from "../services/solicitaGabineteRadicadoWorkflow.service";
import type {
  ListaDocumentosRadicadosQueryRequest,
  ListaDocumentosRadicadosRowDto,
} from "../types/listaDocumentosRadicados.types";

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
}): ListaDocumentosRadicadosQueryRequest => {
  const base: ListaDocumentosRadicadosQueryRequest = {
    ViewMode: "flatDocuments",
    Page: 1,
    PageSize: 25,
    SortDir: "ASC",
    Search: "",
    StructuredFilters: [],
    IncludeConfig: true,
    EnablePagination: false,
    EnableColumnFilters: false,
    ParentRowId: null,
    ParentNodeType: null,
    Level: 1,

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

export const useListaDocumentosRadicadosTreeTable = (
  idTareaWf?: number,
): ListaDocumentosRadicadosTreeTable => {
  const latestRowRef = useRef<Map<string, ListaDocumentosRadicadosRowDto>>(new Map());
  const gabineteRef = useRef<string | undefined>(undefined);

  const load = useCallback(async (): Promise<AppTreeTableLoadResult> => {
    try {
      let gabinete: string | undefined;
      if (typeof idTareaWf === "number" && Number.isFinite(idTareaWf) && idTareaWf > 0) {
        const gabineteResponse = await getSolicitaGabinetePorTareaWorkflow(idTareaWf);
        gabinete = gabineteResponse?.data?.NombreGabinete;
        if (!gabineteResponse.success) {
          const message =
            (gabineteResponse.errors as any)?.[0]?.Message ??
            (gabineteResponse.errors as any)?.[0]?.errorMessage ??
            gabineteResponse.message ??
            "No fue posible resolver el gabinete del radicado.";
          return { ok: false, message };
        }
      }

      gabineteRef.current = gabinete;
      const response = await queryListaDocumentosRadicados(buildInitialQuery({ nombreGabinete: gabinete }));
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
  }, [idTareaWf]);

  const loadChildren = useCallback(async (row: AppTreeTableRow): Promise<AppTreeTableLoadChildrenResult> => {
    const parentNodeType = String(row.meta?.NodeType ?? row.meta?.nodeType ?? "");
    const gabinete = gabineteRef.current;
    const request: ListaDocumentosRadicadosQueryRequest = {
      ...buildInitialQuery({ nombreGabinete: gabinete }),
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
  }, []);

  const onSelectRow = useCallback(async (rowId: string) => {
    const selected = latestRowRef.current.get(rowId);
    const meta = selected?.Meta;
    const values = selected?.Values;

    const nodeType = readString(meta, "NodeType", "nodeType") ?? "documento";
    const documentIdFromMeta = readNumber(meta, "DocumentId", "documentId");
    const gabinete =
      readString(meta, "NombreGabinete", "nombreGabinete", "NOMBRE_GABINETE") ??
      readString(values, "NOMBRE_GABINETE", "NombreGabinete", "NOMBREGABINETE");

    if (!gabinete) {
      // Evita disparar action inválida y deja un error funcional visible en consola.
      // La UI consume este error mediante el wrapper `success=false` del servicio.
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
        NombreGabinete: gabinete,
      },
    });

    const resolveRequest = actionResponse.data?.DocumentResolveRequest;
    if (actionResponse.success && resolveRequest) {
      await resolveDocumentoVisualizacion(resolveRequest);
    }
  }, []);

  const columns = useMemo(() => {
    // La regla del contrato: si no hay config en response, el orden se infiere determinísticamente
    // desde la primera fila. Esta implementación expone columnas vía `load()`; el consumidor puede
    // pasar columns explícitas a AppTreeTable si necesita estabilidad previa al render.
    // Como no tenemos `load` sync, aquí devolvemos un valor fijo vacío y dejamos al componente inferir.
    return [] as string[];
  }, []);

  return {
    columns,
    load,
    loadChildren,
    onSelectRow,
  };
};
