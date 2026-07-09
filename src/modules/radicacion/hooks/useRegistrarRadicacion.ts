import { useCallback, useState } from "react";
import { useMutation } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import {
  registrarRadicacionEntrante,
} from "../services/radicacionRegistro.service";
import { useRadicacionDocumentalContext } from "./useRadicacionDocumentalContext";
import type {
  AppResponses,
  RadicacionPostRegistroState,
  RegistrarRadicacionEntranteRequestDto,
  RegistrarRadicacionEntranteResponseDto,
  ReturnRegistraRadicacionDto,
} from "../types/radicacionRegistro.types";

type RegistrarRadicacionErrorPayload = {
  message?: string | null;
  Message?: string | null;
  title?: string | null;
  Title?: string | null;
  errors?: unknown;
  Errors?: unknown;
};

type UseRegistrarRadicacionParams = {
  onSuccess?: (state: RadicacionPostRegistroState) => void;
  onError?: (message: string) => void;
};

const toNumber = (value: unknown): number => {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string" && value.trim().length > 0) {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
  }

  return 0;
};

const toStringValue = (value: unknown): string =>
  typeof value === "string" || typeof value === "number"
    ? String(value).trim()
    : "";

const readBooleanSignal = (
  metadata: Record<string, unknown>,
  keys: ReadonlyArray<string>,
): boolean | null => {
  for (const key of keys) {
    const value = metadata[key];
    if (typeof value === "boolean") return value;
    if (typeof value === "number") {
      if (value === 1) return true;
      if (value === 0) return false;
    }
    if (typeof value === "string") {
      const normalized = value.trim().toLowerCase();
      if (["true", "1", "si", "sí"].includes(normalized)) return true;
      if (["false", "0", "no"].includes(normalized)) return false;
    }
  }

  return null;
};

const getEnvelopeData = (
  envelope: AppResponses<RegistrarRadicacionEntranteResponseDto>,
) => envelope.data ?? envelope.Data ?? null;

const getEnvelopeSuccess = (
  envelope: AppResponses<RegistrarRadicacionEntranteResponseDto>,
) => envelope.success ?? envelope.Success ?? false;

const getEnvelopeMessage = (
  envelope: AppResponses<RegistrarRadicacionEntranteResponseDto>,
) => envelope.message ?? envelope.Message ?? "";

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null && !Array.isArray(value);

const getErrorMessageFromRecord = (record: Record<string, unknown>): string => {
  const message =
    record.message ??
    record.Message ??
    record.errorMessage ??
    record.ErrorMessage;
  return toStringValue(message);
};

const stringifyErrorDetail = (value: unknown): string => {
  if (typeof value === "string" || typeof value === "number") {
    return toStringValue(value);
  }

  if (Array.isArray(value)) {
    return value
      .map(stringifyErrorDetail)
      .filter(Boolean)
      .join(", ");
  }

  if (isRecord(value)) {
    const message = getErrorMessageFromRecord(value);
    if (message) return message;

    return Object.values(value)
      .map(stringifyErrorDetail)
      .filter(Boolean)
      .join(", ");
  }

  return "";
};

const extractErrorDetails = (errors: unknown): string[] => {
  if (!errors) return [];

  if (Array.isArray(errors)) {
    return errors.map(stringifyErrorDetail).filter(Boolean);
  }

  if (isRecord(errors)) {
    return Object.entries(errors)
      .map(([field, detail]) => {
        const normalizedDetail = stringifyErrorDetail(detail);
        return normalizedDetail ? `${field}: ${normalizedDetail}` : "";
      })
      .filter(Boolean);
  }

  const detail = stringifyErrorDetail(errors);
  return detail ? [detail] : [];
};

const joinValidationMessage = (
  fallback: string,
  errors: unknown,
): string => {
  const details = extractErrorDetails(errors);
  if (details.length === 0) return fallback;
  return `${fallback}: ${details.join(" | ")}`;
};

const getReturnRegistro = (
  response: RegistrarRadicacionEntranteResponseDto,
): ReturnRegistraRadicacionDto =>
  response.ReturnRegistraRadicacion ??
  response.returnRegistraRadicacion ??
  {};

const getMetadataOperativa = (
  response: RegistrarRadicacionEntranteResponseDto,
): Record<string, unknown> =>
  response.MetadataOperativa ?? response.metadataOperativa ?? {};

const buildPostRegistroState = (
  response: RegistrarRadicacionEntranteResponseDto,
): RadicacionPostRegistroState => {
  const returnRegistro = getReturnRegistro(response);
  const metadataOperativa = getMetadataOperativa(response);
  const consecutivoRadicado =
    toStringValue(returnRegistro.ConsecutivoRadicado) ||
    toStringValue(returnRegistro.consecutivoRadicado) ||
    toStringValue(response.ConsecutivoRadicado) ||
    toStringValue(response.consecutivoRadicado);
  const idEstadoRadicado = toNumber(
    returnRegistro.IdEstadoRadicado ?? returnRegistro.idEstadoRadicado,
  );
  const requiereGestionDocumental =
    readBooleanSignal(metadataOperativa, [
      "requiereGestionDocumental",
      "RequiereGestionDocumental",
    ]) === true;
  const tieneTramiteDocumentalActivoEstado0 =
    readBooleanSignal(metadataOperativa, [
      "tieneTramiteDocumentalActivoEstado0",
      "TieneTramiteDocumentalActivoEstado0",
      "tieneActivoEstado0",
      "TieneActivoEstado0",
    ]) === true;
  const destinoPostRegistro =
    requiereGestionDocumental && tieneTramiteDocumentalActivoEstado0
      ? "documentos"
      : "resumen";

  return {
    consecutivoRadicado,
    idRadicado: toNumber(returnRegistro.IdRadicado ?? returnRegistro.idRadicado),
    idEstadoRadicado,
    estadoAsignacion:
      toStringValue(response.EstadoAsignacion) ||
      toStringValue(response.estadoAsignacion),
    metadataOperativa,
    requiereGestionDocumental,
    tieneTramiteDocumentalActivoEstado0,
    destinoPostRegistro,
  };
};

const getFunctionalErrorMessage = (
  envelope: AppResponses<RegistrarRadicacionEntranteResponseDto>,
): string => {
  const fallback =
    getEnvelopeMessage(envelope) ||
    "No fue posible registrar la radicacion entrante.";
  return joinValidationMessage(fallback, envelope.errors ?? envelope.Errors);
};

const getAxiosErrorMessage = (
  error: AxiosError<RegistrarRadicacionErrorPayload> | unknown,
): string => {
  if (
    typeof error === "object" &&
    error !== null &&
    "response" in error &&
    typeof error.response === "object" &&
    error.response !== null
  ) {
    const data = (error.response as { data?: RegistrarRadicacionErrorPayload }).data;
    const fallback =
      data?.message ??
      data?.Message ??
      data?.title ??
      data?.Title ??
      "No fue posible registrar la radicacion entrante.";
    return joinValidationMessage(fallback, data?.errors ?? data?.Errors);
  }

  return error instanceof Error
    ? error.message
    : "No fue posible registrar la radicacion entrante.";
};

export function useRegistrarRadicacion({
  onSuccess,
  onError,
}: UseRegistrarRadicacionParams = {}) {
  const { setContextoDocumental } = useRadicacionDocumentalContext();
  const [postRegistro, setPostRegistro] =
    useState<RadicacionPostRegistroState | null>(null);

  const mutation = useMutation({
    mutationFn: async (request: RegistrarRadicacionEntranteRequestDto) => {
      const envelope = await registrarRadicacionEntrante(request);
      if (!getEnvelopeSuccess(envelope)) {
        throw new Error(getFunctionalErrorMessage(envelope));
      }

      const data = getEnvelopeData(envelope);
      if (!data) {
        throw new Error("El backend no retorno datos de radicacion.");
      }

      return buildPostRegistroState(data);
    },
    onSuccess: (state) => {
      setPostRegistro(state);
      if (
        state.requiereGestionDocumental &&
        state.tieneTramiteDocumentalActivoEstado0 &&
        state.idEstadoRadicado > 0
      ) {
        setContextoDocumental({
          idEstadoRadicado: state.idEstadoRadicado,
          idRadicado: state.idRadicado,
          consecutivoRadicado: state.consecutivoRadicado,
          estadoActual: 0,
          requiereGestionDocumental: true,
          tieneTramiteDocumentalActivoEstado0: true,
          destinoPostRegistro: "documentos",
          metadataOperativa: state.metadataOperativa,
          contextoDocumental: state.contextoDocumental ?? null,
        });
      }
      onSuccess?.(state);
    },
    onError: (error) => {
      const message = getAxiosErrorMessage(error);
      console.warn("[Radicacion][Registrar][error]", {
        message,
        error,
      });
      onError?.(message);
    },
  });

  const registrar = useCallback(
    (request: RegistrarRadicacionEntranteRequestDto) => mutation.mutateAsync(request),
    [mutation],
  );

  return {
    registrar,
    postRegistro,
    isSubmitting: mutation.isPending,
    isSuccess: mutation.isSuccess,
    isError: mutation.isError,
    error: mutation.error,
  };
}
