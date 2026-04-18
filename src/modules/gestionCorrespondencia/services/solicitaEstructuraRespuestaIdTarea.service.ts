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

  return data;
};
