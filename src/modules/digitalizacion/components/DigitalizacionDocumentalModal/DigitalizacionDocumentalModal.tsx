import { useCallback, useMemo } from "react";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppModal } from "../../../../app/Components/UI/AppModal";
import { useDigitalizacionDocumentalState } from "../../hooks/useDigitalizacionDocumentalState";
import type {
  DigitalizacionDocumentalProps,
  DigitalizacionFunctionalError,
} from "../../types/digitalizacion.types";
import styles from "./DigitalizacionDocumentalModal.module.css";

const buildTitle = (modo?: string) =>
  modo === "adjuntar" ? "Adjuntar digitalizacion" : "Digitalizar documento";

const readableMode = (modo?: string) => (modo === "adjuntar" ? "adjuntar" : "crear");

export function DigitalizacionDocumentalModal({
  open,
  context,
  onClose,
  onCompleted,
  onError,
}: DigitalizacionDocumentalProps) {
  const handleInvalidContext = useCallback(
    (error: DigitalizacionFunctionalError) => {
      onError?.(error);
    },
    [onError],
  );

  const { state, clear, clearPages, canSubmit } = useDigitalizacionDocumentalState({
    open,
    context,
    onInvalidContext: handleInvalidContext,
  });

  const activeContext = state.context ?? context;
  const title = activeContext?.titulo ?? buildTitle(activeContext?.modo);
  const primaryLabel =
    activeContext?.modo === "adjuntar" ? "Adjuntar digitalizacion" : "Guardar documento";
  const submitDisabledReason = state.validationError
    ? state.validationError.message
    : "Pendiente captura PDF";

  const handleCancel = useCallback(() => {
    clear();
    onCompleted({ accion: "cancelado" });
    onClose();
  }, [clear, onClose, onCompleted]);

  const handleClose = useCallback(() => {
    clear();
    onClose();
  }, [clear, onClose]);

  const summaryItems = useMemo(
    () => [
      ["Gabinete", activeContext?.nombreGabinete || "Sin gabinete"],
      ["Radicado", activeContext?.radicado || "No informado"],
      [
        "Documento destino",
        activeContext?.idDocumentoDestino
          ? String(activeContext.idDocumentoDestino)
          : activeContext?.modo === "adjuntar"
            ? "Requerido"
            : "Nuevo documento",
      ],
    ],
    [activeContext],
  );

  return (
    <AppModal
      open={open}
      title={title}
      width={980}
      onClose={handleClose}
      secondaryAction={{
        label: "Cancelar",
        onClick: handleCancel,
      }}
      primaryAction={{
        label: primaryLabel,
        disabled: !canSubmit,
      }}
    >
      <section
        className={styles.shell}
        aria-label="Digitalizacion documental"
        data-testid="digitalizacion-modal"
      >
        <header className={styles.header}>
          <div className={styles.titleLine}>
            <span className={styles.modeBadge}>{readableMode(activeContext?.modo)}</span>
            <span className={styles.footerNote}>{submitDisabledReason}</span>
          </div>
          <div className={styles.summary}>
            {summaryItems.map(([label, value]) => (
              <div className={styles.summaryItem} key={label}>
                <span className={styles.summaryLabel}>{label}</span>
                <span className={styles.summaryValue}>{value}</span>
              </div>
            ))}
          </div>
        </header>

        {state.validationError ? (
          <div className={styles.error} role="alert">
            {state.validationError.message}
          </div>
        ) : null}

        <div className={styles.toolbar}>
          <div className={styles.scannerSelect} aria-disabled="true">
            Scanner no inicializado
          </div>
          <AppButton variant="secondary" disabled>
            Escanear
          </AppButton>
          <AppButton variant="ghost" onClick={clearPages}>
            Limpiar
          </AppButton>
          <AppButton disabled={!canSubmit}>{primaryLabel}</AppButton>
        </div>

        <main className={styles.main}>
          <section className={styles.panel} aria-label="Miniaturas">
            <div className={styles.panelHeader}>Miniaturas</div>
            <div className={styles.panelBody}>
              <span className={styles.placeholderTitle}>Sin paginas</span>
              <span>0 paginas capturadas</span>
            </div>
          </section>

          <section className={styles.panel} aria-label="Preview digitalizacion">
            <div className={styles.panelHeader}>Preview PDF</div>
            <div className={`${styles.panelBody} ${styles.preview}`}>
              <span className={styles.placeholderTitle}>PDF pendiente</span>
              <span>La generacion se habilitara en la fase de scanner.</span>
            </div>
          </section>

          <section className={styles.panel} aria-label="Metadata documental">
            <div className={styles.panelHeader}>Metadata</div>
            <div className={styles.panelBody}>
              <span className={styles.placeholderTitle}>
                {state.metadata.required ? "Metadata requerida" : "Metadata opcional"}
              </span>
              <span>TRD sin resolver</span>
            </div>
          </section>
        </main>
      </section>
    </AppModal>
  );
}
