import { useCallback, useEffect, useRef } from "react";
import { useEditor } from "@tiptap/react";
import type { Editor } from "@tiptap/react";
import type { UseAppEditorOptions, UseAppEditorResult } from "../domain/editor.types";
import { clampSelection, normalizeEditorValue } from "../domain/editor.model";
import { createAppEditorConfig } from "../infrastructure/tiptap.config";
import { generateLocalImageId } from "./localImageIds";
import { appEditorImageStore } from "../infrastructure/indexeddb/appEditorImageStore";
import type { LocalImage } from "../infrastructure/indexeddb/localImage.types";

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

export function useAppEditor({
  value,
  defaultValue,
  onChange,
  placeholder,
  disabled = false,
  readOnly = false,
}: UseAppEditorOptions): UseAppEditorResult {
  const isControlled = value !== undefined;
  const initialContentRef = useRef(
    normalizeEditorValue(isControlled ? value : defaultValue),
  );
  const lastKnownValueRef = useRef(initialContentRef.current);
  const localImageUrlsRef = useRef(new Map<string, string>());
  const localImageScopeRef = useRef(buildLocalImageScope());
  const localImageSyncTokenRef = useRef(0);

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

      chain
        .setImage({
          src: objectUrl,
          localImageId,
          source: "local",
        })
        .run();

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

  return {
    editor,
    isEditable: !(disabled || readOnly),
    insertLocalImage,
  };
}
