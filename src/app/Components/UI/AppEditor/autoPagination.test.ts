import { Editor } from "@tiptap/core";
import { describe, expect, it } from "vitest";
import { resolveAutoPageBreakActions } from "./application/autoPagination";
import { buildAppEditorExtensions } from "./infrastructure/tiptap.extensions";

function createDomRect(top: number, bottom: number): DOMRect {
  return {
    x: 0,
    y: top,
    width: 0,
    height: bottom - top,
    top,
    right: 0,
    bottom,
    left: 0,
    toJSON: () => ({}),
  } as DOMRect;
}

describe("autoPagination", () => {
  it("mueve una bulletList completa a la siguiente pagina cuando desborda el final actual", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>uno</p><ul><li><p>dos</p></li></ul>",
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [paragraphBlock, bulletListBlock] = Array.from(proseMirror.children) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 160);
    paragraphBlock.getBoundingClientRect = () => createDomRect(0, 40);
    bulletListBlock.getBoundingClientRect = () => createDomRect(70, 110);

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toEqual([
      {
        type: "before",
        position: editor.state.doc.child(0).nodeSize,
      },
    ]);

    editor.destroy();
  });

  it("divide una bulletList por item cuando parte de la lista ya cabe en la pagina actual", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<ul><li><p>uno</p></li><li><p>dos</p></li></ul>",
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [bulletListBlock] = Array.from(proseMirror.children) as HTMLElement[];
    const [firstItem, secondItem] = Array.from(bulletListBlock.children) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 220);
    bulletListBlock.getBoundingClientRect = () => createDomRect(0, 130);
    firstItem.getBoundingClientRect = () => createDomRect(0, 40);
    secondItem.getBoundingClientRect = () => createDomRect(80, 130);

    Object.defineProperty(firstItem, "offsetTop", { configurable: true, value: 0 });
    Object.defineProperty(firstItem, "offsetHeight", { configurable: true, value: 40 });
    Object.defineProperty(secondItem, "offsetTop", { configurable: true, value: 80 });
    Object.defineProperty(secondItem, "offsetHeight", { configurable: true, value: 50 });

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toEqual([
      {
        type: "list-item",
        listPosition: 0,
        itemPosition: 8,
      },
    ]);

    editor.destroy();
  });

  it("mueve una imagen completa a la siguiente pagina cuando ya no cabe en el espacio restante", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: '<p>intro</p><img src="https://cdn.example.com/image.png" />',
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [paragraphBlock, imageBlock] = Array.from(proseMirror.children) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 220);
    paragraphBlock.getBoundingClientRect = () => createDomRect(0, 40);
    imageBlock.getBoundingClientRect = () => createDomRect(70, 130);

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toEqual([
      {
        type: "before",
        position: editor.state.doc.child(0).nodeSize,
      },
    ]);

    editor.destroy();
  });

  it("divide una taskList por item cuando el siguiente task item ya no cabe en la pagina actual", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<ul data-type="taskList"><li data-type="taskItem" data-checked="false"><p>uno</p></li><li data-type="taskItem" data-checked="false"><p>dos</p></li></ul>',
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [taskListBlock] = Array.from(proseMirror.children) as HTMLElement[];
    const [firstItem, secondItem] = Array.from(taskListBlock.children) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 220);
    taskListBlock.getBoundingClientRect = () => createDomRect(0, 130);
    firstItem.getBoundingClientRect = () => createDomRect(0, 40);
    secondItem.getBoundingClientRect = () => createDomRect(82, 130);

    Object.defineProperty(firstItem, "offsetTop", { configurable: true, value: 0 });
    Object.defineProperty(firstItem, "offsetHeight", { configurable: true, value: 40 });
    Object.defineProperty(secondItem, "offsetTop", { configurable: true, value: 82 });
    Object.defineProperty(secondItem, "offsetHeight", { configurable: true, value: 48 });

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toEqual([
      {
        type: "list-item",
        listPosition: 0,
        itemPosition: 8,
      },
    ]);

    editor.destroy();
  });

  it("divide una orderedList por item cuando el siguiente item numerado ya no cabe", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<ol><li><p>uno</p></li><li><p>dos</p></li></ol>",
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [orderedListBlock] = Array.from(proseMirror.children) as HTMLElement[];
    const [firstItem, secondItem] = Array.from(orderedListBlock.children) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 220);
    orderedListBlock.getBoundingClientRect = () => createDomRect(0, 130);
    firstItem.getBoundingClientRect = () => createDomRect(0, 40);
    secondItem.getBoundingClientRect = () => createDomRect(82, 130);

    Object.defineProperty(firstItem, "offsetTop", { configurable: true, value: 0 });
    Object.defineProperty(firstItem, "offsetHeight", { configurable: true, value: 40 });
    Object.defineProperty(secondItem, "offsetTop", { configurable: true, value: 82 });
    Object.defineProperty(secondItem, "offsetHeight", { configurable: true, value: 48 });

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toEqual([
      {
        type: "list-item",
        listPosition: 0,
        itemPosition: 8,
      },
    ]);

    editor.destroy();
  });

  it("divide un parrafo largo antes del borde inferior usando una posicion de split preventiva", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>uno dos tres cuatro cinco seis siete ocho nueve diez once doce trece catorce quince</p>",
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [paragraphBlock] = Array.from(proseMirror.children) as HTMLElement[];
    const originalCoordsAtPos = editor.view.coordsAtPos.bind(editor.view);
    const textEnd = editor.state.doc.child(0).nodeSize - 1;

    proseMirror.getBoundingClientRect = () => createDomRect(0, 220);
    paragraphBlock.getBoundingClientRect = () => createDomRect(0, 150);
    editor.view.coordsAtPos = ((position: number) => ({
      left: 0,
      right: 0,
      top: 0,
      bottom: position < textEnd - 24 ? 82 : 112,
    })) as typeof editor.view.coordsAtPos;

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toHaveLength(1);
    expect(actions[0]?.type).toBe("split");
    if (actions[0]?.type === "split") {
      expect(actions[0].position).toBeGreaterThan(1);
      expect(actions[0].position).toBeLessThan(textEnd - 1);
    }

    editor.view.coordsAtPos = originalCoordsAtPos;
    editor.destroy();
  });

  it("parte un parrafo corto en el ultimo espacio disponible en vez de mover el bloque completo", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>intro</p><p>uno dos tres cuatro cinco seis siete ocho nueve diez</p>",
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [, secondParagraphBlock] = Array.from(proseMirror.children) as HTMLElement[];
    const originalCoordsAtPos = editor.view.coordsAtPos.bind(editor.view);
    const secondBlockStart = editor.state.doc.child(0).nodeSize + 1;
    const secondBlockEnd =
      editor.state.doc.child(0).nodeSize + editor.state.doc.child(1).nodeSize - 1;

    proseMirror.getBoundingClientRect = () => createDomRect(0, 240);
    secondParagraphBlock.getBoundingClientRect = () => createDomRect(70, 118);
    editor.view.coordsAtPos = ((position: number) => ({
      left: 0,
      right: 0,
      top: 0,
      bottom: position < secondBlockEnd - 12 ? 82 : 108,
    })) as typeof editor.view.coordsAtPos;

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toHaveLength(1);
    expect(actions[0]?.type).toBe("split");
    if (actions[0]?.type === "split") {
      expect(actions[0].position).toBeGreaterThan(secondBlockStart + 1);
      expect(actions[0].position).toBeLessThan(secondBlockEnd);
    }

    editor.view.coordsAtPos = originalCoordsAtPos;
    editor.destroy();
  });

  it("permite split por caracter cuando no existe un corte limpio cercano dentro del texto", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>intro</p><p>supercalifragilisticoespialidoso</p>",
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [, secondParagraphBlock] = Array.from(proseMirror.children) as HTMLElement[];
    const secondBlockPosition = editor.state.doc.child(0).nodeSize;
    const originalCoordsAtPos = editor.view.coordsAtPos.bind(editor.view);

    proseMirror.getBoundingClientRect = () => createDomRect(0, 240);
    secondParagraphBlock.getBoundingClientRect = () => createDomRect(70, 118);
    editor.view.coordsAtPos = ((position: number) => ({
      left: 0,
      right: 0,
      top: 0,
      bottom: position < secondBlockPosition + 20 ? 82 : 108,
    })) as typeof editor.view.coordsAtPos;

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toHaveLength(1);
    expect(actions[0]?.type).toBe("split");
    if (actions[0]?.type === "split") {
      expect(actions[0].position).toBeGreaterThan(secondBlockPosition);
    }

    editor.view.coordsAtPos = originalCoordsAtPos;
    editor.destroy();
  });

  it("no inserta un pageBreak extra antes de un bloque top-level que ya inicia en la nueva pagina", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: '<p>uno</p><img src="https://cdn.example.com/image.png" />',
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [, imageBlock] = Array.from(proseMirror.children) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 320);
    imageBlock.getBoundingClientRect = () => createDomRect(120, 222);

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toEqual([]);

    editor.destroy();
  });

  it("prioriza el primer bloque en conflicto en contenido mixto antes de procesar bloques posteriores", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<p>intro</p><ul><li><p>uno</p></li><li><p>dos</p></li></ul><img src="https://cdn.example.com/image.png" />',
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [paragraphBlock, bulletListBlock] = Array.from(proseMirror.children) as HTMLElement[];
    const [firstItem, secondItem] = Array.from(bulletListBlock.children) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 320);
    paragraphBlock.getBoundingClientRect = () => createDomRect(0, 30);
    bulletListBlock.getBoundingClientRect = () => createDomRect(32, 160);
    firstItem.getBoundingClientRect = () => createDomRect(32, 72);
    secondItem.getBoundingClientRect = () => createDomRect(110, 160);

    Object.defineProperty(firstItem, "offsetTop", { configurable: true, value: 0 });
    Object.defineProperty(firstItem, "offsetHeight", { configurable: true, value: 40 });
    Object.defineProperty(secondItem, "offsetTop", { configurable: true, value: 78 });
    Object.defineProperty(secondItem, "offsetHeight", { configurable: true, value: 50 });

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions[0]).toEqual({
      type: "list-item",
      listPosition: editor.state.doc.child(0).nodeSize,
      itemPosition: editor.state.doc.child(0).nodeSize + 8,
    });

    editor.destroy();
  });

  it("prioriza el primer parrafo desbordado en un contenido pegado largo antes de bloques posteriores", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        "<p>uno dos tres cuatro cinco seis siete ocho nueve diez once doce trece catorce quince dieciseis diecisiete dieciocho</p><p>segundo bloque del pegado</p><ul><li><p>item uno</p></li><li><p>item dos</p></li></ul>",
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [firstParagraphBlock, secondParagraphBlock] = Array.from(
      proseMirror.children,
    ) as HTMLElement[];
    const originalCoordsAtPos = editor.view.coordsAtPos.bind(editor.view);
    const firstParagraphEnd = editor.state.doc.child(0).nodeSize - 1;

    proseMirror.getBoundingClientRect = () => createDomRect(0, 360);
    firstParagraphBlock.getBoundingClientRect = () => createDomRect(0, 150);
    secondParagraphBlock.getBoundingClientRect = () => createDomRect(154, 194);

    editor.view.coordsAtPos = ((position: number) => ({
      left: 0,
      right: 0,
      top: 0,
      bottom: position < firstParagraphEnd - 24 ? 84 : 116,
    })) as typeof editor.view.coordsAtPos;

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
    });

    expect(actions).toHaveLength(1);
    expect(actions[0]?.type).toBe("split");
    if (actions[0]?.type === "split") {
      expect(actions[0].position).toBeGreaterThan(1);
      expect(actions[0].position).toBeLessThan(firstParagraphEnd - 1);
    }

    editor.view.coordsAtPos = originalCoordsAtPos;
    editor.destroy();
  });

  it("permite invalidacion incremental comenzando desde un bloque top-level especifico", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: '<p>primero</p><p>segundo</p><img src="https://cdn.example.com/tercero.png" />',
    });

    const proseMirror = editor.view.dom as HTMLElement;
    const [, secondParagraphBlock, thirdBlock] = Array.from(
      proseMirror.children,
    ) as HTMLElement[];

    proseMirror.getBoundingClientRect = () => createDomRect(0, 360);
    secondParagraphBlock.getBoundingClientRect = () => createDomRect(130, 180);
    thirdBlock.getBoundingClientRect = () => createDomRect(250, 370);

    const actions = resolveAutoPageBreakActions({
      editor: editor as never,
      proseMirror,
      pageContentHeight: 100,
      pageStride: 120,
      startChildIndex: 2,
    });

    expect(actions).toEqual([
      {
        type: "before",
        position: editor.state.doc.child(0).nodeSize + editor.state.doc.child(1).nodeSize,
      },
    ]);

    editor.destroy();
  });
});
