import { Layout, Space, Typography } from "antd";
import { Outlet } from "react-router-dom";
import styles from "../style/GestionCorrespondenciaLayout.module.css";

const { Content } = Layout;
const { Paragraph, Title } = Typography;

export default function GestionCorrespondenciaLayout() {
  return (
    <div className={styles.wrapper}>
      <section className={styles.hero}>
        <Space orientation="vertical" size={8}>
          <Title level={2} style={{ margin: 0 }}>
            Gestion de Correspondencia
          </Title>
          <Paragraph style={{ margin: 0, maxWidth: 760 }}>
            Estructura inicial del modulo para gestionar bandejas, respuestas y
            futuras acciones de correspondencia sin perder el contexto del
            dashboard.
          </Paragraph>
        </Space>
      </section>

      <Layout className={styles.body}>
        <Content>
          <Outlet />
        </Content>
      </Layout>
    </div>
  );
}
