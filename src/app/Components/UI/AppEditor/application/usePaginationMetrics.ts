import { useCallback, useLayoutEffect, useRef, useState } from "react";
import type { RefObject } from "react";
import type { Editor } from "@tiptap/react";
import type { AppEditorPageMargins } from "../domain/editor.types";

type PaginationMetrics = {
  contentHeight: number;
  pageContentHeight: number;
  totalPages: number;
  guideOffsets: number[];
};

type UsePaginationMetricsOptions = {
  editor: Editor | null;
  enabled: boolean;
  pageHeight: number;
  pageMargins: AppEditorPageMargins;
  containerRef: RefObject<HTMLElement | null>;
  debounceMs?: number;
};

const DEFAULT_METRICS: PaginationMetrics = {
  contentHeight: 0,
  pageContentHeight: 0,
  totalPages: 1,
  guideOffsets: [],
};

function areMetricsEqual(left: PaginationMetrics, right: PaginationMetrics) {
  return (
    left.contentHeight === right.contentHeight &&
    left.pageContentHeight === right.pageContentHeight &&
    left.totalPages === right.totalPages &&
    left.guideOffsets.length === right.guideOffsets.length &&
    left.guideOffsets.every((offset, index) => offset === right.guideOffsets[index])
  );
}

export function calculatePaginationMetrics({
  contentHeight,
  pageHeight,
  pageMargins,
}: {
  contentHeight: number;
  pageHeight: number;
  pageMargins: AppEditorPageMargins;
}): PaginationMetrics {
  const safeContentHeight = Math.max(0, Math.ceil(contentHeight));
  const pageContentHeight = Math.max(1, pageHeight - pageMargins.top - pageMargins.bottom);
  const totalPages = Math.max(1, Math.ceil(safeContentHeight / pageContentHeight));
  const guideOffsets = Array.from({ length: Math.max(0, totalPages - 1) }, (_, index) =>
    (index + 1) * pageContentHeight + pageMargins.top,
  );

  return {
    contentHeight: safeContentHeight,
    pageContentHeight,
    totalPages,
    guideOffsets,
  };
}

export function usePaginationMetrics({
  editor,
  enabled,
  pageHeight,
  pageMargins,
  containerRef,
  debounceMs = 32,
}: UsePaginationMetricsOptions): PaginationMetrics {
  const [metrics, setMetrics] = useState<PaginationMetrics>(DEFAULT_METRICS);
  const timeoutRef = useRef<number | null>(null);
  const frameRef = useRef<number | null>(null);

  const clearPendingMeasure = useCallback(() => {
    if (timeoutRef.current !== null) {
      window.clearTimeout(timeoutRef.current);
      timeoutRef.current = null;
    }

    if (frameRef.current !== null) {
      window.cancelAnimationFrame(frameRef.current);
      frameRef.current = null;
    }
  }, []);

  const commitMetrics = useCallback((nextMetrics: PaginationMetrics) => {
    setMetrics((previousMetrics) =>
      areMetricsEqual(previousMetrics, nextMetrics) ? previousMetrics : nextMetrics,
    );
  }, []);

  const measure = useCallback(() => {
    if (!enabled) {
      commitMetrics(DEFAULT_METRICS);
      return;
    }

    const container = containerRef.current;
    const proseMirror = container?.querySelector(".ProseMirror");

    if (!(proseMirror instanceof HTMLElement)) {
      commitMetrics(DEFAULT_METRICS);
      return;
    }

    commitMetrics(
      calculatePaginationMetrics({
        contentHeight: proseMirror.scrollHeight,
        pageHeight,
        pageMargins,
      }),
    );
  }, [commitMetrics, containerRef, enabled, pageHeight, pageMargins]);

  const scheduleMeasure = useCallback(() => {
    clearPendingMeasure();

    timeoutRef.current = window.setTimeout(() => {
      frameRef.current = window.requestAnimationFrame(() => {
        measure();
        frameRef.current = null;
      });
      timeoutRef.current = null;
    }, debounceMs);
  }, [clearPendingMeasure, debounceMs, measure]);

  useLayoutEffect(() => {
    if (!enabled) {
      clearPendingMeasure();
      commitMetrics(DEFAULT_METRICS);
      return undefined;
    }

    scheduleMeasure();

    const handleResize = () => {
      scheduleMeasure();
    };

    window.addEventListener("resize", handleResize);

    const resizeObserver =
      typeof ResizeObserver !== "undefined"
        ? new ResizeObserver(() => {
            scheduleMeasure();
          })
        : null;

    const container = containerRef.current;
    const proseMirror = container?.querySelector(".ProseMirror");

    if (container instanceof HTMLElement) {
      resizeObserver?.observe(container);
    }

    if (proseMirror instanceof HTMLElement) {
      resizeObserver?.observe(proseMirror);
    }

    editor?.on("update", scheduleMeasure);

    return () => {
      clearPendingMeasure();
      window.removeEventListener("resize", handleResize);
      resizeObserver?.disconnect();
      editor?.off("update", scheduleMeasure);
    };
  }, [
    clearPendingMeasure,
    commitMetrics,
    containerRef,
    editor,
    enabled,
    scheduleMeasure,
  ]);

  return metrics;
}
