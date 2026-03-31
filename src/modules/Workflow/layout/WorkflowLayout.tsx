import { Layout, Typography } from "antd";
import { Outlet } from "react-router-dom";
import styles from "../style/WorkflowLayout.module.css";

const { Content, Header } = Layout;

export default function WorkflowLayout() {
  return (
    <div className={styles.wrapper}>
      <Layout className={styles.body}>
        <Header className={styles.header}>
          <Typography.Title level={4} className={styles.title}>
            Workflow
          </Typography.Title>
          <Typography.Paragraph className={styles.subtitle}>
            Esqueleto inicial del modulo para organizar flujos y acciones contextuales.
          </Typography.Paragraph>
        </Header>
        <Content className={styles.content}>
          <Outlet />
        </Content>
      </Layout>
    </div>
  );
}
