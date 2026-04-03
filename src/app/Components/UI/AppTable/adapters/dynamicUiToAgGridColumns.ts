import { groupCellActionsByColumnKey, mapDynamicUiActions } from "../utils/dynamicUiActionMapper";
import type {
  AppDataTableAgGrid,
  AppGridColumn,
  DynamicUiTableDto,
  DynamicUiUnknownRecord,
  UiColumnDto,
} from "../types/dynamicUiTable.types";
import { mapDynamicUiRowsToAppGridRows } from "./dynamicUiToAgGridRows";

const pickString = (...values: Array<string | null | undefined>): string | undefined =>
  values.find((value) => typeof value === "string" && value.trim().length > 0)?.trim();

const pickBoolean = (
  fallback: boolean,
  ...values: Array<boolean | null | undefined>
): boolean => {
  const match = values.find((value) => typeof value === "boolean");
  return match ?? fallback;
};

const pickNumber = (...values: Array<number | null | undefined>): number | undefined =>
  values.find((value) => typeof value === "number" && Number.isFinite(value));

const pickRecord = (
  ...values: Array<DynamicUiUnknownRecord | null | undefined>
): DynamicUiUnknownRecord | undefined => {
  const match = values.find((value) => value != null);
  return match ? { ...match } : undefined;
};

const resolveField = (column: UiColumnDto, index: number): string =>
  pickString(
    column.dataIndex,
    column.DataIndex,
    column.field,
    column.Field,
    column.columnName,
    column.ColumnName,
    column.key,
    column.Key,
    column.columnKey,
    column.ColumnKey,
    column.id,
    column.Id,
  ) ?? `column-${index}`;

const resolveHeaderName = (column: UiColumnDto, field: string): string =>
  pickString(column.headerName, column.HeaderName, column.title, column.Title, column.columnName, column.ColumnName) ??
  field;

const resolveColumnKey = (column: UiColumnDto, field: string): string =>
  pickString(column.columnKey, column.ColumnKey, column.key, column.Key, column.id, column.Id) ?? field;

const resolveVisible = (column: UiColumnDto): boolean => {
  const visible = column.visible ?? column.Visible;
  return visible !== false;
};

const resolveOrder = (column: UiColumnDto): number => pickNumber(column.order, column.Order) ?? Number.MAX_SAFE_INTEGER;

const mapActionColumn = (
  column: UiColumnDto,
  field: string,
  columnKey: string,
  table: DynamicUiTableDto,
): AppGridColumn => {
  const cellActionGroups = groupCellActionsByColumnKey(table.cellActions ?? table.CellActions);
  const rowActions = mapDynamicUiActions(table.rowActions ?? table.RowActions);
  const actions = cellActionGroups[columnKey] ?? (rowActions.length > 0 ? rowActions : []);

  return {
    field,
    headerName: resolveHeaderName(column, field),
    visible: resolveVisible(column),
    sortable: pickBoolean(false, column.sortable, column.Sortable),
    filterable: pickBoolean(false, column.filterable, column.Filterable),
    width: pickNumber(column.width, column.Width),
    pinned: pickString(column.pinned, column.Pinned) as AppGridColumn["pinned"],
    lockPinned: pickBoolean(false, column.lockPinned, column.LockPinned),
    align: column.align ?? column.Align ?? undefined,
    isActionColumn: true,
    renderType: pickString(column.renderType, column.RenderType, column.presentation, column.Presentation, "actions"),
    dataType: pickString(column.dataType, column.DataType),
    order: pickNumber(column.order, column.Order),
    filterType: pickString(column.filterType, column.FilterType),
    agGridFilterType: pickString(column.agGridFilterType, column.AgGridFilterType),
    filterOptions: column.filterOptions ?? column.FilterOptions,
    actions,
    metadata: pickRecord(column.metadata, column.Metadata),
  };
};

export const mapDynamicUiColumnsToAppGridColumns = (
  table: Pick<DynamicUiTableDto, "columns" | "Columns" | "rowActions" | "RowActions" | "cellActions" | "CellActions"> | null | undefined,
): AppGridColumn[] => {
  const columns = table?.columns ?? table?.Columns;
  if (!columns?.length) {
    return [];
  }

  return columns
    .map((column, index) => {
      const field = resolveField(column, index);
      const columnKey = resolveColumnKey(column, field);
      const isActionColumn = pickBoolean(false, column.isActionColumn, column.IsActionColumn);

      if (isActionColumn) {
        return mapActionColumn(column, field, columnKey, table);
      }

      return {
        field,
        headerName: resolveHeaderName(column, field),
        visible: resolveVisible(column),
        sortable: pickBoolean(true, column.sortable, column.Sortable),
        filterable: pickBoolean(true, column.filterable, column.Filterable),
        width: pickNumber(column.width, column.Width),
        pinned: pickString(column.pinned, column.Pinned) as AppGridColumn["pinned"],
        lockPinned: pickBoolean(false, column.lockPinned, column.LockPinned),
        align: column.align ?? column.Align ?? undefined,
        renderType: pickString(column.renderType, column.RenderType),
        dataType: pickString(column.dataType, column.DataType),
        order: pickNumber(column.order, column.Order),
        filterType: pickString(column.filterType, column.FilterType),
        agGridFilterType: pickString(column.agGridFilterType, column.AgGridFilterType),
        filterOptions: column.filterOptions ?? column.FilterOptions,
        metadata: pickRecord(column.metadata, column.Metadata),
      };
    })
    .filter((column) => column.visible)
    .sort((left, right) => resolveOrder({ order: left.order }) - resolveOrder({ order: right.order }));
};

export const mapDynamicUiTableToAppDataTableAgGrid = (
  table: DynamicUiTableDto | null | undefined,
): AppDataTableAgGrid => ({
  tableId: pickString(table?.tableId, table?.TableId),
  title: pickString(table?.title, table?.Title),
  columns: mapDynamicUiColumnsToAppGridColumns(table),
  rows: mapDynamicUiRowsToAppGridRows(table?.rows ?? table?.Rows),
  pagination: table?.pagination ?? table?.Pagination ?? undefined,
  sorting: table?.sorting ?? table?.Sorting ?? undefined,
  toolbarActions: mapDynamicUiActions(table?.toolbarActions ?? table?.ToolbarActions),
  bulkActions: mapDynamicUiActions(table?.bulkActions ?? table?.BulkActions),
  rowActions: mapDynamicUiActions(table?.rowActions ?? table?.RowActions),
  menuActions: mapDynamicUiActions(table?.menuActions ?? table?.MenuActions),
  userClaims: [...(table?.userClaims ?? table?.UserClaims ?? [])],
  metadata: pickRecord(table?.meta, table?.metadata, table?.Metadata),
});
