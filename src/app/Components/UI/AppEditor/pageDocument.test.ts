import { describe, expect, it } from "vitest";
import {
  hasManualPageBreaks,
  hasVisualPageWrappers,
  serializeVisualPageHtml,
  splitHtmlByManualPageBreaks,
  unwrapVisualPageHtml,
  wrapHtmlInVisualPages,
} from "./application/pageDocument";
import { Editor } from "@tiptap/core";
import { buildAppEditorExtensions } from "./infrastructure/tiptap.extensions";

describe("pageDocument helpers", () => {
  it("divide html plano por pageBreaks manuales", () => {
    expect(
      splitHtmlByManualPageBreaks(
        '<p>Uno</p><div data-page-break="true"></div><p>Dos</p><div data-page-break="true"></div><p>Tres</p>',
      ),
    ).toEqual(["<p>Uno</p>", "<p>Dos</p>", "<p>Tres</p>"]);
  });

  it("envuelve contenido en paginas visuales reales a partir de segmentos", () => {
    expect(
      wrapHtmlInVisualPages('<p>Uno</p><div data-page-break="true"></div><p>Dos</p>'),
    ).toBe(
      '<div data-app-editor-page="true"><p>Uno</p></div><div data-app-editor-page="true"><p>Dos</p></div>',
    );
  });

  it("detecta pageBreaks manuales como candidato de migracion a paginas reales", () => {
    expect(
      hasManualPageBreaks('<p>Uno</p><div data-page-break="true"></div><p>Dos</p>'),
    ).toBe(true);
  });

  it("puede remover wrappers de paginas visuales para serializacion externa", () => {
    const wrapped =
      '<div data-app-editor-page="true"><p>Uno</p></div><div data-app-editor-page="true"><p>Dos</p></div>';

    expect(hasVisualPageWrappers(wrapped)).toBe(true);
    expect(unwrapVisualPageHtml(wrapped)).toBe("<p>Uno</p><p>Dos</p>");
  });

  it("serializa paginas reales a pageBreaks manuales para roundtrip persistente", () => {
    const wrapped =
      '<div data-app-editor-page="true"><p>Uno</p></div><div data-app-editor-page="true"><p>Dos</p></div>';

    expect(serializeVisualPageHtml(wrapped)).toBe(
      '<p>Uno</p><div data-page-break="true"></div><p>Dos</p>',
    );
  });

  it("detecta pageBreaks manuales de forma estable en llamadas repetidas", () => {
    const value = '<p>Uno</p><div data-page-break="true"></div><p>Dos</p>';

    expect(hasManualPageBreaks(value)).toBe(true);
    expect(hasManualPageBreaks(value)).toBe(true);
  });
});

describe("paginated document schema", () => {
  it("permite parsear paginas reales cuando se activa el schema paginado", () => {
    const editor = new Editor({
      extensions: buildAppEditorExtensions(undefined, { paginatedDocument: true }),
      content:
        '<div data-app-editor-page="true"><p>Uno</p></div><div data-app-editor-page="true"><p>Dos</p></div>',
    });

    expect(editor.getJSON().content?.map((node) => node.type)).toEqual(["page", "page"]);
    expect(editor.getHTML()).toContain('data-app-editor-page="true"');

    editor.destroy();
  });
});
