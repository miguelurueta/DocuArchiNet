import { memo } from "react";
import {
  DownloadOutlined,
  DeleteOutlined,
  FileSyncOutlined,
  FormOutlined,
  LockOutlined,
  SaveOutlined,
  UnlockOutlined,
  MenuOutlined,
  MoreOutlined,
  PrinterOutlined,
  QuestionCircleOutlined,
  RotateLeftOutlined,
  RotateRightOutlined,
  ZoomInOutlined,
  ZoomOutOutlined,
} from "@ant-design/icons";

import { AppDropdown, type AppDropdownItem } from "../../AppDropdown";
import styles from "./AppPdfToolbar.module.css";

export interface AppPdfToolbarProps {
  zoomLevel: number;

  onZoomIn(): void;
  onZoomOut(): void;
  onResetZoom(): void;

  onToggleThumbnails(): void;
  isThumbnailOpen: boolean;

  isZoomDisabled?: boolean;

  onRotateLeft(): void;
  onRotateRight(): void;

  onToggleSignatureModal(): void;
  isSignatureModalOpen: boolean;
  isSignatureDisabled?: boolean;

  onDeleteSelectedSignature(): void;
  canDeleteSelectedSignature: boolean;
  isDeleteSelectedSignatureDisabled?: boolean;

  onSaveSignedPdf(): void;
  isSignatureLocked: boolean;
  isSaveSignedPdfDisabled?: boolean;
  isSavingSignedPdf?: boolean;
  isSignatureLockToggleDisabled?: boolean;

  onPrint(): void;
  onExport(): void;
  isPrintDisabled?: boolean;
  isExportDisabled?: boolean;

  onSaveAnnotatedPages?: () => void;
  isSaveAnnotatedPagesDisabled?: boolean;
  isSavingAnnotatedPages?: boolean;
  saveAnnotatedPagesProgress?: number;

  onStartGuideTour?: () => void;
  isGuideTourAvailable?: boolean;
}

function formatZoom(zoomLevel: number) {
  const percent = Math.round(zoomLevel * 100);
  return `${percent}%`;
}

export const AppPdfToolbar = memo(function AppPdfToolbar({
  zoomLevel,
  onZoomIn,
  onZoomOut,
  onResetZoom,
  onToggleThumbnails,
  isThumbnailOpen,
  isZoomDisabled = false,
  onRotateLeft,
  onRotateRight,
  onToggleSignatureModal,
  isSignatureModalOpen,
  isSignatureDisabled = false,
  onDeleteSelectedSignature,
  canDeleteSelectedSignature,
  isDeleteSelectedSignatureDisabled = false,
  onSaveSignedPdf,
  isSignatureLocked,
  isSaveSignedPdfDisabled = false,
  isSavingSignedPdf = false,
  isSignatureLockToggleDisabled = false,
  onPrint,
  onExport,
  isPrintDisabled = false,
  isExportDisabled = false,
  onSaveAnnotatedPages,
  isSaveAnnotatedPagesDisabled = false,
  isSavingAnnotatedPages = false,
  saveAnnotatedPagesProgress,
  onStartGuideTour,
  isGuideTourAvailable = false,
}: AppPdfToolbarProps) {
  const zoomDisabledTitle = isZoomDisabled
    ? "Zoom deshabilitado cuando hay rotación (estabilidad)"
    : undefined;

  const showGuideTourButton = Boolean(onStartGuideTour) && isGuideTourAvailable;
  const normalizedSaveProgress =
    typeof saveAnnotatedPagesProgress === "number" && Number.isFinite(saveAnnotatedPagesProgress)
      ? Math.min(100, Math.max(0, Math.round(saveAnnotatedPagesProgress * 100)))
      : null;
  const isSaveAnnotatedPagesReady = Boolean(onSaveAnnotatedPages) && !isSaveAnnotatedPagesDisabled && !isSavingAnnotatedPages;
  const isSaveAnnotatedPagesBlocked = Boolean(onSaveAnnotatedPages) && isSaveAnnotatedPagesDisabled && !isSavingAnnotatedPages;
  const overflowItems: AppDropdownItem[] = [
    {
      key: "rotate-left",
      label: "Rotar izquierda",
      icon: <RotateLeftOutlined />,
      onSelect: onRotateLeft,
    },
    {
      key: "rotate-right",
      label: "Rotar derecha",
      icon: <RotateRightOutlined />,
      onSelect: onRotateRight,
    },
    {
      key: "reset-zoom",
      label: "Reset zoom",
      icon: <FileSyncOutlined />,
      disabled: isZoomDisabled,
      onSelect: onResetZoom,
    },
    {
      key: "print",
      label: "Imprimir",
      icon: <PrinterOutlined />,
      disabled: isPrintDisabled,
      onSelect: onPrint,
    },
    {
      key: "export",
      label: "Exportar",
      icon: <DownloadOutlined />,
      disabled: isExportDisabled,
      onSelect: onExport,
    },
    ...(showGuideTourButton
      ? [
          {
            key: "guide",
            label: "Guia interactiva",
            icon: <QuestionCircleOutlined />,
            onSelect: onStartGuideTour,
          },
        ]
      : []),
  ];

  return (
    <>
      <button
        type="button"
        className={styles.button}
        onClick={onToggleThumbnails}
        data-guide-tour-id="pdf-thumbnails-toggle"
        aria-label="Abrir thumbnails"
        aria-pressed={isThumbnailOpen}
        title={isThumbnailOpen ? "Cerrar thumbnails" : "Abrir thumbnails"}
      >
        <span className={styles.icon} aria-hidden="true">
          <MenuOutlined />
        </span>
      </button>

      <button
        type="button"
        className={styles.button}
        onClick={onZoomOut}
        data-guide-tour-id="pdf-zoom-out"
        aria-label="Zoom out"
        disabled={isZoomDisabled}
        title={zoomDisabledTitle ?? "Zoom -"}
      >
        <span className={styles.icon} aria-hidden="true">
          <ZoomOutOutlined />
        </span>
      </button>
      <div className={styles.zoomLevel} data-guide-tour-id="pdf-zoom-level" aria-label="Zoom actual">
        {formatZoom(zoomLevel)}
      </div>
      <button
        type="button"
        className={styles.button}
        onClick={onZoomIn}
        data-guide-tour-id="pdf-zoom-in"
        aria-label="Zoom in"
        disabled={isZoomDisabled}
        title={zoomDisabledTitle ?? "Zoom +"}
      >
        <span className={styles.icon} aria-hidden="true">
          <ZoomInOutlined />
        </span>
      </button>
      <button
        type="button"
        className={`${styles.button} ${styles.resetOverflowAction}`}
        onClick={onResetZoom}
        data-guide-tour-id="pdf-reset-zoom"
        aria-label="Reset zoom"
        disabled={isZoomDisabled}
        title={zoomDisabledTitle ?? "Reset zoom (100%)"}
      >
        <span className={styles.icon} aria-hidden="true">
          <FileSyncOutlined />
        </span>
      </button>

      <span className={`${styles.divider} ${styles.collapsibleAction}`} aria-hidden="true" />

      <button
        type="button"
        className={`${styles.button} ${styles.collapsibleAction}`}
        onClick={onRotateLeft}
        data-guide-tour-id="pdf-rotate-left"
        aria-label="Rotar izquierda"
        title="Rotar izquierda (90°)"
      >
        <span className={styles.icon} aria-hidden="true">
          <RotateLeftOutlined />
        </span>
      </button>
      <button
        type="button"
        className={`${styles.button} ${styles.collapsibleAction}`}
        onClick={onRotateRight}
        data-guide-tour-id="pdf-rotate-right"
        aria-label="Rotar derecha"
        title="Rotar derecha (90°)"
      >
        <span className={styles.icon} aria-hidden="true">
          <RotateRightOutlined />
        </span>
      </button>

      <button
        type="button"
        className={styles.button}
        onClick={onToggleSignatureModal}
        data-guide-tour-id="pdf-signature"
        aria-label="Signature"
        aria-pressed={isSignatureModalOpen}
        title={
          isSignatureDisabled
            ? "Firmas deshabilitadas por política"
            : isSignatureModalOpen
              ? "Cerrar firmas"
              : "Abrir firmas"
        }
        disabled={isSignatureDisabled}
      >
        <span className={styles.icon} aria-hidden="true">
          <FormOutlined />
        </span>
      </button>

      <button
        type="button"
        className={styles.button}
        onClick={onSaveSignedPdf}
        data-guide-tour-id="pdf-lock-signature"
        hidden
        aria-hidden="true"
        aria-label={isSignatureLocked ? "Desbloquear firma" : "Bloquear firma"}
        title={
          isSavingSignedPdf
            ? "Validando\u2026"
            : isSignatureLocked
              ? "Seleccionar para desbloquear"
              : isSignatureLockToggleDisabled
                ? "Bloquear firma deshabilitado por política"
                : isSaveSignedPdfDisabled
                ? "Bloquear firma (requiere al menos 1 firma)"
                : "Bloquear firma"
        }
        disabled={isSaveSignedPdfDisabled || isSavingSignedPdf || isSignatureLockToggleDisabled}
      >
        <span
          className={`${styles.icon} ${isSignatureLocked ? styles.lockLocked : styles.lockUnlocked}`}
          aria-hidden="true"
        >
          {isSignatureLocked ? <LockOutlined /> : <UnlockOutlined />}
        </span>
      </button>

      {onSaveAnnotatedPages ? (
        <button
          type="button"
          className={`${styles.button} ${isSaveAnnotatedPagesReady ? styles.saveAnnotatedReady : ""} ${
            isSaveAnnotatedPagesBlocked ? styles.saveAnnotatedBlocked : ""
          }`}
          onClick={onSaveAnnotatedPages}
          data-guide-tour-id="pdf-save-annotated-pages"
          aria-label="Guardar paginas anotadas"
          title={
            isSavingAnnotatedPages
              ? `Guardando paginas anotadas${normalizedSaveProgress != null ? ` (${normalizedSaveProgress}%)` : ""}`
              : isSaveAnnotatedPagesDisabled
                ? "Guardar paginas anotadas deshabilitado"
                : "Guardar paginas anotadas"
          }
          disabled={isSaveAnnotatedPagesDisabled || isSavingAnnotatedPages}
          aria-valuenow={normalizedSaveProgress ?? undefined}
        >
          <span className={styles.icon} aria-hidden="true">
            <SaveOutlined />
          </span>
        </button>
      ) : null}

      <button
        type="button"
        className={styles.button}
        onClick={onDeleteSelectedSignature}
        data-guide-tour-id="pdf-delete-signature"
        aria-label="Eliminar firma seleccionada"
        title={
          isDeleteSelectedSignatureDisabled
            ? "Eliminar firma deshabilitado por política"
            : canDeleteSelectedSignature
            ? "Eliminar firma seleccionada"
            : "Selecciona una firma para eliminarla"
        }
        disabled={!canDeleteSelectedSignature || isDeleteSelectedSignatureDisabled}
      >
        <span className={styles.icon} aria-hidden="true">
          <DeleteOutlined />
        </span>
      </button>

      <span className={styles.spacer} aria-hidden="true" />

      <AppDropdown
        className={styles.overflowActions}
        ariaLabel="Mas acciones PDF"
        placement="bottomRight"
        items={overflowItems}
        trigger={
          <button
            type="button"
            className={styles.button}
            aria-label="Mas acciones PDF"
            title="Mas acciones"
          >
            <span className={styles.icon} aria-hidden="true">
              <MoreOutlined />
            </span>
          </button>
        }
      />

      <button
        type="button"
        className={`${styles.button} ${styles.collapsibleAction}`}
        onClick={onPrint}
        data-guide-tour-id="pdf-print"
        aria-label="Print"
        title={isPrintDisabled ? "Impresión deshabilitada por política" : "Print"}
        disabled={isPrintDisabled}
      >
        <span className={styles.icon} aria-hidden="true">
          <PrinterOutlined />
        </span>
      </button>
      <button
        type="button"
        className={`${styles.button} ${styles.collapsibleAction}`}
        onClick={onExport}
        data-guide-tour-id="pdf-export"
        aria-label="Export"
        title={isExportDisabled ? "Exportación deshabilitada por política" : "Export"}
        disabled={isExportDisabled}
      >
        <span className={styles.icon} aria-hidden="true">
          <DownloadOutlined />
        </span>
      </button>
      {showGuideTourButton ? (
        <button
          type="button"
          className={`${styles.button} ${styles.guideButton} ${styles.collapsibleAction}`}
          onClick={onStartGuideTour}
          data-guide-tour-id="pdf-help"
          aria-label="Guia interactiva"
          title="Ayuda - Guia interactiva"
        >
          <span className={styles.icon} aria-hidden="true">
            <QuestionCircleOutlined />
          </span>
          <span className={styles.guideButtonDot} aria-hidden="true">
            1
          </span>
        </button>
      ) : null}
    </>
  );
});
