import { DownloadOutlined, EyeFilled, FileExcelFilled, FilePdfFilled, UndoOutlined } from "@ant-design/icons";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppDropdown } from "../../../app/Components/UI/AppDropdown";
import AppTable from "../../../app/Components/UI/AppTable/AppTable";
import { AppInput } from "../../../app/Components/UI/AppInput";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";
import styles from "../style/GestionCorrespondencia.module.css";

export default function GestionCorrespondencia() {
  const navigate = useNavigate();
  const [search, setSearch] = useState("");
  const [category, setCategory] = useState<string | undefined>();
  const [pageSize, setPageSize] = useState(10);

  const rows = [
    { id: "1", asunto: "Documento A", categoria: "Entrada" },
    { id: "2", asunto: "Documento B", categoria: "Salida" },
  ];

  const columns = [
    { field: "asunto", headerName: "Asunto", flex: 1 },
    { field: "categoria", headerName: "Categoria", flex: 1 },
  ];

  const handleRefresh = () => {
    console.log("Actualizar datos");
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
              value={search}
              onChange={(event) => setSearch(event.target.value)}
            />

            <AppInput
              type="select"
              placeholder="Categoria"
              value={category}
              onChange={(value) => setCategory(String(value))}
              options={[
                { label: "Entrada", value: "entrada" },
                { label: "Salida", value: "salida" },
              ]}
            />
          </div>

          <div className={styles.paginationRow}>
            <span className={styles.total}>Cantidad de registros: {rows.length}</span>

            <div className={styles.paginationControls}>
              <span>Paginacion</span>

              <AppInput
                type="select"
                value={pageSize}
                onChange={(value) => setPageSize(Number(value))}
                options={[
                  { label: "10", value: 10 },
                  { label: "20", value: 20 },
                  { label: "30", value: 30 },
                ]}
              />
            </div>
          </div>

          <div className={styles.tableWrapper}>
            <AppTable
              rows={rows}
              columns={columns}
              rowSelection="multiple"
              loading={false}
            />
          </div>
        </div>
      </AppContent>
    </div>
  );
}
