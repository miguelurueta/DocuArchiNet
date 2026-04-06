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
    exportAppTableFile: vi.fn(),
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

describe("[SPEC:IMPLEMENTACION-LISTA-GESTION-CORRESPONDENCIA] [SPEC:APPTABLE-EXPORT-21] [SPEC:refinar-apptablequerywrapper] useGestionCorrespondenciaTable", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(dynamicUiTableService.exportAppTableFile).mockResolvedValue({
      blob: new Blob(["export"], { type: "text/csv;charset=utf-8;" }),
      fileName: "gestion.csv",
      contentType: "text/csv;charset=utf-8;",
    });
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
          PageSize: 25,
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
        PageSize: 25,
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
    expect(result.current.pageSize).toBe(25);
    expect(result.current.queryState).toEqual(
      expect.objectContaining({
        page: 1,
        pageSize: 25,
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

  it("builds an allMatching request from the active query without coupling export to the visible page", async () => {
    vi.mocked(dynamicUiTableService.getDynamicTable)
      .mockResolvedValueOnce({
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
            Page: 2,
            PageSize: 25,
            Total: 7,
          },
        },
        errors: [],
      })
      .mockResolvedValueOnce({
        success: true,
        message: "OK",
        data: {
          TableId: "workflowInboxgestion",
          Columns: [],
          Rows: [
            {
              Id: "924",
              Values: {
                RADICADO: "2500456700023",
              },
            },
            {
              Id: "925",
              Values: {
                RADICADO: "2500456700024",
              },
            },
          ],
          Pagination: {
            Page: 1,
            PageSize: 7,
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

    let rows: Awaited<ReturnType<typeof result.current.getAllMatchingRows>> = [];
    await act(async () => {
      rows = await result.current.getAllMatchingRows();
    });

    expect(dynamicUiTableService.getDynamicTable).toHaveBeenLastCalledWith(
      expect.objectContaining({
        TableId: "workflowInboxgestion",
        Page: 1,
        PageSize: 25,
        SortField: "fecha_inicio",
        SortDir: "DESC",
        IncludeConfig: false,
      }),
    );
    expect(rows).toEqual([
      {
        id: "924",
        RADICADO: "2500456700023",
      },
      {
        id: "925",
        RADICADO: "2500456700024",
      },
    ]);
  });

  it("maps the active query state to the backend export contract", async () => {
    vi.mocked(dynamicUiTableService.getDynamicTable).mockResolvedValue({
      success: true,
      message: "OK",
      data: {
        TableId: "workflowInboxgestion",
        Columns: [],
        Rows: [
          {
            Id: "924",
            Values: {
              RADICADO: "2500456700023",
            },
          },
        ],
        Pagination: {
          Page: 2,
          PageSize: 25,
          Total: 25,
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

    act(() => {
      result.current.onQueryChange({
        search: "radicado",
        structuredFilters: [
          {
            field: "estado",
            operator: "eq",
            value: "Pendiente",
          },
        ],
      });
    });

    await act(async () => {
      await result.current.getBackendExportFile({
        columns: [{ field: "RADICADO", headerName: "Radicado" }],
        format: "xlsx",
        mode: "allMatching",
        reportMeta: {
          reportName: "Bandeja de gestion correspondencia",
          generatedBy: "DocuArchiCore",
          moduleName: "Gestion Correspondencia",
          reportType: "Operativo",
          generatedAt: "2026-04-05T05:00:00.000Z",
          rowCount: 1,
          description: "Exportacion desde la bandeja operativa",
          companyImageAsset: "public/branding/reports/company-report-logo.png",
        },
      });
    });

    expect(dynamicUiTableService.exportAppTableFile).toHaveBeenCalledWith(
      expect.objectContaining({
        ColumnMode: 2,
        EstadoTramite: "",
        SearchType: 1,
        Search: "radicado",
        SortField: "fecha_inicio",
        SortDir: "DESC",
        Page: 1,
        PageSize: 25,
        Format: "xlsx",
        ExportMode: "allMatching",
        ReportTitle: "Bandeja de gestion correspondencia",
        StructuredFilters: [
          {
            Field: "estado",
            Operator: "eq",
            Value: "Pendiente",
            ValueFrom: undefined,
            ValueTo: undefined,
          },
        ],
      }),
    );
  });
});
