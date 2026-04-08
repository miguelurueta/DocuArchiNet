import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { beforeEach, describe, expect, it, vi } from "vitest";
import GestionCorrespondencia from "../pages/GestionCorrespondencia";
import type { GestionCorrespondenciaTableResult } from "../hooks/useGestionCorrespondenciaTable";
import * as workflowInboxAutocompleteHook from "../hooks/useWorkflowInboxAutocomplete";

vi.mock("../../../app/Components/UI/AppTable/AppTable", () => ({
  default: ({
    paginationMode,
    layoutMode,
    rowSelection,
    responsivePresentation,
  }: {
    paginationMode?: string;
    layoutMode?: string;
    rowSelection?: string;
    responsivePresentation?: { enabled?: boolean; cardsBelow?: number };
  }) => (
    <div
      data-testid="mock-app-table"
      data-pagination-mode={paginationMode}
      data-layout-mode={layoutMode}
      data-row-selection={rowSelection}
      data-responsive-enabled={responsivePresentation?.enabled ? "true" : "false"}
      data-cards-below={responsivePresentation?.cardsBelow}
    >
      Mocked AppTable
    </div>
  ),
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

describe("GestionCorrespondencia [SPEC:APPTABLE-EXPORT-18] [SPEC:APPTABLE-EXPORT-21] [SPEC:22-FE-INTEGRAR-APPTABLEEXPORT-CON-API-APPTABLE-EXPORT-MD] [SPEC:refinar-apptablequerywrapper]", () => {
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

  it("compone AppTableQueryWrapper con AppTable en server mode y ubica exportacion en paginationActions", () => {
    const table = createTable();

    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    expect(screen.getByTestId("app-table-query-wrapper")).toBeInTheDocument();
    expect(screen.getByTestId("mock-app-table")).toHaveAttribute(
      "data-pagination-mode",
      "server",
    );
    expect(screen.getByTestId("mock-app-table")).toHaveAttribute("data-layout-mode", "fill");
    expect(screen.getByTestId("mock-app-table")).toHaveAttribute("data-row-selection", "single");
    expect(screen.getByTestId("mock-app-table")).toHaveAttribute(
      "data-responsive-enabled",
      "true",
    );
    expect(screen.getByTestId("mock-app-table")).toHaveAttribute("data-cards-below", "768");
    expect(screen.getByTestId("app-table-pagination-actions")).toContainElement(
      screen.getByRole("button", { name: /Exportar/i }),
    );
    expect(screen.getByRole("button", { name: /Actualizar/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Abrir respuesta contextual/i })).toBeInTheDocument();
    expect(screen.getByRole("combobox", { name: "Buscar tareas workflow" })).toBeInTheDocument();
    expect(screen.queryByRole("combobox", { name: "Buscar en la tabla" })).not.toBeInTheDocument();
  });

  it("escribe contra autocomplete sin disparar la tabla por cada tecla", () => {
    const table = createTable();

    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    const searchInputs = screen.getAllByRole("combobox", {
      name: "Buscar tareas workflow",
    });
    expect(searchInputs).toHaveLength(1);

    fireEvent.change(searchInputs[0], { target: { value: "radicado" } });

    expect(
      vi.mocked(workflowInboxAutocompleteHook.useWorkflowInboxAutocomplete).mock.results[0]?.value
        ?.setSearchText,
    ).toHaveBeenCalledWith("radicado");
    expect(table.onQueryChange).not.toHaveBeenCalled();
    expect(screen.queryByRole("combobox", { name: "Buscar en la tabla" })).not.toBeInTheDocument();
  });

  it("limpia el buscador usando el flujo de queryState", () => {
    const table = createTable();
    table.queryState.search = "radicado";

    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    expect(screen.getByRole("combobox", { name: "Buscar tareas workflow" })).toHaveValue(
      "radicado",
    );

    fireEvent.click(screen.getByRole("button", { name: "Limpiar" }));

    expect(
      vi.mocked(workflowInboxAutocompleteHook.useWorkflowInboxAutocomplete).mock.results[0]?.value
        ?.clear,
    ).toHaveBeenCalledTimes(1);
    expect(table.onQueryChange).toHaveBeenCalledWith({ search: "" });
  });

  it("aplica búsqueda real por Enter y por el icono de buscar", () => {
    const table = createTable();

    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    const input = screen.getByRole("combobox", { name: "Buscar tareas workflow" });

    fireEvent.change(input, { target: { value: "radicado" } });
    fireEvent.keyDown(input, { key: "Enter" });
    fireEvent.click(screen.getByRole("button", { name: "Buscar" }));

    expect(table.onQueryChange).toHaveBeenNthCalledWith(1, { search: "radicado" });
    expect(table.onQueryChange).toHaveBeenNthCalledWith(2, { search: "radicado" });
  });

  it("aplica búsqueda real al seleccionar una sugerencia", async () => {
    const table = createTable();

    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    const input = screen.getByRole("combobox", { name: "Buscar tareas workflow" });

    fireEvent.change(input, { target: { value: "rad" } });
    fireEvent.click(await screen.findByText("Radicado sugerido"));

    expect(table.onQueryChange).toHaveBeenCalledWith({ search: "RAD-1" });
  });

  it("usa las acciones del hook para refresh y navegación secundaria", () => {
    const table = createTable();

    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    fireEvent.click(screen.getByRole("button", { name: /Actualizar/i }));
    fireEvent.click(screen.getByRole("button", { name: /Abrir respuesta contextual/i }));

    expect(table.onQueryChange).not.toHaveBeenCalled();
    expect(table.refetch).toHaveBeenCalledTimes(1);
  });

  it("expone formatos ejecutivos sobre allMatching y mantiene la tabla visible durante la exportacion backend", async () => {
    const table = createTable();

    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    fireEvent.click(screen.getByRole("button", { name: /Exportar/i }));

    expect((await screen.findAllByText("Página actual")).length).toBeGreaterThan(0);
    expect(screen.getAllByText("Seleccionados (sin selección)").length).toBeGreaterThan(0);
    expect(screen.getAllByText("Todos los resultados").length).toBeGreaterThan(0);
    expect(screen.queryByText("Todos los cargados")).not.toBeInTheDocument();
    expect(screen.getByText("Exportar en Excel")).toBeInTheDocument();
    expect(screen.getByText("Exportar en PDF")).toBeInTheDocument();
    expect(screen.getByTestId("mock-app-table")).toBeInTheDocument();
  });
});
