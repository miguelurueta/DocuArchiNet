import { useCallback, useLayoutEffect, useRef, useState } from "react";
import type { RefObject } from "react";
import type { Editor } from "@tiptap/react";
import type { AppEditorPageMargins } from "../domain/editor.types";

type PaginationMetrics = {
  contentHeight: number;
  pageContentHeight: number;
  totalPages: number;
  guideOffsets: number[];
  pageBoundaries: number[];
  visualPageBoundaries: number[];
  manualBreakOffsets: number[];
  pageStride: number;
  visualContentHeight: number;
};

type UsePaginationMetricsOptions = {
  editor: Editor | null;
  enabled: boolean;
  pageHeight: number;
  pageGap: number;
  pageMargins: AppEditorPageMargins;
  containerRef: RefObject<HTMLElement | null>;
  debounceMs?: number;
};

const DEFAULT_METRICS: PaginationMetrics = {
  contentHeight: 0,
  pageContentHeight: 0,
  totalPages: 1,
  guideOffsets: [],
  pageBoundaries: [],
  visualPageBoundaries: [],
  manualBreakOffsets: [],
  pageStride: 0,
  visualContentHeight: 0,
};

function areMetricsEqual(left: PaginationMetrics, right: PaginationMetrics) {
  return (
    left.contentHeight === right.contentHeight &&
    left.pageContentHeight === right.pageContentHeight &&
    left.totalPages === right.totalPages &&
    left.pageBoundaries.length === right.pageBoundaries.length &&
    left.pageBoundaries.every((offset, index) => offset === right.pageBoundaries[index]) &&
    left.visualPageBoundaries.length === right.visualPageBoundaries.length &&
    left.visualPageBoundaries.every(
      (offset, index) => offset === right.visualPageBoundaries[index],
    ) &&
    left.manualBreakOffsets.length === right.manualBreakOffsets.length &&
    left.manualBreakOffsets.every((offset, index) => offset === right.manualBreakOffsets[index]) &&
    left.guideOffsets.length === right.guideOffsets.length &&
    left.guideOffsets.every((offset, index) => offset === right.guideOffsets[index]) &&
    left.pageStride === right.pageStride &&
    left.visualContentHeight === right.visualContentHeight
  );
}

export function calculatePaginationMetrics({
  contentHeight,
  pageHeight,
  pageGap = 0,
  pageMargins,
  manualBreakOffsets = [],
}: {
  contentHeight: number;
  pageHeight: number;
  pageGap?: number;
  pageMargins: AppEditorPageMargins;
  manualBreakOffsets?: number[];
}): PaginationMetrics {
  const safeContentHeight = Math.max(0, Math.ceil(contentHeight));
  const pageContentHeight = Math.max(1, pageHeight - pageMargins.top - pageMargins.bottom);
  const pageStride = Math.max(1, pageHeight + pageGap);
  const normalizedManualBreakOffsets = Array.from(
    new Set(
      manualBreakOffsets
        .map((offset) => Math.max(0, Math.ceil(offset)))
        .filter((offset) => offset > 0 && offset < safeContentHeight),
    ),
  ).sort((left, right) => left - right);
  const pageBoundaries: number[] = [];
  const guideOffsets: number[] = [];
  let segmentStart = 0;

  const appendSoftBoundaries = (segmentEnd: number) => {
    const segmentHeight = Math.max(0, segmentEnd - segmentStart);
    const segmentPages = Math.max(1, Math.ceil(segmentHeight / pageContentHeight));

    for (let pageIndex = 1; pageIndex < segmentPages; pageIndex += 1) {
      const boundary = segmentStart + pageIndex * pageContentHeight;
      pageBoundaries.push(boundary);
      guideOffsets.push(boundary + pageMargins.top);
    }
  };

  normalizedManualBreakOffsets.forEach((manualBreakOffset) => {
    appendSoftBoundaries(manualBreakOffset);
    pageBoundaries.push(manualBreakOffset);
    segmentStart = manualBreakOffset;
  });

  appendSoftBoundaries(safeContentHeight);

  const totalPages = Math.max(1, pageBoundaries.length + 1);
  const visualPageBoundaries = Array.from(
    { length: Math.max(0, totalPages - 1) },
    (_, index) => (index + 1) * pageStride,
  );
  const visualContentHeight =
    Math.max(0, totalPages - 1) * pageStride + pageContentHeight;

  return {
    contentHeight: safeContentHeight,
    pageContentHeight,
    totalPages,
    guideOffsets,
    pageBoundaries,
    visualPageBoundaries,
    manualBreakOffsets: normalizedManualBreakOffsets,
    pageStride,
    visualContentHeight,
  };
}

function clearBlockShiftStyles(proseMirror: HTMLElement) {
  Array.from(proseMirror.children).forEach((child) => {
    if (!(child instanceof HTMLElement)) {
      return;
    }

    if (child.style.getPropertyValue("--app-editor-block-gap-before")) {
      child.style.removeProperty("--app-editor-block-gap-before");
    }

    if (child.hasAttribute("data-pagination-page")) {
      child.removeAttribute("data-pagination-page");
    }

    if (child.hasAttribute("data-pagination-gap-before")) {
      child.removeAttribute("data-pagination-gap-before");
    }
  });
}

function applyVisualPaginationLayout({
  proseMirror,
  pageHeight,
  pageGap,
  pageContentHeight,
}: {
  proseMirror: HTMLElement;
  pageHeight: number;
  pageGap: number;
  pageContentHeight: number;
}) {
  const pageStride = Math.max(1, pageHeight + pageGap);
  let cumulativeAppliedGap = 0;
  const blocks = Array.from(proseMirror.children)
    .filter((child): child is HTMLElement => child instanceof HTMLElement)
    .map((block, index, siblings) => {
      const nextSibling = siblings[index + 1];
      const blockGapBefore = Number(block.getAttribute("data-pagination-gap-before") ?? "0");
      cumulativeAppliedGap += blockGapBefore;
      const naturalTop = Math.max(0, Math.ceil(block.offsetTop - cumulativeAppliedGap));
      const blockHeight = Math.max(
        1,
        Math.ceil(block.offsetHeight || block.getBoundingClientRect().height || block.scrollHeight || 0),
      );
      const nextSiblingGapBefore =
        nextSibling instanceof HTMLElement
          ? Number(nextSibling.getAttribute("data-pagination-gap-before") ?? "0")
          : 0;
      const nextTop =
        nextSibling instanceof HTMLElement
          ? Math.max(
              naturalTop,
              Math.ceil(nextSibling.offsetTop - (cumulativeAppliedGap + nextSiblingGapBefore)),
            )
          : null;
      const flowHeight = Math.max(blockHeight, nextTop === null ? blockHeight : nextTop - naturalTop);

      return {
        block,
        naturalTop,
        blockHeight,
        flowHeight,
        isPageBreak: block.matches('[data-page-break="true"]'),
      };
    });

  if (blocks.length === 0) {
    clearBlockShiftStyles(proseMirror);
    return {
      totalPagesFromLayout: 1,
      visualContentHeight: 0,
    };
  }

  let currentPage = 0;
  let currentOffsetWithinPage = 0;
  let cumulativeGapBefore = 0;
  let maxVisualBottom = 0;

  blocks.forEach((entry) => {
    const requiresNextPage =
      !entry.isPageBreak &&
      currentOffsetWithinPage > 0 &&
      currentOffsetWithinPage + entry.flowHeight > pageContentHeight &&
      entry.flowHeight <= pageContentHeight;

    if (requiresNextPage) {
      currentPage += 1;
      currentOffsetWithinPage = 0;
    }

    const visualTop = currentPage * pageStride + currentOffsetWithinPage;
    const gapBefore = Math.max(0, Math.round(visualTop - entry.naturalTop - cumulativeGapBefore));

    const gapBeforeValue = `${gapBefore}px`;
    const pageValue = String(currentPage + 1);
    const gapAttributeValue = String(gapBefore);

    if (entry.block.style.getPropertyValue("--app-editor-block-gap-before") !== gapBeforeValue) {
      entry.block.style.setProperty("--app-editor-block-gap-before", gapBeforeValue);
    }

    if (entry.block.getAttribute("data-pagination-page") !== pageValue) {
      entry.block.setAttribute("data-pagination-page", pageValue);
    }

    if (entry.block.getAttribute("data-pagination-gap-before") !== gapAttributeValue) {
      entry.block.setAttribute("data-pagination-gap-before", gapAttributeValue);
    }

    cumulativeGapBefore += gapBefore;

    maxVisualBottom = Math.max(maxVisualBottom, visualTop + entry.blockHeight);

    if (entry.isPageBreak) {
      currentPage += 1;
      currentOffsetWithinPage = 0;
      return;
    }

    currentOffsetWithinPage += entry.flowHeight;
  });

  const totalPagesFromLayout = Math.max(
    currentPage + 1,
    Math.max(1, Math.ceil(maxVisualBottom / pageStride)),
  );
  const minimumVisualContentHeight =
    Math.max(0, totalPagesFromLayout - 1) * pageStride + pageContentHeight;
  const visualContentHeight = Math.max(maxVisualBottom, minimumVisualContentHeight);

  return {
    totalPagesFromLayout,
    visualContentHeight,
  };
}

export function usePaginationMetrics({
  editor,
  enabled,
  pageHeight,
  pageGap,
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

    const nextMetrics = calculatePaginationMetrics({
      contentHeight: proseMirror.scrollHeight,
      pageHeight,
      pageGap,
      pageMargins,
      manualBreakOffsets: Array.from(
        proseMirror.querySelectorAll('[data-page-break="true"]'),
      )
        .filter((element): element is HTMLElement => element instanceof HTMLElement)
        .map((element) => element.offsetTop),
    });
    const layoutResult = applyVisualPaginationLayout({
      proseMirror,
      pageHeight,
      pageGap,
      pageContentHeight: nextMetrics.pageContentHeight,
    });
    const totalPages = Math.max(nextMetrics.totalPages, layoutResult.totalPagesFromLayout);

    commitMetrics({
      ...nextMetrics,
      totalPages,
      visualPageBoundaries: Array.from(
        { length: Math.max(0, totalPages - 1) },
        (_, index) => (index + 1) * nextMetrics.pageStride,
      ),
      visualContentHeight: Math.max(
        layoutResult.visualContentHeight,
        Math.max(0, totalPages - 1) * nextMetrics.pageStride + nextMetrics.pageContentHeight,
      ),
    });
  }, [commitMetrics, containerRef, enabled, pageGap, pageHeight, pageMargins]);

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
      const proseMirror = containerRef.current?.querySelector(".ProseMirror");
      if (proseMirror instanceof HTMLElement) {
        clearBlockShiftStyles(proseMirror);
      }
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

    const imageLoadListeners =
      proseMirror instanceof HTMLElement
        ? Array.from(proseMirror.querySelectorAll("img"))
            .filter((image): image is HTMLImageElement => image instanceof HTMLImageElement)
            .map((image) => {
              const handleImageLoad = () => {
                scheduleMeasure();
              };

              image.addEventListener("load", handleImageLoad, { once: true });

              return {
                image,
                handleImageLoad,
              };
            })
        : [];

    editor?.on("update", scheduleMeasure);

    return () => {
      clearPendingMeasure();
      const proseMirror = containerRef.current?.querySelector(".ProseMirror");
      if (proseMirror instanceof HTMLElement) {
        clearBlockShiftStyles(proseMirror);
      }
      window.removeEventListener("resize", handleResize);
      resizeObserver?.disconnect();
      imageLoadListeners.forEach(({ image, handleImageLoad }) => {
        image.removeEventListener("load", handleImageLoad);
      });
      editor?.off("update", scheduleMeasure);
    };
  }, [
    clearPendingMeasure,
    commitMetrics,
    containerRef,
    editor,
    enabled,
    pageGap,
    scheduleMeasure,
  ]);

  return metrics;
}
