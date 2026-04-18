import type {
  GestionRespuestaEstructuraRespuesta,
  SolicitaEstructuraRespuestaBackendItem,
} from "../types/gestionRespuestaEstructura.types";

export const mapEstructuraRespuesta = (
  item?: SolicitaEstructuraRespuestaBackendItem,
): GestionRespuestaEstructuraRespuesta => ({
  Radicado: item?.Radicado ?? item?.radicado ?? "",
  Destinatario: item?.Destinatario ?? item?.destinatario ?? "",
  TramiteDocumento: item?.TramiteDocumento ?? item?.tramiteDocumento ?? "",
});
