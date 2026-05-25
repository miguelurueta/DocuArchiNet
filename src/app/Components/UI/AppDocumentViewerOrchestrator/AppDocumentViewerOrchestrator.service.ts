import axios from "axios";
import clienteApi from "../../../../api/Clienteaxios";
import type {
  DocumentFirmaElectronicaResponseDto,
  DocumentVisualizacionResolveRequestDto,
  DocumentVisualizacionResolveResponseDto,
} from "./AppDocumentViewerOrchestrator.types";

export const DOCUMENTOS_VISUALIZACION_RESOLVE_ENDPOINT =
  "/api/gestor-documental/documentos/visualizacion/resolve";

export const buildFirmaElectronicaEndpoint = (params: {
  idArchivo: number;
  nombreGabinete: string;
}) => {
  const { idArchivo, nombreGabinete } = params;
  const encodedGabinete = encodeURIComponent(nombreGabinete);
  return `/api/gestor-documental/documentos/${idArchivo}/firma-electronica?nombreGabinete=${encodedGabinete}`;
};

export type ApiResponse<T> = { success?: boolean; message?: string; data?: T } | T;

const unwrapApiData = <T>(payload: ApiResponse<T>): T => {
  if (payload && typeof payload === "object" && "data" in payload) {
    return (payload as { data: T }).data;
  }
  return payload as T;
};

export async function resolveVisualizacionDocumento(params: {
  request: DocumentVisualizacionResolveRequestDto;
  signal?: AbortSignal;
}): Promise<DocumentVisualizacionResolveResponseDto> {
  const { request, signal } = params;
  try {
    const response = await clienteApi.post<ApiResponse<DocumentVisualizacionResolveResponseDto>>(
      DOCUMENTOS_VISUALIZACION_RESOLVE_ENDPOINT,
      request,
      signal ? { signal } : undefined,
    );
    const data = unwrapApiData(response.data);
    if (!data || typeof data !== "object") {
      throw new Error("INVALID_RESPONSE");
    }
    return data as DocumentVisualizacionResolveResponseDto;
  } catch (error) {
    if (axios.isAxiosError(error) && error.code === "ERR_CANCELED") {
      throw new DOMException("Request cancelled", "AbortError");
    }
    throw error;
  }
}

export async function fetchFirmaElectronica(params: {
  idArchivo: number;
  nombreGabinete: string;
  signal?: AbortSignal;
}): Promise<DocumentFirmaElectronicaResponseDto> {
  const { idArchivo, nombreGabinete, signal } = params;
  const url = buildFirmaElectronicaEndpoint({ idArchivo, nombreGabinete });
  try {
    const response = await clienteApi.get<ApiResponse<DocumentFirmaElectronicaResponseDto>>(
      url,
      signal ? { signal } : undefined,
    );
    const data = unwrapApiData(response.data);
    if (!data || typeof data !== "object") {
      throw new Error("INVALID_RESPONSE");
    }
    return data as DocumentFirmaElectronicaResponseDto;
  } catch (error) {
    if (axios.isAxiosError(error) && error.code === "ERR_CANCELED") {
      throw new DOMException("Request cancelled", "AbortError");
    }
    throw error;
  }
}
