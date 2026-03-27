import { Button, Card, Col, Flex, Row, Space, Tag, Typography } from "antd";
import { Link } from "react-router-dom";
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
  return (
    <div className={styles.page}>
      <Card variant="borderless">
        <Flex justify="space-between" align="start" gap={16} wrap>
          <Space orientation="vertical" size={6}>
            <Typography.Title level={3} style={{ margin: 0 }}>
              Centro operativo del modulo
            </Typography.Title>
            <Typography.Paragraph style={{ margin: 0, maxWidth: 720 }}>
              Esta vista deja lista la composicion base para incorporar bandejas,
              detalle de correspondencia y acciones de respuesta en iteraciones
              posteriores.
            </Typography.Paragraph>
            <Space wrap>
              <Tag color="blue">React Router anidado</Tag>
              <Tag color="cyan">Ant Design</Tag>
              <Tag color="geekblue">Sin logica de negocio</Tag>
            </Space>
          </Space>

          <div className={styles.ctaRow}>
            <Button type="primary">
              <Link to="respuesta">Abrir respuesta contextual</Link>
            </Button>
          </div>
        </Flex>
      </Card>

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
