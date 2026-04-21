import type { AppError } from "./AppError";

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

export function normalizeUnknownError(
  error: unknown,
  fallbackMessage = "Ocurrio un error inesperado.",
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

  if (isRecord(error)) {
    const message =
      (typeof error.message === "string" && error.message) ||
      (typeof error.Message === "string" && error.Message) ||
      fallbackMessage;

    return {
      source: "unknown",
      severity: "error",
      message,
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

