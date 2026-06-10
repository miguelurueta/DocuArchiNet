import type { Editor } from "@tiptap/react";
import type { AppEditorPageMargins } from "../domain/editor.types";

export const AUTO_PAGE_BREAK_SAFETY_MARGIN = 12;
const MIN_PAGE_NUMBER = 1;
const MIN_ZOOM_LEVEL = 0.1;
const MIN_BLOCK_HEIGHT = 1;
const DEFAULT_PAGE_GAP = 1;

export type VisualPage = {
  pageNumber: number;
  top: number;
  bottom: number;
  contentTop: number;
  contentBottom: number;
  startBlockIndex: number;
  endBlockIndex: number;
};

export type VisualPagination = {
  pages: VisualPage[];
};

export type AutoPaginationInput = {
  editor: Editor;
  proseMirror: HTMLElement;
  pageContentHeight: number;
  pageHeight: number;
  pageStride: number;
  pageMargins: AppEditorPageMargins;
  zoomLevel?: number;
  startChildIndex?: number;
};

function resolveBlockPosition(editor: Editor, childIndex: number) {
  let blockPosition = 0;

  for (let index = 0; index < editor.state.doc.childCount; index += 1) {
    const blockNode = editor.state.doc.child(index);

    if (index === childIndex) {
      return { blockPosition, blockNode };
    }

    blockPosition += blockNode.nodeSize;
  }

  return null;
}

export function resolveTopLevelChildIndexFromPosition(editor: Editor, position: number) {
  const safePosition = Math.max(0, Math.min(position, editor.state.doc.content.size));
  const resolvedPosition = editor.state.doc.resolve(safePosition);

  return Math.max(
    0,
    Math.min(resolvedPosition.index(0), Math.max(0, editor.state.doc.childCount - 1)),
  );
}

export function resolveTopLevelBlockStartPosition(editor: Editor, childIndex: number) {
  return resolveBlockPosition(editor, childIndex)?.blockPosition ?? 0;
}

function clampToPositiveInt(value: number) {
  return Number.isFinite(value) ? Math.max(0, Math.floor(value)) : 0;
}

function resolveContentTopOffset({
  block,
  zoomLevel,
}: {
  block: HTMLElement;
  zoomLevel: number;
}) {
  const safeZoom = Math.max(MIN_ZOOM_LEVEL, zoomLevel);
  const rect = block.getBoundingClientRect();
  const rawHeight = Math.max(
    0,
    Number.isFinite(block.offsetHeight) ? block.offsetHeight / safeZoom : 0,
    Number.isFinite(rect.height / safeZoom) ? rect.height / safeZoom : 0,
  );

  return {
    top: Number.isFinite(rect.top / safeZoom) ? rect.top / safeZoom : 0,
    height: Math.max(MIN_BLOCK_HEIGHT, rawHeight),
  };
}

function createPageRecord(
  pageNumber: number,
  pageHeight: number,
  pageStride: number,
  pageMargins: AppEditorPageMargins,
  blockIndex: number,
) {
  const safePageNumber = Math.max(MIN_PAGE_NUMBER, Math.floor(pageNumber));
  const safePageHeight = Math.max(1, Math.floor(pageHeight));
  const pageTop = getPageBaseOffset(safePageNumber, pageStride);
  const pageBottom = pageTop + safePageHeight;
  const contentTop = pageTop + Math.max(0, pageMargins.top);

  return {
    pageNumber: safePageNumber,
    top: pageTop,
    bottom: pageBottom,
    contentTop,
    contentBottom: Math.max(contentTop, pageBottom - Math.max(0, pageMargins.bottom)),
    startBlockIndex: blockIndex,
    endBlockIndex: blockIndex,
  };
}

function getPageIndexFromOffset(offset: number, pageStride: number) {
  return Math.max(MIN_PAGE_NUMBER, Math.floor(Math.max(0, offset) / Math.max(1, pageStride)) + 1);
}

function getPageBaseOffset(pageNumber: number, pageStride: number) {
  return (Math.max(MIN_PAGE_NUMBER, Math.floor(pageNumber)) - 1) * Math.max(1, pageStride);
}

function clampOffset(value: number) {
  if (!Number.isFinite(value)) {
    return 0;
  }

  return Math.max(0, value);
}

function resolveNormalizedBlockHeight(height: number) {
  if (!Number.isFinite(height) || height <= 0) {
    return MIN_BLOCK_HEIGHT;
  }

  return Math.max(MIN_BLOCK_HEIGHT, Math.floor(height));
}

function computePagePlacement({
  requestedOffset,
  blockHeight,
  pageContentHeight,
  pageStride,
}: {
  requestedOffset: number;
  blockHeight: number;
  pageContentHeight: number;
  pageStride: number;
}) {
  const safePageContentHeight = Math.max(1, Math.floor(pageContentHeight));
  const safeStride = Math.max(
    Math.max(1, Math.floor(pageStride)),
    safePageContentHeight + DEFAULT_PAGE_GAP,
  );

  let startOffset = clampOffset(requestedOffset);
  const currentPage = getPageIndexFromOffset(startOffset, safeStride);
  const currentPageBase = getPageBaseOffset(currentPage, safeStride);
  const currentPageOffset = clampOffset(startOffset - currentPageBase);

  const blockFitsCurrentPage =
    blockHeight <= safePageContentHeight &&
    currentPageOffset + blockHeight + AUTO_PAGE_BREAK_SAFETY_MARGIN <= safePageContentHeight;

  if (!blockFitsCurrentPage && currentPageOffset > 0) {
    startOffset = getPageBaseOffset(currentPage + 1, safeStride);
  }

  const startPage = getPageIndexFromOffset(startOffset, safeStride);
  const startPageBase = getPageBaseOffset(startPage, safeStride);
  const pageOffsetTop = clampOffset(startOffset - startPageBase);
  const pageOffsetBottom = Math.min(safePageContentHeight, pageOffsetTop + blockHeight);
  const endOffset = clampOffset(startOffset + blockHeight);
  const endPage = getPageIndexFromOffset(Math.max(0, endOffset - 0.0001), safeStride);

  return {
    startOffset,
    endOffset,
    startPage,
    endPage,
    pageOffsetTop,
    pageOffsetBottom,
    safeStride,
  };
}

function ensurePageRecord({
  pageRecords,
  pageNumber,
  pageHeight,
  pageStride,
  pageMargins,
  blockIndex,
}: {
  pageRecords: Map<number, VisualPage>;
  pageNumber: number;
  pageHeight: number;
  pageStride: number;
  pageMargins: AppEditorPageMargins;
  blockIndex: number;
}) {
  const safePageNumber = Math.max(MIN_PAGE_NUMBER, Math.floor(pageNumber));
  const existing = pageRecords.get(safePageNumber);

  if (existing) {
    existing.startBlockIndex = Math.min(existing.startBlockIndex, blockIndex);
    existing.endBlockIndex = Math.max(existing.endBlockIndex, blockIndex);
    return;
  }

  pageRecords.set(
    safePageNumber,
    createPageRecord(safePageNumber, pageHeight, pageStride, pageMargins, blockIndex),
  );
}

export function resolveAutoPageBreakActions({
  editor,
  proseMirror,
  pageContentHeight,
  pageHeight,
  pageStride,
  pageMargins,
  zoomLevel = 1,
  startChildIndex = 0,
}: AutoPaginationInput): VisualPagination {
  const safePageContentHeight = Math.max(1, Math.floor(pageContentHeight));
  const safePageHeight = Math.max(1, Math.floor(pageHeight));
  const safePageStride = Math.max(
    Math.floor(pageStride),
    safePageHeight + DEFAULT_PAGE_GAP,
  );

  const blocks = Array.from(proseMirror.children).filter(
    (child): child is HTMLElement => child instanceof HTMLElement,
  );

  const safeStartChildIndex = Math.max(0, clampToPositiveInt(startChildIndex));

  const fallbackPages: VisualPage[] = [
    createPageRecord(MIN_PAGE_NUMBER, safePageHeight, safePageStride, pageMargins, 0),
  ];

  if (blocks.length === 0) {
    return {
      pages: fallbackPages,
    };
  }

  const blockInfos = blocks
    .map((block, blockIndex) => {
      const { height, top } = resolveContentTopOffset({
        block,
        zoomLevel,
      });

      return {
        block,
        blockIndex,
        top,
        height,
      };
    })
    .filter(({ blockIndex }) => blockIndex >= safeStartChildIndex)
    .sort((left, right) => {
      if (left.top === right.top) {
        return left.blockIndex - right.blockIndex;
      }

      return left.top - right.top;
    });

  if (blockInfos.length === 0) {
    return {
      pages: fallbackPages,
    };
  }

  const pagesByNumber = new Map<number, VisualPage>();
  let maxPageNumber = MIN_PAGE_NUMBER;
  let visualCursor = 0;

  for (const info of blockInfos) {
    const blockNode = resolveBlockPosition(editor, info.blockIndex)?.blockNode;
    if (!blockNode) {
      continue;
    }

    const blockHeight = resolveNormalizedBlockHeight(info.height);
    const requestedOffset = Math.max(0, visualCursor);

    const placement = computePagePlacement({
      requestedOffset,
      blockHeight,
      pageContentHeight: safePageContentHeight,
      pageStride: safePageStride,
    });

    for (let pageNumber = placement.startPage; pageNumber <= placement.endPage; pageNumber += 1) {
      ensurePageRecord({
        pageRecords: pagesByNumber,
        pageNumber,
        pageHeight: safePageHeight,
        pageStride: safePageStride,
        pageMargins,
        blockIndex: info.blockIndex,
      });
      maxPageNumber = Math.max(maxPageNumber, pageNumber);
    }

    visualCursor = Math.max(visualCursor, placement.endOffset);
  }

  const sortedPages = Array.from(pagesByNumber.values()).sort(
    (left, right) => left.pageNumber - right.pageNumber,
  );
  const normalizedPages: VisualPage[] = [];
  let previousEndBlockIndex = blockInfos[0]?.blockIndex ?? 0;

  for (let pageNumber = MIN_PAGE_NUMBER; pageNumber <= maxPageNumber; pageNumber += 1) {
    const existingPage = sortedPages.find((page) => page.pageNumber === pageNumber);
    const fallbackPage = createPageRecord(
      pageNumber,
      safePageHeight,
      safePageStride,
      pageMargins,
      previousEndBlockIndex,
    );

    if (!existingPage) {
      normalizedPages.push(fallbackPage);
      continue;
    }

    existingPage.startBlockIndex = Math.max(existingPage.startBlockIndex, previousEndBlockIndex);
    existingPage.endBlockIndex = Math.max(existingPage.endBlockIndex, existingPage.startBlockIndex);

    normalizedPages.push({
      ...existingPage,
      endBlockIndex: existingPage.endBlockIndex,
    });

    previousEndBlockIndex = existingPage.endBlockIndex;
  }

  return {
    pages: normalizedPages.length > 0 ? normalizedPages : fallbackPages,
  };
}
