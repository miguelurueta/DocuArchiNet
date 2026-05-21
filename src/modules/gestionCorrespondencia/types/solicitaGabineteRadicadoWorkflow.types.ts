import type { ApiResponse } from "../../../api/ApiResponse";

export type EstadoExistenciaRadicado = "YES" | "NO";

export type RadicadoGabineteWorkflowDto = {
  EstadoExistenciaRadicado?: EstadoExistenciaRadicado;
  NombreGabinete?: string;
};

export type SolicitaGabineteRadicadoWorkflowResponse = ApiResponse<RadicadoGabineteWorkflowDto | null>;

