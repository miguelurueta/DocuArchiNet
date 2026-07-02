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

const resolveIdRutaWf = (
  item?: SolicitaEstructuraRespuestaBackendItem,
): number | undefined => {
  const rawValue =
    item?.idRutaWf ??
    item?.IdRutaWf ??
    item?.idRutaWorkflow ??
    item?.IdRutaWorkflow ??
    item?.ID_RUTA_WF ??
    item?.id_ruta_wf;
  const normalized = typeof rawValue === "string" ? Number(rawValue) : rawValue;

  return typeof normalized === "number" && Number.isFinite(normalized) && normalized > 0
    ? normalized
    : undefined;
};

export const mapEstructuraRespuesta = (
  item?: SolicitaEstructuraRespuestaBackendItem,
): GestionRespuestaEstructuraRespuesta => {
  const idRespuestaRadicado = resolveIdRespuestaRadicado(item);
  const idRutaWf = resolveIdRutaWf(item);

  return {
    Radicado: item?.Radicado ?? item?.radicado ?? "",
    Destinatario: item?.Destinatario ?? item?.destinatario ?? "",
    TramiteDocumento: item?.TramiteDocumento ?? item?.tramiteDocumento ?? "",
    ...(idRutaWf !== undefined ? { idRutaWf } : {}),
    ...(idRespuestaRadicado !== undefined ? { idRespuestaRadicado } : {}),
  };
};
