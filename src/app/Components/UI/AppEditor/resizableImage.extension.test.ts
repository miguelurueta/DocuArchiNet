import { Editor } from "@tiptap/core";
import { describe, expect, it } from "vitest";
import { buildAppEditorExtensions } from "./infrastructure/tiptap.extensions";

describe("ResizableImage extension [SPEC:AJUSTE-DESPLAZAMIENTO-IMAGEN-APPEDITOR-GESTIONCORRESPONDENCIA-11-FE]", () => {
  it("serializa y rehidrata data-align junto con data-width", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<p>Intro</p><img src="https://cdn.example.com/image.png" data-width="75%" data-align="center" />',
    });

    const html = editor.getHTML();

    expect(html).toContain('data-align="center"');
    expect(html).toContain('data-width="75%"');
    expect(editor.getJSON().content?.[1]?.attrs).toMatchObject({
      align: "center",
      width: "75%",
    });

    editor.destroy();
  });

  it("setImageAlign actualiza la imagen activa sin perder width", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(),
      content:
        '<img src="https://cdn.example.com/image.png" data-width="50%" data-align="left" />',
    });

    editor.commands.selectNodeForward();
    editor.commands.setImageAlign("right");

    expect(editor.getHTML()).toContain('data-align="right"');
    expect(editor.getHTML()).toContain('data-width="50%"');

    editor.destroy();
  });
});
