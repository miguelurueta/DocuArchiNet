import { FileTextOutlined, InfoCircleOutlined } from "@ant-design/icons";
import { DocumentosWorkbench } from "../components/documentosWorkbench";
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
      children: <DocumentosWorkbench />,
    },
  ];

  return (
    <div className={styles.tabsShell}>
      <AppTabs items={items} fullWidth className={styles.tabs} />
    </div>
  );
}
