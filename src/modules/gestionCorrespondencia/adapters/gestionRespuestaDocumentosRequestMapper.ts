import type { ListaDocumentosRadicadosQueryRequest } from "../types/listaDocumentosRadicados.types";

const TABLE_ID = "InboxListaDocumentosRadicado";
const DEFAULT_APLICA_TRD = 0;
const DEFAULT_COLUMN_MODE = 2;
const DEFAULT_SEARCH_TYPE = 1;
const DEFAULT_SORT_FIELD = "ID";

export type GestionRespuestaDocumentosQueryContext = {
  idTareaWf?: number;
  nombreGabinete?: string;
};

export const buildListaDocumentosRadicadosRootQuery = (
  context: GestionRespuestaDocumentosQueryContext,
): ListaDocumentosRadicadosQueryRequest => {
  const nombreGabinete = context.nombreGabinete?.trim();

  return {
    ViewMode: "flatDocuments",
    Page: 1,
    PageSize: 25,
    SortDir: "ASC",
    ColumnMode: DEFAULT_COLUMN_MODE,
    SearchType: DEFAULT_SEARCH_TYPE,
    Search: "",
    SortField: DEFAULT_SORT_FIELD,
    StructuredFilters: [],
    IncludeConfig: true,
    EnablePagination: false,
    EnableColumnFilters: false,
    ParentRowId: null,
    ParentNodeType: null,
    Level: 1,

    TableId: TABLE_ID,
    EstadoTramite: "",
    CampoRadicado: "",
    Radicado: "",
    AplicaTrd: DEFAULT_APLICA_TRD,
    ...(nombreGabinete ? { NombreGabinete: nombreGabinete } : {}),
  };
};

export const buildListaDocumentosRadicadosChildrenQuery = (input: {
  nombreGabinete?: string;
  parentRowId: string;
  parentNodeType?: string | null;
  level: number;
}): ListaDocumentosRadicadosQueryRequest => {
  const nombreGabinete = input.nombreGabinete?.trim();

  return {
    ViewMode: "hierarchical",
    Page: 1,
    PageSize: 25,
    SortDir: "ASC",
    Search: "",
    StructuredFilters: [],
    IncludeConfig: false,
    EnablePagination: false,
    EnableColumnFilters: false,
    ParentRowId: input.parentRowId,
    ParentNodeType: input.parentNodeType ?? null,
    Level: input.level,

    TableId: TABLE_ID,
    CampoRadicado: "",
    Radicado: "",
    AplicaTrd: DEFAULT_APLICA_TRD,
    ...(nombreGabinete ? { NombreGabinete: nombreGabinete } : {}),
  };
};
