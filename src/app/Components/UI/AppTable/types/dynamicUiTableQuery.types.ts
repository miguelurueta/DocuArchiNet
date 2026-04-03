import type {
  ApiResponse,
  AppGridColumn,
  AppGridRow,
  DynamicUiTableDto,
} from "./dynamicUiTable.types";

export type DynamicTableQueryInput = {
  tableId: string;
  page?: number;
  pageSize?: number;
  search?: string;
  sortField?: string;
  sortDirection?: "asc" | "desc";
  includeConfig?: boolean;
};

export type RequestMapper<TRequest> = (input: DynamicTableQueryInput) => TRequest;

export type DynamicUiTableQueryPagination = {
  page: number;
  pageSize: number;
};

export type DynamicUiTableQueryResult = {
  tableId?: string;
  rows: AppGridRow[];
  columns: AppGridColumn[];
  userClaims?: string[];
  total: number;
  pagination: DynamicUiTableQueryPagination;
  loading: boolean;
  error: Error | null;
  isEmpty: boolean;
  refetch: () => void;
  rawResponse?: ApiResponse<DynamicUiTableDto | null>;
};

export type UseDynamicUiTableQueryParams<TRequest> = {
  input: DynamicTableQueryInput;
  requestMapper: RequestMapper<TRequest>;
  queryFn: (request: TRequest) => Promise<ApiResponse<DynamicUiTableDto | null>>;
  enabled?: boolean;
};
