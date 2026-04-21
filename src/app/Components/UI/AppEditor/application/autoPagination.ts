import type { Editor } from "@tiptap/react";

export const AUTO_PAGE_BREAK_SAFETY_MARGIN = 12;
const PAGE_BOUNDARY_TOLERANCE = 2;
const SPLIT_SEARCH_WINDOW = 32;
const MIN_LEADING_TEXT_CHARS = 24;
const MIN_TRAILING_TEXT_CHARS = 24;

export type AutoPageBreakAction =
  | {
      type: "before";
      position: number;
    }
  | {
      type: "split";
      position: number;
    };

function resolveBlockPosition(editor: Editor, childIndex: number) {
  let blockPosition = 0;

  for (let index = 0; index < editor.state.doc.childCount; index += 1) {
    const blockNode = editor.state.doc.child(index);

    if (index === childIndex) {
      if (blockNode.content.size === 0) {
        return null;
      }

      return {
        blockPosition,
        blockNode,
      };
    }

    blockPosition += blockNode.nodeSize;
  }

  return null;
}

function resolveTextBlockPosition(editor: Editor, childIndex: number) {
  const blockPositionInfo = resolveTopLevelBlockPosition(editor, childIndex);

  if (!blockPositionInfo?.blockNode.isTextblock) {
    return null;
  }

  return {
    ...blockPositionInfo,
    textStart: blockPositionInfo.blockPosition + 1,
    textEnd: blockPositionInfo.blockPosition + blockPositionInfo.blockNode.nodeSize - 1,
  };
}

function clampCandidatePosition({
  textStart,
  textEnd,
  candidate,
}: {
  textStart: number;
  textEnd: number;
  candidate: number;
}) {
  return Math.min(
    Math.max(candidate, textStart + MIN_LEADING_TEXT_CHARS),
    textEnd - MIN_TRAILING_TEXT_CHARS,
  );
}

function resolveWordBoundarySplitPosition({
  editor,
  textStart,
  textEnd,
  candidate,
}: {
  editor: Editor;
  textStart: number;
  textEnd: number;
  candidate: number;
}) {
  const normalizedCandidate = clampCandidatePosition({
    textStart,
    textEnd,
    candidate,
  });

  if (normalizedCandidate <= textStart || normalizedCandidate >= textEnd) {
    return null;
  }

  const blockText = editor.state.doc.textBetween(textStart, textEnd, "", "");
  const relativeCandidate = normalizedCandidate - textStart;
  const backwardLimit = Math.max(MIN_LEADING_TEXT_CHARS, relativeCandidate - SPLIT_SEARCH_WINDOW);

  for (let index = relativeCandidate; index >= backwardLimit; index -= 1) {
    if (/\s/.test(blockText[index] ?? "")) {
      const splitPosition = textStart + index;
      if (splitPosition > textStart && splitPosition < textEnd) {
        return splitPosition;
      }
    }
  }

  return normalizedCandidate;
}

function resolveContentPageIndex(
  offset: number,
  pageStride: number,
  pageContentHeight: number,
) {
  const nonContentHeight = Math.max(0, pageStride - pageContentHeight);

  return Math.max(
    0,
    Math.floor((offset + nonContentHeight + PAGE_BOUNDARY_TOLERANCE) / pageStride),
  );
}

function resolveSplitPositionForBoundary({
  editor,
  proseMirrorRect,
  blockTop,
  blockBottom,
  textStart,
  textEnd,
  searchStart,
  targetBoundary,
  zoomLevel,
}: {
  editor: Editor;
  proseMirrorRect: DOMRect;
  blockTop: number;
  blockBottom: number;
  textStart: number;
  textEnd: number;
  searchStart: number;
  targetBoundary: number;
  zoomLevel: number;
}) {
  const availableHeight = Math.max(1, targetBoundary - blockTop);
  const blockHeight = Math.max(1, blockBottom - blockTop);
  const searchableSpan = Math.max(1, textEnd - searchStart);
  const roughRatio = Math.min(Math.max(availableHeight / blockHeight, 0.05), 0.95);
  const roughCandidate = clampCandidatePosition({
    textStart,
    textEnd,
    candidate: searchStart + Math.floor(searchableSpan * roughRatio),
  });

  let low = searchStart;
  let high = textEnd;
  let candidate = roughCandidate;

  const roughPositionRect = editor.view.coordsAtPos(roughCandidate);
  const roughRelativeBottom = Math.max(0, (roughPositionRect.bottom - proseMirrorRect.top) / zoomLevel);

  if (roughRelativeBottom >= targetBoundary) {
    high = roughCandidate;
  } else {
    low = roughCandidate;
    candidate = textEnd;
  }

  while (low <= high) {
    const middle = Math.floor((low + high) / 2);
    const positionRect = editor.view.coordsAtPos(middle);
    const relativeBottom = Math.max(0, (positionRect.bottom - proseMirrorRect.top) / zoomLevel);

    if (relativeBottom >= targetBoundary) {
      candidate = middle;
      high = middle - 1;
    } else {
      low = middle + 1;
    }
  }

  return resolveWordBoundarySplitPosition({
    editor,
    textStart,
    textEnd,
    candidate,
  });
}

export function syncAutoPageBreakSpacerHeights(
  editor: Editor,
  proseMirror: HTMLElement,
  pageStride: number,
) {
  let cumulativeAutoSpacerHeight = 0;
  let previousNaturalBottom = 0;
  let transaction = editor.state.tr;
  let hasChanges = false;

  Array.from(proseMirror.children).forEach((child) => {
    if (!(child instanceof HTMLElement)) {
      return;
    }

    const rawPosition = editor.view.posAtDOM(child, 0);
    const rawNode = editor.state.doc.nodeAt(rawPosition);
    const position = rawNode?.isText && rawPosition > 0 ? rawPosition - 1 : rawPosition;
    const node = editor.state.doc.nodeAt(position);
    const previousSpacerHeight =
      node?.type.name === "pageBreak" && typeof node.attrs.spacerHeight === "number"
        ? node.attrs.spacerHeight
        : 0;
    const naturalTop = Math.max(0, child.offsetTop - cumulativeAutoSpacerHeight);
    const naturalHeight = Math.max(
      0,
      child.offsetHeight || child.getBoundingClientRect().height || child.scrollHeight || 0,
    );
    const naturalBottom = naturalTop + naturalHeight;

    if (child.matches('[data-page-break="true"]')) {
      const previousVisualBottom = previousNaturalBottom + cumulativeAutoSpacerHeight;
      const remainder = previousVisualBottom % pageStride;
      const spacerHeight = remainder === 0 ? 0 : Math.max(0, pageStride - remainder);

      if (node?.type.name === "pageBreak" && previousSpacerHeight !== spacerHeight) {
        transaction = transaction.setNodeMarkup(position, undefined, {
          ...node.attrs,
          spacerHeight,
        });
        hasChanges = true;
      }

      cumulativeAutoSpacerHeight += spacerHeight;
      return;
    }

    previousNaturalBottom = naturalBottom;
  });

  if (hasChanges) {
    editor.view.dispatch(transaction);
  }
}

export function removeAutoPageBreaks(editor: Editor) {
  const positionsToRemove: number[] = [];

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name !== "pageBreak" || node.attrs.auto !== true) {
      return;
    }

    positionsToRemove.push(pos);
  });

  if (positionsToRemove.length === 0) {
    return false;
  }

  let transaction = editor.state.tr;

  positionsToRemove
    .sort((left, right) => right - left)
    .forEach((position) => {
      const pageBreakNode = transaction.doc.nodeAt(position);

      if (!pageBreakNode || pageBreakNode.type.name !== "pageBreak") {
        return;
      }

      const resolvedPosition = transaction.doc.resolve(position);
      const parent = resolvedPosition.parent;
      const index = resolvedPosition.index();
      const previousNode = index > 0 ? parent.child(index - 1) : null;
      const nextNode = index < parent.childCount - 1 ? parent.child(index + 1) : null;

      if (
        pageBreakNode.attrs.mergeOnRemove === true &&
        previousNode &&
        nextNode &&
        previousNode.isTextblock &&
        nextNode.isTextblock &&
        previousNode.sameMarkup(nextNode)
      ) {
        const previousStart = position - previousNode.nodeSize;
        const nextEnd = position + pageBreakNode.nodeSize + nextNode.nodeSize;
        const mergedNode = previousNode.type.create(
          previousNode.attrs,
          previousNode.content.append(nextNode.content),
          previousNode.marks,
        );

        transaction = transaction.replaceWith(previousStart, nextEnd, mergedNode);
        return;
      }

      transaction = transaction.delete(position, position + pageBreakNode.nodeSize);
    });

  editor.view.dispatch(transaction);
  return true;
}

export function resolveAutoPageBreakActions({
  editor,
  proseMirror,
  pageContentHeight,
  pageStride,
  safetyMargin = AUTO_PAGE_BREAK_SAFETY_MARGIN,
  zoomLevel = 1,
}: {
  editor: Editor;
  proseMirror: HTMLElement;
  pageContentHeight: number;
  pageStride: number;
  safetyMargin?: number;
  zoomLevel?: number;
}): AutoPageBreakAction[] {
  const proseMirrorRect = proseMirror.getBoundingClientRect();
  const actions: AutoPageBreakAction[] = [];
  const topLevelChildren = Array.from(proseMirror.children);
  const safeZoomLevel = Math.max(0.1, zoomLevel);

  for (const [childIndex, child] of topLevelChildren.entries()) {
    if (!(child instanceof HTMLElement) || child.matches('[data-page-break="true"]')) {
      continue;
    }

    const block = child;
    const blockRect = block.getBoundingClientRect();
    const blockTop = Math.max(0, (blockRect.top - proseMirrorRect.top) / safeZoomLevel);
    const blockBottom = Math.max(0, (blockRect.bottom - proseMirrorRect.top) / safeZoomLevel);
    const blockHeight = Math.max(1, blockBottom - blockTop);
    const blockPage = resolveContentPageIndex(blockTop, pageStride, pageContentHeight);
    const pageEnd = blockPage * pageStride + pageContentHeight;
    const allowedBlockBottom = pageEnd - safetyMargin;
    const overflowTolerance = 2;

    if (blockBottom <= allowedBlockBottom + overflowTolerance) {
      continue;
    }

    const blockPositionInfo = resolveTopLevelBlockPosition(editor, childIndex);
    if (!blockPositionInfo) {
      continue;
    }

    const { blockPosition, blockNode } = blockPositionInfo;

    if (blockHeight <= pageContentHeight) {
      if (blockPosition > 0 && actions[actions.length - 1]?.position !== blockPosition) {
        actions.push({
          type: "before",
          position: blockPosition,
        });
      }
      continue;
    }

    if (!blockNode.isTextblock) {
      continue;
    }

    const textBlockPositionInfo = resolveTextBlockPosition(editor, childIndex);
    if (!textBlockPositionInfo) {
      continue;
    }

    const { textStart, textEnd } = textBlockPositionInfo;

    let searchStart = textStart + MIN_LEADING_TEXT_CHARS;
    const firstOverflowPage = resolveContentPageIndex(blockTop, pageStride, pageContentHeight);
    const lastOverflowPage = Math.max(
      firstOverflowPage,
      resolveContentPageIndex(Math.max(blockTop, blockBottom - 1), pageStride, pageContentHeight),
    );

    for (let pageIndex = firstOverflowPage; pageIndex <= lastOverflowPage; pageIndex += 1) {
      const targetBoundary =
        pageIndex * pageStride + pageContentHeight - Math.max(1, safetyMargin);

      if (blockBottom <= targetBoundary + overflowTolerance) {
        continue;
      }

      if (searchStart >= textEnd - MIN_TRAILING_TEXT_CHARS) {
        break;
      }

      const splitPosition = resolveSplitPositionForBoundary({
        editor,
        proseMirrorRect,
        blockTop,
        blockBottom,
        textStart,
        textEnd,
        searchStart,
        targetBoundary,
        zoomLevel: safeZoomLevel,
      });

      if (splitPosition === null) {
        continue;
      }

      const normalizedSplitPosition = clampCandidatePosition({
        textStart,
        textEnd,
        candidate: splitPosition,
      });

      if (
        normalizedSplitPosition <= textStart + MIN_LEADING_TEXT_CHARS - 1 ||
        normalizedSplitPosition >= textEnd - MIN_TRAILING_TEXT_CHARS + 1
      ) {
        continue;
      }

      if (actions[actions.length - 1]?.position !== normalizedSplitPosition) {
        actions.push({
          type: "split",
          position: normalizedSplitPosition,
        });
      }

      searchStart = normalizedSplitPosition + MIN_LEADING_TEXT_CHARS;
    }
  }

  return actions;
}
