import type { ColDef } from "ag-grid-community";
import type {
  AppGridColumn,
  AppTableRow,
  DynamicUiUnknownRecord,
} from "../types/dynamicUiTable.types";

const resolveFilter = (column: AppGridColumn): ColDef<AppTableRow>["filter"] => {
  if (!column.filterable) {
    return false;
  }

  if (column.agGridFilterType && column.agGridFilterType !== "none") {
    return column.agGridFilterType;
  }

  return true;
};

const resolveCellStyle = (
  column: AppGridColumn,
): ColDef<AppTableRow>["cellStyle"] => {
  if (!column.align) {
    return undefined;
  }

  return {
    textAlign: column.align,
  };
};

const buildActionValueGetter = (field: string) => () => "";

export const mapAppGridColumnsToAppTableColumns = <T extends AppTableRow = AppTableRow>(
  columns: ReadonlyArray<AppGridColumn> | null | undefined,
): ColDef<T>[] => {
  if (!columns?.length) {
    return [];
  }

  return columns.map((column) => {
    const colDef: ColDef<T> = {
      field: column.field as keyof T & string,
      headerName: column.headerName,
      hide: column.visible === false,
      sortable: column.sortable,
      filter: resolveFilter(column),
      width: column.width,
      cellStyle: resolveCellStyle(column),
      colId: column.field,
    };

    if (column.isActionColumn) {
      colDef.sortable = false;
      colDef.filter = false;
      colDef.valueGetter = buildActionValueGetter(column.field);
    }

    if (column.metadata) {
      colDef.headerComponentParams = {
        metadata: { ...(column.metadata as DynamicUiUnknownRecord) },
      };
    }

    return colDef;
  });
};
