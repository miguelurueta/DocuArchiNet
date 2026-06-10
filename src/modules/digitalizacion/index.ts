export { DigitalizacionDocumentalModal } from "./components/DigitalizacionDocumentalModal";
export { useDigitalizacionDocumentalState } from "./hooks/useDigitalizacionDocumentalState";
export { useDigitalizacionScanner } from "./hooks/useDigitalizacionScanner";
export {
  buildDigitalizacionContextSignature,
  validateDigitalizacionContext,
} from "./services/digitalizacionContract";
export { DynamsoftTwainClient } from "./infrastructure/dynamsoft";
export type {
  DigitalizacionScannerClient,
  DynamsoftScannerErrorCode,
  PdfGenerationResult,
  ScanColorMode,
  ScanOptions,
  ScanPage,
  ScannerDevice,
} from "./infrastructure/dynamsoft";
export { DynamsoftScannerError } from "./infrastructure/dynamsoft";
export type {
  DigitalizacionContext,
  DigitalizacionDocumentalProps,
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
