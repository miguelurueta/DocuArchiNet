import { DownloadOutlined, EyeFilled, FileExcelFilled, FilePdfFilled, UndoOutlined } from "@ant-design/icons";
import type { ColDef } from "ag-grid-community";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppDropdown } from "../../../app/Components/UI/AppDropdown";
import AppTable from "../../../app/Components/UI/AppTable/AppTable";
import type { AppTableRow } from "../../../app/Components/UI/AppTable/AppTable.types";
import { AppInput } from "../../../app/Components/UI/AppInput";
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

  const handleRefresh = () => {
    table.refetch();
  };

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
              onClick={handleRefresh}
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
        data-testid="gestion-correspondencia-content"
        width="full"
        density="compact"
      >
        <div className={styles.page}>
          <div className={styles.filtersRow}>
            <AppInput
              type="search"
              placeholder="Buscar..."
              value={table.search}
              onChange={(event) => table.setSearch(event.target.value)}
            />

            <AppInput
              type="select"
              placeholder="Categoria"
              value={table.category}
              onChange={(value) => table.setCategory(value ? String(value) : undefined)}
              options={[
                { label: "Entrada", value: "entrada" },
                { label: "Salida", value: "salida" },
              ]}
            />
          </div>

          <div className={styles.paginationRow}>
            <span className={styles.total}>Cantidad de registros: {table.total}</span>

            <div className={styles.paginationControls}>
              <span>Paginacion</span>

              <AppInput
                type="select"
                value={table.pageSize}
                onChange={(value) => table.setPageSize(Number(value))}
                options={[
                  { label: "10", value: 10 },
                  { label: "20", value: 20 },
                  { label: "25", value: 25 },
                  { label: "30", value: 30 },
                ]}
              />
            </div>
          </div>

          <div className={styles.tableWrapper}>
            <AppTable
              rows={table.rows}
              columns={table.columns as ColDef<T>[]}
              rowSelection="multiple"
              total={table.total}
              loading={table.loading && table.hasLoadedOnce}
            />
          </div>
        </div>
      </AppContent>
    </div>
  );
}
