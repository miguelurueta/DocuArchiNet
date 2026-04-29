import { useEffect, useMemo, useRef } from "react";
import styles from "./VisorPdfThumbnails.module.css";

export type VisorPdfThumbnailsProps = {
  pageCount: number;
  activePage: number;
  onSelectPage: (page: number) => void;
  onRequestClose: () => void;
  variant: "overlay" | "rail";
  isOpen: boolean;
  labelledById: string;
  restoreFocusId?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export function VisorPdfThumbnails({
  pageCount,
  activePage,
  onSelectPage,
  onRequestClose,
  variant,
  isOpen,
  labelledById,
  restoreFocusId,
}: VisorPdfThumbnailsProps) {
  const listRef = useRef<HTMLDivElement>(null);

  const pages = useMemo(() => {
    const safeCount = Math.max(0, Math.floor(pageCount));
    const cap = 30;
    const visibleCount = Math.min(safeCount, cap);
    return Array.from({ length: visibleCount }, (_, idx) => idx + 1);
  }, [pageCount]);

  useEffect(() => {
    if (!isOpen) return;
    const firstButton = listRef.current?.querySelector<HTMLButtonElement>(
      "button[data-page]",
    );
    firstButton?.focus();
  }, [isOpen]);

  useEffect(() => {
    if (isOpen) return;
    if (!restoreFocusId) return;
    const node = document.getElementById(restoreFocusId);
    if (node instanceof HTMLElement) node.focus();
  }, [isOpen, restoreFocusId]);

  if (!isOpen) return null;

  const content = (
    <div
      id="app-visorpdf-thumbnails"
      role="region"
      aria-labelledby={labelledById}
      className={joinClasses(
        styles.panel,
        variant === "overlay" ? styles.overlay : styles.rail,
      )}
    >
      <div className={styles.header}>
        <span className={styles.title}>Thumbnails</span>
        <button
          type="button"
          className={styles.close}
          aria-label="Cerrar thumbnails"
          onClick={onRequestClose}
        >
          Cerrar
        </button>
      </div>
      <div className={styles.list} ref={listRef}>
        {pages.map((page) => (
          <button
            key={page}
            type="button"
            data-page={page}
            aria-label={`Ir a página ${page}`}
            aria-current={page === activePage ? "page" : undefined}
            className={joinClasses(
              styles.item,
              page === activePage && styles.active,
            )}
            onClick={() => {
              onSelectPage(page);
              onRequestClose();
            }}
          >
            {page}
          </button>
        ))}
      </div>
    </div>
  );

  if (variant !== "overlay") return content;

  return (
    <div className={styles.backdrop} role="presentation" onClick={onRequestClose}>
      <div role="presentation" onClick={(e) => e.stopPropagation()}>
        {content}
      </div>
    </div>
  );
}
