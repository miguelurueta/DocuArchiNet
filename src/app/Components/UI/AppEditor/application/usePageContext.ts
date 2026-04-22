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
    setCurrentPage((previousPage) => (previousPage === nextPage ? previousPage : nextPage));
  }, []);

  const resolvePageFromScroll = useCallback(() => {
    const canvas = canvasRef.current;
    if (!(canvas instanceof HTMLElement)) {
      return DEFAULT_PAGE;
    }

    const sheet = canvas.querySelector('[data-pagination-sheet="true"]');
    if (!(sheet instanceof HTMLElement)) {
      return resolvePageFromOffset(canvas.scrollTop, pageBoundaries, totalPages, zoomLevel);
    }

    const offset = Math.max(0, canvas.scrollTop - sheet.offsetTop);
    return resolvePageFromOffset(offset, pageBoundaries, totalPages, zoomLevel);
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
    const proseMirror = canvasRef.current?.querySelector(".ProseMirror");

    canvas?.addEventListener("scroll", handleScroll, { passive: true });
    proseMirror?.addEventListener("app-editor-pagination-updated", handlePaginationUpdated);
    return () => {
      clearPending();
      canvas?.removeEventListener("scroll", handleScroll);
      proseMirror?.removeEventListener("app-editor-pagination-updated", handlePaginationUpdated);
    };
  }, [canvasRef, clearPending, commitPage, enabled, scheduleUpdate]);

  return {
    currentPage: clampPage(currentPage, totalPages),
  };
}
