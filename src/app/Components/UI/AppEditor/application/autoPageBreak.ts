import type { Editor } from "@tiptap/react";
import type { Node as ProseMirrorNode } from "@tiptap/pm/model";
import { TextSelection } from "@tiptap/pm/state";

function getSiblingPageBreakState(editor: Editor) {
  const { $from } = editor.state.selection;
  const containerDepth = $from.depth - 1;

  if (containerDepth < 0) {
    return null;
  }

  const container = $from.node(containerDepth);
  const blockIndex = $from.index(containerDepth);
  const previousSibling = blockIndex > 0 ? container.child(blockIndex - 1) : null;
  const nextSibling = blockIndex < container.childCount - 1 ? container.child(blockIndex + 1) : null;

  return {
    previousIsPageBreak: previousSibling?.type.name === "pageBreak",
    nextIsPageBreak: nextSibling?.type.name === "pageBreak",
  };
}

type PageBreakAttributes = {
  auto?: boolean;
  mergeOnRemove?: boolean;
  spacerHeight?: number | null;
};

type SplitPageBreakOptions = {
  preserveSelection?: boolean;
};

function applyAutoPaginationHistoryMeta(transaction: ReturnType<Editor["state"]["tr"]["setMeta"]>) {
  return transaction.setMeta("addToHistory", false);
}

function hasAdjacentPageBreakAtResolvedPosition(editor: Editor, position: number) {
  const $position = editor.state.doc.resolve(position);
  const previousNode = $position.nodeBefore;
  const nextNode = $position.nodeAfter;

  return previousNode?.type.name === "pageBreak" || nextNode?.type.name === "pageBreak";
}

function resolveTextblockContentRangeAroundPosition(editor: Editor, position: number) {
  const resolvedPosition = editor.state.doc.resolve(position);
  const previousNode = resolvedPosition.nodeBefore;
  const nextNode = resolvedPosition.nodeAfter;

  const previousRange =
    previousNode?.isTextblock
      ? {
          start: position - previousNode.nodeSize + 1,
          end: position - 1,
        }
      : null;
  const nextRange =
    nextNode?.isTextblock
      ? {
          start: position + 1,
          end: position + nextNode.content.size,
        }
      : null;

  return {
    previousRange,
    nextRange,
  };
}

function insertPageBreakAtCurrentSelection(editor: Editor, attributes?: PageBreakAttributes) {
  const { selection } = editor.state;

  if (!selection.empty || !selection.$from.parent.isTextblock) {
    return false;
  }

  const currentText = selection.$from.parent.textContent;
  const currentOffset = selection.$from.parentOffset;
  const siblingPageBreakState = getSiblingPageBreakState(editor);

  if (!currentText.trim()) {
    return false;
  }

  if (
    currentOffset === 0 &&
    (siblingPageBreakState?.previousIsPageBreak || siblingPageBreakState?.nextIsPageBreak)
  ) {
    return false;
  }

  return editor.chain().focus().splitBlock().insertPageBreak(attributes).run();
}

export function splitBlockAndInsertPageBreak(editor: Editor) {
  return insertPageBreakAtCurrentSelection(editor);
}

export function insertPageBreakBeforeBlock(
  editor: Editor,
  position: number,
  attributes?: PageBreakAttributes,
  options?: SplitPageBreakOptions,
) {
  const pageBreakType = editor.state.schema.nodes.pageBreak;

  if (!pageBreakType || position <= 0 || hasAdjacentPageBreakAtResolvedPosition(editor, position)) {
    return false;
  }

  const originalSelectionFrom = editor.state.selection.from;
  const originalSelectionTo = editor.state.selection.to;
  const isCollapsedSelection = originalSelectionFrom === originalSelectionTo;
  let transaction = editor.state.tr.insert(position, pageBreakType.create(attributes));

  if (attributes?.auto === true) {
    transaction = applyAutoPaginationHistoryMeta(transaction);
  }

  if (options?.preserveSelection) {
    const { previousRange, nextRange } = resolveTextblockContentRangeAroundPosition(editor, position);

    if (isCollapsedSelection && previousRange && isPositionWithinContentRange(originalSelectionFrom, previousRange.start, previousRange.end)) {
      transaction = transaction.setSelection(
        TextSelection.create(
          transaction.doc,
          Math.max(1, Math.min(originalSelectionFrom, transaction.doc.content.size)),
        ),
      );
    } else if (isCollapsedSelection && nextRange && isPositionWithinContentRange(originalSelectionFrom, nextRange.start, nextRange.end)) {
      transaction = transaction.setSelection(
        TextSelection.create(
          transaction.doc,
          Math.max(1, Math.min(transaction.mapping.map(originalSelectionFrom, 1), transaction.doc.content.size)),
        ),
      );
    } else {
      const mappedSelection = editor.state.selection.map(transaction.doc, transaction.mapping);
      transaction = transaction.setSelection(mappedSelection);
    }
  }

  editor.view.dispatch(transaction);
  return true;
}

function isListStructuredNodeName(nodeName: string) {
  return nodeName === "bulletList" || nodeName === "orderedList" || nodeName === "taskList";
}

function resolveFirstTextRangeInNode(node: ProseMirrorNode, nodePosition: number) {
  let firstTextRange: { start: number; end: number } | null = null;

  node.descendants((descendant, pos) => {
    if (!descendant.isTextblock) {
      return undefined;
    }

    const start = nodePosition + 1 + pos + 1;
    firstTextRange = {
      start,
      end: start + descendant.content.size,
    };

    return false;
  });

  return firstTextRange;
}

function isPositionWithinContentRange(position: number, start: number, end: number) {
  return position >= start && position <= end;
}

export function splitListBlockBeforeItemAndInsertPageBreak(
  editor: Editor,
  listPosition: number,
  itemPosition: number,
  attributes?: PageBreakAttributes,
  options?: SplitPageBreakOptions,
) {
  const listNode = editor.state.doc.nodeAt(listPosition);
  const pageBreakType = editor.state.schema.nodes.pageBreak;

  if (!listNode || !pageBreakType || !isListStructuredNodeName(listNode.type.name)) {
    return false;
  }

  let childOffset = 0;
  let targetChildOffset: number | null = null;

  for (let index = 0; index < listNode.childCount; index += 1) {
    const child = listNode.child(index);
    const currentItemPosition = listPosition + 1 + childOffset;

    if (currentItemPosition === itemPosition) {
      if (index === 0) {
        return false;
      }

      targetChildOffset = childOffset;
      break;
    }

    childOffset += child.nodeSize;
  }

  if (
    targetChildOffset === null ||
    targetChildOffset <= 0 ||
    targetChildOffset >= listNode.content.size
  ) {
    return false;
  }

  const leftContent = listNode.content.cut(0, targetChildOffset);
  const rightContent = listNode.content.cut(targetChildOffset, listNode.content.size);

  if (leftContent.size === 0 || rightContent.size === 0) {
    return false;
  }

  const leftList = listNode.type.create(listNode.attrs, leftContent, listNode.marks);
  const rightList = listNode.type.create(listNode.attrs, rightContent, listNode.marks);
  const pageBreakNode = pageBreakType.create(attributes);
  const originalSelectionFrom = editor.state.selection.from;
  const originalSelectionTo = editor.state.selection.to;
  const isCollapsedSelection = originalSelectionFrom === originalSelectionTo;
  let transaction = editor.state.tr.replaceWith(
    listPosition,
    listPosition + listNode.nodeSize,
    [leftList, pageBreakNode, rightList],
  );

  if (attributes?.auto === true) {
    transaction = applyAutoPaginationHistoryMeta(transaction);
  }

  if (!options?.preserveSelection) {
    const rightListPosition = listPosition + leftList.nodeSize + pageBreakNode.nodeSize;
    const nextSelectionPosition = Math.min(
      rightListPosition + 2,
      rightListPosition + rightList.nodeSize - 1,
    );

    if (nextSelectionPosition <= rightListPosition) {
      return false;
    }

    transaction = transaction.setSelection(
      TextSelection.create(transaction.doc, nextSelectionPosition),
    );
  }

  if (options?.preserveSelection) {
    const mappedSelection = editor.state.selection.map(transaction.doc, transaction.mapping);
    const rightListPosition = listPosition + leftList.nodeSize + pageBreakNode.nodeSize;
    const rightListEnd = rightListPosition + rightList.nodeSize;
    const mappedSelectionStaysInTextblock =
      mappedSelection.$from.parent.isTextblock &&
      mappedSelection.from >= rightListPosition &&
      mappedSelection.to <= rightListEnd;

    if (mappedSelectionStaysInTextblock) {
      transaction = transaction.setSelection(mappedSelection);
    } else {
      const firstTextRange = resolveFirstTextRangeInNode(rightList, rightListPosition);

      if (!firstTextRange) {
        transaction = transaction.setSelection(mappedSelection);
      } else if (isCollapsedSelection && originalSelectionFrom >= itemPosition) {
        const relativeOffset = originalSelectionFrom - itemPosition;
        const { start, end } = firstTextRange;
        const safeSelectionPosition = Math.max(
          start,
          Math.min(start + relativeOffset, end),
        );

        transaction = transaction.setSelection(
          TextSelection.create(transaction.doc, safeSelectionPosition),
        );
      } else {
        transaction = transaction.setSelection(
          TextSelection.create(transaction.doc, firstTextRange.start),
        );
      }
    }

    editor.view.dispatch(transaction);
    return true;
  }

  editor.view.dispatch(transaction.scrollIntoView());
  return true;
}

export function splitTextBlockAtPositionAndInsertPageBreak(
  editor: Editor,
  position: number,
  attributes?: PageBreakAttributes,
  options?: SplitPageBreakOptions,
) {
  const resolvedPosition = editor.state.doc.resolve(position);
  const blockDepth = resolvedPosition.depth;
  const blockPosition = resolvedPosition.before(blockDepth);
  const parentStart = resolvedPosition.start();
  const parentEnd = resolvedPosition.end();
  const pageBreakType = editor.state.schema.nodes.pageBreak;
  const blockNode = resolvedPosition.parent;

  if (
    position <= parentStart ||
    position > parentEnd ||
    !pageBreakType ||
    !blockNode.isTextblock
  ) {
    return false;
  }

  const splitOffset = position - parentStart;
  const leftBlock = blockNode.cut(0, splitOffset);
  const rightBlock = blockNode.cut(splitOffset, blockNode.content.size);
  const originalSelectionFrom = editor.state.selection.from;
  const originalSelectionTo = editor.state.selection.to;
  const isCollapsedSelection = originalSelectionFrom === originalSelectionTo;
  const selectionWasInsideSplitBlock =
    isCollapsedSelection &&
    isPositionWithinContentRange(originalSelectionFrom, parentStart, parentEnd) &&
    isPositionWithinContentRange(originalSelectionTo, parentStart, parentEnd);

  if (leftBlock.content.size === 0 || rightBlock.content.size === 0) {
    return false;
  }

  const pageBreakNode = pageBreakType.create(attributes);
  let transaction = editor.state.tr.replaceWith(blockPosition, blockPosition + blockNode.nodeSize, [
    leftBlock,
    pageBreakNode,
    rightBlock,
  ]);

  if (attributes?.auto === true) {
    transaction = applyAutoPaginationHistoryMeta(transaction);
  }
  const nextSelectionFrom = transaction.mapping.map(originalSelectionFrom, 1);
  const nextSelectionTo = transaction.mapping.map(originalSelectionTo, 1);
  const leftBlockPosition = blockPosition;
  const leftBlockTextStart = leftBlockPosition + 1;
  const leftBlockTextEnd = leftBlockPosition + leftBlock.content.size;
  const rightBlockPosition = blockPosition + leftBlock.nodeSize + pageBreakNode.nodeSize;
  const rightBlockTextStart = rightBlockPosition + 1;
  const rightBlockTextEnd = rightBlockPosition + rightBlock.content.size;
  const safeSelectionFrom =
    selectionWasInsideSplitBlock
      ? originalSelectionFrom >= position
        ? Math.max(
            rightBlockTextStart,
            Math.min(rightBlockTextStart + (originalSelectionFrom - position), rightBlockTextEnd),
          )
        : Math.max(
            leftBlockTextStart,
            Math.min(leftBlockTextStart + (originalSelectionFrom - parentStart), leftBlockTextEnd),
          )
      : Math.max(1, Math.min(nextSelectionFrom, transaction.doc.content.size));
  const safeSelectionTo =
    selectionWasInsideSplitBlock
      ? originalSelectionTo >= position
        ? Math.max(
            rightBlockTextStart,
            Math.min(rightBlockTextStart + (originalSelectionTo - position), rightBlockTextEnd),
          )
        : Math.max(
            leftBlockTextStart,
            Math.min(leftBlockTextStart + (originalSelectionTo - parentStart), leftBlockTextEnd),
          )
      : Math.max(1, Math.min(nextSelectionTo, transaction.doc.content.size));

  transaction = transaction.setSelection(
    safeSelectionFrom === safeSelectionTo
      ? TextSelection.create(transaction.doc, safeSelectionFrom)
      : TextSelection.between(
          transaction.doc.resolve(safeSelectionFrom),
          transaction.doc.resolve(safeSelectionTo),
        ),
  );

  if (options?.preserveSelection) {
    editor.view.dispatch(transaction);
    return true;
  }

  editor.view.dispatch(transaction.scrollIntoView());
  return true;
}
