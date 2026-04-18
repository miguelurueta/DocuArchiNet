import type { UseEditorOptions } from "@tiptap/react";
import { buildAppEditorExtensions } from "./tiptap.extensions";

type CreateAppEditorConfigOptions = {
  content: string;
  placeholder?: string;
  editable: boolean;
  onUpdate: NonNullable<UseEditorOptions["onUpdate"]>;
};

export function createAppEditorConfig({
  content,
  placeholder,
  editable,
  onUpdate,
}: CreateAppEditorConfigOptions): UseEditorOptions {
  return {
    extensions: buildAppEditorExtensions(placeholder),
    content,
    editable,
    editorProps: {
      attributes: {
        class: "app-editor-prosemirror",
        spellcheck: "true",
      },
    },
    onUpdate,
  };
}
