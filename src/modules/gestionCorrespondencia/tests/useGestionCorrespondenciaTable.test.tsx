import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { act, renderHook, waitFor } from "@testing-library/react";
import React, { type ReactNode } from "react";
import { describe, expect, it, vi, beforeEach } from "vitest";
import { useGestionCorrespondenciaTable } from "../hooks/useGestionCorrespondenciaTable";
import * as dynamicUiTableService from "../../../app/Components/UI/AppTable/services/dynamicUiTable.service";

vi.mock("../../../app/Components/UI/AppTable/services/dynamicUiTable.service", async () => {
  const actual = await vi.importActual<
    typeof import("../../../app/Components/UI/AppTable/services/dynamicUiTable.service")
  >("../../../app/Components/UI/AppTable/services/dynamicUiTable.service");

  return {
    ...actual,
    getDynamicTable: vi.fn(),
  };
});

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });

  return ({ children }: { children: ReactNode }) =>
    React.createElement(QueryClientProvider, { client: queryClient }, children);
};

describe("[SPEC:IMPLEMENTACION-LISTA-GESTION-CORRESPONDENCIA] useGestionCorrespondenciaTable", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("maps the dynamic query result to AppTable rows and columns", async () => {
    vi.mocked(dynamicUiTableService.getDynamicTable).mockResolvedValue({
      success: true,
      message: "OK",
      data: {
        TableId: "workflowInboxgestion",
        Columns: [
          {
            DataIndex: "RADICADO",
            HeaderName: "Radicado",
            Visible: true,
            Sortable: true,
            Filterable: true,
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
          PageSize: 10,
          Total: 7,
        },
      },
      errors: [],
    });

    const { result } = renderHook(() => useGestionCorrespondenciaTable(), {
      wrapper: createWrapper(),
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(dynamicUiTableService.getDynamicTable).toHaveBeenCalledWith(
      expect.objectContaining({
        TableId: "workflowInboxgestion",
        Page: 1,
        PageSize: 10,
        SortField: "fecha_inicio",
        SortDir: "DESC",
        IncludeConfig: true,
      }),
    );
    expect(result.current.rows).toEqual([
      {
        id: "924",
        RADICADO: "2500456700023",
      },
    ]);
    expect(result.current.columns).toEqual([
      expect.objectContaining({
        field: "RADICADO",
        headerName: "Radicado",
      }),
    ]);
    expect(result.current.total).toBe(7);
    expect(result.current.pageSize).toBe(10);
    expect(result.current.queryState).toEqual(
      expect.objectContaining({
        page: 1,
        pageSize: 10,
        search: "",
        sortField: "fecha_inicio",
        sortDir: "desc",
      }),
    );
    expect(result.current.hasLoadedOnce).toBe(true);

    act(() => {
      result.current.onQueryChange({ search: "radicado" });
    });

    expect(result.current.queryState.search).toBe("radicado");
  });
});
