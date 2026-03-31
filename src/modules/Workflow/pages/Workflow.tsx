import { Button, Card, Col, Row, Typography } from "antd";
import { useNavigate } from "react-router-dom";
import styles from "../style/Workflow.module.css";

const highlights = [
  {
    title: "Asignacion inteligente",
    description: "Zona preparada para reglas de priorizacion y responsables.",
  },
  {
    title: "Enlaces operativos",
    description: "Espacio para conexiones entre procesos y dependencias.",
  },
  {
    title: "Visibilidad total",
    description: "Panel futuro con estados, tiempos y cuellos de botella.",
  },
];

export default function Workflow() {
  const navigate = useNavigate();

  return (
    <div className={styles.page} data-testid="workflow-content">
      <div className={styles.hero}>
        <div>
          <Typography.Title level={3} className={styles.heroTitle}>
            Centro de Workflow
          </Typography.Title>
          <Typography.Paragraph className={styles.heroText}>
            Base visual para gestionar flujos, dependencias y acciones de soporte sin
            logica de negocio.
          </Typography.Paragraph>
        </div>
        <div className={styles.heroActions}>
          <Button type="primary" onClick={() => navigate("asignacion")}>
            Abrir asignacion
          </Button>
          <Button onClick={() => navigate("enlace")}>Abrir enlace</Button>
        </div>
      </div>

      <Row gutter={[16, 16]} className={styles.summaryRow}>
        {highlights.map((item) => (
          <Col key={item.title} xs={24} md={8}>
            <Card title={item.title}>
              <Typography.Paragraph style={{ marginBottom: 0 }}>
                {item.description}
              </Typography.Paragraph>
            </Card>
          </Col>
        ))}
      </Row>

      <Card title="Siguientes pasos sugeridos">
        <ul className={styles.featureList}>
          <li>
            <Typography.Text>
              Integrar reglas de asignacion y validaciones por perfil.
            </Typography.Text>
          </li>
          <li>
            <Typography.Text>
              Incorporar indicadores de desempeno y tiempos de ciclo.
            </Typography.Text>
          </li>
          <li>
            <Typography.Text>
              Conectar con servicios de correspondencia y alertas contextuales.
            </Typography.Text>
          </li>
        </ul>
      </Card>
    </div>
  );
}
