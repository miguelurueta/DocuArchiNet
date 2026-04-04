import { DownloadOutlined, EyeFilled, FileExcelFilled, FilePdfFilled, UndoOutlined } from "@ant-design/icons";
import type { ColDef } from "ag-grid-community";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppDropdown } from "../../../app/Components/UI/AppDropdown";
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

  return (
    <div className={styles.shell}>
      <AppToolbar
        className={styles.toolbar}
        actionContent={
          <div className={styles.toolbarActionGroup}>
            <AppDropdown
              ariaLabel="Exportar"
              className={styles.toolbarControl}
              trigger={
                <AppButton variant="ghost" size="sm" fullWidth>
                  Exportar
                </AppButton>
              }
              items={[
                {
                  key: "export-excel",
                  label: "Exportar en Excel",
                  leftIcon: <FileExcelFilled />,
                  children: [
                    {
                      key: "export-excel-all",
                      label: "Exportar Todo",
                      leftIcon: <DownloadOutlined />,
                    },
                    {
                      key: "export-excel-selected",
                      label: "Exportar Seleccionados",
                      leftIcon: <DownloadOutlined />,
                    },
                  ],
                },
                {
                  key: "export-pdf",
                  label: "Exportar en Pdf",
                  leftIcon: <FilePdfFilled />,
                  children: [
                    {
                      key: "export-pdf-all",
                      label: "Exportar Todo",
                      leftIcon: <DownloadOutlined />,
                    },
                    {
                      key: "export-pdf-selected",
                      label: "Exportar Seleccionados",
                      leftIcon: <DownloadOutlined />,
                    },
                  ],
                },
              ]}
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
          >
            <div className={styles.tableWrapper}>
              <AppTable
                rows={table.rows}
                columns={table.columns as ColDef<T>[]}
                rowSelection="multiple"
                total={table.total}
                loading={table.loading && table.hasLoadedOnce}
                paginationMode="server"
                layoutMode="fill"
                responsivePresentation={{ enabled: true, cardsBelow: 768 }}
              />
            </div>
          </AppTableQueryWrapper>
        </div>
      </AppContent>
    </div>
  );
}
