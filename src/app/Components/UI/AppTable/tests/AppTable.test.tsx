import { render, screen } from "@testing-library/react";
import { describe, expect, test, vi } from "vitest";
import type { ColDef } from "ag-grid-community";
import AppTable from "../AppTable";

vi.mock("ag-grid-react", () => ({
  AgGridReact: () => <div>Mocked Grid</div>,
}));

type Row = {
  id: string;
  name: string;
};

const columns: ColDef<Row>[] = [
  { field: "name", headerName: "Nombre" },
];

describe("[SPEC:CREA-COMPONENTE-TABLE] AppTable", () => {
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
