import { memo } from "react";
import {
  DownloadOutlined,
  DeleteOutlined,
  FileSyncOutlined,
  FormOutlined,
  LockOutlined,
  UnlockOutlined,
  MenuOutlined,
  PrinterOutlined,
  RotateLeftOutlined,
  RotateRightOutlined,
  ZoomInOutlined,
  ZoomOutOutlined,
} from "@ant-design/icons";

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
}: AppPdfToolbarProps) {
  const zoomDisabledTitle = isZoomDisabled
    ? "Zoom deshabilitado cuando hay rotación (estabilidad)"
    : undefined;

  return (
    <>
      <button
        type="button"
        className={styles.button}
        onClick={onToggleThumbnails}
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
        aria-label="Zoom out"
        disabled={isZoomDisabled}
        title={zoomDisabledTitle ?? "Zoom -"}
      >
        <span className={styles.icon} aria-hidden="true">
          <ZoomOutOutlined />
        </span>
      </button>
      <div className={styles.zoomLevel} aria-label="Zoom actual">
        {formatZoom(zoomLevel)}
      </div>
      <button
        type="button"
        className={styles.button}
        onClick={onZoomIn}
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
        className={styles.button}
        onClick={onResetZoom}
        aria-label="Reset zoom"
        disabled={isZoomDisabled}
        title={zoomDisabledTitle ?? "Reset zoom (100%)"}
      >
        <span className={styles.icon} aria-hidden="true">
          <FileSyncOutlined />
        </span>
      </button>

      <span className={styles.divider} aria-hidden="true" />

      <button
        type="button"
        className={styles.button}
        onClick={onRotateLeft}
        aria-label="Rotar izquierda"
        title="Rotar izquierda (90°)"
      >
        <span className={styles.icon} aria-hidden="true">
          <RotateLeftOutlined />
        </span>
      </button>
      <button
        type="button"
        className={styles.button}
        onClick={onRotateRight}
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

      <button
        type="button"
        className={styles.button}
        onClick={onDeleteSelectedSignature}
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

      <button
        type="button"
        className={styles.button}
        onClick={onPrint}
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
        className={styles.button}
        onClick={onExport}
        aria-label="Export"
        title={isExportDisabled ? "Exportación deshabilitada por política" : "Export"}
        disabled={isExportDisabled}
      >
        <span className={styles.icon} aria-hidden="true">
          <DownloadOutlined />
        </span>
      </button>
    </>
  );
});
