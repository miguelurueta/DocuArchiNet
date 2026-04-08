import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../app/Components/UI/AppTable/types/dynamicUiTable.types";
import type {
  WorkflowInboxAutocompleteBackendItem,
  WorkflowInboxAutocompleteBackendResponse,
  WorkflowInboxAutocompleteItem,
  WorkflowInboxAutocompleteRequest,
} from "../types/workflowInboxAutocomplete.types";

export const WORKFLOW_INBOX_AUTOCOMPLETE_ENDPOINT =
  "/api/workflowInboxgestion/inboxgestion/autocomplete";

const normalizeBackendItem = (
  item: WorkflowInboxAutocompleteBackendItem,
): WorkflowInboxAutocompleteItem | null => {
  const value = item.value ?? item.Value;

  if (typeof value !== "string" || value.trim().length === 0) {
    return null;
  }

  const label = item.label ?? item.Label;

  return {
    value: value.trim(),
    ...(typeof label === "string" && label.trim().length > 0
      ? { label: label.trim() }
      : {}),
  };
};

const resolveItems = (
  response: ApiResponse<
    WorkflowInboxAutocompleteBackendResponse | WorkflowInboxAutocompleteBackendItem[]
  >,
): WorkflowInboxAutocompleteBackendItem[] => {
  const payload = response.data ?? response.Data;

  if (Array.isArray(payload)) {
    return payload;
  }

  return payload?.items ?? payload?.Items ?? [];
};

export const getWorkflowInboxAutocomplete = async (
  request: WorkflowInboxAutocompleteRequest,
): Promise<WorkflowInboxAutocompleteItem[]> => {
  const response = await clienteApi.post<
    ApiResponse<
      WorkflowInboxAutocompleteBackendResponse | WorkflowInboxAutocompleteBackendItem[]
    >
  >(WORKFLOW_INBOX_AUTOCOMPLETE_ENDPOINT, request);

  return resolveItems(response.data)
    .map(normalizeBackendItem)
    .filter((item): item is WorkflowInboxAutocompleteItem => item !== null);
};
