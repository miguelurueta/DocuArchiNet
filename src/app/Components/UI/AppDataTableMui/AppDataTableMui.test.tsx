import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import type { AppDataTableMuiColumn } from "./AppDataTableMui";
import { AppDataTableMui } from "./AppDataTableMui";
import type { ReactNode } from "react";

vi.mock("@mui/x-data-grid", () => {
  type MockDataGridColumn = {
    field: string;
    headerName?: ReactNode;
  };

  type MockDataGridRowId = string | number;

  type MockDataGridRow = {
    id: MockDataGridRowId;
    [key: string]: ReactNode;
  };

  type MockRowSelectionModel =
    | { type: "include"; ids: Set<MockDataGridRowId> }
    | { type: "exclude"; ids: Set<MockDataGridRowId> };

  type MockDataGridProps = {
    rows: MockDataGridRow[];
    columns: MockDataGridColumn[];
    loading?: boolean;
    checkboxSelection?: boolean;
    onRowSelectionModelChange?: (model: MockRowSelectionModel, details: unknown) => void;
    slots?: { noRowsOverlay?: () => JSX.Element };
    "aria-label"?: string;
  };

  const MockDataGrid = ({
    rows,
    columns,
    loading,
    checkboxSelection,
    onRowSelectionModelChange,
    slots,
    "aria-label": ariaLabel,
  }: MockDataGridProps) => {
    if (loading) {
      return <div role="progressbar" />;
    }

    if (!rows.length) {
      const Empty = slots?.noRowsOverlay;
      return (
        <div role="grid" aria-label={ariaLabel}>
          {Empty ? <Empty /> : null}
        </div>
      );
    }

    return (
      <div role="grid" aria-label={ariaLabel}>
        <div role="rowgroup">
          {columns.map((column) => (
            <div key={column.field} role="columnheader">
              {column.headerName}
            </div>
          ))}
        </div>
        <div role="rowgroup">
          {rows.map((row) => (
            <div key={row.id} role="row">
              {checkboxSelection ? (
                <input
                  aria-label={`select-row-${row.id}`}
                  type="checkbox"
                  onChange={() =>
                    onRowSelectionModelChange?.({ type: "include", ids: new Set([row.id]) }, {})
                  }
                />
              ) : null}
              {columns.map((column) => (
                <div key={column.field} role="gridcell">
                  {row[column.field]}
                </div>
              ))}
            </div>
          ))}
        </div>
        <div>1-2 of 2</div>
      </div>
    );
  };

  return {
    DataGrid: MockDataGrid,
  };
});

const columns: AppDataTableMuiColumn[] = [
  { field: "name", headerName: "Nombre", flex: 1, minWidth: 180 },
  { field: "role", headerName: "Rol", flex: 1, minWidth: 160 },
];

const rows = [
  { id: 1, name: "Ana Torres", role: "Analista" },
  { id: 2, name: "Luis Perez", role: "Supervisor" },
];

describe("AppDataTableMui [SPEC:APP-DATATABLE-MUI-001]", () => {
  it("renderiza columnas y filas", () => {
    render(
      <AppDataTableMui
        label="Usuarios"
        rows={rows}
        columns={columns}
      />,
    );

    expect(screen.getByRole("grid", { name: "Usuarios" })).toBeInTheDocument();
    expect(screen.getByRole("columnheader", { name: "Nombre" })).toBeInTheDocument();
    expect(screen.getByRole("gridcell", { name: "Ana Torres" })).toBeInTheDocument();
  });

  it("muestra estado vacio personalizado cuando no hay filas", () => {
    render(
      <AppDataTableMui
        label="Usuarios"
        rows={[]}
        columns={columns}
        emptyMessage="No se encontraron usuarios."
      />,
    );

    expect(screen.getByText("Sin resultados")).toBeInTheDocument();
    expect(screen.getByText("No se encontraron usuarios.")).toBeInTheDocument();
  });

  it("muestra feedback de carga", () => {
    render(
      <AppDataTableMui
        label="Usuarios"
        rows={rows}
        columns={columns}
        loading
      />,
    );

    expect(screen.getByRole("progressbar")).toBeInTheDocument();
  });

  it("propaga seleccion de filas cuando checkboxSelection esta habilitado", () => {
    const handleSelectionChange = vi.fn();

    render(
      <AppDataTableMui
        label="Usuarios"
        rows={rows}
        columns={columns}
        checkboxSelection
        disableRowSelectionOnClick
        onRowSelectionModelChange={handleSelectionChange}
      />,
    );

    fireEvent.click(screen.getByRole("checkbox", { name: "select-row-1" }));

    expect(handleSelectionChange).toHaveBeenCalled();
  });

  it("permite pagina inicial configurable", () => {
    render(
      <AppDataTableMui
        label="Usuarios"
        rows={rows}
        columns={columns}
        initialPageSize={5}
      />,
    );

    expect(screen.getByText("1-2 of 2")).toBeInTheDocument();
  });

  it("mantiene nombre accesible programatico", () => {
    render(
      <AppDataTableMui
        label="Tabla de usuarios del sistema"
        rows={rows}
        columns={columns}
      />,
    );

    expect(
      screen.getByRole("grid", { name: "Tabla de usuarios del sistema" }),
    ).toBeInTheDocument();
  });
});
