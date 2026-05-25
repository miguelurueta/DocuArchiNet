import type {
  AppDocumentViewerOrchestratorInput,
  AppDocumentViewerRuntimeState,
  DocumentVisualizacionResolveResponseDto,
  FirmaCheckStatus,
  ResolveStatus,
} from "./AppDocumentViewerOrchestrator.types";

export const pickResolvedFileUrl = (dto: Pick<
  DocumentVisualizacionResolveResponseDto,
  "UrlTemporalAbsoluta" | "UrlTemporal"
>): string | null => {
  const absolute = dto.UrlTemporalAbsoluta?.trim();
  if (absolute) return absolute;
  const relative = dto.UrlTemporal?.trim();
  return relative || null;
};

export const isPdfFromContentType = (contentType: string | null, fileName?: string): boolean => {
  const normalized = (contentType || "").toLowerCase();
  if (normalized.includes("application/pdf")) return true;
  if (normalized === "pdf") return true;
  const safeName = (fileName || "").toLowerCase();
  return safeName.endsWith(".pdf");
};

export const buildInitialRuntimeState = (
  input: AppDocumentViewerOrchestratorInput,
): AppDocumentViewerRuntimeState => ({
  documentId: input.documentId,
  nombreGabinete: input.nombreGabinete,
  fileUrl: null,
  contentType: null,
  isPdf: false,
  isElectronicallySigned: null,
  firmaCheckStatus: "not_required",
  resolveStatus: "idle",
  errors: [],
});

export const buildResolvedRuntimeState = (params: {
  input: AppDocumentViewerOrchestratorInput;
  resolve: DocumentVisualizacionResolveResponseDto;
  resolveStatus: Extract<ResolveStatus, "resolved">;
  firmaCheckStatus: FirmaCheckStatus;
  isElectronicallySigned: boolean | null;
}): AppDocumentViewerRuntimeState => {
  const { input, resolve, resolveStatus, firmaCheckStatus, isElectronicallySigned } = params;
  const fileUrl = pickResolvedFileUrl(resolve);
  const contentType = resolve.ContentType ?? null;
  const isPdf = isPdfFromContentType(contentType, resolve.FileName);
  return {
    documentId: input.documentId,
    nombreGabinete: input.nombreGabinete,
    fileUrl,
    contentType,
    isPdf,
    isElectronicallySigned,
    firmaCheckStatus,
    resolveStatus,
    errors: [],
  };
};

