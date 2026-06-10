export { DigitalizacionDocumentalModal } from "./components/DigitalizacionDocumentalModal";
export { useDigitalizacionDocumentalState } from "./hooks/useDigitalizacionDocumentalState";
export {
  buildDigitalizacionContextSignature,
  validateDigitalizacionContext,
} from "./services/digitalizacionContract";
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
