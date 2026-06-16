import { useCallback, useEffect, useRef } from "react";
import { useEditor } from "@tiptap/react";
import type { Editor } from "@tiptap/react";
import { NodeSelection, TextSelection, type Transaction } from "@tiptap/pm/state";
import type { UseAppEditorOptions, UseAppEditorResult } from "../domain/editor.types";
import {
  clampSelection,
  normalizeEditorValue,
  createSafeTextSelectionFromRange,
  resolveSafeTextSelectionRange,
} from "../domain/editor.model";
import { createAppEditorConfig } from "../infrastructure/tiptap.config";
import { generateEditorImageId, generateLocalImageId } from "./localImageIds";
import { appEditorImageStore } from "../infrastructure/indexeddb/appEditorImageStore";
import type { LocalImage } from "../infrastructure/indexeddb/localImage.types";
import { normalizeEditorHtml } from "./normalizeEditorHtml";
import {
  serializeVisualPageHtml,
} from "./pageDocument";
import {
  resolveAutoPageBreakActions,
} from "./autoPagination";
import { normalizeImageWidth } from "./imageSizing";

const AUTO_PAGINATION_DEBOUNCE_MS = 20;
const AUTO_PAGINATION_TYPING_DEBOUNCE_MS = 24;
const AUTO_PAGINATION_IMAGE_RESIZE_DEBOUNCE_MS = 0;
const IMAGE_INTERACTION_LOCK_MS = 600;

type EditorWithAppHistory = Editor & {
  appEditorHistory?: {
    undo?: () => boolean;
    redo?: () => boolean;
  };
};

type HistorySelectionSnapshot =
  | {
      type: "node";
      from: number;
      to: number;
      imageIdentity:
        | {
            imageId: string | null;
            localImageId: string | null;
            src: string | null;
          }
        | null;
    }
  | {
      type: "text";
      from: number;
      to: number;
    };

function resolveAutoPaginationDebounceMs() {
  if (
    typeof navigator !== "undefined" &&
    typeof navigator.userAgent === "string" &&
    /jsdom/i.test(navigator.userAgent)
  ) {
    return 0;
  }

  return AUTO_PAGINATION_DEBOUNCE_MS;
}

function findScrollableAncestor(element: HTMLElement | null) {
  const explicitScrollContainer = element?.closest("[data-app-editor-scroll-container='true']");

  if (explicitScrollContainer instanceof HTMLElement) {
    return explicitScrollContainer;
  }

  return null;
}

function syncControlledValue(editor: Editor, nextValue: string) {
  const editorHtml = editor.getHTML();
  const currentValue = normalizeEditorValue(
    normalizeEditorHtml(serializeVisualPageHtml(editorHtml)),
  );
  const normalizedNextValue = normalizeEditorValue(
    normalizeEditorHtml(serializeVisualPageHtml(nextValue)),
  );

  if (currentValue === normalizedNextValue) {
    return;
  }

  const { from, to } = editor.state.selection;

  editor.commands.setContent(normalizedNextValue, { emitUpdate: false });
  if (!dispatchSafeSelection(editor, from, to)) {
    return;
  }
}

function setSafeTextSelectionOnTransaction(
  transaction: Transaction,
  from: number,
  to = from,
) {
  const safeSelection = createSafeTextSelectionFromRange(transaction.doc, from, to);
  if (!safeSelection) {
    return transaction;
  }

  try {
    return transaction.setSelection(safeSelection);
  } catch {
    return transaction;
  }
}

function createHistorySelectionSnapshot(editor: Editor): HistorySelectionSnapshot {
  const currentSelection = editor.state.selection;

  if (currentSelection instanceof NodeSelection) {
    const selectedNode = currentSelection.node;
    return {
      type: "node",
      from: clampSelection(currentSelection.from, editor.state.doc.content.size),
      to: clampSelection(currentSelection.to, editor.state.doc.content.size),
      imageIdentity:
        selectedNode.type.name === "image"
          ? {
              imageId:
                typeof selectedNode.attrs.imageId === "string"
                  ? selectedNode.attrs.imageId
                  : null,
              localImageId:
                typeof selectedNode.attrs.localImageId === "string"
                  ? selectedNode.attrs.localImageId
                  : null,
              src:
                typeof selectedNode.attrs.src === "string"
                  ? selectedNode.attrs.src
                  : null,
            }
          : null,
    };
  }

  return {
    type: "text",
    from: clampSelection(currentSelection.from, editor.state.doc.content.size),
    to: clampSelection(currentSelection.to, editor.state.doc.content.size),
  };
}

function resolveImagePositionFromHistorySnapshot(
  editor: Editor,
  snapshot: Extract<HistorySelectionSnapshot, { type: "node" }>,
) {
  const nodeAtPreviousPosition = editor.state.doc.nodeAt(snapshot.from);
  if (nodeAtPreviousPosition?.type.name === "image") {
    return snapshot.from;
  }

  const identity = snapshot.imageIdentity;
  if (!identity) {
    return null;
  }

  let matchedPosition: number | null = null;
  editor.state.doc.descendants((node, pos) => {
    if (matchedPosition !== null || node.type.name !== "image") {
      return false;
    }

    const imageId = typeof node.attrs.imageId === "string" ? node.attrs.imageId : null;
    const localImageId =
      typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null;
    const src = typeof node.attrs.src === "string" ? node.attrs.src : null;

    if (
      (identity.imageId && identity.imageId === imageId) ||
      (identity.localImageId && identity.localImageId === localImageId) ||
      (identity.src && identity.src === src)
    ) {
      matchedPosition = pos;
      return false;
    }

    return undefined;
  });

  return matchedPosition;
}

function restoreHistorySelection(editor: Editor, snapshot: HistorySelectionSnapshot) {
  if (snapshot.type === "node") {
    const imagePosition = resolveImagePositionFromHistorySnapshot(editor, snapshot);
    if (imagePosition !== null) {
      try {
        editor.view.dispatch(
          editor.state.tr.setSelection(NodeSelection.create(editor.state.doc, imagePosition)),
        );
        return true;
      } catch {
        return false;
      }
    }
  }

  const safeSelection = resolveSafeTextSelectionRange(
    editor.state.doc,
    clampSelection(snapshot.from, editor.state.doc.content.size),
    clampSelection(snapshot.to, editor.state.doc.content.size),
  );
  if (!safeSelection) {
    return false;
  }

  editor.view.dispatch(
    setSafeTextSelectionOnTransaction(
      editor.state.tr,
      safeSelection.from,
      safeSelection.to,
    ),
  );
  return true;
}

function resolveDocTextLength(doc: Editor["state"]["doc"]): number {
  try {
    return doc.textBetween(0, doc.content.size, "", "").length;
  } catch {
    return doc.content.size;
  }
}

function getStepInsertedSize(step: unknown) {
  const directSlice = typeof (step as { slice?: { size?: unknown } })?.slice === "object"
    ? (step as { slice?: { size?: unknown } }).slice
    : null;
  if (directSlice && typeof directSlice.size === "number") {
    return directSlice.size;
  }

  const stepAsJson = typeof (step as { toJSON?: () => unknown })?.toJSON === "function"
    ? (step as { toJSON: () => unknown }).toJSON()
    : null;
  if (!stepAsJson || typeof stepAsJson !== "object") {
    return null;
  }

  const jsonSlice = (stepAsJson as { slice?: { size?: unknown } }).slice;
  if (jsonSlice && typeof jsonSlice === "object" && typeof jsonSlice.size === "number") {
    return jsonSlice.size;
  }

  return null;
}

function dispatchSafeSelection(
  editor: Editor,
  from: number,
  to = from,
) {
  const safeSelection = resolveSafeTextSelectionRange(editor.state.doc, from, to);
  if (!safeSelection) {
    return false;
  }

  try {
    editor.view.dispatch(
      setSafeTextSelectionOnTransaction(
        editor.state.tr,
        safeSelection.from,
        safeSelection.to,
      ),
    );
  } catch {
    return false;
  }

  return true;
}

function resolveSafeSelectionRangeFromSelection(
  editor: Editor,
  selectionFrom: number,
  selectionTo: number,
) {
  const maxSelectionPosition = editor.state.doc.content.size;
  return resolveSafeTextSelectionRange(
    editor.state.doc,
    clampSelection(selectionFrom, maxSelectionPosition),
    clampSelection(selectionTo, maxSelectionPosition),
  );
}

function collectLocalImageIds(editor: Editor) {
  const ids = new Set<string>();

  editor.state.doc.descendants((node) => {
    if (node.type.name !== "image") {
      return;
    }

    const localImageId = node.attrs.localImageId as string | null | undefined;
    if (localImageId) {
      ids.add(localImageId);
    }
  });

  return ids;
}

function buildLocalImageScope() {
  return {
    sessionId:
      typeof crypto !== "undefined" && typeof crypto.randomUUID === "function"
        ? crypto.randomUUID()
        : `session-${Date.now()}-${Math.random().toString(16).slice(2)}`,
  };
}

function getImageIdentityFromAttrs(attrs: Record<string, unknown> | null | undefined) {
  if (!attrs) {
    return null;
  }

  return {
    imageId: typeof attrs.imageId === "string" ? attrs.imageId : null,
    localImageId: typeof attrs.localImageId === "string" ? attrs.localImageId : null,
    src: typeof attrs.src === "string" ? attrs.src : null,
  };
}

type EditorWithImageSelectionState = Editor & {
  __appEditorLastImagePos?: number | null;
  __appEditorLastImageIdentity?: {
    imageId?: string | null;
    localImageId?: string | null;
    src?: string | null;
  } | null;
};

type ImageInsertionResult = {
  inserted: boolean;
  position: number | null;
};

function findImagePositionByIdentity(
  editor: Editor,
  identity:
    | {
        localImageId?: string | null;
        imageId?: string | null;
        src?: string | null;
      }
    | null
) {
  if (!identity) {
    return null;
  }

  let matchedPosition: number | null = null;

  editor.state.doc.descendants((node, pos) => {
    if (matchedPosition !== null || node.type.name !== "image") {
      return false;
    }

    const nodeImageId =
      typeof node.attrs.imageId === "string" ? node.attrs.imageId : null;
    const nodeLocalImageId =
      typeof node.attrs.localImageId === "string" ? node.attrs.localImageId : null;
    const nodeSrc = typeof node.attrs.src === "string" ? node.attrs.src : null;
    const matchesImageId = identity.imageId && nodeImageId === identity.imageId;
    const matchesLocalImageId =
      identity.localImageId && nodeLocalImageId === identity.localImageId;
    const matchesSrc = identity.src && nodeSrc === identity.src;

    if (matchesImageId || matchesLocalImageId || matchesSrc) {
      matchedPosition = pos;
      return false;
    }

    return undefined;
  });

  return matchedPosition;
}

function resolveImageNodeAfterInsertionPosition(
  editor: Editor,
  imageIdentity: {
    imageId?: string | null;
    localImageId?: string | null;
    src?: string | null;
  } | null,
  fallbackPosition: number | null,
) {
  const matchedPosition = imageIdentity
    ? findImagePositionByIdentity(editor, imageIdentity)
    : null;
  const anchorPosition =
    matchedPosition !== null ? matchedPosition : fallbackPosition;

  if (typeof anchorPosition !== "number") {
    return null;
  }

  const imageNode = editor.state.doc.nodeAt(anchorPosition);
  if (!imageNode || imageNode.type.name !== "image") {
    return clampSelection(Math.min(editor.state.doc.content.size, Math.max(0, anchorPosition)), editor.state.doc.content.size);
  }

  return clampSelection(
    Math.min(
      editor.state.doc.content.size,
      Math.max(0, anchorPosition + imageNode.nodeSize),
    ),
    editor.state.doc.content.size,
  );
}

function resolveSafeImageInsertionSelection(
  editor: Editor,
  explicitSelection?: {
    from: number;
    to: number;
  },
) {
  const doc = editor.state.doc;
  const maxContentSize = Math.max(0, doc.content.size);
  const selection = editor.state.selection;

  const baseCandidates = explicitSelection
    ? [
        clampSelection(explicitSelection.from, maxContentSize),
        clampSelection(explicitSelection.to, maxContentSize),
      ]
    : [];

  const currentSelectionCandidates =
    selection instanceof NodeSelection
      ? [
          clampSelection(selection.to, maxContentSize),
          clampSelection(selection.to + 1, maxContentSize),
          clampSelection(selection.to - 1, maxContentSize),
        ]
      : [
          clampSelection(selection.from, maxContentSize),
          clampSelection(selection.to, maxContentSize),
          clampSelection(selection.from + 1, maxContentSize),
          clampSelection(selection.to + 1, maxContentSize),
          clampSelection(selection.from - 1, maxContentSize),
          clampSelection(selection.to - 1, maxContentSize),
        ];

  const fallbackCandidates = [
    0,
    1,
    Math.max(0, maxContentSize - 1),
    maxContentSize,
  ];
  const uniqueCandidates = [
    ...new Set([
      ...baseCandidates,
      ...currentSelectionCandidates,
      ...fallbackCandidates,
    ]),
  ];

  for (const candidate of uniqueCandidates) {
    const safeCandidate = resolveSafeTextSelectionRange(doc, candidate, candidate);
    if (safeCandidate) {
      return safeCandidate;
    }

    const safeMax = Math.max(0, doc.content.size - 1);
    const resolvedCandidate = clampSelection(candidate, safeMax);

    try {
      const resolved = doc.resolve(resolvedCandidate);
      const nearSelection = TextSelection.near(resolved, -1);
      const safeNearSelection = resolveSafeTextSelectionRange(
        doc,
        nearSelection.from,
        nearSelection.to,
      );

      if (safeNearSelection) {
        return safeNearSelection;
      }
    } catch {
      continue;
    }
  }

  const safeFallbackPosition = clampSelection(
    Math.max(0, Math.min(1, maxContentSize)),
    maxContentSize,
  );

  return {
    from: safeFallbackPosition,
    to: safeFallbackPosition,
  };
}

function syncActiveImageIndicator(editor: Editor) {
  const proseMirror = editor.view.dom;

  if (!(proseMirror instanceof HTMLElement)) {
    return;
  }

  const selection = editor.state.selection;
  const selectedImagePosition =
    selection instanceof NodeSelection && selection.node.type.name === "image"
      ? selection.from
      : (editor as EditorWithImageSelectionState).__appEditorLastImagePos ?? null;
  const selectedImageNode =
    typeof selectedImagePosition === "number" ? editor.state.doc.nodeAt(selectedImagePosition) : null;
  const selectedImageIdentity =
    selectedImageNode?.type.name === "image"
      ? getImageIdentityFromAttrs(selectedImageNode.attrs)
      : (editor as EditorWithImageSelectionState).__appEditorLastImageIdentity ?? null;

  Array.from(proseMirror.querySelectorAll("[data-app-editor-image-node='true']")).forEach(
    (node) => {
      if (!(node instanceof HTMLElement)) {
        return;
      }

      const nodeImageId = node.getAttribute("data-image-id");
      const nodeLocalImageId = node.getAttribute("data-local-image-id");
      const nodeSrc = node.getAttribute("data-src");
      const matchesIdentity =
        selectedImageIdentity &&
        ((selectedImageIdentity.imageId &&
          nodeImageId === selectedImageIdentity.imageId) ||
          (selectedImageIdentity.localImageId &&
            nodeLocalImageId === selectedImageIdentity.localImageId) ||
          (selectedImageIdentity.src && nodeSrc === selectedImageIdentity.src));

      if (matchesIdentity) {
        setImageNodeActiveState(node, true);
      } else {
        setImageNodeActiveState(node, false);
      }
    },
  );
}

function setImageNodeActiveState(node: HTMLElement, isActive: boolean) {
  if (isActive) {
    node.setAttribute("data-app-editor-image-active", "true");
    node.setAttribute("data-app-editor-image-persistent", "true");
  } else {
    node.removeAttribute("data-app-editor-image-active");
    node.removeAttribute("data-app-editor-image-persistent");
  }

  const image = node.querySelector("img");
  if (!(image instanceof HTMLImageElement)) {
    return;
  }

  if (isActive) {
    image.setAttribute("data-app-editor-image-active", "true");
    image.setAttribute("data-app-editor-image-persistent", "true");
  } else {
    image.removeAttribute("data-app-editor-image-active");
    image.removeAttribute("data-app-editor-image-persistent");
  }
}

function observeProseMirrorBlocks(
  root: HTMLElement,
  blockResizeObserver: ResizeObserver | null,
) {
  if (!blockResizeObserver) {
    return;
  }

  Array.from(root.children).forEach((child) => {
    if (!(child instanceof HTMLElement)) {
      return;
    }

    blockResizeObserver.observe(child);
  });
}

export function useAppEditor({
  value,
  defaultValue,
  onChange,
  placeholder,
  disabled = false,
  readOnly = false,
  paginationMode = "none",
  pageHeight = 0,
  pageGap = 0,
  pageMargins,
  zoomLevel = 1,
}: UseAppEditorOptions): UseAppEditorResult {
  const isControlled = value !== undefined;
  const externalSourceContent = isControlled ? value : defaultValue;
  const externalInitialContent = normalizeEditorValue(
    normalizeEditorHtml(serializeVisualPageHtml(externalSourceContent)),
  );
  const usePaginatedDocument = false;
  const initialContentRef = useRef(externalInitialContent);
  const lastKnownValueRef = useRef(externalInitialContent);
  const localImageUrlsRef = useRef(new Map<string, string>());
  const localImageScopeRef = useRef(buildLocalImageScope());
  const localImageSyncTokenRef = useRef(0);
  const lastImageInteractionAtRef = useRef(0);
  const lastSelectionInteractionAtRef = useRef(0);
  const pendingControlledValueRef = useRef<string | null>(null);
  const isPointerSelectingRef = useRef(false);
  const logicalHistoryDoneRef = useRef<string[]>([externalInitialContent]);
  const logicalHistoryUndoneRef = useRef<string[]>([]);
  const logicalHistoryApplyingRef = useRef(false);
  const isUserScrollingRef = useRef(false);
  const manualScrollActivityTimerRef = useRef<number | null>(null);
  const previousSelectionRangeRef = useRef<{ from: number; to: number } | null>(null);
  const lastTypingTransactionAtRef = useRef(0);
  const pendingOnChangeFrameRef = useRef<number | null>(null);
  const pendingOnChangeValueRef = useRef<string | null>(null);

  const editor = useEditor(
    {
      ...createAppEditorConfig({
        content: initialContentRef.current,
        placeholder,
        editable: !(disabled || readOnly),
        paginatedDocument: usePaginatedDocument,
        onUpdate: ({ editor: currentEditor }) => {
          const nextValue = normalizeEditorValue(
            normalizeEditorHtml(serializeVisualPageHtml(currentEditor.getHTML())),
          );
          const isValueUpdated = nextValue !== lastKnownValueRef.current;

          if (
            localImageUrlsRef.current.size > 0 &&
            isValueUpdated
          ) {
            const currentLocalImageIds = collectLocalImageIds(currentEditor);

            for (const [localImageId, url] of localImageUrlsRef.current.entries()) {
              if (!currentLocalImageIds.has(localImageId)) {
                URL.revokeObjectURL(url);
                localImageUrlsRef.current.delete(localImageId);
              }
            }
          }

          if (!isValueUpdated) {
            return;
          }

          lastKnownValueRef.current = nextValue;
          pendingOnChangeValueRef.current = nextValue;

          if (onChange === undefined) {
            return;
          }

          if (pendingOnChangeFrameRef.current !== null) {
            return;
          }

          pendingOnChangeFrameRef.current = window.requestAnimationFrame(() => {
            pendingOnChangeFrameRef.current = null;
            const latestValue = pendingOnChangeValueRef.current;
            if (latestValue === null) {
              return;
            }

            onChange(latestValue);
          });
        },
        shouldPreventScrollToSelection: () =>
          isUserScrollingRef.current || logicalHistoryApplyingRef.current,
      }),
      immediatelyRender: false,
      shouldRerenderOnTransaction: false,
      },
    [usePaginatedDocument],
  );

  const rehydrateLocalImages = useCallback(async () => {
    if (!editor) {
      return;
    }

    const syncToken = ++localImageSyncTokenRef.current;
    const updates: Array<{ pos: number; attrs: Record<string, unknown> }> = [];
    const currentLocalImageIds = new Set<string>();

    const imageNodes: Array<{ pos: number; attrs: Record<string, unknown> }> = [];
    editor.state.doc.descendants((node, pos) => {
      if (node.type.name !== "image") {
        return;
      }

      imageNodes.push({ pos, attrs: { ...node.attrs } });
    });

    for (const imageNode of imageNodes) {
      const localImageId = imageNode.attrs.localImageId as string | null | undefined;
      const source = imageNode.attrs.source as string | null | undefined;

      if (!localImageId || source !== "local") {
        continue;
      }

      currentLocalImageIds.add(localImageId);
      const previousUrl = localImageUrlsRef.current.get(localImageId);

      if (previousUrl) {
        if (imageNode.attrs.src !== previousUrl) {
          updates.push({
            pos: imageNode.pos,
            attrs: {
              ...imageNode.attrs,
              src: previousUrl,
            },
          });
        }

        continue;
      }

      const localImage = await appEditorImageStore.getImage(localImageId);

      if (!localImage || syncToken !== localImageSyncTokenRef.current) {
        continue;
      }

      const nextUrl = URL.createObjectURL(localImage.blob);
      localImageUrlsRef.current.set(localImageId, nextUrl);

      if (imageNode.attrs.src !== nextUrl) {
        updates.push({
          pos: imageNode.pos,
          attrs: {
            ...imageNode.attrs,
            src: nextUrl,
          },
        });
      }
    }

    for (const [localImageId, url] of localImageUrlsRef.current.entries()) {
      if (!currentLocalImageIds.has(localImageId)) {
        URL.revokeObjectURL(url);
        localImageUrlsRef.current.delete(localImageId);
      }
    }

    if (!editor || syncToken !== localImageSyncTokenRef.current || updates.length === 0) {
      return;
    }

    const transaction = editor.state.tr;

    for (const update of updates) {
      transaction.setNodeMarkup(update.pos, undefined, update.attrs);
    }

    editor.view.dispatch(transaction);
  }, [editor]);

  useEffect(() => {
    if (!editor) {
      return undefined;
    }

    logicalHistoryDoneRef.current = [
      normalizeEditorValue(
        normalizeEditorHtml(serializeVisualPageHtml(editor.getHTML())),
      ),
    ];
    logicalHistoryUndoneRef.current = [];

    const readCurrentHistoryValue = () =>
      normalizeEditorValue(
        normalizeEditorHtml(serializeVisualPageHtml(editor.getHTML())),
      );

    const syncLogicalHistoryWithEditor = (replaceCurrent = false) => {
      const nextValue = readCurrentHistoryValue();
      const currentValue =
        logicalHistoryDoneRef.current.length > 0
          ? logicalHistoryDoneRef.current[logicalHistoryDoneRef.current.length - 1]
          : initialContentRef.current;

      if (nextValue === currentValue) {
        return;
      }

      if (replaceCurrent) {
        if (logicalHistoryDoneRef.current.length === 0) {
          logicalHistoryDoneRef.current.push(nextValue);
        } else {
          logicalHistoryDoneRef.current[logicalHistoryDoneRef.current.length - 1] = nextValue;
        }
        return;
      }

      logicalHistoryDoneRef.current.push(nextValue);
      logicalHistoryUndoneRef.current = [];
    };

    const historyAwareEditor = editor as EditorWithAppHistory;
    const previousAppHistory = historyAwareEditor.appEditorHistory;

    const handleTransaction = ({
      transaction,
    }: {
      transaction: {
        docChanged?: boolean;
        getMeta?: (key: string) => unknown;
      };
    }) => {
      if (!transaction.docChanged) {
        return;
      }

      if (logicalHistoryApplyingRef.current) {
        return;
      }

      syncLogicalHistoryWithEditor(
        transaction.getMeta?.("appEditorPagination") === true,
      );
    };

    const applyHistorySnapshot = (value: string) => {
      const selectionSnapshot = createHistorySelectionSnapshot(editor);

      logicalHistoryApplyingRef.current = true;

      try {
        const applied = editor.commands.setContent(value);
        if (!applied) {
          return false;
        }

        return restoreHistorySelection(editor, selectionSnapshot);
      } finally {
        logicalHistoryApplyingRef.current = false;
      }
    };
    const runLogicalUndo = () => {
      syncLogicalHistoryWithEditor();

      if (logicalHistoryDoneRef.current.length <= 1) {
        return false;
      }

      const currentValue = logicalHistoryDoneRef.current.pop();
      const previousValue =
        logicalHistoryDoneRef.current[logicalHistoryDoneRef.current.length - 1];
      if (!currentValue || !previousValue) {
        if (currentValue) {
          logicalHistoryDoneRef.current.push(currentValue);
        }

        return false;
      }

      logicalHistoryUndoneRef.current.push(currentValue);
      const applied = applyHistorySnapshot(previousValue);

      if (!applied) {
        logicalHistoryDoneRef.current.push(currentValue);
        logicalHistoryUndoneRef.current.pop();
        return false;
      }

      return true;
    };
    const runLogicalRedo = () => {
      const nextValue = logicalHistoryUndoneRef.current.pop();
      if (!nextValue) {
        return false;
      }

      const applied = applyHistorySnapshot(nextValue);
      if (!applied) {
        logicalHistoryUndoneRef.current.push(nextValue);
        return false;
      }

      logicalHistoryDoneRef.current.push(nextValue);
      return true;
    };
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.defaultPrevented) {
        return false;
      }

      const eventTarget = event.target;
      if (!(eventTarget instanceof Node) || !editor.view.dom.contains(eventTarget)) {
        return false;
      }

      if (!editor.isEditable) {
        return false;
      }

      const isModifierKey =
        event.key === "Shift" ||
        event.key === "Control" ||
        event.key === "Meta" ||
        event.key === "Alt" ||
        event.key === "CapsLock";
      const now = Date.now();

      const isSelectionNavigationKey =
        event.shiftKey &&
        (event.key.startsWith("Arrow") ||
          event.key === "Home" ||
          event.key === "End" ||
          event.key === "PageUp" ||
          event.key === "PageDown");

      if (!isModifierKey || isSelectionNavigationKey) {
        lastSelectionInteractionAtRef.current = now;
      }

      const selection = editor.state.selection;

      if (event.key === "Backspace" || event.key === "Delete") {
        const isImageSelection =
          selection instanceof NodeSelection &&
          selection.node.type.name === "image";

        if (isImageSelection) {
          event.preventDefault();
          event.stopPropagation();
          editor.chain().focus().deleteSelection().run();
          return true;
        }
      }

      if (!(event.ctrlKey || event.metaKey) || event.altKey) {
        return false;
      }

      const isUndo = !event.shiftKey && event.key.toLowerCase() === "z";
      const isRedo =
        (event.shiftKey && event.key.toLowerCase() === "z") ||
        event.key.toLowerCase() === "y";
      if (isUndo || isRedo) {
        event.preventDefault();
        event.stopPropagation();
        event.stopImmediatePropagation();
        if (isUndo) {
          historyAwareEditor.appEditorHistory?.undo?.();
        } else {
          historyAwareEditor.appEditorHistory?.redo?.();
        }
        return true;
      }

      return false;
    };

    historyAwareEditor.appEditorHistory = {
      undo: runLogicalUndo,
      redo: runLogicalRedo,
    };
    const editorDocument = editor.view.dom.ownerDocument;
    const previousHandleKeyDown = editor.view.props.handleKeyDown;
    editor.view.setProps({
      handleKeyDown: (view, event) => {
        if (handleKeyDown(event)) {
          return true;
        }

        return previousHandleKeyDown?.(view, event) ?? false;
      },
    });
    editor.on("transaction", handleTransaction);
    editorDocument.addEventListener("keydown", handleKeyDown, { capture: true });

    return () => {
      historyAwareEditor.appEditorHistory = previousAppHistory;
      editor.view.setProps({
        handleKeyDown: previousHandleKeyDown,
      });
      editorDocument.removeEventListener("keydown", handleKeyDown, { capture: true });
      editor.off("transaction", handleTransaction);
    };
  }, [editor, usePaginatedDocument]);

  const insertImageWithFallback = useCallback(
    (
      imageAttributes: Record<string, unknown>,
      insertionSelection?: {
        from: number;
        to: number;
      },
    ): ImageInsertionResult => {
      if (!editor) {
        return { inserted: false, position: null };
      }

      const imageNode = editor.state.schema.nodes.image;
      if (!imageNode) {
        return { inserted: false, position: null };
      }

      const safeMaxPosition = Math.max(
        editor.state.doc.childCount > 0 ? 1 : 0,
        editor.state.doc.content.size - 1,
      );
      const safeSelection = resolveSafeImageInsertionSelection(editor, insertionSelection);

      const fallbackSelection = safeSelection ?? {
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
        const insertedPosition = Math.min(
          Math.max(0, safeInsertionFrom),
          transaction.doc.content.size,
        );

        editor.view.dispatch(transaction);
        return {
          inserted: true,
          position: insertedPosition,
        };
      } catch {
        return { inserted: false, position: null };
      }
    },
    [editor],
  );

  const insertLocalImage = useCallback(
    async (
      file: File,
      width?: string,
      insertionSelection?: {
        from: number;
        to: number;
      },
    ) => {
      if (!editor) {
        return;
      }

      const localImageId = generateLocalImageId();
      const localImage: LocalImage = {
        id: localImageId,
        fileName: file.name,
        contentType: file.type || "application/octet-stream",
        size: file.size,
        blob: file,
        createdAt: Date.now(),
        sessionId: localImageScopeRef.current.sessionId,
      };

      await appEditorImageStore.saveImage(localImage);

      const objectUrl = URL.createObjectURL(file);
      localImageUrlsRef.current.set(localImageId, objectUrl);

      const safeSelection = resolveSafeImageInsertionSelection(
        editor,
        insertionSelection,
      );
      const imageWidthValue = typeof width === "string" ? normalizeImageWidth(width) : undefined;
      const imageId = generateEditorImageId();
      const imageIdentity = {
        imageId,
        localImageId,
        src: objectUrl,
      };
      const imageAttributes = {
        imageId,
        src: objectUrl,
        localImageId,
        source: "local",
        ...(imageWidthValue ? { width: imageWidthValue } : {}),
      };
      const directInsertedResult = insertImageWithFallback(imageAttributes, safeSelection);
      let inserted = directInsertedResult.inserted;

      if (!inserted) {
        inserted = insertImageWithFallback(imageAttributes, {
          from: Math.max(
            editor.state.doc.childCount > 0 ? 1 : 0,
            editor.state.doc.content.size - 1,
          ),
          to: Math.max(
            editor.state.doc.childCount > 0 ? 1 : 0,
            editor.state.doc.content.size - 1,
          ),
        }).inserted;
      }

      if (inserted && editor.state.selection instanceof NodeSelection && editor.state.selection.node.type.name === "image") {
        (editor as EditorWithImageSelectionState).__appEditorLastImagePos = editor.state.selection.from;
        (editor as EditorWithImageSelectionState).__appEditorLastImageIdentity = {
          imageId,
          localImageId,
          src: objectUrl,
        };
        return;
      }

      if (inserted) {
        const insertedPosition = resolveImageNodeAfterInsertionPosition(
          editor,
          imageIdentity,
          safeSelection?.to ?? safeSelection?.from ?? null,
        );
        (editor as EditorWithImageSelectionState).__appEditorLastImagePos = insertedPosition;
        (editor as EditorWithImageSelectionState).__appEditorLastImageIdentity = {
          imageId,
          localImageId,
          src: objectUrl,
        };
      }
    },
    [editor, insertImageWithFallback],
  );

  useEffect(() => {
    if (!editor) {
      return;
    }

    editor.setEditable(!(disabled || readOnly));
  }, [disabled, editor, readOnly]);

  useEffect(() => {
    if (!editor || !isControlled) {
      return;
    }

    const nextValue = normalizeEditorValue(value);
    if (nextValue === lastKnownValueRef.current) {
      return;
    }

    const currentEditorValue = normalizeEditorValue(
      normalizeEditorHtml(serializeVisualPageHtml(editor.getHTML())),
    );
    if (currentEditorValue === nextValue) {
      lastKnownValueRef.current = nextValue;
      return;
    }

    const now = Date.now();
    const isInActiveTypingWindow =
      now - lastTypingTransactionAtRef.current <= AUTO_PAGINATION_TYPING_DEBOUNCE_MS + 220;
    const isEditorFocused = (() => {
      const activeElement = document.activeElement;
      const proseMirrorElement = editor.view?.dom;

      return Boolean(
        (proseMirrorElement &&
          (proseMirrorElement === activeElement ||
            (activeElement !== null && proseMirrorElement.contains(activeElement)))),
      );
    })();
    const isLikelyInteracting =
      isEditorFocused && now - lastSelectionInteractionAtRef.current <= 900;

    if (isEditorFocused || isLikelyInteracting || isInActiveTypingWindow) {
      pendingControlledValueRef.current = nextValue;
      return;
    }

    if (pendingControlledValueRef.current === nextValue) {
      pendingControlledValueRef.current = null;
    }

    syncControlledValue(editor, nextValue);
    lastKnownValueRef.current = nextValue;
  }, [editor, isControlled, usePaginatedDocument, value]);

  useEffect(() => {
    if (!editor || !isControlled) {
      return;
    }

    const handleBlur = () => {
      const pendingValue = pendingControlledValueRef.current;
      if (pendingValue === null) {
        return;
      }

      const normalizedPendingValue = normalizeEditorValue(pendingValue);
      const currentEditorValue = normalizeEditorValue(
        normalizeEditorHtml(serializeVisualPageHtml(editor.getHTML())),
      );

      pendingControlledValueRef.current = null;

      if (normalizedPendingValue === currentEditorValue) {
        lastKnownValueRef.current = normalizedPendingValue;
        return;
      }

      syncControlledValue(editor, normalizedPendingValue);
      lastKnownValueRef.current = normalizedPendingValue;
    };

    const proseMirrorDom = editor.view.dom;
    proseMirrorDom.addEventListener("blur", handleBlur, { passive: true });

    return () => {
      proseMirrorDom.removeEventListener("blur", handleBlur);
    };
  }, [editor, isControlled]);

  useEffect(() => {
    if (!editor) {
      return undefined;
    }

    const syncImageSelectionState = () => {
      const selection = editor.state.selection;

      if (!(selection instanceof NodeSelection) || selection.node.type.name !== "image") {
        return;
      }

      lastImageInteractionAtRef.current = Date.now();
      (editor as EditorWithImageSelectionState).__appEditorLastImagePos = selection.from;
      (editor as EditorWithImageSelectionState).__appEditorLastImageIdentity =
        getImageIdentityFromAttrs(selection.node.attrs);
    };

    editor.on("selectionUpdate", syncImageSelectionState);
    editor.on("transaction", syncImageSelectionState);
    syncImageSelectionState();

    return () => {
      editor.off("selectionUpdate", syncImageSelectionState);
      editor.off("transaction", syncImageSelectionState);
    };
  }, [editor]);

  useEffect(() => {
    if (!editor) {
      return undefined;
    }

    const proseMirror = editor.view.dom;
    if (!(proseMirror instanceof HTMLElement)) {
      return undefined;
    }

    const handlePointerDown = (event: MouseEvent | TouchEvent) => {
      const target = event.target;
      if (!(target instanceof Element)) {
        return;
      }

      if (target.closest("[data-app-editor-image-node='true']")) {
        return;
      }

      lastSelectionInteractionAtRef.current = Date.now();
      isPointerSelectingRef.current = true;

      (editor as EditorWithImageSelectionState).__appEditorLastImagePos = null;
      (editor as EditorWithImageSelectionState).__appEditorLastImageIdentity = null;
      syncActiveImageIndicator(editor);
    };

    const handlePointerUp = () => {
      isPointerSelectingRef.current = false;
      lastSelectionInteractionAtRef.current = Date.now();
      syncSelectionRange(undefined, true);
      syncActiveImageIndicator(editor);
    };

    const syncIndicator = () => {
      if (isPointerSelectingRef.current) {
        return;
      }

      syncActiveImageIndicator(editor);
    };
    const syncSelectionRange = (
      eventOrForceSafe?: boolean | { [key: string]: unknown },
      forceSafe = false,
    ) => {
      const effectiveForceSafe = typeof eventOrForceSafe === "boolean"
        ? eventOrForceSafe
        : forceSafe;
      const selection = editor.state.selection;

      if (!effectiveForceSafe && isPointerSelectingRef.current) {
        previousSelectionRangeRef.current = {
          from: selection.from,
          to: selection.to,
        };
        return;
      }

      const safeSelection = resolveSafeSelectionRangeFromSelection(
        editor,
        selection.from,
        selection.to,
      );

      previousSelectionRangeRef.current = safeSelection;
    };

    editor.on("selectionUpdate", syncIndicator);
    editor.on("transaction", syncIndicator);
    editor.on("selectionUpdate", syncSelectionRange);
    editor.on("transaction", syncSelectionRange);
    proseMirror.addEventListener("mousedown", handlePointerDown, true);
    proseMirror.addEventListener("touchstart", handlePointerDown, true);
    window.addEventListener("mouseup", handlePointerUp, true);
    window.addEventListener("touchend", handlePointerUp, true);

    syncIndicator();
    syncSelectionRange();

    return () => {
      editor.off("selectionUpdate", syncIndicator);
      editor.off("transaction", syncIndicator);
      editor.off("selectionUpdate", syncSelectionRange);
      editor.off("transaction", syncSelectionRange);
      proseMirror.removeEventListener("mousedown", handlePointerDown, true);
      proseMirror.removeEventListener("touchstart", handlePointerDown, true);
      window.removeEventListener("mouseup", handlePointerUp, true);
      window.removeEventListener("touchend", handlePointerUp, true);
    };
  }, [editor]);

  useEffect(() => {
    void rehydrateLocalImages();
  }, [editor, rehydrateLocalImages, value]);

  useEffect(() => {
    return () => {
      localImageSyncTokenRef.current += 1;

      for (const url of localImageUrlsRef.current.values()) {
        URL.revokeObjectURL(url);
      }

      localImageUrlsRef.current.clear();
    };
  }, []);

  useEffect(() => {
    if (
      !editor ||
      usePaginatedDocument ||
      paginationMode !== "visual" ||
      pageHeight <= 0 ||
      !pageMargins
    ) {
      return undefined;
    }

    let frameId = 0;
    let pendingTimerId = 0;
    let typingAutoPaginationTimerId = 0;
    let imageResizeAutoPaginationTimerId = 0;
    let resizeObserver: ResizeObserver | null = null;
    let blockResizeObserver: ResizeObserver | null = null;
    let isRunning = false;
    let suppressScheduling = false;
    let skipObserverAutoPaginationPasses = 0;
    const pendingImageElements = new WeakSet<HTMLImageElement>();
    const pageContentHeight = Math.max(
      1,
      pageHeight - pageMargins.top - pageMargins.bottom,
    );
    const pageStride = pageHeight + pageGap;
    const autoPaginationDebounceMs = resolveAutoPaginationDebounceMs();
    const initialProseMirror = editor.view.dom;
    const scrollContainer =
      initialProseMirror instanceof HTMLElement
        ? findScrollableAncestor(initialProseMirror)
        : null;

    const hasPendingImageLoad = (root: HTMLElement) => {
      const images = Array.from(root.querySelectorAll("img")).filter(
        (image): image is HTMLImageElement => image instanceof HTMLImageElement,
      );

      return images.some((image) => !image.complete || image.naturalWidth === 0);
    };

    const subscribeToPendingImages = (root: HTMLElement) => {
      const images = Array.from(root.querySelectorAll("img")).filter(
        (image): image is HTMLImageElement => image instanceof HTMLImageElement,
      );

      images.forEach((image) => {
        if ((image.complete && image.naturalWidth > 0) || pendingImageElements.has(image)) {
          return;
        }

        const handleImageSettled = () => {
          pendingImageElements.delete(image);
          scheduleAutoPagination("immediate");
        };

        pendingImageElements.add(image);
        image.addEventListener("load", handleImageSettled, { once: true });
        image.addEventListener("error", handleImageSettled, { once: true });
      });
    };

    type PaginationScrollBehavior = "none" | "preserve-cursor";
    const performAutoPagination = (scrollBehavior: PaginationScrollBehavior = "none") => {
      void (scrollBehavior === "preserve-cursor");
      const proseMirror = editor.view.dom;

      if (!(proseMirror instanceof HTMLElement)) {
        return;
      }

      const hasActiveImageNodeSelection =
        editor.state.selection instanceof NodeSelection &&
        editor.state.selection.node.type.name === "image";
      const isWithinImageInteractionLock =
        Date.now() - lastImageInteractionAtRef.current < IMAGE_INTERACTION_LOCK_MS;

      if (hasActiveImageNodeSelection || isWithinImageInteractionLock) {
        scheduleAutoPagination("immediate");
        return;
      }

      if (hasPendingImageLoad(proseMirror)) {
        subscribeToPendingImages(proseMirror);
        return;
      }

      isRunning = true;
      suppressScheduling = true;

      try {
        const visualPagination = resolveAutoPageBreakActions({
          editor,
          proseMirror,
          pageContentHeight,
          pageHeight,
          pageStride,
          pageMargins,
          zoomLevel,
          startChildIndex: 0,
        });
        const visualPages = visualPagination.pages;

        observeProseMirrorBlocks(proseMirror, blockResizeObserver);
        proseMirror.dispatchEvent(
          new CustomEvent("app-editor-pagination-updated", {
            bubbles: true,
            detail: {
              pages: visualPages,
              pageContentHeight,
              pageStride,
              pageCount:
                visualPages.length > 0
                  ? Math.max(
                      1,
                      Math.max(
                        ...visualPages.map((page) => page.pageNumber),
                        visualPages.length,
                      ),
                    )
                  : 1,
            },
          }),
        );
      } finally {
        isRunning = false;
        suppressScheduling = false;
      }
    };

    const runAutoPagination = (
      priority: "immediate" | "deferred" = "deferred",
      scrollBehavior: PaginationScrollBehavior = "none",
    ) => {
      if (isRunning || suppressScheduling) {
        return;
      }

      if (priority === "immediate") {
        window.cancelAnimationFrame(frameId);
        frameId = window.requestAnimationFrame(() => {
          frameId = 0;
          performAutoPagination(scrollBehavior);
        });
        return;
      }

      frameId = window.requestAnimationFrame(() => {
        frameId = 0;
        performAutoPagination(scrollBehavior);
      });
    };

    const scheduleAutoPagination = (
      priority: "immediate" | "deferred" = "deferred",
      scrollBehavior: PaginationScrollBehavior = "none",
    ) => {
      if (suppressScheduling) {
        return;
      }

      const resolvedDebounceMs = autoPaginationDebounceMs;

      window.clearTimeout(pendingTimerId);
      if (priority === "immediate") {
        runAutoPagination(
          "immediate",
          scrollBehavior,
        );
        return;
      }

      pendingTimerId = window.setTimeout(() => {
        runAutoPagination(
          "deferred",
          scrollBehavior,
        );
      }, resolvedDebounceMs);
    };

    const handleWindowResize = () => {
      scheduleAutoPagination();
    };

    const handleScrollActivity = () => {
      if (manualScrollActivityTimerRef.current !== null) {
        window.clearTimeout(manualScrollActivityTimerRef.current);
      }

      isUserScrollingRef.current = true;

      manualScrollActivityTimerRef.current = window.setTimeout(() => {
        isUserScrollingRef.current = false;
      }, 160);
    };

    if (typeof ResizeObserver !== "undefined") {
      resizeObserver = new ResizeObserver(() => {
        if (skipObserverAutoPaginationPasses > 0) {
          skipObserverAutoPaginationPasses -= 1;
          return;
      }
      scheduleAutoPagination(
        "deferred",
        "none",
      );
    });
      resizeObserver.observe(editor.view.dom);
    }

    if (typeof ResizeObserver !== "undefined") {
      blockResizeObserver = new ResizeObserver(() => {
        if (skipObserverAutoPaginationPasses > 0) {
          skipObserverAutoPaginationPasses -= 1;
          return;
        }
        if (suppressScheduling || isRunning) {
          return;
        }
        const root = editor.view.dom;
        if (!(root instanceof HTMLElement)) {
          return;
        }
        scheduleAutoPagination(
          "deferred",
          "none",
        );
      });

      const root = editor.view.dom;
      if (root instanceof HTMLElement) {
        observeProseMirrorBlocks(root, blockResizeObserver);
      }
    }

    scrollContainer?.addEventListener("scroll", handleScrollActivity, { passive: true });

    const handleEditorTransaction = (event: { transaction?: Transaction } = {}) => {
      const transaction = event.transaction;

      if (!transaction || !transaction.docChanged) {
        return;
      }
      if (transaction.getMeta?.("appEditorPagination") === true) {
        return;
      }

      const previousDocForReplacement = transaction.before;
      const currentDocSize = editor.state.doc.content.size;
      const currentDocTextLength = resolveDocTextLength(editor.state.doc);
      const uiEvent = transaction.getMeta?.("uiEvent");
      const isLikelyTypingUiEvent = uiEvent === "input" || uiEvent === "keyboard";
      const isPasteLikeEditorTransaction =
        uiEvent === "paste" || transaction.getMeta?.("paste") === true;
      const hasTinyInsertStep = transaction.steps.some((step) => {
        const insertedSize = getStepInsertedSize(step);
        return insertedSize !== null && insertedSize > 0 && insertedSize <= 2;
      });
      const hasAnyInsertStep = transaction.steps.some((step) => {
        const insertedSize = getStepInsertedSize(step);

        return insertedSize !== null && insertedSize > 0;
      });
      const selectionBeforeWasCollapsed = Boolean(
        transaction.selection && transaction.selection.from === transaction.selection.to,
      );
      const docSizeDelta = currentDocSize - (previousDocForReplacement?.content.size ?? currentDocSize);
      const isSmallInsertTypingCandidate =
        docSizeDelta > 0 &&
        docSizeDelta <= 24 &&
        hasAnyInsertStep &&
        selectionBeforeWasCollapsed &&
        !isPasteLikeEditorTransaction &&
        transaction.steps.length <= 8;
      const isLikelyTypingReplacement =
        hasAnyInsertStep &&
        docSizeDelta > -4 &&
        docSizeDelta <= 24 &&
        transaction.steps.length <= 10 &&
        selectionBeforeWasCollapsed &&
        !isPasteLikeEditorTransaction;
      const isLikelyTypingTransaction =
        isLikelyTypingUiEvent ||
        isSmallInsertTypingCandidate ||
        isLikelyTypingReplacement ||
        (hasTinyInsertStep && transaction.steps.length <= 3 && !isPasteLikeEditorTransaction);
      const replacementDocForSelection = previousDocForReplacement ?? editor.state.doc;
      const selectionBeforeTransaction = transaction.selection;
      const selectionBeforeRangeFromMapping = (() => {
        if (!selectionBeforeTransaction) {
          return null;
        }

        try {
          const inverseMapping = transaction.mapping.invert();
          const maxPosition = replacementDocForSelection.content.size;

          return {
            from: clampSelection(
              inverseMapping.map(selectionBeforeTransaction.from, -1),
              maxPosition,
            ),
            to: clampSelection(
              inverseMapping.map(selectionBeforeTransaction.to, -1),
              maxPosition,
            ),
          };
        } catch {
          return null;
        }
      })();
      const selectionBeforeRangeFallback = selectionBeforeTransaction
        ? {
            from: clampSelection(selectionBeforeTransaction.from, replacementDocForSelection.content.size),
            to: clampSelection(selectionBeforeTransaction.to, replacementDocForSelection.content.size),
          }
        : null;
      const selectionBeforeRange =
        selectionBeforeRangeFromMapping ?? selectionBeforeRangeFallback;
      const previousSelectionRange = selectionBeforeRange ?? previousSelectionRangeRef.current;
      const previousSelectionWasFullDocument = Boolean(
        previousSelectionRange &&
        previousSelectionRange.from <= 1 &&
        previousSelectionRange.to >= Math.max(
          0,
          previousDocForReplacement.content.size - 1,
        ) &&
        previousSelectionRange.to - previousSelectionRange.from >=
          Math.max(1, previousDocForReplacement.content.size - 2),
      );
      const fullReplaceDetection = (() => {
        if (!previousDocForReplacement || previousDocForReplacement.content.size <= 0) {
          return {
            wasWholeDocumentReplaced: false,
            isTinyFullSelectionTypingReplacement: false,
          };
        }

        const isFullReplaceStep = (step: unknown) => {
          const resolveBoundary = (key: "from" | "to") => {
            const direct = step as { [key: string]: number | undefined };
            const directValue = direct[key];
            if (typeof directValue === "number") {
              return directValue;
            }

            const stepAsJson = typeof (step as { toJSON?: () => unknown })?.toJSON === "function"
              ? (step as { toJSON: () => unknown }).toJSON()
              : null;
            if (stepAsJson && typeof stepAsJson === "object") {
              const jsonValue = (stepAsJson as { [key: string]: unknown })[key];
              if (typeof jsonValue === "number") {
                return jsonValue;
              }
            }

            return undefined;
          };

          const candidate = {
            from: resolveBoundary("from"),
            to: resolveBoundary("to"),
          };

          return (
            typeof candidate.from === "number" &&
            typeof candidate.to === "number" &&
            candidate.from <= 1 &&
            candidate.to >= previousDocForReplacement.content.size - 1 &&
            candidate.to <= previousDocForReplacement.content.size + 2
          );
        };

        const isSmallInputFullReplaceStep = transaction.steps.some((step) => {
          return isFullReplaceStep(step) && (getStepInsertedSize(step) ?? 2) <= 2;
        });
        const hasFullReplaceStep = transaction.steps.some(isFullReplaceStep);
        const typedSelectionWasFullDocument = Boolean(
          hasFullReplaceStep ||
            previousSelectionWasFullDocument ||
            (previousSelectionRange &&
              previousSelectionRange.from <= 1 &&
              previousSelectionRange.to >= Math.max(
                0,
                previousDocForReplacement.content.size - 1,
              ) &&
              previousSelectionRange.to - previousSelectionRange.from >=
                Math.max(1, previousDocForReplacement.content.size - 2)),
        );

        return {
          wasWholeDocumentReplaced:
            hasFullReplaceStep &&
            !isLikelyTypingUiEvent &&
            !isSmallInputFullReplaceStep &&
            (isPasteLikeEditorTransaction || previousSelectionRange !== null) &&
            typedSelectionWasFullDocument,
          isTinyFullSelectionTypingReplacement:
            hasFullReplaceStep &&
            isSmallInputFullReplaceStep &&
            (isLikelyTypingUiEvent || previousSelectionRange !== null) &&
            typedSelectionWasFullDocument,
        };
      })();
      const previousDocTextLength = previousDocForReplacement
        ? resolveDocTextLength(previousDocForReplacement)
        : 0;
      const previousDocSize = previousDocForReplacement?.content.size ?? 0;
      const previousDocTextLengthForSelectionCoverage =
        previousDocTextLength > 0 ? previousDocTextLength : currentDocTextLength;
      const isLikelyFullDocumentTypingReplacement =
        isLikelyTypingUiEvent &&
        (previousSelectionWasFullDocument ||
          (previousSelectionRange !== null &&
            previousSelectionRange.from <= 1 &&
            previousSelectionRange.to - previousSelectionRange.from >=
              Math.max(
                6,
                previousDocTextLengthForSelectionCoverage * 0.8,
              )));
      const isExtremeSizeDropFromTyping = previousDocSize >= 24
        && currentDocSize <= Math.max(6, Math.floor(previousDocSize * 0.1))
        && currentDocSize < previousDocSize;
      const isTextSizeDropFromTyping = previousDocTextLength >= 24
        && isLikelyTypingUiEvent
        && currentDocTextLength <= Math.max(2, Math.floor(previousDocTextLength * 0.1))
        && currentDocTextLength < previousDocTextLength;
      const wasWholeDocumentReplaced = fullReplaceDetection.wasWholeDocumentReplaced;
      const isTinyFullSelectionTypingReplacement =
        fullReplaceDetection.isTinyFullSelectionTypingReplacement
        || isExtremeSizeDropFromTyping
        || isTextSizeDropFromTyping
        || isLikelyFullDocumentTypingReplacement;
      const wasFullDocumentSelectionByReplace =
        wasWholeDocumentReplaced || fullReplaceDetection.isTinyFullSelectionTypingReplacement;
      const isFullDocumentSelectionBefore =
        isLikelyTypingUiEvent &&
        (previousSelectionWasFullDocument || wasFullDocumentSelectionByReplace);
      const isTypingTransaction = isLikelyTypingTransaction;
      if (isTypingTransaction) {
        lastTypingTransactionAtRef.current = Date.now();
      }
      const isImageResizeTransaction = transaction.getMeta?.("appEditorImageResize") === true;
      const skipFollowUpPaginationRun =
        isTinyFullSelectionTypingReplacement ||
        wasFullDocumentSelectionByReplace ||
        isFullDocumentSelectionBefore;

      const scrollBehavior: PaginationScrollBehavior = isTypingTransaction || isImageResizeTransaction
        ? "none"
        : "preserve-cursor";
      const root = editor.view.dom;
      if (skipFollowUpPaginationRun) {
        skipObserverAutoPaginationPasses = 12;
      }

      if (isImageResizeTransaction) {
        window.clearTimeout(typingAutoPaginationTimerId);
        window.clearTimeout(imageResizeAutoPaginationTimerId);
        if (AUTO_PAGINATION_IMAGE_RESIZE_DEBOUNCE_MS === 0) {
          if (root instanceof HTMLElement) {
            observeProseMirrorBlocks(root, blockResizeObserver);
          }

          runAutoPagination("immediate", "none");
          return;
        }

        imageResizeAutoPaginationTimerId = window.setTimeout(() => {
          if (root instanceof HTMLElement) {
            observeProseMirrorBlocks(root, blockResizeObserver);
          }

          runAutoPagination("immediate", "none");
        }, AUTO_PAGINATION_IMAGE_RESIZE_DEBOUNCE_MS);

        return;
      }

      if (isTypingTransaction) {
        window.clearTimeout(imageResizeAutoPaginationTimerId);
        window.clearTimeout(typingAutoPaginationTimerId);
        window.clearTimeout(pendingTimerId);
        if (frameId) {
          window.cancelAnimationFrame(frameId);
          frameId = 0;
        }
        typingAutoPaginationTimerId = window.setTimeout(() => {
          if (root instanceof HTMLElement) {
            observeProseMirrorBlocks(root, blockResizeObserver);
          }

          runAutoPagination(
            "deferred",
            "none",
          );
        }, AUTO_PAGINATION_TYPING_DEBOUNCE_MS);

        return;
      }

      window.clearTimeout(imageResizeAutoPaginationTimerId);
      window.clearTimeout(typingAutoPaginationTimerId);

      const schedulePriority: "immediate" | "deferred" = isPasteLikeEditorTransaction
        ? "deferred"
        : "immediate";

      if (root instanceof HTMLElement) {
        observeProseMirrorBlocks(root, blockResizeObserver);
      }

      scheduleAutoPagination(
        schedulePriority,
        scrollBehavior,
      );
    };

    editor.on("transaction", handleEditorTransaction);
    window.addEventListener("resize", handleWindowResize);
    scheduleAutoPagination("immediate");

    return () => {
      window.clearTimeout(pendingTimerId);
      window.clearTimeout(typingAutoPaginationTimerId);
      window.clearTimeout(imageResizeAutoPaginationTimerId);
      window.cancelAnimationFrame(frameId);
      resizeObserver?.disconnect();
      blockResizeObserver?.disconnect();
      window.removeEventListener("resize", handleWindowResize);
      if (manualScrollActivityTimerRef.current !== null) {
        window.clearTimeout(manualScrollActivityTimerRef.current);
        manualScrollActivityTimerRef.current = null;
      }
      scrollContainer?.removeEventListener("scroll", handleScrollActivity);
      editor.off("transaction", handleEditorTransaction);

    };
  }, [editor, pageGap, pageHeight, pageMargins, paginationMode, usePaginatedDocument, zoomLevel]);

  useEffect(() => {
    return () => {
      if (pendingOnChangeFrameRef.current !== null) {
        window.cancelAnimationFrame(pendingOnChangeFrameRef.current);
        pendingOnChangeFrameRef.current = null;
      }
    };
  }, [editor]);

  return {
    editor,
    isEditable: !(disabled || readOnly),
    insertLocalImage,
  };
}

