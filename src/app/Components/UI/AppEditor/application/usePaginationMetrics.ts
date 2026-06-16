import { useCallback, useLayoutEffect, useRef, useState } from "react";
import type { RefObject } from "react";
import type { AppEditorPageMargins } from "../domain/editor.types";
import type { VisualPage } from "./autoPagination";

type PaginationMetrics = {
  contentHeight: number;
  pageContentHeight: number;
  totalPages: number;
  pages: VisualPage[];
  guideOffsets: number[];
  pageBoundaries: number[];
  visualPageBoundaries: number[];
  pageStride: number;
  visualContentHeight: number;
};

type NaturalPaginationStructure = {
  contentHeight: number;
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
  pages: [],
  guideOffsets: [],
  pageBoundaries: [],
  visualPageBoundaries: [],
  pageStride: 0,
  visualContentHeight: 0,
};

function areMetricsEqual(left: PaginationMetrics, right: PaginationMetrics) {
  return (
    left.contentHeight === right.contentHeight &&
    left.pageContentHeight === right.pageContentHeight &&
    left.totalPages === right.totalPages &&
    left.pages.length === right.pages.length &&
    left.pages.every((page, index) => {
      const rightPage = right.pages[index];
      return (
        rightPage !== undefined &&
        page.pageNumber === rightPage.pageNumber &&
        page.top === rightPage.top &&
        page.bottom === rightPage.bottom &&
        page.contentTop === rightPage.contentTop &&
        page.contentBottom === rightPage.contentBottom &&
        page.startBlockIndex === rightPage.startBlockIndex &&
        page.endBlockIndex === rightPage.endBlockIndex
      );
    }) &&
    left.pageBoundaries.length === right.pageBoundaries.length &&
    left.pageBoundaries.every((offset, index) => offset === right.pageBoundaries[index]) &&
    left.visualPageBoundaries.length === right.visualPageBoundaries.length &&
    left.visualPageBoundaries.every((offset, index) => offset === right.visualPageBoundaries[index]) &&
    left.guideOffsets.length === right.guideOffsets.length &&
    left.guideOffsets.every((offset, index) => offset === right.guideOffsets[index]) &&
    left.pageStride === right.pageStride &&
    left.visualContentHeight === right.visualContentHeight
  );
}

function roundPositive(value: number) {
  return Number.isFinite(value) ? Math.max(0, Math.ceil(value)) : 0;
}

function buildVisualPagesFromCount({
  totalPages,
  pageHeight,
  pageStride,
  pageMargins,
}: {
  totalPages: number;
  pageHeight: number;
  pageStride: number;
  pageMargins: AppEditorPageMargins;
}): VisualPage[] {
  const safeTotalPages = Math.max(1, Math.ceil(totalPages));
  const safePageHeight = Math.max(1, roundPositive(pageHeight));
  const safePageStride = Math.max(1, roundPositive(pageStride));

  return Array.from({ length: safeTotalPages }, (_, index) => {
    const top = index * safePageStride;
    const bottom = top + safePageHeight;
    const contentTop = top + Math.max(0, pageMargins.top);

    return {
      pageNumber: index + 1,
      top,
      bottom,
      contentTop,
      contentBottom: Math.max(contentTop, bottom - Math.max(0, pageMargins.bottom)),
      startBlockIndex: 0,
      endBlockIndex: 0,
    };
  });
}

export function calculatePaginationMetrics({
  contentHeight,
  pageHeight,
  pageGap = 0,
  pageMargins,
}: {
  contentHeight: number;
  pageHeight: number;
  pageGap?: number;
  pageMargins: AppEditorPageMargins;
}) {
  const safeContentHeight = roundPositive(contentHeight);
  const pageContentHeight = Math.max(1, roundPositive(pageHeight - pageMargins.top - pageMargins.bottom));
  const pageStride = Math.max(1, roundPositive(pageHeight + pageGap));

  const pageBoundaries: number[] = [];
  const guideOffsets: number[] = [];
  const segmentStart = 0;

  const appendSoftBoundaries = (segmentEnd: number) => {
    const segmentHeight = Math.max(0, segmentEnd - segmentStart);
    const segmentPages = Math.max(1, Math.ceil(segmentHeight / pageContentHeight));

    for (let pageIndex = 1; pageIndex < segmentPages; pageIndex += 1) {
      const boundary = segmentStart + pageIndex * pageContentHeight;
      pageBoundaries.push(boundary);
      guideOffsets.push(boundary + pageMargins.top);
    }
  };

  appendSoftBoundaries(safeContentHeight);

  const totalPages = Math.max(1, pageBoundaries.length + 1);
  const visualPageBoundaries = Array.from(
    { length: Math.max(0, totalPages - 1) },
    (_, index) => (index + 1) * pageStride,
  );
  const visualContentHeight = Math.max(0, totalPages - 1) * pageStride + pageHeight;
  const pages = buildVisualPagesFromCount({
    totalPages,
    pageHeight,
    pageStride,
    pageMargins,
  });

  return {
    contentHeight: safeContentHeight,
    pageContentHeight,
    totalPages,
    pages,
    guideOffsets,
    pageBoundaries,
    visualPageBoundaries,
    pageStride,
    visualContentHeight,
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
}) {
  const safeTotalPages = Math.max(1, Math.ceil(totalPages));
  const pageContentHeight = Math.max(
    1,
    roundPositive(pageHeight - pageMargins.top - pageMargins.bottom),
  );
  const pageStride = Math.max(1, roundPositive(pageHeight + pageGap));
  const pages = buildVisualPagesFromCount({
    totalPages: safeTotalPages,
    pageHeight,
    pageStride,
    pageMargins,
  });

  return {
    contentHeight: safeTotalPages * pageContentHeight,
    pageContentHeight,
    totalPages: safeTotalPages,
    pages,
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
    pageStride,
    visualContentHeight: Math.max(0, safeTotalPages - 1) * pageStride + pageHeight,
  };
}

function collectNaturalPaginationStructure(proseMirror: HTMLElement): NaturalPaginationStructure {
  let naturalContentHeight = 0;

  Array.from(proseMirror.children).forEach((child) => {
    if (!(child instanceof HTMLElement)) {
      return;
    }

    const top = roundPositive(child.offsetTop);
    const height = Math.max(
      0,
      roundPositive(child.offsetHeight || child.getBoundingClientRect().height || child.scrollHeight || 0),
    );

    naturalContentHeight = Math.max(naturalContentHeight, top + height);
  });

  return {
    contentHeight: naturalContentHeight,
  };
}

function clampVisualPageCount(value: number, fallback: number) {
  const normalized = roundPositive(value);
  return normalized > 0 ? normalized : fallback;
}

function isVisualPage(value: unknown): value is VisualPage {
  if (!value || typeof value !== "object") {
    return false;
  }

  const candidate = value as Partial<Record<string, unknown>>;

  return (
    typeof candidate.pageNumber === "number" &&
    Number.isFinite(candidate.pageNumber) &&
    typeof candidate.top === "number" &&
    Number.isFinite(candidate.top) &&
    typeof candidate.bottom === "number" &&
    Number.isFinite(candidate.bottom) &&
    typeof candidate.contentTop === "number" &&
    Number.isFinite(candidate.contentTop) &&
    typeof candidate.contentBottom === "number" &&
    Number.isFinite(candidate.contentBottom) &&
    typeof candidate.startBlockIndex === "number" &&
    Number.isFinite(candidate.startBlockIndex) &&
    typeof candidate.endBlockIndex === "number" &&
    Number.isFinite(candidate.endBlockIndex)
  );
}

type AutoPaginationEventDetail = {
  pages?: Array<unknown> | null;
  pageContentHeight?: number;
  pageStride?: number;
  pageCount?: number;
};

function normalizeAutoPaginationPayload(
  detail: unknown,
  fallbackPageCount: number,
): {
  pages: VisualPage[];
  pageCount: number;
  pageContentHeight: number;
  pageStride: number;
} | null {
  if (!detail || typeof detail !== "object") {
    return null;
  }

  const candidate = detail as Partial<AutoPaginationEventDetail>;
  const pagesInput = Array.isArray(candidate.pages) ? candidate.pages : [];
  const normalizedPages = pagesInput.filter(isVisualPage);
  const maxPageFromPayload =
    normalizedPages.length > 0
      ? Math.max(...normalizedPages.map((page) => page.pageNumber))
      : 0;
  const safePageCount = clampVisualPageCount(candidate.pageCount ?? 0, fallbackPageCount);
  const pageCount = Math.max(
    normalizedPages.length,
    maxPageFromPayload,
    safePageCount,
    1,
  );
  const pageContentHeight = roundPositive(candidate.pageContentHeight ?? 0);
  const pageStride = roundPositive(candidate.pageStride ?? 0);

  if (normalizedPages.length === 0 && pageCount <= 1 && pageContentHeight === 0 && pageStride === 0) {
    return null;
  }

  return {
    pages: normalizedPages,
    pageCount,
    pageContentHeight,
    pageStride,
  };
}

function buildMetricsFromAutoPagination({
  pages,
  pageCount,
  fallbackPageContentHeight,
  fallbackPageHeight,
  fallbackPageGap,
  pageMargins,
  pageStride,
  pageContentHeight,
}: {
  pages: VisualPage[];
  pageCount: number;
  fallbackPageContentHeight: number;
  fallbackPageHeight: number;
  fallbackPageGap: number;
  pageMargins: AppEditorPageMargins;
  pageStride: number;
  pageContentHeight: number;
}) {
  void fallbackPageContentHeight;
  const resolvedPageCount = clampVisualPageCount(pageCount, 1);
  const fallbackPageHeightByGap = Math.max(
    1,
    fallbackPageHeight + fallbackPageGap - pageMargins.top - pageMargins.bottom,
  );
  const pageStrideValue = Math.max(1, pageStride);
  const pageContentHeightValue = pageContentHeight > 0 ? pageContentHeight : fallbackPageHeightByGap;
  const pageHeightFromContent = pageContentHeightValue + pageMargins.top + pageMargins.bottom;
  const fixed = calculateFixedPageMetrics({
    totalPages: resolvedPageCount,
    pageHeight: pageHeightFromContent,
    pageGap: fallbackPageGap,
    pageMargins,
  });

  const pageByNumber = new Map<number, VisualPage>();
  pages.forEach((page) => {
    const safePageNumber = Math.max(1, Math.floor(page.pageNumber));
    pageByNumber.set(safePageNumber, page);
  });

  const pageBoundaries = Array.from({ length: Math.max(0, resolvedPageCount - 1) }, (_, index) => {
    const pageNumber = index + 2;
    return roundPositive(
      (pageByNumber.get(pageNumber)?.top ?? (index + 1) * pageStrideValue),
    );
  });

  const visualBottom = pages.length > 0
    ? Math.max(...pages.map((page) => page.bottom))
    : fixed.visualContentHeight;
  const visualContentHeight = Math.max(
    fixed.visualContentHeight,
    Math.max(0, roundPositive(visualBottom)),
  );

  return {
    contentHeight: Math.max(0, visualContentHeight - pageMargins.top - pageMargins.bottom),
    pageContentHeight: Math.max(1, fixed.pageContentHeight),
    totalPages: resolvedPageCount,
    pages: pages.length > 0
      ? pages
      : buildVisualPagesFromCount({
          totalPages: resolvedPageCount,
          pageHeight: pageHeightFromContent,
          pageStride: pageStrideValue,
          pageMargins,
        }),
    guideOffsets: pageBoundaries.map((boundary) => boundary + pageMargins.top),
    pageBoundaries,
    visualPageBoundaries: pageBoundaries,
    pageStride: pageStrideValue,
    visualContentHeight,
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
  void zoomLevel;
  const latestAutoPagesRef = useRef<{
    pages: VisualPage[];
    pageCount: number;
    pageContentHeight: number;
    pageStride: number;
  } | null>(null);

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

    const autoPaginationState = latestAutoPagesRef.current;
    if (autoPaginationState && autoPaginationState.pages.length > 0) {
      commitMetrics(
        buildMetricsFromAutoPagination({
          pages: autoPaginationState.pages,
          pageCount: autoPaginationState.pageCount,
          fallbackPageContentHeight: Math.max(
            1,
            roundPositive(pageHeight - pageMargins.top - pageMargins.bottom),
          ),
          fallbackPageHeight: pageHeight,
          fallbackPageGap: pageGap,
          pageMargins,
          pageStride: autoPaginationState.pageStride,
          pageContentHeight: autoPaginationState.pageContentHeight,
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
    });
    const totalPages = nextMetrics.totalPages;
    const fixedMetrics = calculateFixedPageMetrics({
      totalPages,
      pageHeight,
      pageGap,
      pageMargins,
    });

    commitMetrics({
      ...nextMetrics,
      totalPages,
      pages: fixedMetrics.pages,
      visualContentHeight: Math.max(nextMetrics.visualContentHeight, fixedMetrics.visualContentHeight),
      pageStride: fixedMetrics.pageStride,
      pageBoundaries: fixedMetrics.pageBoundaries,
      visualPageBoundaries: fixedMetrics.visualPageBoundaries,
      guideOffsets: fixedMetrics.guideOffsets,
    });
  }, [
    commitMetrics,
    containerRef,
    enabled,
    pageGap,
    pageHeight,
    pageMargins,
  ]);

  const scheduleMeasure = useCallback(
    (priority: "immediate" | "deferred" = "deferred") => {
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
    },
    [clearPendingMeasure, debounceMs, measure],
  );

  useLayoutEffect(() => {
    if (!enabled) {
      clearPendingMeasure();
      latestAutoPagesRef.current = null;
      return undefined;
    }

    const handleResize = () => {
      scheduleMeasure("deferred");
    };

    const resizeObserver =
      typeof ResizeObserver !== "undefined"
        ? new ResizeObserver(() => {
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

    const handlePaginationUpdated = (event: Event) => {
      const detail = (event as CustomEvent).detail as unknown;
      const resolved = normalizeAutoPaginationPayload(detail, 1);

      if (!resolved) {
        return;
      }

      latestAutoPagesRef.current = {
        pages: resolved.pages,
        pageCount: resolved.pageCount,
        pageContentHeight: resolved.pageContentHeight,
        pageStride: resolved.pageStride,
      };
      scheduleMeasure("immediate");
    };

    container?.addEventListener(
      "app-editor-pagination-updated",
      handlePaginationUpdated as EventListener,
    );
    window.addEventListener("resize", handleResize);

    const initialMeasureTimer = window.setTimeout(() => {
      scheduleMeasure("deferred");
    }, 0);

    return () => {
      window.clearTimeout(initialMeasureTimer);
      clearPendingMeasure();
      latestAutoPagesRef.current = null;
      window.removeEventListener("resize", handleResize);
      resizeObserver?.disconnect();
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

  return enabled ? metrics : DEFAULT_METRICS;
}
