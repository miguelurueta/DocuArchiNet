import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type { RefObject } from "react";
import type { Editor } from "@tiptap/react";
import type { Transaction } from "@tiptap/pm/state";
import type { VisualPage } from "./autoPagination";

type UseVisualPageNavigationOptions = {
  enabled: boolean;
  editor: Editor | null;
  pages: VisualPage[];
  totalPages: number;
  canvasRef: RefObject<HTMLElement | null>;
  zoomLevel?: number;
};

type PagePosition = {
  page: number;
  y: number;
};

const DEFAULT_PAGE = 1;
const DEFAULT_PROGRESS = 0;

function clamp(value: number, min: number, max: number) {
  return Math.min(Math.max(value, min), max);
}

function clampPage(pageNumber: number, totalPages: number) {
  return clamp(
    Number.isFinite(pageNumber) ? Math.floor(pageNumber) : DEFAULT_PAGE,
    DEFAULT_PAGE,
    Math.max(DEFAULT_PAGE, totalPages),
  );
}

function normalizeProgress(value: number) {
  return clamp(Number.isFinite(value) ? value : DEFAULT_PROGRESS, 0, 1);
}

function normalizePages(pages: VisualPage[], totalPages: number): VisualPage[] {
  if (pages.length > 0) {
    return pages;
  }

  return Array.from({ length: Math.max(DEFAULT_PAGE, totalPages) }, (_, index) => ({
    pageNumber: index + 1,
    top: index,
    bottom: index + 1,
    contentTop: index,
    contentBottom: index + 1,
    startBlockIndex: 0,
    endBlockIndex: 0,
  }));
}

function resolvePageFromY(y: number, pages: VisualPage[], totalPages: number) {
  const safeY = Math.max(0, Number.isFinite(y) ? y : 0);
  const safePages = normalizePages(pages, totalPages);
  let low = 0;
  let high = safePages.length - 1;

  while (low <= high) {
    const mid = (low + high) >>> 1;
    const page = safePages[mid];

    if (safeY < page.top) {
      high = mid - 1;
    } else if (safeY >= page.bottom) {
      low = mid + 1;
    } else {
      return page;
    }
  }

  if (safeY < safePages[0].top) {
    return safePages[0];
  }

  for (let index = 0; index < safePages.length - 1; index += 1) {
    const currentPage = safePages[index];
    const nextPage = safePages[index + 1];

    if (safeY > currentPage.bottom && safeY < nextPage.top) {
      const gapMidpoint = currentPage.bottom + (nextPage.top - currentPage.bottom) / 2;
      return safeY < gapMidpoint ? currentPage : nextPage;
    }
  }

  return safePages[safePages.length - 1];
}

function resolveDocumentBottom(pages: VisualPage[]) {
  return pages.reduce((maxBottom, page) => Math.max(maxBottom, page.bottom), 0);
}

function resolvePositionMetrics(y: number, pages: VisualPage[], totalPages: number) {
  const page = resolvePageFromY(y, pages, totalPages);
  const pageHeight = Math.max(1, page.bottom - page.top);
  const documentBottom = Math.max(1, resolveDocumentBottom(normalizePages(pages, totalPages)));

  return {
    page: clampPage(page.pageNumber, totalPages),
    relativePageProgress: normalizeProgress((Math.max(0, y) - page.top) / pageHeight),
    documentProgress: normalizeProgress(Math.max(0, y) / documentBottom),
  };
}

function resolveCursorY({
  editor,
  canvas,
  zoomLevel,
}: {
  editor: Editor | null;
  canvas: HTMLElement;
  zoomLevel: number;
}) {
  const selection = editor?.state.selection;
  const view = editor?.view;

  if (!selection || !view || typeof view.coordsAtPos !== "function") {
    return null;
  }

  const positions = [selection.head, selection.to, selection.from].filter(
    (position, index, allPositions) =>
      typeof position === "number" && allPositions.indexOf(position) === index,
  );
  const sheet = canvas.querySelector<HTMLElement>('[data-pagination-sheet="true"]');
  const containerRect = (sheet ?? canvas).getBoundingClientRect();

  for (const position of positions) {
    try {
      const coords = view.coordsAtPos(position);
      const centerY = (coords.top + coords.bottom) / 2;
      const y = (centerY - containerRect.top) / Math.max(0.1, zoomLevel);

      if (Number.isFinite(y)) {
        return Math.max(0, y);
      }
    } catch {
      continue;
    }
  }

  const domSelection = window.getSelection();
  if (domSelection && domSelection.rangeCount > 0) {
    const range = domSelection.getRangeAt(0);
    const rect = range.getBoundingClientRect();
    const centerY = (rect.top + rect.bottom) / 2;
    const y = (centerY - containerRect.top) / Math.max(0.1, zoomLevel);

    if (Number.isFinite(y)) {
      return Math.max(0, y);
    }
  }

  return null;
}

function resolveViewportCenterY(canvas: HTMLElement, zoomLevel: number) {
  const sheet = canvas.querySelector<HTMLElement>('[data-pagination-sheet="true"]');
  const originRect = (sheet ?? canvas).getBoundingClientRect();
  const canvasRect = canvas.getBoundingClientRect();
  const viewportCenterY = canvasRect.top + canvas.clientHeight / 2;
  const y = (viewportCenterY - originRect.top) / Math.max(0.1, zoomLevel);

  return Math.max(0, Number.isFinite(y) ? y : 0);
}

export function useVisualPageNavigation({
  enabled,
  editor,
  pages,
  totalPages,
  canvasRef,
  zoomLevel = 1,
}: UseVisualPageNavigationOptions) {
  const safeTotalPages = Math.max(DEFAULT_PAGE, totalPages);
  const safePages = useMemo(() => normalizePages(pages, safeTotalPages), [pages, safeTotalPages]);
  const [pagePosition, setPagePosition] = useState<PagePosition>({
    page: DEFAULT_PAGE,
    y: 0,
  });
  const frameRef = useRef<number | null>(null);
  const preferScrollUntilSelectionChangeRef = useRef(false);

  const commitPosition = useCallback(
    (nextPosition: PagePosition) => {
      setPagePosition((previousPosition) =>
        previousPosition.page === nextPosition.page && previousPosition.y === nextPosition.y
          ? previousPosition
          : nextPosition,
      );
    },
    [],
  );

  const updateFromCurrentContext = useCallback(() => {
    if (!enabled) {
      commitPosition({ page: DEFAULT_PAGE, y: 0 });
      return;
    }

    const canvas = canvasRef.current;
    if (!(canvas instanceof HTMLElement)) {
      commitPosition({ page: DEFAULT_PAGE, y: 0 });
      return;
    }

    const viewportY = resolveViewportCenterY(canvas, zoomLevel);
    const cursorY = preferScrollUntilSelectionChangeRef.current
      ? null
      : resolveCursorY({
          editor,
          canvas,
          zoomLevel,
        });
    const y = cursorY ?? viewportY;
    const metrics = resolvePositionMetrics(y, safePages, safeTotalPages);

    commitPosition({
      page: metrics.page,
      y,
    });
  }, [canvasRef, commitPosition, editor, enabled, safePages, safeTotalPages, zoomLevel]);

  const scheduleUpdate = useCallback(() => {
    if (frameRef.current !== null) {
      window.cancelAnimationFrame(frameRef.current);
    }

    frameRef.current = window.requestAnimationFrame(() => {
      frameRef.current = null;
      updateFromCurrentContext();
    });
  }, [updateFromCurrentContext]);

  const goToPage = useCallback(
    (pageNumber: number) => {
      const canvas = canvasRef.current;
      if (!(canvas instanceof HTMLElement)) {
        return;
      }

      const safePage = clampPage(pageNumber, safeTotalPages);
      const page = safePages.find((candidate) => candidate.pageNumber === safePage) ?? safePages[0];
      const maxTop = Math.max(0, canvas.scrollHeight - canvas.clientHeight);
      const nextTop = clamp(page.top * Math.max(0.1, zoomLevel), 0, maxTop);

      preferScrollUntilSelectionChangeRef.current = true;
      canvas.scrollTo({
        top: nextTop,
        behavior: "smooth",
      });
      commitPosition({
        page: safePage,
        y: page.top,
      });
    },
    [canvasRef, commitPosition, safePages, safeTotalPages, zoomLevel],
  );

  const goToPreviousPage = useCallback(() => {
    goToPage(pagePosition.page - 1);
  }, [goToPage, pagePosition.page]);

  const goToNextPage = useCallback(() => {
    goToPage(pagePosition.page + 1);
  }, [goToPage, pagePosition.page]);

  useEffect(() => {
    if (!enabled) {
      return undefined;
    }

    const canvas = canvasRef.current;
    const handleSelectionUpdate = () => {
      preferScrollUntilSelectionChangeRef.current = false;
      scheduleUpdate();
    };
    const handleTransaction = (...args: unknown[]) => {
      const payload = args[0];
      const transaction = payload &&
        typeof payload === "object" &&
        "transaction" in payload
        ? (payload as { transaction?: Transaction }).transaction
        : undefined;

      if (transaction?.selectionSet === true || transaction?.docChanged === true) {
        preferScrollUntilSelectionChangeRef.current = false;
      }

      if (transaction?.selectionSet === true || transaction?.docChanged === true) {
        scheduleUpdate();
      }
    };
    const handleScroll = () => {
      preferScrollUntilSelectionChangeRef.current = true;
      scheduleUpdate();
    };
    const handlePaginationUpdated = () => {
      scheduleUpdate();
    };

    scheduleUpdate();
    canvas?.addEventListener("scroll", handleScroll, { passive: true });
    canvas?.addEventListener("app-editor-pagination-updated", handlePaginationUpdated as EventListener);
    editor?.on("selectionUpdate", handleSelectionUpdate);
    editor?.on("transaction", handleTransaction);

    return () => {
      if (frameRef.current !== null) {
        window.cancelAnimationFrame(frameRef.current);
        frameRef.current = null;
      }

      canvas?.removeEventListener("scroll", handleScroll);
      canvas?.removeEventListener(
        "app-editor-pagination-updated",
        handlePaginationUpdated as EventListener,
      );
      editor?.off("selectionUpdate", handleSelectionUpdate);
      editor?.off("transaction", handleTransaction);
    };
  }, [canvasRef, commitPosition, editor, enabled, scheduleUpdate]);

  const positionMetrics = resolvePositionMetrics(pagePosition.y, safePages, safeTotalPages);

  return {
    currentPage: enabled ? clampPage(pagePosition.page, safeTotalPages) : DEFAULT_PAGE,
    totalPages: safeTotalPages,
    relativePageProgress: enabled ? positionMetrics.relativePageProgress : DEFAULT_PROGRESS,
    documentProgress: enabled ? positionMetrics.documentProgress : DEFAULT_PROGRESS,
    pages: safePages,
    goToPage,
    goToPreviousPage,
    goToNextPage,
  };
}
