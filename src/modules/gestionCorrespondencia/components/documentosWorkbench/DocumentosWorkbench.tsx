import { BookOutlined } from "@ant-design/icons";
import { useEffect, useId, useMemo, useRef, useState } from "react";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import styles from "./DocumentosWorkbench.module.css";
import { DocumentosToolbar } from "./DocumentosToolbar";

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
    if (!isMobile) return;
    const root = rootRef.current;
    if (!root || typeof MutationObserver === "undefined") return;

    const isHidden = () => {
      const hiddenAttr = root.hasAttribute("hidden");
      const ariaHidden = root.getAttribute("aria-hidden") === "true";
      return hiddenAttr || ariaHidden;
    };

    // If the tab system hides panels without unmounting, ensure overlay closes.
    const observer = new MutationObserver(() => {
      if (isHidden()) {
        setCollapsed(true);
      }
    });

    observer.observe(root, {
      attributes: true,
      attributeFilter: ["hidden", "aria-hidden", "style", "class"],
    });

    return () => observer.disconnect();
  }, [isMobile]);

  return (
    <section
      ref={(node) => {
        rootRef.current = node;
      }}
      className={styles.workbench}
      aria-label="Workbench de documentos"
    >
      <DocumentosToolbar className={styles.toolbar} />
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
            <p className={styles.mainHint} aria-label="Zona de documento">
              Sin visor ni acciones. Solo layout base.
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
                <span className={styles.previewMeta}>Demo</span>
              </div>
              <div className={styles.previewSurface}>
                <div className={styles.previewPlaceholder}>
                  <p className={styles.previewHint}>Documento 1</p>
                  <p className={styles.previewHint}>Documento 2</p>
                  <p className={styles.previewHint}>Documento 3</p>
                </div>
              </div>
            </section>
          </div>
        </AppCollapseRail>
      </div>
    </section>
  );
}
