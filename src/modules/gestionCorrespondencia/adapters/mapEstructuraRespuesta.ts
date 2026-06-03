import type {
  GestionRespuestaEstructuraRespuesta,
  SolicitaEstructuraRespuestaBackendItem,
} from "../types/gestionRespuestaEstructura.types";

const resolveIdRespuestaRadicado = (
  item?: SolicitaEstructuraRespuestaBackendItem,
): string | number | undefined =>
  item?.idRespuestaRadicado ??
  item?.IdRespuestaRadicado ??
  item?.ID_RESPUESTA_RADICADO ??
  item?.id_respuesta_radicado;

export const mapEstructuraRespuesta = (
  item?: SolicitaEstructuraRespuestaBackendItem,
): GestionRespuestaEstructuraRespuesta => {
  const idRespuestaRadicado = resolveIdRespuestaRadicado(item);

  return {
    Radicado: item?.Radicado ?? item?.radicado ?? "",
    Destinatario: item?.Destinatario ?? item?.destinatario ?? "",
    TramiteDocumento: item?.TramiteDocumento ?? item?.tramiteDocumento ?? "",
    ...(idRespuestaRadicado !== undefined ? { idRespuestaRadicado } : {}),
  };
};
