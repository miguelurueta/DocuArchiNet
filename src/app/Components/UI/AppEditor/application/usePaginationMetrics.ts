import { useCallback, useLayoutEffect, useRef, useState } from "react";
import type { RefObject } from "react";
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

type NaturalPaginationStructure = {
  contentHeight: number;
  manualBreakOffsets: number[];
  pageBreakCount: number;
};

type UsePaginationMetricsOptions = {
  enabled: boolean;
  pageHeight: number;
  pageGap: number;
  pageMargins: AppEditorPageMargins;
  containerRef: RefObject<HTMLElement | null>;
  zoomLevel?: number;
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
}: {
  proseMirror: HTMLElement;
  pageHeight: number;
  pageGap: number;
}) {
  const pageStride = Math.max(1, pageHeight + pageGap);
  const blocks = Array.from(proseMirror.children).filter(
    (child): child is HTMLElement => child instanceof HTMLElement,
  );

  if (blocks.length === 0) {
    clearBlockShiftStyles(proseMirror);
    return {
      visualContentHeight: 0,
      visualPageCount: 1,
    };
  }

  let maxVisualBottom = 0;
  let maxVisualPage = 1;

  blocks.forEach((block) => {
    const isAutoPageBreak =
      block.matches('[data-page-break="true"]') &&
      block.getAttribute("data-page-break-auto") === "true";

    if (isAutoPageBreak) {
      if (block.style.getPropertyValue("--app-editor-block-gap-before")) {
        block.style.removeProperty("--app-editor-block-gap-before");
      }

      if (block.hasAttribute("data-pagination-page")) {
        block.removeAttribute("data-pagination-page");
      }

      if (block.hasAttribute("data-pagination-gap-before")) {
        block.removeAttribute("data-pagination-gap-before");
      }

      return;
    }

    const top = Math.max(0, Math.ceil(block.offsetTop));
    const height = Math.max(
      1,
      Math.ceil(block.offsetHeight || block.getBoundingClientRect().height || block.scrollHeight || 0),
    );
    const page = Math.max(1, Math.floor(top / pageStride) + 1);
    const pageValue = String(page);

    if (block.style.getPropertyValue("--app-editor-block-gap-before")) {
      block.style.removeProperty("--app-editor-block-gap-before");
    }

    if (block.getAttribute("data-pagination-page") !== pageValue) {
      block.setAttribute("data-pagination-page", pageValue);
    }

    if (block.getAttribute("data-pagination-gap-before") !== "0") {
      block.setAttribute("data-pagination-gap-before", "0");
    }
    maxVisualBottom = Math.max(maxVisualBottom, top + height);
    maxVisualPage = Math.max(maxVisualPage, page);
  });

  return {
    visualContentHeight: maxVisualBottom,
    visualPageCount: maxVisualPage,
  };
}

export function calculateFixedPageMetrics({
  totalPages,
  pageHeight,
  pageGap = 0,
  pageMargins,
}: {
  totalPages: number;
  pageHeight: number;
  pageGap?: number;
  pageMargins: AppEditorPageMargins;
}): PaginationMetrics {
  const safeTotalPages = Math.max(1, Math.ceil(totalPages));
  const pageContentHeight = Math.max(1, pageHeight - pageMargins.top - pageMargins.bottom);
  const pageStride = Math.max(1, pageHeight + pageGap);

  return {
    contentHeight: safeTotalPages * pageContentHeight,
    pageContentHeight,
    totalPages: safeTotalPages,
    guideOffsets: Array.from(
      { length: Math.max(0, safeTotalPages - 1) },
      (_, index) => (index + 1) * pageContentHeight + pageMargins.top,
    ),
    pageBoundaries: Array.from(
      { length: Math.max(0, safeTotalPages - 1) },
      (_, index) => (index + 1) * pageContentHeight,
    ),
    visualPageBoundaries: Array.from(
      { length: Math.max(0, safeTotalPages - 1) },
      (_, index) => (index + 1) * pageStride,
    ),
    manualBreakOffsets: [],
    pageStride,
    visualContentHeight: Math.max(0, safeTotalPages - 1) * pageStride + pageContentHeight,
  };
}

function collectNaturalPaginationStructure(proseMirror: HTMLElement): NaturalPaginationStructure {
  let cumulativeBreakHeight = 0;
  let naturalContentHeight = 0;
  const manualBreakOffsets: number[] = [];
  let pageBreakCount = 0;

  Array.from(proseMirror.children).forEach((child) => {
    if (!(child instanceof HTMLElement)) {
      return;
    }

    const top = Math.max(0, Math.ceil(child.offsetTop - cumulativeBreakHeight));
    const height = Math.max(
      0,
      Math.ceil(child.offsetHeight || child.getBoundingClientRect().height || child.scrollHeight || 0),
    );

    if (child.matches('[data-page-break="true"]')) {
      pageBreakCount += 1;
      if (child.getAttribute("data-page-break-auto") !== "true") {
        manualBreakOffsets.push(top);
      }
      cumulativeBreakHeight += height;
      return;
    }

    naturalContentHeight = Math.max(naturalContentHeight, top + height);
  });

  return {
    contentHeight: naturalContentHeight,
    manualBreakOffsets,
    pageBreakCount,
  };
}

export function usePaginationMetrics({
  enabled,
  pageHeight,
  pageGap,
  pageMargins,
  containerRef,
  zoomLevel = 1,
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

    const pageWrappers = Array.from(proseMirror.children).filter(
      (child): child is HTMLElement =>
        child instanceof HTMLElement && child.matches('[data-app-editor-page="true"]'),
    );

    if (pageWrappers.length > 0) {
      commitMetrics(
        calculateFixedPageMetrics({
          totalPages: pageWrappers.length,
          pageHeight,
          pageGap,
          pageMargins,
        }),
      );
      return;
    }

    const naturalStructure = collectNaturalPaginationStructure(proseMirror);
    const nextMetrics = calculatePaginationMetrics({
      contentHeight:
        naturalStructure.contentHeight > 0
          ? naturalStructure.contentHeight
          : proseMirror.scrollHeight,
      pageHeight,
      pageGap,
      pageMargins,
      manualBreakOffsets: naturalStructure.manualBreakOffsets,
    });
    const layoutResult = applyVisualPaginationLayout({
      proseMirror,
      pageHeight,
      pageGap,
    });
    const totalPages = Math.max(
      nextMetrics.totalPages,
      naturalStructure.pageBreakCount + 1,
      layoutResult.visualPageCount,
    );

    commitMetrics({
      ...nextMetrics,
      totalPages,
      visualPageBoundaries: Array.from(
        { length: Math.max(0, totalPages - 1) },
        (_, index) => (index + 1) * nextMetrics.pageStride,
      ),
      visualContentHeight: Math.max(
        Math.max(0, totalPages - 1) * nextMetrics.pageStride + nextMetrics.pageContentHeight,
        layoutResult.visualContentHeight,
      ),
    });
  }, [commitMetrics, containerRef, enabled, pageGap, pageHeight, pageMargins]);

  const scheduleMeasure = useCallback((priority: "immediate" | "deferred" = "deferred") => {
    clearPendingMeasure();

    if (priority === "immediate") {
      measure();
      return;
    }

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
      scheduleMeasure("deferred");
    };

    window.addEventListener("resize", handleResize);

    const resizeObserver =
      typeof ResizeObserver !== "undefined"
        ? new ResizeObserver(() => {
            scheduleMeasure("deferred");
          })
        : null;
    const mutationObserver =
      typeof MutationObserver !== "undefined"
        ? new MutationObserver(() => {
            scheduleMeasure("deferred");
          })
        : null;

    const container = containerRef.current;
    const proseMirror = container?.querySelector(".ProseMirror");

    if (container instanceof HTMLElement) {
      resizeObserver?.observe(container);
    }

    if (proseMirror instanceof HTMLElement) {
      resizeObserver?.observe(proseMirror);
      mutationObserver?.observe(proseMirror, {
        childList: true,
        subtree: true,
        attributes: true,
        attributeFilter: ["style", "data-page-break-spacer"],
      });
    }

    const imageLoadListeners =
      proseMirror instanceof HTMLElement
        ? Array.from(proseMirror.querySelectorAll("img"))
            .filter((image): image is HTMLImageElement => image instanceof HTMLImageElement)
            .map((image) => {
              const handleImageLoad = () => {
                scheduleMeasure("deferred");
              };

              image.addEventListener("load", handleImageLoad, { once: true });

              return {
                image,
                handleImageLoad,
              };
            })
        : [];
    const handlePaginationUpdated = () => {
      scheduleMeasure("immediate");
    };

    container?.addEventListener("app-editor-pagination-updated", handlePaginationUpdated as EventListener);

    return () => {
      clearPendingMeasure();
      const proseMirror = containerRef.current?.querySelector(".ProseMirror");
      if (proseMirror instanceof HTMLElement) {
        clearBlockShiftStyles(proseMirror);
      }
      window.removeEventListener("resize", handleResize);
      resizeObserver?.disconnect();
      mutationObserver?.disconnect();
      imageLoadListeners.forEach(({ image, handleImageLoad }) => {
        image.removeEventListener("load", handleImageLoad);
      });
      container?.removeEventListener(
        "app-editor-pagination-updated",
        handlePaginationUpdated as EventListener,
      );
    };
  }, [
    clearPendingMeasure,
    commitMetrics,
    containerRef,
    enabled,
    pageGap,
    scheduleMeasure,
    zoomLevel,
  ]);

  return metrics;
}
