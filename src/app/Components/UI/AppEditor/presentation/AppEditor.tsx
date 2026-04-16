import { useEffect, useId, useRef, useState } from "react";
import type { CSSProperties } from "react";
import type { AppEditorProps } from "../domain/editor.types";
import { useAppEditor } from "../application/useAppEditor";
import { usePageContext } from "../application/usePageContext";
import { usePaginationMetrics } from "../application/usePaginationMetrics";
import { TiptapEditorContent } from "../infrastructure/TiptapEditorContent";
import { AppEditorToolbar } from "./AppEditorToolbar";
import styles from "../AppEditor.module.css";

const DEFAULT_PAGE_MARGINS = {
  top: 96,
  right: 72,
  bottom: 96,
  left: 72,
} as const;
const DEFAULT_PAGE_GAP = 32;
const DEFAULT_ZOOM_LEVEL = 1;
const DEFAULT_MIN_ZOOM_LEVEL = 0.5;
const DEFAULT_MAX_ZOOM_LEVEL = 1.5;
const ZOOM_STEP = 0.25;

const PAGE_DIMENSIONS = {
  A4: {
    portrait: {
      width: 794,
      height: 1123,
    },
    landscape: {
      width: 1123,
      height: 794,
    },
  },
} as const;

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

function buildAriaLabel({
  "aria-label": ariaLabel,
  label,
  title,
}: Pick<AppEditorProps, "aria-label" | "label" | "title">) {
  if (ariaLabel) {
    return ariaLabel;
  }

  if (typeof label === "string" && label.trim()) {
    return label;
  }

  if (typeof title === "string" && title.trim()) {
    return title;
  }

  return "Editor enriquecido";
}

function resolvePaginationMetrics({
  minHeight,
  pageFormat,
  pageOrientation,
  pageMargins,
}: Pick<
  AppEditorProps,
  "minHeight" | "pageFormat" | "pageOrientation" | "pageMargins"
>) {
  const dimensions = PAGE_DIMENSIONS[pageFormat ?? "A4"][pageOrientation ?? "portrait"];
  const resolvedMargins = {
    top: pageMargins?.top ?? DEFAULT_PAGE_MARGINS.top,
    right: pageMargins?.right ?? DEFAULT_PAGE_MARGINS.right,
    bottom: pageMargins?.bottom ?? DEFAULT_PAGE_MARGINS.bottom,
    left: pageMargins?.left ?? DEFAULT_PAGE_MARGINS.left,
  };

  return {
    pageWidthValue: dimensions.width,
    pageHeightValue: dimensions.height,
    pageGapValue: DEFAULT_PAGE_GAP,
    pageHeight: `${dimensions.height}px`,
    pageWidth: `${dimensions.width}px`,
    pageGap: `${DEFAULT_PAGE_GAP}px`,
    minHeight: typeof minHeight === "number" ? `${minHeight}px` : minHeight,
    resolvedMargins,
    marginTop: `${resolvedMargins.top}px`,
    marginRight: `${resolvedMargins.right}px`,
    marginBottom: `${resolvedMargins.bottom}px`,
    marginLeft: `${resolvedMargins.left}px`,
  };
}

function clampZoomLevel(value: number, minZoomLevel: number, maxZoomLevel: number) {
  return Math.min(Math.max(value, minZoomLevel), maxZoomLevel);
}

function normalizeZoomLevel(value: number, minZoomLevel: number, maxZoomLevel: number) {
  const normalized = Number.isFinite(value) ? value : DEFAULT_ZOOM_LEVEL;
  return Number(clampZoomLevel(normalized, minZoomLevel, maxZoomLevel).toFixed(2));
}

export function AppEditor({
  value,
  defaultValue,
  onChange,
  placeholder,
  disabled = false,
  readOnly = false,
  label,
  error,
  helperText,
  className,
  title,
  description,
  headerActions,
  toolbarActions,
  surfaceClassName,
  minHeight = 280,
  themeMode,
  defaultThemeMode = "light",
  paginationMode = "none",
  pageFormat = "A4",
  pageOrientation = "portrait",
  pageMargins,
  zoomLevel,
  defaultZoomLevel = DEFAULT_ZOOM_LEVEL,
  minZoomLevel = DEFAULT_MIN_ZOOM_LEVEL,
  maxZoomLevel = DEFAULT_MAX_ZOOM_LEVEL,
  onZoomChange,
  "aria-label": ariaLabel,
}: AppEditorProps) {
  const fieldId = useId();
  const paginationContainerRef = useRef<HTMLDivElement>(null);
  const paginationCanvasRef = useRef<HTMLDivElement>(null);
  const labelId = label ? `${fieldId}-label` : undefined;
  const helperId = helperText ? `${fieldId}-helper` : undefined;
  const errorId = error ? `${fieldId}-error` : undefined;
  const describedBy = [errorId, helperId].filter(Boolean).join(" ") || undefined;
  const resolvedThemeMode = themeMode ?? defaultThemeMode;
  const isVisualPagination = paginationMode === "visual";
  const resolvedMinZoomLevel = Math.min(minZoomLevel, maxZoomLevel);
  const resolvedMaxZoomLevel = Math.max(minZoomLevel, maxZoomLevel);
  const isControlledZoom = typeof zoomLevel === "number";
  const [uncontrolledZoomLevel, setUncontrolledZoomLevel] = useState(() =>
    normalizeZoomLevel(defaultZoomLevel, resolvedMinZoomLevel, resolvedMaxZoomLevel),
  );
  const paginationMetrics = resolvePaginationMetrics({
    minHeight,
    pageFormat,
    pageOrientation,
    pageMargins,
  });
  const resolvedZoomLevel = normalizeZoomLevel(
    isControlledZoom ? zoomLevel ?? defaultZoomLevel : uncontrolledZoomLevel,
    resolvedMinZoomLevel,
    resolvedMaxZoomLevel,
  );
  const effectiveZoomLevel = isVisualPagination ? resolvedZoomLevel : DEFAULT_ZOOM_LEVEL;
  const { editor, isEditable, insertLocalImage } = useAppEditor({
    value,
    defaultValue,
    onChange,
    placeholder,
    disabled,
    readOnly,
  });
  const { totalPages, visualPageBoundaries } = usePaginationMetrics({
    editor,
    enabled: isVisualPagination,
    pageHeight: paginationMetrics.pageHeightValue,
    pageGap: paginationMetrics.pageGapValue,
    pageMargins: paginationMetrics.resolvedMargins,
    containerRef: paginationContainerRef,
    zoomLevel: effectiveZoomLevel,
  });
  const pageIndices = Array.from({ length: totalPages }, (_, index) => index + 1);
  const { currentPage } = usePageContext({
    editor,
    enabled: isVisualPagination,
    totalPages,
    pageBoundaries: visualPageBoundaries,
    canvasRef: paginationCanvasRef,
    zoomLevel: effectiveZoomLevel,
  });
  const sheetHeightValue =
    totalPages * paginationMetrics.pageHeightValue +
    Math.max(0, totalPages - 1) * paginationMetrics.pageGapValue;
  const zoomedSheetWidthValue = paginationMetrics.pageWidthValue * effectiveZoomLevel;
  const zoomedSheetHeightValue = sheetHeightValue * effectiveZoomLevel;
  const canDecreaseZoom = effectiveZoomLevel > resolvedMinZoomLevel;
  const canIncreaseZoom = effectiveZoomLevel < resolvedMaxZoomLevel;

  useEffect(() => {
    if (isControlledZoom) {
      return;
    }

    setUncontrolledZoomLevel((previousZoomLevel) =>
      normalizeZoomLevel(previousZoomLevel, resolvedMinZoomLevel, resolvedMaxZoomLevel),
    );
  }, [isControlledZoom, resolvedMaxZoomLevel, resolvedMinZoomLevel]);

  const handleZoomChange = (nextZoomLevel: number) => {
    const normalizedNextZoomLevel = normalizeZoomLevel(
      nextZoomLevel,
      resolvedMinZoomLevel,
      resolvedMaxZoomLevel,
    );

    if (!isControlledZoom) {
      setUncontrolledZoomLevel((previousZoomLevel) =>
        previousZoomLevel === normalizedNextZoomLevel ? previousZoomLevel : normalizedNextZoomLevel,
      );
    }

    if (normalizedNextZoomLevel !== resolvedZoomLevel) {
      onZoomChange?.(normalizedNextZoomLevel);
    }
  };

  return (
    <section
      className={joinClasses(styles.editor, className)}
      data-disabled={disabled}
      data-readonly={readOnly}
      data-error={Boolean(error)}
      data-theme={resolvedThemeMode}
      data-pagination-mode={paginationMode}
    >
      {title || description || headerActions ? (
        <header className={styles.header}>
          <div className={styles.headerContent}>
            {title ? <h2 className={styles.title}>{title}</h2> : null}
            {description ? <p className={styles.description}>{description}</p> : null}
          </div>
          {headerActions ? <div className={styles.headerActions}>{headerActions}</div> : null}
        </header>
      ) : null}

      {label ? (
        <label id={labelId} className={styles.label}>
          {label}
        </label>
      ) : null}

      <div className={styles.frame}>
        <AppEditorToolbar
          editor={editor}
          disabled={!isEditable}
          onInsertLocalImage={insertLocalImage}
          toolbarActions={toolbarActions}
          trailingContent={
            isVisualPagination ? (
              <div className={styles.zoomControls} role="group" aria-label="Control de zoom">
                <button
                  type="button"
                  className={styles.zoomButton}
                  aria-label="Reducir zoom"
                  disabled={!canDecreaseZoom}
                  onClick={() => handleZoomChange(resolvedZoomLevel - ZOOM_STEP)}
                >
                  -
                </button>
                <output className={styles.zoomValue} aria-live="polite">
                  {Math.round(effectiveZoomLevel * 100)}%
                </output>
                <button
                  type="button"
                  className={styles.zoomButton}
                  aria-label="Aumentar zoom"
                  disabled={!canIncreaseZoom}
                  onClick={() => handleZoomChange(resolvedZoomLevel + ZOOM_STEP)}
                >
                  +
                </button>
              </div>
            ) : null
          }
        />
        {isVisualPagination ? (
          <div
            className={styles.editorWrapper}
            ref={paginationContainerRef}
            style={
              {
                "--app-editor-min-height": paginationMetrics.minHeight,
                "--app-editor-page-height": paginationMetrics.pageHeight,
                "--app-editor-page-width": paginationMetrics.pageWidth,
                "--app-editor-page-gap": paginationMetrics.pageGap,
                "--app-editor-total-pages": String(totalPages),
                "--app-editor-page-margin-top": paginationMetrics.marginTop,
                "--app-editor-page-margin-right": paginationMetrics.marginRight,
                "--app-editor-page-margin-bottom": paginationMetrics.marginBottom,
                "--app-editor-page-margin-left": paginationMetrics.marginLeft,
                "--app-editor-zoom": String(effectiveZoomLevel),
                "--app-editor-sheet-height": `${sheetHeightValue}px`,
                "--app-editor-zoomed-sheet-width": `${zoomedSheetWidthValue}px`,
                "--app-editor-zoomed-sheet-height": `${zoomedSheetHeightValue}px`,
              } as CSSProperties
            }
          >
            <div className={styles.canvas} ref={paginationCanvasRef}>
              <div className={styles.zoomStage}>
                <div className={styles.sheet} data-pagination-sheet="true">
                  <div className={styles.pageStack} aria-hidden="true">
                    {pageIndices.map((pageNumber) => (
                      <div
                        key={pageNumber}
                        className={styles.pageShell}
                        data-pagination-page-shell={pageNumber}
                      />
                    ))}
                  </div>
                  <div className={styles.contentFlow} data-pagination-content-flow="true">
                    <div
                      className={joinClasses(
                        styles.surface,
                        styles.surfacePaged,
                        surfaceClassName,
                        Boolean(error) && styles.surfaceError,
                      )}
                    >
                      <TiptapEditorContent
                        editor={editor}
                        className={joinClasses(styles.editorContent, styles.editorContentPaged)}
                        aria-labelledby={labelId}
                        aria-label={buildAriaLabel({ "aria-label": ariaLabel, label, title })}
                        aria-describedby={describedBy}
                        aria-invalid={Boolean(error)}
                      />
                    </div>
                  </div>
                  {totalPages > 1 ? (
                    <div className={styles.pageCounter} aria-live="polite">
                      Pagina {currentPage} de {totalPages}
                    </div>
                  ) : null}
                </div>
              </div>
            </div>
          </div>
        ) : (
          <div
            className={joinClasses(
              styles.surface,
              surfaceClassName,
              Boolean(error) && styles.surfaceError,
            )}
            style={{ "--app-editor-min-height": paginationMetrics.minHeight } as CSSProperties}
          >
            <TiptapEditorContent
              editor={editor}
              className={styles.editorContent}
              aria-labelledby={labelId}
              aria-label={buildAriaLabel({ "aria-label": ariaLabel, label, title })}
              aria-describedby={describedBy}
              aria-invalid={Boolean(error)}
            />
          </div>
        )}
      </div>

      {error ? (
        <p id={errorId} className={styles.errorText}>
          {error}
        </p>
      ) : null}

      {helperText ? (
        <p id={helperId} className={styles.helperText}>
          {helperText}
        </p>
      ) : null}
    </section>
  );
}
