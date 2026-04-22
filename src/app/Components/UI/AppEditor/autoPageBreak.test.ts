import { Editor } from "@tiptap/core";
import { describe, expect, it } from "vitest";
import { buildAppEditorExtensions } from "./infrastructure/tiptap.extensions";
import {
  insertPageBreakBeforeBlock,
  splitListBlockBeforeItemAndInsertPageBreak,
  splitBlockAndInsertPageBreak,
  splitTextBlockAtPositionAndInsertPageBreak,
} from "./application/autoPageBreak";
import {
  removeAutoPageBreaks,
  resolveAutoPageBreakCleanupStartPosition,
} from "./application/autoPagination";

describe("autoPageBreak", () => {
  it("divide el parrafo actual e inserta un pageBreak en el cursor", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>abcdef</p>",
    });

    editor.commands.setTextSelection(4);

    const result = splitBlockAndInsertPageBreak(editor as never);

    expect(result).toBe(true);
    expect(editor.getHTML()).toContain('<div data-page-break="true"></div>');
    expect(editor.getJSON().content?.map((node) => node.type)).toEqual([
      "paragraph",
      "pageBreak",
      "paragraph",
    ]);

    editor.destroy();
  });

  it("no inserta un pageBreak si el cursor esta al inicio de un parrafo ya separado", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: '<p>Uno</p><div data-page-break="true"></div><p>Dos</p>',
    });

    editor.commands.setTextSelection(7);

    const result = splitBlockAndInsertPageBreak(editor as never);

    expect(result).toBe(false);
    expect(editor.getJSON().content?.filter((node) => node.type === "pageBreak")).toHaveLength(1);

    editor.destroy();
  });

  it("permite insertar el pageBreak en una posicion especifica dentro del parrafo actual", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>abcdef</p>",
    });

    editor.commands.setTextSelection(7);

    const result = splitTextBlockAtPositionAndInsertPageBreak(editor as never, 4);

    expect(result).toBe(true);
    expect(editor.getJSON().content?.map((node) => node.type)).toEqual([
      "paragraph",
      "pageBreak",
      "paragraph",
    ]);
    expect(editor.getHTML()).toContain("<div data-page-break=\"true\"></div>");
    expect(editor.getHTML()).toContain(">abc</p>");
    expect(editor.getHTML()).toContain(">def</p>");

    editor.destroy();
  });

  it("usa la posicion objetivo aunque la seleccion actual este en otro bloque", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>uno</p><p>abcdef</p>",
    });

    editor.commands.setTextSelection(2);

    const result = splitTextBlockAtPositionAndInsertPageBreak(editor as never, 10, {
      auto: true,
    });

    expect(result).toBe(true);
    expect(editor.getJSON().content?.map((node) => node.type)).toEqual([
      "paragraph",
      "paragraph",
      "pageBreak",
      "paragraph",
    ]);
    expect(editor.getHTML()).toContain('data-page-break-auto="true"');

    editor.destroy();
  });

  it("recompone el parrafo original al retirar un pageBreak automatico de split", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<p>abc</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true"></div><p>def</p>',
    });

    const result = removeAutoPageBreaks(editor as never);

    expect(result).toBe(true);
    expect(editor.getJSON().content?.map((node) => node.type)).toEqual(["paragraph"]);
    expect(editor.getHTML()).toContain(">abcdef</p>");

    editor.destroy();
  });

  it("inserta un pageBreak antes de un bloque completo cuando ya no cabe en la pagina", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>uno</p><p>dos</p>",
    });

    const result = insertPageBreakBeforeBlock(editor as never, 6, {
      auto: true,
    });

    expect(result).toBe(true);
    expect(editor.getJSON().content?.map((node) => node.type)).toEqual([
      "paragraph",
      "paragraph",
      "pageBreak",
      "paragraph",
    ]);
    expect(editor.getHTML()).toContain('data-page-break-auto="true"');

    editor.destroy();
  });

  it("con preserveSelection mantiene el cursor en la continuidad del bloque derecho tras el split", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>abcdef</p>",
    });

    editor.commands.setTextSelection(6);

    const result = splitTextBlockAtPositionAndInsertPageBreak(
      editor as never,
      4,
      {
        auto: true,
      },
      {
        preserveSelection: true,
      },
    );

    expect(result).toBe(true);
    expect(editor.state.selection.from).toBeGreaterThan(4);
    expect(editor.state.selection.from).toBe(editor.state.selection.to);

    editor.destroy();
  });

  it("cuando el cursor coincide con el punto de corte continua en el bloque derecho", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>abcdef</p>",
    });

    editor.commands.setTextSelection(4);

    const result = splitTextBlockAtPositionAndInsertPageBreak(
      editor as never,
      4,
      {
        auto: true,
      },
      {
        preserveSelection: true,
      },
    );

    expect(result).toBe(true);
    expect(editor.state.selection.from).toBeGreaterThan(4);
    expect(editor.state.selection.$from.parent.textContent).toBe("def");
    expect(editor.state.selection.$from.parentOffset).toBe(0);

    editor.destroy();
  });

  it("mantiene el offset relativo dentro del bloque derecho cuando el cursor ya estaba en la continuacion", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>abcdef</p>",
    });

    editor.commands.setTextSelection(6);

    const result = splitTextBlockAtPositionAndInsertPageBreak(
      editor as never,
      4,
      {
        auto: true,
      },
      {
        preserveSelection: true,
      },
    );

    expect(result).toBe(true);
    expect(editor.state.selection.$from.parent.textContent).toBe("def");
    expect(editor.state.selection.$from.parentOffset).toBe(2);

    editor.destroy();
  });

  it("divide una lista top-level antes de un item y conserva ambas mitades", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<ul><li><p>uno</p></li><li><p>dos</p></li></ul>",
    });

    const result = splitListBlockBeforeItemAndInsertPageBreak(editor as never, 0, 8, {
      auto: true,
      mergeOnRemove: true,
    }, {
      preserveSelection: true,
    });

    expect(result).toBe(true);
    expect(editor.getJSON().content?.slice(0, 3).map((node) => node.type)).toEqual([
      "bulletList",
      "pageBreak",
      "bulletList",
    ]);

    const content = editor.getJSON().content ?? [];
    expect(content[0]?.content?.[0]?.content?.[0]?.content?.[0]?.text).toBe("uno");
    expect(content[2]?.content?.[0]?.content?.[0]?.content?.[0]?.text).toBe("dos");

    editor.destroy();
  });

  it("recompone una lista original al retirar un pageBreak automatico entre listas compatibles", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: {
        type: "doc",
        content: [
          {
            type: "bulletList",
            content: [
              {
                type: "listItem",
                content: [{ type: "paragraph", content: [{ type: "text", text: "uno" }] }],
              },
            ],
          },
          {
            type: "pageBreak",
            attrs: {
              auto: true,
              mergeOnRemove: true,
            },
          },
          {
            type: "bulletList",
            content: [
              {
                type: "listItem",
                content: [{ type: "paragraph", content: [{ type: "text", text: "dos" }] }],
              },
            ],
          },
        ],
      },
    });

    const result = removeAutoPageBreaks(editor as never);

    expect(result).toBe(true);
    expect(editor.getJSON().content?.[0]?.type).toBe("bulletList");
    expect(editor.getJSON().content?.[0]?.content?.map((node) => node.type)).toEqual([
      "listItem",
      "listItem",
    ]);
    expect(editor.getJSON().content?.[0]?.content?.[0]?.content?.[0]?.content?.[0]?.text).toBe("uno");
    expect(editor.getJSON().content?.[0]?.content?.[1]?.content?.[0]?.content?.[0]?.text).toBe("dos");

    editor.destroy();
  });

  it("retira solo pageBreaks automaticos desde una posicion invalida sin tocar los anteriores", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<p>uno</p><div data-page-break="true" data-page-break-auto="true"></div><p>dos</p><div data-page-break="true" data-page-break-auto="true"></div><p>tres</p>',
    });

    const result = removeAutoPageBreaks(editor as never, editor.state.doc.child(0).nodeSize + 1);

    expect(result).toBe(true);
    expect(editor.getJSON().content?.filter((node) => node.type === "pageBreak")).toHaveLength(1);
    expect(editor.getJSON().content?.[1]?.type).toBe("pageBreak");

    editor.destroy();
  });

  it("extiende la limpieza incremental para incluir el pageBreak automatico anterior al bloque editado", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<p>uno</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true"></div><p>dos</p>',
    });

    const cleanupPosition = resolveAutoPageBreakCleanupStartPosition(editor as never, 2, {
      includePreviousAutoBreak: true,
    });

    expect(cleanupPosition).toBe(editor.state.doc.child(0).nodeSize);

    editor.destroy();
  });

  it("mantiene la limpieza incremental en el bloque actual cuando no hay cambio estructural", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<p>uno</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true"></div><p>dos</p>',
    });

    const cleanupPosition = resolveAutoPageBreakCleanupStartPosition(editor as never, 2);

    expect(cleanupPosition).toBe(
      editor.state.doc.child(0).nodeSize + editor.state.doc.child(1).nodeSize,
    );

    editor.destroy();
  });
});
