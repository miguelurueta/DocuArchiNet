import { fireEvent, render, screen } from "@testing-library/react";
import type { ColDef } from "ag-grid-community";
import { describe, expect, it, vi } from "vitest";
import { AppTableExport } from "../AppTableExport";
import { AppTableQueryWrapper } from "../AppTableQueryWrapper";
import type { AppTableQueryState } from "../types/appTableQueryState.types";
import type { AppTableRow } from "../AppTable.types";
import type { AppTableExportReportMeta } from "../AppTableExport.types";

const createQueryState = (
  overrides: Partial<AppTableQueryState> = {},
): AppTableQueryState => ({
  page: 2,
  pageSize: 25,
  search: "",
  structuredFilters: [],
  ...overrides,
});

type ExportRow = AppTableRow & {
  id: string;
  name: string;
};

const exportColumns: ColDef<ExportRow>[] = [
  { field: "name", headerName: "Nombre" },
];
const exportReportMeta: AppTableExportReportMeta = {
  reportName: "Bandeja",
  generatedBy: "DocuArchiCore",
  moduleName: "Gestion Correspondencia",
  reportType: "Operativo",
  generatedAt: "2026-04-05T05:00:00.000Z",
  rowCount: 1,
  description: "Exportacion de prueba",
  companyImageAsset: "public/branding/reports/company-report-logo.png",
};

describe("AppTableQueryWrapper [SPEC:APPTABLE-EXPORT-18] [SPEC:refinar-apptablequerywrapper] [SPEC:app-input-search]", () => {
  it("renderiza la estructura completa con rango visible y children", () => {
    render(
      <AppTableQueryWrapper
        queryState={createQueryState()}
        onQueryChange={vi.fn()}
        total={87}
        headerActions={<button type="button">Acción extra</button>}
        paginationActions={<button type="button">Exportar tabla</button>}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    expect(screen.getByTestId("app-table-query-wrapper")).toBeInTheDocument();
    expect(screen.getByText("Tabla mock")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: "Acción extra" }),
    ).toBeInTheDocument();
    expect(screen.getByTestId("app-table-pagination-actions")).toContainElement(
      screen.getByRole("button", { name: "Exportar tabla" }),
    );
    expect(screen.getByTestId("app-table-query-range")).toHaveTextContent(
      "26-50 de 87",
    );
  });

  it("emite patches simples al cambiar la búsqueda y navegar páginas", () => {
    const onQueryChange = vi.fn();

    render(
      <AppTableQueryWrapper
        queryState={createQueryState()}
        onQueryChange={onQueryChange}
        total={87}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    fireEvent.change(
      screen.getByRole("combobox", { name: "Buscar en la tabla" }),
      {
        target: { value: "radicado" },
      },
    );
    fireEvent.click(screen.getByRole("button", { name: "Página anterior" }));
    fireEvent.click(screen.getByRole("button", { name: "Página siguiente" }));

    expect(onQueryChange).toHaveBeenNthCalledWith(1, { search: "radicado" });
    expect(onQueryChange).toHaveBeenNthCalledWith(2, { page: 1 });
    expect(onQueryChange).toHaveBeenNthCalledWith(3, { page: 3 });
  });

  it("bloquea navegación anterior y siguiente cuando no hay páginas disponibles", () => {
    const onQueryChange = vi.fn();

    render(
      <AppTableQueryWrapper
        queryState={createQueryState({ page: 1 })}
        onQueryChange={onQueryChange}
        total={25}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Página anterior" }));
    fireEvent.click(screen.getByRole("button", { name: "Página siguiente" }));

    expect(onQueryChange).not.toHaveBeenCalled();
  });

  it("ejecuta refresh sin alterar el query state y permite cambiar page size", async () => {
    const onQueryChange = vi.fn();
    const onRefresh = vi.fn();

    render(
      <AppTableQueryWrapper
        queryState={createQueryState({ page: 1 })}
        onQueryChange={onQueryChange}
        onRefresh={onRefresh}
        total={87}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Actualizar tabla" }));
    expect(onRefresh).toHaveBeenCalledTimes(1);
    expect(onQueryChange).not.toHaveBeenCalled();

    expect(
      screen.getByRole("button", { name: "Cantidad de registros por página" }),
    ).toHaveTextContent("25 por página");

    fireEvent.click(
      screen.getByRole("button", { name: "Cantidad de registros por página" }),
    );
    fireEvent.click(await screen.findByText("50 por página"));

    expect(onQueryChange).toHaveBeenCalledWith({ pageSize: 50 });
  });

  it("permite ocultar el buscador sin afectar el resto del wrapper", () => {
    render(
      <AppTableQueryWrapper
        queryState={createQueryState()}
        onQueryChange={vi.fn()}
        total={87}
        showSearch={false}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    expect(
      screen.queryByRole("combobox", { name: "Buscar en la tabla" }),
    ).not.toBeInTheDocument();
    expect(screen.getByTestId("app-table-query-range")).toHaveTextContent(
      "26-50 de 87",
    );
  });

  it("permite ocultar la paginacion sin remover buscador ni contenido", () => {
    render(
      <AppTableQueryWrapper
        queryState={createQueryState()}
        onQueryChange={vi.fn()}
        total={87}
        paginationActions={<button type="button">Exportar tabla</button>}
        showPagination={false}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    expect(screen.getByText("Tabla mock")).toBeInTheDocument();
    expect(
      screen.getByRole("combobox", { name: "Buscar en la tabla" }),
    ).toBeInTheDocument();
    expect(
      screen.queryByTestId("app-table-query-range"),
    ).not.toBeInTheDocument();
    expect(
      screen.queryByRole("button", {
        name: "Cantidad de registros por pÃ¡gina",
      }),
    ).not.toBeInTheDocument();
    expect(
      screen.queryByRole("button", { name: "PÃ¡gina anterior" }),
    ).not.toBeInTheDocument();
    expect(
      screen.queryByTestId("app-table-pagination-actions"),
    ).not.toBeInTheDocument();
  });

  it("integra AppTableExport dentro de la banda de controles sin ocultar la tabla", async () => {
    const createObjectUrlSpy = vi.fn(() => "blob:mock");
    const revokeObjectUrlSpy = vi.fn();
    const anchorClickSpy = vi.fn();

    vi.stubGlobal("URL", {
      createObjectURL: createObjectUrlSpy,
      revokeObjectURL: revokeObjectUrlSpy,
    });
    vi.spyOn(HTMLAnchorElement.prototype, "click").mockImplementation(
      anchorClickSpy,
    );

    render(
      <AppTableQueryWrapper
        queryState={createQueryState()}
        onQueryChange={vi.fn()}
        total={87}
        paginationActions={
          <AppTableExport
            columns={exportColumns}
            dataSource={{
              getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
            }}
            formats={["csv"]}
            reportMeta={exportReportMeta}
            enabledModes={["currentPage"]}
          />
        }
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    expect(screen.getByTestId("app-table-pagination-actions")).toContainElement(
      screen.getByRole("button", { name: "Exportar" }),
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Página actual"));

    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(anchorClickSpy).toHaveBeenCalledTimes(1);
    expect(screen.getByText("Tabla mock")).toBeInTheDocument();

    vi.restoreAllMocks();
    vi.unstubAllGlobals();
  });
});
