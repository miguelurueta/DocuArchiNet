import type { DynamicTableQueryInput } from "../../../app/Components/UI/AppTable/types/dynamicUiTableQuery.types";

export type GestionCorrespondenciaTableRequest = {
  TableId: string;
  Page?: number;
  PageSize?: number;
  Search?: string;
  SortField?: string;
  SortDir?: "ASC" | "DESC";
  IncludeConfig?: boolean;
};

export const mapGestionCorrespondenciaTableRequest = (
  input: DynamicTableQueryInput,
): GestionCorrespondenciaTableRequest => ({
  TableId: input.tableId,
  Page: input.page,
  PageSize: input.pageSize,
  Search: input.search?.trim() || undefined,
  SortField: input.sortField,
  SortDir: input.sortDirection?.toUpperCase() as "ASC" | "DESC" | undefined,
  IncludeConfig: input.includeConfig,
});
