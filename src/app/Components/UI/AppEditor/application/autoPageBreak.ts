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

  if (leftBlock.content.size === 0 || rightBlock.content.size === 0) {
    return false;
  }

  const pageBreakNode = pageBreakType.create(attributes);
  let transaction = editor.state.tr.replaceWith(blockPosition, blockPosition + blockNode.nodeSize, [
    leftBlock,
    pageBreakNode,
    rightBlock,
  ]);

  if (!options?.preserveSelection) {
    const originalSelectionFrom = editor.state.selection.from;
    const rightBlockPosition = blockPosition + leftBlock.nodeSize + pageBreakNode.nodeSize;
    const rightContentStart = rightBlockPosition + 1;
    const selectionOffsetWithinRight = Math.max(0, originalSelectionFrom - position);
    const nextSelectionPosition = Math.min(
      rightContentStart + selectionOffsetWithinRight,
      rightContentStart + rightBlock.content.size,
    );

    if (nextSelectionPosition <= rightContentStart - 1) {
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
