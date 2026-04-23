import type { Editor } from "@tiptap/react";
import { TextSelection } from "@tiptap/pm/state";

export const AUTO_PAGE_BREAK_SAFETY_MARGIN = 12;
const PAGE_BOUNDARY_TOLERANCE = 2;
const SPLIT_SEARCH_WINDOW = 32;
const MAX_WORD_SNAP_BACKTRACK = 2;
const MIN_LEADING_TEXT_CHARS = 1;
const MIN_TRAILING_TEXT_CHARS = 1;

export type AutoPageBreakAction =
  | {
      type: "before";
      position: number;
    }
  | {
      type: "list-item";
      listPosition: number;
      itemPosition: number;
    }
  | {
      type: "split";
      position: number;
    };

function getPositionedActionPosition(action: AutoPageBreakAction | undefined) {
  if (!action || action.type === "list-item") {
    return null;
  }

  return action.position;
}

type BlockLayoutKind =
  | "text-divisible"
  | "list-structured"
  | "atomic-indivisible"
  | "manual-break"
  | "generic-block";

function resolveBlockLayoutKind(nodeName: string, isTextblock: boolean): BlockLayoutKind {
  if (nodeName === "pageBreak") {
    return "manual-break";
  }

  if (isTextblock) {
    return "text-divisible";
  }

  if (nodeName === "bulletList" || nodeName === "orderedList" || nodeName === "taskList") {
    return "list-structured";
  }

  if (nodeName === "image") {
    return "atomic-indivisible";
  }

  return "generic-block";
}

function resolveSplitPositionFromDomText({
  editor,
  block,
  proseMirrorRect,
  targetBoundary,
  minPosition,
  maxPosition,
  zoomLevel,
}: {
  editor: Editor;
  block: HTMLElement;
  proseMirrorRect: DOMRect;
  targetBoundary: number;
  minPosition: number;
  maxPosition: number;
  zoomLevel: number;
}) {
  if (typeof document === "undefined" || typeof Node === "undefined") {
    return null;
  }

  const textNodes: Text[] = [];
  const collectTextNodes = (node: Node) => {
    if (node.nodeType === Node.TEXT_NODE) {
      const textNode = node as Text;
      if ((textNode.textContent?.length ?? 0) > 0) {
        textNodes.push(textNode);
      }
      return;
    }

    node.childNodes.forEach((childNode) => {
      collectTextNodes(childNode);
    });
  };

  collectTextNodes(block);

  const measureCaretBottom = (textNode: Text, offset: number) => {
    const range = document.createRange();
    const safeOffset = Math.max(1, Math.min(offset, textNode.textContent?.length ?? 0));
    range.setStart(textNode, safeOffset - 1);
    range.setEnd(textNode, safeOffset);
    if (typeof range.getBoundingClientRect !== "function") {
      range.detach?.();
      return null;
    }
    const rect = range.getBoundingClientRect();
    range.detach?.();
    return Math.max(0, (rect.bottom - proseMirrorRect.top) / zoomLevel);
  };

  let lastFittingPosition: number | null = null;

  for (const textNode of textNodes) {
    const textLength = textNode.textContent?.length ?? 0;

    if (textLength <= 0) {
      continue;
    }

    const firstBottom = measureCaretBottom(textNode, 1);
    const lastBottom = measureCaretBottom(textNode, textLength);

    if (firstBottom === null || lastBottom === null) {
      return null;
    }

    if (lastBottom <= targetBoundary) {
      const fittingPosition = editor.view.posAtDOM(textNode, textLength);
      if (fittingPosition >= minPosition && fittingPosition <= maxPosition) {
        lastFittingPosition = fittingPosition;
      }
      continue;
    }

    if (firstBottom > targetBoundary) {
      return lastFittingPosition;
    }

    let low = 1;
    let high = textLength;
    let candidateOffset = 1;

    while (low <= high) {
      const middle = Math.floor((low + high) / 2);
      const bottom = measureCaretBottom(textNode, middle);

      if (bottom === null) {
        return null;
      }

      if (bottom <= targetBoundary) {
        candidateOffset = middle;
        low = middle + 1;
      } else {
        high = middle - 1;
      }
    }

    const position = editor.view.posAtDOM(textNode, candidateOffset);

    if (position >= minPosition && position <= maxPosition) {
      return position;
    }

    return lastFittingPosition;
  }

  return lastFittingPosition;
}

function resolveTopLevelBlockPosition(editor: Editor, childIndex: number) {
  let blockPosition = 0;

  for (let index = 0; index < editor.state.doc.childCount; index += 1) {
    const blockNode = editor.state.doc.child(index);

    if (index === childIndex) {
      const layoutKind = resolveBlockLayoutKind(blockNode.type.name, blockNode.isTextblock);
      const isMeasurableEmptyAtomic = layoutKind === "atomic-indivisible";

      if (blockNode.content.size === 0 && !isMeasurableEmptyAtomic) {
        return null;
      }

      return {
        blockPosition,
        blockNode,
        layoutKind,
      };
    }

    blockPosition += blockNode.nodeSize;
  }

  return null;
}

export function resolveTopLevelBlockStartPosition(editor: Editor, childIndex: number) {
  return resolveTopLevelBlockPosition(editor, childIndex)?.blockPosition ?? 0;
}

export function resolveAutoPageBreakCleanupStartPosition(
  editor: Editor,
  childIndex: number,
  options?: {
    includePreviousAutoBreak?: boolean;
  },
) {
  const safeChildIndex = Math.max(0, Math.min(childIndex, Math.max(0, editor.state.doc.childCount - 1)));
  const includePreviousAutoBreak = options?.includePreviousAutoBreak === true;
  let blockPosition = 0;

  for (let index = 0; index < editor.state.doc.childCount; index += 1) {
    const node = editor.state.doc.child(index);

    if (index === safeChildIndex) {
      if (!includePreviousAutoBreak) {
        return blockPosition;
      }

      let cleanupPosition = blockPosition;
      let previousIndex = index - 1;

      while (previousIndex >= 0) {
        const previousNode = editor.state.doc.child(previousIndex);
        const previousPosition = cleanupPosition - previousNode.nodeSize;
        const isAutoPageBreak =
          previousNode.type.name === "pageBreak" && previousNode.attrs.auto === true;

        if (!isAutoPageBreak) {
          break;
        }

        cleanupPosition = previousPosition;
        previousIndex -= 1;
      }

      return cleanupPosition;
    }

    blockPosition += node.nodeSize;
  }

  return 0;
}

export function resolveTopLevelChildIndexFromPosition(editor: Editor, position: number) {
  const safePosition = Math.max(0, Math.min(position, editor.state.doc.content.size));
  const resolvedPosition = editor.state.doc.resolve(safePosition);
  return Math.max(
    0,
    Math.min(resolvedPosition.index(0), Math.max(0, editor.state.doc.childCount - 1)),
  );
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

function resolvePreferredTextSplitPosition({
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
  const backwardLimit = Math.max(
    MIN_LEADING_TEXT_CHARS,
    relativeCandidate - Math.min(SPLIT_SEARCH_WINDOW, MAX_WORD_SNAP_BACKTRACK),
  );

  for (let index = relativeCandidate; index >= backwardLimit; index -= 1) {
    if (/\s/.test(blockText[index - 1] ?? "")) {
      const splitPosition = textStart + index;
      if (splitPosition > textStart && splitPosition < textEnd) {
        return splitPosition;
      }
    }
  }

  return normalizedCandidate;
}

function isPositionWithinContentRange(position: number, start: number, end: number) {
  return position >= start && position <= end;
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
  block,
  proseMirrorRect,
  textStart,
  textEnd,
  searchStart,
  targetBoundary,
  zoomLevel,
}: {
  editor: Editor;
  block: HTMLElement;
  proseMirrorRect: DOMRect;
  textStart: number;
  textEnd: number;
  searchStart: number;
  targetBoundary: number;
  zoomLevel: number;
}) {
  const exactSplitPosition = resolveSplitPositionFromDomText({
    editor,
    block,
    proseMirrorRect,
    targetBoundary,
    minPosition: searchStart,
    maxPosition: textEnd,
    zoomLevel,
  });

  if (exactSplitPosition !== null) {
    return resolvePreferredTextSplitPosition({
      editor,
      textStart,
      textEnd,
      candidate: exactSplitPosition,
    });
  }

  let low = searchStart;
  let high = textEnd - MIN_TRAILING_TEXT_CHARS;
  let candidate: number | null = null;

  while (low <= high) {
    const middle = Math.floor((low + high) / 2);
    const positionRect = editor.view.coordsAtPos(middle);
    const relativeBottom = Math.max(0, (positionRect.bottom - proseMirrorRect.top) / zoomLevel);

    if (relativeBottom <= targetBoundary) {
      candidate = middle;
      low = middle + 1;
    } else {
      high = middle - 1;
    }
  }

  if (candidate === null) {
    return null;
  }

  return resolvePreferredTextSplitPosition({
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
    editor.view.dispatch(transaction.setMeta("addToHistory", false));
  }
}

export function removeAutoPageBreaks(
  editor: Editor,
  fromPosition = 0,
) {
  const positionsToRemove: number[] = [];
  const safeFromPosition = Math.max(0, fromPosition);

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name !== "pageBreak" || node.attrs.auto !== true || pos < safeFromPosition) {
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
      const canMergeStructuredNodes =
        previousNode &&
        nextNode &&
        previousNode.sameMarkup(nextNode) &&
        ((previousNode.isTextblock && nextNode.isTextblock) ||
          ((previousNode.type.name === "bulletList" ||
            previousNode.type.name === "orderedList" ||
            previousNode.type.name === "taskList") &&
            previousNode.type.name === nextNode.type.name));

      if (
        pageBreakNode.attrs.mergeOnRemove === true &&
        canMergeStructuredNodes
      ) {
        const currentSelectionFrom = transaction.selection.from;
        const currentSelectionTo = transaction.selection.to;
        const previousStart = position - previousNode.nodeSize;
        const previousContentStart = previousStart + 1;
        const previousContentEnd = previousStart + previousNode.content.size;
        const nextStart = position + pageBreakNode.nodeSize;
        const nextContentStart = nextStart + 1;
        const nextContentEnd = nextStart + nextNode.content.size;
        const nextEnd = position + pageBreakNode.nodeSize + nextNode.nodeSize;
        const mergedNode = previousNode.type.create(
          previousNode.attrs,
          previousNode.content.append(nextNode.content),
          previousNode.marks,
        );
        const mergedContentStart = previousStart + 1;
        const mergedContentEnd = previousStart + mergedNode.content.size;

        transaction = transaction.replaceWith(previousStart, nextEnd, mergedNode);

        if (previousNode.isTextblock && nextNode.isTextblock) {
          const resolveMergedSelectionPosition = (selectionPosition: number) => {
            if (
              isPositionWithinContentRange(
                selectionPosition,
                previousContentStart,
                previousContentEnd,
              )
            ) {
              return Math.max(
                mergedContentStart,
                Math.min(
                  mergedContentStart + (selectionPosition - previousContentStart),
                  mergedContentEnd,
                ),
              );
            }

            if (
              isPositionWithinContentRange(
                selectionPosition,
                nextContentStart,
                nextContentEnd,
              )
            ) {
              return Math.max(
                mergedContentStart,
                Math.min(
                  mergedContentStart +
                    previousNode.content.size +
                    (selectionPosition - nextContentStart),
                  mergedContentEnd,
                ),
              );
            }

            return Math.max(
              1,
              Math.min(transaction.mapping.map(selectionPosition, 1), transaction.doc.content.size),
            );
          };

          transaction = transaction.setSelection(
            TextSelection.between(
              transaction.doc.resolve(resolveMergedSelectionPosition(currentSelectionFrom)),
              transaction.doc.resolve(resolveMergedSelectionPosition(currentSelectionTo)),
            ),
          );
        }
        return;
      }

      transaction = transaction.delete(position, position + pageBreakNode.nodeSize);
    });

  editor.view.dispatch(transaction.setMeta("addToHistory", false));
  return true;
}

function resolveDirectChildOverflowAction({
  block,
  blockTop,
  blockNode,
  blockPosition,
  blockPage,
  pageStride,
  allowedBlockBottom,
  overflowTolerance,
}: {
  block: HTMLElement;
  blockTop: number;
  blockNode: NonNullable<ReturnType<typeof resolveTopLevelBlockPosition>>["blockNode"];
  blockPosition: number;
  blockPage: number;
  pageStride: number;
  allowedBlockBottom: number;
  overflowTolerance: number;
}): AutoPageBreakAction | null {
  const directChildren = Array.from(block.children).filter(
    (child): child is HTMLElement => child instanceof HTMLElement,
  );
  if (directChildren.length <= 1) {
    return null;
  }

  const pageStart = blockPage * pageStride;

  for (let index = 0; index < directChildren.length; index += 1) {
    const child = directChildren[index];
    const childTop = Math.max(0, blockTop + child.offsetTop);
    const childBottom = childTop + Math.max(
      1,
      child.offsetHeight || child.getBoundingClientRect().height || child.scrollHeight || 0,
    );

    if (childBottom <= allowedBlockBottom + overflowTolerance) {
      continue;
    }

    if (index === 0) {
      if (blockPosition > 0 && childTop > pageStart + overflowTolerance) {
        return {
          type: "before",
          position: blockPosition,
        };
      }

      return null;
    }

    let childOffset = 0;
    for (let childIndex = 0; childIndex < index; childIndex += 1) {
      childOffset += blockNode.child(childIndex)?.nodeSize ?? 0;
    }

    return {
      type: "list-item",
      listPosition: blockPosition,
      itemPosition: blockPosition + 1 + childOffset,
    };
  }

  return null;
}

function resolveBlockSafetyMargin({
  baseSafetyMargin,
}: {
  baseSafetyMargin: number;
}) {
  return Math.max(1, Math.ceil(baseSafetyMargin));
}

export function resolveAutoPageBreakActions({
  editor,
  proseMirror,
  pageContentHeight,
  pageStride,
  safetyMargin = AUTO_PAGE_BREAK_SAFETY_MARGIN,
  zoomLevel = 1,
  startChildIndex = 0,
}: {
  editor: Editor;
  proseMirror: HTMLElement;
  pageContentHeight: number;
  pageStride: number;
  safetyMargin?: number;
  zoomLevel?: number;
  startChildIndex?: number;
}): AutoPageBreakAction[] {
  const proseMirrorRect = proseMirror.getBoundingClientRect();
  const actions: AutoPageBreakAction[] = [];
  const topLevelChildren = Array.from(proseMirror.children);
  const safeZoomLevel = Math.max(0.1, zoomLevel);
  const safeStartChildIndex = Math.max(
    0,
    Math.min(startChildIndex, Math.max(0, topLevelChildren.length - 1)),
  );

  for (let childIndex = safeStartChildIndex; childIndex < topLevelChildren.length; childIndex += 1) {
    const child = topLevelChildren[childIndex];
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
    const effectiveSafetyMargin = resolveBlockSafetyMargin({
      baseSafetyMargin: safetyMargin,
    });
    const allowedBlockBottom = pageEnd - effectiveSafetyMargin;
    const overflowTolerance = 2;

    if (blockBottom <= allowedBlockBottom + overflowTolerance) {
      continue;
    }

    const blockPositionInfo = resolveTopLevelBlockPosition(editor, childIndex);
    if (!blockPositionInfo) {
      continue;
    }

    const { blockPosition, blockNode, layoutKind } = blockPositionInfo;
    const pageStart = blockPage * pageStride;

    if (blockHeight <= pageContentHeight && layoutKind !== "text-divisible") {
      if (
        blockPosition > 0 &&
        blockTop > pageStart + overflowTolerance &&
        getPositionedActionPosition(actions[actions.length - 1]) !== blockPosition
      ) {
        actions.push({
          type: "before",
          position: blockPosition,
        });
      }
      continue;
    }

    if (layoutKind === "list-structured") {
      const listAction = resolveDirectChildOverflowAction({
        block,
        blockTop,
        blockNode,
        blockPosition,
        blockPage,
        pageStride,
        allowedBlockBottom,
        overflowTolerance,
      });

      if (listAction) {
        actions.push(listAction);
        continue;
      }
    }

    if (layoutKind === "atomic-indivisible" || layoutKind === "generic-block") {
      if (blockPosition > 0 && blockTop > pageStart + overflowTolerance) {
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

    let foundSplitForBlock = false;

    for (let pageIndex = firstOverflowPage; pageIndex <= lastOverflowPage; pageIndex += 1) {
      const targetBoundary =
        pageIndex * pageStride + pageContentHeight - Math.max(1, effectiveSafetyMargin);

      if (blockBottom <= targetBoundary + overflowTolerance) {
        continue;
      }

      if (searchStart >= textEnd - MIN_TRAILING_TEXT_CHARS) {
        break;
      }

      const splitPosition = resolveSplitPositionForBoundary({
        editor,
        block,
        proseMirrorRect,
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

      if (getPositionedActionPosition(actions[actions.length - 1]) !== normalizedSplitPosition) {
        actions.push({
          type: "split",
          position: normalizedSplitPosition,
        });
      }

      foundSplitForBlock = true;
      searchStart = normalizedSplitPosition + MIN_LEADING_TEXT_CHARS;
    }

    if (
      !foundSplitForBlock &&
      blockHeight <= pageContentHeight &&
      blockPosition > 0 &&
      blockTop > pageStart + overflowTolerance
    ) {
      actions.push({
        type: "before",
        position: blockPosition,
      });
    }
  }

  return actions;
}
