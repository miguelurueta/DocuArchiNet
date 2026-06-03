import type { ApiResponse } from "../../../api/ApiResponse";

export type SolicitaEstructuraRespuestaBackendItem = {
  Radicado?: string;
  Destinatario?: string;
  TramiteDocumento?: string;
  radicado?: string;
  destinatario?: string;
  tramiteDocumento?: string;
  idRespuestaRadicado?: string | number;
  IdRespuestaRadicado?: string | number;
  ID_RESPUESTA_RADICADO?: string | number;
  id_respuesta_radicado?: string | number;
};

export type GestionRespuestaEstructuraRespuesta = {
  Radicado: string;
  Destinatario: string;
  TramiteDocumento: string;
  idRespuestaRadicado?: string | number;
};

export type SolicitaEstructuraRespuestaIdTareaResponse = ApiResponse<
  SolicitaEstructuraRespuestaBackendItem[]
>;
