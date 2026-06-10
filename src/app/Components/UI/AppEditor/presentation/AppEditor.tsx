import { useCallback, useEffect, useId, useLayoutEffect, useMemo, useRef, useState } from "react";
import type { ChangeEvent, CSSProperties, KeyboardEvent, ReactNode } from "react";
import { createPortal } from "react-dom";
import type { InputRef } from "antd";
import { FullscreenOutlined } from "@ant-design/icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faChevronDown,
  faChevronUp,
  faMagnifyingGlass,
  faXmark,
} from "@fortawesome/free-solid-svg-icons";
import type { AppEditorProps } from "../domain/editor.types";
import { DEFAULT_PAGE_MARGINS, PAGE_DIMENSIONS } from "../domain/page.constants";
import { useAppEditor } from "../application/useAppEditor";
import { useDocumentSearch } from "../application/useDocumentSearch";
import { usePaginationMetrics } from "../application/usePaginationMetrics";
import { useVisualPageNavigation } from "../application/useVisualPageNavigation";
import { AppCollapseRail } from "../../AppCollapseRail";
import { AppButton } from "../../AppButton";
import { AppInputSearch } from "../../AppInputSearch";
import { TiptapEditorContent } from "../infrastructure/TiptapEditorContent";
import { AppEditorNavigationPanel } from "./AppEditorNavigationPanel";
import { AppEditorPreview, type AppEditorPreviewHandle } from "./AppEditorPreview";
import { AppEditorToolbar } from "./AppEditorToolbar";
import styles from "../AppEditor.module.css";

const DEFAULT_PAGE_GAP = 32;
const PAGE_INDICATOR_TOP_OFFSET = -36;
const DEFAULT_ZOOM_LEVEL = 1;
const DEFAULT_MIN_ZOOM_LEVEL = 0.5;
const DEFAULT_MAX_ZOOM_LEVEL = 2;
const ZOOM_STEP = 0.25;
const EDITOR_WHEEL_SCROLL_SPEED = 2;
const MIN_SCROLL_STEP = 0.35;

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const documentCountFormatter = new Intl.NumberFormat("es-CO");

type DocumentTextStats = {
  words: number;
  characters: number;
};

function normalizeWheelDelta(delta: number, deltaMode: number) {
  if (delta === 0) {
    return 0;
  }

  const deltaMultiplier = deltaMode === 1 ? 16 : deltaMode === 2 ? 100 : 1;
  const normalizedDelta = delta * deltaMultiplier * EDITOR_WHEEL_SCROLL_SPEED;

  return Math.abs(normalizedDelta) < MIN_SCROLL_STEP
    ? (normalizedDelta > 0 ? MIN_SCROLL_STEP : -MIN_SCROLL_STEP)
    : normalizedDelta;
}

function calculateDocumentTextStats(editor: ReturnType<typeof useAppEditor>["editor"]) {
  if (!editor?.state?.doc) {
    return { words: 0, characters: 0 };
  }

  const text = editor.state.doc.textBetween(0, editor.state.doc.content.size, " ", " ");
  const normalizedText = text.replace(/\s+/g, " ").trim();
  const words = normalizedText.match(/[\p{L}\p{N}]+(?:['-][\p{L}\p{N}]+)*/gu)?.length ?? 0;

  return {
    words,
    characters: normalizedText.length,
  };
}

function formatDocumentCount(value: number) {
  return documentCountFormatter.format(Math.max(0, value));
}

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
  const pageContentWidth = Math.max(
    1,
    dimensions.width - resolvedMargins.left - resolvedMargins.right,
  );

  return {
    pageWidthValue: dimensions.width,
    pageHeightValue: dimensions.height,
    pageGapValue: DEFAULT_PAGE_GAP,
    pageGapCss: `${DEFAULT_PAGE_GAP}px`,
    pageHeight: `${dimensions.height}px`,
    pageWidth: `${dimensions.width}px`,
    pageContentWidthValue: pageContentWidth,
    pageContentWidth: `${pageContentWidth}px`,
    pageGap: DEFAULT_PAGE_GAP,
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
  paginationMode = "visual",
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
  const resolvedPageMarginsTop = pageMargins?.top ?? DEFAULT_PAGE_MARGINS.top;
  const resolvedPageMarginsRight = pageMargins?.right ?? DEFAULT_PAGE_MARGINS.right;
  const resolvedPageMarginsBottom = pageMargins?.bottom ?? DEFAULT_PAGE_MARGINS.bottom;
  const resolvedPageMarginsLeft = pageMargins?.left ?? DEFAULT_PAGE_MARGINS.left;
  const fieldId = useId();
  const editorRootRef = useRef<HTMLElement>(null);
  const paginationContainerRef = useRef<HTMLDivElement>(null);
  const paginationCanvasRef = useRef<HTMLDivElement>(null);
  const previewRef = useRef<AppEditorPreviewHandle>(null);
  const previewStageLocalSlotRef = useRef<HTMLDivElement>(null);
  const previewStagePortalHostRef = useRef<HTMLDivElement | null>(null);
  const searchInputRef = useRef<InputRef>(null);
  const pendingSelectionRestoreRef = useRef<{
    from: number;
    to: number;
  } | null>(null);
  const labelId = label ? `${fieldId}-label` : undefined;
  const helperId = helperText ? `${fieldId}-helper` : undefined;
  const errorId = error ? `${fieldId}-error` : undefined;
  const describedBy = useMemo(
    () => [errorId, helperId].filter(Boolean).join(" ") || undefined,
    [errorId, helperId],
  );
  const resolvedThemeMode = themeMode ?? defaultThemeMode;
  const isVisualPagination = paginationMode === "visual";
  const resolvedMinZoomLevel = Math.min(minZoomLevel, maxZoomLevel);
  const resolvedMaxZoomLevel = Math.max(minZoomLevel, maxZoomLevel);
  const isControlledZoom = typeof zoomLevel === "number";
  const [uncontrolledZoomLevel, setUncontrolledZoomLevel] = useState(() =>
    normalizeZoomLevel(defaultZoomLevel, resolvedMinZoomLevel, resolvedMaxZoomLevel),
  );
  const [pageInputValue, setPageInputValue] = useState("1");
  const [isSearchOpen, setIsSearchOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState("");
  const [isPreviewMode, setIsPreviewMode] = useState(false);
  const [isPresentationMode, setIsPresentationMode] = useState(false);
  const [previewHtml, setPreviewHtml] = useState(() => value ?? defaultValue ?? "");
  const [previewPageCount, setPreviewPageCount] = useState(1);
  const [previewCurrentPage, setPreviewCurrentPage] = useState(1);
  const [editSurfaceVersion, setEditSurfaceVersion] = useState(0);
  const [structureCollapsed, setStructureCollapsed] = useState(false);
  const [thumbnailsCollapsed, setThumbnailsCollapsed] = useState(false);
  const [documentTextStats, setDocumentTextStats] = useState<DocumentTextStats>({
    words: 0,
    characters: 0,
  });

  if (previewStagePortalHostRef.current === null && typeof document !== "undefined") {
    previewStagePortalHostRef.current = document.createElement("div");
    previewStagePortalHostRef.current.setAttribute("data-app-editor-preview-stage-host", "true");
  }

  const isPresentationPortalActive = isPreviewMode && isPresentationMode;
  const previewStagePortalHost = previewStagePortalHostRef.current;

  const paginationMetrics = useMemo(
    () =>
      resolvePaginationMetrics({
        minHeight,
        pageFormat,
        pageOrientation,
        pageMargins: {
          top: resolvedPageMarginsTop,
          right: resolvedPageMarginsRight,
          bottom: resolvedPageMarginsBottom,
          left: resolvedPageMarginsLeft,
        },
      }),
    [
      minHeight,
      pageFormat,
      pageOrientation,
      resolvedPageMarginsBottom,
      resolvedPageMarginsLeft,
      resolvedPageMarginsRight,
      resolvedPageMarginsTop,
    ],
  );
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
    paginationMode,
    pageHeight: paginationMetrics.pageHeightValue,
    pageGap: paginationMetrics.pageGapValue,
    pageMargins: paginationMetrics.resolvedMargins,
    zoomLevel: effectiveZoomLevel,
  });
  const {
    totalPages: measuredTotalPages,
    visualContentHeight,
    pages,
  } = usePaginationMetrics({
    enabled: isVisualPagination,
    pageHeight: paginationMetrics.pageHeightValue,
    pageGap: paginationMetrics.pageGapValue,
    pageMargins: paginationMetrics.resolvedMargins,
    containerRef: paginationContainerRef,
    zoomLevel: effectiveZoomLevel,
  });
  const {
    currentPage,
    totalPages,
    relativePageProgress,
    documentProgress,
    goToPage,
    goToPreviousPage,
    goToNextPage,
  } = useVisualPageNavigation({
    enabled: isVisualPagination,
    totalPages: measuredTotalPages,
    pages,
    canvasRef: paginationCanvasRef,
    editor,
    zoomLevel: effectiveZoomLevel,
  });
  const {
    activeIndex: activeSearchIndex,
    totalMatches: searchMatchCount,
    goToNext: goToNextSearchMatch,
    goToPrevious: goToPreviousSearchMatch,
    clearHighlights: clearSearchHighlights,
  } = useDocumentSearch({
    editor,
    canvasRef: paginationCanvasRef,
    query: searchQuery,
    enabled: isSearchOpen && !isPreviewMode,
  });
  const visualTotalPages = isPreviewMode ? previewPageCount : totalPages;
  const sheetHeightValue =
    visualTotalPages * paginationMetrics.pageHeightValue +
    Math.max(0, visualTotalPages - 1) * paginationMetrics.pageGapValue;
  const zoomedSheetWidthValue = paginationMetrics.pageWidthValue * effectiveZoomLevel;
  const zoomedSheetHeightValue = sheetHeightValue * effectiveZoomLevel;
  const canDecreaseZoom = effectiveZoomLevel > resolvedMinZoomLevel;
  const canIncreaseZoom = effectiveZoomLevel < resolvedMaxZoomLevel;
  const resolvedAriaLabel = useMemo(
    () => buildAriaLabel({ "aria-label": ariaLabel, label, title }),
    [ariaLabel, label, title],
  );
  const pageProgressLabel = useMemo(
    () =>
      `Pagina ${currentPage} de ${totalPages}. ${Math.round(
        relativePageProgress * 100,
      )}% de la pagina. ${Math.round(documentProgress * 100)}% del documento. ${formatDocumentCount(
        documentTextStats.words,
      )} palabras. ${formatDocumentCount(documentTextStats.characters)} caracteres.`,
    [currentPage, documentProgress, documentTextStats, relativePageProgress, totalPages],
  );
  const displayedTotalPages = isPreviewMode ? previewPageCount : totalPages;
  const displayedPage = isPreviewMode
    ? Math.min(Math.max(previewCurrentPage, 1), Math.max(1, previewPageCount))
    : currentPage;
  const displayedPageLabel = `Pagina ${displayedPage} de ${displayedTotalPages}`;
  const searchResultLabel = searchQuery.trim()
    ? `${searchMatchCount} coincidencia${searchMatchCount === 1 ? "" : "s"}`
    : "Buscar en el documento";

  const openSearch = useCallback(() => {
    if (isPreviewMode) {
      return;
    }

    setIsSearchOpen(true);
    window.requestAnimationFrame(() => {
      searchInputRef.current?.focus?.();
    });
  }, [isPreviewMode]);

  const closeSearch = useCallback(() => {
    setIsSearchOpen(false);
    setSearchQuery("");
    clearSearchHighlights();
  }, [clearSearchHighlights]);

  const handleSearchKeyDown = useCallback(
    (event: KeyboardEvent<HTMLInputElement>) => {
      if (event.key === "Escape") {
        event.preventDefault();
        closeSearch();
        return;
      }

      if (event.key === "Enter") {
        event.preventDefault();
        if (event.shiftKey) {
          goToPreviousSearchMatch();
        } else {
          goToNextSearchMatch();
        }
      }
    },
    [closeSearch, goToNextSearchMatch, goToPreviousSearchMatch],
  );

  useEffect(() => {
    setPageInputValue(String(currentPage));
  }, [currentPage]);

  useEffect(() => {
    const handleKeyDown = (event: globalThis.KeyboardEvent) => {
      const root = editorRootRef.current;
      const target = event.target;
      const isInsideEditor =
        root && target instanceof Node ? root.contains(target) : false;

      if (!isInsideEditor || event.key.toLocaleLowerCase() !== "f") {
        return;
      }

      if (!(event.ctrlKey || event.metaKey) || event.altKey) {
        return;
      }

      event.preventDefault();
      openSearch();
    };

    document.addEventListener("keydown", handleKeyDown, { capture: true });

    return () => {
      document.removeEventListener("keydown", handleKeyDown, { capture: true });
    };
  }, [openSearch]);

  useEffect(() => {
    if (!editor) {
      setDocumentTextStats({ words: 0, characters: 0 });
      return undefined;
    }

    const syncEditorDerivedState = () => {
      setPreviewHtml(editor.getHTML());
      setDocumentTextStats(calculateDocumentTextStats(editor));
    };

    syncEditorDerivedState();
    editor.on("transaction", syncEditorDerivedState);

    return () => {
      editor.off("transaction", syncEditorDerivedState);
    };
  }, [editor]);

  const handleZoomChange = useCallback(
    (nextZoomLevel: number) => {
      const normalizedNextZoomLevel = normalizeZoomLevel(
        nextZoomLevel,
        resolvedMinZoomLevel,
        resolvedMaxZoomLevel,
      );

      if (!isControlledZoom) {
        setUncontrolledZoomLevel((previousZoomLevel) =>
          previousZoomLevel === normalizedNextZoomLevel
            ? previousZoomLevel
            : normalizedNextZoomLevel,
        );
      }

      if (normalizedNextZoomLevel !== resolvedZoomLevel) {
        onZoomChange?.(normalizedNextZoomLevel);
      }
    },
    [
      isControlledZoom,
      onZoomChange,
      resolvedMaxZoomLevel,
      resolvedMinZoomLevel,
      resolvedZoomLevel,
    ],
  );
  const handlePageInputChange = useCallback(
    (event: ChangeEvent<HTMLInputElement>) => {
      setPageInputValue(event.currentTarget.value);
    },
    [],
  );
  const commitPageInput = useCallback(() => {
    const nextPage = Number(pageInputValue);

    if (!Number.isFinite(nextPage)) {
      setPageInputValue(String(currentPage));
      return;
    }

    const safePage = Math.min(Math.max(Math.floor(nextPage), 1), totalPages);
    setPageInputValue(String(safePage));
    goToPage(safePage);
  }, [currentPage, goToPage, pageInputValue, totalPages]);
  const handlePageInputKeyDown = useCallback(
    (event: KeyboardEvent<HTMLInputElement>) => {
      if (event.key !== "Enter") {
        return;
      }

      event.preventDefault();
      commitPageInput();
    },
    [commitPageInput],
  );
  const handlePreviewToggle = useCallback(() => {
    if (editor) {
      setPreviewHtml(editor.getHTML());

      if (!isPreviewMode) {
        pendingSelectionRestoreRef.current = {
          from: editor.state.selection.from,
          to: editor.state.selection.to,
        };
      }
    }

    if (isPreviewMode) {
      setEditSurfaceVersion((previousValue) => previousValue + 1);
      window.requestAnimationFrame(() => {
        window.dispatchEvent(new Event("resize"));
      });
    }

    if (isPreviewMode) {
      setIsPresentationMode(false);
    }

    setIsPreviewMode((previousValue) => !previousValue);
  }, [editor, isPreviewMode]);

  const openPresentationMode = useCallback(() => {
    if (!isPreviewMode) {
      return;
    }

    setIsPresentationMode(true);
  }, [isPreviewMode]);

  const closePresentationMode = useCallback(() => {
    setIsPresentationMode(false);
  }, []);

  useEffect(() => {
    if (!isPreviewMode && isPresentationMode) {
      setIsPresentationMode(false);
    }
  }, [isPreviewMode, isPresentationMode]);

  useEffect(() => {
    if (!isPresentationMode) {
      return undefined;
    }

    const handleKeyDown = (event: globalThis.KeyboardEvent) => {
      if (event.key !== "Escape") {
        return;
      }

      event.preventDefault();
      closePresentationMode();
    };

    document.addEventListener("keydown", handleKeyDown);

    return () => {
      document.removeEventListener("keydown", handleKeyDown);
    };
  }, [closePresentationMode, isPresentationMode]);

  useEffect(() => {
    if (isPreviewMode || !editor || !pendingSelectionRestoreRef.current) {
      return;
    }

    const selection = pendingSelectionRestoreRef.current;
    pendingSelectionRestoreRef.current = null;
    editor.commands.setTextSelection(selection);
    editor.commands.focus();
  }, [editor, isPreviewMode]);

  const handlePreviousPage = useCallback(() => {
    if (isPreviewMode) {
      previewRef.current?.goToPage(displayedPage - 1);
      return;
    }

    goToPreviousPage();
  }, [displayedPage, goToPreviousPage, isPreviewMode]);
  const handleNextPage = useCallback(() => {
    if (isPreviewMode) {
      previewRef.current?.goToPage(displayedPage + 1);
      return;
    }

    goToNextPage();
  }, [displayedPage, goToNextPage, isPreviewMode]);

  const zoomTrailingContent = useMemo(() => {
    if (!isVisualPagination) {
      return null;
    }

    return (
      <div className={styles.toolbarMeta}>
        <div className={styles.modeSwitch} role="group" aria-label="Modo de vista">
          <button
            type="button"
            className={joinClasses(styles.modeSwitchButton, !isPreviewMode && styles.modeSwitchButtonActive)}
            aria-pressed={!isPreviewMode}
            onClick={() => {
              if (isPreviewMode) {
                handlePreviewToggle();
              }
            }}
          >
            Editar
          </button>
          <button
            type="button"
            className={joinClasses(styles.modeSwitchButton, isPreviewMode && styles.modeSwitchButtonActive)}
            aria-pressed={isPreviewMode}
            onClick={() => {
              if (!isPreviewMode) {
                handlePreviewToggle();
              }
            }}
          >
            Vista previa
          </button>
        </div>
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
      </div>
    );
  }, [
    canDecreaseZoom,
    canIncreaseZoom,
    handleZoomChange,
    handlePreviewToggle,
    effectiveZoomLevel,
    isPreviewMode,
    isVisualPagination,
    resolvedZoomLevel,
  ]);
  const visualWrapperStyle = useMemo(
    () =>
      ({
        "--app-editor-min-height": paginationMetrics.minHeight,
        "--app-editor-page-height": paginationMetrics.pageHeight,
        "--app-editor-page-width": paginationMetrics.pageWidth,
        "--app-editor-page-content-width": paginationMetrics.pageContentWidth,
        "--app-editor-page-gap": `${paginationMetrics.pageGap}px`,
        "--app-editor-page-content-height": `${Math.max(
          1,
          paginationMetrics.pageHeightValue - resolvedPageMarginsTop - resolvedPageMarginsBottom,
        )}px`,
        "--app-editor-total-pages": String(visualTotalPages),
        "--app-editor-page-margin-top": paginationMetrics.marginTop,
        "--app-editor-page-margin-right": paginationMetrics.marginRight,
        "--app-editor-page-margin-bottom": paginationMetrics.marginBottom,
        "--app-editor-page-margin-left": paginationMetrics.marginLeft,
        "--app-editor-zoom": String(effectiveZoomLevel),
        "--app-editor-sheet-height": `${sheetHeightValue}px`,
        "--app-editor-zoomed-sheet-width": `${zoomedSheetWidthValue}px`,
        "--app-editor-zoomed-sheet-height": `${zoomedSheetHeightValue}px`,
        "--app-editor-visual-content-height": `${Math.max(
          visualContentHeight,
          paginationMetrics.pageHeightValue - resolvedPageMarginsTop - resolvedPageMarginsBottom,
        )}px`,
      }) as CSSProperties,
    [
      effectiveZoomLevel,
      paginationMetrics.marginBottom,
      paginationMetrics.marginLeft,
      paginationMetrics.marginRight,
      paginationMetrics.marginTop,
      paginationMetrics.minHeight,
      paginationMetrics.pageGap,
      paginationMetrics.pageContentWidth,
      paginationMetrics.pageHeight,
      paginationMetrics.pageWidth,
      paginationMetrics.pageHeightValue,
      resolvedPageMarginsBottom,
      resolvedPageMarginsTop,
      sheetHeightValue,
      visualTotalPages,
      visualContentHeight,
      zoomedSheetHeightValue,
      zoomedSheetWidthValue,
    ],
  );

  useEffect(() => {
    if (!isVisualPagination || isPreviewMode) {
      return undefined;
    }

    const scrollContainer = paginationCanvasRef.current;
    if (!scrollContainer) {
      return undefined;
    }

    const handleWheel = (event: WheelEvent) => {
      const target = event.target;
      if (!(target instanceof Node) || !scrollContainer.contains(target)) {
        return;
      }

      const deltaX = normalizeWheelDelta(event.deltaX, event.deltaMode);
      const deltaY = normalizeWheelDelta(event.deltaY, event.deltaMode);
      if (deltaX === 0 && deltaY === 0) {
        return;
      }

      const maxTop = Math.max(0, scrollContainer.scrollHeight - scrollContainer.clientHeight);
      const maxLeft = Math.max(0, scrollContainer.scrollWidth - scrollContainer.clientWidth);
      const nextTop = Math.max(0, Math.min(maxTop, scrollContainer.scrollTop + deltaY));
      const nextLeft = Math.max(0, Math.min(maxLeft, scrollContainer.scrollLeft + deltaX));

      if (nextTop === scrollContainer.scrollTop && nextLeft === scrollContainer.scrollLeft) {
        return;
      }

      event.preventDefault();
      scrollContainer.scrollTop = nextTop;
      scrollContainer.scrollLeft = nextLeft;
    };

    scrollContainer.addEventListener("wheel", handleWheel, { capture: true, passive: false });

    return () => {
      scrollContainer.removeEventListener("wheel", handleWheel, { capture: true });
    };
  }, [editSurfaceVersion, isPreviewMode, isVisualPagination]);

  useLayoutEffect(() => {
    const host = previewStagePortalHostRef.current;
    if (!host) {
      return;
    }

    host.className = joinClasses(
      styles.previewStageSlot,
      isPresentationPortalActive && styles.previewStageSlotPresentation,
    );
    Object.entries(visualWrapperStyle).forEach(([property, value]) => {
      host.style.setProperty(property, String(value));
    });
  }, [isPresentationPortalActive, visualWrapperStyle]);

  useLayoutEffect(() => {
    const host = previewStagePortalHostRef.current;
    if (!host || !isVisualPagination || !isPreviewMode) {
      return undefined;
    }

    const target = isPresentationPortalActive
      ? document.body
      : previewStageLocalSlotRef.current;

    if (!target) {
      return undefined;
    }

    target.appendChild(host);

    return () => {
      if (host.parentElement === target) {
        target.removeChild(host);
      }
    };
  }, [isPresentationPortalActive, isPreviewMode, isVisualPagination]);

  const continuousWrapperStyle = useMemo(
    () => ({ "--app-editor-min-height": paginationMetrics.minHeight }) as CSSProperties,
    [paginationMetrics.minHeight],
  );
  const searchToolbarAction = useMemo(
    () => (
      <AppButton
        variant={isSearchOpen ? "primary" : "ghost"}
        size="sm"
        icon={<FontAwesomeIcon icon={faMagnifyingGlass} />}
        aria-label="Buscar en el documento"
        tooltip="Buscar"
        disabled={isPreviewMode}
        onClick={isSearchOpen ? closeSearch : openSearch}
      />
    ),
    [closeSearch, isPreviewMode, isSearchOpen, openSearch],
  );
  const presentationToolbarAction = useMemo(
    () => (
      <AppButton
        variant={isPresentationMode ? "primary" : "ghost"}
        size="sm"
        icon={<FullscreenOutlined />}
        aria-label="Modo presentación"
        tooltip="Modo presentación"
        disabled={!isPreviewMode}
        onClick={openPresentationMode}
      />
    ),
    [isPresentationMode, isPreviewMode, openPresentationMode],
  );
  const combinedToolbarActions = useMemo<ReactNode>(
    () => (
      <>
        {toolbarActions}
        {searchToolbarAction}
        {presentationToolbarAction}
      </>
    ),
    [presentationToolbarAction, searchToolbarAction, toolbarActions],
  );

  const pageIndicatorNode = isVisualPagination ? (
    <div
      className={joinClasses(
        styles.pageIndicator,
        isPreviewMode && isPresentationMode && styles.presentationPageIndicator,
      )}
      aria-label={isPreviewMode ? displayedPageLabel : pageProgressLabel}
      title={isPreviewMode ? displayedPageLabel : pageProgressLabel}
    >
      <button
        type="button"
        className={styles.pageNavButton}
        aria-label="Ir a la pagina anterior"
        disabled={displayedPage <= 1}
        onClick={handlePreviousPage}
      >
        &lt;
      </button>
      <span className={styles.pageIndicatorText} aria-live="polite">
        Pagina
      </span>
      {isPreviewMode ? (
        <span className={styles.pageIndicatorText}>{displayedPage}</span>
      ) : (
        <input
          className={styles.pageInput}
          type="number"
          min={1}
          max={totalPages}
          value={pageInputValue}
          aria-label="Ir a pagina"
          onChange={handlePageInputChange}
          onBlur={commitPageInput}
          onKeyDown={handlePageInputKeyDown}
        />
      )}
      <span className={styles.pageIndicatorText} aria-live="polite">
        de {displayedTotalPages}
      </span>
      <button
        type="button"
        className={styles.pageNavButton}
        aria-label="Ir a la pagina siguiente"
        disabled={displayedPage >= displayedTotalPages}
        onClick={handleNextPage}
      >
        &gt;
      </button>
    </div>
  ) : null;

  const previewStagePortal =
    isVisualPagination && isPreviewMode && previewStagePortalHost
      ? createPortal(
          <AppEditorPreview
            ref={previewRef}
            html={previewHtml}
            pageWidth={paginationMetrics.pageWidthValue}
            pageHeight={paginationMetrics.pageHeightValue}
            pageGap={paginationMetrics.pageGapValue}
            pageMargins={paginationMetrics.resolvedMargins}
            zoomLevel={effectiveZoomLevel}
            minHeight={paginationMetrics.minHeight}
            onPageCountChange={setPreviewPageCount}
            onCurrentPageChange={setPreviewCurrentPage}
          />,
          previewStagePortalHost,
        )
      : null;

  const presentationPortal =
    isPresentationPortalActive && typeof document !== "undefined"
      ? createPortal(
          <>
            <div className={styles.presentationOverlay} aria-hidden="true" />
            <AppButton
              variant="secondary"
              size="md"
              className={styles.presentationExitButton}
              onClick={closePresentationMode}
            >
              Salir
            </AppButton>
            {pageIndicatorNode}
          </>,
          document.body,
        )
      : null;

  return (
    <section
      ref={editorRootRef}
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
          disabled={!isEditable || isPreviewMode}
          onInsertLocalImage={insertLocalImage}
          toolbarActions={combinedToolbarActions}
          trailingContent={zoomTrailingContent}
        />
        {isSearchOpen && !isPreviewMode ? (
          <div className={styles.documentSearchBar} role="search" aria-label="Buscar en el documento">
            <AppInputSearch
              ref={searchInputRef}
              value={searchQuery}
              onChange={setSearchQuery}
              onSearch={setSearchQuery}
              onKeyDown={handleSearchKeyDown}
              placeholder="Buscar"
              size="sm"
              clearOnEscape={false}
              className={styles.documentSearchInput}
              aria-label="Texto a buscar en el documento"
            />
            <span className={styles.documentSearchCount} aria-live="polite">
              {searchResultLabel}
              {searchMatchCount > 0 ? ` (${activeSearchIndex + 1}/${searchMatchCount})` : ""}
            </span>
            <div className={styles.documentSearchActions}>
              <AppButton
                variant="ghost"
                size="sm"
                icon={<FontAwesomeIcon icon={faChevronUp} />}
                aria-label="Coincidencia anterior"
                tooltip="Anterior"
                disabled={searchMatchCount === 0}
                onClick={goToPreviousSearchMatch}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<FontAwesomeIcon icon={faChevronDown} />}
                aria-label="Coincidencia siguiente"
                tooltip="Siguiente"
                disabled={searchMatchCount === 0}
                onClick={goToNextSearchMatch}
              />
              <AppButton
                variant="ghost"
                size="sm"
                icon={<FontAwesomeIcon icon={faXmark} />}
                aria-label="Cerrar busqueda"
                tooltip="Cerrar"
                onClick={closeSearch}
              />
            </div>
          </div>
        ) : null}
        {presentationPortal}
        {previewStagePortal}
        {!isPresentationPortalActive ? pageIndicatorNode : null}
        {isVisualPagination ? (
          <div className={styles.pageStatsIndicator} aria-live="polite">
            {formatDocumentCount(documentTextStats.words)} palabras |{" "}
            {formatDocumentCount(documentTextStats.characters)} caracteres
          </div>
        ) : null}
        {isVisualPagination && isPreviewMode ? (
          <div
            className={styles.documentWorkspace}
            data-navigation-collapsed={thumbnailsCollapsed}
            data-presentation-active={isPresentationMode}
          >
            <div className={styles.previewNavigationSlot}>
              <AppCollapseRail
                title="Miniaturas"
                collapsed={thumbnailsCollapsed}
                onToggle={() => setThumbnailsCollapsed((previousValue) => !previousValue)}
                placement="left"
                variant="inline"
                railLabel="Miniaturas"
                className={styles.navigationRail}
              >
                <AppEditorNavigationPanel
                  editor={editor}
                  pages={[]}
                  totalPages={previewPageCount}
                  currentPage={Math.min(
                    Math.max(previewCurrentPage, 1),
                    Math.max(1, previewPageCount),
                  )}
                  canvasRef={paginationCanvasRef}
                  zoomLevel={effectiveZoomLevel}
                  onGoToPage={(pageNumber) => previewRef.current?.goToPage(pageNumber)}
                  showOutline={false}
                />
              </AppCollapseRail>
            </div>
            <div ref={previewStageLocalSlotRef} className={styles.previewStageLocalSlot} />
          </div>
        ) : isVisualPagination ? (
          <div
            className={styles.documentWorkspace}
            data-navigation-collapsed={structureCollapsed}
          >
            <AppCollapseRail
              title="Estructura"
              collapsed={structureCollapsed}
              onToggle={() => setStructureCollapsed((previousValue) => !previousValue)}
              placement="left"
              variant="inline"
              railLabel="Estructura"
              className={styles.navigationRail}
            >
              <AppEditorNavigationPanel
                editor={editor}
                pages={pages}
                totalPages={totalPages}
                currentPage={currentPage}
                canvasRef={paginationCanvasRef}
                zoomLevel={effectiveZoomLevel}
                onGoToPage={goToPage}
              />
            </AppCollapseRail>
            <div
              className={styles.editorWrapper}
              key={editSurfaceVersion}
              ref={paginationContainerRef}
              style={visualWrapperStyle}
            >
              <div
                className={styles.canvas}
                ref={paginationCanvasRef}
                data-app-editor-scroll-container="true"
              >
                <div className={styles.zoomStage}>
                  <div className={styles.sheet} data-pagination-sheet="true">
                    <div className={styles.pageIndicatorLayer} aria-hidden="true">
                      {pages
                        .filter((page) => page.pageNumber > 1)
                        .map((page) => (
                          <span
                            key={page.pageNumber}
                            className={styles.pageIndicatorPill}
                            style={{ top: `${page.top + PAGE_INDICATOR_TOP_OFFSET}px` }}
                          >
                            Pagina {page.pageNumber}
                          </span>
                        ))}
                    </div>
                    <div className={styles.contentFlow} data-app-editor-content-flow="true">
                      <TiptapEditorContent
                        editor={editor}
                        className={joinClasses(
                          styles.editorContent,
                          styles.editorContentPaged,
                          surfaceClassName,
                          Boolean(error) && styles.surfaceError,
                        )}
                        aria-labelledby={labelId}
                        aria-label={resolvedAriaLabel}
                        aria-describedby={describedBy}
                        aria-invalid={Boolean(error)}
                      />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        ) : (
          <TiptapEditorContent
            editor={editor}
            className={joinClasses(
              styles.editorContent,
              surfaceClassName,
              Boolean(error) && styles.surfaceError,
            )}
            style={continuousWrapperStyle}
            aria-labelledby={labelId}
            aria-label={resolvedAriaLabel}
            aria-describedby={describedBy}
            aria-invalid={Boolean(error)}
          />
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
