import clienteApi from "../../../../../api/Clienteaxios";
import type { ApiResponse, DynamicUiTableDto } from "../types/dynamicUiTable.types";

export const DEFAULT_DYNAMIC_UI_TABLE_ENDPOINT = "/api/workflowInboxgestion/inboxgestion";

export async function getDynamicTable<TRequest>(
  request: TRequest,
): Promise<ApiResponse<DynamicUiTableDto | null>>;
export async function getDynamicTable<TRequest>(
  endpoint: string,
  request: TRequest,
): Promise<ApiResponse<DynamicUiTableDto | null>>;
export async function getDynamicTable<TRequest>(
  endpointOrRequest: string | TRequest,
  requestArg?: TRequest,
): Promise<ApiResponse<DynamicUiTableDto | null>> {
  const endpoint =
    typeof endpointOrRequest === "string"
      ? endpointOrRequest
      : DEFAULT_DYNAMIC_UI_TABLE_ENDPOINT;
  const request =
    typeof endpointOrRequest === "string"
      ? requestArg
      : endpointOrRequest;

  const response = await clienteApi.post<ApiResponse<DynamicUiTableDto | null>>(
    endpoint,
    request,
  );

  return response.data;
}

export const createDynamicTableService = (endpoint: string) =>
  async <TRequest>(request: TRequest): Promise<ApiResponse<DynamicUiTableDto | null>> =>
    getDynamicTable(endpoint, request);
