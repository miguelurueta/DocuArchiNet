import { BookOutlined } from "@ant-design/icons";
import { useEffect, useId, useMemo, useRef, useState } from "react";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import styles from "./DocumentosWorkbench.module.css";
import { DocumentosList } from "./DocumentosList";

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

function resolveIsTablet() {
  if (typeof window === "undefined") return false;

  const width = window.innerWidth;
  const isTouchDevice =
    typeof navigator !== "undefined" && (navigator.maxTouchPoints ?? 0) > 0;

  // iPad Pro/Air commonly report widths up to 1366; treat touch devices in this range as tablet.
  return isTouchDevice && width > 768 && width <= 1366;
}

export function DocumentosWorkbench() {
  const panelId = useId();
  const rootRef = useRef<HTMLElement | null>(null);
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [isTablet, setIsTablet] = useState(resolveIsTablet);
  const [collapsed, setCollapsed] = useState(isTablet);
  const [documents, setDocuments] = useState(() => []);
  const [selectedId, setSelectedId] = useState<string | null>(null);

  useEffect(() => {
    const handler = () => setIsTablet(resolveIsTablet());
    window.addEventListener("resize", handler);
    return () => window.removeEventListener("resize", handler);
  }, []);

  useEffect(() => {
    setCollapsed(isTablet);
  }, [isTablet]);

  const variant = useMemo(
    () => (isMobile || isTablet ? "overlay" : "inline"),
    [isMobile, isTablet],
  );
  const layoutCollapsed = variant === "overlay" ? true : collapsed;

  const selectedDoc = useMemo(
    () => documents.find((doc) => doc.id === selectedId) ?? null,
    [documents, selectedId],
  );

  useEffect(() => {
    if (variant !== "overlay") return;
    const root = rootRef.current;
    if (!root || typeof MutationObserver === "undefined") return;

    const tabPane = root.closest(".ant-tabs-tabpane") as HTMLElement | null;
    if (!tabPane) return;

    const isHidden = () => {
      // Antd Tabs hides inactive panels by toggling tabpane classes (display: none).
      return tabPane.classList.contains("ant-tabs-tabpane-hidden");
    };

    // If the tab system hides panels without unmounting, ensure overlay closes.
    const observer = new MutationObserver(() => {
      if (isHidden()) {
        setCollapsed(true);
      }
    });

    observer.observe(tabPane, {
      attributes: true,
      attributeFilter: ["class", "style"],
    });

    return () => observer.disconnect();
  }, [variant]);

  return (
    <section
      ref={(node) => {
        rootRef.current = node;
      }}
      className={styles.workbench}
      aria-label="Workbench de documentos"
    >
      <div
        className={styles.workbenchBody}
        data-collapsed={layoutCollapsed}
        data-variant={variant}
        data-testid="documentos-workbench"
      >
        <main className={styles.mainArea}>
          <header className={styles.mainHeader}>
            <h3 className={styles.mainTitle} aria-hidden="true" />
          </header>
          <div className={styles.mainSurface}>
            <p className={styles.mainHint} role="status" aria-label="Zona de documento">
              {documents.length === 0
                ? "Espacio reservado para el visor de documentos (pendiente de implementación)."
                : selectedDoc
                  ? `Documento seleccionado: ${selectedDoc.title}`
                  : "Selecciona un documento para visualizarlo (pendiente de implementación)."}
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
          railLabel="Ver documentos"
          railIcon={<BookOutlined />}
          className={styles.collapseRail}
        >
          <div className={styles.panelContent}>
            <section className={styles.preview} aria-label="Panel de documentos">
              <div className={styles.previewHeader}>
                <h4 className={styles.previewTitle}>Listado</h4>
                <span className={styles.previewMeta}>Mockup</span>
              </div>
              <div className={styles.previewSurface}>
                {documents.length === 0 ? (
                  <div className={styles.previewPlaceholder}>
                    <p className={styles.previewHint}>Sin documentos adjuntos.</p>
                  </div>
                ) : (
                  <DocumentosList
                    items={documents}
                    selectedId={selectedId}
                    onSelect={(doc) => {
                      setSelectedId(doc.id);
                      if (variant === "overlay") {
                        setCollapsed(true);
                      }
                    }}
                    onDelete={(doc) => {
                      setDocuments((prev) => {
                        const remaining = prev.filter((item) => item.id !== doc.id);
                        setSelectedId((prevSelected) => {
                          if (prevSelected !== doc.id) return prevSelected;
                          return remaining[0]?.id ?? null;
                        });
                        return remaining;
                      });
                    }}
                  />
                )}
              </div>
            </section>
          </div>
        </AppCollapseRail>
      </div>
    </section>
  );
}
