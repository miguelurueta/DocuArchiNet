import axios from "axios";
import clienteApi from "../../../api/Clienteaxios";
import { createStorageRequestId, isRecord } from "../../almacenamientoDocumental/utils/storageFile.utils";

export const ELIMINAR_DOCUMENTO_STORAGE_ENGINE_ENDPOINT = (idAlmacen: number) =>
  `/api/gestor-documental/eliminar-documento/${idAlmacen}`;

export type EliminarDocumentoStorageEngineInput = {
  idAlmacen: number;
  nombreGabinete: string;
  sourceModule?: "WORKFLOW";
  requestId?: string;
  signal?: AbortSignal;
};

export type EliminarDocumentoStorageEngineResult = {
  success: boolean;
  message: string;
  severity: "success" | "warning" | "error";
  requestId?: string;
  httpStatus?: number;
  rawResponse?: unknown;
};

const DEFAULT_SUCCESS_MESSAGE = "Documento eliminado correctamente.";
const DEFAULT_ERROR_MESSAGE = "No fue posible eliminar el documento.";

export async function eliminarDocumentoStorageEngine(
  input: EliminarDocumentoStorageEngineInput,
): Promise<EliminarDocumentoStorageEngineResult> {
  validateInput(input);

  const requestId = input.requestId?.trim() || createStorageRequestId("gestion-respuesta-delete");

  try {
    const response = await clienteApi.delete(
      ELIMINAR_DOCUMENTO_STORAGE_ENGINE_ENDPOINT(input.idAlmacen),
      {
        params: {
          nombreGabinete: input.nombreGabinete.trim(),
          sourceModule: input.sourceModule ?? "WORKFLOW",
        },
        headers: {
          "X-Request-Id": requestId,
        },
        signal: input.signal,
      },
    );

    return normalizeResponse(response.data, {
      defaultSuccessMessage: DEFAULT_SUCCESS_MESSAGE,
      defaultRequestId: requestId,
      httpStatus: response.status,
    });
  } catch (error) {
    if (axios.isAxiosError(error)) {
      const response = error.response;
      if (response) {
        return normalizeResponse(response.data, {
          defaultSuccessMessage: DEFAULT_SUCCESS_MESSAGE,
          defaultRequestId: requestId,
          httpStatus: response.status,
        });
      }
    }

    return {
      success: false,
      message: DEFAULT_ERROR_MESSAGE,
      severity: "error",
      requestId,
      rawResponse: error,
    };
  }
}

function normalizeResponse(
  payload: unknown,
  options: {
    defaultSuccessMessage: string;
    defaultRequestId: string;
    httpStatus?: number;
  },
): EliminarDocumentoStorageEngineResult {
  if (payload == null || payload === "") {
    if (typeof options.httpStatus === "number" && options.httpStatus >= 400) {
      return {
        success: false,
        message: DEFAULT_ERROR_MESSAGE,
        severity: mapSeverity(undefined, options.httpStatus, DEFAULT_ERROR_MESSAGE),
        requestId: options.defaultRequestId,
        httpStatus: options.httpStatus,
        rawResponse: payload,
      };
    }

    return {
      success: true,
      message: options.defaultSuccessMessage,
      severity: "success",
      requestId: options.defaultRequestId,
      httpStatus: options.httpStatus,
      rawResponse: payload,
    };
  }

  if (!isRecord(payload)) {
    if (typeof options.httpStatus === "number" && options.httpStatus >= 400) {
      return {
        success: false,
        message: DEFAULT_ERROR_MESSAGE,
        severity: mapSeverity(undefined, options.httpStatus, DEFAULT_ERROR_MESSAGE),
        requestId: options.defaultRequestId,
        httpStatus: options.httpStatus,
        rawResponse: payload,
      };
    }

    return {
      success: true,
      message: options.defaultSuccessMessage,
      severity: "success",
      requestId: options.defaultRequestId,
      httpStatus: options.httpStatus,
      rawResponse: payload,
    };
  }

  const success = readBoolean(payload, "success", "Success");
  const isHttpFailure = typeof options.httpStatus === "number" && options.httpStatus >= 400;
  const requestId =
    readString(payload, "requestId", "RequestId") ??
    readString(readRecord(payload, "meta", "Meta"), "requestId", "RequestId") ??
    readString(readRecordOrArrayFirst(payload, "errors", "Errors"), "requestId", "RequestId") ??
    options.defaultRequestId;

  const message =
    readString(readRecordOrArrayFirst(payload, "errors", "Errors"), "UserMessage", "userMessage") ??
    readString(readRecordOrArrayFirst(payload, "errors", "Errors"), "Message", "message") ??
    readString(payload, "message", "Message") ??
    (success === true ? options.defaultSuccessMessage : DEFAULT_ERROR_MESSAGE);

  const severity =
    success === true && !isHttpFailure
      ? "success"
      : mapSeverity(
          readString(readRecord(payload, "meta", "Meta"), "status", "Status"),
          options.httpStatus,
          message,
        );

  return {
    success: isHttpFailure ? false : success !== false,
    message,
    severity,
    requestId,
    httpStatus: options.httpStatus ?? readNumber(readRecord(payload, "meta", "Meta"), "httpStatus", "HttpStatus"),
    rawResponse: payload,
  };
}

function mapSeverity(
  status: string | undefined,
  httpStatus: number | undefined,
  message: string,
): "warning" | "error" {
  const normalizedStatus = status?.trim().toLowerCase();

  if (normalizedStatus === "validation" || normalizedStatus === "business") {
    return "warning";
  }

  if (normalizedStatus === "forbidden" || normalizedStatus === "not_found" || normalizedStatus === "error") {
    return "error";
  }

  if (httpStatus === 400 || httpStatus === 401 || httpStatus === 409) {
    return "warning";
  }

  if (httpStatus === 403 || httpStatus === 404 || httpStatus === 500) {
    return "error";
  }

  if (!message || message === DEFAULT_ERROR_MESSAGE) {
    return "error";
  }

  return "warning";
}

function validateInput(input: EliminarDocumentoStorageEngineInput): void {
  if (!Number.isFinite(input.idAlmacen) || input.idAlmacen <= 0) {
    throw new TypeError("idAlmacen debe ser un numero positivo.");
  }

  if (typeof input.nombreGabinete !== "string" || input.nombreGabinete.trim().length === 0) {
    throw new TypeError("nombreGabinete es obligatorio.");
  }
}

function readRecord(record: Record<string, unknown>, ...keys: string[]): Record<string, unknown> | undefined {
  for (const key of keys) {
    const value = record[key];
    if (isRecord(value)) {
      return value;
    }
  }

  return undefined;
}

function readFirstRecord(record: Record<string, unknown>, ...keys: string[]): Record<string, unknown> | undefined {
  for (const key of keys) {
    const value = record[key];
    if (Array.isArray(value) && value.length > 0 && isRecord(value[0])) {
      return value[0];
    }
  }

  return undefined;
}

function readRecordOrArrayFirst(record: Record<string, unknown>, ...keys: string[]): Record<string, unknown> | undefined {
  const arrayRecord = readFirstRecord(record, ...keys);
  if (arrayRecord) {
    return arrayRecord;
  }

  return readRecord(record, ...keys);
}

function readString(record: Record<string, unknown> | undefined, ...keys: string[]): string | undefined {
  if (!record) return undefined;

  for (const key of keys) {
    const value = record[key];
    if (typeof value === "string" && value.trim().length > 0) {
      return value.trim();
    }
  }

  return undefined;
}

function readBoolean(record: Record<string, unknown>, ...keys: string[]): boolean | undefined {
  for (const key of keys) {
    const value = record[key];
    if (typeof value === "boolean") {
      return value;
    }
  }

  return undefined;
}

function readNumber(record: Record<string, unknown> | undefined, ...keys: string[]): number | undefined {
  if (!record) return undefined;

  for (const key of keys) {
    const value = record[key];
    if (typeof value === "number" && Number.isFinite(value)) {
      return value;
    }
  }

  return undefined;
}
