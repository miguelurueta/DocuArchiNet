import { useId, useRef, useState } from "react";
import type { CSSProperties } from "react";
import type { AppEditorProps, AppEditorThemeMode } from "../domain/editor.types";
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
    pageHeightValue: dimensions.height,
    pageHeight: `${dimensions.height}px`,
    pageWidth: `${dimensions.width}px`,
    minHeight: typeof minHeight === "number" ? `${minHeight}px` : minHeight,
    resolvedMargins,
    marginTop: `${resolvedMargins.top}px`,
    marginRight: `${resolvedMargins.right}px`,
    marginBottom: `${resolvedMargins.bottom}px`,
    marginLeft: `${resolvedMargins.left}px`,
  };
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
  surfaceClassName,
  minHeight = 280,
  showThemeToggle = true,
  themeMode,
  defaultThemeMode = "light",
  onThemeModeChange,
  paginationMode = "none",
  pageFormat = "A4",
  pageOrientation = "portrait",
  pageMargins,
  "aria-label": ariaLabel,
}: AppEditorProps) {
  const fieldId = useId();
  const paginationContainerRef = useRef<HTMLDivElement>(null);
  const paginationCanvasRef = useRef<HTMLDivElement>(null);
  const [internalThemeMode, setInternalThemeMode] = useState<AppEditorThemeMode>(defaultThemeMode);
  const labelId = label ? `${fieldId}-label` : undefined;
  const helperId = helperText ? `${fieldId}-helper` : undefined;
  const errorId = error ? `${fieldId}-error` : undefined;
  const describedBy = [errorId, helperId].filter(Boolean).join(" ") || undefined;
  const resolvedThemeMode = themeMode ?? internalThemeMode;
  const isVisualPagination = paginationMode === "visual";
  const paginationMetrics = resolvePaginationMetrics({
    minHeight,
    pageFormat,
    pageOrientation,
    pageMargins,
  });
  const { editor, isEditable } = useAppEditor({
    value,
    defaultValue,
    onChange,
    placeholder,
    disabled,
    readOnly,
  });
  const { guideOffsets, totalPages, pageBoundaries } = usePaginationMetrics({
    editor,
    enabled: isVisualPagination,
    pageHeight: paginationMetrics.pageHeightValue,
    pageMargins: paginationMetrics.resolvedMargins,
    containerRef: paginationContainerRef,
  });
  const { currentPage } = usePageContext({
    editor,
    enabled: isVisualPagination,
    totalPages,
    pageBoundaries,
    canvasRef: paginationCanvasRef,
  });

  const handleThemeModeChange = (nextThemeMode: AppEditorThemeMode) => {
    if (themeMode === undefined) {
      setInternalThemeMode(nextThemeMode);
    }

    onThemeModeChange?.(nextThemeMode);
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
          showThemeToggle={showThemeToggle}
          themeMode={resolvedThemeMode}
          onThemeModeChange={handleThemeModeChange}
        />
        {isVisualPagination ? (
          <div
            className={styles.editorWrapper}
            ref={paginationContainerRef}
            style={{
              "--app-editor-min-height": paginationMetrics.minHeight,
              "--app-editor-page-height": paginationMetrics.pageHeight,
              "--app-editor-page-width": paginationMetrics.pageWidth,
              "--app-editor-page-margin-top": paginationMetrics.marginTop,
              "--app-editor-page-margin-right": paginationMetrics.marginRight,
              "--app-editor-page-margin-bottom": paginationMetrics.marginBottom,
              "--app-editor-page-margin-left": paginationMetrics.marginLeft,
            } as CSSProperties}
          >
            <div className={styles.canvas} ref={paginationCanvasRef}>
              <div className={styles.sheet} data-pagination-sheet="true">
                {guideOffsets.length > 0 ? (
                  <div className={styles.pageGuides} aria-hidden="true">
                    {guideOffsets.map((offset, index) => (
                      <div
                        key={`page-guide-${offset}-${index}`}
                        className={styles.pageGuide}
                        style={{ top: `${offset}px` }}
                      />
                    ))}
                  </div>
                ) : null}
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
                <div className={styles.pageCounter} aria-live="polite">
                  Pagina {currentPage} de {totalPages}
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
