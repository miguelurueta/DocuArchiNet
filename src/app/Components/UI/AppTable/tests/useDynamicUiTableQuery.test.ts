import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { act, renderHook, waitFor } from "@testing-library/react";
import React, { type ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useDynamicUiTableQuery } from "../hooks/useDynamicUiTableQuery";
import type { ApiResponse, DynamicUiTableDto } from "../types/dynamicUiTable.types";
import type { DynamicTableQueryInput } from "../types/dynamicUiTableQuery.types";

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });

  return ({ children }: { children: ReactNode }) => (
    React.createElement(QueryClientProvider, { client: queryClient }, children)
  );
};

const baseInput: DynamicTableQueryInput = {
  tableId: "workflowInboxgestion",
  page: 1,
  pageSize: 25,
  search: "tramite",
  sortField: "fecha_inicio",
  sortDirection: "desc",
  includeConfig: true,
};

const createResponse = (
  data: DynamicUiTableDto | null,
  overrides?: Partial<ApiResponse<DynamicUiTableDto | null>>,
): ApiResponse<DynamicUiTableDto | null> => ({
  success: true,
  message: "OK",
  data,
  errors: [],
  ...overrides,
});

describe("[SPEC:CREA-QUERY-AG-GRID-FASE3] useDynamicUiTableQuery", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("maps a successful response to the shared AppTable model", async () => {
    const requestMapper = vi.fn((input: DynamicTableQueryInput) => ({
      TableId: input.tableId,
      Page: input.page,
    }));
    const queryFn = vi.fn().mockResolvedValue(
      createResponse({
        TableId: "workflowInboxgestion",
        MenuActions: [
          {
            ActionId: "reasignar_tramite",
            Label: "Reasignar trámite",
            Presentation: "menu_item",
            Behavior: "api_call",
          },
        ],
        Columns: [
          {
            DataIndex: "RADICADO",
            HeaderName: "Radicado",
            Visible: true,
            Order: 1,
          },
        ],
        Rows: [
          {
            Id: "924",
            Values: {
              RADICADO: "2500456700023",
            },
          },
        ],
        Pagination: {
          Page: 1,
          PageSize: 25,
          Total: 7,
        },
      }),
    );

    const { result } = renderHook(
      () =>
        useDynamicUiTableQuery({
          input: baseInput,
          requestMapper,
          queryFn,
        }),
      {
        wrapper: createWrapper(),
      },
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(requestMapper).toHaveBeenCalledWith(baseInput);
    expect(queryFn).toHaveBeenCalledWith({
      TableId: "workflowInboxgestion",
      Page: 1,
    });
    expect(result.current.columns).toEqual([
      expect.objectContaining({
        field: "RADICADO",
        headerName: "Radicado",
      }),
    ]);
    expect(result.current.rows).toEqual([
      {
        id: "924",
        data: {
          RADICADO: "2500456700023",
        },
        meta: undefined,
      },
    ]);
    expect(result.current.total).toBe(7);
    expect(result.current.pagination).toEqual({
      page: 1,
      pageSize: 25,
    });
    expect(result.current.menuActions).toEqual([
      expect.objectContaining({
        actionId: "reasignar_tramite",
        label: "Reasignar trámite",
      }),
    ]);
    expect(result.current.isEmpty).toBe(false);
    expect(result.current.error).toBeNull();
  });

  it("treats success with null data as an empty state without error", async () => {
    const { result } = renderHook(
      () =>
        useDynamicUiTableQuery({
          input: baseInput,
          requestMapper: (input) => input,
          queryFn: async () => createResponse(null),
        }),
      {
        wrapper: createWrapper(),
      },
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.rows).toEqual([]);
    expect(result.current.columns).toEqual([]);
    expect(result.current.total).toBe(0);
    expect(result.current.error).toBeNull();
    expect(result.current.isEmpty).toBe(true);
    expect(result.current.pagination).toEqual({
      page: 1,
      pageSize: 25,
    });
  });

  it("preserves input tableId when the response omits TableId", async () => {
    const { result } = renderHook(
      () =>
        useDynamicUiTableQuery({
          input: baseInput,
          requestMapper: (input) => input,
          queryFn: async () =>
            createResponse({
              Columns: [],
              Rows: [],
              Pagination: {
                Page: 1,
                PageSize: 25,
                Total: 0,
              },
            }),
        }),
      {
        wrapper: createWrapper(),
      },
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.tableId).toBe("workflowInboxgestion");
  });

  it("prefers backend pagination values when the API normalizes the requested page", async () => {
    const { result } = renderHook(
      () =>
        useDynamicUiTableQuery({
          input: {
            ...baseInput,
            page: 99,
            pageSize: 100,
          },
          requestMapper: (input) => input,
          queryFn: async () =>
            createResponse({
              Columns: [],
              Rows: [],
              Pagination: {
                Page: 3,
                PageSize: 25,
                Total: 60,
              },
            }),
        }),
      {
        wrapper: createWrapper(),
      },
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.pagination).toEqual({
      page: 3,
      pageSize: 25,
    });
    expect(result.current.total).toBe(60);
  });

  it("surfaces success false as an Error", async () => {
    const { result } = renderHook(
      () =>
        useDynamicUiTableQuery({
          input: baseInput,
          requestMapper: (input) => input,
          queryFn: async () =>
            createResponse(null, {
              success: false,
              message: "Business failure",
            }),
        }),
      {
        wrapper: createWrapper(),
      },
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBeInstanceOf(Error);
    expect(result.current.error?.message).toBe("Business failure");
    expect(result.current.isEmpty).toBe(true);
  });

  it("surfaces transport failures through error while keeping the output stable", async () => {
    const { result } = renderHook(
      () =>
        useDynamicUiTableQuery({
          input: baseInput,
          requestMapper: (input) => input,
          queryFn: async () => {
            throw new Error("network");
          },
        }),
      {
        wrapper: createWrapper(),
      },
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBeInstanceOf(Error);
    expect(result.current.error?.message).toBe("network");
    expect(result.current.rows).toEqual([]);
    expect(result.current.columns).toEqual([]);
    expect(result.current.total).toBe(0);
    expect(result.current.isEmpty).toBe(true);
  });

  it("does not expose loading when the query is disabled", () => {
    const queryFn = vi.fn();

    const { result } = renderHook(
      () =>
        useDynamicUiTableQuery({
          input: baseInput,
          requestMapper: (input) => input,
          queryFn,
          enabled: false,
        }),
      {
        wrapper: createWrapper(),
      },
    );

    expect(queryFn).not.toHaveBeenCalled();
    expect(result.current.loading).toBe(false);
    expect(result.current.error).toBeNull();
    expect(result.current.rows).toEqual([]);
    expect(result.current.columns).toEqual([]);
  });

  it("keeps previous rows and total while fetching a new server page", async () => {
    let resolveSecondResponse: ((value: ApiResponse<DynamicUiTableDto | null>) => void) | undefined;
    const queryFn = vi
      .fn()
      .mockResolvedValueOnce(
        createResponse({
          TableId: "workflowInboxgestion",
          Columns: [
            {
              DataIndex: "RADICADO",
              HeaderName: "Radicado",
              Visible: true,
              Order: 1,
            },
          ],
          Rows: [
            {
              Id: "924",
              Values: {
                RADICADO: "2500456700023",
              },
            },
          ],
          Pagination: {
            Page: 1,
            PageSize: 25,
            Total: 40,
          },
        }),
      )
      .mockImplementationOnce(
        () =>
          new Promise<ApiResponse<DynamicUiTableDto | null>>((resolve) => {
            resolveSecondResponse = resolve;
          }),
      );

    const { result, rerender } = renderHook(
      ({ input }) =>
        useDynamicUiTableQuery({
          input,
          requestMapper: (nextInput) => nextInput,
          queryFn,
        }),
      {
        initialProps: { input: baseInput },
        wrapper: createWrapper(),
      },
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.rows).toEqual([
      {
        id: "924",
        data: {
          RADICADO: "2500456700023",
        },
        meta: undefined,
      },
    ]);
    expect(result.current.total).toBe(40);

    rerender({
      input: {
        ...baseInput,
        page: 2,
      },
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(true);
    });

    expect(result.current.rows).toEqual([
      {
        id: "924",
        data: {
          RADICADO: "2500456700023",
        },
        meta: undefined,
      },
    ]);
    expect(result.current.total).toBe(40);
    expect(result.current.pagination).toEqual({
      page: 1,
      pageSize: 25,
    });

    await act(async () => {
      resolveSecondResponse?.(
        createResponse({
          TableId: "workflowInboxgestion",
          Columns: [
            {
              DataIndex: "RADICADO",
              HeaderName: "Radicado",
              Visible: true,
              Order: 1,
            },
          ],
          Rows: [
            {
              Id: "925",
              Values: {
                RADICADO: "2500456700024",
              },
            },
          ],
          Pagination: {
            Page: 2,
            PageSize: 25,
            Total: 40,
          },
        }),
      );
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.rows).toEqual([
      {
        id: "925",
        data: {
          RADICADO: "2500456700024",
        },
        meta: undefined,
      },
    ]);
    expect(result.current.pagination).toEqual({
      page: 2,
      pageSize: 25,
    });
  });
});
