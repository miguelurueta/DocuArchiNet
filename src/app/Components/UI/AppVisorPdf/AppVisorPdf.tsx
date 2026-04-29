import type { ReactNode } from "react";
import { useEffect, useMemo, useState } from "react";
import { AppButton } from "../AppButton";
import styles from "./AppVisorPdf.module.css";
import type { AppVisorPdfProps } from "./domain/visorPdf.types";
import { useAppVisorPdfController } from "./application/useAppVisorPdfController";
import { AppVisorPdfToolbar } from "./presentation/AppVisorPdfToolbar";
import { createPdfjsEngine } from "./engine/pdfjsEngine";
import { createFabricEngine } from "./engine/fabricEngine";
import { VisorPdfViewport } from "./presentation/VisorPdfViewport";
import { useIsCompact } from "./presentation/useIsCompact";
import { VisorPdfThumbnails } from "./presentation/VisorPdfThumbnails";

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

export function AppVisorPdf(props: AppVisorPdfProps) {
  const {
    input,
    readOnly = false,
    loading: loadingProp,
    error: errorProp,
    onRetry,
    className,
    "aria-label": ariaLabel,
    ...controllerProps
  } = props;

  const resolvedAriaLabel = resolveAriaLabel({ ariaLabel });
  const controller = useAppVisorPdfController(controllerProps);
  const isCompact = useIsCompact(768);

  const engine = useMemo(() => createPdfjsEngine({ maxCacheEntries: 12 }), []);
  useEffect(() => () => engine.destroy(), [engine]);

  const annotateEngine = useMemo(() => createFabricEngine(), []);
  useEffect(() => () => annotateEngine.destroy(), [annotateEngine]);
  useEffect(() => {
    annotateEngine.setTool(controller.tool);
  }, [annotateEngine, controller.tool]);

  const [pageCount, setPageCount] = useState<number>(0);
  const [thumbnailsOpen, setThumbnailsOpen] = useState(false);
  const thumbnailsLabelId = "app-visorpdf-thumbnails-toggle";

  const [internalLoading, setInternalLoading] = useState(false);
  const [internalError, setInternalError] = useState<string | null>(null);

  const effectiveLoading = loadingProp ?? internalLoading;
  const effectiveErrorMessage = errorProp?.message ?? internalError;

  const toolbarDisabled = Boolean(readOnly) || Boolean(effectiveLoading);

  if (!input) {
    return (
      <section
        aria-label={resolvedAriaLabel}
        className={joinClasses(styles.root, className)}
      >
        <AppVisorPdfToolbar
          disabled={toolbarDisabled}
          page={controller.page}
          onPageChange={controller.setPage}
          zoom={controller.zoom}
          onZoomChange={controller.setZoom}
          tool={controller.tool}
          onToolChange={controller.setTool}
          onUndo={readOnly ? undefined : () => annotateEngine.undo()}
          onRedo={readOnly ? undefined : () => annotateEngine.redo()}
          isCompact={isCompact}
          thumbnailsOpen={thumbnailsOpen}
          thumbnailsControlsId="app-visorpdf-thumbnails"
          thumbnailsLabelId={thumbnailsLabelId}
          onToggleThumbnails={() => setThumbnailsOpen((v) => !v)}
        />
        {renderStatus("No hay PDF seleccionado")}
      </section>
    );
  }

  return (
    <section
      aria-label={resolvedAriaLabel}
      className={joinClasses(styles.root, className)}
    >
      <AppVisorPdfToolbar
        disabled={toolbarDisabled}
        page={controller.page}
        onPageChange={controller.setPage}
        zoom={controller.zoom}
        onZoomChange={controller.setZoom}
        tool={controller.tool}
        onToolChange={controller.setTool}
        onUndo={readOnly ? undefined : () => annotateEngine.undo()}
        onRedo={readOnly ? undefined : () => annotateEngine.redo()}
        isCompact={isCompact}
        thumbnailsOpen={thumbnailsOpen}
        thumbnailsControlsId="app-visorpdf-thumbnails"
        thumbnailsLabelId={thumbnailsLabelId}
        onToggleThumbnails={() => setThumbnailsOpen((v) => !v)}
      />
      <div className={styles.layout}>
        <div className={styles.thumbnailsRail}>
          <VisorPdfThumbnails
            pageCount={pageCount}
            activePage={controller.page}
            onSelectPage={controller.setPage}
            onRequestClose={() => setThumbnailsOpen(false)}
            variant="rail"
            isOpen={!isCompact && thumbnailsOpen}
            labelledById={thumbnailsLabelId}
            restoreFocusId={thumbnailsLabelId}
          />
        </div>
        <div className={styles.viewport}>
        <VisorPdfViewport
          input={input}
          engine={engine}
          annotateEngine={readOnly ? undefined : annotateEngine}
          page={controller.page}
          zoom={controller.zoom}
          buffer={1}
          onDocumentInfo={(info) => setPageCount(info.pageCount)}
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
        <VisorPdfThumbnails
          pageCount={pageCount}
          activePage={controller.page}
          onSelectPage={controller.setPage}
          onRequestClose={() => setThumbnailsOpen(false)}
          variant="overlay"
          isOpen={isCompact && thumbnailsOpen}
          labelledById={thumbnailsLabelId}
          restoreFocusId={thumbnailsLabelId}
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
      </div>
    </section>
  );
}

