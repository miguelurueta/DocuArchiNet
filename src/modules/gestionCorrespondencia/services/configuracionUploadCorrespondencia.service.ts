import clienteApi from "../../../api/Clienteaxios";
import type {
  ConfiguracionUploadCorrespondencia,
  ConfiguracionUploadCorrespondenciaBackendItem,
  ConfiguracionUploadCorrespondenciaResponse,
} from "../types/configuracionUploadCorrespondencia.types";

export const CONFIGURACION_UPLOAD_CORRESPONDENCIA_ENDPOINT =
  "/api/gestor-documental/configuracion-upload";

export const CONFIGURACION_UPLOAD_CORRESPONDENCIA_PROCESO = "CORRESPO";

export class ConfiguracionUploadCorrespondenciaError extends Error {
  constructor(message: string) {
    super(message);
    this.name = "ConfiguracionUploadCorrespondenciaError";
  }
}

export async function getConfiguracionUploadCorrespondencia(
  options: { signal?: AbortSignal } = {},
): Promise<ConfiguracionUploadCorrespondencia> {
  const { data } = await clienteApi.get<ConfiguracionUploadCorrespondenciaResponse>(
    CONFIGURACION_UPLOAD_CORRESPONDENCIA_ENDPOINT,
    {
      params: {
        nameProceso: CONFIGURACION_UPLOAD_CORRESPONDENCIA_PROCESO,
      },
      signal: options.signal,
    },
  );

  return normalizeConfiguracionUploadCorrespondenciaResponse(data);
}

export function normalizeConfiguracionUploadCorrespondenciaResponse(
  payload: unknown,
): ConfiguracionUploadCorrespondencia {
  if (!isRecord(payload)) {
    throw new ConfiguracionUploadCorrespondenciaError(
      "La respuesta de configuracion de adjuntos no tiene un formato valido.",
    );
  }

  if (payload.success !== true) {
    throw new ConfiguracionUploadCorrespondenciaError(readFunctionalErrorMessage(payload));
  }

  if (!Array.isArray(payload.data)) {
    throw new ConfiguracionUploadCorrespondenciaError(
      "La respuesta de configuracion de adjuntos no contiene una lista valida.",
    );
  }

  if (payload.data.length === 0) {
    throw new ConfiguracionUploadCorrespondenciaError(
      "No hay configuracion de adjuntos para CORRESPO.",
    );
  }

  const selectedRow = selectConfigurationRow(payload.data);
  const extensionUpload = readStringProperty(selectedRow, "ExtensionUpload", "extensionUpload");
  const allowedExtensions = normalizeUploadExtensions(extensionUpload ?? "");
  const maxSizeBytes = readNumberProperty(selectedRow, "LengUpload", "lengUpload");

  if (allowedExtensions.length === 0) {
    throw new ConfiguracionUploadCorrespondenciaError(
      "La configuracion de adjuntos no contiene extensiones permitidas.",
    );
  }

  if (typeof maxSizeBytes !== "number" || !Number.isFinite(maxSizeBytes) || maxSizeBytes <= 0) {
    throw new ConfiguracionUploadCorrespondenciaError(
      "La configuracion de adjuntos no contiene un tamano maximo valido.",
    );
  }

  return {
    nameProceso: CONFIGURACION_UPLOAD_CORRESPONDENCIA_PROCESO,
    accept: allowedExtensions.join(","),
    allowedExtensions,
    maxSizeBytes,
  };
}

export function normalizeUploadExtensions(raw: string): string[] {
  const normalized = new Set<string>();

  for (const extension of raw.split(",")) {
    const trimmed = extension.trim().toLowerCase();
    if (!trimmed) continue;

    const value = trimmed.startsWith(".") ? trimmed : `.${trimmed}`;
    normalized.add(value);
  }

  return Array.from(normalized);
}

function selectConfigurationRow(
  rows: ConfiguracionUploadCorrespondenciaBackendItem[],
): ConfiguracionUploadCorrespondenciaBackendItem {
  return rows.find((row) => readNumberProperty(row, "EstadoProceso", "estadoProceso") === 1) ?? rows[0];
}

function readFunctionalErrorMessage(payload: Record<string, unknown>): string {
  const firstError = Array.isArray(payload.errors) ? payload.errors[0] : undefined;
  const userMessage = readStringProperty(firstError, "UserMessage", "userMessage");
  const message = readStringProperty(payload, "message", "Message");

  return (
    userMessage ??
    message ??
    "No fue posible cargar la configuracion de adjuntos para CORRESPO."
  );
}

function readStringProperty(source: unknown, ...keys: string[]): string | undefined {
  if (!isRecord(source)) return undefined;

  for (const key of keys) {
    const value = source[key];
    if (typeof value === "string" && value.trim().length > 0) {
      return value.trim();
    }
  }

  return undefined;
}

function readNumberProperty(source: unknown, ...keys: string[]): number | undefined {
  if (!isRecord(source)) return undefined;

  for (const key of keys) {
    const value = source[key];
    if (typeof value === "number" && Number.isFinite(value)) {
      return value;
    }
  }

  return undefined;
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return !!value && typeof value === "object" && !Array.isArray(value);
}

