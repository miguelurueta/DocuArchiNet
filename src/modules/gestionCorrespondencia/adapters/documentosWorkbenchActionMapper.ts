import type { ListaDocumentosRadicadosActionRequest } from "../types/listaDocumentosRadicados.types";

export type DocumentosWorkbenchActionContext = {
  tableId: string;
  viewMode: "flatDocuments" | "hierarchical";
};

export const buildListaDocumentosRadicadosActionRequest = (input: {
  context: DocumentosWorkbenchActionContext;
  actionId: string;
  rowId: string;
  nodeType: string;
  documentId?: number;
  nombreGabinete?: string;
}): ListaDocumentosRadicadosActionRequest => ({
  TableId: input.context.tableId,
  ViewMode: input.context.viewMode,
  ActionId: input.actionId,
  RowId: input.rowId,
  ParentRowId: null,
  NodeType: input.nodeType,
  Payload: {
    IdDocumento: input.documentId,
    DocumentId: input.documentId,
    NombreGabinete: input.nombreGabinete,
  },
});

