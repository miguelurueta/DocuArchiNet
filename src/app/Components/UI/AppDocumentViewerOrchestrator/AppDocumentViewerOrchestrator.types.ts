export type AppDocumentViewerOrchestratorInput = {
  documentId: number;
  nombreGabinete: string;
  context?: {
    idTareaWorkflow?: number;
    radicado?: string;
    grafo?: object;
  };
};

export type ResolveStatus = "idle" | "loading" | "resolved" | "failed" | "cancelled";

export type FirmaCheckStatus = "not_required" | "resolved" | "failed";

export type AppDocumentViewerRuntimeState = {
  documentId: number;
  nombreGabinete: string;
  fileUrl: string | null;
  contentType: string | null;
  isPdf: boolean;
  isElectronicallySigned: boolean | null;
  firmaCheckStatus: FirmaCheckStatus;
  resolveStatus: ResolveStatus;
  errors: string[];
};

export type DocumentVisualizacionResolveRequestDto = {
  NombreGabinete: string;
  IdDocumento: number;
};

export type DocumentVisualizacionResolveResponseDto = {
  IdDocumento: number;
  NombreGabinete: string;
  FileName: string;
  ContentType: string;
  Origen: "ORIGINAL" | "TIF_TO_PDF";
  UrlTemporal: string;
  UrlTemporalAbsoluta: string | null;
  ExpiresAt: string;
};

export type DocumentFirmaElectronicaResponseDto = {
  IdArchivo: number;
  NombreGabinete: string;
  FirmadoElectronico: boolean;
  IdCertificado: number;
};

export type DocumentViewerOrchestratorErrorCode =
  | "RESOLVE_FAILED"
  | "FIRMA_FAILED"
  | "CANCELLED"
  | "INVALID_RESPONSE";

