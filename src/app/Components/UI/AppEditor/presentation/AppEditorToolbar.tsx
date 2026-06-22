import { memo, useCallback, useEffect, useMemo, useRef, useState } from "react";
import { Input, Popover } from "antd";
import { DownOutlined, MoreOutlined } from "@ant-design/icons";
import type { ChangeEvent, MouseEvent, ReactNode } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import type { Node as ProseMirrorNode } from "@tiptap/pm/model";
import { AllSelection, NodeSelection, TextSelection, type Selection } from "@tiptap/pm/state";
import {
  faBold,
  faItalic,
  faUnderline,
  faListUl,
  faListOl,
  faListCheck,
  faArrowRotateLeft,
  faArrowRotateRight,
  faLink,
  faImage,
  faAlignLeft,
  faAlignCenter,
  faAlignRight,
  faAlignJustify,
  faParagraph,
  faChevronDown,
  faHeading,
  faArrowUp,
  faArrowDown,
} from "@fortawesome/free-solid-svg-icons";
import type { Editor } from "@tiptap/react";
import { AppDropdown } from "../../AppDropdown";
import { AppButton } from "../../AppButton";
import type { AppEditorHeadingLevel } from "../domain/editor.types";
import {
  createSafeTextSelectionFromRange,
  resolveSafeTextSelectionRange,
  type TextSelectionRange,
} from "../domain/editor.model";
import { normalizeImageWidth } from "../application/imageSizing";
import { generateEditorImageId } from "../application/localImageIds";
import styles from "../AppEditor.module.css";

type AppEditorToolbarProps = {
  editor: Editor | null;
  disabled?: boolean;
  onInsertLocalImage?: (
    file: File,
    width?: string,
    insertionSelection?: {
      from: number;
      to: number;
    },
  ) => Promise<void>;
  toolbarActions?: ReactNode;
  trailingContent?: ReactNode;
};

type ToolbarButtonConfig = {
  key: string;
  label: string;
  icon: typeof faBold;
  isActive?: boolean;
  disabled?: boolean;
  onClick: () => void;
};

type LastImageIdentity = {
  imageId?: string | null;
  localImageId?: string | null;
  src?: string | null;
};

type LastImageCache = {
  pos: number | null;
  identity: LastImageIdentity | null;
};

type TopLevelNodeEntry = {
  pos: number;
  node: ProseMirrorNode;
};

function getTopLevelMovableEntries(editor: Editor) {
  const entries: TopLevelNodeEntry[] = [];
  editor.state.doc.forEach((node, offset) => {
    entries.push({ pos: offset, node });
  });

  while (
    entries.length > 0 &&
    entries[entries.length - 1].node.type.name === "paragraph" &&
    entries[entries.length - 1].node.content.size === 0
  ) {
    entries.pop();
  }

  return entries;
}

type PreservedSelectionSnapshot =
  | {
      type: "all";
      from: number;
      to: number;
      anchor: number;
      head: number;
    }
  | {
      type: "node";
      from: number;
      to: number;
      anchor: number;
      head: number;
    }
  | {
      type: "text";
      from: number;
      to: number;
      anchor: number;
      head: number;
    };

const lastImageCacheByEditor = new WeakMap<Editor, LastImageCache>();

function getLastImageCache(editor: Editor): LastImageCache {
  return lastImageCacheByEditor.get(editor) ?? { pos: null, identity: null };
}

function setLastImageCache(editor: Editor, next: LastImageCache) {
  lastImageCacheByEditor.set(editor, next);
}

function createSelectionSnapshot(selection?: Selection | null): PreservedSelectionSnapshot | null {
  if (
    !selection ||
    typeof selection.from !== "number" ||
    typeof selection.to !== "number"
  ) {
    return null;
  }

  const anchor = typeof selection.anchor === "number" ? selection.anchor : selection.from;
  const head = typeof selection.head === "number" ? selection.head : selection.to;

  if (selection instanceof AllSelection) {
    return {
      type: "all",
      from: selection.from,
      to: selection.to,
      anchor,
      head,
    };
  }

  if (selection instanceof NodeSelection) {
    return {
      type: "node",
      from: selection.from,
      to: selection.to,
      anchor,
      head,
    };
  }

  return {
    type: "text",
    from: selection.from,
    to: selection.to,
    anchor,
    head,
  };
}

function isSameSelectionSnapshot(
  first: PreservedSelectionSnapshot | null,
  second: PreservedSelectionSnapshot | null,
) {
  return Boolean(
    first &&
      second &&
      first.type === second.type &&
      first.from === second.from &&
      first.to === second.to &&
      first.anchor === second.anchor &&
      first.head === second.head,
  );
}

function createSelectionFromSnapshot(
  editor: Editor,
  snapshot: PreservedSelectionSnapshot,
) {
  const doc = editor.state.doc;
  const maxPosition = Math.max(0, doc.content.size);

  if (snapshot.type === "all") {
    const allSelectionFactory = AllSelection as typeof AllSelection & {
      create?: (selectionDoc: typeof doc) => AllSelection;
    };
    return typeof allSelectionFactory.create === "function"
      ? allSelectionFactory.create(doc)
      : new AllSelection(doc);
  }

  if (snapshot.type === "node") {
    try {
      return NodeSelection.create(
        doc,
        Math.max(0, Math.min(snapshot.from, maxPosition)),
      );
    } catch {
      return null;
    }
  }

  try {
    return TextSelection.create(
      doc,
      Math.max(0, Math.min(snapshot.anchor, maxPosition)),
      Math.max(0, Math.min(snapshot.head, maxPosition)),
    );
  } catch {
    const safeSelection = createSafeTextSelectionFromRange(
      doc,
      Math.max(0, Math.min(snapshot.from, maxPosition)),
      Math.max(0, Math.min(snapshot.to, maxPosition)),
    );
    return safeSelection;
  }
}

function restoreSelectionSnapshot(editor: Editor, snapshot: PreservedSelectionSnapshot | null) {
  if (!snapshot) {
    return false;
  }

  const nextSelection = createSelectionFromSnapshot(editor, snapshot);
  if (!nextSelection) {
    return false;
  }

  try {
    editor.view.dispatch(editor.state.tr.setSelection(nextSelection));
    return true;
  } catch {
    return false;
  }
}

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const HEADING_OPTIONS = [
  { value: "paragraph", label: "Parrafo", shortLabel: "P", icon: faParagraph },
  { value: "h1", label: "Titulo 1", shortLabel: "H1", icon: faHeading },
  { value: "h2", label: "Titulo 2", shortLabel: "H2", icon: faHeading },
  { value: "h3", label: "Titulo 3", shortLabel: "H3", icon: faHeading },
] as const;

// Grouping is intentionally positional to keep the render path simple for the lint rules
// enforced in this repo.

function useCompactToolbarMode(maxWidth = 1024) {
  const [isCompact, setIsCompact] = useState(
    () => typeof window !== "undefined" && window.innerWidth <= maxWidth,
  );

  useEffect(() => {
    if (typeof window === "undefined") {
      return undefined;
    }

    const handleResize = () => {
      setIsCompact(window.innerWidth <= maxWidth);
    };

    handleResize();
    window.addEventListener("resize", handleResize);

    return () => {
      window.removeEventListener("resize", handleResize);
    };
  }, [maxWidth]);

  return isCompact;
}

function dispatchSelectionSafely(editor: Editor, from: number, to = from) {
  const maxPosition = Math.max(0, editor.state.doc.content.size);
  const safeSelection = resolveSafeTextSelectionRange(
    editor.state.doc,
    Math.max(0, Math.min(from, maxPosition)),
    Math.max(0, Math.min(to, maxPosition)),
  );

  if (!safeSelection) {
    return;
  }

  try {
    const textSelection = createSafeTextSelectionFromRange(
      editor.state.doc,
      safeSelection.from,
      safeSelection.to,
    );
    if (!textSelection) {
      return;
    }

    editor.view.dispatch(editor.state.tr.setSelection(textSelection));
  } catch {
    // Keep behavior resilient if selection cannot be mapped for this transient doc state.
  }
}

function getCurrentHeadingValue(editor: Editor | null) {
  if (!editor) {
    return "paragraph";
  }

  if (editor.isActive("heading", { level: 1 })) {
    return "h1";
  }

  if (editor.isActive("heading", { level: 2 })) {
    return "h2";
  }

  if (editor.isActive("heading", { level: 3 })) {
    return "h3";
  }

  return "paragraph";
}

function getHeadingOption(value: string) {
  return HEADING_OPTIONS.find((option) => option.value === value) ?? HEADING_OPTIONS[0];
}

function getCurrentTextAlign(editor: Editor | null) {
  if (!editor) {
    return "left";
  }

  if (editor.isActive({ textAlign: "center" })) {
    return "center";
  }

  if (editor.isActive({ textAlign: "right" })) {
    return "right";
  }

  if (editor.isActive({ textAlign: "justify" })) {
    return "justify";
  }

  return "left";
}

function getTextAlignMeta(value: string) {
  switch (value) {
    case "center":
      return { key: "align-center", label: "Centro", icon: faAlignCenter };
    case "right":
      return { key: "align-right", label: "Derecha", icon: faAlignRight };
    case "justify":
      return { key: "align-justify", label: "Justificar", icon: faAlignJustify };
    default:
      return { key: "align-left", label: "Izquierda", icon: faAlignLeft };
  }
}

function canRun(editor: Editor | null, command: (instance: Editor) => boolean) {
  if (!editor) {
    return false;
  }

  return command(editor);
}

function canSetImageAlign(editor: Editor | null) {
  if (editor?.isActive("image")) {
    return true;
  }

  return getResolvedImagePosition(editor) !== null;
}

function canMoveResolvedImage(editor: Editor | null, direction: "up" | "down") {
  if (!editor?.state?.doc) {
    return false;
  }

  const position = getResolvedImagePosition(editor);
  if (position === null) {
    return false;
  }

  const entries = getTopLevelMovableEntries(editor);

  const imageIndex = entries.findIndex(
    (entry) => entry.pos === position && entry.node.type.name === "image",
  );

  if (imageIndex === -1) {
    return false;
  }

  return direction === "up" ? imageIndex > 0 : imageIndex < entries.length - 1;
}

function moveResolvedImage(editor: Editor | null, direction: "up" | "down") {
  if (!editor?.state?.doc) {
    return false;
  }

  const position = getResolvedImagePosition(editor);
  if (position === null) {
    return false;
  }

  const entries = getTopLevelMovableEntries(editor);

  const imageIndex = entries.findIndex(
    (entry) => entry.pos === position && entry.node.type.name === "image",
  );

  if (imageIndex === -1) {
    return false;
  }

  const targetIndex = direction === "up" ? imageIndex - 1 : imageIndex + 1;
  if (targetIndex < 0 || targetIndex >= entries.length) {
    return false;
  }

  const imageEntry = entries[imageIndex];
  const targetEntry = entries[targetIndex];
  const insertionReference =
    direction === "up"
      ? targetEntry.pos
      : targetEntry.pos + targetEntry.node.nodeSize;

  let transaction = editor.state.tr.delete(
    imageEntry.pos,
    imageEntry.pos + imageEntry.node.nodeSize,
  );
  const nextImagePosition = transaction.mapping.map(
    insertionReference,
    direction === "up" ? -1 : 1,
  );
  const nextImageIdentity = {
    imageId: typeof imageEntry.node.attrs.imageId === "string" ? imageEntry.node.attrs.imageId : null,
    localImageId:
      typeof imageEntry.node.attrs.localImageId === "string" ? imageEntry.node.attrs.localImageId : null,
    src: typeof imageEntry.node.attrs.src === "string" ? imageEntry.node.attrs.src : null,
  };

  (editor as Editor & {
    __appEditorLastImagePos?: number | null;
    __appEditorLastImageIdentity?: LastImageIdentity | null;
  }).__appEditorLastImagePos = nextImagePosition;
  (editor as Editor & {
    __appEditorLastImageIdentity?: LastImageIdentity | null;
  }).__appEditorLastImageIdentity = nextImageIdentity;

  setLastImageCache(editor, {
    pos: nextImagePosition,
    identity: nextImageIdentity,
  });

  transaction = transaction
    .insert(nextImagePosition, imageEntry.node)
    .setSelection(NodeSelection.create(transaction.doc, nextImagePosition))
    .scrollIntoView();
  editor.view.dispatch(transaction);

  return true;
}

function runHistoryCommand(editor: Editor | null, action: "undo" | "redo") {
  if (!editor) {
    return;
  }

  const selectionBefore = editor.state.selection;
  const selectionBeforeRange = {
    from: selectionBefore?.from,
    to: selectionBefore?.to,
  };
  const appHistory = (editor as Editor & {
    appEditorHistory?: {
      undo?: () => boolean;
      redo?: () => boolean;
    };
  }).appEditorHistory;
  const appHistoryCommand = appHistory?.[action];
  if (typeof appHistoryCommand === "function") {
    const handled = appHistoryCommand();
    if (!handled) {
      return;
    }

    if (typeof editor.commands.focus === "function") {
      editor.commands.focus(undefined, { scrollIntoView: false });
    } else if (typeof editor.view?.focus === "function") {
      editor.view.focus();
    }

    return;
  }

  if (selectionBeforeRange.from != null && selectionBeforeRange.to != null) {
    const maxPosition = Math.max(0, editor.state.doc.content.size);
    const safeSelection = resolveSafeTextSelectionRange(
      editor.state.doc,
      Math.max(0, Math.min(selectionBeforeRange.from, maxPosition)),
      Math.max(0, Math.min(selectionBeforeRange.to, maxPosition)),
    );

    if (safeSelection) {
      try {
        const safeRangeSelection = createSafeTextSelectionFromRange(
          editor.state.doc,
          safeSelection.from,
          safeSelection.to,
        );
        if (safeRangeSelection) {
          const selectionTransaction = editor.state.tr.setSelection(safeRangeSelection);
          editor.view.dispatch(selectionTransaction);
        }
      } catch {
        dispatchSelectionSafely(editor, safeSelection.from, safeSelection.to);
      }
    }

    if (typeof editor.commands.focus === "function") {
      editor.commands.focus(undefined, { scrollIntoView: false });
    } else if (typeof editor.view?.focus === "function") {
      editor.view.focus();
    }
  }

  const commands = (editor.commands ?? {}) as {
    undo?: () => boolean;
    redo?: () => boolean;
  };

  const directCommand = commands[action];
  if (typeof directCommand === "function") {
    directCommand.call(commands);
    return;
  }

  const chain = editor.chain() as {
    undo?: () => { run: () => boolean };
    redo?: () => { run: () => boolean };
  };

  const chainedCommand = chain[action];
  if (typeof chainedCommand === "function") {
    chainedCommand.call(chain).run();
  }
}

function runSetImageAlign(editor: Editor | null, align: "left" | "center" | "right") {
  if (!editor) {
    return;
  }

  if (getResolvedImagePosition(editor) !== null) {
    updateResolvedImageAttributes(editor, { align });
    return;
  }

  const chain = editor.chain() as {
    setImageAlign?: (value: "left" | "center" | "right") => { run: () => boolean };
  };

  if (editor.isActive("image") && typeof chain.setImageAlign === "function") {
    chain.setImageAlign(align).run();
  }
}

function formatUrl(value: string) {
  if (/^https?:\/\//i.test(value)) {
    return value;
  }

  return `https://${value}`;
}

function hasActiveImageSelection(editor: Editor | null) {
  if (editor?.isActive("image")) {
    return true;
  }

  return getResolvedImagePosition(editor) !== null;
}

function getLastImagePosition(editor: Editor | null) {
  if (!editor?.state?.doc) {
    return null;
  }

  let lastImagePosition: number | null = null;

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name === "image") {
      lastImagePosition = pos;
    }
  });

  return lastImagePosition;
}

function getResolvedImagePosition(editor: Editor | null) {
  if (!editor || !editor.state?.doc) {
    return null;
  }

  const selection = editor.state?.selection as
    | {
        from?: number;
        node?: { type?: { name?: string } } | null;
        $anchor?: { parent?: { type?: { name?: string } } };
      }
    | undefined;

  if (selection?.node?.type?.name === "image" && typeof selection.from === "number") {
    return selection.from;
  }

  const persistedImagePosition = (editor as Editor & {
    __appEditorLastImagePos?: number | null;
  }).__appEditorLastImagePos;
  if (
    typeof persistedImagePosition === "number" &&
    editor.state.doc.nodeAt(persistedImagePosition)?.type.name === "image"
  ) {
    return persistedImagePosition;
  }

  const lastImagePosition = getLastImageCache(editor).pos;
  if (
    typeof lastImagePosition === "number" &&
    editor.state.doc.nodeAt(lastImagePosition)?.type.name === "image"
  ) {
    return lastImagePosition;
  }

  const lastImageIdentity = getLastImageCache(editor).identity;
  if (lastImageIdentity) {
    let matchedPosition: number | null = null;

    editor.state.doc.descendants((node, pos) => {
      if (matchedPosition !== null || node.type.name !== "image") {
        return false;
      }

      const nodeImageId = typeof node.attrs.imageId === "string" ? node.attrs.imageId : null;
      const nodeLocalImageId =
        typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null;
      const nodeSrc = typeof node.attrs.src === "string" ? node.attrs.src : null;

      if (
        (lastImageIdentity.imageId && nodeImageId === lastImageIdentity.imageId) ||
        (lastImageIdentity.localImageId &&
          nodeLocalImageId === lastImageIdentity.localImageId) ||
        (lastImageIdentity.src && nodeSrc === lastImageIdentity.src)
      ) {
        matchedPosition = pos;
        return false;
      }

      return undefined;
    });

    if (matchedPosition !== null) {
      return matchedPosition;
    }
  }

  let imageCount = 0;
  let singleImagePosition: number | null = null;

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name !== "image") {
      return;
    }

    imageCount += 1;
    if (imageCount === 1) {
      singleImagePosition = pos;
    }
  });

  if (imageCount === 1) {
    return singleImagePosition;
  }

  return getLastImagePosition(editor);
}

function getResolvedImageAttributes(editor: Editor | null) {
  const position = getResolvedImagePosition(editor);

  if (!editor || position === null) {
    return null;
  }

  const node = editor.state.doc.nodeAt(position);
  if (!node || node.type.name !== "image") {
    return null;
  }

  return node.attrs as Record<string, unknown>;
}

function updateResolvedImageAttributes(
  editor: Editor | null,
  attrs: Record<string, unknown>,
) {
  const position = getResolvedImagePosition(editor);

  if (!editor || position === null) {
    return false;
  }

  const node = editor.state.doc.nodeAt(position);
  if (!node || node.type.name !== "image") {
    return false;
  }

  const isImageWidthChange = Object.prototype.hasOwnProperty.call(attrs, "width");

  let transaction = editor.state.tr.setNodeMarkup(position, undefined, {
    ...node.attrs,
    ...attrs,
  });
  if (isImageWidthChange) {
    transaction = transaction.setMeta("appEditorImageResize", true);
  }

  const nextNode = transaction.doc.nodeAt(position);
  if (nextNode?.type.name === "image") {
    transaction = transaction.setSelection(NodeSelection.create(transaction.doc, position));
  }

  editor.view.dispatch(transaction);
  setLastImageCache(editor, {
    pos: position,
    identity: {
      imageId: typeof node.attrs.imageId === "string" ? node.attrs.imageId : null,
      localImageId: typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null,
      src: typeof node.attrs.src === "string" ? node.attrs.src : null,
    },
  });
  return true;
}

function AppEditorToolbarComponent({
  editor,
  disabled = false,
  onInsertLocalImage,
  toolbarActions,
  trailingContent,
}: AppEditorToolbarProps) {
  const isBlocked = disabled || !editor;
  const [editorSnapshotVersion, setEditorSnapshotVersion] = useState(0);
  const isCompactToolbar = useCompactToolbarMode();
  const [isLinkPopoverOpen, setIsLinkPopoverOpen] = useState(false);
  const [isImagePopoverOpen, setIsImagePopoverOpen] = useState(false);
  const [isAlignDropdownOpen, setIsAlignDropdownOpen] = useState(false);
  const [isHeadingDropdownOpen, setIsHeadingDropdownOpen] = useState(false);
  const [linkValue, setLinkValue] = useState("");
  const [imageUrlValue, setImageUrlValue] = useState("");
  const [imageWidthValue, setImageWidthValue] = useState("");
  const fileInputRef = useRef<HTMLInputElement | null>(null);
  const alignSelectionRef = useRef<PreservedSelectionSnapshot | null>(null);
  const headingSelectionRef = useRef<PreservedSelectionSnapshot | null>(null);
  const linkSelectionRef = useRef<PreservedSelectionSnapshot | null>(null);
  const textSelectionRef = useRef<PreservedSelectionSnapshot | null>(null);
  const imageInsertionSelectionRef = useRef<TextSelectionRange | null>(null);
  const hasSelectedImage = hasActiveImageSelection(editor);

  const getCurrentSelectionSnapshot = useCallback(() => {
    return createSelectionSnapshot(editor?.state?.selection);
  }, [editor]);

  const getDomSelection = useCallback((): PreservedSelectionSnapshot | null => {
    if (typeof window === "undefined" || !editor || !editor.view?.dom) {
      return null;
    }

    const nativeSelection = window.getSelection();
    if (!nativeSelection || nativeSelection.rangeCount === 0) {
      return null;
    }

    const range = nativeSelection.getRangeAt(0);
    const root = editor.view.dom;
    if (!root.contains(range.startContainer) || !root.contains(range.endContainer)) {
      return null;
    }

    try {
      const hasChildContent = editor.state.doc.childCount > 0;
      const minPosition = hasChildContent ? 1 : 0;
      const maxPosition = Math.max(minPosition, editor.state.doc.content.size - 1);
      const from = editor.view.posAtDOM(range.startContainer, range.startOffset);
      const to = editor.view.posAtDOM(range.endContainer, range.endOffset);
      const safeFrom = Math.max(minPosition, Math.min(maxPosition, from));
      const safeTo = Math.max(minPosition, Math.min(maxPosition, to));

      return {
        type: "text",
        from: safeFrom,
        to: safeTo,
        anchor: safeFrom,
        head: safeTo,
      };
    } catch {
      return null;
    }
  }, [editor]);

  const getSafeTextSelection = useCallback(
    (from: number, to: number): TextSelectionRange | null => {
      if (!editor) {
        return null;
      }

      return resolveSafeTextSelectionRange(editor.state.doc, from, to);
    },
    [editor],
  );

  const getPreservableSelectionSnapshot = useCallback((): PreservedSelectionSnapshot | null => {
    const sourceSelection = textSelectionRef.current ?? getDomSelection();
    if (!sourceSelection) {
      return null;
    }

    return sourceSelection;
  }, [getDomSelection]);

  const resolveImageInsertionSelection = useCallback(() => {
    if (!editor) {
      return null;
    }

    const cachedSelection =
      imageInsertionSelectionRef.current ??
      textSelectionRef.current ??
      getDomSelection();
    if (!cachedSelection) {
      return null;
    }

    return getSafeTextSelection(cachedSelection.from, cachedSelection.to);
  }, [editor, getDomSelection, getSafeTextSelection]);

  const insertImageUrlWithFallback = useCallback(
    (imageAttributes: Record<string, unknown>, insertionSelection?: TextSelectionRange | null): boolean => {
      if (!editor) {
        return false;
      }

      const imageNode = editor.state.schema.nodes.image;
      if (!imageNode) {
        return false;
      }

      const safeMaxPosition = Math.max(
        editor.state.doc.childCount > 0 ? 1 : 0,
        editor.state.doc.content.size - 1,
      );
      const safeInsertionSelection = insertionSelection
        ? getSafeTextSelection(
            insertionSelection.from,
            insertionSelection.to,
          )
        : resolveSafeTextSelectionRange(
            editor.state.doc,
            editor.state.selection.from,
            editor.state.selection.to,
          );
      const fallbackSelection = safeInsertionSelection ?? {
        from: safeMaxPosition,
        to: safeMaxPosition,
      };
      const insertionFrom = Math.min(
        Math.max(0, fallbackSelection.from),
        safeMaxPosition,
      );
      const insertionTo = Math.min(
        Math.max(0, fallbackSelection.to),
        safeMaxPosition,
      );
      const safeInsertionFrom = Math.min(insertionFrom, safeMaxPosition);
      const safeInsertionTo = Math.min(
        Math.max(insertionFrom, insertionTo),
        editor.state.doc.content.size,
      );

      try {
        const imageNodeToInsert = imageNode.create(imageAttributes);
        const transaction = editor.state.tr
          .deleteRange(safeInsertionFrom, safeInsertionTo)
          .insert(safeInsertionFrom, imageNodeToInsert);
        const selectionPosition = Math.min(
          transaction.doc.content.size,
          Math.max(0, safeInsertionFrom + imageNodeToInsert.nodeSize),
        );
        const safeCursorSelection = resolveSafeTextSelectionRange(
          transaction.doc,
          selectionPosition,
          selectionPosition,
        );
        const selectionTransaction =
          safeCursorSelection === null
            ? transaction
            : (() => {
                const cursorSelection = createSafeTextSelectionFromRange(
                  transaction.doc,
                  safeCursorSelection.from,
                  safeCursorSelection.to,
                );
                return cursorSelection ? transaction.setSelection(cursorSelection) : transaction;
              })();

        editor.view.dispatch(selectionTransaction);
        return true;
      } catch {
        return false;
      }
    },
    [editor, getSafeTextSelection],
  );

  useEffect(() => {
    if (!editor || typeof (editor as { on?: unknown }).on !== "function") {
      return undefined;
    }

    const syncToolbarState = () => {
      const selection = editor.state?.selection;
      const hasSelectedImage = selection instanceof NodeSelection && selection.node.type.name === "image";
      const currentSelection = getCurrentSelectionSnapshot() ?? getDomSelection();
      if (selection && !hasSelectedImage && currentSelection) {
        textSelectionRef.current = currentSelection;
      } else if (hasSelectedImage) {
        textSelectionRef.current = null;
      }

      setEditorSnapshotVersion((currentVersion) => currentVersion + 1);
    };

    editor.on("transaction", syncToolbarState);
    editor.on("selectionUpdate", syncToolbarState);
    editor.on("focus", syncToolbarState);
    editor.on("blur", syncToolbarState);

    return () => {
      if (typeof (editor as { off?: unknown }).off !== "function") {
        return;
      }

      editor.off("transaction", syncToolbarState);
      editor.off("selectionUpdate", syncToolbarState);
      editor.off("focus", syncToolbarState);
      editor.off("blur", syncToolbarState);
    };
  }, [editor, getCurrentSelectionSnapshot, getDomSelection]);

  const handleToolbarMouseDownCapture = useCallback((event: MouseEvent<HTMLDivElement>) => {
    const target = event.target;

    if (!(target instanceof HTMLElement)) {
      return;
    }

    if (
      target.closest("input") ||
      target.closest("textarea") ||
      target.closest('[contenteditable="true"]') ||
      target.closest('[role="menu"]') ||
      target.closest('[role="menuitem"]') ||
      target.closest('.ant-dropdown-menu') ||
      target.closest(".ant-popover") ||
      target.closest(".ant-dropdown")
    ) {
      return;
    }

    const selection = editor?.state?.selection;
    const hasSelectedImage = selection instanceof NodeSelection && selection.node.type.name === "image";
    const currentSelection = getCurrentSelectionSnapshot() ?? getDomSelection();
    if (selection && !hasSelectedImage && currentSelection) {
      textSelectionRef.current = currentSelection;
    } else if (hasSelectedImage) {
      textSelectionRef.current = null;
    }

    event.preventDefault();
  }, [editor, getCurrentSelectionSnapshot, getDomSelection]);

  const captureImageInsertionSelection = useCallback(() => {
    if (!editor) {
      return;
    }

    const hasSelectedImage =
      editor.state.selection instanceof NodeSelection &&
      editor.state.selection.node.type.name === "image";
    if (hasSelectedImage) {
      imageInsertionSelectionRef.current = textSelectionRef.current;
      return;
    }

    const selection = getCurrentSelectionSnapshot() ?? getDomSelection();

    if (selection) {
      imageInsertionSelectionRef.current = selection;
      return;
    }

    imageInsertionSelectionRef.current = null;
  }, [editor, getCurrentSelectionSnapshot, getDomSelection]);

  const runWithPreservedTextSelection = useCallback(
    (
      applyCommand: (chain: Editor["chain"] extends () => infer T ? T : never) => { run: () => boolean },
      forcedSelection?: PreservedSelectionSnapshot | null,
      shouldFocus = true,
    ) => {
      if (!editor || disabled) {
        return;
      }

      const currentSelection = getCurrentSelectionSnapshot() ?? getDomSelection();
      const hasImageSelection =
        editor.state.selection instanceof NodeSelection &&
        editor.state.selection.node.type.name === "image";
      const currentPreservableSelection = hasImageSelection ? null : currentSelection;
      const savedSelection = forcedSelection ?? currentPreservableSelection ?? textSelectionRef.current;
      const shouldRestoreSelection =
        !!savedSelection &&
        !isSameSelectionSnapshot(currentSelection, savedSelection);

      if (shouldRestoreSelection && savedSelection) {
        restoreSelectionSnapshot(editor, savedSelection);
      }

      const needsFocus = shouldFocus && !editor.isFocused;
      const chain = (
        needsFocus ? editor.chain().focus(undefined, { scrollIntoView: false }) : editor.chain()
      ) as Editor["chain"] extends () => infer T ? T : never;
      const baseChain = chain;
      const runCommand = (
        chainToRun: Editor["chain"] extends () => infer T ? T : never,
      ): boolean => {
        const command = applyCommand(chainToRun);
        if (!command || typeof command.run !== "function") {
          return false;
        }

        try {
          return command.run();
        } catch {
          return false;
        }
      };

      const commandApplied = runCommand(chain);
      if (!commandApplied && chain !== baseChain) {
        runCommand(baseChain);
      }

      if (shouldRestoreSelection && savedSelection) {
        restoreSelectionSnapshot(editor, savedSelection);
      }

      const hasResultingImageSelection =
        editor.state?.selection instanceof NodeSelection &&
        editor.state.selection.node.type.name === "image";
      const resultingSelection =
        !hasResultingImageSelection ? createSelectionSnapshot(editor.state?.selection) : null;

      textSelectionRef.current =
        resultingSelection && resultingSelection.from !== resultingSelection.to
          ? resultingSelection
          : savedSelection && savedSelection.from !== savedSelection.to
            ? savedSelection
            : null;
    },
    [disabled, editor, getCurrentSelectionSnapshot, getDomSelection],
  );

  const handleHeadingChange = useCallback((value: string) => {
    if (!editor || disabled) {
      return;
    }

    const directSelection = getCurrentSelectionSnapshot() ?? getDomSelection();
    const preservedSelection =
      headingSelectionRef.current ??
      directSelection ??
      getPreservableSelectionSnapshot();

    if (value === "paragraph") {
    runWithPreservedTextSelection((chain) => {
      const typedChain = chain as Editor["chain"] extends () => infer T
        ? T & {
            setParagraph: () => { run: () => boolean };
          }
        : {
            setParagraph: () => { run: () => boolean };
          };

      return typedChain.setParagraph();
    }, preservedSelection);
      headingSelectionRef.current = null;
      setIsHeadingDropdownOpen(false);
      return;
    }

    const level = Number(value.replace("h", "")) as AppEditorHeadingLevel;
    runWithPreservedTextSelection((chain) => {
      const typedChain = chain as Editor["chain"] extends () => infer T
        ? T & {
            toggleHeading: (options: { level: AppEditorHeadingLevel }) => { run: () => boolean };
          }
        : {
            toggleHeading: (options: { level: AppEditorHeadingLevel }) => { run: () => boolean };
          };

      return typedChain.toggleHeading({ level });
    }, preservedSelection);
    headingSelectionRef.current = null;
    setIsHeadingDropdownOpen(false);
  }, [
    disabled,
    editor,
    getCurrentSelectionSnapshot,
    getDomSelection,
    getPreservableSelectionSnapshot,
    runWithPreservedTextSelection,
  ]);

  const handleHeadingDropdownOpenChange = useCallback((open: boolean) => {
    if (!editor) {
      setIsHeadingDropdownOpen(open);
      return;
    }

    if (open) {
      const currentSelection = getCurrentSelectionSnapshot() ?? getDomSelection();
      headingSelectionRef.current = currentSelection;
    }

    setIsHeadingDropdownOpen(open);
  }, [editor, getCurrentSelectionSnapshot, getDomSelection]);

  const handleOpenLinkPopover = useCallback((open: boolean) => {
    if (disabled || !editor) {
      setIsLinkPopoverOpen(false);
      return;
    }

    if (open) {
      linkSelectionRef.current =
        getCurrentSelectionSnapshot() ??
        getDomSelection() ??
        getPreservableSelectionSnapshot();
      const currentHref = editor.getAttributes("link").href as string | undefined;
      setLinkValue(currentHref ?? "");
    } else {
      linkSelectionRef.current = null;
    }

    setIsLinkPopoverOpen(open);
  }, [
    disabled,
    editor,
    getCurrentSelectionSnapshot,
    getDomSelection,
    getPreservableSelectionSnapshot,
  ]);

  const handleApplyLink = useCallback(() => {
    if (!editor || disabled) {
      return;
    }

    const normalizedHref = linkValue.trim();
    const preservedSelection =
      linkSelectionRef.current ??
      getCurrentSelectionSnapshot() ??
      getDomSelection() ??
      getPreservableSelectionSnapshot();

    if (!normalizedHref) {
      runWithPreservedTextSelection(
        (chain) =>
          (
            chain as Editor["chain"] extends () => infer T
              ? T & {
                  extendMarkRange: (mark: "link") => T & {
                    unsetLink: () => { run: () => boolean };
                  };
                }
              : {
                  extendMarkRange: (mark: "link") => {
                    unsetLink: () => { run: () => boolean };
                  };
                }
          ).extendMarkRange("link").unsetLink(),
        preservedSelection,
      );
      linkSelectionRef.current = null;
      setIsLinkPopoverOpen(false);
      return;
    }

    runWithPreservedTextSelection(
      (chain) =>
        (
          chain as Editor["chain"] extends () => infer T
            ? T & {
                extendMarkRange: (mark: "link") => T & {
                  setLink: (attrs: { href: string }) => { run: () => boolean };
                };
              }
            : {
                extendMarkRange: (mark: "link") => {
                  setLink: (attrs: { href: string }) => { run: () => boolean };
                };
              }
        ).extendMarkRange("link").setLink({ href: formatUrl(normalizedHref) }),
      preservedSelection,
    );

    linkSelectionRef.current = null;
    setIsLinkPopoverOpen(false);
  }, [
    disabled,
    editor,
    getCurrentSelectionSnapshot,
    getDomSelection,
    getPreservableSelectionSnapshot,
    linkValue,
    runWithPreservedTextSelection,
  ]);

  const handleOpenImagePopover = useCallback((open: boolean) => {
    if (disabled || !editor) {
      setIsImagePopoverOpen(false);
      return;
    }

    if (open) {
      captureImageInsertionSelection();
      const resolvedPosition = getResolvedImagePosition(editor);
      if (resolvedPosition !== null) {
        const resolvedNode = editor.state.doc.nodeAt(resolvedPosition);
        if (resolvedNode?.type.name === "image") {
          setLastImageCache(editor, {
            pos: resolvedPosition,
            identity: {
              imageId:
                typeof resolvedNode.attrs.imageId === "string"
                  ? resolvedNode.attrs.imageId
                  : null,
              localImageId:
                typeof resolvedNode.attrs.localImageId === "string"
                  ? resolvedNode.attrs.localImageId
                  : null,
              src: typeof resolvedNode.attrs.src === "string" ? resolvedNode.attrs.src : null,
            },
          });
        }
      }

      const currentImageAttributes =
        getResolvedImageAttributes(editor) ??
        (editor.getAttributes("image") as Record<string, unknown>);
      const currentWidth = currentImageAttributes?.width as string | undefined;
      setImageWidthValue(currentWidth ?? "");
    }

    if (!open) {
      setImageUrlValue("");
      setImageWidthValue("");
    }

    setIsImagePopoverOpen(open);
  }, [captureImageInsertionSelection, disabled, editor]);

  const handleAlignDropdownOpenChange = useCallback((open: boolean) => {
    if (!editor) {
      setIsAlignDropdownOpen(open);
      return;
    }

    if (open) {
      const currentSelection = getCurrentSelectionSnapshot() ?? getDomSelection();
      alignSelectionRef.current = currentSelection;
    }

    setIsAlignDropdownOpen(open);
  }, [editor, getCurrentSelectionSnapshot, getDomSelection]);

  const applySavedTextAlign = useCallback((align: "left" | "center" | "right" | "justify") => {
    if (!editor || disabled) {
      return;
    }

    const alignedSelectionBase = alignSelectionRef.current ?? getCurrentSelectionSnapshot() ?? getDomSelection();
    const alignedSelection = alignedSelectionBase ?? null;

    runWithPreservedTextSelection(
      (chain) =>
        (
          chain as Editor["chain"] extends () => infer T
            ? T & {
                setTextAlign: (value: "left" | "center" | "right" | "justify") => {
                  run: () => boolean;
                };
              }
            : {
                setTextAlign: (value: "left" | "center" | "right" | "justify") => { run: () => boolean };
            }
        ).setTextAlign(align),
      alignedSelection,
      true,
    );

    alignSelectionRef.current = null;
    if (!alignedSelection) {
      textSelectionRef.current = null;
      alignSelectionRef.current = null;
    }

    setIsAlignDropdownOpen(false);
  }, [disabled, editor, getCurrentSelectionSnapshot, getDomSelection, runWithPreservedTextSelection]);

  const handleApplyImageUrl = useCallback(() => {
    if (!editor || disabled) {
      return;
    }

    const insertionSelection = resolveImageInsertionSelection();

    const normalizedWidth = normalizeImageWidth(imageWidthValue);
    const normalizedSrc = imageUrlValue.trim();
    if (!normalizedSrc && hasSelectedImage) {
      updateResolvedImageAttributes(editor, {
        ...(normalizedWidth !== undefined ? { width: normalizedWidth } : {}),
      });
      setIsImagePopoverOpen(false);
      return;
    }

    if (!normalizedSrc) {
      return;
    }

    const safeSelection = insertionSelection
      ? getSafeTextSelection(insertionSelection.from, insertionSelection.to)
      : null;

    const imageId = generateEditorImageId();
      const imageAttributes = {
        imageId,
        src: formatUrl(normalizedSrc),
        ...(normalizedWidth !== undefined ? { width: normalizedWidth } : {}),
      };
    const fallbackSelection = safeSelection ?? insertionSelection;
    insertImageUrlWithFallback(imageAttributes, fallbackSelection ?? null);

    setImageUrlValue("");
    setImageWidthValue("");
    setIsImagePopoverOpen(false);
    imageInsertionSelectionRef.current = null;
  }, [
    disabled,
    editor,
    getSafeTextSelection,
    resolveImageInsertionSelection,
    hasSelectedImage,
    insertImageUrlWithFallback,
    imageUrlValue,
    imageWidthValue,
  ]);

  const handleApplyImagePreset = useCallback((preset: string) => {
    if (!editor || disabled) {
      return;
    }

    const presetWidth = normalizeImageWidth(preset);
    setImageWidthValue(preset);

    if (hasSelectedImage) {
      updateResolvedImageAttributes(editor, {
        width: presetWidth,
      });
    }
  }, [disabled, editor, hasSelectedImage]);

  const handleImageFileChange = useCallback((event: ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file || !editor || disabled) {
        return;
    }

    const insertionSelection = resolveImageInsertionSelection();

    imageInsertionSelectionRef.current = null;

    const normalizedWidth = normalizeImageWidth(imageWidthValue);
    const imageInsertionWidth =
      normalizedWidth !== undefined
        ? normalizedWidth
        : undefined;

    if (onInsertLocalImage) {
      void onInsertLocalImage(
        file,
        imageInsertionWidth,
        insertionSelection ?? undefined,
      );
    }

    event.target.value = "";
    setImageUrlValue("");
    setImageWidthValue("");
    setIsImagePopoverOpen(false);
  }, [
    disabled,
    editor,
    imageWidthValue,
    onInsertLocalImage,
    resolveImageInsertionSelection,
  ]);

  const handleImageFilePickerOpen = useCallback(() => {
    captureImageInsertionSelection();
    const fileInput = fileInputRef.current;
    if (!fileInput) {
      return;
    }

    if ("showPicker" in HTMLInputElement.prototype) {
      (fileInput as HTMLInputElement & { showPicker: () => void }).showPicker();
    } else {
      fileInput.click();
    }
  }, [captureImageInsertionSelection]);

  const handleImageButtonMouseDown = useCallback(() => {
    captureImageInsertionSelection();
  }, [captureImageInsertionSelection]);

  const linkPopoverContent = useMemo(() => (
    <div className={styles.toolbarPopoverContent}>
      <label className={styles.toolbarPopoverLabel} htmlFor="app-editor-link-url">
        URL del enlace
      </label>
      <Input
        id="app-editor-link-url"
        value={linkValue}
        onChange={(event) => setLinkValue(event.target.value)}
        placeholder="https://ejemplo.com"
        disabled={isBlocked}
      />
      <div className={styles.toolbarPopoverActions}>
        <AppButton
          variant="ghost"
          size="sm"
          onClick={() => {
            setLinkValue("");
            if (!editor || disabled) {
              return;
            }

            editor.chain().focus(undefined, { scrollIntoView: false }).extendMarkRange("link").unsetLink().run();
            setIsLinkPopoverOpen(false);
          }}
          disabled={isBlocked}
        >
          Quitar
        </AppButton>
        <AppButton variant="primary" size="sm" onClick={handleApplyLink} disabled={isBlocked}>
          Aplicar
        </AppButton>
      </div>
    </div>
  ), [editor, disabled, handleApplyLink, isBlocked, linkValue]);

  const imagePopoverContent = useMemo(() => (
    <div
      className={styles.toolbarPopoverContent}
      data-editor-snapshot-version={editorSnapshotVersion}
    >
      <label className={styles.toolbarPopoverLabel} htmlFor="app-editor-image-url">
        URL de la imagen
      </label>
      <Input
        id="app-editor-image-url"
        value={imageUrlValue}
        onChange={(event) => setImageUrlValue(event.target.value)}
        placeholder="https://cdn.ejemplo.com/imagen.png"
        disabled={isBlocked}
      />
      <label className={styles.toolbarPopoverLabel} htmlFor="app-editor-image-width">
        Ancho de la imagen
      </label>
      <Input
        id="app-editor-image-width"
        value={imageWidthValue}
        onChange={(event) => setImageWidthValue(event.target.value)}
        placeholder="100% o 480"
        disabled={isBlocked}
      />
      <div className={styles.toolbarPresetGroup} role="group" aria-label="Tamaños rapidos de imagen">
        {["25%", "50%", "75%", "100%"].map((preset) => (
          <AppButton
            key={preset}
            variant={imageWidthValue === preset ? "primary" : "ghost"}
            size="sm"
            onClick={() => handleApplyImagePreset(preset)}
            disabled={isBlocked}
          >
            {preset}
          </AppButton>
        ))}
      </div>
      {hasSelectedImage ? (
        <>
          <label className={styles.toolbarPopoverLabel}>Alineacion horizontal</label>
          <div
            className={styles.toolbarPresetGroup}
            role="group"
            aria-label="Alineacion horizontal de imagen"
          >
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Izquierda"
              onClick={() => runSetImageAlign(editor, "left")}
              disabled={isBlocked || !canSetImageAlign(editor)}
            >
              <FontAwesomeIcon icon={faAlignLeft} />
            </AppButton>
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Centro"
              onClick={() => runSetImageAlign(editor, "center")}
              disabled={isBlocked || !canSetImageAlign(editor)}
            >
              <FontAwesomeIcon icon={faAlignCenter} />
            </AppButton>
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Derecha"
              onClick={() => runSetImageAlign(editor, "right")}
              disabled={isBlocked || !canSetImageAlign(editor)}
            >
              <FontAwesomeIcon icon={faAlignRight} />
            </AppButton>
          </div>
          <label className={styles.toolbarPopoverLabel}>Mover imagen</label>
          <div
            className={styles.toolbarPresetGroup}
            role="group"
            aria-label="Movimiento vertical de imagen"
          >
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Mover arriba"
              onClick={() => moveResolvedImage(editor, "up")}
              disabled={isBlocked || !canMoveResolvedImage(editor, "up")}
            >
              <FontAwesomeIcon icon={faArrowUp} />
            </AppButton>
            <AppButton
              variant="ghost"
              size="sm"
              aria-label="Mover abajo"
              onClick={() => moveResolvedImage(editor, "down")}
              disabled={isBlocked || !canMoveResolvedImage(editor, "down")}
            >
              <FontAwesomeIcon icon={faArrowDown} />
            </AppButton>
          </div>
        </>
      ) : null}
      <div className={styles.toolbarPopoverActions}>
        <AppButton
          variant="secondary"
          size="sm"
          onMouseDown={handleImageButtonMouseDown}
          onClick={handleImageFilePickerOpen}
          disabled={isBlocked}
        >
          Cargar archivo
        </AppButton>
        <AppButton variant="primary" size="sm" onClick={handleApplyImageUrl} disabled={isBlocked}>
          {hasSelectedImage && !imageUrlValue.trim() ? "Aplicar tamaño" : "Insertar"}
        </AppButton>
      </div>
      <input
        ref={fileInputRef}
        type="file"
        accept="image/*"
        onChange={handleImageFileChange}
        className={styles.toolbarHiddenInput}
        tabIndex={-1}
      />
    </div>
    ), [
    editor,
    handleApplyImagePreset,
    handleApplyImageUrl,
    handleImageFileChange,
    handleImageButtonMouseDown,
    handleImageFilePickerOpen,
    hasSelectedImage,
    editorSnapshotVersion,
    imageUrlValue,
    imageWidthValue,
    isBlocked,
  ]);

  const renderActionButton = useCallback((button: ToolbarButtonConfig) => {
    if (button.key === "link") {
      return (
        <Popover
          key={button.key}
          content={linkPopoverContent}
          trigger="click"
          placement="bottomLeft"
          open={isLinkPopoverOpen}
          onOpenChange={handleOpenLinkPopover}
        >
          <span>
            <AppButton
              variant={button.isActive ? "primary" : "ghost"}
              size="sm"
              icon={<FontAwesomeIcon icon={button.icon} />}
              aria-label={button.label}
              tooltip={button.label}
              disabled={button.disabled}
              onMouseDown={handleImageButtonMouseDown}
              className={joinClasses(button.isActive && styles.toolbarButtonActive)}
            />
          </span>
        </Popover>
      );
    }

    if (button.key === "image") {
      return (
        <Popover
          key={button.key}
          content={imagePopoverContent}
          trigger="click"
          placement="bottomLeft"
          open={isImagePopoverOpen}
          onOpenChange={handleOpenImagePopover}
        >
          <span>
            <AppButton
              variant={button.isActive ? "primary" : "ghost"}
              size="sm"
              icon={<FontAwesomeIcon icon={button.icon} />}
              aria-label={button.label}
              tooltip={button.label}
              disabled={button.disabled}
              className={joinClasses(button.isActive && styles.toolbarButtonActive)}
            />
          </span>
        </Popover>
      );
    }

    return (
      <AppButton
        key={button.key}
        variant={button.isActive ? "primary" : "ghost"}
        size="sm"
        icon={<FontAwesomeIcon icon={button.icon} />}
        aria-label={button.label}
        tooltip={button.label}
        disabled={button.disabled}
        onClick={button.onClick}
        className={joinClasses(button.isActive && styles.toolbarButtonActive)}
      />
    );
    }, [
      handleImageButtonMouseDown,
      handleOpenImagePopover,
      handleOpenLinkPopover,
      imagePopoverContent,
      isImagePopoverOpen,
      isLinkPopoverOpen,
      linkPopoverContent,
    ]);

  const groupedButtons = useMemo(() => {
    const allButtons: ToolbarButtonConfig[] = [
      {
        key: "bold",
        label: "Negrita",
        icon: faBold,
        isActive: editor?.isActive("bold"),
        disabled:
          isBlocked ||
          !canRun(editor, (instance) =>
          instance.can().chain().focus(undefined, { scrollIntoView: false }).toggleBold().run(),
          ),
        onClick: () => runWithPreservedTextSelection((chain) => chain.toggleBold()),
      },
      {
        key: "italic",
        label: "Cursiva",
        icon: faItalic,
        isActive: editor?.isActive("italic"),
        disabled:
          isBlocked ||
          !canRun(editor, (instance) =>
          instance.can().chain().focus(undefined, { scrollIntoView: false }).toggleItalic().run(),
          ),
        onClick: () => runWithPreservedTextSelection((chain) => chain.toggleItalic()),
      },
      {
        key: "underline",
        label: "Subrayado",
        icon: faUnderline,
        isActive: editor?.isActive("underline"),
        disabled:
          isBlocked ||
          !canRun(editor, (instance) =>
          instance.can().chain().focus(undefined, { scrollIntoView: false }).toggleUnderline().run(),
          ),
        onClick: () => runWithPreservedTextSelection((chain) => chain.toggleUnderline()),
      },
      {
        key: "bullet-list",
        label: "Lista con vietas",
        icon: faListUl,
        isActive: editor?.isActive("bulletList"),
        disabled:
          isBlocked ||
          !canRun(editor, (instance) =>
          instance.can().chain().focus(undefined, { scrollIntoView: false }).toggleBulletList().run(),
          ),
        onClick: () => runWithPreservedTextSelection((chain) => chain.toggleBulletList()),
      },
      {
        key: "ordered-list",
        label: "Lista numerada",
        icon: faListOl,
        isActive: editor?.isActive("orderedList"),
        disabled:
          isBlocked ||
          !canRun(editor, (instance) =>
          instance.can().chain().focus(undefined, { scrollIntoView: false }).toggleOrderedList().run(),
          ),
        onClick: () => runWithPreservedTextSelection((chain) => chain.toggleOrderedList()),
      },
      {
        key: "task-list",
        label: "Lista de tareas",
        icon: faListCheck,
        isActive: editor?.isActive("taskList"),
        disabled:
          isBlocked ||
          !canRun(editor, (instance) =>
          instance.can().chain().focus(undefined, { scrollIntoView: false }).toggleTaskList().run(),
          ),
        onClick: () => runWithPreservedTextSelection((chain) => chain.toggleTaskList()),
      },
      {
        key: "undo",
        label: "Deshacer",
        icon: faArrowRotateLeft,
        disabled: isBlocked,
        onClick: () => runHistoryCommand(editor, "undo"),
      },
      {
        key: "redo",
        label: "Rehacer",
        icon: faArrowRotateRight,
        disabled: isBlocked,
        onClick: () => runHistoryCommand(editor, "redo"),
      },
      {
        key: "link",
        label: "Insertar enlace",
        icon: faLink,
        isActive: editor?.isActive("link"),
        disabled: isBlocked,
        onClick: () => undefined,
      },
      {
        key: "image",
        label: "Insertar imagen",
        icon: faImage,
        disabled: isBlocked,
        onClick: () => undefined,
      },
    ];

    return {
      formatting: [allButtons[0], allButtons[1], allButtons[2]],
      structure: [allButtons[3], allButtons[4], allButtons[5]],
      history: [allButtons[6], allButtons[7]],
      link: [allButtons[8]],
      image: [allButtons[9]],
    };
  }, [editor, isBlocked, runWithPreservedTextSelection]);

  const currentHeading = useMemo(
    () => getHeadingOption(getCurrentHeadingValue(editor)),
    [editor],
  );
  const currentTextAlign = useMemo(
    () => getTextAlignMeta(getCurrentTextAlign(editor)),
    [editor],
  );
  const headingItems = useMemo(() => HEADING_OPTIONS.map((option) => ({
    key: option.value,
    label: option.label,
    leftIcon: <FontAwesomeIcon icon={option.icon} />,
    onSelect: () => handleHeadingChange(option.value),
  })), [handleHeadingChange]);
  const alignItems = useMemo(() => [
    {
      key: "align-left",
      label: "Izquierda",
      leftIcon: <FontAwesomeIcon icon={faAlignLeft} />,
      onSelect: () => applySavedTextAlign("left"),
    },
    {
      key: "align-center",
      label: "Centro",
      leftIcon: <FontAwesomeIcon icon={faAlignCenter} />,
      onSelect: () => applySavedTextAlign("center"),
    },
    {
      key: "align-right",
      label: "Derecha",
      leftIcon: <FontAwesomeIcon icon={faAlignRight} />,
      onSelect: () => applySavedTextAlign("right"),
    },
    {
      key: "align-justify",
      label: "Justificar",
      leftIcon: <FontAwesomeIcon icon={faAlignJustify} />,
      onSelect: () => applySavedTextAlign("justify"),
    },
  ], [applySavedTextAlign]);
  const renderButtonGroup = useCallback((group: ToolbarButtonConfig[], label: string) => (
    <div className={styles.toolbarButtonGroup} role="group" aria-label={label}>
      {group.map((button) => renderActionButton(button))}
    </div>
  ), [renderActionButton]);
  const toolbarOverflowContent = useMemo(() => (
    <div className={styles.toolbarOverflowPanel} role="group" aria-label="Mas acciones del editor">
      <div className={styles.toolbarOverflowPanelRow}>
        <AppDropdown
          open={isHeadingDropdownOpen}
          onOpenChange={handleHeadingDropdownOpenChange}
          dropdownProps={{
            forceRender: true,
            destroyOnHidden: false,
            mouseEnterDelay: 0,
            mouseLeaveDelay: 0,
            transitionName: "",
          }}
          trigger={
            <AppButton
              variant="ghost"
              size="sm"
              leftIcon={<FontAwesomeIcon icon={currentHeading.icon} />}
              rightIcon={<FontAwesomeIcon icon={faChevronDown} />}
              className={styles.headingButton}
              aria-label={`Nivel de encabezado actual: ${currentHeading.label}`}
            >
              {currentHeading.shortLabel}
            </AppButton>
          }
          items={headingItems}
          disabled={isBlocked}
          className={styles.headingDropdown}
          ariaLabel="Nivel de encabezado"
          placement="bottomLeft"
        />
      </div>
      {renderButtonGroup(groupedButtons.formatting, "Formato de texto")}
      {renderButtonGroup(groupedButtons.structure, "Listas")}
      <div className={styles.toolbarButtonGroup} role="group" aria-label="Alineacion">
        <AppDropdown
          open={isAlignDropdownOpen}
          onOpenChange={handleAlignDropdownOpenChange}
          dropdownProps={{
            forceRender: true,
            destroyOnHidden: false,
            mouseEnterDelay: 0,
            mouseLeaveDelay: 0,
            transitionName: "",
          }}
          trigger={
            <AppButton
              variant="ghost"
              size="sm"
              leftIcon={<FontAwesomeIcon icon={currentTextAlign.icon} />}
              rightIcon={<DownOutlined />}
              className={styles.alignButton}
              aria-label={`Alineacion actual: ${currentTextAlign.label}`}
            >
              <span className={styles.toolbarCompactLabel}> </span>
            </AppButton>
          }
          items={alignItems}
          disabled={isBlocked}
          ariaLabel="Alineacion de texto"
          placement="bottomLeft"
        />
      </div>
      {renderButtonGroup(groupedButtons.history, "Historial de cambios")}
      {renderButtonGroup(groupedButtons.link, "Enlaces")}
      {renderButtonGroup(groupedButtons.image, "Imagenes")}
      {toolbarActions ? (
        <div className={styles.toolbarButtonGroup} role="group" aria-label="Acciones del editor">
          {toolbarActions}
        </div>
      ) : null}
    </div>
  ), [
    alignItems,
    currentHeading.icon,
    currentHeading.label,
    currentHeading.shortLabel,
    currentTextAlign.icon,
    currentTextAlign.label,
    groupedButtons.formatting,
    groupedButtons.history,
    groupedButtons.image,
    groupedButtons.link,
    groupedButtons.structure,
    handleAlignDropdownOpenChange,
    handleHeadingDropdownOpenChange,
    headingItems,
    isAlignDropdownOpen,
    isBlocked,
    isHeadingDropdownOpen,
    renderButtonGroup,
    toolbarActions,
  ]);

  return (
    <div
      className={styles.toolbar}
      role="toolbar"
      aria-label="Barra de herramientas del editor"
      data-toolbar-mode={isCompactToolbar ? "compact" : "default"}
      onMouseDownCapture={handleToolbarMouseDownCapture}
    >
      <div className={styles.toolbarSection} data-group="heading">
        <AppDropdown
          open={isHeadingDropdownOpen}
          onOpenChange={handleHeadingDropdownOpenChange}
          dropdownProps={{
            forceRender: true,
            destroyOnHidden: false,
            mouseEnterDelay: 0,
            mouseLeaveDelay: 0,
            transitionName: "",
          }}
          trigger={
            <AppButton
              variant="ghost"
              size="sm"
              leftIcon={<FontAwesomeIcon icon={currentHeading.icon} />}
              rightIcon={<FontAwesomeIcon icon={faChevronDown} />}
              className={styles.headingButton}
              aria-label={`Nivel de encabezado actual: ${currentHeading.label}`}
            >
              {currentHeading.shortLabel}
            </AppButton>
          }
          items={headingItems}
          disabled={isBlocked}
          className={styles.headingDropdown}
          ariaLabel="Nivel de encabezado"
          placement="bottomLeft"
        />
      </div>

      <div className={styles.toolbarSection} data-group="actions">
        <div className={styles.toolbarPrimaryActions}>
          {/* eslint-disable-next-line react-hooks/refs -- button configs only pass event handlers; refs are read inside those handlers. */}
          {renderButtonGroup(groupedButtons.formatting, "Formato de texto")}
        </div>
        <div className={styles.toolbarDesktopActions}>
          {/* eslint-disable-next-line react-hooks/refs -- button configs only pass event handlers; refs are read inside those handlers. */}
          {renderButtonGroup(groupedButtons.structure, "Listas")}
          <div className={styles.toolbarButtonGroup} role="group" aria-label="Alineacion">
            <AppDropdown
              open={isAlignDropdownOpen}
              onOpenChange={handleAlignDropdownOpenChange}
              dropdownProps={{
                forceRender: true,
                destroyOnHidden: false,
                mouseEnterDelay: 0,
                mouseLeaveDelay: 0,
                transitionName: "",
              }}
              trigger={
                <AppButton
                  variant="ghost"
                  size="sm"
                  leftIcon={<FontAwesomeIcon icon={currentTextAlign.icon} />}
                  rightIcon={<DownOutlined />}
                  className={styles.alignButton}
                  aria-label={`Alineacion actual: ${currentTextAlign.label}`}
                >
                  <span className={styles.toolbarCompactLabel}> </span>
                </AppButton>
              }
              items={alignItems}
              disabled={isBlocked}
              ariaLabel="Alineacion de texto"
              placement="bottomLeft"
            />
          </div>
          {/* eslint-disable-next-line react-hooks/refs -- button configs only pass event handlers; refs are read inside those handlers. */}
          {renderButtonGroup(groupedButtons.history, "Historial de cambios")}
        </div>
        <Popover
          content={toolbarOverflowContent}
          trigger="click"
          placement="bottomLeft"
        >
          <span className={styles.toolbarOverflowActions}>
            <AppButton
              variant="ghost"
              size="sm"
              icon={<MoreOutlined />}
              className={styles.toolbarOverflowButton}
              aria-label="Mas acciones del editor"
              tooltip="Mas acciones"
              disabled={isBlocked}
            />
          </span>
        </Popover>
        {/* eslint-disable-next-line react-hooks/refs -- button configs only pass event handlers; refs are read inside those handlers. */}
        <div className={styles.toolbarLinkActions}>
          {renderButtonGroup(groupedButtons.link, "Enlaces")}
        </div>
        {/* eslint-disable-next-line react-hooks/refs -- button configs only pass event handlers; refs are read inside those handlers. */}
        <div className={styles.toolbarImageActions}>
          {renderButtonGroup(groupedButtons.image, "Imagenes")}
        </div>
        {toolbarActions ? (
          <div className={styles.toolbarExternalActions}>
            <div className={styles.toolbarButtonGroup} role="group" aria-label="Acciones del editor">
              {toolbarActions}
            </div>
          </div>
        ) : null}
      </div>
      {trailingContent ? (
        <div className={joinClasses(styles.toolbarSection, styles.toolbarSectionEnd)} data-group="view">
          {trailingContent}
        </div>
      ) : null}
    </div>
  );
}

export const AppEditorToolbar = memo(AppEditorToolbarComponent);

