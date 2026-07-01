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

export type BackendStorageUploadInitRequest = {
  NombreOriginal: string;
  TamanoBytes: number;
  Extension: string;
  HashSha256Esperado?: string | null;
  NumeroChunks: number;
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
  chunksRecibidos?: number[] | number | null;
  chunksPendientes?: number[] | number | null;
  totalChunks?: number | null;
  tamanoRecibidoBytes?: number | null;
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
  idDocumento?: number | string | null;
  archivoTemporalId: string;
  nombreOriginal: string;
  extension: string;
  numeroPaginas?: number | null;
};

export type BackendDocumentoEntrada = {
  IdDocumento?: number | string | null;
  ArchivoTemporalId: string;
  NombreOriginal: string;
  Extension: string;
  NumeroPaginas?: number | null;
};

export type CampoIndexacionStorage = {
  nombreCampo: string;
  valor?: string | null;
  esObligatorio?: boolean | null;
};

export type BackendCampoIndexacionStorage = {
  NombreCampo: string;
  Valor?: string | null;
  EsObligatorio?: boolean | null;
};

export type TrdStorage = {
  idTipoDocumento?: number | null;
  nombreTipoDocumento?: string | null;
};

export type BackendTrdStorage = {
  IdTipoDocumento?: number | null;
  NombreTipoDocumento?: string | null;
};

export type ExpedienteStorage = {
  idExpediente?: number | null;
  idTipoExpediente?: number | null;
};

export type BackendExpedienteStorage = {
  IdExpediente?: number | null;
  IdTipoExpediente?: number | null;
};

export type WorkflowStorage = {
  idTareaWorkflow?: number | null;
  idRutaWorkflow?: number | null;
};

export type BackendWorkflowStorage = {
  IdTareaWorkflow?: number | null;
  IdRutaWorkflow?: number | null;
};

export type AnexoRespuestaStorage = {
  idRespuestaRadicado: number;
  nombreArchivo: string;
  tipoAdjunto: "respuesta" | string;
  observacion?: string | null;
};

export type BackendAnexoRespuestaStorage = {
  IdRespuestaRadicado: number;
  NombreArchivo: string;
  TipoAdjunto: "respuesta" | string;
  Observacion?: string | null;
};

export type CabinetIndexSeedStorage = {
  sourceModule: "RADICACION" | string;
  providerKey: "RADICACION" | string;
  version: string;
  payload: {
    modoResolucion: "RespuestaRadicado" | string;
    proveedorExterno?: string | null;
    radicadoExterno?: string | null;
    matriculaSII?: string | null;
  };
};

export type BackendCabinetIndexSeedStorage = {
  SourceModule: "RADICACION" | string;
  ProviderKey: "RADICACION" | string;
  Version: string;
  Payload: {
    ModoResolucion: "RespuestaRadicado" | string;
    ProveedorExterno?: string | null;
    RadicadoExterno?: string | null;
    MatriculaSII?: string | null;
  };
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
  cabinetIndexSeed?: CabinetIndexSeedStorage | null;
  anexoRespuesta?: AnexoRespuestaStorage | null;
  fullText?: string | null;
  numeroPaginasDeclaradas?: number | null;
};

export type BackendAlmacenarDocumentoRequest = {
  NombreGabinete: string;
  RutaTemporalId: string;
  NombreDocumento: string;
  RequestId: string;
  Documentos: BackendDocumentoEntrada[];
  CamposIndexacion?: BackendCampoIndexacionStorage[] | null;
  Inventario?: unknown;
  Trd?: BackendTrdStorage | null;
  Expediente?: BackendExpedienteStorage | null;
  Workflow?: BackendWorkflowStorage | null;
  CabinetIndexSeed?: BackendCabinetIndexSeedStorage | null;
  AnexoRespuesta?: BackendAnexoRespuestaStorage | null;
  FullText?: string | null;
  NumeroPaginasDeclaradas?: number | null;
};

export type AlmacenarDocumentoResponse = {
  idAlmacen: number;
  idRegistroProduccionDocumental: number;
  nombreArchivoFinal: string;
  requestId: string;
};

export type WorkflowAnexoStorageResult = {
  documento: {
    idAlmacen: number;
    idRegistroProduccionDocumental: number;
    nombreArchivoFinal: string;
  };
  anexoRespuesta: {
    idAnexoRespuesta?: number | null;
    idRespuestaRadicado: number;
    idAlmacen: number;
    nombreGabinete: string;
    nombreArchivo: string;
    created: boolean;
  };
  indice?: {
    providerKey?: string | null;
    resolved?: boolean | null;
    sourceTrace?: string | null;
  } | null;
  workflow?: {
    logInserted?: boolean | null;
    idTareaWorkflow?: number | null;
    idRutaWorkflow?: number | null;
  } | null;
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
  backendPayloadCase?: "camel" | "pascal";
  validateStatusBeforeComplete?: boolean;
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
