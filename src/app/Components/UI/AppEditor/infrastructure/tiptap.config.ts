import type { UseEditorOptions } from "@tiptap/react";
import { buildAppEditorExtensions } from "./tiptap.extensions";

type CreateAppEditorConfigOptions = {
  content: string;
  placeholder?: string;
  editable: boolean;
  paginatedDocument?: boolean;
  onUpdate: NonNullable<UseEditorOptions["onUpdate"]>;
  shouldPreventScrollToSelection?: () => boolean;
};

export function createAppEditorConfig({
  content,
  placeholder,
  editable,
  paginatedDocument = false,
  onUpdate,
  shouldPreventScrollToSelection,
}: CreateAppEditorConfigOptions): UseEditorOptions {
  return {
    extensions: buildAppEditorExtensions(placeholder, { paginatedDocument }),
    content,
    editable,
    editorProps: {
      attributes: {
        class: "app-editor-prosemirror",
        spellcheck: "true",
        style: "overflow-anchor: none; overscroll-behavior: none;",
      },
      handleScrollToSelection: () => {
        if (typeof shouldPreventScrollToSelection === "function" && shouldPreventScrollToSelection()) {
          return true;
        }

        return false;
      },
    },
    onUpdate,
  };
}
