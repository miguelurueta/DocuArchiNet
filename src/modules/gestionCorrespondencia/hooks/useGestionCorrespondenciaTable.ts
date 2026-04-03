import { useMemo, useState } from "react";
import { getDynamicTable } from "../../../app/Components/UI/AppTable/services/dynamicUiTable.service";
import { mapAppGridColumnsToAppTableColumns } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableColumns";
import { mapAppGridRowsToAppTableRows } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableRows";
import { useDynamicUiTableQuery } from "../../../app/Components/UI/AppTable/hooks/useDynamicUiTableQuery";
import type { AppTableRow } from "../../../app/Components/UI/AppTable/AppTable.types";
import type { ColDef } from "ag-grid-community";
import { mapGestionCorrespondenciaTableRequest } from "../adapters/gestionCorrespondenciaTableRequestMapper";

export type GestionCorrespondenciaTableResult<T extends AppTableRow = AppTableRow> = {
  rows: T[];
  columns: ColDef<T>[];
  total: number;
  page: number;
  pageSize: number;
  search: string;
  category?: string;
  loading: boolean;
  error: Error | null;
  isEmpty: boolean;
  hasLoadedOnce: boolean;
  setSearch: (value: string) => void;
  setCategory: (value: string | undefined) => void;
  setPageSize: (value: number) => void;
  refetch: () => void;
};

const DEFAULT_PAGE_SIZE = 25;

export const useGestionCorrespondenciaTable = <
  T extends AppTableRow = AppTableRow,
>(): GestionCorrespondenciaTableResult<T> => {
  const [search, setSearch] = useState("");
  const [category, setCategory] = useState<string | undefined>();
  const [pageSize, setPageSize] = useState(DEFAULT_PAGE_SIZE);

  const query = useDynamicUiTableQuery({
    input: {
      tableId: "workflowInboxgestion",
      page: 1,
      pageSize,
      search,
      sortField: "fecha_inicio",
      sortDirection: "desc",
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
    () => mapAppGridColumnsToAppTableColumns<T>(query.columns),
    [query.columns],
  );

  return {
    rows,
    columns,
    total: query.total,
    page: query.pagination.page,
    pageSize: query.pagination.pageSize,
    search,
    category,
    loading: query.loading,
    error: query.error,
    isEmpty: query.isEmpty,
    hasLoadedOnce: Boolean(query.rawResponse) || Boolean(query.error),
    setSearch,
    setCategory,
    setPageSize,
    refetch: query.refetch,
  };
};
