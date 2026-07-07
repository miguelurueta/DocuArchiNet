import type { DynamicTableQueryInput } from "../../../app/Components/UI/AppTable/types/dynamicUiTableQuery.types";
import type { RadicacionPendientesTableRequest } from "../services/radicacionPendientes.service";

export const mapRadicacionPendientesTableRequest = ({
  page = 1,
  pageSize = 10,
  search = "",
  searchType = 1,
  sortField = "id_estado_radicado",
  sortDir = "desc",
  includeConfig = true,
}: DynamicTableQueryInput): RadicacionPendientesTableRequest => ({
  SearchType: searchType,
  Search: search,
  SortField: sortField,
  SortDir: sortDir === "asc" ? "ASC" : "DESC",
  Page: page,
  PageSize: pageSize,
  IncludeConfig: includeConfig,
});
