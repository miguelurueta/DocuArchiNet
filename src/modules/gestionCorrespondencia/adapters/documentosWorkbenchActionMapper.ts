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
  idDocumento?: number;
  documentId?: number;
  nombreGabinete?: string;
}): ListaDocumentosRadicadosActionRequest => {
  const payload: ListaDocumentosRadicadosActionRequest["Payload"] = {};

  if (typeof input.idDocumento === "number" && Number.isFinite(input.idDocumento)) {
    payload.IdDocumento = input.idDocumento;
  } else if (typeof input.documentId === "number" && Number.isFinite(input.documentId)) {
    payload.DocumentId = input.documentId;
  }

  if (typeof input.nombreGabinete === "string" && input.nombreGabinete.trim().length > 0) {
    payload.NombreGabinete = input.nombreGabinete;
  }

  return {
    TableId: input.context.tableId,
    ViewMode: input.context.viewMode,
    ActionId: input.actionId,
    RowId: input.rowId,
    ParentRowId: null,
    NodeType: input.nodeType,
    Payload: payload,
  };
};

