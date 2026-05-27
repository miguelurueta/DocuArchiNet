import { BookOutlined, LeftOutlined, RightOutlined } from "@ant-design/icons";
import { useCallback, useEffect, useId, useMemo, useRef, useState } from "react";
import { toast } from "react-toastify";
import type { ToastId } from "react-toastify";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import { AppTreeTable } from "../../../../app/Components/UI/AppTreeTable";
import { AppVisorEmbedPdf } from "../../../../app/Components/UI/AppVisorEmbedPdf";
import type { AppVisorEmbedPdfRef } from "../../../../app/Components/UI/AppVisorEmbedPdf";
import { useDocumentViewerOrchestrator } from "../../../../app/Components/UI/AppDocumentViewerOrchestrator";
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
  const viewSeqRef = useRef(0);
  const lastNotifiedErrorRef = useRef<string | null>(null);
  const toastIdRef = useRef<ToastId | null>(null);
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [isTablet, setIsTablet] = useState(resolveIsTablet);
  const [collapsed, setCollapsed] = useState(isTablet);
  const documentosTable = useGestionRespuestaDocumentosTable(idTareaWf);
  const visorRef = useRef<AppVisorEmbedPdfRef | null>(null);
  const [activeFileUrl, setActiveFileUrl] = useState<string | undefined>(undefined);
  const [activeRowId, setActiveRowId] = useState<string | undefined>(undefined);
  const [viewerError, setViewerError] = useState<string | null>(null);
  const [documentContext, setDocumentContext] = useState<{
    documentId?: number;
    nombreGabinete?: string;
    isPdf?: boolean;
    viewerKind?: "pdf" | "image" | "unknown";
    isElectronicallySigned?: boolean | null;
    firmaCheckStatus?: string;
  } | null>(null);
  const documentViewer = useDocumentViewerOrchestrator();

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

  useEffect(() => {
    const fileUrl = documentViewer.documentoActivo?.fileUrl ?? null;
    if (!fileUrl) return;
    setActiveFileUrl(fileUrl);
  }, [documentViewer.documentoActivo?.fileUrl]);

  useEffect(() => {
    const doc = documentViewer.documentoActivo;
    if (!doc) return;
      setDocumentContext({
      documentId: doc.documentId,
      nombreGabinete: doc.nombreGabinete,
      isPdf: doc.isPdf,
      viewerKind: doc.viewerKind,
      isElectronicallySigned: doc.isElectronicallySigned,
      firmaCheckStatus: doc.firmaCheckStatus,
    });
  }, [documentViewer.documentoActivo]);

  useEffect(() => {
    // Modo managed del visor: cargar con contexto consolidado + permisos/policy.
    const doc = documentViewer.documentoActivo;
    if (!doc) return;
    if (!doc.isPdf) return;
    if (!activeFileUrl) return;
    const ctx = documentosTable.getWorkbenchContext?.();
    const radicado = ctx?.radicado ?? "";
    const idTareaWorkflow = typeof idTareaWf === "number" ? idTareaWf : 0;

    void visorRef.current?.load({
      url: activeFileUrl,
      isElectronicallySigned: Boolean(doc.isElectronicallySigned),
      idImagen: doc.documentId,
      nombreGabinete: doc.nombreGabinete,
      idTareaWorkflow,
      radicado,
      nombre_modulo: "gestioncorrespondencia",
      metadata: {
        activeRowId,
      },
    });
  }, [activeFileUrl, activeRowId, documentViewer.documentoActivo, documentosTable, idTareaWf]);

  useEffect(() => {
    const doc = documentViewer.documentoActivo;
    if (!doc) return;

    const message = doc.errors?.[0];
    if (!message) return;

    if (doc.resolveStatus === "failed") {
      setViewerError(message);
    } else if (doc.firmaCheckStatus === "failed") {
      setViewerError(message);
    }
  }, [documentViewer.documentoActivo]);

  useEffect(() => {
    if (!viewerError) return;
    if (viewerError === lastNotifiedErrorRef.current) return;

    lastNotifiedErrorRef.current = viewerError;
    toastIdRef.current = toast.error(viewerError, { autoClose: false, closeOnClick: false });
  }, [viewerError]);

  useEffect(() => {
    if (!toastIdRef.current) return;

    let cancelled = false;

    const dismiss = () => {
      if (cancelled) return;
      const toastId = toastIdRef.current;
      if (toastId) toast.dismiss(toastId);
      toastIdRef.current = null;
      setViewerError(null);
    };

    // Evita cerrar el toast inmediatamente por el mismo click que lo disparó.
    const timeoutId = window.setTimeout(() => {
      if (cancelled) return;
      window.addEventListener("pointerdown", dismiss, { capture: true });
    }, 0);

    return () => {
      cancelled = true;
      window.clearTimeout(timeoutId);
      window.removeEventListener("pointerdown", dismiss, { capture: true });
    };
  }, [viewerError]);

  const openViewerFromRow = useCallback(
    (rowId: string) => {
      viewSeqRef.current += 1;
      const seq = viewSeqRef.current;
      setViewerError(null);
      // Permite que el mismo mensaje vuelva a notificarse si el usuario reintenta
      // abrir el documento (click repetido en la misma fila).
      lastNotifiedErrorRef.current = null;
      void documentosTable.onSelectRow(rowId).then((result) => {
        if (seq !== viewSeqRef.current) return;
        if (!result?.documentResolveRequest) {
          setViewerError("No fue posible abrir el documento.");
          return;
        }
        setActiveRowId(rowId);
        void documentViewer.visualizarDocumento({
          documentId: result.documentResolveRequest.IdDocumento,
          nombreGabinete: result.documentResolveRequest.NombreGabinete,
          context: typeof idTareaWf === "number" ? { idTareaWorkflow: idTareaWf } : undefined,
        });
      });
    },
    [documentViewer, documentosTable, idTareaWf],
  );

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
      <div className={styles.viewer}>
        {documentViewer.documentoActivo?.viewerKind === "pdf" ? (
          <AppVisorEmbedPdf ref={visorRef} fileUrl={activeFileUrl} />
        ) : documentViewer.documentoActivo?.viewerKind === "image" && activeFileUrl ? (
          <img
            src={activeFileUrl}
            alt="Documento"
            style={{ width: "100%", height: "100%", objectFit: "contain", background: "#fff" }}
          />
        ) : (
          <AppVisorEmbedPdf ref={visorRef} fileUrl={activeFileUrl} />
        )}
      </div>

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
                if (params.actionId === "ver_documento") {
                  openViewerFromRow(params.rowId);
                  return;
                }

                void documentosTable.onActionTriggered({ actionId: params.actionId, rowId: params.rowId });
              }}
              emptyMessage="Sin documentos adjuntos."
            />
          </div>
        </div>
      </AppCollapseRail>
    </section>
  );
}
