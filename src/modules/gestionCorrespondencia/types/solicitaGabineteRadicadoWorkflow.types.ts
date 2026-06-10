import type { ApiResponse } from "../../../api/ApiResponse";

export type EstadoExistenciaRadicado = "YES" | "NO";

export type RadicadoGabineteWorkflowDto = {
  EstadoExistenciaRadicado?: EstadoExistenciaRadicado;
  NombreGabinete?: string;
  Radicado?: string;
  IdTareaWorkflow?: number;
};

export type SolicitaGabineteRadicadoWorkflowResponse = ApiResponse<RadicadoGabineteWorkflowDto | null>;

