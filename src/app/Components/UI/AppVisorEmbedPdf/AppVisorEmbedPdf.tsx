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
import styles from "./styles/AppVisorEmbedPdf.module.css";
import type { AppVisorEmbedPdfProps } from "./types/AppVisorEmbedPdfProps";
import {
  DocumentLoadingState,
  EmptyState,
  EngineLoadingState,
  ErrorState,
} from "./presentation/States";

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
  if (engineState.status === "loading") {
    return (
      <div className={cx(styles.root, className)} style={style}>
        <EngineLoadingState />
      </div>
    );
  }
  if (engineState.status === "error") {
    return (
      <div className={cx(styles.root, className)} style={style}>
        <ErrorState />
      </div>
    );
  }

  if (!effectiveFileUrl) {
    return (
      <div className={cx(styles.root, className)} style={style}>
        <EmptyState />
      </div>
    );
  }

  const pluginRegistration = createBasicPluginRegistration();
  const { provides } = useDocumentManagerCapability();
  const { activeDocumentId } = useActiveDocument();

  // Importante: el módulo consumidor solo provee `fileUrl`. Abrir documento es responsabilidad del visor.
  // Evita abrir repetidamente la misma URL: ref local por instancia.
  const lastOpenedUrlRef = useRef<string | null>(null);
  useEffect(() => {
    if (!provides) return;
    if (!effectiveFileUrl) return;
    if (lastOpenedUrlRef.current === effectiveFileUrl) return;
    lastOpenedUrlRef.current = effectiveFileUrl;
    provides.openDocumentUrl({ url: effectiveFileUrl, name: "document.pdf", autoActivate: true });
  }, [effectiveFileUrl, provides]);

  return (
    <div className={cx(styles.root, className)} style={style}>
      <EmbedPDF engine={engineState.engine} plugins={pluginRegistration}>
        {!activeDocumentId ? (
          <DocumentLoadingState />
        ) : (
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
        )}
      </EmbedPDF>
    </div>
  );
}
import { useEffect, useRef } from "react";
