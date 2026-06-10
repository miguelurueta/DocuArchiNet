import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { AllSelection, NodeSelection } from "@tiptap/pm/state";
import { Editor } from "@tiptap/react";
import { buildAppEditorExtensions } from "../infrastructure/tiptap.extensions";
import { AppEditorToolbar } from "./AppEditorToolbar";

const MULTI_BLOCK_CONTENT = `
  <h1>Titulo inicial</h1>
  <p>Primer parrafo editable</p>
  <p>Segundo parrafo editable</p>
`;

let editor: Editor;

async function runWithAct<T>(callback: () => T) {
  let result: T;
  await act(async () => {
    result = callback();
  });
  return result!;
}

async function clickWithAct(element: Element) {
  await runWithAct(() => fireEvent.click(element));
}

function createAllSelection() {
  const allSelectionFactory = AllSelection as typeof AllSelection & {
    create?: (selectionDoc: typeof editor.state.doc) => AllSelection;
  };

  return typeof allSelectionFactory.create === "function"
    ? allSelectionFactory.create(editor.state.doc)
    : new AllSelection(editor.state.doc);
}

function renderToolbarWithAllSelection(content = MULTI_BLOCK_CONTENT) {
  Object.defineProperty(window, "innerWidth", {
    configurable: true,
    value: 1400,
  });
  window.dispatchEvent(new Event("resize"));

  editor = new Editor({
    extensions: buildAppEditorExtensions(),
    content,
  });
  editor.view.dispatch(editor.state.tr.setSelection(createAllSelection()));

  render(<AppEditorToolbar editor={editor} />);
}

function getTextBlockSummary() {
  const textBlocks: Array<{ type: string; textAlign?: string | null }> = [];
  editor.state.doc.descendants((node) => {
    if (node.isTextblock) {
      textBlocks.push({
        type: node.type.name,
        textAlign:
          typeof node.attrs.textAlign === "string" ? node.attrs.textAlign : null,
      });
    }
  });
  return textBlocks;
}

function countNodes(typeName: string) {
  let count = 0;
  editor.state.doc.descendants((node) => {
    if (node.type.name === typeName) {
      count += 1;
    }
  });
  return count;
}

function getTopLevelOrder() {
  const order: string[] = [];
  editor.state.doc.forEach((node) => {
    if (node.type.name === "paragraph" && node.content.size === 0) {
      return;
    }

    order.push(
      node.type.name === "image"
        ? `image:${typeof node.attrs.src === "string" ? node.attrs.src : ""}`
        : node.textContent,
    );
  });
  return order;
}

function getImagePositionAtIndex(index: number) {
  const imagePositions: number[] = [];
  editor.state.doc.descendants((node, pos) => {
    if (node.type.name === "image") {
      imagePositions.push(pos);
    }

    return undefined;
  });

  const imagePosition = imagePositions[index] ?? null;
  expect(imagePosition).not.toBeNull();

  return imagePosition as number;
}

async function selectImageAtIndex(index: number) {
  const imagePosition = getImagePositionAtIndex(index);
  await runWithAct(() =>
    editor.view.dispatch(
      editor.state.tr.setSelection(
        NodeSelection.create(editor.state.doc, imagePosition),
      ),
    ),
  );

  return imagePosition;
}

async function selectFirstImage() {
  return selectImageAtIndex(0);
}

describe("AppEditorToolbar selection preservation", () => {
  beforeEach(() => {
    editor = null as unknown as Editor;
  });

  afterEach(() => {
    editor?.destroy();
  });

  it.each([
    ["Negrita", "strong"],
    ["Cursiva", "em"],
    ["Subrayado", "u"],
  ])("aplica %s sobre Ctrl+A / AllSelection", async (label, expectedTag) => {
    renderToolbarWithAllSelection();

    await clickWithAct(screen.getByRole("button", { name: label }));

    await waitFor(() => {
      expect(editor.getHTML()).toContain(`<${expectedTag}>Titulo inicial</${expectedTag}>`);
      expect(editor.getHTML()).toContain(
        `<${expectedTag}>Primer parrafo editable</${expectedTag}>`,
      );
      expect(editor.getHTML()).toContain(
        `<${expectedTag}>Segundo parrafo editable</${expectedTag}>`,
      );
      expect(editor.state.selection).toBeInstanceOf(AllSelection);
    });
  });

  it.each([
    ["Izquierda", "left"],
    ["Centro", "center"],
    ["Derecha", "right"],
    ["Justificar", "justify"],
  ])("aplica alineacion %s sobre Ctrl+A / AllSelection", async (label, align) => {
    renderToolbarWithAllSelection();

    await clickWithAct(screen.getByRole("button", { name: "Alineacion de texto" }));
    await clickWithAct(await screen.findByText(label));

    await waitFor(() => {
      expect(getTextBlockSummary()).toEqual([
        { type: "heading", textAlign: align },
        { type: "paragraph", textAlign: align },
        { type: "paragraph", textAlign: align },
      ]);
      expect(editor.state.selection).toBeInstanceOf(AllSelection);
    });
  });

  it.each([
    ["Titulo 1", "heading", 1],
    ["Titulo 2", "heading", 2],
    ["Titulo 3", "heading", 3],
  ])("aplica %s sobre Ctrl+A / AllSelection", async (label, expectedType, level) => {
    renderToolbarWithAllSelection("<p>Uno</p><p>Dos</p><p>Tres</p>");

    await clickWithAct(screen.getByRole("button", { name: "Nivel de encabezado" }));
    await clickWithAct(await screen.findByText(label));

    await waitFor(() => {
      const textBlocks: Array<{ type: string; level?: number; text: string }> = [];
      editor.state.doc.descendants((node) => {
        if (node.isTextblock) {
          textBlocks.push({
            type: node.type.name,
            level: typeof node.attrs.level === "number" ? node.attrs.level : undefined,
            text: node.textContent,
          });
        }
      });

      expect(textBlocks.filter((node) => node.text.length > 0)).toEqual([
        { type: expectedType, level, text: "Uno" },
        { type: expectedType, level, text: "Dos" },
        { type: expectedType, level, text: "Tres" },
      ]);
      expect(editor.state.selection).toBeInstanceOf(AllSelection);
    });
  });

  it.each([
    ["Lista con vietas", "bulletList"],
    ["Lista numerada", "orderedList"],
  ])("aplica %s sobre Ctrl+A / AllSelection", async (label, listType) => {
    renderToolbarWithAllSelection("<p>Uno</p><p>Dos</p><p>Tres</p>");

    await clickWithAct(screen.getByRole("button", { name: label }));

    await waitFor(() => {
      expect(countNodes(listType)).toBe(1);
      expect(countNodes("listItem")).toBe(3);
      expect(editor.state.selection).toBeInstanceOf(AllSelection);
    });
  });

  it("aplica alineacion sobre una seleccion con listas y parrafos", async () => {
    renderToolbarWithAllSelection("<ul><li><p>Uno</p></li></ul><p>Dos</p>");

    await clickWithAct(screen.getByRole("button", { name: "Alineacion de texto" }));
    await clickWithAct(await screen.findByText("Justificar"));

    await waitFor(() => {
      expect(
        getTextBlockSummary().filter((node) => node.type === "paragraph"),
      ).toEqual([
        { type: "paragraph", textAlign: "justify" },
        { type: "paragraph", textAlign: "justify" },
      ]);
      expect(editor.state.selection).toBeInstanceOf(AllSelection);
    });
  });

  it("aplica formato sobre una seleccion con imagen y texto sin perder la imagen", async () => {
    renderToolbarWithAllSelection(
      '<p>Antes</p><img src="https://example.com/image.png"><p>Despues</p>',
    );

    await clickWithAct(screen.getByRole("button", { name: "Negrita" }));

    await waitFor(() => {
      expect(editor.getHTML()).toContain("<strong>Antes</strong>");
      expect(editor.getHTML()).toContain("<strong>Despues</strong>");
      expect(countNodes("image")).toBe(1);
      expect(editor.state.selection).toBeInstanceOf(AllSelection);
    });
  });

  it("mueve una imagen seleccionada hacia arriba y hacia abajo", async () => {
    renderToolbarWithAllSelection(
      '<p>Uno</p><img src="https://example.com/image.png"><p>Dos</p>',
    );
    await selectFirstImage();

    await clickWithAct(screen.getByRole("button", { name: "Insertar imagen" }));
    await clickWithAct(await screen.findByRole("button", { name: "Mover arriba" }));

    await waitFor(() => {
      expect(getTopLevelOrder()).toEqual(["image:https://example.com/image.png", "Uno", "Dos"]);
      expect(editor.state.selection).toBeInstanceOf(NodeSelection);
    });

    await clickWithAct(screen.getByRole("button", { name: "Mover abajo" }));

    await waitFor(() => {
      expect(getTopLevelOrder()).toEqual(["Uno", "image:https://example.com/image.png", "Dos"]);
      expect(editor.state.selection).toBeInstanceOf(NodeSelection);
    });

    await clickWithAct(screen.getByRole("button", { name: "Mover abajo" }));

    await waitFor(() => {
      expect(getTopLevelOrder()).toEqual(["Uno", "Dos", "image:https://example.com/image.png"]);
      expect(editor.state.selection).toBeInstanceOf(NodeSelection);
    });
  }, 15000);

  it("intercambia tres imagenes consecutivas y mantiene NodeSelection", async () => {
    renderToolbarWithAllSelection(
      [
        '<img src="https://example.com/uno.png">',
        '<img src="https://example.com/dos.png">',
        '<img src="https://example.com/tres.png">',
      ].join(""),
    );
    const originPosition = await selectImageAtIndex(1);
    const dispatchSpy = vi.spyOn(editor.view, "dispatch");

    await clickWithAct(screen.getByRole("button", { name: "Insertar imagen" }));
    await clickWithAct(await screen.findByRole("button", { name: "Mover arriba" }));

    await waitFor(() => {
      expect(getTopLevelOrder()).toEqual([
        "image:https://example.com/dos.png",
        "image:https://example.com/uno.png",
        "image:https://example.com/tres.png",
      ]);
      expect(editor.state.selection).toBeInstanceOf(NodeSelection);
      expect(editor.state.selection.from).not.toBe(originPosition);
      expect(dispatchSpy).toHaveBeenCalled();
    });

    const positionAfterMoveUp = editor.state.selection.from;
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Mover abajo" })).not.toBeDisabled();
    });
    await clickWithAct(screen.getByRole("button", { name: "Mover abajo" }));

    await waitFor(() => {
      expect(getTopLevelOrder()).toEqual([
        "image:https://example.com/uno.png",
        "image:https://example.com/dos.png",
        "image:https://example.com/tres.png",
      ]);
      expect(editor.state.selection).toBeInstanceOf(NodeSelection);
      expect(editor.state.selection.from).not.toBe(positionAfterMoveUp);
    });
  }, 15000);
});
