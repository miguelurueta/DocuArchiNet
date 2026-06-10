import type {
  AppDocumentViewerOrchestratorInput,
  AppDocumentViewerRuntimeState,
  DocumentViewerKind,
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

export const getViewerKindFromContentType = (contentType: string | null, fileName?: string): DocumentViewerKind => {
  const normalized = (contentType || "").toLowerCase();
  if (isPdfFromContentType(contentType, fileName)) return "pdf";
  if (normalized.startsWith("image/")) return "image";
  return "unknown";
};

export const buildInitialRuntimeState = (
  input: AppDocumentViewerOrchestratorInput,
): AppDocumentViewerRuntimeState => ({
  attemptId: input.attemptId,
  documentKey: input.documentKey,
  documentId: input.documentId,
  nombreGabinete: input.nombreGabinete,
  fileUrl: null,
  contentType: null,
  viewerKind: "unknown",
  isPdf: false,
  isElectronicallySigned: null,
  firmaCheckStatus: "not_required",
  resolveStatus: "idle",
  errors: [],
});

export const buildResolvedRuntimeState = (params: {
  input: AppDocumentViewerOrchestratorInput;
  resolve: DocumentVisualizacionResolveResponseDto;
  fileUrlOverride?: string | null;
  resolveStatus: Extract<ResolveStatus, "resolved">;
  firmaCheckStatus: FirmaCheckStatus;
  isElectronicallySigned: boolean | null;
}): AppDocumentViewerRuntimeState => {
  const { input, resolve, fileUrlOverride, resolveStatus, firmaCheckStatus, isElectronicallySigned } = params;
  const fileUrl = (fileUrlOverride ?? null) || pickResolvedFileUrl(resolve);
  const contentType = resolve.ContentType ?? null;
  const isPdf = isPdfFromContentType(contentType, resolve.FileName);
  const viewerKind = getViewerKindFromContentType(contentType, resolve.FileName);
  return {
    attemptId: input.attemptId,
    documentKey: input.documentKey,
    documentId: input.documentId,
    nombreGabinete: input.nombreGabinete,
    fileUrl,
    contentType,
    viewerKind,
    isPdf,
    isElectronicallySigned,
    firmaCheckStatus,
    resolveStatus,
    errors: [],
  };
};

