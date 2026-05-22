import type {
  CellClickedEvent,
  CellKeyDownEvent,
  ColDef,
  RowClickedEvent,
  RowSelectedEvent,
  SelectionChangedEvent,
} from "ag-grid-community";

export type AppTableRow = Record<string, unknown>;

export type AppTableRowSelection = "single" | "multiple";
export type AppTableDomLayout = "autoHeight" | "normal" | "print";
export type AppTablePaginationMode = "none" | "client" | "server";
export type AppTableLayoutMode = "content" | "fill";
export type AppTablePresentationMode = "table" | "cards";
export type AppTableLoadingMode = "overlay" | "skeleton";
export type AppTableResponsivePresentation = {
  enabled?: boolean;
  cardsBelow?: number;
};

export const DEFAULT_APP_TABLE_CLIENT_PAGE_SIZE = 25;
export const DEFAULT_APP_TABLE_SUPPRESS_CELL_FOCUS = true;

export type AppTableCellClick<T extends AppTableRow> = {
  row: T;
  field?: string | null;
  value?: unknown;
};

export type AppTableActionTriggered<T extends AppTableRow> = {
  actionId: string;
  row: T;
  columnKey?: string;
};

export type AppTableProps<T extends AppTableRow> = {
  rows: T[];
  columns: ColDef<T>[];
  cardFields?: string[];
  loading?: boolean;
  total?: number;
  paginationMode?: AppTablePaginationMode;
  quickFilterText?: string;
  clientPaginationPageSize?: number;
  layoutMode?: AppTableLayoutMode;
  presentationMode?: AppTablePresentationMode;
  loadingMode?: AppTableLoadingMode;
  responsivePresentation?: AppTableResponsivePresentation;
  rowSelection?: AppTableRowSelection;
  rowSelectionCheckboxes?: boolean;
  rowSelectionHeaderCheckbox?: boolean;
  suppressRowClickSelection?: boolean;
  suppressCellFocus?: boolean;
  rowClickAffordance?: boolean;
  rowClickTooltip?: string;
  domLayout?: AppTableDomLayout;
  className?: string;
  gridClassName?: string;
  getRowId?: (row: T) => string;
  onRowSelected?: (row: T | null) => void;
  onCellClicked?: (params: AppTableCellClick<T>) => void;
  onRowClicked?: (row: T) => void;
  onActionTriggered?: (params: AppTableActionTriggered<T>) => void;
  onSelectionChanged?: (rows: T[]) => void;
};

export type AppTableAgGridHandlers<T extends AppTableRow> = {
  onRowSelected?: (event: RowSelectedEvent<T>) => void;
  onRowClicked?: (event: RowClickedEvent<T>) => void;
  onCellClicked?: (event: CellClickedEvent<T>) => void;
  onCellKeyDown?: (event: CellKeyDownEvent<T>) => void;
  onSelectionChanged?: (event: SelectionChangedEvent<T>) => void;
};
