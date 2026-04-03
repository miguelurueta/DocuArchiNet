import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import { mapDynamicUiTableToAppDataTableAgGrid } from "../adapters/dynamicUiToAgGridColumns";
import type { ApiResponse, DynamicUiPaginationDto, DynamicUiTableDto } from "../types/dynamicUiTable.types";
import type {
  DynamicTableQueryInput,
  DynamicUiTableQueryPagination,
  DynamicUiTableQueryResult,
  UseDynamicUiTableQueryParams,
} from "../types/dynamicUiTableQuery.types";

type DynamicUiTableQueryData = {
  tableId?: string;
  rows: DynamicUiTableQueryResult["rows"];
  columns: DynamicUiTableQueryResult["columns"];
  userClaims?: string[];
  total: number;
  pagination: DynamicUiTableQueryPagination;
  isEmpty: boolean;
  rawResponse?: ApiResponse<DynamicUiTableDto | null>;
};

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 25;

const resolvePageNumber = (...values: Array<number | null | undefined>): number => {
  const match = values.find((value) => typeof value === "number" && Number.isFinite(value) && value > 0);
  return match ?? DEFAULT_PAGE;
};

const resolvePagination = (
  pagination: DynamicUiPaginationDto | null | undefined,
  input: DynamicTableQueryInput,
): DynamicUiTableQueryPagination => ({
  page: resolvePageNumber(pagination?.page, pagination?.Page, input.page),
  pageSize: resolvePageNumber(pagination?.pageSize, pagination?.PageSize, input.pageSize, DEFAULT_PAGE_SIZE),
});

const resolveTotal = (pagination: DynamicUiPaginationDto | null | undefined): number => {
  const total = [pagination?.total, pagination?.Total].find(
    (value) => typeof value === "number" && Number.isFinite(value) && value >= 0,
  );
  return total ?? 0;
};

const isSuccessfulResponse = (response: ApiResponse<DynamicUiTableDto | null>): boolean => {
  if (typeof response.success === "boolean") {
    return response.success;
  }

  if (typeof response.Success === "boolean") {
    return response.Success;
  }

  return true;
};

const resolveMessage = (response: ApiResponse<DynamicUiTableDto | null>): string =>
  response.message?.trim() ||
  response.Message?.trim() ||
  response.errors?.find((error) => error?.message)?.message?.trim() ||
  response.Errors?.find((error) => error?.message)?.message?.trim() ||
  "Dynamic UI table query failed";

const resolveTable = (response: ApiResponse<DynamicUiTableDto | null>): DynamicUiTableDto | null =>
  response.data ?? response.Data ?? null;

const normalizeError = (error: unknown): Error => {
  if (error instanceof Error) {
    return error;
  }

  if (
    typeof error === "object" &&
    error !== null &&
    "message" in error &&
    typeof error.message === "string" &&
    error.message.trim().length > 0
  ) {
    return new Error(error.message);
  }

  return new Error("Dynamic UI table query failed");
};

const buildQueryData = (
  response: ApiResponse<DynamicUiTableDto | null>,
  input: DynamicTableQueryInput,
): DynamicUiTableQueryData => {
  if (!isSuccessfulResponse(response)) {
    throw new Error(resolveMessage(response));
  }

  const table = resolveTable(response);
  const mapped = mapDynamicUiTableToAppDataTableAgGrid(table);
  const pagination = resolvePagination(table?.pagination ?? table?.Pagination, input);
  const total = resolveTotal(table?.pagination ?? table?.Pagination);

  return {
    tableId: mapped.tableId ?? input.tableId,
    rows: mapped.rows,
    columns: mapped.columns,
    userClaims: mapped.userClaims,
    total,
    pagination,
    isEmpty: mapped.rows.length === 0 && mapped.columns.length === 0,
    rawResponse: response,
  };
};

export function useDynamicUiTableQuery<TRequest>({
  input,
  requestMapper,
  queryFn,
  enabled = true,
}: UseDynamicUiTableQueryParams<TRequest>): DynamicUiTableQueryResult {
  const query = useQuery<DynamicUiTableQueryData, Error>({
    queryKey: [
      "dynamic-ui-table",
      input.tableId,
      input.page,
      input.pageSize,
      input.search,
      input.sortField,
      input.sortDirection,
      input.includeConfig,
    ],
    queryFn: async () => {
      const request = requestMapper(input);
      const response = await queryFn(request);
      return buildQueryData(response, input);
    },
    enabled,
    retry: false,
  });

  const data = useMemo<DynamicUiTableQueryData>(() => {
    if (query.data) {
      return query.data;
    }

    return {
      tableId: input.tableId,
      rows: [],
      columns: [],
      userClaims: [],
      total: 0,
      pagination: resolvePagination(undefined, input),
      isEmpty: true,
      rawResponse: undefined,
    };
  }, [input, query.data]);

  return {
    tableId: data.tableId,
    rows: data.rows,
    columns: data.columns,
    userClaims: data.userClaims,
    total: data.total,
    pagination: data.pagination,
    loading: query.isFetching,
    error: query.error ? normalizeError(query.error) : null,
    isEmpty: data.isEmpty,
    refetch: () => {
      void query.refetch();
    },
    rawResponse: data.rawResponse,
  };
}
