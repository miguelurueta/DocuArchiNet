import { Editor } from "@tiptap/core";
import { describe, expect, it } from "vitest";
import { buildAppEditorExtensions } from "./infrastructure/tiptap.extensions";
import {
  insertPageBreakBeforeBlock,
  splitBlockAndInsertPageBreak,
  splitTextBlockAtPositionAndInsertPageBreak,
} from "./application/autoPageBreak";
import { removeAutoPageBreaks } from "./application/autoPagination";

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
});
