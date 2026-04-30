import type { ReactNode } from "react";
import { useEffect, useMemo, useRef, useState } from "react";
import { AppButton } from "../AppButton";
import styles from "./AppVisorPdf.module.css";
import type { AppVisorPdfInput } from "./domain/visorPdf.types";
import { createPdfjsEngine } from "./engine/pdfjsEngine";
import { VisorPdfViewport } from "./presentation/VisorPdfViewport";

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

function resolveAriaLabel({ ariaLabel }: { ariaLabel?: string }) {
  return ariaLabel?.trim() ? ariaLabel : "Visor PDF";
}

function renderStatus(content: ReactNode) {
  return (
    <div className={styles.viewport}>
      <div role="status" className={styles.status}>
        {content}
      </div>
    </div>
  );
}

export type AppVisorPdfCoreProps = {
  input: AppVisorPdfInput | null;
  className?: string;
  "aria-label"?: string;
};

export function AppVisorPdfCore(props: AppVisorPdfCoreProps) {
  const { input, className, "aria-label": ariaLabel } = props;
  const resolvedAriaLabel = resolveAriaLabel({ ariaLabel });

  const engine = useMemo(
    () => createPdfjsEngine({ maxCacheEntries: 12, loadTimeoutMs: 20_000 }),
    [],
  );
  useEffect(() => () => engine.destroy(), [engine]);

  const [reloadToken, setReloadToken] = useState(0);
  const [zoom, setZoom] = useState(1);
  const [renderZoom, setRenderZoom] = useState(1);
  const zoomDebounceRef = useRef<number | null>(null);
  const [pageCount, setPageCount] = useState(0);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const effectiveInput = useMemo(() => {
    if (!input) return null;
    if (input.kind === "url") {
      return { kind: "url", url: input.url } as const;
    }
    return { kind: "bytes", bytes: input.bytes } as const;
  }, [input, reloadToken]);

  useEffect(() => {
    setZoom(1);
    setRenderZoom(1);
    setPageCount(0);
    setError(null);
  }, [effectiveInput]);

  useEffect(() => {
    if (zoomDebounceRef.current) {
      window.clearTimeout(zoomDebounceRef.current);
    }
    zoomDebounceRef.current = window.setTimeout(() => {
      setRenderZoom(zoom);
    }, 150);
    return () => {
      if (zoomDebounceRef.current) {
        window.clearTimeout(zoomDebounceRef.current);
        zoomDebounceRef.current = null;
      }
    };
  }, [zoom]);

  const zoomOutDisabled = zoom <= 0.25;
  const zoomInDisabled = zoom >= 4;

  if (!effectiveInput) {
    return (
      <section
        aria-label={resolvedAriaLabel}
        className={joinClasses(styles.root, className)}
      >
        {renderStatus("No hay PDF seleccionado")}
      </section>
    );
  }

  return (
    <section
      aria-label={resolvedAriaLabel}
      className={joinClasses(styles.root, className)}
    >
      <div className={styles.toolbar}>
        <div className={styles.toolbarGroup}>
          <AppButton
            variant="secondary"
            size="sm"
            disabled={loading || zoomOutDisabled}
            onClick={() => setZoom((z) => Math.max(0.25, Math.round((z - 0.25) * 100) / 100))}
          >
            -
          </AppButton>
          <AppButton
            variant="secondary"
            size="sm"
            disabled={loading || zoomInDisabled}
            onClick={() => setZoom((z) => Math.min(4, Math.round((z + 0.25) * 100) / 100))}
          >
            +
          </AppButton>
        </div>
      </div>

      <div className={styles.layout}>
        <div className={styles.thumbnailsRail} />
        <div className={styles.viewportScrollHost}>
          <VisorPdfViewport
            input={effectiveInput}
            engine={engine}
            page={1}
            zoom={renderZoom}
            buffer={1}
            continuous
            onDocumentInfo={(info) => {
              setPageCount(info.pageCount);
            }}
            onLoadStateChange={(state) => {
              setLoading(state === "loading");
            if (state !== "error") setError(null);
          }}
          onError={(message) => setError(message)}
        />

        {loading ? (
          <div className={styles.overlay}>
            <div role="status" className={styles.status}>
              Cargando PDF...
            </div>
          </div>
        ) : null}

        {error ? (
          <div className={styles.overlay}>
            <div role="status" className={styles.status}>
              <div className={styles.errorMessage}>{error}</div>
              <AppButton
                variant="secondary"
                size="sm"
                onClick={() => setReloadToken((v) => v + 1)}
              >
                Reintentar
              </AppButton>
            </div>
          </div>
        ) : null}
      </div>
      </div>
    </section>
  );
}
