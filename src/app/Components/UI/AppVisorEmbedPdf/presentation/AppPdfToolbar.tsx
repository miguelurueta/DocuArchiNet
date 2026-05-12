import { memo } from "react";
import {
  DownloadOutlined,
  FileSyncOutlined,
  FormOutlined,
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

  onPrint(): void;
  onExport(): void;
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
  onPrint,
  onExport,
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
        title={isSignatureModalOpen ? "Cerrar firmas" : "Abrir firmas"}
      >
        <span className={styles.icon} aria-hidden="true">
          <FormOutlined />
        </span>
      </button>

      <span className={styles.spacer} aria-hidden="true" />

      <button
        type="button"
        className={styles.button}
        onClick={onPrint}
        aria-label="Print"
        title="Print"
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
        title="Export"
      >
        <span className={styles.icon} aria-hidden="true">
          <DownloadOutlined />
        </span>
      </button>
    </>
  );
});

