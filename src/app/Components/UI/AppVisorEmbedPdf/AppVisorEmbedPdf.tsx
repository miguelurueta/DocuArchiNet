import { useEffect, useMemo, useRef, useState } from "react";
import type { AppPdfSource } from "./domain/pdf.types";
import { EmbedPDF } from "@embedpdf/core/react";
import { usePdfiumEngine } from "@embedpdf/engines/react";
import {
  DocumentContent,
  useActiveDocument,
  useDocumentManagerCapability,
} from "@embedpdf/plugin-document-manager/react";
import { Viewport } from "@embedpdf/plugin-viewport/react";
import { Scroller, ScrollStrategy } from "@embedpdf/plugin-scroll/react";
import { RenderLayer } from "@embedpdf/plugin-render/react";
import { useRenderCapability } from "@embedpdf/plugin-render/react";
import { ZoomGestureWrapper, useZoom } from "@embedpdf/plugin-zoom/react";
import { GlobalPointerProvider } from "@embedpdf/plugin-interaction-manager/react";

import {
  DocumentManagerPluginPackage,
  type DocumentManagerPluginConfig,
} from "@embedpdf/plugin-document-manager";
import { ViewportPluginPackage } from "@embedpdf/plugin-viewport";
import { ScrollPluginPackage } from "@embedpdf/plugin-scroll";
import { RenderPluginPackage } from "@embedpdf/plugin-render";
import { ZoomPluginPackage, ZoomMode, type ZoomPluginConfig } from "@embedpdf/plugin-zoom";
import { InteractionManagerPluginPackage } from "@embedpdf/plugin-interaction-manager";

import { AppButton } from "../AppButton";

type PluginRegistration = { package: unknown; config?: unknown };

function buildPluginBatch(): PluginRegistration[] {
  const zoomConfig: Partial<ZoomPluginConfig> = {
    defaultZoomLevel: ZoomMode.FitWidth,
    minZoom: 0.25,
    maxZoom: 4,
    zoomStep: 0.25,
  };

  const documentManagerConfig: Partial<DocumentManagerPluginConfig> = {
    maxDocuments: 1,
  };

  return [
    { package: DocumentManagerPluginPackage, config: documentManagerConfig },
    { package: ViewportPluginPackage },
    { package: ScrollPluginPackage },
    { package: RenderPluginPackage },
    { package: ZoomPluginPackage, config: zoomConfig },
    { package: InteractionManagerPluginPackage },
  ];
}

export function AppVisorEmbedPdf({
  source,
  className,
  "aria-label": ariaLabel,
}: {
  source: AppPdfSource | null;
  className?: string;
  "aria-label"?: string;
}) {
  const { engine, isLoading, error } = usePdfiumEngine();

  const plugins = useMemo(() => buildPluginBatch(), []);

  if (isLoading || !engine) {
    return (
      <section className={className} aria-label={ariaLabel ?? "Visor PDF"}>
        <p style={{ margin: 0, padding: 12 }}>Cargando visor PDFÃ¢â‚¬Â¦</p>
      </section>
    );
  }

  if (error) {
    return (
      <section className={className} aria-label={ariaLabel ?? "Visor PDF"}>
        <p style={{ margin: 0, padding: 12 }}>No se pudo inicializar PDFium.</p>
      </section>
    );
  }

  return (
    <section
      className={className}
      aria-label={ariaLabel ?? "Visor PDF"}
      style={{ height: "100%", width: "100%", display: "grid", gridTemplateRows: "auto 1fr" }}
    >
      <EmbedPDF engine={engine} plugins={plugins as any}>
        <GlobalPointerProvider>
          <EmbedPdfOpenFromSource source={source} />
          <EmbedPdfToolbar />
          <EmbedPdfViewport />
        </GlobalPointerProvider>
      </EmbedPDF>
    </section>
  );
}

function EmbedPdfOpenFromSource({ source }: { source: AppPdfSource | null }) {
  const { provides } = useDocumentManagerCapability();
  const lastOpenKeyRef = useRef<string | null>(null);

  useEffect(() => {
    if (!source) return;
    if (!provides) return;

    const openKey =
      source.kind === "url"
        ? `url:${source.url}`
        : `bytes:${source.filename ?? "unknown"}:${source.bytes.byteLength}`;
    if (lastOpenKeyRef.current === openKey) return;
    lastOpenKeyRef.current = openKey;

    if (source.kind === "url") {
      provides.openDocumentUrl({
        url: source.url,
        name: source.filename,
        autoActivate: true,
      });
      return;
    }

    provides.openDocumentBuffer({
      name: source.filename ?? "document.pdf",
      buffer: source.bytes,
      autoActivate: true,
    });
  }, [provides, source]);

  return null;
}

function EmbedPdfToolbar() {
  const { activeDocumentId } = useActiveDocument();
  return (
    <div style={{ display: "flex", gap: 8, alignItems: "center", padding: "10px 12px" }}>
      {activeDocumentId ? (
        <EmbedPdfZoomToolbar documentId={activeDocumentId} />
      ) : (
        <>
          <AppButton size="sm" variant="secondary" disabled>
            -
          </AppButton>
          <AppButton size="sm" variant="secondary" disabled>
            +
          </AppButton>
        </>
      )}
    </div>
  );
}

function EmbedPdfZoomToolbar({ documentId }: { documentId: string }) {
  const zoom = useZoom(documentId);
  const disabled = !zoom.provides;

  return (
    <>
      <AppButton size="sm" variant="secondary" disabled={disabled} onClick={() => zoom.provides?.zoomOut()}>
        -
      </AppButton>
      <AppButton size="sm" variant="secondary" disabled={disabled} onClick={() => zoom.provides?.zoomIn()}>
        +
      </AppButton>
      <AppButton
        size="sm"
        variant="secondary"
        disabled={disabled}
        onClick={() => zoom.provides?.requestZoom(ZoomMode.FitWidth)}
      >
        Fit width
      </AppButton>
    </>
  );
}

function EmbedPdfViewport() {
  const { activeDocumentId } = useActiveDocument();
  if (!activeDocumentId) {
    return <p style={{ margin: 0, padding: 12 }}>Abriendo documentoÃ¢â‚¬Â¦</p>;
  }

  return (
    <DocumentContent documentId={activeDocumentId}>
      {({ isLoaded, isError, isLoading }) =>
        isLoaded ? (
          <div style={{ minHeight: 0, height: "100%", overflow: "hidden" }}>
            <ZoomGestureWrapper documentId={activeDocumentId}>
              <Viewport documentId={activeDocumentId} style={{ height: "100%", width: "100%" }}>
                <Scroller
                  documentId={activeDocumentId}
                  strategy={ScrollStrategy.Vertical}
                  style={{ height: "100%", width: "100%", overflow: "auto", padding: 12 }}
                >
                  <RenderLayer
                    documentId={activeDocumentId}
                    pageIndex={0}
                    style={{ display: "block", maxWidth: "100%", height: "auto" }}
                  />
                  <RenderPageDebug documentId={activeDocumentId} />
                </Scroller>
              </Viewport>
            </ZoomGestureWrapper>
          </div>
        ) : isError ? (
          <p style={{ margin: 0, padding: 12 }}>No se pudo cargar el PDF.</p>
        ) : isLoading ? (
          <p style={{ margin: 0, padding: 12 }}>Cargando PDFÃ¢â‚¬Â¦</p>
        ) : (
          <p style={{ margin: 0, padding: 12 }}>Preparando visorÃ¢â‚¬Â¦</p>
        )
      }
    </DocumentContent>
  );
}

function RenderPageDebug({ documentId }: { documentId: string }) {
  const { provides } = useRenderCapability();
  const [url, setUrl] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let revoked: string | null = null;
    setError(null);
    setUrl(null);
    if (!provides) return () => undefined;

    const task = provides.forDocument(documentId).renderPage({
      pageIndex: 0,
      options: { scale: 1 },
    });

    task.wait(
      (blob) => {
        const nextUrl = URL.createObjectURL(blob);
        revoked = nextUrl;
        setUrl(nextUrl);
      },
      (err) => {
        setError(typeof err === "string" ? err : JSON.stringify(err));
      },
    );

    return () => {
      if (revoked) URL.revokeObjectURL(revoked);
    };
  }, [documentId, provides]);

  if (error) return <p style={{ margin: "12px 0 0" }}>Render debug error: {error}</p>;
  if (!url) return <p style={{ margin: "12px 0 0" }}>Render debug: esperando blobÃ¢â‚¬Â¦</p>;
  return <img alt="Render debug page 1" src={url} style={{ display: "block", maxWidth: "100%" }} />;
}

