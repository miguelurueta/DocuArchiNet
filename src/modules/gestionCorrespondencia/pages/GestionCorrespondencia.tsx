import { UndoOutlined } from "@ant-design/icons";
import type { ColDef } from "ag-grid-community";
import { useCallback, useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppInputSearch } from "../../../app/Components/UI/AppInputSearch";
import { AppTableExport } from "../../../app/Components/UI/AppTable/AppTableExport";
import type {
  AppTableExportFormat,
  AppTableExportMode,
} from "../../../app/Components/UI/AppTable/AppTableExport.types";
import { AppTableQueryWrapper } from "../../../app/Components/UI/AppTable/AppTableQueryWrapper";
import AppTable from "../../../app/Components/UI/AppTable/AppTable";
import type {
  AppTableCellClick,
  AppTableActionTriggered,
  AppTableRow,
} from "../../../app/Components/UI/AppTable/AppTable.types";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";
import { ReasignarRespuestaModal } from "../components/modalReasignarRespuesta";
import type { GestionCorrespondenciaTableResult } from "../hooks/useGestionCorrespondenciaTable";
import { useWorkflowInboxAutocomplete } from "../hooks/useWorkflowInboxAutocomplete";
import styles from "../style/GestionCorrespondencia.module.css";

type GestionCorrespondenciaProps<T extends AppTableRow = AppTableRow> = {
  table: GestionCorrespondenciaTableResult<T>;
};

const resolveStringField = (row: AppTableRow | null, keys: string[]): string | null => {
  if (!row) return null;

  for (const key of keys) {
    const value = row[key];
    if (typeof value === "string" && value.trim().length > 0) {
      return value.trim();
    }
    if (typeof value === "number") {
      return String(value);
    }
  }

  return null;
};
const EXPORT_FORMATS: AppTableExportFormat[] = ["csv", "xlsx", "pdf"];
const EXPORT_ENABLED_MODES: AppTableExportMode[] = ["currentPage", "selectedRows", "allMatching"];
const RESPONSIVE_PRESENTATION = {
  enabled: true,
  cardsBelow: 768,
} as const;
const STATUS_COLUMN_ID = "__gestion_estado_semaforo";
const STATUS_FIELD_CANDIDATES = [
  "ESTADO",
  "Estado",
  "estado",
  "ESTADO_TRAMITE",
  "EstadoTramite",
  "estadoTramite",
  "NOMBRE_ESTADO",
  "NombreEstado",
  "nombreEstado",
  "STATUS",
  "Status",
  "status",
] as const;

const resolveStatusValue = (row: AppTableRow | null | undefined): string => {
  if (!row) return "";

  for (const field of STATUS_FIELD_CANDIDATES) {
    const value = row[field];
    if (typeof value === "string" && value.trim()) return value.trim();
    if (typeof value === "number") return String(value);
  }

  return "";
};

const resolveStatusTone = (value: string): "success" | "warning" | "danger" | "neutral" => {
  const normalized = value.trim().toLocaleLowerCase();

  if (!normalized) return "neutral";
  if (
    normalized.includes("venc") ||
    normalized.includes("rechaz") ||
    normalized.includes("error") ||
    normalized.includes("bloque")
  ) {
    return "danger";
  }
  if (
    normalized.includes("pend") ||
    normalized.includes("curso") ||
    normalized.includes("proceso") ||
    normalized.includes("revision") ||
    normalized.includes("revisi")
  ) {
    return "warning";
  }
  if (
    normalized.includes("final") ||
    normalized.includes("cerr") ||
    normalized.includes("complet") ||
    normalized.includes("aprob") ||
    normalized.includes("resuelt")
  ) {
    return "success";
  }

  return "neutral";
};

function GestionEstadoSemaforoCell<T extends AppTableRow>({ data }: { data?: T }) {
  const statusValue = resolveStatusValue(data);
  const tone = resolveStatusTone(statusValue);
  const label = statusValue ? `Estado: ${statusValue}` : "Estado sin definir";

  return (
    <span className={styles.statusSemaphore} aria-label={label} title={label}>
      <span className={styles.statusSemaphoreDot} data-tone={tone} />
    </span>
  );
}

export default function GestionCorrespondencia<T extends AppTableRow = AppTableRow>({
  table,
}: GestionCorrespondenciaProps<T>) {
  const navigate = useNavigate();
  const [selectedRows, setSelectedRows] = useState<T[]>([]);
  const [isReasignarOpen, setIsReasignarOpen] = useState(false);
  const [reasignarContextRow, setReasignarContextRow] = useState<T | null>(null);
  const [reasignarUsers, setReasignarUsers] = useState<string[]>([]);
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
  const displayColumns = useMemo<ColDef<T>[]>(
    () => [
      {
        colId: STATUS_COLUMN_ID,
        headerName: "",
        width: 38,
        minWidth: 38,
        maxWidth: 44,
        sortable: false,
        filter: false,
        resizable: false,
        suppressMovable: true,
        lockPosition: "left",
        cellClass: styles.statusSemaphoreCell,
        headerClass: styles.statusSemaphoreHeader,
        cellRenderer: GestionEstadoSemaforoCell<T>,
      },
      ...(table.columns as ColDef<T>[]),
    ],
    [table.columns],
  );

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
    const normalizedActionId = actionId.trim().toLocaleLowerCase();

    if (normalizedActionId === "reasignar_tramite" || normalizedActionId === "reasignar_tramite_menu") {
      setReasignarContextRow(row);
      setReasignarUsers([]);
      setIsReasignarOpen(true);
      return;
    }

    if (normalizedActionId === "gestionar_tramite" || normalizedActionId === "gestionar_tramite_menu") {
      navigateToRowDetail(row);
    }
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

  const radicadoReasignar =
    resolveStringField(reasignarContextRow, [
      "RADICADO",
      "Radicado",
      "radicado",
      "NumeroRadicado",
      "numeroRadicado",
      "id",
    ]) ?? "-";

  const notaReasignar =
    resolveStringField(reasignarContextRow, [
      "ASUNTO",
      "Asunto",
      "asunto",
      "DESCRIPCION",
      "Descripcion",
      "descripcion",
      "TramiteDocumento",
      "tramiteDocumento",
    ]) ?? "Buen dia, Angelica. Se solicita reasignacion del tramite para continuidad operativa.";

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
              variant="primary"
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
                columns={displayColumns}
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

      <ReasignarRespuestaModal
        open={isReasignarOpen}
        onClose={() => setIsReasignarOpen(false)}
        radicado={radicadoReasignar}
        nota={notaReasignar}
        users={reasignarUsers}
        onAddUser={(value) =>
          setReasignarUsers((current) => (current.includes(value) ? current : [...current, value]))
        }
        onRemoveUser={(value) =>
          setReasignarUsers((current) => current.filter((item) => item !== value))
        }
        onRemoveAllUsers={() => setReasignarUsers([])}
        onSubmit={() => setIsReasignarOpen(false)}
      />
    </div>
  );
}
