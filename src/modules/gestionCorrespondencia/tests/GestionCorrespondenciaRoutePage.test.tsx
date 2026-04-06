import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import GestionCorrespondenciaRoutePage from "../pages/GestionCorrespondenciaRoutePage";

vi.mock("../hooks/useGestionCorrespondenciaTable", () => ({
  useGestionCorrespondenciaTable: vi.fn(),
}));

vi.mock("../pages/GestionCorrespondencia", () => ({
  default: () => <div>GestionCorrespondencia Page</div>,
}));

const { useGestionCorrespondenciaTable } = await import("../hooks/useGestionCorrespondenciaTable");

describe("[SPEC:IMPLEMENTACION-LISTA-GESTION-CORRESPONDENCIA] [SPEC:refinar-apptablequerywrapper] GestionCorrespondenciaRoutePage", () => {
  it("renders the screen skeleton during first load", () => {
    vi.mocked(useGestionCorrespondenciaTable).mockReturnValue({
      rows: [],
      columns: [],
      total: 0,
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
      loading: true,
      error: null,
      isEmpty: true,
      hasLoadedOnce: false,
      setCategory: vi.fn(),
      refetch: vi.fn(),
      getAllMatchingRows: vi.fn(),
      getBackendExportFile: vi.fn(),
    });

    render(<GestionCorrespondenciaRoutePage />);

    expect(screen.getByTestId("gestion-correspondencia-skeleton")).toBeInTheDocument();
  });

  it("renders an error state when the inbox query fails", () => {
    vi.mocked(useGestionCorrespondenciaTable).mockReturnValue({
      rows: [],
      columns: [],
      total: 0,
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
      error: new Error("network"),
      isEmpty: true,
      hasLoadedOnce: true,
      setCategory: vi.fn(),
      refetch: vi.fn(),
      getAllMatchingRows: vi.fn(),
      getBackendExportFile: vi.fn(),
    });

    render(<GestionCorrespondenciaRoutePage />);

    expect(
      screen.getByText(/No fue posible cargar la bandeja de gestión de correspondencia/i),
    ).toBeInTheDocument();
  });

  it("renders the page when data is available or resolved", () => {
    vi.mocked(useGestionCorrespondenciaTable).mockReturnValue({
      rows: [{ id: "924", RADICADO: "2500456700023" }],
      columns: [{ field: "RADICADO", headerName: "Radicado" }],
      total: 1,
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
      getAllMatchingRows: vi.fn(),
      getBackendExportFile: vi.fn(),
    });

    render(<GestionCorrespondenciaRoutePage />);

    expect(screen.getByText("GestionCorrespondencia Page")).toBeInTheDocument();
  });
});
