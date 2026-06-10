import clienteApi from "../../../api/Clienteaxios";
import axios from "axios";
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
  try {
    const response = await clienteApi.post<ApiResponse<ListaDocumentosRadicadosQueryData>>(
      LISTA_DOCUMENTOS_RADICADOS_QUERY_ENDPOINT,
      request,
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const data = error.response?.data as ApiResponse<ListaDocumentosRadicadosQueryData> | undefined;
      if (data && typeof data === "object" && typeof data.success === "boolean") return data;
    }
    return { success: false, message: "No fue posible cargar el listado.", data: null };
  }
}

export async function actionListaDocumentosRadicados(
  request: ListaDocumentosRadicadosActionRequest,
): Promise<ApiResponse<ListaDocumentosRadicadosActionData>> {
  try {
    const response = await clienteApi.post<ApiResponse<ListaDocumentosRadicadosActionData>>(
      LISTA_DOCUMENTOS_RADICADOS_ACTION_ENDPOINT,
      request,
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const data = error.response?.data as ApiResponse<ListaDocumentosRadicadosActionData> | undefined;
      if (data && typeof data === "object" && typeof data.success === "boolean") return data;
    }
    return { success: false, message: "No fue posible ejecutar la acción.", data: null };
  }
}

export async function resolveDocumentoVisualizacion(
  request: DocumentResolveRequestDto,
): Promise<ApiResponse<unknown>> {
  try {
    const response = await clienteApi.post<ApiResponse<unknown>>(
      DOCUMENTOS_VISUALIZACION_RESOLVE_ENDPOINT,
      request,
    );
    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const data = error.response?.data as ApiResponse<unknown> | undefined;
      if (data && typeof data === "object" && typeof data.success === "boolean") return data;
    }
    return { success: false, message: "No fue posible resolver el documento.", data: null };
  }
}
