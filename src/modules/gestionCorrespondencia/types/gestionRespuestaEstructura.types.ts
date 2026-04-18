import type { ApiResponse } from "../../../api/ApiResponse";

export type SolicitaEstructuraRespuestaBackendItem = {
  Radicado?: string;
  Destinatario?: string;
  TramiteDocumento?: string;
  radicado?: string;
  destinatario?: string;
  tramiteDocumento?: string;
};

export type GestionRespuestaEstructuraRespuesta = {
  Radicado: string;
  Destinatario: string;
  TramiteDocumento: string;
};

export type SolicitaEstructuraRespuestaIdTareaResponse = ApiResponse<
  SolicitaEstructuraRespuestaBackendItem[]
>;
