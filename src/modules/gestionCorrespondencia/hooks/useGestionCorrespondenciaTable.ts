import { useMemo, useState } from "react";
import { getDynamicTable } from "../../../app/Components/UI/AppTable/services/dynamicUiTable.service";
import { mapAppGridColumnsToAppTableColumns } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableColumns";
import { mapAppGridRowsToAppTableRows } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableRows";
import { useDynamicUiTableQuery } from "../../../app/Components/UI/AppTable/hooks/useDynamicUiTableQuery";
import { useAppTableQueryState } from "../../../app/Components/UI/AppTable/hooks/useAppTableQueryState";
import type { AppTableQueryState } from "../../../app/Components/UI/AppTable/types/appTableQueryState.types";
import type { AppTableRow } from "../../../app/Components/UI/AppTable/AppTable.types";
import type { ColDef } from "ag-grid-community";
import { mapGestionCorrespondenciaTableRequest } from "../adapters/gestionCorrespondenciaTableRequestMapper";
import type { GestionCorrespondenciaTableRequest } from "../adapters/gestionCorrespondenciaTableRequestMapper";

export type GestionCorrespondenciaTableResult<T extends AppTableRow = AppTableRow> = {
  rows: T[];
  columns: ColDef<T>[];
  total: number;
  page: number;
  pageSize: number;
  queryState: AppTableQueryState;
  onQueryChange: (patch: Partial<AppTableQueryState>) => void;
  category?: string;
  loading: boolean;
  error: Error | null;
  isEmpty: boolean;
  hasLoadedOnce: boolean;
  setCategory: (value: string | undefined) => void;
  refetch: () => void;
};

export const useGestionCorrespondenciaTable = <
  T extends AppTableRow = AppTableRow,
>(): GestionCorrespondenciaTableResult<T> => {
  const [category, setCategory] = useState<string | undefined>();
  const { queryState, onQueryChange } = useAppTableQueryState({
    page: 1,
    pageSize: 10,
    search: "",
    sortField: "fecha_inicio",
    sortDir: "desc",
  });

  const query = useDynamicUiTableQuery<GestionCorrespondenciaTableRequest>({
    input: {
      tableId: "workflowInboxgestion",
      page: queryState.page,
      pageSize: queryState.pageSize,
      search: queryState.search,
      searchType: queryState.searchType,
      structuredFilters: queryState.structuredFilters,
      sortField: queryState.sortField,
      sortDir: queryState.sortDir,
      includeConfig: true,
    },
    requestMapper: mapGestionCorrespondenciaTableRequest,
    queryFn: getDynamicTable,
  });

  const rows = useMemo(
    () => mapAppGridRowsToAppTableRows<T>(query.rows),
    [query.rows],
  );
  const columns = useMemo(
    () =>
      mapAppGridColumnsToAppTableColumns<T>(query.columns, {
        tableId: query.tableId,
        menuActions: query.menuActions,
        userClaims: query.userClaims,
      }),
    [query.columns, query.menuActions, query.tableId, query.userClaims],
  );

  const effectiveQueryState = useMemo<AppTableQueryState>(
    () => ({
      ...queryState,
      page: query.pagination.page,
      pageSize: query.pagination.pageSize,
    }),
    [query.pagination.page, query.pagination.pageSize, queryState],
  );

  return {
    rows,
    columns,
    total: query.total,
    page: query.pagination.page,
    pageSize: query.pagination.pageSize,
    queryState: effectiveQueryState,
    onQueryChange,
    category,
    loading: query.loading,
    error: query.error,
    isEmpty: query.isEmpty,
    hasLoadedOnce: Boolean(query.rawResponse) || Boolean(query.error),
    setCategory,
    refetch: query.refetch,
  };
};
