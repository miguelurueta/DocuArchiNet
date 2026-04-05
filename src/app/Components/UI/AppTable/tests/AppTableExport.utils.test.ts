import type { ColDef } from "ag-grid-community";
import { describe, expect, it } from "vitest";
import {
  getAppTableExportRows,
  getAppTableExportableColumns,
  serializeAppTableExportToCsv,
} from "../AppTableExport.utils";

type Row = {
  id: string;
  name: string;
  acciones?: string;
};

const columns: ColDef<Row>[] = [
  { field: "name", headerName: "Nombre" },
  {
    field: "acciones",
    headerName: "Acciones",
    cellRendererParams: {
      actions: [{ id: "ver" }],
    },
  },
];

describe("AppTableExport utilities", () => {
  it("excluye columnas de acciones al resolver columnas exportables", () => {
    expect(getAppTableExportableColumns(columns)).toEqual([
      { field: "name", headerName: "Nombre" },
    ]);
  });

  it("serializa csv usando solo columnas exportables", () => {
    const exportableColumns = getAppTableExportableColumns(columns);

    expect(
      serializeAppTableExportToCsv({
        columns: exportableColumns,
        rows: [{ id: "1", name: "Alpha", acciones: "Ver" }],
      }),
    ).toBe("Nombre\nAlpha");
  });

  it("resuelve selectedRows cuando el modo de exportacion lo requiere", () => {
    expect(
      getAppTableExportRows({
        mode: "selectedRows",
        getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
        getSelectedRows: () => [{ id: "2", name: "Beta" }],
      }),
    ).toEqual([{ id: "2", name: "Beta" }]);
  });
});
