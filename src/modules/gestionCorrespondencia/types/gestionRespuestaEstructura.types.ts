import type { ApiResponse } from "../../../api/ApiResponse";

export type SolicitaEstructuraRespuestaBackendItem = {
  Radicado?: string;
  Destinatario?: string;
  TramiteDocumento?: string;
  radicado?: string;
  destinatario?: string;
  tramiteDocumento?: string;
  idRutaWf?: string | number;
  IdRutaWf?: string | number;
  idRutaWorkflow?: string | number;
  IdRutaWorkflow?: string | number;
  ID_RUTA_WF?: string | number;
  id_ruta_wf?: string | number;
  idRespuestaRadicado?: string | number;
  IdRespuestaRadicado?: string | number;
  ID_RESPUESTA_RADICADO?: string | number;
  id_respuesta_radicado?: string | number;
};

export type GestionRespuestaEstructuraRespuesta = {
  Radicado: string;
  Destinatario: string;
  TramiteDocumento: string;
  idRutaWf?: number;
  idRespuestaRadicado?: string | number;
};

export type SolicitaEstructuraRespuestaIdTareaResponse = ApiResponse<
  SolicitaEstructuraRespuestaBackendItem[]
>;
