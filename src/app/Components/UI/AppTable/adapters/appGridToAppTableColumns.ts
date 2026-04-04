import type { ColDef } from "ag-grid-community";
import AppTableActionCellRenderer from "../renderers/AppTableActionCellRenderer";
import type {
  AppGridCellAction,
  AppGridColumn,
  AppTableRow,
  DynamicUiUnknownRecord,
} from "../types/dynamicUiTable.types";

export type AppTableColumnAdapterOptions = {
  tableId?: string;
  userClaims?: string[];
  menuActions?: AppGridCellAction[];
};

const resolveFilter = (column: AppGridColumn): ColDef<AppTableRow>["filter"] => {
  if (!column.filterable) {
    return false;
  }

  if (column.agGridFilterType && column.agGridFilterType !== "none") {
    return column.agGridFilterType;
  }

  return true;
};

const resolveCellStyle = <T extends AppTableRow>(
  column: AppGridColumn,
): ColDef<T>["cellStyle"] => {
  if (!column.align) {
    return undefined;
  }

  return {
    textAlign: column.align,
  };
};

const buildActionValueGetter = () => "";

export const mapAppGridColumnsToAppTableColumns = <T extends AppTableRow = AppTableRow>(
  columns: ReadonlyArray<AppGridColumn> | null | undefined,
  options: AppTableColumnAdapterOptions = {},
): ColDef<T>[] => {
  if (!columns?.length) {
    return [];
  }

  return columns.map((column) => {
    const colDef: ColDef<T> = {
      field: column.field as ColDef<T>["field"],
      headerName: column.headerName,
      hide: column.visible === false,
      sortable: column.sortable,
      filter: resolveFilter(column),
      width: column.width,
      pinned: column.pinned,
      lockPinned: column.lockPinned,
      cellStyle: resolveCellStyle<T>(column),
      colId: column.field,
    };

    if (column.isActionColumn) {
      colDef.sortable = false;
      colDef.filter = false;
      colDef.valueGetter = buildActionValueGetter;
      colDef.cellRenderer = AppTableActionCellRenderer;
      colDef.cellRendererParams = {
        appGridColumn: column,
        actions: [...(column.actions ?? [])],
        menuActions: [...(options.menuActions ?? [])],
        tableId: options.tableId,
        userClaims: [...(options.userClaims ?? [])],
      };
    }

    if (column.metadata) {
      colDef.headerComponentParams = {
        metadata: { ...(column.metadata as DynamicUiUnknownRecord) },
      };
    }

    return colDef;
  });
};
