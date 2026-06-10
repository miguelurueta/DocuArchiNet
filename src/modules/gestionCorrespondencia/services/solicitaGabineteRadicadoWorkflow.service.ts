import clienteApi from "../../../api/Clienteaxios";
import type { SolicitaGabineteRadicadoWorkflowResponse } from "../types/solicitaGabineteRadicadoWorkflow.types";

type GabineteWorkflowRequestOptions = {
  signal?: AbortSignal;
};

export const SOLICITA_GABINETE_POR_TAREA_WORKFLOW_ENDPOINT = (
  idTareaWorkflow: number,
) => `/api/workflow/ruta-trabajo/tareas/${idTareaWorkflow}/gabinete`;

export const SOLICITA_GABINETE_POR_RADICADO_WORKFLOW_ENDPOINT = (
  consecutivoRadicado: string,
) => `/api/workflow/ruta-trabajo/radicados/${encodeURIComponent(consecutivoRadicado)}/gabinete`;

export async function getSolicitaGabinetePorTareaWorkflow(
  idTareaWorkflow: number,
  options: GabineteWorkflowRequestOptions = {},
): Promise<SolicitaGabineteRadicadoWorkflowResponse> {
  const { data } = await clienteApi.get<SolicitaGabineteRadicadoWorkflowResponse>(
    SOLICITA_GABINETE_POR_TAREA_WORKFLOW_ENDPOINT(idTareaWorkflow),
    { signal: options.signal },
  );
  return data;
}

export async function getSolicitaGabinetePorRadicadoWorkflow(
  consecutivoRadicado: string,
  options: GabineteWorkflowRequestOptions = {},
): Promise<SolicitaGabineteRadicadoWorkflowResponse> {
  const { data } = await clienteApi.get<SolicitaGabineteRadicadoWorkflowResponse>(
    SOLICITA_GABINETE_POR_RADICADO_WORKFLOW_ENDPOINT(consecutivoRadicado),
    { signal: options.signal },
  );
  return data;
}

