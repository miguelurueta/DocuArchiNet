import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { RADICACION_ROUTES } from "../routes/radicacionRoutes";
import {
  buildRadicacionEnviarPendienteEndpoint,
  enviarRadicacionPendiente,
} from "../services/radicacionPendientes.service";
import { useRadicacionDocumentalContext } from "./useRadicacionDocumentalContext";
import { RADICACION_ESTADO_ACTIVO_QUERY_KEY } from "./useRadicacionEstadoActivo";
import { RADICACION_PENDIENTES_CONTADOR_QUERY_KEY } from "./useRadicacionPendientesContador";

type BackendErrorPayload = {
  message?: string;
  Message?: string;
  code?: string;
  Code?: string;
  requestId?: string;
  RequestId?: string;
  correlationId?: string;
  CorrelationId?: string;
  errors?: Array<{
    message?: string;
    Message?: string;
    code?: string;
    Code?: string;
    requestId?: string;
    RequestId?: string;
  }>;
  Errors?: Array<{
    message?: string;
    Message?: string;
    code?: string;
    Code?: string;
    requestId?: string;
    RequestId?: string;
  }>;
};

const getAxiosErrorDetails = (error: unknown) => {
  if (
    typeof error !== "object" ||
    error === null ||
    !("response" in error) ||
    typeof error.response !== "object" ||
    error.response === null
  ) {
    return null;
  }

  const response = error.response as {
    status?: number;
    statusText?: string;
    data?: BackendErrorPayload;
    headers?: Record<string, unknown>;
  };
  const config =
    "config" in error && typeof error.config === "object" && error.config !== null
      ? (error.config as { url?: string; method?: string; baseURL?: string })
      : null;
  const data = response.data;
  const firstError = data?.errors?.[0] ?? data?.Errors?.[0];

  return {
    status: response.status,
    statusText: response.statusText,
    method: config?.method,
    baseURL: config?.baseURL,
    url: config?.url,
    code:
      data?.code ??
      data?.Code ??
      firstError?.code ??
      firstError?.Code ??
      ("code" in error ? String(error.code ?? "") : undefined),
    requestId:
      data?.requestId ??
      data?.RequestId ??
      data?.correlationId ??
      data?.CorrelationId ??
      firstError?.requestId ??
      firstError?.RequestId,
    responseData: data,
    responseHeaders: response.headers,
  };
};

const logEnviarPendiente = (
  phase: "confirm" | "request" | "success" | "error" | "blocked",
  details: Record<string, unknown>,
) => {
  const logFn = phase === "error" || phase === "blocked" ? console.warn : console.info;
  logFn(`[Radicacion][EnviarPendiente][${phase}]`, {
    timestamp: new Date().toISOString(),
    ...details,
  });
};

const buildBackendMessage = (error: unknown): string | null => {
  if (
    typeof error === "object" &&
    error !== null &&
    "response" in error &&
    typeof error.response === "object" &&
    error.response !== null &&
    "data" in error.response
  ) {
    const data = error.response.data as BackendErrorPayload;
    return (
      data.message ??
      data.Message ??
      data.errors?.find((item) => item.message ?? item.Message)?.message ??
      data.errors?.find((item) => item.message ?? item.Message)?.Message ??
      data.Errors?.find((item) => item.message ?? item.Message)?.message ??
      data.Errors?.find((item) => item.message ?? item.Message)?.Message ??
      null
    );
  }

  return null;
};

type UseEnviarRadicadoPendienteParams = {
  onSuccess?: (message: string) => void;
  onError?: (message: string) => void;
};

export function useEnviarRadicadoPendiente({
  onSuccess,
  onError,
}: UseEnviarRadicadoPendienteParams = {}) {
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const {
    idEstadoRadicado,
    estadoActual,
    requiereGestionDocumental,
    tieneTramiteDocumentalActivoEstado0,
    clearContextoDocumental,
  } = useRadicacionDocumentalContext();

  const puedeEnviarAPendiente =
    requiereGestionDocumental === true &&
    tieneTramiteDocumentalActivoEstado0 === true &&
    estadoActual === 0 &&
    typeof idEstadoRadicado === "number" &&
    idEstadoRadicado > 0;

  const mutation = useMutation({
    mutationFn: () => {
      if (!puedeEnviarAPendiente || !idEstadoRadicado) {
        logEnviarPendiente("blocked", {
          reason: "invalid-active-documental-context",
          idEstadoRadicado,
          estadoActual,
          requiereGestionDocumental,
          tieneTramiteDocumentalActivoEstado0,
        });
        throw new Error("No existe un tramite documental activo para enviar a pendiente.");
      }

      logEnviarPendiente("request", {
        idEstadoRadicado,
        endpoint: buildRadicacionEnviarPendienteEndpoint(idEstadoRadicado),
        estadoActual,
        requiereGestionDocumental,
        tieneTramiteDocumentalActivoEstado0,
      });

      return enviarRadicacionPendiente(idEstadoRadicado);
    },
    onSuccess: async (response) => {
      logEnviarPendiente("success", {
        idEstadoRadicado,
        response,
      });

      if (
        !response ||
        response.estadoActual !== 1 ||
        response.tieneTramiteDocumentalActivoEstado0 !== false
      ) {
        onError?.("El backend no confirmo estadoActual 1 para enviar a pendiente.");
        return;
      }

      clearContextoDocumental();
      await queryClient.invalidateQueries({
        queryKey: RADICACION_ESTADO_ACTIVO_QUERY_KEY,
      });
      await queryClient.invalidateQueries({
        queryKey: RADICACION_PENDIENTES_CONTADOR_QUERY_KEY,
      });
      await queryClient.invalidateQueries({
        queryKey: ["dynamic-ui-table", "radicacionPendientes"],
      });

      onSuccess?.(response.mensaje ?? "Tramite enviado a pendiente.");
      navigate(RADICACION_ROUTES.root);
    },
    onError: (error) => {
      const backendMessage = buildBackendMessage(error);
      logEnviarPendiente("error", {
        idEstadoRadicado,
        endpoint:
          typeof idEstadoRadicado === "number"
            ? buildRadicacionEnviarPendienteEndpoint(idEstadoRadicado)
            : undefined,
        backendMessage,
        axios: getAxiosErrorDetails(error),
        rawError: error,
      });
      onError?.(
        backendMessage ??
          (error instanceof Error
            ? error.message
            : "No fue posible enviar el tramite a pendiente."),
      );
    },
  });

  const enviarActivoAPendiente = () => {
    if (mutation.isPending) {
      return;
    }

    if (!puedeEnviarAPendiente) {
      logEnviarPendiente("blocked", {
        reason: "confirm-click-without-permission",
        idEstadoRadicado,
        estadoActual,
        requiereGestionDocumental,
        tieneTramiteDocumentalActivoEstado0,
      });
      onError?.("No existe un tramite documental activo para enviar a pendiente.");
      return;
    }

    logEnviarPendiente("confirm", {
      idEstadoRadicado,
      estadoActual,
      requiereGestionDocumental,
      tieneTramiteDocumentalActivoEstado0,
      endpoint: buildRadicacionEnviarPendienteEndpoint(idEstadoRadicado),
    });
    mutation.mutate();
  };

  return {
    enviarActivoAPendiente,
    puedeEnviarAPendiente,
    isSending: mutation.isPending,
  };
}
