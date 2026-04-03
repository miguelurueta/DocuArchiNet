import type {
  AppTableStructuredFilter,
} from "../types/appTableQueryState.types";
import type { DynamicTableQueryInput } from "../types/dynamicUiTableQuery.types";

export type DynamicUiStructuredFilterRequest = {
  Field: string;
  Operator: AppTableStructuredFilter["operator"];
  Value?: unknown;
  ValueFrom?: unknown;
  ValueTo?: unknown;
};

export type DynamicUiServerTableRequest = {
  TableId: string;
  Page?: number;
  PageSize?: number;
  Search?: string;
  SearchType?: number;
  StructuredFilters?: DynamicUiStructuredFilterRequest[];
  SortField?: string;
  SortDir?: "ASC" | "DESC";
  IncludeConfig?: boolean;
};

const resolveSortDirection = (
  input: DynamicTableQueryInput,
): "ASC" | "DESC" | undefined => {
  const value = input.sortDir ?? input.sortDirection;

  if (value === "asc") {
    return "ASC";
  }

  if (value === "desc") {
    return "DESC";
  }

  return undefined;
};

const mapStructuredFilter = (
  filter: AppTableStructuredFilter,
): DynamicUiStructuredFilterRequest => ({
  Field: filter.field,
  Operator: filter.operator,
  Value: filter.value,
  ValueFrom: filter.valueFrom,
  ValueTo: filter.valueTo,
});

export const mapDynamicUiServerTableRequest = (
  input: DynamicTableQueryInput,
): DynamicUiServerTableRequest => ({
  TableId: input.tableId,
  Page: input.page,
  PageSize: input.pageSize,
  Search: input.search?.trim() || undefined,
  SearchType: input.searchType,
  StructuredFilters:
    input.structuredFilters && input.structuredFilters.length > 0
      ? input.structuredFilters.map(mapStructuredFilter)
      : undefined,
  SortField: input.sortField,
  SortDir: resolveSortDirection(input),
  IncludeConfig: input.includeConfig,
});
