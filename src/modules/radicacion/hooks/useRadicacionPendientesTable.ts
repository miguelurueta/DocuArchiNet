import { useMemo } from "react";
import { mapAppGridColumnsToAppTableColumns } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableColumns";
import { mapAppGridRowsToAppTableRows } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableRows";
import type { AppTableRow } from "../../../app/Components/UI/AppTable/AppTable.types";
import { useAppTableQueryState } from "../../../app/Components/UI/AppTable/hooks/useAppTableQueryState";
import { useDynamicUiTableQuery } from "../../../app/Components/UI/AppTable/hooks/useDynamicUiTableQuery";
import { mapRadicacionPendientesTableRequest } from "../adapters/radicacionPendientesTableRequestMapper";
import {
  fetchRadicacionPendientesTable,
  type RadicacionPendientesTableRequest,
} from "../services/radicacionPendientes.service";
import { RADICACION_PENDIENTE_ACTION_ID } from "../types/radicacionPendientes.types";
import type { AppGridColumn } from "../../../app/Components/UI/AppTable/types/dynamicUiTable.types";

const RADICACION_PENDIENTES_TABLE_ID = "radicacionPendientes";

const normalizePendientesActionPresentation = (
  columns: ReadonlyArray<AppGridColumn>,
): AppGridColumn[] =>
  columns.map((column) => ({
    ...column,
    actions: column.actions?.map((action) =>
      action.actionId === RADICACION_PENDIENTE_ACTION_ID &&
      action.presentation === "button"
        ? { ...action, presentation: "icon_button", icon: action.icon ?? "edit" }
        : action,
    ),
  }));

export function useRadicacionPendientesTable(enabled: boolean) {
  const { queryState, onQueryChange } = useAppTableQueryState({
    page: 1,
    pageSize: 10,
    search: "",
    sortField: "id_estado_radicado",
    sortDir: "desc",
  });

  const query = useDynamicUiTableQuery<RadicacionPendientesTableRequest>({
    input: {
      tableId: RADICACION_PENDIENTES_TABLE_ID,
      page: queryState.page,
      pageSize: queryState.pageSize,
      search: queryState.search,
      searchType: queryState.searchType,
      structuredFilters: queryState.structuredFilters,
      sortField: queryState.sortField,
      sortDir: queryState.sortDir,
      includeConfig: true,
    },
    requestMapper: mapRadicacionPendientesTableRequest,
    queryFn: fetchRadicacionPendientesTable,
    enabled,
  });

  const rows = useMemo(
    () => mapAppGridRowsToAppTableRows<AppTableRow>(query.rows),
    [query.rows],
  );
  const normalizedColumns = useMemo(
    () => normalizePendientesActionPresentation(query.columns),
    [query.columns],
  );
  const columns = useMemo(
    () =>
      mapAppGridColumnsToAppTableColumns<AppTableRow>(normalizedColumns, {
        tableId: query.tableId,
        menuActions: query.menuActions,
        userClaims: query.userClaims,
      }),
    [normalizedColumns, query.menuActions, query.tableId, query.userClaims],
  );

  return {
    rows,
    columns,
    total: query.total,
    queryState: {
      ...queryState,
      page: query.pagination.page,
      pageSize: query.pagination.pageSize,
    },
    onQueryChange,
    loading: query.loading,
    error: query.error,
    isEmpty: query.isEmpty,
    refetch: query.refetch,
  };
}
