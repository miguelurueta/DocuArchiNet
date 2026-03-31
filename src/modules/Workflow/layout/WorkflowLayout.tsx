import { Layout } from "antd";
import { Outlet } from "react-router-dom";
import styles from "../style/WorkflowLayout.module.css";

const { Content } = Layout;

export default function WorkflowLayout() {
  return (
    <div className={styles.wrapper}>
      <Layout className={styles.body}>
        <Content className={styles.content}>
          <Outlet />
        </Content>
      </Layout>
    </div>
  );
}
