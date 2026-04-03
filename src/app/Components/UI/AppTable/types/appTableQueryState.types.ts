export type AppTableSearchType = 1 | 2 | 3;

export type AppTableStructuredFilterOperator =
  | "eq"
  | "neq"
  | "contains"
  | "startsWith"
  | "endsWith"
  | "gt"
  | "gte"
  | "lt"
  | "lte"
  | "between"
  | "isNull"
  | "isNotNull";

export type AppTableStructuredFilter = {
  field: string;
  operator: AppTableStructuredFilterOperator;
  value?: unknown;
  valueFrom?: unknown;
  valueTo?: unknown;
};

export type AppTableQueryState = {
  page: number;
  pageSize: number;
  search: string;
  searchType?: AppTableSearchType;
  structuredFilters: AppTableStructuredFilter[];
  sortField?: string;
  sortDir?: "asc" | "desc";
};
