import { useEffect, useRef } from "react";
import { useEditor } from "@tiptap/react";
import type { Editor } from "@tiptap/react";
import type { UseAppEditorOptions, UseAppEditorResult } from "../domain/editor.types";
import { clampSelection, normalizeEditorValue } from "../domain/editor.model";
import { createAppEditorConfig } from "../infrastructure/tiptap.config";

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

  const editor = useEditor(
    {
      ...createAppEditorConfig({
        content: initialContentRef.current,
        placeholder,
        editable: !(disabled || readOnly),
        onUpdate: ({ editor: currentEditor }) => {
          const nextValue = normalizeEditorValue(currentEditor.getHTML());
          lastKnownValueRef.current = nextValue;
          onChange?.(nextValue);
        },
      }),
      immediatelyRender: false,
      shouldRerenderOnTransaction: false,
    },
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

  return {
    editor,
    isEditable: !(disabled || readOnly),
  };
}
