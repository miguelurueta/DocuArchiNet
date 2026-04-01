import { useMemo } from "react";
import type { GridOptions } from "ag-grid-community";
import type {
  AppTableAgGridHandlers,
  AppTableDomLayout,
  AppTableRow,
  AppTableRowSelection,
} from "../AppTable.types";
import { buildAgGridDefaults } from "../utils/agGridDefaultConfig";

type UseAgGridBaseConfigParams<T extends AppTableRow> = {
  rowSelection?: AppTableRowSelection;
  domLayout?: AppTableDomLayout;
  suppressRowClickSelection?: boolean;
} & AppTableAgGridHandlers<T>;

export const useAgGridBaseConfig = <T extends AppTableRow>({
  rowSelection,
  domLayout,
  suppressRowClickSelection,
  onRowClicked,
  onRowSelected,
  onCellClicked,
  onSelectionChanged,
}: UseAgGridBaseConfigParams<T>) => {
  return useMemo<GridOptions<T>>(() => {
    const defaults = buildAgGridDefaults<T>();

    return {
      ...defaults,
      rowSelection: rowSelection ?? defaults.rowSelection,
      domLayout: domLayout ?? defaults.domLayout,
      suppressRowClickSelection:
        typeof suppressRowClickSelection === "boolean"
          ? suppressRowClickSelection
          : defaults.suppressRowClickSelection,
      onRowClicked,
      onRowSelected,
      onCellClicked,
      onSelectionChanged,
    };
  }, [
    domLayout,
    onCellClicked,
    onRowClicked,
    onRowSelected,
    onSelectionChanged,
    rowSelection,
    suppressRowClickSelection,
  ]);
};
