import type {
  DocumentRelationScope,
  ListaDocumentosRadicadosQueryRequest,
} from "../types/listaDocumentosRadicados.types";

const TABLE_ID = "InboxListaDocumentosRadicado";
const DEFAULT_APLICA_TRD = 0;
const DEFAULT_COLUMN_MODE = 2;
const DEFAULT_SEARCH_TYPE = 1;
const DEFAULT_SORT_FIELD = "ID";
const DEFAULT_CAMPO_RADICADO = "ENLASE";
const DEFAULT_DOCUMENT_RELATION_SCOPE: DocumentRelationScope = "documentsOnly";

export type GestionRespuestaDocumentosQueryContext = {
  idTareaWf?: number;
  nombreGabinete?: string;
  radicado?: string;
  documentRelationScope?: DocumentRelationScope;
  enablePagination?: boolean | null;
  page?: number;
  pageSize?: number;
  search?: string;
  searchType?: number;
};

export const buildListaDocumentosRadicadosRootQuery = (
  context: GestionRespuestaDocumentosQueryContext,
): ListaDocumentosRadicadosQueryRequest => {
  const nombreGabinete = context.nombreGabinete?.trim();
  const radicado = context.radicado?.trim();

  return {
    ViewMode: "flatDocuments",
    Page: context.page ?? 1,
    PageSize: context.pageSize ?? 25,
    SortDir: "ASC",
    ColumnMode: DEFAULT_COLUMN_MODE,
    SearchType: context.searchType ?? DEFAULT_SEARCH_TYPE,
    Search: context.search?.trim() ?? "",
    SortField: DEFAULT_SORT_FIELD,
    StructuredFilters: [],
    IncludeConfig: true,
    EnablePagination: context.enablePagination ?? true,
    EnableColumnFilters: false,
    ParentRowId: null,
    ParentNodeType: null,
    Level: 1,
    DocumentRelationScope: context.documentRelationScope ?? DEFAULT_DOCUMENT_RELATION_SCOPE,

    TableId: TABLE_ID,
    EstadoTramite: "",
    CampoRadicado: DEFAULT_CAMPO_RADICADO,
    Radicado: radicado ?? "",
    AplicaTrd: DEFAULT_APLICA_TRD,
    ...(nombreGabinete ? { NombreGabinete: nombreGabinete } : {}),
  };
};

export const buildListaDocumentosRadicadosChildrenQuery = (input: {
  nombreGabinete?: string;
  radicado?: string;
  parentRowId: string;
  parentNodeType?: string | null;
  level: number;
  documentRelationScope?: DocumentRelationScope;
  enablePagination?: boolean | null;
  page?: number;
  pageSize?: number;
  search?: string;
  searchType?: number;
}): ListaDocumentosRadicadosQueryRequest => {
  const nombreGabinete = input.nombreGabinete?.trim();
  const radicado = input.radicado?.trim();

  return {
    ViewMode: "hierarchical",
    Page: input.page ?? 1,
    PageSize: input.pageSize ?? 25,
    SortDir: "ASC",
    Search: input.search?.trim() ?? "",
    SearchType: input.searchType ?? DEFAULT_SEARCH_TYPE,
    StructuredFilters: [],
    IncludeConfig: false,
    EnablePagination: input.enablePagination ?? true,
    EnableColumnFilters: false,
    ParentRowId: input.parentRowId,
    ParentNodeType: input.parentNodeType ?? null,
    Level: input.level,
    DocumentRelationScope: input.documentRelationScope ?? DEFAULT_DOCUMENT_RELATION_SCOPE,

    TableId: TABLE_ID,
    CampoRadicado: DEFAULT_CAMPO_RADICADO,
    Radicado: radicado ?? "",
    AplicaTrd: DEFAULT_APLICA_TRD,
    ...(nombreGabinete ? { NombreGabinete: nombreGabinete } : {}),
  };
};
