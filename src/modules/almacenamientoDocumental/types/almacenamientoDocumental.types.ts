export type AppResponse<T> = {
  success?: boolean;
  Success?: boolean;
  data?: T;
  Data?: T;
  message?: string | null;
  Message?: string | null;
  errors?: unknown;
  Errors?: unknown;
  meta?: unknown;
  Meta?: unknown;
};

export type StorageUploadInitRequest = {
  nombreOriginal: string;
  tamanoBytes: number;
  extension: string;
  hashSha256Esperado?: string | null;
  numeroChunks: number;
};

export type StorageUploadInitResponse = {
  rutaTemporalId: string;
  archivoTemporalId: string;
  chunkSizeBytes: number;
  estado: string;
};

export type StorageUploadStatusResponse = {
  rutaTemporalId: string;
  archivoTemporalId: string;
  estado: string;
  chunksRecibidos?: number | null;
  totalChunks?: number | null;
  completado?: boolean | null;
};

export type StorageUploadCompleteResponse = {
  rutaTemporalId: string;
  archivoTemporalId: string;
  estado: string;
  completado?: boolean | null;
};

export type StorageUploadCancelResponse = {
  rutaTemporalId?: string | null;
  archivoTemporalId?: string | null;
  cancelado?: boolean | null;
  estado?: string | null;
};

export type DocumentoEntrada = {
  idDocumento?: number | null;
  archivoTemporalId: string;
  nombreOriginal: string;
  extension: string;
  numeroPaginas?: number | null;
};

export type CampoIndexacionStorage = {
  nombreCampo: string;
  valor?: string | null;
  esObligatorio?: boolean | null;
};

export type TrdStorage = {
  idTipoDocumento?: number | null;
  nombreTipoDocumento?: string | null;
};

export type ExpedienteStorage = {
  idExpediente?: number | null;
  idTipoExpediente?: number | null;
};

export type WorkflowStorage = {
  idTareaWorkflow?: number | null;
  idRutaWorkflow?: number | null;
};

export type AlmacenarDocumentoRequest = {
  nombreGabinete: string;
  rutaTemporalId: string;
  nombreDocumento: string;
  requestId: string;
  documentos: DocumentoEntrada[];
  camposIndexacion?: CampoIndexacionStorage[] | null;
  inventario?: unknown;
  trd?: TrdStorage | null;
  expediente?: ExpedienteStorage | null;
  workflow?: WorkflowStorage | null;
  fullText?: string | null;
  numeroPaginasDeclaradas?: number | null;
};

export type AlmacenarDocumentoResponse = {
  idAlmacen: number;
  idRegistroProduccionDocumental: number;
  nombreArchivoFinal: string;
  requestId: string;
};

export type UploadStoragePhase = "initializing" | "uploading" | "completing" | "storing";

export type UploadStorageProgress = {
  fileUid: string;
  phase: UploadStoragePhase;
  chunkIndex?: number;
  totalChunks?: number;
  loadedBytes?: number;
  totalBytes?: number;
  percent: number;
};

export type UploadOneDocumentInput = {
  fileUid: string;
  file: File;
  request: Omit<AlmacenarDocumentoRequest, "rutaTemporalId" | "documentos"> & {
    documento?: Partial<DocumentoEntrada>;
  };
  initialChunkSizeBytes?: number;
  signal?: AbortSignal;
  onProgress?: (progress: UploadStorageProgress) => void;
};

export type UploadOneDocumentResult = {
  temporal: StorageUploadInitResponse;
  response: AlmacenarDocumentoResponse;
  rawBackendResult?: unknown;
};

export type AlmacenamientoDocumentalUploadErrorCode =
  | "storage_contract_error"
  | "storage_init_error"
  | "storage_chunk_error"
  | "storage_status_error"
  | "storage_complete_error"
  | "storage_cancel_error"
  | "storage_store_error"
  | "storage_aborted";

export type AlmacenamientoDocumentalUploadErrorParams = {
  code: AlmacenamientoDocumentalUploadErrorCode;
  phase?: UploadStoragePhase | "status" | "cancel";
  message: string;
  requestId?: string;
  details?: unknown;
  cause?: unknown;
};

export class AlmacenamientoDocumentalUploadError extends Error {
  public readonly code: AlmacenamientoDocumentalUploadErrorCode;
  public readonly phase?: UploadStoragePhase | "status" | "cancel";
  public readonly requestId?: string;
  public readonly details?: unknown;
  public readonly cause?: unknown;

  public constructor(params: AlmacenamientoDocumentalUploadErrorParams) {
    super(params.message);
    this.name = "AlmacenamientoDocumentalUploadError";
    this.code = params.code;
    this.phase = params.phase;
    this.requestId = params.requestId;
    this.details = params.details;
    this.cause = params.cause;
  }
}
