import type {
  AppTableQueryState,
  AppTableStructuredFilter,
  AppTableStructuredFilterOperator,
} from "../types/appTableQueryState.types";

const DEFAULT_PAGE = 1;
const DEFAULT_PAGE_SIZE = 25;

const normalizePositiveNumber = (value: number | undefined, fallback: number): number =>
  typeof value === "number" && Number.isFinite(value) && value > 0 ? value : fallback;

const normalizeSearch = (value: string | undefined): string => value ?? "";

const normalizeSortDir = (value: "asc" | "desc" | undefined): "asc" | "desc" | undefined => {
  if (value === "asc" || value === "desc") {
    return value;
  }

  return undefined;
};

const normalizeFilterOperator = (value: AppTableStructuredFilterOperator): AppTableStructuredFilterOperator => value;

const normalizeStructuredFilter = (filter: AppTableStructuredFilter): AppTableStructuredFilter => ({
  field: filter.field,
  operator: normalizeFilterOperator(filter.operator),
  value: filter.value,
  valueFrom: filter.valueFrom,
  valueTo: filter.valueTo,
});

const normalizeStructuredFilters = (
  filters: AppTableStructuredFilter[] | undefined,
): AppTableStructuredFilter[] => (filters ?? []).map(normalizeStructuredFilter);

const stableSerializeValue = (value: unknown): string => {
  if (value === null) {
    return "null";
  }

  if (value === undefined) {
    return "undefined";
  }

  if (typeof value === "number" || typeof value === "boolean") {
    return JSON.stringify(value);
  }

  if (typeof value === "string") {
    return JSON.stringify(value);
  }

  if (Array.isArray(value)) {
    return `[${value.map(stableSerializeValue).join(",")}]`;
  }

  if (typeof value === "object") {
    const entries = Object.entries(value as Record<string, unknown>)
      .sort(([left], [right]) => left.localeCompare(right))
      .map(([key, entryValue]) => `${JSON.stringify(key)}:${stableSerializeValue(entryValue)}`);

    return `{${entries.join(",")}}`;
  }

  return JSON.stringify(String(value));
};

const hasEffectiveValueChange = (left: unknown, right: unknown): boolean =>
  stableSerializeValue(left) !== stableSerializeValue(right);

const normalizeQueryState = (state: AppTableQueryState): AppTableQueryState => ({
  page: normalizePositiveNumber(state.page, DEFAULT_PAGE),
  pageSize: normalizePositiveNumber(state.pageSize, DEFAULT_PAGE_SIZE),
  search: normalizeSearch(state.search),
  searchType: state.searchType,
  structuredFilters: normalizeStructuredFilters(state.structuredFilters),
  sortField: state.sortField,
  sortDir: normalizeSortDir(state.sortDir),
});

const shouldResetPage = (prev: AppTableQueryState, next: AppTableQueryState): boolean =>
  hasEffectiveValueChange(prev.search, next.search) ||
  hasEffectiveValueChange(prev.searchType, next.searchType) ||
  hasEffectiveValueChange(prev.structuredFilters, next.structuredFilters) ||
  hasEffectiveValueChange(prev.sortField, next.sortField) ||
  hasEffectiveValueChange(prev.sortDir, next.sortDir) ||
  hasEffectiveValueChange(prev.pageSize, next.pageSize);

export const getDefaultAppTableQueryState = (): AppTableQueryState => ({
  page: DEFAULT_PAGE,
  pageSize: DEFAULT_PAGE_SIZE,
  search: "",
  structuredFilters: [],
  sortField: undefined,
  sortDir: undefined,
  searchType: undefined,
});

export const createAppTableQueryState = (
  initialState?: Partial<AppTableQueryState>,
): AppTableQueryState =>
  normalizeQueryState({
    ...getDefaultAppTableQueryState(),
    ...initialState,
    structuredFilters:
      initialState?.structuredFilters ?? getDefaultAppTableQueryState().structuredFilters,
  });

export const updateAppTableQueryState = (
  prev: AppTableQueryState,
  patch: Partial<AppTableQueryState>,
): AppTableQueryState => {
  const normalizedPrev = normalizeQueryState(prev);
  const normalizedNext = normalizeQueryState({
    ...normalizedPrev,
    ...patch,
    structuredFilters:
      patch.structuredFilters === undefined ? normalizedPrev.structuredFilters : patch.structuredFilters,
  });

  if (!shouldResetPage(normalizedPrev, normalizedNext)) {
    return normalizedNext;
  }

  return {
    ...normalizedNext,
    page: DEFAULT_PAGE,
  };
};

export const serializeAppTableQueryState = (
  state: AppTableQueryState,
): Record<string, unknown> => {
  const normalized = normalizeQueryState(state);

  return {
    page: normalized.page,
    pageSize: normalized.pageSize,
    search: normalized.search,
    searchType: normalized.searchType,
    structuredFilters: normalized.structuredFilters.map((filter) => ({ ...filter })),
    sortField: normalized.sortField,
    sortDir: normalized.sortDir,
  };
};
