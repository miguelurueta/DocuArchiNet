import { useCallback, useEffect, useRef } from "react";
import { useZoom } from "@embedpdf/plugin-zoom/react";

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

  const onZoomIn = useCallback(() => {
    if (zoomLevel >= 4) return;
    zoom.provides?.zoomIn();
  }, [zoom.provides, zoomLevel]);

  const onZoomOut = useCallback(() => zoom.provides?.zoomOut(), [zoom.provides]);
  const onResetZoom = useCallback(() => zoom.provides?.requestZoom(1), [zoom.provides]);

  return (
    <>
      <div className={styles.toolbarShell} role="toolbar" aria-label="Toolbar PDF">
        <AppPdfToolbar
          zoomLevel={zoomLevel}
          onZoomIn={onZoomIn}
          onZoomOut={onZoomOut}
          onResetZoom={onResetZoom}
        />
      </div>
      <Viewport documentId={documentId} className={styles.viewport}>
        <Scroller
          documentId={documentId}
          renderPage={({ pageIndex }) => (
            <RenderLayer documentId={documentId} pageIndex={pageIndex} />
          )}
        />
      </Viewport>
    </>
  );
}
