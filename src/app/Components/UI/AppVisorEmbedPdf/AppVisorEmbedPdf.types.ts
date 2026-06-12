export type ViewerEffectivePermissions = {
  allowSignaturePlacement: boolean;
  allowSignatureDelete: boolean;
  allowSignatureLockToggle: boolean;
  allowAnnotationEdit: boolean;
  allowExport: boolean;
  allowPrint: boolean;
};

export type AppVisorLoadInput = {
  /**
   * Identidad del intento (latest-wins).
   * Opcional por compatibilidad; cuando existe debe propagarse end-to-end.
   */
  attemptId?: number;
  /**
   * Alternativa estable para correlación (latest-wins).
   */
  documentKey?: string;
  url: string;
  isElectronicallySigned: boolean;
  idImagen: number;
  nombreGabinete: string;
  idTareaWorkflow: number;
  radicado: string;
  nombre_modulo: string;
  metadata?: Record<string, unknown>;
};

export type AppVisorLoadResult = {
  ok: boolean;
  attemptId?: number;
  documentKey?: string;
  fileUrl: string | null;
  /**
   * Estado de carga del visor (para cancelación/stale/handshake).
   * `cancelled` NO debe tratarse como error visible.
   */
  loadStatus?: "loaded" | "failed" | "cancelled";
  permissionsRaw: Record<string, boolean>;
  permissionsEffective: ViewerEffectivePermissions;
  isElectronicallySigned: boolean;
  permissionStatus: "resolved" | "failed" | "not_required";
  errors: string[];
};

export type AppVisorAnnotatedPdfPage = {
  pageNumber: number;
  fileName: string;
  blob: Blob;
  sizeBytes: number;
  hashSha256?: string;
  sourcePageWidth?: number;
  sourcePageHeight?: number;
  sourcePageRotation?: number;
  sourcePageFingerprintSha256?: string;
};

export type AppVisorExportAnnotatedPdfPagesOptions = {
  calculateHashSha256?: boolean;
  signal?: AbortSignal;
};

export type AppVisorExportAnnotatedPdfPagesResult = {
  hasAnnotations: boolean;
  annotatedPages: number[];
  pageNumbers: number[];
  pages: AppVisorAnnotatedPdfPage[];
};

export type AppVisorEmbedPdfRef = {
  load(input: AppVisorLoadInput): Promise<AppVisorLoadResult>;
  reset(): void;
  cancelCurrentLoad(): void;
  getOriginalPdfPassword(): string | undefined;
  markAnnotatedPagesPersisted(): Promise<void>;
  exportAnnotatedPdfPages(
    options?: AppVisorExportAnnotatedPdfPagesOptions,
  ): Promise<AppVisorExportAnnotatedPdfPagesResult>;
};
