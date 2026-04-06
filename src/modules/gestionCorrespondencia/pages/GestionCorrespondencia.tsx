import { EyeFilled, UndoOutlined } from "@ant-design/icons";
import type { ColDef } from "ag-grid-community";
import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppTableExport } from "../../../app/Components/UI/AppTable/AppTableExport";
import { AppTableQueryWrapper } from "../../../app/Components/UI/AppTable/AppTableQueryWrapper";
import AppTable from "../../../app/Components/UI/AppTable/AppTable";
import type { AppTableRow } from "../../../app/Components/UI/AppTable/AppTable.types";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";
import type { GestionCorrespondenciaTableResult } from "../hooks/useGestionCorrespondenciaTable";
import styles from "../style/GestionCorrespondencia.module.css";

type GestionCorrespondenciaProps<T extends AppTableRow = AppTableRow> = {
  table: GestionCorrespondenciaTableResult<T>;
};

export default function GestionCorrespondencia<T extends AppTableRow = AppTableRow>({
  table,
}: GestionCorrespondenciaProps<T>) {
  const navigate = useNavigate();
  const [selectedRows, setSelectedRows] = useState<T[]>([]);
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

  return (
    <div className={styles.shell}>
      <AppToolbar
        className={styles.toolbar}
        actionContent={
          <div className={styles.toolbarActionGroup}>
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

            <AppButton
              className={styles.toolbarControl}
              variant="ghost"
              size="sm"
              leftIcon={<EyeFilled />}
              fullWidth
              onClick={() => navigate("respuesta")}
            >
              Abrir respuesta contextual
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
            paginationActions={
              <AppTableExport
                columns={table.columns as ColDef<T>[]}
                dataSource={{
                  getCurrentPageRows: () => table.rows,
                  getSelectedRows: () => selectedRows,
                  getAllMatchingRows: table.getAllMatchingRows,
                  getBackendExportFile: table.getBackendExportFile,
                }}
                formats={["csv", "xlsx", "pdf"]}
                reportMeta={exportReportMeta}
                enabledModes={["currentPage", "selectedRows", "allMatching"]}
              />
            }
          >
            <div className={styles.tableWrapper}>
              <AppTable
                rows={table.rows}
                columns={table.columns as ColDef<T>[]}
                rowSelection="single"
                total={table.total}
                loading={table.loading && table.hasLoadedOnce}
                paginationMode="server"
                layoutMode="fill"
                responsivePresentation={{ enabled: true, cardsBelow: 768 }}
                onSelectionChanged={setSelectedRows}
              />
            </div>
          </AppTableQueryWrapper>
        </div>
      </AppContent>
    </div>
  );
}
