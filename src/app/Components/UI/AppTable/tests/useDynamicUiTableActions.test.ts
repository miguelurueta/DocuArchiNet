import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import React, { type ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useDynamicUiTableActions } from "../hooks/useDynamicUiTableActions";
import type { ApiResponse } from "../types/dynamicUiTable.types";
import type { DynamicUiActionExecutionRequest } from "../types/dynamicUiTableAction.types";

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      mutations: {
        retry: false,
      },
    },
  });

  return ({ children }: { children: ReactNode }) => (
    React.createElement(QueryClientProvider, { client: queryClient }, children)
  );
};

const sampleAction = {
  actionId: "gestionar_tramite",
  label: "Gestionar",
  placement: "row",
  presentation: "icon_button",
  behavior: "client_event",
  request: {
    RowIdField: "id_tarea",
    PayloadFields: {
      id_tarea: "id_tarea",
    },
  },
};

describe("[SPEC:CREA-ACTION-LAYER-AG-GRID] useDynamicUiTableActions", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("executes a mutation and returns a normalized result shape", async () => {
    const executeActionFn = vi.fn<
      (request: DynamicUiActionExecutionRequest) => Promise<ApiResponse<unknown>>
    >().mockResolvedValue({
      success: true,
      message: "OK",
      data: {
        processed: true,
      },
      errors: [],
    });

    const { result } = renderHook(
      () =>
        useDynamicUiTableActions({
          executeActionFn,
        }),
      {
        wrapper: createWrapper(),
      },
    );

    const executionResult = await result.current.executeAction({
      tableId: "workflowInboxgestion",
      actionId: "gestionar_tramite",
      rowId: "924",
    });

    expect(executeActionFn).toHaveBeenCalledWith({
      tableId: "workflowInboxgestion",
      actionId: "gestionar_tramite",
      rowId: "924",
    });
    expect(executionResult).toEqual({
      success: true,
      message: "OK",
      data: {
        processed: true,
      },
      rawResponse: {
        success: true,
        message: "OK",
        data: {
          processed: true,
        },
        errors: [],
      },
    });

    await waitFor(() => {
      expect(result.current.lastActionResult).toEqual(executionResult);
    });

    expect(result.current.actionError).toBeNull();
    expect(result.current.isExecutingAction).toBe(false);
  });

  it("exposes actionError when the mutation fails", async () => {
    const executeActionFn = vi.fn().mockRejectedValue(new Error("network"));

    const { result } = renderHook(
      () =>
        useDynamicUiTableActions({
          executeActionFn,
        }),
      {
        wrapper: createWrapper(),
      },
    );

    await expect(
      result.current.executeAction({
        tableId: "workflowInboxgestion",
        actionId: "gestionar_tramite",
      }),
    ).rejects.toThrow("network");

    await waitFor(() => {
      expect(result.current.actionError?.message).toBe("network");
    });

    expect(result.current.lastActionResult).toBeNull();
    expect(result.current.isExecutingAction).toBe(false);
  });

  it("exposes pure helpers without coupling the hook to UI logic", () => {
    const { result } = renderHook(() => useDynamicUiTableActions(), {
      wrapper: createWrapper(),
    });

    expect(
      result.current.buildActionPayload(
        {
          row: {
            id: "924",
            data: {
              id_tarea: 924,
            },
          },
          userClaims: ["tramites.gestionar"],
        },
        sampleAction,
        {
          extra: true,
        },
      ),
    ).toEqual({
      id_tarea: 924,
      rowId: 924,
      extra: true,
    });

    expect(
      result.current.evaluateActionAvailability(
        {
          ...sampleAction,
          requiredClaimsAny: ["tramites.gestionar"],
        },
        {
          userClaims: ["tramites.gestionar"],
        },
      ),
    ).toEqual({
      isVisible: true,
      isEnabled: true,
      reasons: undefined,
    });

    expect(result.current.resolveActionBehavior(sampleAction)).toEqual({
      kind: "client_event",
      rawValue: "client_event",
      isKnown: true,
      config: undefined,
    });

    expect(result.current.resolveActionPresentation(sampleAction)).toEqual({
      kind: "icon_button",
      rawValue: "icon_button",
      isKnown: true,
      config: undefined,
    });
  });
});
