import type { ReactNode } from "react";
import { AppButton } from "../AppButton";
import styles from "./AppVisorPdf.module.css";
import type { AppVisorPdfProps } from "./domain/visorPdf.types";
import { useAppVisorPdfController } from "./application/useAppVisorPdfController";
import { AppVisorPdfToolbar } from "./presentation/AppVisorPdfToolbar";

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
    loading = false,
    error = null,
    onRetry,
    className,
    "aria-label": ariaLabel,
    ...controllerProps
  } = props;

  const resolvedAriaLabel = resolveAriaLabel({ ariaLabel });
  const controller = useAppVisorPdfController(controllerProps);
  const toolbarDisabled = Boolean(readOnly) || Boolean(loading);

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
        />
        {renderStatus("No hay PDF seleccionado")}
      </section>
    );
  }

  if (loading) {
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
        />
        {renderStatus("Cargando PDF...")}
      </section>
    );
  }

  if (error) {
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
        />
        {renderStatus(
          <div>
            <div className={styles.errorMessage}>{error.message}</div>
            {onRetry ? (
              <AppButton variant="secondary" size="sm" onClick={onRetry}>
                Reintentar
              </AppButton>
            ) : null}
          </div>,
        )}
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
      />
      {renderStatus("Viewport PDF (mock)")}
    </section>
  );
}

