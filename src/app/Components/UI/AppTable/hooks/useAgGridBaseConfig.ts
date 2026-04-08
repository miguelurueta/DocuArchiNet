import { useMemo } from "react";
import type { GridOptions } from "ag-grid-community";
import type {
  AppTableAgGridHandlers,
  AppTableDomLayout,
  AppTableLayoutMode,
  AppTablePaginationMode,
  AppTableRow,
  AppTableRowSelection,
} from "../AppTable.types";
import { DEFAULT_APP_TABLE_CLIENT_PAGE_SIZE } from "../AppTable.types";
import {
  buildAgGridDefaults,
  createRowSelectionConfig,
} from "../utils/agGridDefaultConfig";

type UseAgGridBaseConfigParams<T extends AppTableRow> = {
  rowSelection?: AppTableRowSelection;
  domLayout?: AppTableDomLayout;
  layoutMode?: AppTableLayoutMode;
  paginationMode?: AppTablePaginationMode;
  clientPaginationPageSize?: number;
  suppressRowClickSelection?: boolean;
  suppressCellFocus?: boolean;
} & AppTableAgGridHandlers<T>;

export const useAgGridBaseConfig = <T extends AppTableRow>({
  rowSelection,
  domLayout,
  layoutMode,
  paginationMode,
  clientPaginationPageSize,
  suppressRowClickSelection,
  suppressCellFocus,
  onRowClicked,
  onRowSelected,
  onCellClicked,
  onCellKeyDown,
  onSelectionChanged,
}: UseAgGridBaseConfigParams<T>) => {
  return useMemo<GridOptions<T>>(() => {
    const defaults = buildAgGridDefaults<T>();

    return {
      ...defaults,
      rowSelection: createRowSelectionConfig(
        rowSelection ?? "multiple",
        suppressRowClickSelection ?? false,
      ),
      suppressCellFocus: suppressCellFocus ?? defaults.suppressCellFocus,
      domLayout: layoutMode === "fill" ? "normal" : (domLayout ?? defaults.domLayout),
      pagination: paginationMode === "client",
      paginationPageSize:
        paginationMode === "client"
          ? (clientPaginationPageSize ?? DEFAULT_APP_TABLE_CLIENT_PAGE_SIZE)
          : undefined,
      onRowClicked,
      onRowSelected,
      onCellClicked,
      onCellKeyDown,
      onSelectionChanged,
    };
  }, [
    clientPaginationPageSize,
    domLayout,
    layoutMode,
    onCellClicked,
    onCellKeyDown,
    onRowClicked,
    onRowSelected,
    onSelectionChanged,
    paginationMode,
    rowSelection,
    suppressCellFocus,
    suppressRowClickSelection,
  ]);
};
