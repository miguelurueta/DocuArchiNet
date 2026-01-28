// shared/errors/normalizeUnknownError.ts
import type { AppError } from "./AppError";

export function normalizeUnknownError(
  error: unknown,
  fallbackMessage = "Ocurrió un error inesperado."
): AppError {
  if (error instanceof Error) {
    return {
      source: "runtime",
      severity: "error",
      message: error.message || fallbackMessage,
      details: error,
    };
  }

  if (typeof error === "string") {
    return {
      source: "unknown",
      severity: "error",
      message: error,
    };
  }

  if (typeof error === "object" && error !== null) {
    const anyErr = error as any;

    return {
      source: "unknown",
      severity: "error",
      message:
        anyErr.message ||
        anyErr.Message ||
        fallbackMessage,
      details: error,
    };
  }

  return {
    source: "unknown",
    severity: "error",
    message: fallbackMessage,
    details: error,
  };
}
