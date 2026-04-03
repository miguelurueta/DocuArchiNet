import type { ColDef } from "ag-grid-community";

export type DynamicUiAlign = "left" | "center" | "right";

export type DynamicUiVisibility = boolean | null | undefined;

export type DynamicUiUnknownRecord = Record<string, unknown>;

export type ApiError = {
  code?: string | number | null;
  message?: string | null;
  detail?: string | null;
  metadata?: DynamicUiUnknownRecord | null;
};

export type ApiResponse<T> = {
  success?: boolean;
  Success?: boolean;
  message?: string | null;
  Message?: string | null;
  data?: T | null;
  Data?: T | null;
  meta?: DynamicUiUnknownRecord | null;
  errors?: ApiError[] | null;
  Errors?: ApiError[] | null;
  metadata?: DynamicUiUnknownRecord | null;
  Metadata?: DynamicUiUnknownRecord | null;
};

export type DynamicUiPaginationDto = {
  page?: number | null;
  Page?: number | null;
  pageSize?: number | null;
  PageSize?: number | null;
  total?: number | null;
  Total?: number | null;
};

export type DynamicUiSortingDto = {
  field?: string | null;
  Field?: string | null;
  sortField?: string | null;
  SortField?: string | null;
  direction?: string | null;
  Direction?: string | null;
  sortDir?: string | null;
  SortDir?: string | null;
};

export type UiActionDto = {
  ActionId?: string | null;
  actionId?: string | null;
  Label?: string | null;
  label?: string | null;
  Placement?: string | null;
  placement?: string | null;
  Presentation?: string | null;
  presentation?: string | null;
  Behavior?: string | null;
  behavior?: string | null;
  BehaviorConfig?: DynamicUiUnknownRecord | null;
  behaviorConfig?: DynamicUiUnknownRecord | null;
  Request?: DynamicUiUnknownRecord | null;
  request?: DynamicUiUnknownRecord | null;
  Icon?: string | null;
  icon?: string | null;
  Tone?: string | null;
  tone?: string | null;
  RequiresConfirm?: boolean | null;
  requiresConfirm?: boolean | null;
  ConfirmTitle?: string | null;
  confirmTitle?: string | null;
  ConfirmMessage?: string | null;
  confirmMessage?: string | null;
  RequiredClaimsAny?: string[] | null;
  requiredClaimsAny?: string[] | null;
  RequiredClaimsAll?: string[] | null;
  requiredClaimsAll?: string[] | null;
  ClaimKey?: string | null;
  claimKey?: string | null;
  Rules?: DynamicUiUnknownRecord | null;
  rules?: DynamicUiUnknownRecord | null;
  Metadata?: DynamicUiUnknownRecord | null;
  metadata?: DynamicUiUnknownRecord | null;
  Payload?: DynamicUiUnknownRecord | null;
  payload?: DynamicUiUnknownRecord | null;
};

export type UiCellActionDto = UiActionDto & {
  ColumnKey?: string | null;
  columnKey?: string | null;
  Action?: UiActionDto | null;
  action?: UiActionDto | null;
};

export type UiColumnDto = {
  Id?: string | null;
  id?: string | null;
  ColumnKey?: string | null;
  columnKey?: string | null;
  DataIndex?: string | null;
  dataIndex?: string | null;
  Field?: string | null;
  field?: string | null;
  ColumnName?: string | null;
  columnName?: string | null;
  Key?: string | null;
  key?: string | null;
  HeaderName?: string | null;
  headerName?: string | null;
  Title?: string | null;
  title?: string | null;
  Visible?: DynamicUiVisibility;
  visible?: DynamicUiVisibility;
  Sortable?: boolean | null;
  sortable?: boolean | null;
  Filterable?: boolean | null;
  filterable?: boolean | null;
  Width?: number | null;
  width?: number | null;
  Align?: DynamicUiAlign | null;
  align?: DynamicUiAlign | null;
  RenderType?: string | null;
  renderType?: string | null;
  DataType?: string | null;
  dataType?: string | null;
  IsActionColumn?: boolean | null;
  isActionColumn?: boolean | null;
  Order?: number | null;
  order?: number | null;
  Behavior?: string | null;
  behavior?: string | null;
  Presentation?: string | null;
  presentation?: string | null;
  FilterType?: string | null;
  filterType?: string | null;
  AgGridFilterType?: string | null;
  agGridFilterType?: string | null;
  FilterOptions?: unknown;
  filterOptions?: unknown;
  Metadata?: DynamicUiUnknownRecord | null;
  metadata?: DynamicUiUnknownRecord | null;
};

export type UiRowDto = {
  Id?: string | number | null;
  id?: string | number | null;
  Key?: string | number | null;
  key?: string | number | null;
  Values?: DynamicUiUnknownRecord | null;
  values?: DynamicUiUnknownRecord | null;
  Meta?: DynamicUiUnknownRecord | null;
  meta?: DynamicUiUnknownRecord | null;
};

export type DynamicUiRowsOnlyDto = {
  Rows?: UiRowDto[] | null;
  rows?: UiRowDto[] | null;
  Pagination?: DynamicUiPaginationDto | null;
  pagination?: DynamicUiPaginationDto | null;
  Sorting?: DynamicUiSortingDto | null;
  sorting?: DynamicUiSortingDto | null;
  Metadata?: DynamicUiUnknownRecord | null;
  metadata?: DynamicUiUnknownRecord | null;
};

export type DynamicUiTableDto = {
  TableId?: string | null;
  tableId?: string | null;
  Title?: string | null;
  title?: string | null;
  Columns?: UiColumnDto[] | null;
  columns?: UiColumnDto[] | null;
  Rows?: UiRowDto[] | null;
  rows?: UiRowDto[] | null;
  ToolbarActions?: UiActionDto[] | null;
  toolbarActions?: UiActionDto[] | null;
  BulkActions?: UiActionDto[] | null;
  bulkActions?: UiActionDto[] | null;
  RowActions?: UiActionDto[] | null;
  rowActions?: UiActionDto[] | null;
  CellActions?: UiCellActionDto[] | null;
  cellActions?: UiCellActionDto[] | null;
  UserClaims?: string[] | null;
  userClaims?: string[] | null;
  Pagination?: DynamicUiPaginationDto | null;
  pagination?: DynamicUiPaginationDto | null;
  Sorting?: DynamicUiSortingDto | null;
  sorting?: DynamicUiSortingDto | null;
  meta?: DynamicUiUnknownRecord | null;
  Metadata?: DynamicUiUnknownRecord | null;
  metadata?: DynamicUiUnknownRecord | null;
};

export type AppGridCellAction = {
  actionId: string;
  label: string;
  placement: string;
  presentation: string;
  behavior: string;
  behaviorConfig?: DynamicUiUnknownRecord;
  request?: DynamicUiUnknownRecord;
  icon?: string;
  tone?: string;
  requiresConfirm?: boolean;
  confirmTitle?: string;
  confirmMessage?: string;
  requiredClaimsAny?: string[];
  requiredClaimsAll?: string[];
  claimKey?: string;
  rules?: DynamicUiUnknownRecord;
  payload?: DynamicUiUnknownRecord;
  metadata?: DynamicUiUnknownRecord;
};

export type AppGridColumn = {
  field: string;
  headerName: string;
  visible: boolean;
  sortable: boolean;
  filterable: boolean;
  width?: number;
  align?: DynamicUiAlign;
  isActionColumn?: boolean;
  renderType?: string;
  dataType?: string;
  order?: number;
  filterType?: string;
  agGridFilterType?: string;
  filterOptions?: unknown;
  actions?: AppGridCellAction[];
  metadata?: DynamicUiUnknownRecord;
};

export type AppGridRow = {
  id: string;
  data: DynamicUiUnknownRecord;
  meta?: DynamicUiUnknownRecord;
};

export type AppTableRow = Record<string, unknown>;

export type AppDataTableAgGrid = {
  tableId?: string;
  title?: string;
  columns: AppGridColumn[];
  rows: AppGridRow[];
  pagination?: DynamicUiPaginationDto;
  sorting?: DynamicUiSortingDto;
  toolbarActions?: AppGridCellAction[];
  bulkActions?: AppGridCellAction[];
  rowActions?: AppGridCellAction[];
  userClaims?: string[];
  metadata?: DynamicUiUnknownRecord;
};

export type AppGridColumnWithColDef = AppGridColumn & {
  colDef: ColDef<DynamicUiUnknownRecord>;
};
