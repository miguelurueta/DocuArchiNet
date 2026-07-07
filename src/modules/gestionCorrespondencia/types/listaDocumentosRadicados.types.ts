export type ListaDocumentosRadicadosViewMode = "hierarchical" | "flatDocuments";

export type DocumentRelationScope =
  | "documentsOnly"
  | "includeResponseAttachments"
  | "responseAttachmentsOnly";

export type ApiErrorDto = {
  errorCode?: string;
  errorMessage?: string;
};

export type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T | null;
  meta?: Record<string, unknown> | null;
  errors?: ApiErrorDto[] | null;
};

export type ListaDocumentosRadicadosQueryRequest = {
  ViewMode: ListaDocumentosRadicadosViewMode;
  Page: number;
  PageSize: number;
  SortDir: "ASC" | "DESC";

  ColumnMode?: number;
  EstadoTramite?: string;
  SearchType?: number;
  Search?: string;
  SortField?: string;
  StructuredFilters?: Array<{
    Field: string;
    Operator: string;
    Value?: string;
    ValueFrom?: string;
    ValueTo?: string;
  }>;
  IncludeConfig?: boolean;
  EnablePagination?: boolean | null;
  EnableColumnFilters?: boolean | null;
  ParentRowId?: string | null;
  ParentNodeType?: string | null;
  Level?: number;
  DocumentRelationScope?: DocumentRelationScope;

  // Extensiones adicionales usadas por el backend (fuera del contrato mínimo SCRUM-205).
  TableId?: string;
  NombreGabinete?: string;
  CampoRadicado?: string;
  Radicado?: string;
  AplicaTrd?: number;
};

export type ListaDocumentosRadicadosRowMeta = {
  NodeType?: string;
  ParentId?: string | null;
  HasChildren?: boolean;
  CanAddChild?: boolean;
  CanDelete?: boolean;
  DocumentId?: number;
  NombreGabinete?: string;
};

export type ListaDocumentosRadicadosRowDto = {
  RowId: string;
  Values: Record<string, string | number | boolean | null>;
  Meta?: ListaDocumentosRadicadosRowMeta;
};

export type ListaDocumentosRadicadosPagination = {
  page?: number;
  pageSize?: number;
  total?: number;
};

export type ListaDocumentosRadicadosQueryData = {
  Rows: ListaDocumentosRadicadosRowDto[];
  // Nota: el contrato menciona IncludeConfig, pero no define shape en el ejemplo.
  // Se mantiene como unknown para compatibilidad futura.
  Config?: unknown;
  Columns?: unknown;
  pagination?: ListaDocumentosRadicadosPagination | null;
  Pagination?: ListaDocumentosRadicadosPagination | null;
};

export type ListaDocumentosRadicadosActionRequest = {
  TableId: string;
  ViewMode: ListaDocumentosRadicadosViewMode;
  ActionId: "ver_documento" | "agregar_item" | "eliminar_item" | string;
  RowId: string;
  ParentRowId?: string | null;
  NodeType: string;
  Payload: {
    IdDocumento?: number;
    DocumentId?: number;
    NombreGabinete?: string;
    [key: string]: unknown;
  };
};

export type DocumentResolveRequestDto = {
  NombreGabinete: string;
  IdDocumento: number;
};

export type ListaDocumentosRadicadosActionData = {
  Operation?: string;
  AffectedRowId?: string;
  ParentRowId?: string | null;
  RequiresReloadNode?: boolean;
  Row?: unknown;
  DocumentResolveRequest?: DocumentResolveRequestDto;
};
