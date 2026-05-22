import { BookOutlined, LeftOutlined, RightOutlined } from "@ant-design/icons";
import { useCallback, useEffect, useId, useMemo, useRef, useState } from "react";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import { AppTreeTable } from "../../../../app/Components/UI/AppTreeTable";
import { AppVisorEmbedPdf } from "../../../../app/Components/UI/AppVisorEmbedPdf";
import { useGestionRespuestaDocumentosTable } from "../../hooks/useGestionRespuestaDocumentosTable";
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

type DocumentosWorkbenchProps = {
  idTareaWf?: number;
};

export function DocumentosWorkbench({ idTareaWf }: DocumentosWorkbenchProps) {
  const panelId = useId();
  const rootRef = useRef<HTMLElement | null>(null);
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [isTablet, setIsTablet] = useState(resolveIsTablet);
  const [collapsed, setCollapsed] = useState(isTablet);
  const documentosTable = useGestionRespuestaDocumentosTable(idTareaWf);
  const [activeFileUrl, setActiveFileUrl] = useState<string | undefined>(undefined);
  const [activeRowId, setActiveRowId] = useState<string | undefined>(undefined);

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
  const documentsCounter = useMemo(() => {
    const total = documentosTable.totalDocumentsCount ?? 0;
    const selected = documentosTable.selectedDocumentsCount ?? 0;
    return selected > 0
      ? `Documentos (${total}) · Seleccionados (${selected})`
      : `Documentos (${total})`;
  }, [documentosTable.selectedDocumentsCount, documentosTable.totalDocumentsCount]);

  const toggleIcon = layoutCollapsed ? <LeftOutlined /> : <RightOutlined />;

  const openViewerFromRow = useCallback((rowId: string) => {
    setActiveRowId(rowId);
    void documentosTable.onSelectRow(rowId).then((result) => {
      if (!result?.fileUrl) return;
      setActiveFileUrl(result.fileUrl);
    });
  }, [documentosTable]);

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
      className={styles.workbenchBody}
      aria-label="Workbench de documentos"
      data-collapsed={layoutCollapsed}
      data-variant={variant}
      data-testid="documentos-workbench"
    >
      <AppVisorEmbedPdf className={styles.viewer} fileUrl={activeFileUrl} />

      <AppCollapseRail
        title="Documentos"
        collapsed={collapsed}
        onToggle={() => setCollapsed((prev) => !prev)}
        placement="right"
        variant={variant}
        panelId={panelId}
        railLabel="Documentos"
        railIcon={<BookOutlined />}
        className={styles.collapseRail}
      >
        <div className={styles.listPanel}>
          <header className={styles.listHeader}>
            <h3 className={styles.listTitle}>{documentsCounter}</h3>
            <AppButton
              variant="ghost"
              size="sm"
              onClick={() => setCollapsed((prev) => !prev)}
              aria-label={layoutCollapsed ? "Mostrar documentos" : "Ocultar documentos"}
              icon={toggleIcon}
              className={styles.collapseButton}
            />
          </header>
          <div className={styles.listSurface} aria-label="Listado de documentos">
          <AppTreeTable
              load={documentosTable.load}
              loadChildren={documentosTable.loadChildren}
              tableColumns={documentosTable.getTableColumns()}
              columns={documentosTable.getColumns()}
              tableLayoutMode="fill"
              rowClickAffordance
              rowClickTooltip="Visualizar documento"
              rowSelection="multiple"
              rowSelectionCheckboxes
              rowSelectionHeaderCheckbox
              suppressRowClickSelection={false}
              onSelectionChanged={documentosTable.onSelectionChanged}
              activeRowId={activeRowId}
              onSelectRow={openViewerFromRow}
              onActionTriggered={(params) => {
                void documentosTable
                  .onActionTriggered({ actionId: params.actionId, rowId: params.rowId })
                  .then((result) => {
                    if (!result?.fileUrl) return;
                    setActiveFileUrl(result.fileUrl);
                    setActiveRowId(params.rowId);
                  });
              }}
              emptyMessage="Sin documentos adjuntos."
            />
          </div>
        </div>
      </AppCollapseRail>
    </section>
  );
}
