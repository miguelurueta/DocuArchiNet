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
});
