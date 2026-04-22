import type { Editor } from "@tiptap/react";
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

function hasAdjacentPageBreakAtResolvedPosition(editor: Editor, position: number) {
  const $position = editor.state.doc.resolve(position);
  const previousNode = $position.nodeBefore;
  const nextNode = $position.nodeAfter;

  return previousNode?.type.name === "pageBreak" || nextNode?.type.name === "pageBreak";
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
) {
  const pageBreakType = editor.state.schema.nodes.pageBreak;

  if (!pageBreakType || position <= 0 || hasAdjacentPageBreakAtResolvedPosition(editor, position)) {
    return false;
  }

  const transaction = editor.state.tr.insert(position, pageBreakType.create(attributes));
  editor.view.dispatch(transaction);
  return true;
}

function isListStructuredNodeName(nodeName: string) {
  return nodeName === "bulletList" || nodeName === "orderedList" || nodeName === "taskList";
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
  let transaction = editor.state.tr.replaceWith(
    listPosition,
    listPosition + listNode.nodeSize,
    [leftList, pageBreakNode, rightList],
  );

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

  if (leftBlock.content.size === 0 || rightBlock.content.size === 0) {
    return false;
  }

  const pageBreakNode = pageBreakType.create(attributes);
  let transaction = editor.state.tr.replaceWith(blockPosition, blockPosition + blockNode.nodeSize, [
    leftBlock,
    pageBreakNode,
    rightBlock,
  ]);
  const nextSelectionFrom = transaction.mapping.map(originalSelectionFrom, 1);
  const nextSelectionTo = transaction.mapping.map(originalSelectionTo, 1);
  const rightBlockPosition = blockPosition + leftBlock.nodeSize + pageBreakNode.nodeSize;
  const rightBlockTextStart = rightBlockPosition + 1;
  const rightBlockTextEnd = rightBlockPosition + rightBlock.content.size;
  const safeSelectionFrom =
    isCollapsedSelection && originalSelectionFrom >= position
      ? Math.max(
          rightBlockTextStart,
          Math.min(rightBlockTextStart + (originalSelectionFrom - position), rightBlockTextEnd),
        )
      : Math.max(1, Math.min(nextSelectionFrom, transaction.doc.content.size));
  const safeSelectionTo =
    isCollapsedSelection && originalSelectionTo >= position
      ? Math.max(
          rightBlockTextStart,
          Math.min(rightBlockTextStart + (originalSelectionTo - position), rightBlockTextEnd),
        )
      : Math.max(1, Math.min(nextSelectionTo, transaction.doc.content.size));

  transaction = transaction.setSelection(
    TextSelection.between(
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
