import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { AppTableActionTriggered, AppTableRow } from "../../../app/Components/UI/AppTable/AppTable.types";
import { useNavigate } from "react-router";
import { useRadicacionDocumentalContext } from "./useRadicacionDocumentalContext";
import { RADICACION_ESTADO_ACTIVO_QUERY_KEY } from "./useRadicacionEstadoActivo";
import { RADICACION_PENDIENTES_CONTADOR_QUERY_KEY } from "./useRadicacionPendientesContador";
import { RADICACION_ROUTES } from "../routes/radicacionRoutes";
import {
  fetchRadicacionEstadoActivo,
  mapEstadoActivoToDocumentalState,
  tomarRadicacionPendiente,
} from "../services/radicacionPendientes.service";
import type { RadicacionDocumentalState } from "../types/radicacionDocumental.types";
import {
  extractRadicacionPendienteActionPayload,
  RADICACION_PENDIENTE_ACTION_ID,
  type RadicacionPendienteActionPayload,
  type TomarRadicadoPendienteResponseDto,
} from "../types/radicacionPendientes.types";

const buildBackendMessage = (error: unknown): string => {
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
      "No fue posible tomar el radicado pendiente."
    );
  }

  return "No fue posible tomar el radicado pendiente.";
};

const mapTomarResponseToDocumentalState = (
  response: TomarRadicadoPendienteResponseDto | null,
): RadicacionDocumentalState | null => {
  if (
    !response ||
    response.estadoActual !== 0 ||
    response.requiereGestionDocumental !== true ||
    response.tieneTramiteDocumentalActivoEstado0 !== true ||
    !response.idEstadoRadicado
  ) {
    return null;
  }

  return {
    idEstadoRadicado: response.idEstadoRadicado,
    idRadicado: response.idRadicado ?? null,
    consecutivoRadicado: response.consecutivoRadicado ?? null,
    idTareaWorkflow: response.idTareaWorkflow ?? null,
    estadoActual: 0,
    tramite: response.tramite ?? response.metadataOperativa?.tramite ?? null,
    remitente: response.remitente ?? response.metadataOperativa?.remitente ?? null,
    plantillaId: response.plantillaId ?? response.metadataOperativa?.plantillaId ?? null,
    tipoPlantillaId: response.tipoPlantillaId ?? null,
    requiereGestionDocumental: true,
    tieneTramiteDocumentalActivoEstado0: true,
    destinoPostRegistro: "documentos",
    contextoDocumental: response.contextoDocumental ?? null,
    metadataOperativa: response.metadataOperativa ?? null,
  };
};

type UseTomarRadicadoPendienteParams = {
  onSuccess?: () => void;
  onError?: (message: string) => void;
};

export function useTomarRadicadoPendiente({
  onSuccess,
  onError,
}: UseTomarRadicadoPendienteParams = {}) {
  const queryClient = useQueryClient();
  const navigate = useNavigate();
  const { tieneTramiteDocumentalActivoEstado0, setContextoDocumental } =
    useRadicacionDocumentalContext();

  const mutation = useMutation({
    mutationFn: (payload: RadicacionPendienteActionPayload) =>
      tomarRadicacionPendiente(payload.idEstadoRadicado, {
        idTareaWorkflow: payload.idTareaWorkflow,
      }),
    onSuccess: async (response) => {
      const contexto = mapTomarResponseToDocumentalState(response);
      if (!contexto) {
        onError?.("El backend no confirmo estadoActual 0 para este radicado.");
        return;
      }

      const idEstadoRadicado = contexto.idEstadoRadicado;
      if (!idEstadoRadicado) {
        onError?.("El backend no retorno idEstadoRadicado para navegar.");
        return;
      }

      setContextoDocumental(contexto);
      await queryClient.invalidateQueries({
        queryKey: RADICACION_ESTADO_ACTIVO_QUERY_KEY,
      });
      await queryClient.invalidateQueries({
        queryKey: RADICACION_PENDIENTES_CONTADOR_QUERY_KEY,
      });
      await queryClient.invalidateQueries({
        queryKey: ["dynamic-ui-table", "radicacionPendientes"],
      });

      onSuccess?.();
      navigate(RADICACION_ROUTES.documentos(idEstadoRadicado));
    },
    onError: async (error) => {
      const message = buildBackendMessage(error);

      if (message.includes("RADICACION_TOMAR_PENDIENTE_ACTIVE_EXISTS")) {
        try {
          const estadoActivo = await fetchRadicacionEstadoActivo();
          const contextoActivo =
            mapEstadoActivoToDocumentalState(estadoActivo);

          if (contextoActivo) {
            setContextoDocumental(contextoActivo);
            await queryClient.invalidateQueries({
              queryKey: RADICACION_ESTADO_ACTIVO_QUERY_KEY,
            });
          }
        } catch {
          // Se conserva el mensaje funcional original si no se puede sincronizar.
        }
      }

      onError?.(message);
    },
  });

  const tomarDesdeFila = (row: AppTableRow) => {
    if (mutation.isPending) {
      return;
    }

    if (tieneTramiteDocumentalActivoEstado0) {
      onError?.(
        "Tarea asignada para gestion y asignacion, debe terminar la tarea actual o subirla a estado pendiente para continuar con la asignacion.",
      );
      return;
    }

    const payload = extractRadicacionPendienteActionPayload(row);
    if (!payload) {
      onError?.("El radicado pendiente no trae id_estado_radicado.");
      return;
    }

    mutation.mutate(payload);
  };

  const tomarDesdeAccion = (params: AppTableActionTriggered<AppTableRow>) => {
    const normalizedActionId = params.actionId.trim().toLowerCase();
    if (normalizedActionId !== RADICACION_PENDIENTE_ACTION_ID) {
      return;
    }

    tomarDesdeFila(params.row);
  };

  return {
    tomarDesdeAccion,
    tomarDesdeFila,
    isTaking: mutation.isPending,
  };
}
