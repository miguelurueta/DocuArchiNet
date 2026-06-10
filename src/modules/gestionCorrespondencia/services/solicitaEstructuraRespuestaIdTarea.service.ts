import clienteApi from "../../../api/Clienteaxios";
import type { SolicitaEstructuraRespuestaIdTareaResponse } from "../types/gestionRespuestaEstructura.types";

export const SOLICITA_ESTRUCTURA_RESPUESTA_ID_TAREA_ENDPOINT =
  "/api/GestionCorrespondencia/solicita-estructura-respuesta-id-tarea";

export const getSolicitaEstructuraRespuestaIdTarea = async (
  idTareaWf: number,
): Promise<SolicitaEstructuraRespuestaIdTareaResponse> => {
  const { data } = await clienteApi.get<SolicitaEstructuraRespuestaIdTareaResponse>(
    SOLICITA_ESTRUCTURA_RESPUESTA_ID_TAREA_ENDPOINT,
    {
      params: { idTareaWf },
    },
  );

  if (import.meta.env.MODE !== "production") {
    const responseData = data as {
      success?: unknown;
      Success?: unknown;
      data?: unknown;
      Data?: unknown;
    };
    const success = responseData.success ?? responseData.Success;
    const payload = responseData.data ?? responseData.Data;
    const size = Array.isArray(payload) ? payload.length : payload ? 1 : 0;

    console.groupCollapsed(
      `[gestion-correspondencia] estructura-respuesta idTareaWf=${idTareaWf} success=${String(success)} items=${size}`,
    );
    console.log("endpoint:", SOLICITA_ESTRUCTURA_RESPUESTA_ID_TAREA_ENDPOINT);
    console.log("raw response:", data);
    console.groupEnd();
  }

  return data;
};
