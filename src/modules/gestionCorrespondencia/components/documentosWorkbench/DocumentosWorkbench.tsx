import { BookOutlined } from "@ant-design/icons";
import { useEffect, useId, useMemo, useRef, useState } from "react";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import { AppTreeTable } from "../../../../app/Components/UI/AppTreeTable";
import { AppVisorEmbedPdf } from "../../../../app/Components/UI/AppVisorEmbedPdf";
import styles from "./DocumentosWorkbench.module.css";

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

  return isTouchDevice && width > 768 && width <= 1366;
}

export function DocumentosWorkbench() {
  const panelId = useId();
  const rootRef = useRef<HTMLElement | null>(null);
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [isTablet, setIsTablet] = useState(resolveIsTablet);
  const [collapsed, setCollapsed] = useState(isTablet);

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

  useEffect(() => {
    if (variant !== "overlay") return;
    const root = rootRef.current;
    if (!root || typeof MutationObserver === "undefined") return;

    const tabPane = root.closest(".ant-tabs-tabpane") as HTMLElement | null;
    if (!tabPane) return;

    const isHidden = () => tabPane.classList.contains("ant-tabs-tabpane-hidden");

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
        <AppVisorEmbedPdf className={styles.viewer} />

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
                <span className={styles.previewMeta}>Vacío</span>
              </div>
              <div className={styles.previewSurface}>
                <div className={styles.listSurface}>
                  <AppTreeTable rows={[]} emptyMessage="Sin documentos adjuntos." />
                </div>
              </div>
            </section>
          </div>
        </AppCollapseRail>
      </div>
    </section>
  );
}
