export { DigitalizacionDocumentalModal } from "./components/DigitalizacionDocumentalModal";
export {
  buildDigitalizacionTitle,
  DigitalizacionDocumentalWorkspace,
} from "./components/DigitalizacionDocumentalWorkspace";
export { useDigitalizacionDocumentalState } from "./hooks/useDigitalizacionDocumentalState";
export { useDigitalizacionScanner } from "./hooks/useDigitalizacionScanner";
export { useDigitalizacionConfiguracion } from "./hooks/useDigitalizacionConfiguracion";
export { useDigitalizacionListaChequeo } from "./hooks/useDigitalizacionListaChequeo";
export { useDigitalizacionMetadataResolve } from "./hooks/useDigitalizacionMetadataResolve";
export { useCrearDocumentoDigitalizado } from "./hooks/useCrearDocumentoDigitalizado";
export { useAdjuntarDigitalizacion } from "./hooks/useAdjuntarDigitalizacion";
export { useUploadTemporalPdf } from "./hooks/useUploadTemporalPdf";
export { useDigitalizacionOperationOrchestrator } from "./hooks/useDigitalizacionOperationOrchestrator";
export type {
  DigitalizacionOperationOrchestratorState,
  DigitalizacionOperationSubmitInput,
  DigitalizacionOrchestratorStatus,
} from "./hooks/useDigitalizacionOperationOrchestrator";
export {
  digitalizacionApiClient,
} from "./services/digitalizacionApi";
export {
  buildDigitalizacionContextSignature,
  validateDigitalizacionContext,
} from "./services/digitalizacionContract";
export { DynamsoftTwainClient } from "./infrastructure/dynamsoft";
export type {
  AutomaticImageProcessingOptions,
  AutomaticImageProcessingResult,
  AutomaticImageProcessingStatus,
  DigitalizacionScannerClient,
  DynamsoftRuntimeOptions,
  DynamsoftScannerErrorCode,
  PageCropSelection,
  PdfGenerationResult,
  ScanColorMode,
  ScanOptions,
  ScanPage,
  ScannerDevice,
} from "./infrastructure/dynamsoft";
export { DynamsoftScannerError } from "./infrastructure/dynamsoft";
export {
  DigitalizacionApiContractError,
  assertPdfFile,
  toDigitalizacionApiError,
  validateDigitalizacionApiContext,
} from "./services/digitalizacionApiClient";
export {
  getDigitalizacionConfiguracion,
  DIGITALIZACION_CONFIGURACION_ENDPOINT,
} from "./services/digitalizacionConfiguracion.api";
export {
  getDigitalizacionListaChequeo,
  DIGITALIZACION_LISTA_CHEQUEO_ENDPOINT,
} from "./services/digitalizacionListaChequeo.api";
export {
  resolveDigitalizacionMetadata,
  DIGITALIZACION_METADATA_RESOLVE_ENDPOINT,
} from "./services/digitalizacionMetadata.api";
export {
  crearDocumentoDigitalizado,
  DIGITALIZACION_DOCUMENTOS_ENDPOINT,
} from "./services/digitalizacionDocumentos.api";
export {
  adjuntarDigitalizacion,
  validarAdjuntarDigitalizacion,
} from "./services/adjuntarDigitalizacion.api";
export {
  uploadPdfTemporal,
  DEFAULT_UPLOAD_TEMPORAL_CHUNK_SIZE_BYTES,
} from "./services/digitalizacionUploadTemporal.api";
export type {
  DigitalizacionContext,
  DigitalizacionDocumentalError,
  DigitalizacionDocumentalProps,
  DigitalizacionDocumentalWorkspaceProps,
  DigitalizacionFunctionalError,
  DigitalizacionFunctionalErrorCode,
  DigitalizacionGeneratedPdf,
  DigitalizacionMetadataState,
  DigitalizacionModo,
  DigitalizacionOperationState,
  DigitalizacionResult,
  DigitalizacionScannedPage,
  DigitalizacionScannerState,
  DigitalizacionState,
  DigitalizacionTrdMetadata,
} from "./types/digitalizacion.types";
export type {
  AdjuntarDigitalizacionPdfRequest,
  AdjuntarDigitalizacionPdfResponse,
  AdjuntarDigitalizacionValidacionQuery,
  AdjuntarDigitalizacionValidacionResponse,
  CrearDocumentoDigitalizadoRequest,
  CrearDocumentoDigitalizadoResponse,
  DigitalizacionApiClient,
  DigitalizacionApiError,
  DigitalizacionApiOperationState,
  DigitalizacionConfiguracionQuery,
  DigitalizacionConfiguracionResponse,
  DigitalizacionListaChequeoItem,
  DigitalizacionListaChequeoQuery,
  DigitalizacionListaChequeoResponse,
  DigitalizacionMetadataResolveRequest,
  DigitalizacionMetadataResolveResponse,
  UploadTemporalPdfProgress,
  UploadTemporalReferencia,
} from "./types/digitalizacionApi.types";
