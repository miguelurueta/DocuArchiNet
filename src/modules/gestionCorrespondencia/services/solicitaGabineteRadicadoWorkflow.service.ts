import clienteApi from "../../../api/Clienteaxios";
import type { SolicitaGabineteRadicadoWorkflowResponse } from "../types/solicitaGabineteRadicadoWorkflow.types";

export const SOLICITA_GABINETE_POR_TAREA_WORKFLOW_ENDPOINT = (
  idTareaWorkflow: number,
) => `/api/workflow/ruta-trabajo/tareas/${idTareaWorkflow}/gabinete`;

export const SOLICITA_GABINETE_POR_RADICADO_WORKFLOW_ENDPOINT = (
  consecutivoRadicado: string,
) => `/api/workflow/ruta-trabajo/radicados/${encodeURIComponent(consecutivoRadicado)}/gabinete`;

export async function getSolicitaGabinetePorTareaWorkflow(
  idTareaWorkflow: number,
): Promise<SolicitaGabineteRadicadoWorkflowResponse> {
  const { data } = await clienteApi.get<SolicitaGabineteRadicadoWorkflowResponse>(
    SOLICITA_GABINETE_POR_TAREA_WORKFLOW_ENDPOINT(idTareaWorkflow),
  );
  return data;
}

export async function getSolicitaGabinetePorRadicadoWorkflow(
  consecutivoRadicado: string,
): Promise<SolicitaGabineteRadicadoWorkflowResponse> {
  const { data } = await clienteApi.get<SolicitaGabineteRadicadoWorkflowResponse>(
    SOLICITA_GABINETE_POR_RADICADO_WORKFLOW_ENDPOINT(consecutivoRadicado),
  );
  return data;
}

