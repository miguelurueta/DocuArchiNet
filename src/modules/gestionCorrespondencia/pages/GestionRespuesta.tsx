import { FileTextOutlined, InfoCircleOutlined, PaperClipOutlined } from "@ant-design/icons";
import { Divider, Typography } from "antd";
import type { AppTabItem } from "../../../app/Components/UI/AppTabs";
import { AppTabs } from "../../../app/Components/UI/AppTabs";
import styles from "../style/GestionRespuesta.module.css";

export default function GestionRespuesta() {
  const items: AppTabItem[] = [
    {
      key: "contexto",
      label: "Contexto",
      icon: <InfoCircleOutlined />,
      children: (
        <section className={styles.tabSection}>
          <Typography.Title level={5} className={styles.sectionTitle}>
            Resumen de respuesta
          </Typography.Title>
          <Typography.Paragraph className={styles.sectionCopy}>
            Vista secundaria con el contexto minimo para revisar la respuesta
            sin salir de la bandeja principal.
          </Typography.Paragraph>
          <div className={styles.sectionMeta}>
            <span>Origen: Bandeja de correspondencia</span>
            <span>Estado: Pendiente de validacion</span>
          </div>
        </section>
      ),
    },
    {
      key: "detalle",
      label: "Detalle",
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
    {
      key: "adjuntos",
      label: "Adjuntos",
      icon: <PaperClipOutlined />,
      disabled: true,
      children: null,
    },
  ];

  return (
    <div className={styles.tabsShell}>
      <AppTabs items={items} fullWidth className={styles.tabs} />
    </div>
  );
}
