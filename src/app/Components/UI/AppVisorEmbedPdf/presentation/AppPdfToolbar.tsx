import { memo } from "react";

import styles from "./AppPdfToolbar.module.css";

export interface AppPdfToolbarProps {
  zoomLevel: number;
  onZoomIn(): void;
  onZoomOut(): void;
  onResetZoom(): void;
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
}: AppPdfToolbarProps) {
  return (
    <>
      <button type="button" className={styles.button} onClick={onZoomOut} aria-label="Zoom out">
        -
      </button>
      <div className={styles.zoomLevel} aria-label="Zoom actual">
        {formatZoom(zoomLevel)}
      </div>
      <button type="button" className={styles.button} onClick={onZoomIn} aria-label="Zoom in">
        +
      </button>
      <button type="button" className={styles.button} onClick={onResetZoom} aria-label="Reset zoom">
        Reset
      </button>
    </>
  );
});
