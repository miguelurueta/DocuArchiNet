import clienteApi from "../../../../../api/Clienteaxios";
import type { ApiResponse } from "../types/dynamicUiTable.types";
import type {
  DynamicUiActionExecutionRequest,
} from "../types/dynamicUiTableAction.types";

export const DEFAULT_DYNAMIC_UI_ACTION_ENDPOINT = "/api/dynamic-ui-table/actions/execute";

export async function executeDynamicUiAction(
  request: DynamicUiActionExecutionRequest,
): Promise<ApiResponse<unknown>>;
export async function executeDynamicUiAction(
  endpoint: string,
  request: DynamicUiActionExecutionRequest,
): Promise<ApiResponse<unknown>>;
export async function executeDynamicUiAction(
  endpointOrRequest: string | DynamicUiActionExecutionRequest,
  requestArg?: DynamicUiActionExecutionRequest,
): Promise<ApiResponse<unknown>> {
  const endpoint =
    typeof endpointOrRequest === "string"
      ? endpointOrRequest
      : DEFAULT_DYNAMIC_UI_ACTION_ENDPOINT;
  const request =
    typeof endpointOrRequest === "string"
      ? requestArg
      : endpointOrRequest;

  const response = await clienteApi.post<ApiResponse<unknown>>(endpoint, request);

  return response.data;
}

export const createDynamicUiActionService = (endpoint?: string) =>
  async (request: DynamicUiActionExecutionRequest): Promise<ApiResponse<unknown>> =>
    endpoint ? executeDynamicUiAction(endpoint, request) : executeDynamicUiAction(request);
