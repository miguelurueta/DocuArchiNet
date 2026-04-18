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
import {
  insertPageBreakBeforeBlock,
  splitTextBlockAtPositionAndInsertPageBreak,
} from "./autoPageBreak";
import {
  removeAutoPageBreaks,
  resolveAutoPageBreakActions,
  syncAutoPageBreakSpacerHeights,
} from "./autoPagination";

const AUTO_PAGINATION_DEBOUNCE_MS = 150;
const IMAGE_INTERACTION_LOCK_MS = 600;

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

function syncControlledValue(editor: Editor, nextValue: string) {
  const currentValue = normalizeEditorValue(editor.getHTML());

  if (currentValue === nextValue) {
    return;
  }

  const { from, to } = editor.state.selection;
  editor.commands.setContent(nextValue, { emitUpdate: false });

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
  const initialContentRef = useRef(
    normalizeEditorValue(isControlled ? value : defaultValue),
  );
  const lastKnownValueRef = useRef(initialContentRef.current);
  const localImageUrlsRef = useRef(new Map<string, string>());
  const localImageScopeRef = useRef(buildLocalImageScope());
  const localImageSyncTokenRef = useRef(0);
  const lastImageInteractionAtRef = useRef(0);

  const editor = useEditor(
    {
      ...createAppEditorConfig({
        content: initialContentRef.current,
        placeholder,
        editable: !(disabled || readOnly),
        onUpdate: ({ editor: currentEditor }) => {
          const currentLocalImageIds = collectLocalImageIds(currentEditor);

          for (const [localImageId, url] of localImageUrlsRef.current.entries()) {
            if (!currentLocalImageIds.has(localImageId)) {
              URL.revokeObjectURL(url);
              localImageUrlsRef.current.delete(localImageId);
            }
          }

          const nextValue = normalizeEditorValue(currentEditor.getHTML());
          lastKnownValueRef.current = nextValue;
          onChange?.(nextValue);
        },
      }),
      immediatelyRender: false,
      shouldRerenderOnTransaction: false,
    },
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
      paginationMode !== "visual" ||
      pageHeight <= 0 ||
      !pageMargins
    ) {
      return undefined;
    }

    let frameId = 0;
    let pendingTimerId = 0;
    let resizeObserver: ResizeObserver | null = null;
    let isRunning = false;
    let suppressScheduling = false;
    const pendingImageElements = new WeakSet<HTMLImageElement>();

    const pageContentHeight = Math.max(
      1,
      pageHeight - pageMargins.top - pageMargins.bottom,
    );
    const pageStride = pageHeight + pageGap;
    const autoPaginationDebounceMs = resolveAutoPaginationDebounceMs();

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
          scheduleAutoPagination();
        };

        pendingImageElements.add(image);
        image.addEventListener("load", handleImageSettled, { once: true });
        image.addEventListener("error", handleImageSettled, { once: true });
      });
    };

    const runAutoPagination = () => {
      if (isRunning || suppressScheduling) {
        return;
      }

      frameId = window.requestAnimationFrame(() => {
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
          scheduleAutoPagination();
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
          const originalSelectionState = editor.state.selection;
          const originalSelectionRange = {
            from: originalSelectionState.from,
            to: originalSelectionState.to,
          };
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
          const scrollContainer = findScrollableAncestor(proseMirror);
          const preservedScrollTop = scrollContainer?.scrollTop ?? 0;
          const preservedScrollLeft = scrollContainer?.scrollLeft ?? 0;

          removeAutoPageBreaks(editor);

          let iterations = 0;

          while (iterations < 12) {
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
            });

            const nextAction = pageBreakActions[0];
            if (!nextAction) {
              break;
            }

            if (nextAction.type === "before") {
              const inserted = insertPageBreakBeforeBlock(editor, nextAction.position, {
                auto: true,
              });

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
              syncAutoPageBreakSpacerHeights(editor, repaginatedProseMirror, pageStride);
            }

            iterations += 1;
          }

          const finalProseMirror = editor.view.dom;
          if (finalProseMirror instanceof HTMLElement) {
            syncAutoPageBreakSpacerHeights(editor, finalProseMirror, pageStride);
            const maxPosition = editor.state.doc.content.size;
            const nextFrom = clampSelection(originalSelectionRange.from, maxPosition);
            const nextTo = clampSelection(originalSelectionRange.to, maxPosition);

            if (
              editor.state.selection.from !== nextFrom ||
              editor.state.selection.to !== nextTo
            ) {
              const resolvedImagePosition = findImagePositionByIdentity(
                editor,
                originalSelectedImage,
              );
              const restoredSelection =
                typeof resolvedImagePosition === "number" &&
                editor.state.doc.nodeAt(resolvedImagePosition)?.type.name === "image"
                  ? NodeSelection.create(editor.state.doc, resolvedImagePosition)
                  : originalSelectionState instanceof NodeSelection &&
                      editor.state.doc.nodeAt(nextFrom)
                    ? NodeSelection.create(editor.state.doc, nextFrom)
                    : TextSelection.create(editor.state.doc, nextFrom, nextTo);
              const selectionTransaction = editor.state.tr.setSelection(restoredSelection);
              editor.view.dispatch(selectionTransaction);
            }

            finalProseMirror.dispatchEvent(
              new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
            );

            if (scrollContainer) {
              scrollContainer.scrollTop = preservedScrollTop;
              scrollContainer.scrollLeft = preservedScrollLeft;
            }

            needsFollowUpRun =
              resolveAutoPageBreakActions({
                editor,
                proseMirror: finalProseMirror,
                pageContentHeight,
                pageStride,
                zoomLevel,
              }).length > 0;
          }
        } finally {
          window.setTimeout(() => {
            isRunning = false;
            suppressScheduling = false;
            if (needsFollowUpRun) {
              scheduleAutoPagination();
            }
          }, 0);
        }
      });
    };

    const scheduleAutoPagination = () => {
      if (suppressScheduling) {
        return;
      }

      window.clearTimeout(pendingTimerId);
      pendingTimerId = window.setTimeout(() => {
        runAutoPagination();
      }, autoPaginationDebounceMs);
    };

    const handleWindowResize = () => {
      scheduleAutoPagination();
    };

    if (typeof ResizeObserver !== "undefined") {
      resizeObserver = new ResizeObserver(() => {
        scheduleAutoPagination();
      });
      resizeObserver.observe(editor.view.dom);
    }

    editor.on("update", scheduleAutoPagination);
    window.addEventListener("resize", handleWindowResize);
    scheduleAutoPagination();

    return () => {
      window.clearTimeout(pendingTimerId);
      window.cancelAnimationFrame(frameId);
      resizeObserver?.disconnect();
      window.removeEventListener("resize", handleWindowResize);
      editor.off("update", scheduleAutoPagination);
    };
  }, [editor, pageGap, pageHeight, pageMargins, paginationMode, zoomLevel]);

  return {
    editor,
    isEditable: !(disabled || readOnly),
    insertLocalImage,
  };
}
