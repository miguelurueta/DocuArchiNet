import { FileTextOutlined, InfoCircleOutlined } from "@ant-design/icons";
import { Switch } from "antd";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import type { AppTabItem } from "../../../app/Components/UI/AppTabs";
import { AppTabs } from "../../../app/Components/UI/AppTabs";
import { DocumentosWorkbench } from "../components/documentosWorkbench";
import { GestionWorkbenchParallelTabs } from "../components/workbenchParallelTabs";
import { GestionRespuestaDocumentosProvider } from "../context/GestionRespuestaDocumentosContext";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";
import styles from "../style/GestionRespuesta.module.css";

type GestionWorkbenchLayoutMode = "tabs" | "parallel";

const PARALLEL_LAYOUT_QUERY = "(min-width: 901px)";

function useCanUseParallelLayout() {
  const [canUseParallelLayout, setCanUseParallelLayout] = useState(() =>
    typeof window === "undefined" || typeof window.matchMedia !== "function"
      ? true
      : window.matchMedia(PARALLEL_LAYOUT_QUERY).matches,
  );

  useEffect(() => {
    if (typeof window === "undefined" || typeof window.matchMedia !== "function") return;
    const mediaQueryList = window.matchMedia(PARALLEL_LAYOUT_QUERY);
    const update = (event: MediaQueryListEvent) => setCanUseParallelLayout(event.matches);

    setCanUseParallelLayout(mediaQueryList.matches);
    mediaQueryList.addEventListener("change", update);
    return () => mediaQueryList.removeEventListener("change", update);
  }, []);

  return canUseParallelLayout;
}

type GestionRespuestaProps = {
  idTareaWf?: number;
  radicado?: string;
  idRespuestaRadicado?: string | number;
  detailState?: "loading" | "ready" | "blocked-empty" | "blocked-error" | "blocked-invalid-id";
};

export default function GestionRespuesta({
  idTareaWf: idTareaWfFromRoute,
  radicado,
  idRespuestaRadicado,
}: GestionRespuestaProps = {}) {
  const params = useParams();
  const canUseParallelLayout = useCanUseParallelLayout();
  const [layoutMode, setLayoutMode] = useState<GestionWorkbenchLayoutMode>("tabs");
  const rawId = params.id;
  const fallbackId = typeof rawId === "string" ? Number.parseInt(rawId, 10) : Number.NaN;
  const idTareaWf =
    typeof idTareaWfFromRoute === "number" && Number.isFinite(idTareaWfFromRoute)
      ? idTareaWfFromRoute
      : fallbackId;
  const resolvedIdTareaWf = Number.isFinite(idTareaWf) ? idTareaWf : undefined;
  const isParallel = layoutMode === "parallel" && canUseParallelLayout;

  useEffect(() => {
    if (!canUseParallelLayout && layoutMode === "parallel") {
      setLayoutMode("tabs");
    }
  }, [canUseParallelLayout, layoutMode]);

  const gestionContent = <GestionRespuestaMainTabContent idTareaWf={resolvedIdTareaWf} />;
  const documentosContent = <DocumentosWorkbench idTareaWf={resolvedIdTareaWf} />;
  const layoutToggleButton = (
    <label
      className={styles.layoutSwitchControl}
      data-layout-state={isParallel ? "active" : "inactive"}
      title={
        canUseParallelLayout
          ? undefined
          : "La vista paralela esta disponible en pantallas mas anchas."
      }
    >
      <span className={styles.layoutSwitchText}>Vista paralela</span>
      <Switch
        checked={isParallel}
        disabled={!canUseParallelLayout}
        aria-label="Vista paralela"
        aria-pressed={isParallel}
        onChange={(checked) => setLayoutMode(checked ? "parallel" : "tabs")}
      />
    </label>
  );

  const items: AppTabItem[] = [
    {
      key: "gestion",
      label: "Gestion",
      icon: <InfoCircleOutlined />,
      children: gestionContent,
    },
    {
      key: "documentos",
      label: "Documentos",
      icon: <FileTextOutlined />,
      children: documentosContent,
    },
  ];

  return (
    <div className={styles.tabsShell}>
      <GestionRespuestaDocumentosProvider
        idTareaWf={resolvedIdTareaWf}
        radicado={radicado}
        idRespuestaRadicado={idRespuestaRadicado}
      >
        <div className={styles.layoutBody}>
          {isParallel ? (
            <>
              <div className={styles.parallelTabsNav} role="tablist" aria-label="Vista paralela">
                <div className={styles.parallelTabsList}>
                  <span className={styles.parallelTabItem}>
                    <InfoCircleOutlined />
                    <span>Gestion</span>
                  </span>
                  <span className={styles.parallelTabItem}>
                    <FileTextOutlined />
                    <span>Documentos</span>
                  </span>
                </div>
                <div className={styles.parallelTabsExtra}>{layoutToggleButton}</div>
              </div>
              <GestionWorkbenchParallelTabs
                gestion={gestionContent}
                documentos={documentosContent}
              />
            </>
          ) : (
            <AppTabs
              items={items}
              fullWidth
              className={styles.tabs}
              tabBarExtraContent={{ right: layoutToggleButton }}
            />
          )}
        </div>
      </GestionRespuestaDocumentosProvider>
    </div>
  );
}
