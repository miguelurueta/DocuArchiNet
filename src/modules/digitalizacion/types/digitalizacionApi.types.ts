import type { DigitalizacionContext, DigitalizacionTrdMetadata } from "./digitalizacion.types";

export type DigitalizacionApiErrorStatus = "validation" | "conflict" | "error" | "aborted" | "stale";

export type DigitalizacionApiError = {
  code: string;
  message: string;
  field?: string;
  status?: DigitalizacionApiErrorStatus;
};

export type DigitalizacionApiRequestOptions = {
  signal?: AbortSignal;
};

export type DigitalizacionApiResponseEnvelope<T> = {
  success?: boolean;
  Success?: boolean;
  message?: string;
  Message?: string;
  data?: T | null;
  Data?: T | null;
  meta?: {
    status?: string;
    Status?: string;
  };
  Meta?: {
    status?: string;
    Status?: string;
  };
  errors?: unknown[];
  Errors?: unknown[];
};

export type DigitalizacionConfiguracionQuery = {
  TipoDigitalizacion: string;
  IdTramite?: number;
  TipoTramite?: string;
  Radicado?: string;
  NombreGabinete: string;
  IdTareaWorkflow?: number;
  IdRutaWorkflow?: number;
};

export type DigitalizacionConfiguracionResponse = {
  idConfiguracionDigitalizacion: number;
  tipoDigitalizacion: string;
  nombreGabinete: string;
  activaListaChequeo: boolean;
  obligaListaChequeo: boolean;
  permiteCrearDocumento: boolean;
  permiteAdjuntarDocumento: boolean;
  requiereMetadata: boolean;
  formatosPermitidos: string[];
};

export type DigitalizacionListaChequeoQuery = {
  IdTramite?: number;
  TipoTramite?: string;
  IdConfiguracionDigitalizacion: number;
  NombreGabinete: string;
  Radicado?: string;
};

export type DigitalizacionListaChequeoItem = {
  idTipoListaChequeo: number;
  nombreTipoDocumento: string;
  idArea?: number;
  idSerie?: number;
  idSubSerie?: number;
  idTipoDocumento?: number;
  nombreArea?: string;
  nombreSerie?: string;
  nombreSubSerie?: string;
  esUnico: boolean;
  obligatorio: boolean;
  disponible: boolean;
  mensajeNoDisponible?: string;
};

export type DigitalizacionListaChequeoResponse = {
  idConfiguracionDigitalizacion: number;
  obligaListaChequeo: boolean;
  items: DigitalizacionListaChequeoItem[];
};

export type DigitalizacionMetadataResolveRequest = {
  NombreGabinete: string;
  IdTipoListaChequeo: number;
  IdConfiguracionDigitalizacion: number;
  Radicado?: string;
  IdImagen?: number;
  ValidarUnicidad?: boolean;
  RequestId?: string;
};

export type DigitalizacionMetadataResolveResponse = {
  idTipoListaChequeo: number;
  idConfiguracionDigitalizacion: number;
  obligaListaChequeo: boolean;
  esUnico: boolean;
  unicidadValidada: boolean;
  trd: DigitalizacionTrdMetadata | null;
};

export type UploadTemporalInitRequest = {
  NombreArchivo: string;
  ContentType: string;
  SizeBytes: number;
  ChunkSizeBytes: number;
  TotalChunks: number;
  ModuloRegistro?: string;
  RequestId?: string;
};

export type UploadTemporalInitResponse = {
  rutaTemporalId: string;
  archivoTemporalId: string;
  chunkSizeBytes: number;
  totalChunks: number;
};

export type UploadTemporalCompleteRequest = {
  SizeBytes: number;
  TotalChunks: number;
  RequestId?: string;
};

export type UploadTemporalCompleteResponse = {
  rutaTemporalId: string;
  archivoTemporalId: string;
  completado: boolean;
};

export type UploadTemporalReferencia = {
  rutaTemporalId: string;
  archivoTemporalId: string;
};

export type UploadTemporalPdfProgress = {
  uploadedChunks: number;
  totalChunks: number;
  progress: number;
};

export type UploadTemporalPdfOptions = DigitalizacionApiRequestOptions & {
  chunkSizeBytes?: number;
  requestId?: string;
  onProgress?: (progress: UploadTemporalPdfProgress) => void;
};

export type CrearDocumentoDigitalizadoRequest = {
  NombreGabinete: string;
  RutaTemporalId: string;
  ArchivoTemporalId: string;
  NombreDocumento: string;
  RequestId?: string;
  Radicado?: string;
  IdTareaWorkflow?: number;
  IdRutaWorkflow?: number;
  IdConfiguracionDigitalizacion?: number;
  IdTipoListaChequeo?: number;
  Trd?: DigitalizacionTrdMetadata | null;
  NumeroPaginasDeclaradas?: number;
};

export type CrearDocumentoDigitalizadoResponse = {
  idDocumento: number;
  nombreGabinete: string;
  nombreDocumento: string;
  extension: string;
  numeroPaginas: number;
  radicado?: string;
  requestId?: string;
};

export type AdjuntarDigitalizacionValidacionQuery = {
  NombreGabinete: string;
  Radicado?: string;
};

export type AdjuntarDigitalizacionValidacionResponse = {
  idDocumento: number;
  nombreGabinete: string;
  permitido: boolean;
  codigoBloqueo?: string;
  mensajeBloqueo?: string;
  esPdf: boolean;
  estaFirmado: boolean;
  estaBloqueado: boolean;
  radicadoNoModificable: boolean;
  numeroPaginasActual?: number;
};

export type AdjuntarDigitalizacionPdfRequest = {
  NombreGabinete: string;
  RutaTemporalId: string;
  ArchivoTemporalId: string;
  RequestId?: string;
  Radicado?: string;
  IdTareaWorkflow?: number;
  IdRutaWorkflow?: number;
  Motivo?: string;
  ModuloRegistro?: string;
  TipologiaDocumental?: string;
};

export type AdjuntarDigitalizacionPdfResponse = {
  idDocumento: number;
  nombreGabinete: string;
  extension: string;
  numeroPaginasAnterior: number;
  numeroPaginasAgregadas: number;
  numeroPaginasFinal: number;
  documentoActualizado: boolean;
  requestId?: string;
};

export type DigitalizacionApiClient = {
  getConfiguracion: (
    query: DigitalizacionConfiguracionQuery,
    options?: DigitalizacionApiRequestOptions,
  ) => Promise<DigitalizacionConfiguracionResponse>;
  getListaChequeo: (
    query: DigitalizacionListaChequeoQuery,
    options?: DigitalizacionApiRequestOptions,
  ) => Promise<DigitalizacionListaChequeoResponse>;
  resolveMetadata: (
    request: DigitalizacionMetadataResolveRequest,
    options?: DigitalizacionApiRequestOptions,
  ) => Promise<DigitalizacionMetadataResolveResponse>;
  uploadPdfTemporal: (
    file: File,
    options?: UploadTemporalPdfOptions,
  ) => Promise<UploadTemporalReferencia>;
  crearDocumentoDigitalizado: (
    request: CrearDocumentoDigitalizadoRequest,
    options?: DigitalizacionApiRequestOptions,
  ) => Promise<CrearDocumentoDigitalizadoResponse>;
  validarAdjuntarDigitalizacion: (
    idDocumento: number,
    query: AdjuntarDigitalizacionValidacionQuery,
    options?: DigitalizacionApiRequestOptions,
  ) => Promise<AdjuntarDigitalizacionValidacionResponse>;
  adjuntarDigitalizacion: (
    idDocumento: number,
    request: AdjuntarDigitalizacionPdfRequest,
    options?: DigitalizacionApiRequestOptions,
  ) => Promise<AdjuntarDigitalizacionPdfResponse>;
};

export type DigitalizacionApiOperationState<TData> = {
  loading: boolean;
  data: TData | null;
  error: DigitalizacionApiError | null;
};

export type DigitalizacionPersistenciaContext = Pick<
  DigitalizacionContext,
  | "modo"
  | "nombreGabinete"
  | "radicado"
  | "idDocumentoDestino"
  | "idTareaWorkflow"
  | "idRutaWorkflow"
>;
