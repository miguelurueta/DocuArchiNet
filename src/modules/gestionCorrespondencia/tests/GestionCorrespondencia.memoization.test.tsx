import { render } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import GestionCorrespondencia from "../pages/GestionCorrespondencia";
import type { GestionCorrespondenciaTableResult } from "../hooks/useGestionCorrespondenciaTable";
import * as workflowInboxAutocompleteHook from "../hooks/useWorkflowInboxAutocomplete";

const exportSpy = vi.fn();
const tableSpy = vi.fn();

vi.mock("../../../app/Components/UI/AppTable/AppTableExport", () => ({
  AppTableExport: (props: unknown) => {
    exportSpy(props);
    return <div data-testid="mock-app-table-export">Mocked AppTableExport</div>;
  },
}));

vi.mock("../../../app/Components/UI/AppTable/AppTable", () => ({
  default: (props: unknown) => {
    tableSpy(props);
    return <div data-testid="mock-app-table">Mocked AppTable</div>;
  },
}));

vi.mock("../hooks/useWorkflowInboxAutocomplete", () => ({
  useWorkflowInboxAutocomplete: vi.fn(),
}));

const createTable = (): GestionCorrespondenciaTableResult => ({
  rows: [{ id: "924", RADICADO: "2500456700023" }],
  columns: [{ field: "RADICADO", headerName: "Radicado" }],
  total: 7,
  page: 1,
  pageSize: 25,
  queryState: {
    page: 1,
    pageSize: 25,
    search: "",
    structuredFilters: [],
    sortField: "fecha_inicio",
    sortDir: "desc",
    searchType: undefined,
  },
  onQueryChange: vi.fn(),
  category: undefined,
  loading: false,
  error: null,
  isEmpty: false,
  hasLoadedOnce: true,
  setCategory: vi.fn(),
  refetch: vi.fn(),
  getAllMatchingRows: vi.fn().mockResolvedValue([
    { id: "924", RADICADO: "2500456700023" },
  ]),
  getBackendExportFile: vi.fn().mockResolvedValue({
    blob: new Blob(["xlsx"], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    }),
    fileName: "gestion.xlsx",
  }),
});

describe("GestionCorrespondencia memoization", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(
      workflowInboxAutocompleteHook.useWorkflowInboxAutocomplete,
    ).mockReturnValue({
      items: [{ value: "RAD-1", label: "Radicado sugerido" }],
      loading: false,
      error: null,
      setSearchText: vi.fn(),
      clear: vi.fn(),
    });
  });

  it("keeps export dataSource and responsivePresentation stable across equivalent rerenders", () => {
    const table = createTable();
    const { rerender } = render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    const firstExportProps = exportSpy.mock.calls.at(-1)?.[0] as {
      dataSource: unknown;
      reportMeta: unknown;
    };
    const firstTableProps = tableSpy.mock.calls.at(-1)?.[0] as {
      responsivePresentation: unknown;
      onActionTriggered: unknown;
      onCellClicked: unknown;
    };

    rerender(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    const secondExportProps = exportSpy.mock.calls.at(-1)?.[0] as {
      dataSource: unknown;
      reportMeta: unknown;
    };
    const secondTableProps = tableSpy.mock.calls.at(-1)?.[0] as {
      responsivePresentation: unknown;
      onActionTriggered: unknown;
      onCellClicked: unknown;
    };

    expect(secondExportProps.dataSource).toBe(firstExportProps.dataSource);
    expect(secondExportProps.reportMeta).toBe(firstExportProps.reportMeta);
    expect(secondTableProps.responsivePresentation).toBe(firstTableProps.responsivePresentation);
    expect(secondTableProps.onActionTriggered).toBe(firstTableProps.onActionTriggered);
    expect(secondTableProps.onCellClicked).toBe(firstTableProps.onCellClicked);
  });
});
