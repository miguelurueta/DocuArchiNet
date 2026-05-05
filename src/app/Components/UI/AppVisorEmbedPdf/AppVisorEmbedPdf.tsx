import { useEffect, useRef } from "react";

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
          <Viewport documentId={activeDocumentId} className={styles.viewport}>
            <Scroller
              documentId={activeDocumentId}
              renderPage={({ pageIndex }) => (
                <RenderLayer documentId={activeDocumentId} pageIndex={pageIndex} />
              )}
            />
          </Viewport>
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
