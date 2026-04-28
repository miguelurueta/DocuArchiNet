import { useCallback, useEffect, useMemo, useState } from "react";
import type { CSSProperties } from "react";
import { AppEditor } from "../AppEditor";
import { AppEditorPdfPageBreakAction } from "./AppEditorPdfPageBreakAction";
import type {
  AppEditorPdfPageContext,
  AppEditorPdfPageMargins,
  AppEditorPdfProps,
  AppEditorPdfVisualGuides,
  AppEditorPdfVisualMetrics,
} from "./domain/editor-pdf.types";
import styles from "./AppEditorPdf.module.css";

const DEFAULT_PAGE_MARGINS = {
  top: 96,
  right: 72,
  bottom: 96,
  left: 72,
} as const;
const DEFAULT_GUIDES: Required<AppEditorPdfVisualGuides> = {
  enabled: true,
  showPageBoundaries: true,
  showReadingFrame: true,
  readingFrameInset: 16,
};
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

const joinClassNames = (...values: Array<string | undefined>) =>
  values.filter(Boolean).join(" ");

function resolveAriaLabel({
  ariaLabel,
  label,
}: {
  ariaLabel?: string;
  label?: AppEditorPdfProps["label"];
}) {
  if (ariaLabel?.trim()) {
    return ariaLabel;
  }

  if (typeof label === "string" && label.trim()) {
    return label;
  }

  return "Editor PDF";
}

function clampPage(page: number, totalPages: number) {
  if (!Number.isFinite(page)) {
    return 1;
  }

  const normalized = Math.floor(page);
  return Math.min(Math.max(normalized, 1), totalPages);
}

function normalizeGuidesConfig(visualGuides?: AppEditorPdfVisualGuides) {
  return {
    enabled: visualGuides?.enabled ?? DEFAULT_GUIDES.enabled,
    showPageBoundaries:
      visualGuides?.showPageBoundaries ?? DEFAULT_GUIDES.showPageBoundaries,
    showReadingFrame:
      visualGuides?.showReadingFrame ?? DEFAULT_GUIDES.showReadingFrame,
    readingFrameInset: Math.max(
      0,
      visualGuides?.readingFrameInset ?? DEFAULT_GUIDES.readingFrameInset,
    ),
  };
}

export function AppEditorPdf(props: AppEditorPdfProps) {
  const {
    className,
    label,
    paginationMode = "visual",
    pageFormat = "A4",
    pageOrientation = "portrait",
    pageMargins,
    documentSource = "default",
    totalPages = 1,
    activePage,
    defaultActivePage = 1,
    onActivePageChange,
    onPageContextChange,
    showPageBreakAction = false,
    visualGuides,
    onMetricsChange,
    zoomLevel,
    defaultZoomLevel = 1,
    toolbarActions,
    "aria-label": ariaLabel,
    ...rest
  } = props;

  const resolvedPageMargins = useMemo(
    () => ({
      ...DEFAULT_PAGE_MARGINS,
      ...pageMargins,
    }),
    [
      pageMargins?.bottom,
      pageMargins?.left,
      pageMargins?.right,
      pageMargins?.top,
    ],
  );
  const resolvedAriaLabel = resolveAriaLabel({ ariaLabel, label });
  const fallbackTotalPages = Math.max(1, Math.floor(totalPages));
  const [resolvedPageContext, setResolvedPageContext] =
    useState<AppEditorPdfPageContext | null>(null);
  const [insertPageBreakCommand, setInsertPageBreakCommand] = useState<
    (() => boolean) | null
  >(null);
  const resolvedTotalPages =
    paginationMode === "visual" && resolvedPageContext
      ? Math.max(1, Math.floor(resolvedPageContext.totalPages))
      : fallbackTotalPages;
  const isControlledPage = typeof activePage === "number";
  const [uncontrolledActivePage, setUncontrolledActivePage] = useState(() =>
    clampPage(defaultActivePage, resolvedTotalPages),
  );
  const baseActivePage = clampPage(
    isControlledPage ? activePage ?? 1 : uncontrolledActivePage,
    resolvedTotalPages,
  );
  const resolvedActivePage =
    !isControlledPage && paginationMode === "visual" && resolvedPageContext
      ? clampPage(resolvedPageContext.currentPage, resolvedTotalPages)
      : baseActivePage;
  const resolvedZoomLevel = zoomLevel ?? defaultZoomLevel;
  const guidesConfig = useMemo(
    () => normalizeGuidesConfig(visualGuides),
    [visualGuides],
  );
  const pageDimensions = PAGE_DIMENSIONS[pageFormat][pageOrientation];

  useEffect(() => {
    if (!isControlledPage) {
      setUncontrolledActivePage((previousPage) =>
        clampPage(previousPage, resolvedTotalPages),
      );
    }
  }, [isControlledPage, resolvedTotalPages]);
  useEffect(() => {
    if (paginationMode !== "visual") {
      setResolvedPageContext(null);
    }
  }, [paginationMode]);
  useEffect(() => {
    if (!showPageBreakAction) {
      setInsertPageBreakCommand(null);
    }
  }, [showPageBreakAction]);

  const metrics = useMemo<AppEditorPdfVisualMetrics>(() => {
    const contentWidth = Math.max(
      0,
      pageDimensions.width - resolvedPageMargins.left - resolvedPageMargins.right,
    );
    const contentHeight = Math.max(
      0,
      pageDimensions.height - resolvedPageMargins.top - resolvedPageMargins.bottom,
    );

    return {
      documentSource,
      currentPage: resolvedActivePage,
      totalPages: resolvedTotalPages,
      zoomLevel: resolvedZoomLevel,
      pageWidth: pageDimensions.width,
      pageHeight: pageDimensions.height,
      contentWidth,
      contentHeight,
      pageMargins: resolvedPageMargins as AppEditorPdfPageMargins,
    };
  }, [
    documentSource,
    pageDimensions.height,
    pageDimensions.width,
    resolvedActivePage,
    resolvedPageMargins,
    resolvedTotalPages,
    resolvedZoomLevel,
  ]);

  useEffect(() => {
    onMetricsChange?.(metrics);
  }, [metrics, onMetricsChange]);
  const handlePageContextChange = useCallback(
    (context: {
      currentPage: number;
      totalPages: number;
      source: "cursor" | "scroll";
    }) => {
      const nextContext: AppEditorPdfPageContext = {
        currentPage: clampPage(context.currentPage, Math.max(1, context.totalPages)),
        totalPages: Math.max(1, Math.floor(context.totalPages)),
        source: context.source,
      };

      setResolvedPageContext((previousContext) =>
        previousContext &&
        previousContext.currentPage === nextContext.currentPage &&
        previousContext.totalPages === nextContext.totalPages &&
        previousContext.source === nextContext.source
          ? previousContext
          : nextContext,
      );
      onPageContextChange?.(nextContext);
    },
    [onPageContextChange],
  );
  const handlePageBreakCommandReady = useCallback(
    (command: (() => boolean) | null) => {
      setInsertPageBreakCommand(command ? () => command : null);
    },
    [],
  );
  const composedToolbarActions = useMemo(() => {
    if (!showPageBreakAction) {
      return toolbarActions;
    }

    const pageBreakAction = (
      <AppEditorPdfPageBreakAction
        disabled={!insertPageBreakCommand}
        onInsertPageBreak={() => insertPageBreakCommand?.() ?? false}
      />
    );

    if (!toolbarActions) {
      return pageBreakAction;
    }

    return (
      <>
        {toolbarActions}
        {pageBreakAction}
      </>
    );
  }, [insertPageBreakCommand, showPageBreakAction, toolbarActions]);

  const handleNavigateToPage = useCallback(
    (nextPage: number) => {
      const normalizedPage = clampPage(nextPage, resolvedTotalPages);

      if (!isControlledPage) {
        setUncontrolledActivePage(normalizedPage);
      }

      if (normalizedPage !== resolvedActivePage) {
        onActivePageChange?.(normalizedPage);
      }
    },
    [isControlledPage, onActivePageChange, resolvedActivePage, resolvedTotalPages],
  );

  return (
    <div
      className={styles.shell}
      data-document-source={documentSource}
      data-page={resolvedActivePage}
      data-total-pages={resolvedTotalPages}
      data-pagination-mode={paginationMode}
      data-zoom-level={resolvedZoomLevel}
    >
      <AppEditor
        {...rest}
        label={label}
        paginationMode={paginationMode}
        pageFormat={pageFormat}
        pageOrientation={pageOrientation}
        pageMargins={resolvedPageMargins}
        zoomLevel={resolvedZoomLevel}
        defaultZoomLevel={defaultZoomLevel}
        onPageContextChange={handlePageContextChange}
        onPageBreakCommandReady={handlePageBreakCommandReady}
        aria-label={resolvedAriaLabel}
        className={joinClassNames(styles.root, className)}
        toolbarActions={composedToolbarActions}
      />
      {paginationMode === "visual" && guidesConfig.enabled ? (
        <div className={styles.guides} aria-hidden="true">
          {guidesConfig.showPageBoundaries ? (
            <div
              className={styles.pageBoundary}
              data-testid="app-editor-pdf-page-boundary-guide"
            />
          ) : null}
          {guidesConfig.showReadingFrame ? (
            <div
              className={styles.readingFrame}
              data-testid="app-editor-pdf-reading-frame-guide"
              style={
                {
                  "--app-editor-pdf-guide-inset": `${guidesConfig.readingFrameInset}px`,
                } as CSSProperties
              }
            />
          ) : null}
        </div>
      ) : null}
      {paginationMode === "visual" && resolvedTotalPages > 1 ? (
        <div className={styles.pageNavigation} role="group" aria-label="Navegacion de pagina PDF">
          <button
            type="button"
            className={styles.pageButton}
            onClick={() => handleNavigateToPage(resolvedActivePage - 1)}
            disabled={resolvedActivePage <= 1}
          >
            Pagina anterior
          </button>
          <span className={styles.pageIndicator}>
            Pagina {resolvedActivePage} de {resolvedTotalPages}
          </span>
          <button
            type="button"
            className={styles.pageButton}
            onClick={() => handleNavigateToPage(resolvedActivePage + 1)}
            disabled={resolvedActivePage >= resolvedTotalPages}
          >
            Pagina siguiente
          </button>
        </div>
      ) : null}
    </div>
  );
}
