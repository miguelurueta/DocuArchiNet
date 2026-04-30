import type { ReactNode } from "react";
import { useEffect, useMemo, useState } from "react";
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

export type AppVisorPdfSimpleProps = {
  input: AppVisorPdfInput | null;
  className?: string;
  "aria-label"?: string;
  loading?: boolean;
  error?: Error | null;
  onRetry?: () => void;
};

export function AppVisorPdfSimple(props: AppVisorPdfSimpleProps) {
  const {
    input,
    loading: loadingProp,
    error: errorProp,
    onRetry,
    className,
    "aria-label": ariaLabel,
  } = props;

  const resolvedAriaLabel = resolveAriaLabel({ ariaLabel });

  const engine = useMemo(
    () => createPdfjsEngine({ maxCacheEntries: 6, loadTimeoutMs: 20_000 }),
    [],
  );
  useEffect(() => () => engine.destroy(), [engine]);

  const [internalLoading, setInternalLoading] = useState(false);
  const [internalError, setInternalError] = useState<string | null>(null);

  const effectiveLoading = loadingProp ?? internalLoading;
  const effectiveErrorMessage = errorProp?.message ?? internalError;

  if (!input) {
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
      <div className={styles.viewport}>
        <VisorPdfViewport
          input={input}
          engine={engine}
          page={1}
          zoom={1}
          buffer={0}
          onLoadStateChange={(state) => {
            if (loadingProp === undefined) {
              setInternalLoading(state === "loading");
            }
            if (state !== "error" && errorProp === undefined) {
              setInternalError(null);
            }
          }}
          onError={(message) => {
            if (errorProp === undefined) {
              setInternalError(message);
            }
          }}
        />

        {effectiveLoading ? (
          <div className={styles.overlay}>
            <div role="status" className={styles.status}>
              Cargando PDF...
            </div>
          </div>
        ) : null}

        {effectiveErrorMessage ? (
          <div className={styles.overlay}>
            <div role="status" className={styles.status}>
              <div className={styles.errorMessage}>{effectiveErrorMessage}</div>
              {onRetry ? (
                <AppButton variant="secondary" size="sm" onClick={onRetry}>
                  Reintentar
                </AppButton>
              ) : null}
            </div>
          </div>
        ) : null}
      </div>
    </section>
  );
}

