import { Card, Col, Row, Typography } from "antd";
import {
  DownloadOutlined,
  EyeFilled,
  FileExcelFilled,
  FilePdfFilled,
} from "@ant-design/icons";
import { useNavigate } from "react-router-dom";
import { AppButton } from "../../../app/Components/UI/AppButton";
import { AppContent } from "../../../app/Components/UI/AppContent";
import { AppDropdown } from "../../../app/Components/UI/AppDropdown";
import { AppToolbar } from "../../../app/Components/UI/AppToolbar";
import styles from "../style/GestionCorrespondencia.module.css";

const placeholderColumns = [
  {
    title: "Bandeja prioritaria",
    description: "Resumen para documentos pendientes de clasificacion y respuesta.",
  },
  {
    title: "Trazabilidad",
    description: "Zona preparada para estados, tiempos de atencion y responsables.",
  },
  {
    title: "Acciones guiadas",
    description: "Espacio para iniciar respuestas, revisiones y flujos derivados.",
  },
];

export default function GestionCorrespondencia() {
  const navigate = useNavigate();

  return (
    <div className={styles.page}>
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
        <Row gutter={[16, 16]} className={styles.summaryRow}>
          {placeholderColumns.map((item) => (
            <Col key={item.title} xs={24} md={8}>
              <Card title={item.title}>
                <Typography.Paragraph style={{ marginBottom: 0 }}>
                  {item.description}
                </Typography.Paragraph>
              </Card>
            </Col>
          ))}
        </Row>

        <Card title="Proxima evolucion del modulo">
          <ul className={styles.featureList}>
            <li>
              <Typography.Text>
                Integracion con bandejas reales y filtros operativos.
              </Typography.Text>
            </li>
            <li>
              <Typography.Text>
                Detalle de correspondencia con historial y anexos.
              </Typography.Text>
            </li>
            <li>
              <Typography.Text>
                Flujos de respuesta con permisos y trazabilidad.
              </Typography.Text>
            </li>
          </ul>
        </Card>
      </AppContent>
    </div>
  );
}
