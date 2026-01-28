// shared/errors/useAppErrorNotifier.ts
import type { AppError } from "../errors/AppError";
import { normalizeUnknownError } from "../errors/normalizeUnknownError";
import { notifyAppError } from "../errors/notifyAppError";

export function useAppErrorNotifier() {
  /**
   * Notifica cualquier error NO relacionado directamente con Axios
   */
  function notify(
    error: unknown,
    fallbackMessage?: string
  ) {
    const appError: AppError = isAppError(error)
      ? error
      : normalizeUnknownError(error, fallbackMessage);

    notifyAppError(appError);
  }

  return notify;
}

function isAppError(value: unknown): value is AppError {
  return (
    typeof value === "object" &&
    value !== null &&
    "message" in value &&
    "severity" in value &&
    "source" in value
  );
}
