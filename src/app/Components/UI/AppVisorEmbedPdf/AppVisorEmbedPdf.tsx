import { useCallback, useEffect, useRef, useState } from "react";
import { useZoom } from "@embedpdf/plugin-zoom/react";
import { ThumbnailsPane, ThumbImg } from "@embedpdf/plugin-thumbnail/react";
import { useScroll } from "@embedpdf/plugin-scroll/react";

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
  const pluginRegistration = createBasicPluginRegistration();

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

  const onZoomIn = useCallback(() => {
    if (zoomLevel >= 4) return;
    zoom.provides?.zoomIn();
  }, [zoom.provides, zoomLevel]);

  const onZoomOut = useCallback(() => zoom.provides?.zoomOut(), [zoom.provides]);
  const onResetZoom = useCallback(() => zoom.provides?.requestZoom(1), [zoom.provides]);
  const onToggleThumbnails = useCallback(() => setIsThumbnailOpen((value) => !value), []);
  const currentPageIndex = Math.max(0, (scroll.state.currentPage || 1) - 1);
  const onSelectThumbnail = useCallback(
    (pageIndex: number) => {
      scroll.provides?.scrollToPage({ pageNumber: pageIndex + 1, behavior: "smooth", alignY: 0 });
    },
    [scroll.provides],
  );

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
        />
      </div>
      <div className={styles.main}>
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
            renderPage={({ pageIndex }) => (
              <RenderLayer documentId={documentId} pageIndex={pageIndex} />
            )}
          />
        </Viewport>
      </div>
    </>
  );
}
