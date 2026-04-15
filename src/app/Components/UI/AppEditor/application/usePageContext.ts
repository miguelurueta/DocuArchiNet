import { useCallback, useEffect, useRef, useState } from "react";
import type { RefObject } from "react";
import type { Editor } from "@tiptap/react";

type UsePageContextOptions = {
  editor: Editor | null;
  enabled: boolean;
  totalPages: number;
  pageBoundaries: number[];
  canvasRef: RefObject<HTMLElement | null>;
  debounceMs?: number;
  scrollPriorityMs?: number;
};

const DEFAULT_PAGE = 1;

function clampPage(page: number, totalPages: number) {
  return Math.min(Math.max(page, DEFAULT_PAGE), Math.max(DEFAULT_PAGE, totalPages));
}

function resolvePageFromOffset(offset: number, pageBoundaries: number[], totalPages: number) {
  const safeOffset = Math.max(0, offset);
  const crossedBoundaries = pageBoundaries.filter((boundary) => safeOffset >= boundary).length;
  return clampPage(crossedBoundaries + 1, totalPages);
}

export function calculatePageFromOffset({
  offset,
  pageBoundaries,
  totalPages,
}: {
  offset: number;
  pageBoundaries: number[];
  totalPages: number;
}) {
  return resolvePageFromOffset(offset, pageBoundaries, totalPages);
}

export function usePageContext({
  editor,
  enabled,
  totalPages,
  pageBoundaries,
  canvasRef,
  debounceMs = 32,
  scrollPriorityMs = 240,
}: UsePageContextOptions) {
  const [currentPage, setCurrentPage] = useState(DEFAULT_PAGE);
  const timeoutRef = useRef<number | null>(null);
  const frameRef = useRef<number | null>(null);
  const lastScrollAtRef = useRef<number>(0);

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
      return resolvePageFromOffset(canvas.scrollTop, pageBoundaries, totalPages);
    }

    const offset = Math.max(0, canvas.scrollTop - sheet.offsetTop);
    return resolvePageFromOffset(offset, pageBoundaries, totalPages);
  }, [canvasRef, pageBoundaries, totalPages]);

  const resolvePageFromCursor = useCallback(() => {
    if (!editor?.isFocused) {
      return null;
    }

    const proseMirror = canvasRef.current?.querySelector(".ProseMirror");
    if (!(proseMirror instanceof HTMLElement)) {
      return null;
    }

    try {
      const { from } = editor.state.selection;
      const coords = editor.view.coordsAtPos(from);
      const proseMirrorRect = proseMirror.getBoundingClientRect();
      const offset = coords.top - proseMirrorRect.top;
      return resolvePageFromOffset(offset, pageBoundaries, totalPages);
    } catch {
      return null;
    }
  }, [canvasRef, editor, pageBoundaries, totalPages]);

  const updateCurrentPage = useCallback(() => {
    if (!enabled) {
      commitPage(DEFAULT_PAGE);
      return;
    }

    const shouldPrioritizeScroll =
      lastScrollAtRef.current > 0 && Date.now() - lastScrollAtRef.current <= scrollPriorityMs;

    if (shouldPrioritizeScroll) {
      commitPage(resolvePageFromScroll());
      return;
    }

    const pageFromCursor = resolvePageFromCursor();
    commitPage(pageFromCursor ?? resolvePageFromScroll());
  }, [commitPage, enabled, resolvePageFromCursor, resolvePageFromScroll, scrollPriorityMs]);

  const scheduleUpdate = useCallback(() => {
    clearPending();

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

    scheduleUpdate();

    const canvas = canvasRef.current;
    const handleScroll = () => {
      lastScrollAtRef.current = Date.now();
      scheduleUpdate();
    };

    canvas?.addEventListener("scroll", handleScroll, { passive: true });
    editor?.on("selectionUpdate", scheduleUpdate);
    editor?.on("focus", scheduleUpdate);
    editor?.on("blur", scheduleUpdate);
    editor?.on("update", scheduleUpdate);

    return () => {
      clearPending();
      canvas?.removeEventListener("scroll", handleScroll);
      editor?.off("selectionUpdate", scheduleUpdate);
      editor?.off("focus", scheduleUpdate);
      editor?.off("blur", scheduleUpdate);
      editor?.off("update", scheduleUpdate);
    };
  }, [canvasRef, clearPending, commitPage, editor, enabled, scheduleUpdate]);

  return {
    currentPage: clampPage(currentPage, totalPages),
  };
}
