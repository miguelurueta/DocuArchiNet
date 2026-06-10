import axios from "axios";
import type { AxiosRequestConfig } from "axios";
import type {
  DigitalizacionApiError,
  DigitalizacionApiErrorStatus,
  DigitalizacionApiResponseEnvelope,
} from "../types/digitalizacionApi.types";
import type { DigitalizacionContext, DigitalizacionTrdMetadata } from "../types/digitalizacion.types";

export class DigitalizacionApiContractError extends Error {
  readonly detail: DigitalizacionApiError;

  constructor(detail: DigitalizacionApiError) {
    super(detail.message);
    this.name = "DigitalizacionApiContractError";
    this.detail = detail;
  }
}

export const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null && !Array.isArray(value);

export const getString = (record: Record<string, unknown>, camel: string, pascal?: string) => {
  const value = record[camel] ?? (pascal ? record[pascal] : undefined);
  return typeof value === "string" && value.trim().length > 0 ? value.trim() : undefined;
};

export const getNumber = (record: Record<string, unknown>, camel: string, pascal?: string) => {
  const value = record[camel] ?? (pascal ? record[pascal] : undefined);
  return typeof value === "number" && Number.isFinite(value) ? value : undefined;
};

export const getBoolean = (record: Record<string, unknown>, camel: string, pascal?: string) => {
  const value = record[camel] ?? (pascal ? record[pascal] : undefined);
  return typeof value === "boolean" ? value : undefined;
};

export const getStringArray = (record: Record<string, unknown>, camel: string, pascal?: string) => {
  const value = record[camel] ?? (pascal ? record[pascal] : undefined);
  return Array.isArray(value) ? value.filter((item): item is string => typeof item === "string") : [];
};

export const createDigitalizacionApiError = (
  code: string,
  message: string,
  status: DigitalizacionApiErrorStatus = "error",
  field?: string,
): DigitalizacionApiContractError =>
  new DigitalizacionApiContractError({
    code,
    message,
    status,
    ...(field ? { field } : {}),
  });

const getEnvelopeSuccess = (envelope: DigitalizacionApiResponseEnvelope<unknown>) =>
  envelope.success ?? envelope.Success;

const getEnvelopeMessage = (envelope: DigitalizacionApiResponseEnvelope<unknown>) =>
  envelope.message ?? envelope.Message ?? "Respuesta invalida del servicio.";

const getEnvelopeData = <T>(envelope: DigitalizacionApiResponseEnvelope<T>) =>
  envelope.data ?? envelope.Data ?? null;

const getEnvelopeStatus = (envelope: DigitalizacionApiResponseEnvelope<unknown>) => {
  const meta = envelope.meta ?? envelope.Meta;
  const status = meta?.status ?? meta?.Status;
  return status === "validation" || status === "conflict" ? status : "error";
};

export const unwrapAppResponse = <T>(
  response: unknown,
  validateData: (data: unknown) => T,
  endpointName: string,
): T => {
  if (!isRecord(response)) {
    throw createDigitalizacionApiError(
      "APP_RESPONSE_INVALID",
      `${endpointName}: respuesta sin envelope AppResponses.`,
      "error",
    );
  }

  const envelope = response as DigitalizacionApiResponseEnvelope<unknown>;
  if (getEnvelopeSuccess(envelope) !== true) {
    throw createDigitalizacionApiError(
      "APP_RESPONSE_UNSUCCESSFUL",
      getEnvelopeMessage(envelope),
      getEnvelopeStatus(envelope),
    );
  }

  const data = getEnvelopeData(envelope);
  if (data === null) {
    throw createDigitalizacionApiError(
      "APP_RESPONSE_DATA_REQUIRED",
      `${endpointName}: data es obligatorio.`,
      "error",
      "data",
    );
  }

  return validateData(data);
};

export const assertRecord = (value: unknown, code: string, message: string) => {
  if (!isRecord(value)) {
    throw createDigitalizacionApiError(code, message, "error");
  }
  return value;
};

export const assertNonEmptyString = (
  value: unknown,
  code: string,
  message: string,
  field?: string,
) => {
  if (typeof value !== "string" || value.trim().length === 0) {
    throw createDigitalizacionApiError(code, message, "validation", field);
  }
  return value.trim();
};

export const assertPositiveNumber = (
  value: unknown,
  code: string,
  message: string,
  field?: string,
) => {
  if (typeof value !== "number" || !Number.isFinite(value) || value <= 0) {
    throw createDigitalizacionApiError(code, message, "validation", field);
  }
  return value;
};

export const assertNonNegativeNumber = (
  value: unknown,
  code: string,
  message: string,
  field?: string,
) => {
  if (typeof value !== "number" || !Number.isFinite(value) || value < 0) {
    throw createDigitalizacionApiError(code, message, "validation", field);
  }
  return value;
};

export const assertPdfFile = (file: File | null | undefined) => {
  if (!file) {
    throw createDigitalizacionApiError("PDF_REQUIRED", "El archivo PDF es obligatorio.", "validation", "pdf");
  }
  if (file.size <= 0) {
    throw createDigitalizacionApiError("PDF_EMPTY", "El archivo PDF no tiene contenido.", "validation", "pdf");
  }
  const isPdf = file.type === "application/pdf" || file.name.toLowerCase().endsWith(".pdf");
  if (!isPdf) {
    throw createDigitalizacionApiError("PDF_INVALID_TYPE", "El archivo debe ser PDF.", "validation", "pdf");
  }
  return file;
};

export const validateDigitalizacionApiContext = (context: DigitalizacionContext | null) => {
  if (!context) {
    throw createDigitalizacionApiError(
      "CONTEXT_REQUIRED",
      "El contexto documental es obligatorio.",
      "validation",
      "context",
    );
  }
  if (context.modo !== "crear" && context.modo !== "adjuntar") {
    throw createDigitalizacionApiError("INVALID_MODE", "Modo de digitalizacion invalido.", "validation", "modo");
  }
  assertNonEmptyString(
    context.nombreGabinete,
    "NOMBRE_GABINETE_REQUIRED",
    "nombreGabinete es obligatorio.",
    "nombreGabinete",
  );
  if (context.modo === "adjuntar") {
    assertPositiveNumber(
      context.idDocumentoDestino,
      "ID_DOCUMENTO_DESTINO_REQUIRED",
      "idDocumentoDestino es obligatorio para adjuntar.",
      "idDocumentoDestino",
    );
  }
  return context;
};

export const normalizeTrd = (value: unknown): DigitalizacionTrdMetadata | null => {
  if (value === null || value === undefined) return null;
  const record = assertRecord(value, "TRD_INVALID", "TRD invalida.");
  return {
    idArea: getNumber(record, "idArea", "IdArea"),
    idSerie: getNumber(record, "idSerie", "IdSerie"),
    idSubSerie: getNumber(record, "idSubSerie", "IdSubSerie"),
    idTipoDocumento: getNumber(record, "idTipoDocumento", "IdTipoDocumento"),
    nombreArea: getString(record, "nombreArea", "NombreArea"),
    nombreSerie: getString(record, "nombreSerie", "NombreSerie"),
    nombreSubSerie: getString(record, "nombreSubSerie", "NombreSubSerie"),
    nombreTipoDocumento: getString(record, "nombreTipoDocumento", "NombreTipoDocumento"),
  };
};

export const toDigitalizacionApiError = (error: unknown): DigitalizacionApiError => {
  if (error instanceof DigitalizacionApiContractError) {
    return error.detail;
  }

  if (axios.isAxiosError(error)) {
    if (error.code === "ERR_CANCELED") {
      return {
        code: "REQUEST_ABORTED",
        message: "Operacion cancelada.",
        status: "aborted",
      };
    }
    const responseData = error.response?.data;
    if (isRecord(responseData)) {
      const message =
        getString(responseData, "message", "Message") ?? error.message ?? "Error de servicio.";
      return {
        code: `HTTP_${error.response?.status ?? "ERROR"}`,
        message,
        status: "error",
      };
    }
    return {
      code: error.code ?? "NETWORK_ERROR",
      message: error.message || "No fue posible completar la solicitud.",
      status: "error",
    };
  }

  return {
    code: "UNKNOWN_ERROR",
    message: error instanceof Error ? error.message : "Error desconocido.",
    status: "error",
  };
};

export const withSignal = (signal?: AbortSignal): AxiosRequestConfig =>
  signal ? { signal } : {};
