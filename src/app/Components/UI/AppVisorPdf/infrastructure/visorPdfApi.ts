import clienteApi from "../../../../../api/Clienteaxios";
import type { ApiResponse } from "../../../../../api/ApiResponse";
import type {
  AppVisorPdfApi,
  VisorPdfStampConfig,
} from "../domain/visorPdfApi.types";
import type { VisorPdfAnnotationsPayloadV1 } from "../domain/annotations.types";

type CreateVisorPdfApiOptions = {
  /**
   * Prefijo para endpoints del backend.
   * Nota: este ticket NO define rutas reales; se mantiene como scaffold configurable.
   */
  basePath?: string;
};

function joinPath(basePath: string, path: string) {
  const normalizedBase = basePath.replace(/\/+$/, "");
  const normalizedPath = path.replace(/^\/+/, "");
  return `${normalizedBase}/${normalizedPath}`;
}

export function createAppVisorPdfApi(
  options: CreateVisorPdfApiOptions = {},
): AppVisorPdfApi {
  const basePath = options.basePath ?? "/visor-pdf";

  return {
    getPdfUrl: async (
      documentId: string,
    ): Promise<ApiResponse<{ url: string; expiresAtIso?: string }>> => {
      const response = await clienteApi.get<ApiResponse<{ url: string; expiresAtIso?: string }>>(
        joinPath(basePath, `documents/${encodeURIComponent(documentId)}/pdf-url`),
      );
      return response.data;
    },

    getAnnotations: async (
      documentId: string,
    ): Promise<ApiResponse<VisorPdfAnnotationsPayloadV1>> => {
      const response = await clienteApi.get<ApiResponse<VisorPdfAnnotationsPayloadV1>>(
        joinPath(
          basePath,
          `documents/${encodeURIComponent(documentId)}/annotations`,
        ),
      );
      return response.data;
    },

    saveAnnotations: async (
      documentId: string,
      payload: VisorPdfAnnotationsPayloadV1,
    ): Promise<ApiResponse<{ savedAtIso: string }>> => {
      const response = await clienteApi.put<ApiResponse<{ savedAtIso: string }>>(
        joinPath(
          basePath,
          `documents/${encodeURIComponent(documentId)}/annotations`,
        ),
        payload,
      );
      return response.data;
    },

    getStampConfig: async (): Promise<ApiResponse<VisorPdfStampConfig>> => {
      const response = await clienteApi.get<ApiResponse<VisorPdfStampConfig>>(
        joinPath(basePath, "stamp-config"),
      );
      return response.data;
    },

    saveStampConfig: async (
      payload: VisorPdfStampConfig,
    ): Promise<ApiResponse<{ savedAtIso: string }>> => {
      const response = await clienteApi.put<ApiResponse<{ savedAtIso: string }>>(
        joinPath(basePath, "stamp-config"),
        payload,
      );
      return response.data;
    },
  };
}
