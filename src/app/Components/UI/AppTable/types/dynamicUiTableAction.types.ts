import type { ICellRendererParams } from "ag-grid-community";
import type {
  ApiResponse,
  AppGridCellAction,
  AppGridColumn,
  AppGridRow,
  AppTableRow,
  DynamicUiUnknownRecord,
  UiActionDto,
} from "./dynamicUiTable.types";

export type DynamicUiExecutableAction = AppGridCellAction | UiActionDto;

export type DynamicUiActionExecutionRequest = {
  tableId: string;
  actionId: string;
  rowId?: string;
  columnKey?: string;
  selectedRowIds?: string[];
  payload?: Record<string, unknown>;
};

export type DynamicUiActionExecutionResult = {
  success: boolean;
  message?: string;
  data?: Record<string, unknown> | null;
  rawResponse?: unknown;
};

export type DynamicUiActionContext = {
  tableId?: string;
  row?: AppGridRow;
  selectedRows?: AppGridRow[];
  columnKey?: string;
  userClaims?: string[];
};

export type DynamicUiActionAvailabilityResult = {
  isVisible: boolean;
  isEnabled: boolean;
  reasons?: string[];
};

export type DynamicUiResolvedBehavior = {
  kind: string;
  rawValue: string;
  isKnown: boolean;
  config?: Record<string, unknown>;
};

export type DynamicUiResolvedPresentation = {
  kind: string;
  rawValue: string;
  isKnown: boolean;
  config?: Record<string, unknown>;
};

export type DynamicUiActionHookResult = {
  executeAction: (
    input: DynamicUiActionExecutionRequest,
  ) => Promise<DynamicUiActionExecutionResult>;
  buildActionPayload: (
    context: DynamicUiActionContext,
    action: DynamicUiExecutableAction,
    manualPayload?: Record<string, unknown>,
  ) => Record<string, unknown>;
  evaluateActionAvailability: (
    action: DynamicUiExecutableAction,
    context: DynamicUiActionContext,
  ) => DynamicUiActionAvailabilityResult;
  resolveActionBehavior: (
    action: DynamicUiExecutableAction,
  ) => DynamicUiResolvedBehavior;
  resolveActionPresentation: (
    action: DynamicUiExecutableAction,
  ) => DynamicUiResolvedPresentation;
  isExecutingAction: boolean;
  actionError: Error | null;
  lastActionResult: DynamicUiActionExecutionResult | null;
};

export type AppTableActionCellRendererParams = ICellRendererParams<AppTableRow> & {
  appGridColumn: AppGridColumn;
  actions: AppGridCellAction[];
  menuActions?: AppGridCellAction[];
  tableId?: string;
  userClaims?: string[];
};

export type UseDynamicUiTableActionsParams = {
  endpoint?: string;
  executeActionFn?: DynamicUiExecuteActionFn;
};

export type DynamicUiExecuteActionFn = (
  request: DynamicUiActionExecutionRequest,
) => Promise<ApiResponse<unknown>>;

export type DynamicUiActionServiceFactory = (
  endpoint?: string,
) => DynamicUiExecuteActionFn;

export type DynamicUiActionRuleSet = DynamicUiUnknownRecord;
