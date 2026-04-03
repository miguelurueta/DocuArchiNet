import { render, screen } from "@testing-library/react";
import { describe, expect, test, vi } from "vitest";
import type { ColDef } from "ag-grid-community";
import AppTable from "../AppTable";

const agGridReactSpy = vi.fn();

vi.mock("ag-grid-react", () => ({
  AgGridReact: (props: unknown) => {
    agGridReactSpy(props);
    return <div>Mocked Grid</div>;
  },
}));

type Row = {
  id: string;
  name: string;
};

const columns: ColDef<Row>[] = [
  { field: "name", headerName: "Nombre" },
];

describe("[SPEC:CREA-COMPONENTE-TABLE] AppTable", () => {
  test("preserva compatibilidad hacia atrás sin paginationMode", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(false);
    expect(lastCall.gridOptions?.paginationPageSize).toBeUndefined();
    expect(lastCall.quickFilterText).toBeUndefined();
  });

  test("renderiza el componente base", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "ready");
    expect(screen.getByText("Mocked Grid")).toBeInTheDocument();
  });

  test("expone estado empty cuando no hay filas", () => {
    render(<AppTable rows={[]} columns={columns} />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "empty");
  });

  test("expone estado loading cuando loading es true", () => {
    render(<AppTable rows={[]} columns={columns} loading />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "loading");
  });

  test("configura client mode con paginación nativa y page size custom", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="client"
        clientPaginationPageSize={50}
        quickFilterText="alpha"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(true);
    expect(lastCall.gridOptions?.paginationPageSize).toBe(50);
    expect(lastCall.quickFilterText).toBe("alpha");
  });

  test("usa page size default 25 en client mode cuando no se informa override", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="client"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
    };

    expect(lastCall.gridOptions?.pagination).toBe(true);
    expect(lastCall.gridOptions?.paginationPageSize).toBe(25);
  });

  test("configura none mode sin paginación y conserva quickFilter local", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="none"
        quickFilterText="alpha"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(false);
    expect(lastCall.gridOptions?.paginationPageSize).toBeUndefined();
    expect(lastCall.quickFilterText).toBe("alpha");
  });

  test("configura server mode sin paginación del grid e ignora quickFilter local", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="server"
        quickFilterText="alpha"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(false);
    expect(lastCall.gridOptions?.paginationPageSize).toBeUndefined();
    expect(lastCall.quickFilterText).toBeUndefined();
  });

  test("ejecuta callbacks de seleccion y click", () => {
    const onRowClicked = vi.fn();
    const onCellClicked = vi.fn();
    const onSelectionChanged = vi.fn();

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        onRowClicked={onRowClicked}
        onCellClicked={onCellClicked}
        onSelectionChanged={onSelectionChanged}
      />,
    );

    expect(onRowClicked).not.toHaveBeenCalled();
    expect(onCellClicked).not.toHaveBeenCalled();
    expect(onSelectionChanged).not.toHaveBeenCalled();
  });
});
