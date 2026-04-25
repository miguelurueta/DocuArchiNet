import { useCallback, useEffect, useRef } from "react";
import { useEditor } from "@tiptap/react";
import type { Editor } from "@tiptap/react";
import { NodeSelection, TextSelection } from "@tiptap/pm/state";
import type { UseAppEditorOptions, UseAppEditorResult } from "../domain/editor.types";
import { clampSelection, normalizeEditorValue } from "../domain/editor.model";
import { createAppEditorConfig } from "../infrastructure/tiptap.config";
import { generateEditorImageId, generateLocalImageId } from "./localImageIds";
import { appEditorImageStore } from "../infrastructure/indexeddb/appEditorImageStore";
import type { LocalImage } from "../infrastructure/indexeddb/localImage.types";
import { normalizeEditorHtml } from "./normalizeEditorHtml";
import {
  hasManualPageBreaks,
  hasVisualPageWrappers,
  serializeVisualPageHtml,
  wrapHtmlInVisualPages,
} from "./pageDocument";
import {
  insertPageBreakBeforeBlock,
  splitListBlockBeforeItemAndInsertPageBreak,
  splitTextBlockAtPositionAndInsertPageBreak,
} from "./autoPageBreak";
import {
  removeAutoPageBreaks,
  resolveAutoPageBreakCleanupStartPosition,
  resolveTopLevelChildIndexFromPosition,
  resolveAutoPageBreakActions,
  syncAutoPageBreakSpacerHeights,
} from "./autoPagination";

const AUTO_PAGINATION_DEBOUNCE_MS = 180;
const IMAGE_INTERACTION_LOCK_MS = 600;

type PaginationScrollAnchor = {
  viewportOffset: number;
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
  let current: HTMLElement | null = element?.parentElement ?? null;

  while (current) {
    const styles = window.getComputedStyle(current);
    const overflowY = styles.overflowY;
    const canScroll =
      (overflowY === "auto" || overflowY === "scroll") &&
      current.scrollHeight > current.clientHeight;

    if (canScroll) {
      return current;
    }

    current = current.parentElement;
  }

  return null;
}

export function capturePaginationScrollAnchor(
  editor: Editor,
  scrollContainer: HTMLElement | null,
) {
  const selection = editor.state.selection;

  if (!scrollContainer || selection instanceof NodeSelection) {
    return null;
  }

  const containerRect = scrollContainer.getBoundingClientRect();
  let caretRect: ReturnType<typeof editor.view.coordsAtPos>;

  try {
    caretRect = editor.view.coordsAtPos(selection.to);
  } catch {
    return null;
  }

  return {
    viewportOffset: caretRect.top - containerRect.top,
  } satisfies PaginationScrollAnchor;
}

export function restorePaginationScrollAnchor(
  editor: Editor,
  scrollContainer: HTMLElement | null,
  anchor: PaginationScrollAnchor | null,
) {
  const selection = editor.state.selection;

  if (!scrollContainer || !anchor || selection instanceof NodeSelection) {
    return;
  }

  const containerRect = scrollContainer.getBoundingClientRect();
  let caretRect: ReturnType<typeof editor.view.coordsAtPos>;

  try {
    caretRect = editor.view.coordsAtPos(selection.to);
  } catch {
    return;
  }

  const currentViewportOffset = caretRect.top - containerRect.top;
  const nextScrollTop = Math.max(
    0,
    scrollContainer.scrollTop + (currentViewportOffset - anchor.viewportOffset),
  );

  if (Math.abs(nextScrollTop - scrollContainer.scrollTop) > 1) {
    scrollContainer.scrollTop = nextScrollTop;
  }
}

function resolveSelectionPageIndex(
  editor: Editor,
  proseMirror: HTMLElement,
  pageStride: number,
  zoomLevel: number,
) {
  const selection = editor.state.selection;

  if (selection instanceof NodeSelection) {
    return null;
  }

  const proseMirrorRect = proseMirror.getBoundingClientRect();
  let caretRect: ReturnType<typeof editor.view.coordsAtPos>;

  try {
    caretRect = editor.view.coordsAtPos(selection.to);
  } catch {
    return null;
  }

  const safeZoomLevel = Math.max(0.1, zoomLevel);
  const safePageStride = Math.max(1, pageStride * safeZoomLevel);
  const caretTop = Math.max(0, caretRect.top - proseMirrorRect.top);

  return Math.max(0, Math.floor(caretTop / safePageStride));
}

function scrollSelectionIntoViewWithinContainer(
  editor: Editor,
  scrollContainer: HTMLElement | null,
  targetViewportOffset: number,
) {
  const selection = editor.state.selection;

  if (!scrollContainer || selection instanceof NodeSelection) {
    return;
  }

  const containerRect = scrollContainer.getBoundingClientRect();
  let caretRect: ReturnType<typeof editor.view.coordsAtPos>;

  try {
    caretRect = editor.view.coordsAtPos(selection.to);
  } catch {
    return;
  }

  const currentViewportOffset = caretRect.top - containerRect.top;
  const nextScrollTop = Math.max(
    0,
    scrollContainer.scrollTop + (currentViewportOffset - targetViewportOffset),
  );

  if (Math.abs(nextScrollTop - scrollContainer.scrollTop) > 1) {
    scrollContainer.scrollTop = nextScrollTop;
  }
}

function hasTrailingAutoPageBreakAtSelection(editor: Editor) {
  const selection = editor.state.selection;

  if (selection instanceof NodeSelection || selection.from !== selection.to || !selection.$from.parent.isTextblock) {
    return false;
  }

  const topLevelIndex = selection.$from.index(0);
  const nextTopLevelNode =
    topLevelIndex < editor.state.doc.childCount - 1 ? editor.state.doc.child(topLevelIndex + 1) : null;

  return nextTopLevelNode?.type.name === "pageBreak" && nextTopLevelNode.attrs.auto === true;
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
  const shouldUsePaginatedDocument =
    hasVisualPageWrappers(editorHtml) ||
    hasManualPageBreaks(editorHtml) ||
    hasVisualPageWrappers(nextValue) ||
    hasManualPageBreaks(nextValue);
  const editorContent = shouldUsePaginatedDocument
    ? wrapHtmlInVisualPages(normalizedNextValue)
    : normalizedNextValue;

  editor.commands.setContent(editorContent, { emitUpdate: false });

  const maxPosition = editor.state.doc.content.size;
  editor.commands.setTextSelection({
    from: clampSelection(from, maxPosition),
    to: clampSelection(to, maxPosition),
  });
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

function getLastImagePosition(editor: Editor) {
  let lastImagePosition: number | null = null;

  editor.state.doc.descendants((node, pos) => {
    if (node.type.name === "image") {
      lastImagePosition = pos;
    }
  });

  return lastImagePosition;
}

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
        node.setAttribute("data-app-editor-image-active", "true");
        node.setAttribute("data-app-editor-image-persistent", "true");
        const image = node.querySelector("img");
        if (image instanceof HTMLImageElement) {
          image.setAttribute("data-app-editor-image-active", "true");
          image.setAttribute("data-app-editor-image-persistent", "true");
        }
      } else {
        node.removeAttribute("data-app-editor-image-active");
        node.removeAttribute("data-app-editor-image-persistent");
        const image = node.querySelector("img");
        if (image instanceof HTMLImageElement) {
          image.removeAttribute("data-app-editor-image-active");
          image.removeAttribute("data-app-editor-image-persistent");
        }
      }
    },
  );
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
  const usePaginatedDocument =
    paginationMode === "visual" &&
    (hasVisualPageWrappers(externalSourceContent) || hasManualPageBreaks(externalSourceContent));
  const initialContentRef = useRef(
    usePaginatedDocument
      ? wrapHtmlInVisualPages(externalInitialContent)
      : externalInitialContent,
  );
  const lastKnownValueRef = useRef(initialContentRef.current);
  const localImageUrlsRef = useRef(new Map<string, string>());
  const localImageScopeRef = useRef(buildLocalImageScope());
  const localImageSyncTokenRef = useRef(0);
  const lastImageInteractionAtRef = useRef(0);
  const dirtyStartChildIndexRef = useRef<number | null>(null);
  const dirtyNeedsPreviousBreakCleanupRef = useRef(false);

  const editor = useEditor(
    {
      ...createAppEditorConfig({
        content: initialContentRef.current,
        placeholder,
        editable: !(disabled || readOnly),
        paginatedDocument: usePaginatedDocument,
        onUpdate: ({ editor: currentEditor }) => {
          const currentLocalImageIds = collectLocalImageIds(currentEditor);

          for (const [localImageId, url] of localImageUrlsRef.current.entries()) {
            if (!currentLocalImageIds.has(localImageId)) {
              URL.revokeObjectURL(url);
              localImageUrlsRef.current.delete(localImageId);
            }
          }

          const nextValue = normalizeEditorValue(
            normalizeEditorHtml(serializeVisualPageHtml(currentEditor.getHTML())),
          );
          lastKnownValueRef.current = nextValue;
          onChange?.(nextValue);
        },
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

  const insertLocalImage = useCallback(
    async (file: File, width?: string) => {
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

      const chain = editor.chain().focus() as unknown as {
        setImage: (attributes: Record<string, unknown>) => { run: () => boolean };
      };
      const imageId = generateEditorImageId();

      chain
        .setImage({
          imageId,
          src: objectUrl,
          localImageId,
          source: "local",
        })
        .run();

      const insertedImagePosition = getLastImagePosition(editor);
      if (insertedImagePosition !== null) {
        (editor as EditorWithImageSelectionState).__appEditorLastImagePos = insertedImagePosition;
        (editor as EditorWithImageSelectionState).__appEditorLastImageIdentity = {
          imageId,
          localImageId,
          src: objectUrl,
        };
      }

      if (width) {
        editor
          .chain()
          .focus()
          .updateAttributes("image", {
            width,
          })
          .run();
      }
    },
    [editor],
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

    syncControlledValue(editor, nextValue);
    lastKnownValueRef.current = nextValue;
  }, [editor, isControlled, value]);

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

    const handlePointerDown = (event: MouseEvent) => {
      const target = event.target;
      if (!(target instanceof Element)) {
        return;
      }

      if (target.closest("[data-app-editor-image-node='true']")) {
        return;
      }

      (editor as EditorWithImageSelectionState).__appEditorLastImagePos = null;
      (editor as EditorWithImageSelectionState).__appEditorLastImageIdentity = null;
      syncActiveImageIndicator(editor);
    };

    proseMirror.addEventListener("mousedown", handlePointerDown, true);

    return () => {
      proseMirror.removeEventListener("mousedown", handlePointerDown, true);
    };
  }, [editor]);

  useEffect(() => {
    if (!editor) {
      return undefined;
    }

    const syncIndicator = () => {
      syncActiveImageIndicator(editor);
    };

    editor.on("selectionUpdate", syncIndicator);
    editor.on("transaction", syncIndicator);
    syncIndicator();

    return () => {
      editor.off("selectionUpdate", syncIndicator);
      editor.off("transaction", syncIndicator);
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
    let userScrollIdleTimerId = 0;
    let resizeObserver: ResizeObserver | null = null;
    let isRunning = false;
    let suppressScheduling = false;
    let isUserScrolling = false;
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

    const performAutoPagination = () => {
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
      let needsFollowUpRun = false;

      try {
        const scrollAnchor = capturePaginationScrollAnchor(editor, scrollContainer);
        const selectionPageBefore = resolveSelectionPageIndex(
          editor,
          proseMirror,
          pageStride,
          zoomLevel,
        );
        const originalSelectionState = editor.state.selection;
        const originalSelectionRange = {
          from: originalSelectionState.from,
          to: originalSelectionState.to,
        };
        const shouldPreserveAbsoluteSelectionThroughCleanup =
          hasTrailingAutoPageBreakAtSelection(editor);
        const originalSelectedImage =
          originalSelectionState instanceof NodeSelection &&
          originalSelectionState.node.type.name === "image"
            ? {
                imageId:
                  typeof originalSelectionState.node.attrs.imageId === "string"
                    ? originalSelectionState.node.attrs.imageId
                    : null,
                localImageId:
                  typeof originalSelectionState.node.attrs.localImageId === "string"
                    ? originalSelectionState.node.attrs.localImageId
                    : null,
                src:
                  typeof originalSelectionState.node.attrs.src === "string"
                    ? originalSelectionState.node.attrs.src
                    : null,
              }
            : null;
        const dirtyStartChildIndex = dirtyStartChildIndexRef.current ?? 0;
        const dirtyStartPosition = resolveAutoPageBreakCleanupStartPosition(
          editor,
          dirtyStartChildIndex,
          {
            includePreviousAutoBreak: dirtyNeedsPreviousBreakCleanupRef.current,
          },
        );
        const maxIterations = Math.min(
          200,
          Math.max(24, Math.ceil(editor.state.doc.childCount * 2)),
        );
        const removedAutoBreaks = removeAutoPageBreaks(editor, dirtyStartPosition);
        const actionStartChildIndex =
          editor.state.doc.childCount > 0
            ? resolveTopLevelChildIndexFromPosition(
                editor,
                Math.min(dirtyStartPosition, editor.state.doc.content.size),
              )
            : 0;

        if (
          removedAutoBreaks &&
          !originalSelectedImage &&
          !(originalSelectionState instanceof NodeSelection)
        ) {
          const maxPositionAfterCleanup = editor.state.doc.content.size;
          const currentSelectionAfterCleanup = editor.state.selection;

          if (
            shouldPreserveAbsoluteSelectionThroughCleanup ||
            currentSelectionAfterCleanup.from < 0 ||
            currentSelectionAfterCleanup.to > maxPositionAfterCleanup
          ) {
            const restoreSelectionTransaction = editor.state.tr.setSelection(
              TextSelection.between(
                editor.state.doc.resolve(
                  clampSelection(
                    shouldPreserveAbsoluteSelectionThroughCleanup
                      ? originalSelectionRange.from
                      : currentSelectionAfterCleanup.from,
                    maxPositionAfterCleanup,
                  ),
                ),
                editor.state.doc.resolve(
                  clampSelection(
                    shouldPreserveAbsoluteSelectionThroughCleanup
                      ? originalSelectionRange.to
                      : currentSelectionAfterCleanup.to,
                    maxPositionAfterCleanup,
                  ),
                ),
              ),
            );
            editor.view.dispatch(restoreSelectionTransaction);
          }
        }

        let iterations = 0;

        while (iterations < maxIterations) {
          const currentProseMirror = editor.view.dom;
          if (!(currentProseMirror instanceof HTMLElement)) {
            return;
          }

          const pageBreakActions = resolveAutoPageBreakActions({
            editor,
            proseMirror: currentProseMirror,
            pageContentHeight,
            pageStride,
            zoomLevel,
            startChildIndex: actionStartChildIndex,
          });

          const nextAction = pageBreakActions[0];
          if (!nextAction) {
            break;
          }

          if (nextAction.type === "before") {
            const inserted = insertPageBreakBeforeBlock(editor, nextAction.position, {
              auto: true,
            }, {
              preserveSelection: true,
            });

            if (!inserted) {
              break;
            }
          } else if (nextAction.type === "list-item") {
            const inserted = splitListBlockBeforeItemAndInsertPageBreak(
              editor,
              nextAction.listPosition,
              nextAction.itemPosition,
              {
                auto: true,
                mergeOnRemove: true,
              },
              {
                preserveSelection: true,
              },
            );

            if (!inserted) {
              break;
            }
          } else {
            const inserted = splitTextBlockAtPositionAndInsertPageBreak(
              editor,
              nextAction.position,
              {
                auto: true,
                mergeOnRemove: true,
              },
              {
                preserveSelection: true,
              },
            );

            if (!inserted) {
              break;
            }
          }

          const repaginatedProseMirror = editor.view.dom;
          if (repaginatedProseMirror instanceof HTMLElement) {
            syncAutoPageBreakSpacerHeights(
              editor,
              repaginatedProseMirror,
              pageStride,
              pageContentHeight,
            );
          }

          iterations += 1;
        }

        const finalProseMirror = editor.view.dom;
        if (finalProseMirror instanceof HTMLElement) {
          syncAutoPageBreakSpacerHeights(
            editor,
            finalProseMirror,
            pageStride,
            pageContentHeight,
          );
          const maxPosition = editor.state.doc.content.size;
          const nextFrom = clampSelection(originalSelectionRange.from, maxPosition);
          const nextTo = clampSelection(originalSelectionRange.to, maxPosition);
          const currentSelection = editor.state.selection;

          if (originalSelectedImage || originalSelectionState instanceof NodeSelection) {
            const resolvedImagePosition = findImagePositionByIdentity(
              editor,
              originalSelectedImage,
            );
            const resolvedSelectionPosition =
              typeof resolvedImagePosition === "number" &&
              editor.state.doc.nodeAt(resolvedImagePosition)?.type.name === "image"
                ? resolvedImagePosition
                : nextFrom;

            if (
              currentSelection.from !== resolvedSelectionPosition ||
              currentSelection.to !== resolvedSelectionPosition
            ) {
              const restoredSelection = NodeSelection.create(
                editor.state.doc,
                resolvedSelectionPosition,
              );
              const selectionTransaction = editor.state.tr.setSelection(restoredSelection);
              editor.view.dispatch(selectionTransaction);
            }
          } else if (
            currentSelection.from < 0 ||
            currentSelection.to > maxPosition
          ) {
            const selectionTransaction = editor.state.tr.setSelection(
              TextSelection.create(editor.state.doc, nextFrom, nextTo),
            );
            editor.view.dispatch(selectionTransaction);
          }

          finalProseMirror.dispatchEvent(
            new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
          );
          const selectionPageAfter = resolveSelectionPageIndex(
            editor,
            finalProseMirror,
            pageStride,
            zoomLevel,
          );
          const movedSelectionForwardAcrossPages =
            selectionPageBefore !== null &&
            selectionPageAfter !== null &&
            selectionPageAfter > selectionPageBefore;

          if (movedSelectionForwardAcrossPages) {
            scrollSelectionIntoViewWithinContainer(
              editor,
              scrollContainer,
              Math.max(24, pageMargins.top * Math.max(0.1, zoomLevel)),
            );
          } else {
            restorePaginationScrollAnchor(editor, scrollContainer, scrollAnchor);
          }

          needsFollowUpRun =
            resolveAutoPageBreakActions({
              editor,
              proseMirror: finalProseMirror,
              pageContentHeight,
              pageStride,
              zoomLevel,
              startChildIndex: actionStartChildIndex,
            }).length > 0;
          dirtyStartChildIndexRef.current = null;
          dirtyNeedsPreviousBreakCleanupRef.current = false;
        }
      } finally {
        isRunning = false;
        suppressScheduling = false;
        if (needsFollowUpRun) {
          scheduleAutoPagination(isUserScrolling ? "deferred" : "immediate");
        }
      }
    };

    const runAutoPagination = (priority: "immediate" | "deferred" = "deferred") => {
      if (isRunning || suppressScheduling) {
        return;
      }

      if (priority === "immediate") {
        window.cancelAnimationFrame(frameId);
        frameId = window.requestAnimationFrame(() => {
          frameId = 0;
          performAutoPagination();
        });
        return;
      }

      frameId = window.requestAnimationFrame(() => {
        frameId = 0;
        performAutoPagination();
      });
    };

    const scheduleAutoPagination = (priority: "immediate" | "deferred" = "deferred") => {
      if (suppressScheduling) {
        return;
      }

      window.clearTimeout(pendingTimerId);
      if (priority === "immediate") {
        runAutoPagination("immediate");
        return;
      }

      pendingTimerId = window.setTimeout(() => {
        runAutoPagination("deferred");
      }, autoPaginationDebounceMs);
    };

    const handleWindowResize = () => {
      dirtyStartChildIndexRef.current = 0;
      scheduleAutoPagination();
    };

    const handleScrollActivity = () => {
      isUserScrolling = true;
      window.clearTimeout(userScrollIdleTimerId);
      userScrollIdleTimerId = window.setTimeout(() => {
        isUserScrolling = false;
      }, 140);
    };

    if (typeof ResizeObserver !== "undefined") {
      resizeObserver = new ResizeObserver(() => {
        scheduleAutoPagination();
      });
      resizeObserver.observe(editor.view.dom);
    }

    scrollContainer?.addEventListener("scroll", handleScrollActivity, { passive: true });

    const handleEditorTransaction = ({
      transaction,
    }: {
      transaction: {
        docChanged?: boolean;
        selectionSet?: boolean;
        getMeta?: (key: string) => unknown;
        mapping?: {
          invert: () => {
            map: (position: number, assoc?: number) => number;
          };
        };
        selection?: { from?: number };
        before?: {
          childCount: number;
          child: (index: number) => { type: { name: string } } | null;
          content: { size: number };
          resolve: (position: number) => { index: (depth: number) => number };
        };
      };
    }) => {
      if (!transaction.docChanged) {
        return;
      }

      const affectedPosition =
        typeof transaction.selection?.from === "number"
          ? transaction.selection.from
          : editor.state.selection.from;
      const nextDirtyStartIndex = resolveTopLevelChildIndexFromPosition(editor, affectedPosition);
      const previousDoc = transaction.before;
      const previousDirtyIndex =
        previousDoc && previousDoc.childCount > 0
          ? Math.max(
              0,
              Math.min(
                previousDoc.resolve(
                  Math.max(0, Math.min(affectedPosition, previousDoc.content.size)),
                ).index(0),
                previousDoc.childCount - 1,
              ),
            )
          : null;
      const previousChild =
        previousDoc && previousDirtyIndex !== null ? previousDoc.child(previousDirtyIndex) : null;
      const nextChild =
        editor.state.doc.childCount > 0 ? editor.state.doc.child(nextDirtyStartIndex) : null;
      const previousSiblingInCurrentDoc =
        nextDirtyStartIndex > 0 ? editor.state.doc.child(nextDirtyStartIndex - 1) : null;
      const hasPreviousAutoPageBreakInCurrentDoc =
        previousSiblingInCurrentDoc?.type.name === "pageBreak" &&
        previousSiblingInCurrentDoc.attrs.auto === true;
      const needsPreviousBreakCleanup =
        previousChild?.type.name !== nextChild?.type.name ||
        hasPreviousAutoPageBreakInCurrentDoc;

      dirtyStartChildIndexRef.current =
        dirtyStartChildIndexRef.current === null
          ? nextDirtyStartIndex
          : Math.min(dirtyStartChildIndexRef.current, nextDirtyStartIndex);
      dirtyNeedsPreviousBreakCleanupRef.current =
        dirtyNeedsPreviousBreakCleanupRef.current || needsPreviousBreakCleanup;
      const isPasteLikeTransaction =
        transaction.getMeta?.("uiEvent") === "paste" ||
        transaction.getMeta?.("paste") === true;
      const isSimpleTypingTransaction =
        !isPasteLikeTransaction &&
        !needsPreviousBreakCleanup &&
        previousChild?.type.name === nextChild?.type.name &&
        previousDoc?.childCount === editor.state.doc.childCount;

      scheduleAutoPagination(isSimpleTypingTransaction ? "deferred" : "immediate");
    };

    editor.on("transaction", handleEditorTransaction);
    window.addEventListener("resize", handleWindowResize);
    scheduleAutoPagination("immediate");

    return () => {
      window.clearTimeout(pendingTimerId);
      window.clearTimeout(userScrollIdleTimerId);
      window.cancelAnimationFrame(frameId);
      resizeObserver?.disconnect();
      window.removeEventListener("resize", handleWindowResize);
      scrollContainer?.removeEventListener("scroll", handleScrollActivity);
      editor.off("transaction", handleEditorTransaction);
    };
  }, [editor, pageGap, pageHeight, pageMargins, paginationMode, usePaginatedDocument, zoomLevel]);

  return {
    editor,
    isEditable: !(disabled || readOnly),
    insertLocalImage,
  };
}
