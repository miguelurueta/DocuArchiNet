import { BookOutlined } from "@ant-design/icons";
import { useEffect, useId, useMemo, useRef, useState } from "react";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import { AppVisorPdf } from "../../../../app/Components/UI/AppVisorPdf";
import styles from "./DocumentosWorkbench.module.css";
import { DocumentosList } from "./DocumentosList";
import { DocumentosToolbar } from "./DocumentosToolbar";
import { useGestionRespuestaDocumentos } from "../../hooks/useGestionRespuestaDocumentos";

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

type DocumentosWorkbenchProps = {
  idTareaWf?: number;
};

const formatBytes = (size: number) => {
  if (!Number.isFinite(size) || size <= 0) return "0 B";
  const units = ["B", "KB", "MB", "GB"] as const;
  const idx = Math.min(Math.floor(Math.log(size) / Math.log(1024)), units.length - 1);
  const value = size / 1024 ** idx;
  const display = value >= 10 || idx === 0 ? Math.round(value) : Math.round(value * 10) / 10;
  return `${display} ${units[idx]}`;
};

const isPdfFile = (fileName: string, mime?: string) =>
  (mime?.toLowerCase() === "application/pdf" || fileName.toLowerCase().endsWith(".pdf")) ?? false;

export function DocumentosWorkbench({ idTareaWf }: DocumentosWorkbenchProps) {
  const panelId = useId();
  const rootRef = useRef<HTMLElement | null>(null);
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [isTablet, setIsTablet] = useState(resolveIsTablet);
  const [collapsed, setCollapsed] = useState(isTablet);
  const { files } = useGestionRespuestaDocumentos();
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [selectedUrl, setSelectedUrl] = useState<string | null>(null);
  const [selectedUnsupported, setSelectedUnsupported] = useState<string | null>(null);
  const [selectedFileName, setSelectedFileName] = useState<string | null>(null);

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

  const listItems = useMemo(() => {
    return files.map((file) => ({
      id: file.uid,
      title: file.name,
      meta: `${isPdfFile(file.name, file.type) ? "PDF" : "Documento"} · ${formatBytes(file.size)}`,
      kind: isPdfFile(file.name, file.type) ? ("pdf" as const) : ("doc" as const),
    }));
  }, [files]);

  const selectedFile = useMemo(
    () => files.find((f) => f.uid === selectedId) ?? null,
    [files, selectedId],
  );

  useEffect(() => {
    if (!selectedId) return;
    if (files.some((file) => file.uid === selectedId)) return;
    setSelectedId(null);
  }, [files, selectedId]);

  useEffect(() => {
    setSelectedUnsupported(null);
    setSelectedFileName(selectedFile?.name ?? null);

    if (!selectedFile) {
      setSelectedUrl(null);
      return;
    }

    if (!isPdfFile(selectedFile.name, selectedFile.type)) {
      setSelectedUrl(null);
      setSelectedUnsupported(selectedFile.name);
      return;
    }

    if (selectedFile.url) {
      setSelectedUrl(selectedFile.url);
      return;
    }

    if (!selectedFile.originFile) {
      setSelectedUrl(null);
      setSelectedUnsupported(selectedFile.name);
      return;
    }

    const url = URL.createObjectURL(selectedFile.originFile);
    setSelectedUrl(url);
    return () => {
      URL.revokeObjectURL(url);
    };
  }, [selectedFile]);

  const canOpenSelected = Boolean(selectedUrl);

  const openSelectedInNewTab = () => {
    if (!canOpenSelected || !selectedUrl) return;
    if (typeof window === "undefined") return;
    window.open(selectedUrl, "_blank", "noopener,noreferrer");
  };

  useEffect(() => {
    if (variant !== "overlay") return;
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
  }, [variant]);

  return (
    <section
      ref={(node) => {
        rootRef.current = node;
      }}
      className={styles.workbench}
      aria-label="Workbench de documentos"
    >
      <DocumentosToolbar
        className={styles.toolbar}
        hasDocuments={listItems.length > 0}
        canOpenSelected={canOpenSelected}
        onOpenDocuments={() => {
          if (canOpenSelected) {
            openSelectedInNewTab();
            return;
          }
          setCollapsed(false);
        }}
        onSearchDocuments={() => setCollapsed(false)}
      />
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
            {selectedUnsupported ? (
              <p className={styles.mainHint} role="status" aria-label="Zona de documento">
                El archivo &quot;{selectedUnsupported}&quot; no es compatible con el visor PDF.
              </p>
            ) : selectedUrl ? (
              <AppVisorPdf
                input={{ kind: "url", url: selectedUrl }}
                documentId={selectedId ?? undefined}
                aria-label="Visor de documentos PDF"
                className={styles.viewer}
              />
            ) : listItems.length === 0 ? (
              <p className={styles.mainHint} role="status" aria-label="Zona de documento">
                No hay documentos adjuntos para visualizar.
              </p>
            ) : (
              <p className={styles.mainHint} role="status" aria-label="Zona de documento">
                Selecciona un documento PDF para visualizarlo.
              </p>
            )}
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
                <span className={styles.previewMeta}>
                  {typeof idTareaWf === "number" && Number.isFinite(idTareaWf) && idTareaWf > 0
                    ? `IdTareaWf ${idTareaWf}`
                    : "Sin contexto"}
                </span>
              </div>
              <div className={styles.previewSurface}>
                {listItems.length === 0 ? (
                  <div className={styles.previewPlaceholder}>
                    <p className={styles.previewHint}>Sin documentos adjuntos.</p>
                  </div>
                ) : (
                  <DocumentosList
                    items={listItems}
                    selectedId={selectedId}
                    onSelect={(doc) => {
                      setSelectedId(doc.id);
                      if (variant === "overlay") {
                        setCollapsed(true);
                      }
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
