import { BookOutlined } from "@ant-design/icons";
import { useEffect, useId, useMemo, useState } from "react";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import styles from "./DocumentosWorkbench.module.css";
import { DocumentosList } from "./DocumentosList";
import { DocumentosPreview } from "./DocumentosPreview";
import { DocumentosToolbar } from "./DocumentosToolbar";

const TABLET_QUERY = "(max-width: 1024px)";
const MOBILE_QUERY = "(max-width: 768px)";

const useMediaQuery = (query: string) => {
  const getMatches = () =>
    typeof window !== "undefined" ? window.matchMedia(query).matches : false;
  const [matches, setMatches] = useState(getMatches);

  useEffect(() => {
    const mediaQueryList = window.matchMedia(query);
    const update = (event: MediaQueryListEvent) => {
      setMatches(event.matches);
    };

    setMatches(mediaQueryList.matches);
    mediaQueryList.addEventListener("change", update);
    return () => {
      mediaQueryList.removeEventListener("change", update);
    };
  }, [query]);

  return matches;
};

export function DocumentosWorkbench() {
  const panelId = useId();
  const isTablet = useMediaQuery(TABLET_QUERY);
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [collapsed, setCollapsed] = useState(isTablet);

  useEffect(() => {
    setCollapsed(isTablet);
  }, [isTablet]);

  const variant = useMemo(() => (isMobile ? "overlay" : "inline"), [isMobile]);

  return (
    <section className={styles.workbench} aria-label="Workbench de documentos">
      <DocumentosToolbar className={styles.toolbar} />

      <div
        className={styles.workbenchBody}
        data-collapsed={collapsed}
        data-variant={variant}
        data-testid="documentos-workbench"
      >
        <main className={styles.mainArea}>
          <header className={styles.mainHeader}>
            <h3 className={styles.mainTitle}>Contenido principal</h3>
            <p className={styles.mainCopy}>
              Zona de edicion y lectura del documento seleccionado.
            </p>
          </header>
          <div className={styles.mainSurface}>
            <p className={styles.mainHint}>
              Arrastra aqui los componentes principales del editor.
            </p>
          </div>
        </main>

        <AppCollapseRail
          title="Visualizar documentos"
          collapsed={collapsed}
          onToggle={() => setCollapsed((prev) => !prev)}
          placement="right"
          variant={variant}
          panelId={panelId}
          railLabel="Documentos"
          railIcon={<BookOutlined />}
          className={styles.collapseRail}
        >
          <div className={styles.panelContent}>
            <DocumentosList />
            <DocumentosPreview />
          </div>
        </AppCollapseRail>
      </div>
    </section>
  );
}
