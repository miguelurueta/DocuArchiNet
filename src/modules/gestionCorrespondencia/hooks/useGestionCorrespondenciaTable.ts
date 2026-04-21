import { useCallback, useMemo, useRef, useState } from "react";
import {
  exportAppTableFile,
  getDynamicTable,
} from "../../../app/Components/UI/AppTable/services/dynamicUiTable.service";
import { mapAppGridColumnsToAppTableColumns } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableColumns";
import { mapAppGridRowsToAppTableRows } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableRows";
import { useDynamicUiTableQuery } from "../../../app/Components/UI/AppTable/hooks/useDynamicUiTableQuery";
import { useAppTableQueryState } from "../../../app/Components/UI/AppTable/hooks/useAppTableQueryState";
import type { AppTableQueryState } from "../../../app/Components/UI/AppTable/types/appTableQueryState.types";
import type {
  AppTableBackendExportFile,
  AppTableBackendExportRequest,
} from "../../../app/Components/UI/AppTable/AppTableExport.types";
import type { AppGridRow, AppTableRow, UiRowDto } from "../../../app/Components/UI/AppTable/types/dynamicUiTable.types";
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
  getAllMatchingRows: () => Promise<T[]>;
  getBackendExportFile: (
    request: AppTableBackendExportRequest<T>,
  ) => Promise<AppTableBackendExportFile>;
};

const GESTION_CORRESPONDENCIA_TABLE_ID = "workflowInboxgestion";
const GESTION_CORRESPONDENCIA_EXPORT_COLUMN_MODE = 2;

const resolveAllMatchingPageSize = (total: number, fallbackPageSize: number): number => {
  const candidates = [total, fallbackPageSize].filter(
    (value) => Number.isFinite(value) && value > 0,
  );

  return candidates.length > 0 ? Math.max(...candidates) : 1;
};

const mapUiRowsToAppGridRows = (rows: UiRowDto[] | null | undefined): AppGridRow[] =>
  (rows ?? []).map((row) => ({
    id: String(row.Id ?? row.id ?? row.Key ?? row.key ?? ""),
    data: row.Values ?? row.values ?? {},
    meta: row.Meta ?? row.meta ?? undefined,
  }));

type GestionCorrespondenciaExportRequest = {
  ColumnMode: number;
  EstadoTramite: string;
  SearchType: number;
  Search?: string;
  SortField?: string;
  SortDir?: "ASC" | "DESC";
  Page: number;
  PageSize: number;
  Format: AppTableBackendExportRequest["format"];
  ExportMode: AppTableBackendExportRequest["mode"];
  ReportTitle: string;
  StructuredFilters?: GestionCorrespondenciaTableRequest["StructuredFilters"];
};

export const useGestionCorrespondenciaTable = <
  T extends AppTableRow = AppTableRow,
>(): GestionCorrespondenciaTableResult<T> => {
  const [category, setCategory] = useState<string | undefined>();
  const { queryState, onQueryChange } = useAppTableQueryState({
    page: 1,
    pageSize: 25,
    search: "",
    sortField: "fecha_inicio",
    sortDir: "desc",
  });

  const query = useDynamicUiTableQuery<GestionCorrespondenciaTableRequest>({
    input: {
      tableId: GESTION_CORRESPONDENCIA_TABLE_ID,
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
  const latestQueryStateRef = useRef(effectiveQueryState);
  const latestTotalRef = useRef(query.total);

  latestQueryStateRef.current = effectiveQueryState;
  latestTotalRef.current = query.total;

  const getAllMatchingRows = useCallback(async () => {
    const latestQueryState = latestQueryStateRef.current;
    const latestTotal = latestTotalRef.current;
    const response = await getDynamicTable(
      mapGestionCorrespondenciaTableRequest({
        tableId: GESTION_CORRESPONDENCIA_TABLE_ID,
        page: 1,
        pageSize: resolveAllMatchingPageSize(latestTotal, latestQueryState.pageSize),
        search: latestQueryState.search,
        searchType: latestQueryState.searchType,
        structuredFilters: latestQueryState.structuredFilters,
        sortField: latestQueryState.sortField,
        sortDir: latestQueryState.sortDir,
        includeConfig: false,
      }),
    );

    return mapAppGridRowsToAppTableRows<T>(
      mapUiRowsToAppGridRows(response.data?.Rows ?? response.Data?.Rows),
    );
  }, []);

  const getBackendExportFile = useCallback(async ({
    format,
    mode,
    reportMeta,
  }: AppTableBackendExportRequest<T>) => {
    const latestQueryState = latestQueryStateRef.current;
    const latestTotal = latestTotalRef.current;
    const mappedQuery = mapGestionCorrespondenciaTableRequest({
      tableId: GESTION_CORRESPONDENCIA_TABLE_ID,
      page: 1,
      pageSize: resolveAllMatchingPageSize(latestTotal, latestQueryState.pageSize),
      search: latestQueryState.search,
      searchType: latestQueryState.searchType,
      structuredFilters: latestQueryState.structuredFilters,
      sortField: latestQueryState.sortField,
      sortDir: latestQueryState.sortDir,
      includeConfig: false,
    });
    const request: GestionCorrespondenciaExportRequest = {
      ColumnMode: GESTION_CORRESPONDENCIA_EXPORT_COLUMN_MODE,
      EstadoTramite: "",
      SearchType: mappedQuery.SearchType ?? 1,
      Search: mappedQuery.Search,
      SortField: mappedQuery.SortField,
      SortDir: mappedQuery.SortDir,
      Page: 1,
      PageSize: resolveAllMatchingPageSize(latestTotal, latestQueryState.pageSize),
      Format: format,
      ExportMode: mode,
      ReportTitle: reportMeta.reportName,
      StructuredFilters:
        latestQueryState.structuredFilters.length > 0
          ? mappedQuery.StructuredFilters
          : undefined,
    };

    return exportAppTableFile(request);
  }, []);

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
    getAllMatchingRows,
    getBackendExportFile,
  };
};
