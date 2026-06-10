import { useEffect } from "react";
import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { EditorContent, type Editor } from "@tiptap/react";
import { NodeSelection } from "@tiptap/pm/state";
import { useAppEditor } from "./useAppEditor";
import { AppEditorToolbar } from "../presentation/AppEditorToolbar";

type HistoryEditor = Editor & {
  appEditorHistory?: {
    undo?: () => boolean;
    redo?: () => boolean;
  };
};

function HistoryHarness({
  controlled = false,
  onEditor,
}: {
  controlled?: boolean;
  onEditor: (editor: HistoryEditor) => void;
}) {
  const editorState = useAppEditor({
    defaultValue: controlled ? undefined : "<p>Inicial</p>",
    value: controlled ? "<p>Inicial</p>" : undefined,
    onChange: vi.fn(),
    paginationMode: "none",
  });

  useEffect(() => {
    if (editorState.editor) {
      onEditor(editorState.editor as HistoryEditor);
    }
  }, [editorState.editor, onEditor]);

  return (
    <>
      <EditorContent editor={editorState.editor} />
      <AppEditorToolbar editor={editorState.editor} />
    </>
  );
}

async function renderHistoryHarness(controlled = false) {
  let editor: HistoryEditor | null = null;
  const rendered = render(
    <HistoryHarness
      controlled={controlled}
      onEditor={(nextEditor) => {
        editor = nextEditor;
      }}
    />,
  );

  await waitFor(() => {
    expect(editor).not.toBeNull();
    expect(typeof editor?.appEditorHistory?.undo).toBe("function");
    expect(typeof editor?.appEditorHistory?.redo).toBe("function");
  });

  return {
    editor: editor as HistoryEditor,
    ...rendered,
  };
}

async function runWithAct<T>(callback: () => T) {
  let result: T;
  await act(async () => {
    result = callback();
  });
  return result!;
}

function setCursorAtDocumentEnd(editor: Editor) {
  editor.commands.setTextSelection(editor.state.doc.content.size - 1);
}

function getText(editor: Editor) {
  return editor.state.doc.textContent;
}

function getFirstImagePosition(editor: Editor) {
  let imagePosition: number | null = null;
  editor.state.doc.descendants((node, position) => {
    if (imagePosition === null && node.type.name === "image") {
      imagePosition = position;
    }
  });

  return imagePosition;
}

function runEditorKeyboardShortcut(editor: Editor, options: KeyboardEventInit) {
  const event = {
    altKey: false,
    ctrlKey: false,
    defaultPrevented: false,
    key: "",
    metaKey: false,
    preventDefault: vi.fn(function preventDefault(this: { defaultPrevented: boolean }) {
      this.defaultPrevented = true;
    }),
    shiftKey: false,
    stopImmediatePropagation: vi.fn(),
    stopPropagation: vi.fn(),
    target: editor.view.dom,
    ...options,
  } as unknown as KeyboardEvent;

  return editor.view.props.handleKeyDown?.(editor.view, event) ?? false;
}

describe("useAppEditor history", () => {
  it("usa appEditorHistory para los botones Deshacer y Rehacer", async () => {
    const { editor } = await renderHistoryHarness();

    await runWithAct(() => {
      setCursorAtDocumentEnd(editor);
      editor.commands.insertContent(" texto");
    });
    await waitFor(() => expect(getText(editor)).toBe("Inicial texto"));

    await runWithAct(() => fireEvent.click(screen.getByRole("button", { name: "Deshacer" })));
    await waitFor(() => expect(getText(editor)).toBe("Inicial"));

    await runWithAct(() => fireEvent.click(screen.getByRole("button", { name: "Rehacer" })));
    await waitFor(() => expect(getText(editor)).toBe("Inicial texto"));
  });

  it("intercepta Ctrl+Z, Ctrl+Y y Ctrl+Shift+Z sin usar UndoRedo nativo", async () => {
    const { editor } = await renderHistoryHarness();

    expect(editor.commands.undo).toBeUndefined();
    expect(editor.commands.redo).toBeUndefined();

    await runWithAct(() => {
      setCursorAtDocumentEnd(editor);
      editor.commands.insertContent(" teclado");
    });
    await waitFor(() => expect(getText(editor)).toBe("Inicial teclado"));

    const undo = editor.appEditorHistory?.undo;
    const redo = editor.appEditorHistory?.redo;
    const undoSpy = vi.fn(() => undo?.() ?? false);
    const redoSpy = vi.fn(() => redo?.() ?? false);
    editor.appEditorHistory = {
      undo: undoSpy,
      redo: redoSpy,
    };

    expect(await runWithAct(() => runEditorKeyboardShortcut(editor, { key: "z", ctrlKey: true }))).toBe(true);
    expect(undoSpy).toHaveBeenCalledTimes(1);
    await waitFor(() => expect(getText(editor)).toBe("Inicial"));

    expect(await runWithAct(() => runEditorKeyboardShortcut(editor, { key: "y", ctrlKey: true }))).toBe(true);
    expect(redoSpy).toHaveBeenCalledTimes(1);
    await waitFor(() => expect(getText(editor)).toBe("Inicial teclado"));

    expect(await runWithAct(() => runEditorKeyboardShortcut(editor, { key: "z", ctrlKey: true }))).toBe(true);
    expect(undoSpy).toHaveBeenCalledTimes(2);
    await waitFor(() => expect(getText(editor)).toBe("Inicial"));

    expect(
      await runWithAct(() =>
        runEditorKeyboardShortcut(editor, { key: "z", ctrlKey: true, shiftKey: true }),
      ),
    ).toBe(true);
    expect(redoSpy).toHaveBeenCalledTimes(2);
    await waitFor(() => expect(getText(editor)).toBe("Inicial teclado"));
  });

  it("restaura texto, formato, alineacion, listas y task list", async () => {
    const { editor } = await renderHistoryHarness();

    await runWithAct(() => {
      editor.commands.setTextSelection({ from: 1, to: 8 });
      editor.commands.toggleBold();
    });
    await waitFor(() => expect(editor.getHTML()).toContain("<strong>Inicial</strong>"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain("<strong>"));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("<strong>Inicial</strong>"));

    await runWithAct(() => editor.commands.setTextAlign("center"));
    await waitFor(() => expect(editor.getHTML()).toContain("text-align: center"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain("text-align: center"));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("text-align: center"));

    await runWithAct(() => editor.commands.toggleBulletList());
    await waitFor(() => expect(editor.getHTML()).toContain("<ul>"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain("<ul>"));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("<ul>"));

    await runWithAct(() => {
      editor.commands.toggleBulletList();
      editor.commands.toggleOrderedList();
    });
    await waitFor(() => expect(editor.getHTML()).toContain("<ol>"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain("<ol>"));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("<ol>"));

    await runWithAct(() => {
      editor.commands.toggleOrderedList();
      editor.commands.toggleTaskList();
    });
    await waitFor(() => expect(editor.getHTML()).toContain('data-type="taskList"'));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain('data-type="taskList"'));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain('data-type="taskList"'));
  });

  it("restaura enlaces", async () => {
    const { editor } = await renderHistoryHarness();

    await runWithAct(() => {
      editor.commands.setTextSelection({ from: 1, to: 8 });
      editor.commands.setLink({ href: "https://example.com/a" });
    });
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/a"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain("https://example.com/a"));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/a"));

    await runWithAct(() => editor.commands.setLink({ href: "https://example.com/b" }));
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/b"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/a"));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/b"));

    await runWithAct(() => editor.commands.unsetLink());
    await waitFor(() => expect(editor.getHTML()).not.toContain("https://example.com/b"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/b"));
  });

  it("preserva NodeSelection al deshacer y rehacer cambios de imagen", async () => {
    const { editor } = await renderHistoryHarness();

    await runWithAct(() => editor.commands.setImage({ src: "https://example.com/a.png" }));
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/a.png"));
    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain("https://example.com/a.png"));
    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain("https://example.com/a.png"));

    const imagePosition = getFirstImagePosition(editor);
    expect(imagePosition).not.toBeNull();

    await runWithAct(() => {
      editor.view.dispatch(
        editor.state.tr.setSelection(NodeSelection.create(editor.state.doc, imagePosition ?? 0)),
      );
      editor.commands.updateAttributes("image", { width: "50%" });
    });
    await waitFor(() => expect(editor.getHTML()).toContain('width="50%"'));
    expect(editor.state.selection).toBeInstanceOf(NodeSelection);

    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).not.toContain('width="50%"'));
    expect(editor.state.selection).toBeInstanceOf(NodeSelection);

    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(editor.getHTML()).toContain('width="50%"'));
    expect(editor.state.selection).toBeInstanceOf(NodeSelection);
  });

  it("instala appEditorHistory tambien en modo controlado", async () => {
    const { editor } = await renderHistoryHarness(true);

    await runWithAct(() => {
      setCursorAtDocumentEnd(editor);
      editor.commands.insertContent(" controlado");
    });
    await waitFor(() => expect(getText(editor)).toBe("Inicial controlado"));

    expect(await runWithAct(() => editor.appEditorHistory?.undo?.())).toBe(true);
    await waitFor(() => expect(getText(editor)).toBe("Inicial"));

    expect(await runWithAct(() => editor.appEditorHistory?.redo?.())).toBe(true);
    await waitFor(() => expect(getText(editor)).toBe("Inicial controlado"));
  });
});
