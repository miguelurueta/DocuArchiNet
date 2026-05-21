import type { ColDef } from "ag-grid-community";

export type AppTreeTableRowId = string;

export type AppTreeTableCellValue = string | number | boolean | null;

export type AppTreeTableRow = {
  id: AppTreeTableRowId;
  label: string;
  values?: Record<string, AppTreeTableCellValue>;
  meta?: Record<string, unknown>;
  hasChildren?: boolean;
  children?: AppTreeTableRow[];
};

export type AppTreeTableLoadResult =
  | { ok: true; rows: AppTreeTableRow[] }
  | { ok: false; message: string };

export type AppTreeTableLoadChildrenResult =
  | { ok: true; rows: AppTreeTableRow[] }
  | { ok: false; message: string };

export type AppTreeTableProps = {
  rows?: AppTreeTableRow[];
  load?: () => Promise<AppTreeTableLoadResult>;
  loadChildren?: (row: AppTreeTableRow) => Promise<AppTreeTableLoadChildrenResult>;
  onSelectRow?: (rowId: AppTreeTableRowId) => void;
  onCellClicked?: (params: { rowId: AppTreeTableRowId; field?: string | null; value?: unknown }) => void;
  onActionTriggered?: (params: { actionId: string; rowId: AppTreeTableRowId; columnKey?: string }) => void;
  tableColumns?: ColDef<Record<string, unknown>>[];
  columns?: string[];
  emptyMessage?: string;
  isRetryEnabled?: boolean;
  className?: string;
};
