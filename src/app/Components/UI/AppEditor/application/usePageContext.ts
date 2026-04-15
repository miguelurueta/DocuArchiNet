import { useCallback, useEffect, useRef, useState } from "react";
import type { RefObject } from "react";
import type { Editor } from "@tiptap/react";

type UsePageContextOptions = {
  editor: Editor | null;
  enabled: boolean;
  totalPages: number;
  pageContentHeight: number;
  canvasRef: RefObject<HTMLElement | null>;
  debounceMs?: number;
};

const DEFAULT_PAGE = 1;

function clampPage(page: number, totalPages: number) {
  return Math.min(Math.max(page, DEFAULT_PAGE), Math.max(DEFAULT_PAGE, totalPages));
}

function resolvePageFromOffset(offset: number, pageContentHeight: number, totalPages: number) {
  if (pageContentHeight <= 0) {
    return DEFAULT_PAGE;
  }

  return clampPage(Math.floor(Math.max(0, offset) / pageContentHeight) + 1, totalPages);
}

export function calculatePageFromOffset({
  offset,
  pageContentHeight,
  totalPages,
}: {
  offset: number;
  pageContentHeight: number;
  totalPages: number;
}) {
  return resolvePageFromOffset(offset, pageContentHeight, totalPages);
}

export function usePageContext({
  editor,
  enabled,
  totalPages,
  pageContentHeight,
  canvasRef,
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
      return resolvePageFromOffset(canvas.scrollTop, pageContentHeight, totalPages);
    }

    const offset = Math.max(0, canvas.scrollTop - sheet.offsetTop);
    return resolvePageFromOffset(offset, pageContentHeight, totalPages);
  }, [canvasRef, pageContentHeight, totalPages]);

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
      return resolvePageFromOffset(offset, pageContentHeight, totalPages);
    } catch {
      return null;
    }
  }, [canvasRef, editor, pageContentHeight, totalPages]);

  const updateCurrentPage = useCallback(() => {
    if (!enabled) {
      commitPage(DEFAULT_PAGE);
      return;
    }

    const pageFromCursor = resolvePageFromCursor();
    commitPage(pageFromCursor ?? resolvePageFromScroll());
  }, [commitPage, enabled, resolvePageFromCursor, resolvePageFromScroll]);

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
