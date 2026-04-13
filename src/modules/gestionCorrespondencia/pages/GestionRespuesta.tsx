import { FileTextOutlined, InfoCircleOutlined } from "@ant-design/icons";
import { Divider, Typography } from "antd";
import type { AppTabItem } from "../../../app/Components/UI/AppTabs";
import { AppTabs } from "../../../app/Components/UI/AppTabs";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";
import styles from "../style/GestionRespuesta.module.css";

export default function GestionRespuesta() {
  const items: AppTabItem[] = [
    {
      key: "gestion",
      label: "Gestion",
      icon: <InfoCircleOutlined />,
      children: <GestionRespuestaMainTabContent />,
    },
    {
      key: "documentos",
      label: "Documentos",
      icon: <FileTextOutlined />,
      children: (
        <section className={styles.tabSection}>
          <Typography.Title level={5} className={styles.sectionTitle}>
            Detalle operativo
          </Typography.Title>
          <Typography.Paragraph className={styles.sectionCopy}>
            Espacio reservado para datos del documento, acciones y trazabilidad.
          </Typography.Paragraph>
          <Divider className={styles.sectionDivider} />
          <Typography.Text className={styles.sectionHint}>
            La integracion funcional se activara en el siguiente refinement.
          </Typography.Text>
        </section>
      ),
    },
  ];

  return (
    <div className={styles.tabsShell}>
      <AppTabs items={items} fullWidth className={styles.tabs} />
    </div>
  );
}
