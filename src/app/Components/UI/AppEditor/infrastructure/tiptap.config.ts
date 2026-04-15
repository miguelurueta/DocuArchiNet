import type { UseEditorOptions } from "@tiptap/react";
import { NodeSelection } from "@tiptap/pm/state";
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
      handleClickOn(view, _pos, node, nodePos, event, direct) {
        if (!direct || node.type.name !== "image") {
          return false;
        }

        event.preventDefault();
        const transaction = view.state.tr.setSelection(
          NodeSelection.create(view.state.doc, nodePos),
        );
        view.dispatch(transaction);
        view.focus();
        return true;
      },
    },
    onUpdate,
  };
}
