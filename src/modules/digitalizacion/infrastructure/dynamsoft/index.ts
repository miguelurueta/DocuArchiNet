export { DynamsoftTwainClient } from "./DynamsoftTwainClient";
export {
  DYNAMSOFT_ALLOWED_COLOR_MODES,
  DYNAMSOFT_CONTAINER_ID,
  DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
  DYNAMSOFT_DEFAULT_SCRIPT_SRC,
  DYNAMSOFT_MAX_RESOLUTION_DPI,
  DYNAMSOFT_MIN_RESOLUTION_DPI,
  DYNAMSOFT_SCRIPT_ID,
} from "./dynamsoft.constants";
export { DynamsoftScannerError, toDynamsoftScannerError } from "./dynamsoft.errors";
export type { DynamsoftScannerErrorCode } from "./dynamsoft.errors";
export { loadDynamsoftScripts } from "./loadDynamsoftScripts";
export type {
  DigitalizacionScannerClient,
  DynamsoftAcquireOptions,
  DynamsoftImageType,
  DynamsoftRuntimeOptions,
  DynamsoftWebTwainFactory,
  DynamsoftWebTwainObject,
  DynamsoftWindow,
  PdfGenerationResult,
  ScanColorMode,
  ScanOptions,
  ScanPage,
  ScannerDevice,
} from "./dynamsoft.types";
