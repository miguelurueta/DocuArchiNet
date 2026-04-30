import { FileTextOutlined, InfoCircleOutlined } from "@ant-design/icons";
import { useParams } from "react-router-dom";
import type { AppTabItem } from "../../../app/Components/UI/AppTabs";
import { AppTabs } from "../../../app/Components/UI/AppTabs";
import { DocumentosWorkbench } from "../components/documentosWorkbench";
import { GestionRespuestaDocumentosProvider } from "../context/GestionRespuestaDocumentosContext";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";
import styles from "../style/GestionRespuesta.module.css";

type GestionRespuestaProps = {
  idTareaWf?: number;
  detailState?: "loading" | "ready" | "blocked-empty" | "blocked-error" | "blocked-invalid-id";
};

export default function GestionRespuesta({ idTareaWf: idTareaWfFromRoute }: GestionRespuestaProps = {}) {
  const params = useParams();
  const rawId = params.id;
  const fallbackId = typeof rawId === "string" ? Number.parseInt(rawId, 10) : Number.NaN;
  const idTareaWf =
    typeof idTareaWfFromRoute === "number" && Number.isFinite(idTareaWfFromRoute)
      ? idTareaWfFromRoute
      : fallbackId;

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
      children: <DocumentosWorkbench idTareaWf={Number.isFinite(idTareaWf) ? idTareaWf : undefined} />,
    },
  ];

  return (
    <div className={styles.tabsShell}>
      <GestionRespuestaDocumentosProvider>
        <AppTabs items={items} fullWidth className={styles.tabs} />
      </GestionRespuestaDocumentosProvider>
    </div>
  );
}
