import { Layout } from "antd";
import { Outlet } from "react-router-dom";
import styles from "../style/GestionCorrespondenciaLayout.module.css";

const { Content } = Layout;

export default function GestionCorrespondenciaLayout() {
  return (
    <div className={styles.wrapper}>
      <Layout className={styles.body}>
        <Content>
          <Outlet />
        </Content>
      </Layout>
    </div>
  );
}
