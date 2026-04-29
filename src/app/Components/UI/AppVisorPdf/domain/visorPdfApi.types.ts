import type { ApiResponse } from "../../../../../api/ApiResponse";
import type { VisorPdfAnnotationsPayloadV1 } from "./annotations.types";

export type VisorPdfStampConfig = {
  enabled: boolean;
  opacity: number;
  scale: number;
  rotationDeg: number;
};

export interface AppVisorPdfApi {
  getPdfUrl: (
    documentId: string,
  ) => Promise<ApiResponse<{ url: string; expiresAtIso?: string }>>;

  getAnnotations: (
    documentId: string,
  ) => Promise<ApiResponse<VisorPdfAnnotationsPayloadV1>>;

  saveAnnotations: (
    documentId: string,
    payload: VisorPdfAnnotationsPayloadV1,
  ) => Promise<ApiResponse<{ savedAtIso: string }>>;

  getStampConfig: () => Promise<ApiResponse<VisorPdfStampConfig>>;

  saveStampConfig: (
    payload: VisorPdfStampConfig,
  ) => Promise<ApiResponse<{ savedAtIso: string }>>;
}
