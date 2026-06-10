import type {
  ColDef,
  GridOptions,
  RowSelectionOptions,
} from "ag-grid-community";
import {
  DEFAULT_APP_TABLE_SUPPRESS_CELL_FOCUS,
  type AppTableDomLayout,
  type AppTableRow,
  type AppTableRowSelection,
} from "../AppTable.types";

export type AppTableDefaultConfig<T extends AppTableRow> = {
  rowSelection: RowSelectionOptions<T>;
  domLayout: AppTableDomLayout;
  defaultColDef: ColDef<T>;
  suppressCellFocus: boolean;
  overlayLoadingTemplate: string;
  overlayNoRowsTemplate: string;
};

export const AG_GRID_OVERLAYS = {
  loading: "<span class=\"ag-overlay-loading-center\">Cargando...</span>",
  empty: "<span class=\"ag-overlay-no-rows-center\">Sin registros</span>",
};

export const createRowSelectionConfig = <T extends AppTableRow>(
  selection: AppTableRowSelection = "multiple",
  suppressRowClickSelection = false,
  options?: {
    checkboxes?: boolean;
    headerCheckbox?: boolean;
  },
): RowSelectionOptions<T> => ({
  mode: selection === "single" ? "singleRow" : "multiRow",
  enableClickSelection: suppressRowClickSelection ? false : true,
  ...(typeof options?.checkboxes === "boolean" ? { checkboxes: options.checkboxes } : {}),
  ...(typeof options?.headerCheckbox === "boolean"
    ? { headerCheckbox: options.headerCheckbox }
    : {}),
});

export const createAgGridDefaultConfig = <T extends AppTableRow>(): AppTableDefaultConfig<T> => ({
  rowSelection: createRowSelectionConfig<T>("multiple"),
  domLayout: "autoHeight",
  suppressCellFocus: DEFAULT_APP_TABLE_SUPPRESS_CELL_FOCUS,
  defaultColDef: {
    resizable: true,
    sortable: true,
    filter: true,
  },
  overlayLoadingTemplate: AG_GRID_OVERLAYS.loading,
  overlayNoRowsTemplate: AG_GRID_OVERLAYS.empty,
});

export const buildAgGridDefaults = <T extends AppTableRow>(): GridOptions<T> => {
  const defaults = createAgGridDefaultConfig<T>();

  return {
    rowSelection: defaults.rowSelection,
    domLayout: defaults.domLayout,
    suppressCellFocus: defaults.suppressCellFocus,
    defaultColDef: defaults.defaultColDef,
    overlayLoadingTemplate: defaults.overlayLoadingTemplate,
    overlayNoRowsTemplate: defaults.overlayNoRowsTemplate,
  };
};
