import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { RADICACION_ROUTES } from "../routes/radicacionRoutes";
import { enviarRadicacionPendiente } from "../services/radicacionPendientes.service";
import { useRadicacionDocumentalContext } from "./useRadicacionDocumentalContext";
import { RADICACION_ESTADO_ACTIVO_QUERY_KEY } from "./useRadicacionEstadoActivo";
import { RADICACION_PENDIENTES_CONTADOR_QUERY_KEY } from "./useRadicacionPendientesContador";

const buildBackendMessage = (error: unknown): string | null => {
  if (
    typeof error === "object" &&
    error !== null &&
    "response" in error &&
    typeof error.response === "object" &&
    error.response !== null &&
    "data" in error.response
  ) {
    const data = error.response.data as {
      message?: string;
      Message?: string;
      errors?: Array<{ message?: string }>;
      Errors?: Array<{ message?: string }>;
    };
    return (
      data.message ??
      data.Message ??
      data.errors?.find((item) => item.message)?.message ??
      data.Errors?.find((item) => item.message)?.message ??
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
        throw new Error("No existe un tramite documental activo para enviar a pendiente.");
      }

      return enviarRadicacionPendiente(idEstadoRadicado);
    },
    onSuccess: async (response) => {
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
      onError?.("No existe un tramite documental activo para enviar a pendiente.");
      return;
    }

    mutation.mutate();
  };

  return {
    enviarActivoAPendiente,
    puedeEnviarAPendiente,
    isSending: mutation.isPending,
  };
}
