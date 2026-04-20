import { UndoOutlined } from "@ant-design/icons";
import type { ColDef } from "ag-grid-community";
import { useCallback, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppInputSearch } from "../../../app/Components/UI/AppInputSearch";
import { AppTableExport } from "../../../app/Components/UI/AppTable/AppTableExport";
import { AppTableQueryWrapper } from "../../../app/Components/UI/AppTable/AppTableQueryWrapper";
import AppTable from "../../../app/Components/UI/AppTable/AppTable";
import type {
  AppTableCellClick,
  AppTableActionTriggered,
  AppTableRow,
} from "../../../app/Components/UI/AppTable/AppTable.types";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";
import type { GestionCorrespondenciaTableResult } from "../hooks/useGestionCorrespondenciaTable";
import { useWorkflowInboxAutocomplete } from "../hooks/useWorkflowInboxAutocomplete";
import styles from "../style/GestionCorrespondencia.module.css";

type GestionCorrespondenciaProps<T extends AppTableRow = AppTableRow> = {
  table: GestionCorrespondenciaTableResult<T>;
};

const EXPORT_FORMATS = ["csv", "xlsx", "pdf"] as const;
const EXPORT_ENABLED_MODES = ["currentPage", "selectedRows", "allMatching"] as const;
const RESPONSIVE_PRESENTATION = {
  enabled: true,
  cardsBelow: 768,
} as const;

export default function GestionCorrespondencia<T extends AppTableRow = AppTableRow>({
  table,
}: GestionCorrespondenciaProps<T>) {
  const navigate = useNavigate();
  const [selectedRows, setSelectedRows] = useState<T[]>([]);
  const [searchDraft, setSearchDraft] = useState(table.queryState.search);
  const autocomplete = useWorkflowInboxAutocomplete({
    minLength: 2,
    limit: 10,
  });

  const exportReportMeta = useMemo(
    () => ({
      reportName: "Bandeja de gestion correspondencia",
      generatedBy: "DocuArchiCore",
      moduleName: "Gestion Correspondencia",
      reportType: "Operativo",
      generatedAt: new Date().toISOString(),
      rowCount: table.rows.length,
      description: "Exportacion desde la bandeja operativa",
      companyImageAsset: "public/branding/reports/company-report-logo.png",
    }),
    [table.rows.length],
  );
  const getCurrentPageRows = useCallback(() => table.rows, [table.rows]);
  const getSelectedRows = useCallback(() => selectedRows, [selectedRows]);

  const applySearch = useCallback((search: string) => {
    const normalizedSearch = search.trim();
    setSearchDraft(normalizedSearch);
    autocomplete.setSearchText(normalizedSearch);
    table.onQueryChange({ search: normalizedSearch });
  }, [autocomplete, table]);

  const handleSearchChange = useCallback((search: string) => {
    setSearchDraft(search);
    autocomplete.setSearchText(search);
  }, [autocomplete]);

  const handleSearchClear = useCallback(() => {
    setSearchDraft("");
    autocomplete.clear();
    table.onQueryChange({ search: "" });
  }, [autocomplete, table]);

  const navigateToRowDetail = useCallback((row: T) => {
    const rowId = row.id;
    if (typeof rowId !== "string" && typeof rowId !== "number") {
      return;
    }

    navigate(`respuesta/${String(rowId)}`);
  }, [navigate]);

  const handleTableAction = useCallback(({ actionId, row }: AppTableActionTriggered<T>) => {
    if (actionId !== "gestionar_tramite" && actionId !== "gestionar_tramite_menu") {
      return;
    }

    navigateToRowDetail(row);
  }, [navigateToRowDetail]);

  const handleTableCellClick = useCallback(({ row, field }: AppTableCellClick<T>) => {
    if (!field || field === "acciones" || field === "ag-Grid-SelectionColumn") {
      return;
    }

    navigateToRowDetail(row);
  }, [navigateToRowDetail]);

  const exportDataSource = useMemo(
    () => ({
      getCurrentPageRows,
      getSelectedRows,
      getAllMatchingRows: table.getAllMatchingRows,
      getBackendExportFile: table.getBackendExportFile,
    }),
    [getCurrentPageRows, getSelectedRows, table.getAllMatchingRows, table.getBackendExportFile],
  );
  const paginationActions = useMemo(
    () => (
      <AppTableExport
        columns={table.columns as ColDef<T>[]}
        dataSource={exportDataSource}
        formats={EXPORT_FORMATS}
        reportMeta={exportReportMeta}
        enabledModes={EXPORT_ENABLED_MODES}
      />
    ),
    [exportDataSource, exportReportMeta, table.columns],
  );

  return (
    <div className={styles.shell}>
      <AppToolbar
        className={styles.toolbar}
        actionContent={
          <div className={styles.toolbarActionGroup}>
            <AppInputSearch
              aria-label="Buscar tareas workflow"
              className={styles.toolbarSearch}
              debounceMs={0}
              loading={autocomplete.loading}
              options={autocomplete.items}
              placeholder="Buscar tareas workflow"
              value={searchDraft}
              onChange={handleSearchChange}
              onClear={handleSearchClear}
              onSearch={applySearch}
            />

            <AppButton
              className={styles.toolbarControl}
              variant="ghost"
              size="sm"
              leftIcon={<UndoOutlined />}
              loading={table.loading && table.hasLoadedOnce}
              fullWidth
              onClick={table.refetch}
            >
              Actualizar
            </AppButton>
          </div>
        }
      />

      <AppContent
        className={styles.content}
        contentClassName={styles.contentBody}
        data-testid="gestion-correspondencia-content"
        width="full"
        density="compact"
      >
        <div className={styles.page}>
          <AppTableQueryWrapper
            className={styles.queryWrapper}
            queryState={table.queryState}
            onQueryChange={table.onQueryChange}
            total={table.total}
            loading={table.loading && table.hasLoadedOnce}
            showSearch={false}
            paginationActions={paginationActions}
          >
            <div className={styles.tableWrapper}>
              <AppTable
                rows={table.rows}
                columns={table.columns as ColDef<T>[]}
                rowSelection="single"
                rowClickAffordance
                rowClickTooltip="Gestionar trámite"
                total={table.total}
                loading={table.loading && table.hasLoadedOnce}
                paginationMode="server"
                layoutMode="fill"
                responsivePresentation={RESPONSIVE_PRESENTATION}
                onCellClicked={handleTableCellClick}
                onActionTriggered={handleTableAction}
                onSelectionChanged={setSelectedRows}
              />
            </div>
          </AppTableQueryWrapper>
        </div>
      </AppContent>
    </div>
  );
}
