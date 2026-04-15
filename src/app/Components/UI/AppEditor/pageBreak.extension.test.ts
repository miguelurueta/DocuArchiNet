import { Editor } from "@tiptap/core";
import { describe, expect, it } from "vitest";
import { buildAppEditorExtensions } from "./infrastructure/tiptap.extensions";

describe("PageBreak extension [SPEC:IMPLEMENTACION-PAGINACION-APPEDITOR-09-FE]", () => {
  it("inserta un salto manual persistido y evita duplicados consecutivos", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: "<p>Uno</p><p>Dos</p>",
    });

    editor.commands.insertPageBreak();
    expect(editor.getHTML()).toContain('<div data-page-break="true"></div>');

    editor.commands.insertPageBreak();
    expect(editor.getJSON().content?.filter((node) => node.type === "pageBreak")).toHaveLength(1);

    editor.destroy();
  });

  it("rehidrata pageBreak desde HTML persistido", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content: '<p>Inicio</p><div data-page-break="true"></div><p>Fin</p>',
    });

    expect(editor.getJSON().content?.map((node) => node.type)).toContain("pageBreak");
    expect(editor.getHTML()).toContain('<div data-page-break="true"></div>');

    editor.destroy();
  });
});
