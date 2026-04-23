import { useCallback, useEffect, useRef, useState } from "react";
import type { RefObject } from "react";
type UsePageContextOptions = {
  enabled: boolean;
  totalPages: number;
  pageBoundaries: number[];
  canvasRef: RefObject<HTMLElement | null>;
  zoomLevel?: number;
  debounceMs?: number;
};

const DEFAULT_PAGE = 1;
const MIN_HYSTERESIS_PX = 18;
const MAX_HYSTERESIS_PX = 56;

function clampPage(page: number, totalPages: number) {
  return Math.min(Math.max(page, DEFAULT_PAGE), Math.max(DEFAULT_PAGE, totalPages));
}

function resolvePageFromOffset(
  offset: number,
  pageBoundaries: number[],
  totalPages: number,
  boundaryScale = 1,
) {
  const safeOffset = Math.max(0, offset);
  const safeBoundaryScale = Math.max(0.1, boundaryScale);
  const crossedBoundaries = pageBoundaries.filter(
    (boundary) => safeOffset >= boundary * safeBoundaryScale,
  ).length;
  return clampPage(crossedBoundaries + 1, totalPages);
}

function resolvePageHysteresis({
  viewportHeight,
  boundaryScale,
}: {
  viewportHeight: number;
  boundaryScale: number;
}) {
  const safeViewportHeight = Math.max(0, viewportHeight);
  const safeBoundaryScale = Math.max(0.1, boundaryScale);
  const normalizedViewportHeight = safeViewportHeight / safeBoundaryScale;
  return Math.min(
    Math.max(Math.round(normalizedViewportHeight * 0.04), MIN_HYSTERESIS_PX),
    MAX_HYSTERESIS_PX,
  );
}

function resolveStablePageFromOffset({
  offset,
  pageBoundaries,
  totalPages,
  boundaryScale = 1,
  previousPage,
  hysteresisPx,
}: {
  offset: number;
  pageBoundaries: number[];
  totalPages: number;
  boundaryScale?: number;
  previousPage: number;
  hysteresisPx: number;
}) {
  const nextPage = resolvePageFromOffset(offset, pageBoundaries, totalPages, boundaryScale);
  const stablePreviousPage = clampPage(previousPage, totalPages);

  if (nextPage === stablePreviousPage || Math.abs(nextPage - stablePreviousPage) > 1) {
    return nextPage;
  }

  const safeBoundaryScale = Math.max(0.1, boundaryScale);
  const safeOffset = Math.max(0, offset);
  const safeHysteresisPx = Math.max(0, hysteresisPx);

  if (nextPage > stablePreviousPage) {
    const forwardBoundary = pageBoundaries[stablePreviousPage - 1];
    if (typeof forwardBoundary !== "number") {
      return nextPage;
    }

    return safeOffset >= forwardBoundary * safeBoundaryScale + safeHysteresisPx
      ? nextPage
      : stablePreviousPage;
  }

  const backwardBoundary = pageBoundaries[stablePreviousPage - 2];
  if (typeof backwardBoundary !== "number") {
    return nextPage;
  }

  return safeOffset < backwardBoundary * safeBoundaryScale - safeHysteresisPx
    ? nextPage
    : stablePreviousPage;
}

export function calculatePageFromOffset({
  offset,
  pageBoundaries,
  totalPages,
  boundaryScale = 1,
}: {
  offset: number;
  pageBoundaries: number[];
  totalPages: number;
  boundaryScale?: number;
}) {
  return resolvePageFromOffset(offset, pageBoundaries, totalPages, boundaryScale);
}

export function usePageContext({
  enabled,
  totalPages,
  pageBoundaries,
  canvasRef,
  zoomLevel = 1,
  debounceMs = 32,
}: UsePageContextOptions) {
  const [currentPage, setCurrentPage] = useState(DEFAULT_PAGE);
  const timeoutRef = useRef<number | null>(null);
  const frameRef = useRef<number | null>(null);
  const currentPageRef = useRef(DEFAULT_PAGE);

  const clearPending = useCallback(() => {
    if (timeoutRef.current !== null) {
      window.clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }

    if (frameRef.current !== null) {
      window.cancelAnimationFrame(frameRef.current);
      frameRef.current = null;
    }
  }, []);

  const commitPage = useCallback((nextPage: number) => {
    currentPageRef.current = nextPage;
    setCurrentPage((previousPage) => (previousPage === nextPage ? previousPage : nextPage));
  }, []);

  const resolvePageFromScroll = useCallback(() => {
    const canvas = canvasRef.current;
    if (!(canvas instanceof HTMLElement)) {
      return DEFAULT_PAGE;
    }

    const sheet = canvas.querySelector('[data-pagination-sheet="true"]');
    const offset = !(sheet instanceof HTMLElement)
      ? canvas.scrollTop
      : Math.max(0, canvas.scrollTop - sheet.offsetTop);

    return resolveStablePageFromOffset({
      offset,
      pageBoundaries,
      totalPages,
      boundaryScale: zoomLevel,
      previousPage: currentPageRef.current,
      hysteresisPx: resolvePageHysteresis({
        viewportHeight: canvas.clientHeight,
        boundaryScale: zoomLevel,
      }),
    });
  }, [canvasRef, pageBoundaries, totalPages, zoomLevel]);

  const updateCurrentPage = useCallback(() => {
    if (!enabled) {
      commitPage(DEFAULT_PAGE);
      return;
    }
    commitPage(resolvePageFromScroll());
  }, [commitPage, enabled, resolvePageFromScroll]);

  const scheduleUpdate = useCallback((priority: "immediate" | "frame" | "deferred" = "deferred") => {
    clearPending();

    if (priority === "immediate") {
      updateCurrentPage();
      return;
    }

    if (priority === "frame") {
      frameRef.current = window.requestAnimationFrame(() => {
        updateCurrentPage();
        frameRef.current = null;
      });
      return;
    }

    timeoutRef.current = window.setTimeout(() => {
      frameRef.current = window.requestAnimationFrame(() => {
        updateCurrentPage();
        frameRef.current = null;
      });
      timeoutRef.current = null;
    }, debounceMs);
  }, [clearPending, debounceMs, updateCurrentPage]);

  useEffect(() => {
    if (!enabled) {
      clearPending();
      commitPage(DEFAULT_PAGE);
      return undefined;
    }

    scheduleUpdate("deferred");

    const canvas = canvasRef.current;
    const handleScroll = () => {
      scheduleUpdate("frame");
    };
    const handlePaginationUpdated = () => {
      scheduleUpdate("immediate");
    };

    canvas?.addEventListener("scroll", handleScroll, { passive: true });
    canvas?.addEventListener("app-editor-pagination-updated", handlePaginationUpdated as EventListener);
    return () => {
      clearPending();
      canvas?.removeEventListener("scroll", handleScroll);
      canvas?.removeEventListener(
        "app-editor-pagination-updated",
        handlePaginationUpdated as EventListener,
      );
    };
  }, [canvasRef, clearPending, commitPage, enabled, scheduleUpdate]);

  return {
    currentPage: clampPage(currentPage, totalPages),
  };
}
