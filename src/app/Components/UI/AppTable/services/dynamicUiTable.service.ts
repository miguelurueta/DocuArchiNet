import clienteApi from "../../../../../api/Clienteaxios";
import type { ApiResponse, DynamicUiTableDto } from "../types/dynamicUiTable.types";

export const DEFAULT_DYNAMIC_UI_TABLE_ENDPOINT = "/api/workflowInboxgestion/inboxgestion";
export const DEFAULT_APP_TABLE_EXPORT_ENDPOINT = "/api/AppTable/export";

export type AppTableExportApiFile = {
  blob: Blob;
  fileName?: string;
  contentType?: string;
};

const resolveHeaderValue = (
  headers: Record<string, unknown> | undefined,
  name: string,
): string | undefined => {
  const value = headers?.[name] ?? headers?.[name.toLowerCase()] ?? headers?.[name.toUpperCase()];
  return typeof value === "string" && value.trim().length > 0 ? value.trim() : undefined;
};

const extractFileNameFromContentDisposition = (
  contentDisposition: string | undefined,
): string | undefined => {
  if (!contentDisposition) {
    return undefined;
  }

  const utf8Match = /filename\*=UTF-8''([^;]+)/i.exec(contentDisposition);
  if (utf8Match?.[1]) {
    return decodeURIComponent(utf8Match[1]);
  }

  const quotedMatch = /filename="([^"]+)"/i.exec(contentDisposition);
  if (quotedMatch?.[1]) {
    return quotedMatch[1];
  }

  const plainMatch = /filename=([^;]+)/i.exec(contentDisposition);
  return plainMatch?.[1]?.trim();
};

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

export async function exportAppTableFile<TRequest>(
  request: TRequest,
): Promise<AppTableExportApiFile>;
export async function exportAppTableFile<TRequest>(
  endpoint: string,
  request: TRequest,
): Promise<AppTableExportApiFile>;
export async function exportAppTableFile<TRequest>(
  endpointOrRequest: string | TRequest,
  requestArg?: TRequest,
): Promise<AppTableExportApiFile> {
  const endpoint =
    typeof endpointOrRequest === "string"
      ? endpointOrRequest
      : DEFAULT_APP_TABLE_EXPORT_ENDPOINT;
  const request =
    typeof endpointOrRequest === "string"
      ? requestArg
      : endpointOrRequest;

  const response = await clienteApi.post<Blob>(endpoint, request, {
    responseType: "blob",
  });

  const contentDisposition = resolveHeaderValue(response.headers, "content-disposition");
  const contentType = resolveHeaderValue(response.headers, "content-type");
  const blob =
    response.data instanceof Blob
      ? response.data
      : new Blob([response.data], {
          type: contentType ?? "application/octet-stream",
        });

  return {
    blob,
    fileName: extractFileNameFromContentDisposition(contentDisposition),
    contentType,
  };
}

export const createAppTableExportService = (endpoint: string) =>
  async <TRequest>(request: TRequest): Promise<AppTableExportApiFile> =>
    exportAppTableFile(endpoint, request);
