import { useMutation } from "@tanstack/react-query";
import { createDynamicUiActionService } from "../services/dynamicUiAction.service";
import type { ApiResponse, DynamicUiUnknownRecord } from "../types/dynamicUiTable.types";
import type {
  DynamicUiActionExecutionRequest,
  DynamicUiActionExecutionResult,
  DynamicUiActionHookResult,
  UseDynamicUiTableActionsParams,
} from "../types/dynamicUiTableAction.types";
import { resolveDynamicUiActionBehavior } from "../utils/dynamicUiActionBehaviorResolver";
import { evaluateDynamicUiActionAvailability } from "../utils/dynamicUiActionGuard";
import { buildDynamicUiActionPayload } from "../utils/dynamicUiActionPayloadBuilder";
import { resolveDynamicUiActionPresentation } from "../utils/dynamicUiActionPresentationResolver";

const isSuccessfulResponse = (response: ApiResponse<unknown>): boolean => {
  if (typeof response.success === "boolean") {
    return response.success;
  }

  if (typeof response.Success === "boolean") {
    return response.Success;
  }

  return true;
};

const resolveMessage = (response: ApiResponse<unknown>): string | undefined =>
  response.message?.trim() ||
  response.Message?.trim() ||
  response.errors?.find((error) => error?.message)?.message?.trim() ||
  response.Errors?.find((error) => error?.message)?.message?.trim() ||
  undefined;

const resolveData = (response: ApiResponse<unknown>): Record<string, unknown> | null => {
  const rawData = response.data ?? response.Data ?? null;

  if (rawData == null) {
    return null;
  }

  if (typeof rawData !== "object" || Array.isArray(rawData)) {
    return null;
  }

  return { ...(rawData as DynamicUiUnknownRecord) };
};

const normalizeError = (error: unknown): Error => {
  if (error instanceof Error) {
    return error;
  }

  if (
    typeof error === "object" &&
    error !== null &&
    "message" in error &&
    typeof error.message === "string" &&
    error.message.trim().length > 0
  ) {
    return new Error(error.message);
  }

  return new Error("Dynamic UI action execution failed");
};

const normalizeActionResponse = (
  response: ApiResponse<unknown>,
): DynamicUiActionExecutionResult => ({
  success: isSuccessfulResponse(response),
  message: resolveMessage(response),
  data: resolveData(response),
  rawResponse: response,
});

export function useDynamicUiTableActions(
  params: UseDynamicUiTableActionsParams = {},
): DynamicUiActionHookResult {
  const resolvedExecuteActionFn =
    params.executeActionFn ?? createDynamicUiActionService(params.endpoint);

  const mutation = useMutation<DynamicUiActionExecutionResult, Error, DynamicUiActionExecutionRequest>({
    mutationFn: async (input) => {
      const response = await resolvedExecuteActionFn(input);
      return normalizeActionResponse(response);
    },
    retry: false,
  });

  return {
    executeAction: async (input: DynamicUiActionExecutionRequest) => mutation.mutateAsync(input),
    buildActionPayload: (context, action, manualPayload) =>
      buildDynamicUiActionPayload(action, context, manualPayload),
    evaluateActionAvailability: (action, context) =>
      evaluateDynamicUiActionAvailability(action, context),
    resolveActionBehavior: (action) => resolveDynamicUiActionBehavior(action),
    resolveActionPresentation: (action) => resolveDynamicUiActionPresentation(action),
    isExecutingAction: mutation.isPending,
    actionError: mutation.error ? normalizeError(mutation.error) : null,
    lastActionResult: mutation.data ?? null,
  };
}
