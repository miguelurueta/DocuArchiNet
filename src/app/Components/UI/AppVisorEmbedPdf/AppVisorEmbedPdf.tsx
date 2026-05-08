import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { useZoom } from "@embedpdf/plugin-zoom/react";
import { ThumbnailsPane, ThumbImg } from "@embedpdf/plugin-thumbnail/react";
import { useScroll } from "@embedpdf/plugin-scroll/react";
import { Rotate, useRotate } from "@embedpdf/plugin-rotate/react";
import { useViewportCapability } from "@embedpdf/plugin-viewport/react";
import { UpOutlined } from "@ant-design/icons";

import {
  DocumentContent,
  EmbedPDF,
  RenderLayer,
  Scroller,
  useActiveDocument,
  useDocumentManagerCapability,
  Viewport,
} from "./engine/embedPdfAdapter";
import { useEmbedPdfEngine } from "./engine/useEmbedPdfEngine";
import { useDemoPdfUrl } from "./hooks/useDemoPdfUrl";
import { createBasicPluginRegistration } from "./plugins/pluginRegistration";
import {
  DocumentLoadingState,
  EmptyState,
  EngineLoadingState,
  ErrorState,
} from "./presentation/States";
import { AppPdfToolbar } from "./presentation/AppPdfToolbar";
import styles from "./styles/AppVisorEmbedPdf.module.css";
import type { AppVisorEmbedPdfProps } from "./types/AppVisorEmbedPdfProps";

function cx(...parts: Array<string | undefined>) {
  return parts.filter(Boolean).join(" ");
}

/**
 * AppVisorEmbedPdf (01-FE)
 *
 * NOTA: Este componente encapsula EmbedPDF/Pdfium y expone una API mínima.
 * Se prohíbe filtrar detalles del engine hacia módulos consumidores.
 */
export function AppVisorEmbedPdf({ fileUrl, className, style }: AppVisorEmbedPdfProps) {
  const demoUrl = useDemoPdfUrl();
  const effectiveFileUrl = fileUrl?.trim() ? fileUrl.trim() : demoUrl;

  const engineState = useEmbedPdfEngine();
  const pluginRegistration = useMemo(() => createBasicPluginRegistration(), []);

  if (engineState.status === "loading") {
    return (
      <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
        <EngineLoadingState />
      </div>
    );
  }

  if (engineState.status === "error") {
    return (
      <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
        <ErrorState />
      </div>
    );
  }

  if (!effectiveFileUrl) {
    return (
      <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
        <EmptyState />
      </div>
    );
  }

  return (
    <div className={cx(styles.root, className)} style={style} role="status" aria-label="Zona de documento">
      <EmbedPDF engine={engineState.engine} plugins={pluginRegistration}>
        <EmbedPdfDocumentHost fileUrl={effectiveFileUrl} />
      </EmbedPDF>
    </div>
  );
}

function EmbedPdfDocumentHost({ fileUrl }: { fileUrl: string }) {
  const { provides } = useDocumentManagerCapability();
  const { activeDocumentId } = useActiveDocument();

  const lastOpenedUrlRef = useRef<string | null>(null);
  useEffect(() => {
    if (!provides) return;
    if (!fileUrl) return;
    if (lastOpenedUrlRef.current === fileUrl) return;
    lastOpenedUrlRef.current = fileUrl;
    provides.openDocumentUrl({ url: fileUrl, name: "document.pdf", autoActivate: true });
  }, [fileUrl, provides]);

  if (!activeDocumentId) {
    return <DocumentLoadingState />;
  }

  return (
    <DocumentContent documentId={activeDocumentId}>
      {({ isLoaded, isError, isLoading }) =>
        isLoaded ? (
          <EmbedPdfLoadedDocumentView documentId={activeDocumentId} />
        ) : isError ? (
          <ErrorState />
        ) : isLoading ? (
          <DocumentLoadingState />
        ) : (
          <DocumentLoadingState />
        )
      }
    </DocumentContent>
  );
}

function EmbedPdfLoadedDocumentView({ documentId }: { documentId: string }) {
  const zoom = useZoom(documentId);
  const zoomLevel = typeof zoom.state.currentZoomLevel === "number" ? zoom.state.currentZoomLevel : 1;
  const [isThumbnailOpen, setIsThumbnailOpen] = useState(false);
  const scroll = useScroll(documentId);
  const rotate = useRotate(documentId);
  const rotation = rotate.rotation ?? 0;
  const viewport = useViewportCapability();
  const [showScrollTop, setShowScrollTop] = useState(false);
  const rafRef = useRef<number | null>(null);
  const isZoomDisabled = rotation !== 0;

  const onZoomIn = useCallback(() => {
    if (isZoomDisabled) return;
    if (rotation === 0) {
      zoom.provides?.zoomIn();
      return;
    }

    const scope = viewport.provides?.forDocument(documentId);
    const m = scope?.getMetrics();
    const snap = m ? { x: m.scrollLeft, y: m.scrollTop } : null;

    zoom.provides?.zoomIn();

    if (scope && snap) {
      requestAnimationFrame(() => {
        requestAnimationFrame(() => {
          scope.scrollTo({ x: snap.x, y: snap.y, behavior: "instant" });
        });
      });
    }
  }, [zoom.provides, rotation, viewport.provides, documentId, isZoomDisabled]);

  const onZoomOut = useCallback(() => {
    if (isZoomDisabled) return;
    if (rotation === 0) {
      zoom.provides?.zoomOut();
      return;
    }

    const scope = viewport.provides?.forDocument(documentId);
    const m = scope?.getMetrics();
    const snap = m ? { x: m.scrollLeft, y: m.scrollTop } : null;

    zoom.provides?.zoomOut();

    if (scope && snap) {
      requestAnimationFrame(() => {
        requestAnimationFrame(() => {
          scope.scrollTo({ x: snap.x, y: snap.y, behavior: "instant" });
        });
      });
    }
  }, [zoom.provides, rotation, viewport.provides, documentId, isZoomDisabled]);
  const onResetZoom = useCallback(() => {
    if (isZoomDisabled) return;
    zoom.provides?.requestZoom(1);
  }, [zoom.provides, isZoomDisabled]);
  const onToggleThumbnails = useCallback(() => setIsThumbnailOpen((value) => !value), []);
  const currentPageIndex = Math.max(0, (scroll.state.currentPage || 1) - 1);
  const onSelectThumbnail = useCallback(
    (pageIndex: number) => {
      scroll.provides?.scrollToPage({ pageNumber: pageIndex + 1, behavior: "smooth", alignY: 0 });
    },
    [scroll.provides],
  );

  const onRotateLeft = useCallback(() => rotate.provides?.rotateBackward(), [rotate.provides]);
  const onRotateRight = useCallback(() => rotate.provides?.rotateForward(), [rotate.provides]);
  const onResetRotation = useCallback(() => rotate.provides?.setRotation(0), [rotate.provides]);

  useEffect(() => {
    const provides = viewport.provides;
    if (!provides) return;

    const scope = provides.forDocument(documentId);

    const sync = () => {
      rafRef.current = null;
      const m = scope.getMetrics();
      // Comportamiento tipo WhatsApp: aparece solo cuando realmente estás "abajo".
      setShowScrollTop(m.scrollTop > Math.max(120, m.clientHeight * 0.5));
    };

    const off = scope.onScrollChange(() => {
      if (rafRef.current != null) return;
      rafRef.current = requestAnimationFrame(sync);
    });

    sync();

    return () => {
      off?.();
      if (rafRef.current != null) cancelAnimationFrame(rafRef.current);
      rafRef.current = null;
    };
  }, [viewport.provides, documentId]);

  const onScrollToTop = useCallback(() => {
    const scope = viewport.provides?.forDocument(documentId);
    scope?.scrollTo({ x: 0, y: 0, behavior: "smooth" });
  }, [viewport.provides, documentId]);

  return (
    <>
      <div className={styles.toolbarShell} role="toolbar" aria-label="Toolbar PDF">
        <AppPdfToolbar
          zoomLevel={zoomLevel}
          onZoomIn={onZoomIn}
          onZoomOut={onZoomOut}
          onResetZoom={onResetZoom}
          onToggleThumbnails={onToggleThumbnails}
          isThumbnailOpen={isThumbnailOpen}
          isZoomDisabled={isZoomDisabled}
          onRotateLeft={onRotateLeft}
          onRotateRight={onRotateRight}
        />
      </div>
      <div className={styles.main}>
        <button
          type="button"
          className={`${styles.scrollTopFab} ${showScrollTop ? "" : styles.scrollTopFabHidden}`}
          onClick={onScrollToTop}
          aria-label="Ir arriba"
          title="Ir arriba"
        >
          <UpOutlined aria-hidden="true" />
        </button>
        {isThumbnailOpen ? (
          <aside className={styles.thumbnails} aria-label="Panel thumbnails">
            <ThumbnailsPane documentId={documentId} className={styles.thumbnailsPane}>
              {(meta) => (
                <div
                  key={meta.pageIndex}
                  className={`${styles.thumbRow} ${meta.pageIndex === currentPageIndex ? styles.thumbRowActive : ""}`}
                  style={{ top: meta.top, height: meta.wrapperHeight }}
                  role="button"
                  tabIndex={0}
                  aria-label={`Ir a página ${meta.pageIndex + 1}`}
                  onClick={() => onSelectThumbnail(meta.pageIndex)}
                  onKeyDown={(e) => {
                    if (e.key === "Enter" || e.key === " ") onSelectThumbnail(meta.pageIndex);
                  }}
                >
                  <ThumbImg documentId={documentId} meta={meta} className={styles.thumbImg} />
                  <div className={styles.thumbLabel}>{meta.pageIndex + 1}</div>
                </div>
              )}
            </ThumbnailsPane>
          </aside>
        ) : null}
        <Viewport documentId={documentId} className={styles.viewport}>
            <Scroller
              documentId={documentId}
            renderPage={({ pageIndex, width, height, rotatedWidth, rotatedHeight }) => (
              <div
                className={styles.pageLayer}
                // Mantener el layout estable para evitar "jump" del scroll virtualizado.
                // El Rotate plugin transforma el contenido, pero el "slot" del layout
                // se mantiene en dimensiones base (no rotadas).
                style={rotation === 0 ? { width: "100%", height: "100%" } : { width, height }}
              >
                {rotation === 0 ? (
                  <RenderLayer documentId={documentId} pageIndex={pageIndex} />
                ) : (
                  <Rotate documentId={documentId} pageIndex={pageIndex}>
                    <RenderLayer documentId={documentId} pageIndex={pageIndex} />
                  </Rotate>
                )}
              </div>
            )}
          />
        </Viewport>
      </div>
    </>
  );
}
