import type { ColDef } from "ag-grid-community";
import type {
  AppTableDomLayout,
  AppTableLayoutMode,
  AppTableRowSelection,
} from "../AppTable/AppTable.types";

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
  activeRowId?: AppTreeTableRowId;
  onCellClicked?: (params: { rowId: AppTreeTableRowId; field?: string | null; value?: unknown }) => void;
  onActionTriggered?: (params: { actionId: string; rowId: AppTreeTableRowId; columnKey?: string }) => void;
  rowClickAffordance?: boolean;
  rowClickTooltip?: string;
  rowSelection?: AppTableRowSelection;
  rowSelectionCheckboxes?: boolean;
  rowSelectionHeaderCheckbox?: boolean;
  suppressRowClickSelection?: boolean;
  tableDomLayout?: AppTableDomLayout;
  tableLayoutMode?: AppTableLayoutMode;
  tableColumns?: ColDef<Record<string, unknown>>[];
  columns?: string[];
  emptyMessage?: string;
  isRetryEnabled?: boolean;
  className?: string;
};
