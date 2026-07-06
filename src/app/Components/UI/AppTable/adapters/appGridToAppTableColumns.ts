import type { ColDef } from "ag-grid-community";
import AppTableActionCellRenderer from "../renderers/AppTableActionCellRenderer";
import { formatAppTableDateValue } from "../utils/appTableValueFormatters";
import type {
  AppGridCellAction,
  AppGridColumn,
  AppTableRow,
  DynamicUiUnknownRecord,
} from "../types/dynamicUiTable.types";

const ACTION_CELL_CLASS = "app-table-action-cell";

export type AppTableColumnAdapterOptions = {
  tableId?: string;
  userClaims?: string[];
  menuActions?: AppGridCellAction[];
  onClientEvent?: (input: {
    actionId: string;
    row: AppTableRow;
    columnKey?: string;
  }) => void;
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

const resolveActionCellStyle = <T extends AppTableRow>(
  column: AppGridColumn,
): ColDef<T>["cellStyle"] => ({
  alignItems: "center",
  display: "flex",
  height: "100%",
  justifyContent:
    column.align === "left" ? "flex-start" : column.align === "right" ? "flex-end" : "center",
  lineHeight: "normal",
  textAlign: column.align ?? "center",
});

const normalizeColumnType = (value: string | undefined): string =>
  value?.trim().toLowerCase() ?? "";

const isDateColumn = (column: AppGridColumn): boolean => {
  const dataType = normalizeColumnType(column.dataType);
  const renderType = normalizeColumnType(column.renderType);
  const filterType = normalizeColumnType(column.filterType);
  const agGridFilterType = normalizeColumnType(column.agGridFilterType);

  return (
    dataType.includes("date") ||
    renderType.includes("date") ||
    filterType.includes("date") ||
    agGridFilterType.includes("date")
  );
};

const shouldIncludeTime = (column: AppGridColumn): boolean =>
  normalizeColumnType(column.dataType).includes("datetime");

const buildDateValueFormatter =
  (column: AppGridColumn): NonNullable<ColDef<AppTableRow>["valueFormatter"]> =>
  (params) =>
    formatAppTableDateValue(params.value, { includeTime: shouldIncludeTime(column) });

const buildActionValueGetter = () => "";

const WORKBENCH_TWO_COLUMN_TABLE_IDS = new Set(["InboxListaDocumentosRadicado"]);

const LEGACY_FLAT_DOCUMENTS_COLUMNS = new Set([
  "PAG",
  "ESTADO_FIRMA_DIGITAL",
  "DBT",
]);

const normalizeColumnKey = (value: string | undefined): string =>
  value?.trim().toUpperCase() ?? "";

const isWorkbenchTwoColumnContext = (tableId?: string): boolean =>
  !!tableId && WORKBENCH_TWO_COLUMN_TABLE_IDS.has(tableId);

const isSelectableWorkbenchColumn = (column: AppGridColumn, primaryKey?: string): boolean => {
  if (column.visible === false) return false;
  if (column.isActionColumn) return false;
  const key = normalizeColumnKey(column.field);
  if (!key) return false;
  if (primaryKey && key === normalizeColumnKey(primaryKey)) return false;
  if (LEGACY_FLAT_DOCUMENTS_COLUMNS.has(key)) return false;
  return true;
};

const pickWorkbenchTwoColumns = (
  inputColumns: ReadonlyArray<AppGridColumn>,
): { primary?: AppGridColumn; secondary?: AppGridColumn } => {
  const visibleColumns = inputColumns.filter((column) => column.visible !== false);

  const primary =
    visibleColumns.find((column) => normalizeColumnKey(column.field) === "TIPODOCUMENTO") ??
    visibleColumns.find((column) => !column.isActionColumn) ??
    visibleColumns[0];

  const actionColumn = visibleColumns.find((column) => column.isActionColumn);

  const secondary =
    actionColumn ??
    visibleColumns.find((column) => isSelectableWorkbenchColumn(column, primary?.field));

  return { primary, secondary };
};

export const mapAppGridColumnsToAppTableColumns = <T extends AppTableRow = AppTableRow>(
  columns: ReadonlyArray<AppGridColumn> | null | undefined,
  options: AppTableColumnAdapterOptions = {},
): ColDef<T>[] => {
  if (!columns?.length) {
    return [];
  }

  const scopedColumns = isWorkbenchTwoColumnContext(options.tableId)
    ? (() => {
        const { primary, secondary } = pickWorkbenchTwoColumns(columns);
        const result = [primary, secondary].filter(
          (column): column is AppGridColumn => !!column,
        );
        return result.length > 0 ? result : [...columns];
      })()
    : [...columns];

  const shouldApplyWorkbenchSizing = isWorkbenchTwoColumnContext(options.tableId);

  return scopedColumns.map((column, index) => {
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

    if (shouldApplyWorkbenchSizing) {
      colDef.flex = index === 0 ? 2 : 1;
      // Workbench list panel puede tener anchos muy pequeños (ej: 280px). Este preset prioriza
      // mantener visibles "Documento" + columna secundaria (acciones) sin forzar scroll horizontal.
      colDef.minWidth = index === 0 ? 60 : 80;
      colDef.width = undefined;
    }

    if (column.isActionColumn) {
      colDef.sortable = false;
      colDef.filter = false;
      colDef.cellClass = ACTION_CELL_CLASS;
      colDef.cellStyle = resolveActionCellStyle<T>(column);
      colDef.valueGetter = buildActionValueGetter;
      colDef.cellRenderer = AppTableActionCellRenderer;
      colDef.cellRendererParams = {
        appGridColumn: column,
        actions: [...(column.actions ?? [])],
        menuActions: [...(options.menuActions ?? [])],
        tableId: options.tableId,
        userClaims: [...(options.userClaims ?? [])],
        onClientEvent: options.onClientEvent,
        suppressMouseEventHandling: () => true,
      };
    }

    if (!column.isActionColumn && isDateColumn(column)) {
      colDef.valueFormatter = buildDateValueFormatter(column) as ColDef<T>["valueFormatter"];
    }

    if (column.metadata) {
      colDef.headerComponentParams = {
        metadata: { ...(column.metadata as DynamicUiUnknownRecord) },
      };
    }

    return colDef;
  });
};
