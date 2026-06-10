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

function isAxiosCancelled(error: unknown): boolean {
  if (!error || typeof error !== "object") return false;
  const code = (error as { code?: unknown }).code;
  return code === "ERR_CANCELED";
}

function normalizeDownloadUrl(url: string): string {
  const trimmed = url.trim();
  if (!trimmed) return trimmed;
  // Si backend retorna UrlTemporalAbsoluta con host/puerto distinto, convertir a ruta relativa
  // para que `clienteApi` aplique baseURL, cookies e interceptores del proyecto.
  if (/^https?:\/\//i.test(trimmed)) {
    try {
      const u = new URL(trimmed);
      let path = `${u.pathname}${u.search}`;
      // Guardrail: evitar duplicar prefijos cuando `baseURL` ya contiene un path base
      // (ej. baseURL = http://localhost:5173/DocuArchiApi y pathname inicia con /DocuArchiApi/...).
      const base = String((clienteApi.defaults as { baseURL?: unknown })?.baseURL ?? "");
      if (base) {
        try {
          const origin =
            typeof window !== "undefined" && window.location?.origin ? window.location.origin : "http://localhost";
          const baseUrl = new URL(base, origin);
          const basePath = baseUrl.pathname.replace(/\/+$/, "");
          if (basePath && path.startsWith(`${basePath}/`)) {
            path = path.slice(basePath.length);
          }
        } catch {
          // ignore
        }
      }
      return path;
    } catch {
      return trimmed;
    }
  }
  return trimmed;
}

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
    if (isAxiosCancelled(error)) {
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

export async function downloadVisualizacionBlob(params: {
  fileUrl: string;
  signal?: AbortSignal;
}): Promise<Blob> {
  const { fileUrl, signal } = params;
  const url = normalizeDownloadUrl(fileUrl);
  try {
    const response = await clienteApi.get(url, {
      ...(signal ? { signal } : {}),
      responseType: "blob",
    });
    const blob = response.data as Blob;
    if (!(blob instanceof Blob)) {
      throw new Error("INVALID_BLOB_RESPONSE");
    }
    return blob;
  } catch (error) {
    if (isAxiosCancelled(error)) {
      throw new DOMException("Request cancelled", "AbortError");
    }
    throw error;
  }
}
