import clienteApi from "../../../api/Clienteaxios";
import type {
  ApiResponse,
  DocumentResolveRequestDto,
  ListaDocumentosRadicadosActionData,
  ListaDocumentosRadicadosActionRequest,
  ListaDocumentosRadicadosQueryData,
  ListaDocumentosRadicadosQueryRequest,
} from "../types/listaDocumentosRadicados.types";

export const LISTA_DOCUMENTOS_RADICADOS_QUERY_ENDPOINT =
  "/api/GestorDocumental/Documentos/ListaDocumentosRadicados/query";

export const LISTA_DOCUMENTOS_RADICADOS_ACTION_ENDPOINT =
  "/api/GestorDocumental/Documentos/ListaDocumentosRadicados/action";

export const DOCUMENTOS_VISUALIZACION_RESOLVE_ENDPOINT =
  "/api/gestor-documental/documentos/visualizacion/resolve";

export async function queryListaDocumentosRadicados(
  request: ListaDocumentosRadicadosQueryRequest,
): Promise<ApiResponse<ListaDocumentosRadicadosQueryData>> {
  const response = await clienteApi.post<ApiResponse<ListaDocumentosRadicadosQueryData>>(
    LISTA_DOCUMENTOS_RADICADOS_QUERY_ENDPOINT,
    request,
  );
  return response.data;
}

export async function actionListaDocumentosRadicados(
  request: ListaDocumentosRadicadosActionRequest,
): Promise<ApiResponse<ListaDocumentosRadicadosActionData>> {
  const response = await clienteApi.post<ApiResponse<ListaDocumentosRadicadosActionData>>(
    LISTA_DOCUMENTOS_RADICADOS_ACTION_ENDPOINT,
    request,
  );
  return response.data;
}

export async function resolveDocumentoVisualizacion(
  request: DocumentResolveRequestDto,
): Promise<ApiResponse<unknown>> {
  const response = await clienteApi.post<ApiResponse<unknown>>(
    DOCUMENTOS_VISUALIZACION_RESOLVE_ENDPOINT,
    request,
  );
  return response.data;
}
