export type DynamsoftScannerErrorCode =
  | "DYNAMSOFT_CSS_LOAD_FAILED"
  | "DYNAMSOFT_SCRIPT_LOAD_FAILED"
  | "DYNAMSOFT_RUNTIME_UNAVAILABLE"
  | "DYNAMSOFT_LICENSE_INVALID"
  | "SCANNER_NOT_SELECTED"
  | "SCANNER_NOT_FOUND"
  | "SCAN_IN_PROGRESS"
  | "SCAN_CANCELLED"
  | "SCAN_FAILED"
  | "PDF_EMPTY"
  | "PDF_GENERATION_FAILED"
  | "INVALID_SCAN_OPTIONS"
  | "INVALID_DEVICE_ID"
  | "STALE_OPERATION_IGNORED";

export class DynamsoftScannerError extends Error {
  readonly code: DynamsoftScannerErrorCode;
  readonly recoverable: boolean;

  constructor({
    code,
    message,
    recoverable = true,
  }: {
    code: DynamsoftScannerErrorCode;
    message: string;
    recoverable?: boolean;
  }) {
    super(message);
    this.name = "DynamsoftScannerError";
    this.code = code;
    this.recoverable = recoverable;
  }
}

export const toDynamsoftScannerError = (
  error: unknown,
  fallbackCode: DynamsoftScannerErrorCode,
  fallbackMessage: string,
) => {
  if (error instanceof DynamsoftScannerError) {
    return error;
  }

  return new DynamsoftScannerError({
    code: fallbackCode,
    message: fallbackMessage,
  });
};
