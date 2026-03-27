import { Card, Col, Row, Space, Tag, Typography } from "antd";
import { useNavigate } from "react-router-dom";
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
        title="Centro operativo del modulo"
        subtitle="AppToolbar enterprise"
        description="Esta vista deja lista la composicion base para incorporar bandejas, detalle de correspondencia y acciones de respuesta en iteraciones posteriores."
        breadcrumbs={[
          { key: "dashboard", label: "Dashboard", to: "/dashboard" },
          { key: "gestion-correspondencia", label: "Gestion de correspondencia", current: true },
        ]}
        extra={
          <Space wrap>
            <Tag color="blue">React Router anidado</Tag>
            <Tag color="cyan">Ant Design</Tag>
            <Tag color="geekblue">Sin logica de negocio</Tag>
          </Space>
        }
        actions={[{ key: "refresh", label: "Actualizar resumen", variant: "secondary" }]}
        secondaryActions={[
          { key: "share", label: "Compartir contexto", variant: "ghost" },
          { key: "export", label: "Exportar vista", variant: "ghost" },
        ]}
        primaryAction={{
          key: "open-response",
          label: "Abrir respuesta contextual",
          variant: "primary",
          onClick: () => navigate("respuesta"),
        }}
      />

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
    </div>
  );
}
