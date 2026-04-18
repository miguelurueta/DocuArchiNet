import { FileTextOutlined, InfoCircleOutlined } from "@ant-design/icons";
import { useParams } from "react-router-dom";
import { DocumentosWorkbench } from "../components/documentosWorkbench";
import type { AppTabItem } from "../../../app/Components/UI/AppTabs";
import { AppTabs } from "../../../app/Components/UI/AppTabs";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";
import styles from "../style/GestionRespuesta.module.css";

export default function GestionRespuesta() {
  const params = useParams();
  const rawId = params.id;
  // `useParams()` siempre retorna string | undefined. `Number("924-foo")` da NaN, pero
  // el backend espera un número; `parseInt` nos permite tolerar sufijos accidentales.
  const idTareaWf = typeof rawId === "string" ? Number.parseInt(rawId, 10) : Number.NaN;

  const items: AppTabItem[] = [
    {
      key: "gestion",
      label: "Gestion",
      icon: <InfoCircleOutlined />,
      children: <GestionRespuestaMainTabContent idTareaWf={Number.isFinite(idTareaWf) ? idTareaWf : undefined} />,
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
