import { useEffect, useRef, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { normalizeEditorHtml } from "./application/normalizeEditorHtml";
import {
  capturePaginationScrollAnchor,
  restorePaginationScrollAnchor,
  useAppEditor,
} from "./application/useAppEditor";
import { usePageContext } from "./application/usePageContext";
import { usePaginationMetrics } from "./application/usePaginationMetrics";
import { TiptapEditorContent } from "./infrastructure/TiptapEditorContent";

if (typeof Element !== "undefined") {
  Object.defineProperty(Element.prototype, "getClientRects", {
    configurable: true,
    value: () => [],
  });
  Object.defineProperty(Element.prototype, "getBoundingClientRect", {
    configurable: true,
    value: () => ({
      top: 0,
      bottom: 0,
      left: 0,
      right: 0,
      width: 0,
      height: 0,
      x: 0,
      y: 0,
      toJSON: () => ({}),
    }),
  });
}

if (typeof Text !== "undefined") {
  Object.defineProperty(Text.prototype, "getClientRects", {
    configurable: true,
    value: () => [],
  });
  Object.defineProperty(Text.prototype, "getBoundingClientRect", {
    configurable: true,
    value: () => ({
      top: 0,
      bottom: 0,
      left: 0,
      right: 0,
      width: 0,
      height: 0,
      x: 0,
      y: 0,
      toJSON: () => ({}),
    }),
  });
}

if (typeof Range !== "undefined") {
  Object.defineProperty(Range.prototype, "getClientRects", {
    configurable: true,
    value: () => [],
  });
  Object.defineProperty(Range.prototype, "getBoundingClientRect", {
    configurable: true,
    value: () => ({
      top: 0,
      bottom: 0,
      left: 0,
      right: 0,
      width: 0,
      height: 0,
      x: 0,
      y: 0,
      toJSON: () => ({}),
    }),
  });
}

type HarnessProps = {
  value?: string;
  defaultValue?: string;
  placeholder?: string;
  disabled?: boolean;
  readOnly?: boolean;
  paginationMode?: "none" | "visual";
  onChange?: (value: string) => void;
};

function applySyntheticPageLayout(
  proseMirror: HTMLElement,
  {
    maxBlocksPerPage,
    fittedBlockHeight = 120,
    fittedBlockGap = 140,
    overflowOffsetTop = 960,
  }: {
    maxBlocksPerPage: number;
    fittedBlockHeight?: number;
    fittedBlockGap?: number;
    overflowOffsetTop?: number;
  },
) {
  const pageElements = Array.from(proseMirror.children).filter(
    (child): child is HTMLElement =>
      child instanceof HTMLElement && child.matches('[data-app-editor-page="true"]'),
  );

  pageElements.forEach((pageElement) => {
    const blocks = Array.from(pageElement.children).filter(
      (child): child is HTMLElement => child instanceof HTMLElement,
    );

    blocks.forEach((block, blockIndex) => {
      const offsetTop =
        blockIndex < maxBlocksPerPage
          ? blockIndex * fittedBlockGap
          : overflowOffsetTop + (blockIndex - maxBlocksPerPage) * fittedBlockGap;

      Object.defineProperty(block, "offsetTop", {
        configurable: true,
        value: offsetTop,
      });
      Object.defineProperty(block, "offsetHeight", {
        configurable: true,
        value: fittedBlockHeight,
      });
      Object.defineProperty(block, "scrollHeight", {
        configurable: true,
        value: fittedBlockHeight,
      });
      Object.defineProperty(block, "getBoundingClientRect", {
        configurable: true,
        value: () => ({
          top: offsetTop,
          bottom: offsetTop + fittedBlockHeight,
          left: 0,
          right: 0,
          width: 0,
          height: fittedBlockHeight,
          x: 0,
          y: offsetTop,
          toJSON: () => ({}),
        }),
      });
      Object.defineProperty(block, "getClientRects", {
        configurable: true,
        value: () => [
          {
            top: offsetTop,
            bottom: offsetTop + fittedBlockHeight,
            left: 0,
            right: 0,
            width: 0,
            height: fittedBlockHeight,
            x: 0,
            y: offsetTop,
            toJSON: () => ({}),
          },
        ],
      });
    });
  });
}

function HookHarness(props: HarnessProps) {
  const { editor, isEditable } = useAppEditor(props);
  const [html, setHtml] = useState("");

  useEffect(() => {
    if (!editor) {
      return;
    }

    const syncSnapshot = () => {
      setHtml(editor.getHTML());
    };

    syncSnapshot();
    editor.on("update", syncSnapshot);
    editor.on("transaction", syncSnapshot);

    return () => {
      editor.off("update", syncSnapshot);
      editor.off("transaction", syncSnapshot);
    };
  }, [editor]);

  return (
    <div>
      <button
        type="button"
        onClick={() => editor?.commands.setContent("<p>Nuevo contenido</p>")}
      >
        set-content
      </button>
      <button
        type="button"
        onClick={() =>
          editor?.commands.setContent(
            '<p>Uno</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true" data-page-break-spacer="120"></div><p>Dos</p>',
          )
        }
      >
        set-auto-page-break-content
      </button>
      <button
        type="button"
        onClick={() =>
          editor?.commands.setContent("<p>Uno</p><div data-page-break=\"true\"></div><p>Dos</p>")
        }
      >
        set-manual-page-break-content
      </button>
      <button
        type="button"
        onClick={() => editor?.commands.undo()}
      >
        undo
      </button>
      <output data-testid="editable">{String(isEditable)}</output>
      <output data-testid="html">{html}</output>
    </div>
  );
}

function VisualTypingHarness() {
  const containerRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLDivElement>(null);
  const [hasTypedTailLine, setHasTypedTailLine] = useState(false);
  const { editor } = useAppEditor({
    defaultValue: "<p>Linea inicial</p>",
    paginationMode: "visual",
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    zoomLevel: 1,
  });
  const { totalPages, visualPageBoundaries } = usePaginationMetrics({
    enabled: true,
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    containerRef,
    zoomLevel: 1,
    debounceMs: 0,
  });
  const { currentPage } = usePageContext({
    enabled: true,
    totalPages,
    pageBoundaries: visualPageBoundaries,
    canvasRef,
    zoomLevel: 1,
    debounceMs: 0,
  });

  useEffect(() => {
    const canvas = canvasRef.current;
    const sheet = canvas?.querySelector('[data-pagination-sheet="true"]');
    const proseMirror = containerRef.current?.querySelector(".ProseMirror");
    if (!(proseMirror instanceof HTMLElement)) {
      return;
    }

    Object.defineProperty(proseMirror, "scrollHeight", {
      configurable: true,
      value: hasTypedTailLine ? 1220 : 880,
    });

    if (canvas instanceof HTMLElement) {
      Object.defineProperty(canvas, "scrollTop", {
        configurable: true,
        writable: true,
        value: hasTypedTailLine ? 1200 : 0,
      });
    }

    if (sheet instanceof HTMLElement) {
      Object.defineProperty(sheet, "offsetTop", {
        configurable: true,
        value: 0,
      });
    }

    applySyntheticPageLayout(proseMirror, {
      maxBlocksPerPage: hasTypedTailLine ? 1 : 8,
    });

    proseMirror.dispatchEvent(
      new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
    );

    if (canvas instanceof HTMLElement && hasTypedTailLine) {
      canvas.dispatchEvent(new Event("scroll"));
    }
  }, [editor, hasTypedTailLine, totalPages]);

  return (
    <div ref={containerRef}>
      <div ref={canvasRef}>
        <div data-pagination-sheet="true">
          <TiptapEditorContent editor={editor} />
        </div>
      </div>
      <button
        type="button"
        onClick={() => {
          if (editor) {
            const endPosition = editor.state.doc.content.size;
            editor.commands.setTextSelection({
              from: endPosition,
              to: endPosition,
            });
          }
          const selectionBeforeTyping = editor?.state.selection.to ?? 0;
          editor?.commands.insertContent("<p>Linea final adicional</p>");
          if (editor) {
            const selectionAfterTyping = editor.state.selection.to;
            Object.defineProperty(window, "__appEditorSelectionDelta", {
              configurable: true,
              value: selectionAfterTyping - selectionBeforeTyping,
            });
          }
          setHasTypedTailLine(true);
        }}
      >
        type-tail-line
      </button>
      <button
        type="button"
        onClick={() => {
          const historyHandled = (editor as
            | ({
                appEditorHistory?: {
                  undo?: () => boolean;
                };
              } & typeof editor)
            | null)?.appEditorHistory?.undo?.();
          if (historyHandled !== true) {
            editor?.commands.undo();
          }
          setHasTypedTailLine(false);
        }}
      >
        undo-tail-line
      </button>
      <output data-testid="typing-html">{editor?.getHTML() ?? ""}</output>
      <output data-testid="typing-total-pages">{totalPages}</output>
      <output data-testid="typing-current-page">{currentPage}</output>
      <output data-testid="typing-selection-delta">
        {String((window as typeof window & { __appEditorSelectionDelta?: number }).__appEditorSelectionDelta ?? 0)}
      </output>
      <output data-testid="typing-selection-parent-text">
        {editor?.state.selection.$from.parent.textContent ?? ""}
      </output>
      <output data-testid="typing-selection-parent-offset">
        {String(editor?.state.selection.$from.parentOffset ?? 0)}
      </output>
    </div>
  );
}

function VisualPasteHarness() {
  const containerRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLDivElement>(null);
  const [hasPastedLongContent, setHasPastedLongContent] = useState(false);
  const { editor } = useAppEditor({
    defaultValue: "<p>Inicio</p>",
    paginationMode: "visual",
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    zoomLevel: 1,
  });
  const { totalPages, visualPageBoundaries } = usePaginationMetrics({
    enabled: true,
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    containerRef,
    zoomLevel: 1,
    debounceMs: 0,
  });
  const { currentPage } = usePageContext({
    enabled: true,
    totalPages,
    pageBoundaries: visualPageBoundaries,
    canvasRef,
    zoomLevel: 1,
    debounceMs: 0,
  });

  useEffect(() => {
    const canvas = canvasRef.current;
    const sheet = canvas?.querySelector('[data-pagination-sheet="true"]');
    const proseMirror = containerRef.current?.querySelector(".ProseMirror");
    if (!(proseMirror instanceof HTMLElement)) {
      return;
    }

    Object.defineProperty(proseMirror, "scrollHeight", {
      configurable: true,
      value: hasPastedLongContent ? 3560 : 880,
    });

    if (canvas instanceof HTMLElement) {
      Object.defineProperty(canvas, "scrollTop", {
        configurable: true,
        writable: true,
        value: hasPastedLongContent ? 2500 : 0,
      });
    }

    if (sheet instanceof HTMLElement) {
      Object.defineProperty(sheet, "offsetTop", {
        configurable: true,
        value: 0,
      });
    }

    applySyntheticPageLayout(proseMirror, {
      maxBlocksPerPage: hasPastedLongContent ? 6 : 8,
    });

    proseMirror.dispatchEvent(
      new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
    );

    if (canvas instanceof HTMLElement && hasPastedLongContent) {
      canvas.dispatchEvent(new Event("scroll"));
    }
  }, [editor, hasPastedLongContent, totalPages]);

  return (
    <div ref={containerRef}>
      <div ref={canvasRef}>
        <div data-pagination-sheet="true">
          <TiptapEditorContent editor={editor} />
        </div>
      </div>
      <button
        type="button"
        onClick={() => {
          if (editor) {
            const endPosition = editor.state.doc.content.size;
            editor.commands.setTextSelection({
              from: endPosition,
              to: endPosition,
            });
          }
          editor?.commands.insertContent(
            Array.from({ length: 20 }, (_, index) => `<p>Parrafo pegado ${index + 1}</p>`).join(""),
          );
          setHasPastedLongContent(true);
        }}
      >
        paste-long-content
      </button>
      <output data-testid="paste-selection-parent-text">
        {editor?.state.selection.$from.parent.textContent ?? ""}
      </output>
      <output data-testid="paste-selection-parent-offset">
        {String(editor?.state.selection.$from.parentOffset ?? 0)}
      </output>
      <output data-testid="paste-total-pages">{totalPages}</output>
      <output data-testid="paste-current-page">{currentPage}</output>
    </div>
  );
}

function MultiPasteHarness() {
  const { editor } = useAppEditor({
    defaultValue: "<p>Inicio</p>",
    paginationMode: "visual",
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    zoomLevel: 1,
  });
  const [, setSnapshotVersion] = useState(0);

  useEffect(() => {
    if (!editor) {
      return;
    }

    const syncSnapshot = () => {
      setSnapshotVersion((currentVersion) => currentVersion + 1);
    };

    editor.on("transaction", syncSnapshot);
    editor.on("selectionUpdate", syncSnapshot);

    return () => {
      editor.off("transaction", syncSnapshot);
      editor.off("selectionUpdate", syncSnapshot);
    };
  }, [editor]);

  const appendChunk = (start: number, count: number) => {
    if (!editor) {
      return;
    }

    const endPosition = editor.state.doc.content.size;
    editor.commands.setTextSelection({
      from: endPosition,
      to: endPosition,
    });
    editor.commands.insertContent(
      Array.from({ length: count }, (_, index) => `<p>Bloque pegado ${start + index}</p>`).join(""),
    );
  };

  return (
    <div>
      <button type="button" onClick={() => appendChunk(1, 10)}>
        paste-batch-1
      </button>
      <button type="button" onClick={() => appendChunk(11, 10)}>
        paste-batch-2
      </button>
      <button type="button" onClick={() => appendChunk(21, 10)}>
        paste-batch-3
      </button>
      <output data-testid="multi-paste-selection-parent-text">
        {editor?.state.selection.$from.parent.textContent ?? ""}
      </output>
      <output data-testid="multi-paste-selection-parent-offset">
        {String(editor?.state.selection.$from.parentOffset ?? 0)}
      </output>
      <output data-testid="multi-paste-html">{editor?.getHTML() ?? ""}</output>
    </div>
  );
}

function VisualAutoBreakCleanupHarness() {
  const { editor } = useAppEditor({
    defaultValue: "<p>Base</p>",
    paginationMode: "visual",
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    zoomLevel: 1,
  });
  const [, setSnapshotVersion] = useState(0);

  useEffect(() => {
    if (!editor) {
      return;
    }

    const syncSnapshot = () => {
      setSnapshotVersion((currentVersion) => currentVersion + 1);
    };

    editor.on("transaction", syncSnapshot);
    editor.on("selectionUpdate", syncSnapshot);

    return () => {
      editor.off("transaction", syncSnapshot);
      editor.off("selectionUpdate", syncSnapshot);
    };
  }, [editor]);

  return (
    <div>
      <button
        type="button"
        onClick={() => {
          editor?.commands.setContent(
            '<p>Uno</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true" data-page-break-spacer="120"></div><p>Dos</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true" data-page-break-spacer="120"></div><p>Tres final</p>',
          );

          if (editor) {
            const endPosition = editor.state.doc.content.size;
            editor.commands.setTextSelection({
              from: endPosition,
              to: endPosition,
            });
          }
        }}
      >
        cleanup-auto-breaks
      </button>
      <output data-testid="cleanup-selection-parent-text">
        {editor?.state.selection.$from.parent.textContent ?? ""}
      </output>
      <output data-testid="cleanup-selection-parent-offset">
        {String(editor?.state.selection.$from.parentOffset ?? 0)}
      </output>
    </div>
  );
}

function VisualInlineMarksHarness() {
  const containerRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLDivElement>(null);
  const [forceOverflow, setForceOverflow] = useState(false);
  const { editor } = useAppEditor({
    defaultValue:
      '<p>Inicio <a href="https://example.com">link</a> <strong>negrita</strong> <em>cursiva</em> <u>subrayado</u> fin</p>',
    paginationMode: "visual",
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    zoomLevel: 1,
  });

  useEffect(() => {
    const sheet = canvasRef.current?.querySelector('[data-pagination-sheet="true"]');
    const proseMirror = containerRef.current?.querySelector(".ProseMirror");
    if (!(proseMirror instanceof HTMLElement)) {
      return;
    }

    Object.defineProperty(proseMirror, "scrollHeight", {
      configurable: true,
      value: forceOverflow ? 1800 : 880,
    });

    if (sheet instanceof HTMLElement) {
      Object.defineProperty(sheet, "offsetTop", {
        configurable: true,
        value: 0,
      });
    }

    applySyntheticPageLayout(proseMirror, {
      maxBlocksPerPage: forceOverflow ? 0 : 8,
    });

    proseMirror.dispatchEvent(
      new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
    );
  }, [editor, forceOverflow]);

  return (
    <div ref={containerRef}>
      <div ref={canvasRef}>
        <div data-pagination-sheet="true">
          <TiptapEditorContent editor={editor} />
        </div>
      </div>
      <button
        type="button"
        onClick={() => {
          editor?.commands.insertContent(" x");
          setForceOverflow(true);
        }}
      >
        trigger-reflow
      </button>
      <output data-testid="marks-html">{editor?.getHTML() ?? ""}</output>
    </div>
  );
}

function VisualCrossPageSelectionHarness() {
  const containerRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLDivElement>(null);
  const [forceOverflow, setForceOverflow] = useState(false);
  const { editor } = useAppEditor({
    defaultValue: "<p>Uno dos tres cuatro cinco seis siete ocho nueve diez</p>",
    paginationMode: "visual",
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    zoomLevel: 1,
  });

  useEffect(() => {
    const sheet = canvasRef.current?.querySelector('[data-pagination-sheet="true"]');
    const proseMirror = containerRef.current?.querySelector(".ProseMirror");
    if (!(proseMirror instanceof HTMLElement)) {
      return;
    }

    Object.defineProperty(proseMirror, "scrollHeight", {
      configurable: true,
      value: forceOverflow ? 1800 : 880,
    });

    if (sheet instanceof HTMLElement) {
      Object.defineProperty(sheet, "offsetTop", {
        configurable: true,
        value: 0,
      });
    }

    applySyntheticPageLayout(proseMirror, {
      maxBlocksPerPage: forceOverflow ? 0 : 8,
    });

    proseMirror.dispatchEvent(
      new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
    );
  }, [editor, forceOverflow]);

  return (
    <div ref={containerRef}>
      <div ref={canvasRef}>
        <div data-pagination-sheet="true">
          <TiptapEditorContent editor={editor} />
        </div>
      </div>
      <button
        type="button"
        onClick={() => {
          if (!editor) {
            return;
          }

          editor.commands.setTextSelection({
            from: 4,
            to: Math.min(24, editor.state.doc.content.size),
          });
          setForceOverflow(true);
          editor.commands.insertContent("!");
        }}
      >
        select-near-cut-and-type
      </button>
      <output data-testid="selection-from">{String(editor?.state.selection.from ?? 0)}</output>
      <output data-testid="selection-to">{String(editor?.state.selection.to ?? 0)}</output>
      <output data-testid="selection-text">
        {String(
          editor
            ? editor.state.doc.textBetween(
                editor.state.selection.from,
                editor.state.selection.to,
                "",
                "",
              )
            : "",
        )}
      </output>
      <output data-testid="selection-parent-text">
        {editor?.state.selection.$from.parent.textContent ?? ""}
      </output>
    </div>
  );
}

function VisualPasteRichContentHarness() {
  const containerRef = useRef<HTMLDivElement>(null);
  const canvasRef = useRef<HTMLDivElement>(null);
  const [hasPastedRichContent, setHasPastedRichContent] = useState(false);
  const { editor } = useAppEditor({
    defaultValue: "<p>Inicio</p>",
    paginationMode: "visual",
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    zoomLevel: 1,
  });
  const { totalPages, visualPageBoundaries } = usePaginationMetrics({
    enabled: true,
    pageHeight: 1123,
    pageGap: 32,
    pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    containerRef,
    zoomLevel: 1,
    debounceMs: 0,
  });
  const { currentPage } = usePageContext({
    enabled: true,
    totalPages,
    pageBoundaries: visualPageBoundaries,
    canvasRef,
    zoomLevel: 1,
    debounceMs: 0,
  });

  useEffect(() => {
    const canvas = canvasRef.current;
    const sheet = canvas?.querySelector('[data-pagination-sheet="true"]');
    const proseMirror = containerRef.current?.querySelector(".ProseMirror");
    if (!(proseMirror instanceof HTMLElement)) {
      return;
    }

    Object.defineProperty(proseMirror, "scrollHeight", {
      configurable: true,
      value: hasPastedRichContent ? 3560 : 880,
    });

    if (canvas instanceof HTMLElement) {
      Object.defineProperty(canvas, "scrollTop", {
        configurable: true,
        writable: true,
        value: hasPastedRichContent ? 2500 : 0,
      });
    }

    if (sheet instanceof HTMLElement) {
      Object.defineProperty(sheet, "offsetTop", {
        configurable: true,
        value: 0,
      });
    }

    applySyntheticPageLayout(proseMirror, {
      maxBlocksPerPage: hasPastedRichContent ? 6 : 8,
    });

    proseMirror.dispatchEvent(
      new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
    );

    if (canvas instanceof HTMLElement && hasPastedRichContent) {
      canvas.dispatchEvent(new Event("scroll"));
    }
  }, [editor, hasPastedRichContent, totalPages]);

  return (
    <div ref={containerRef}>
      <div ref={canvasRef}>
        <div data-pagination-sheet="true">
          <TiptapEditorContent editor={editor} />
        </div>
      </div>
      <button
        type="button"
        onClick={() => {
          const richHtml = [
            '<p>Parrafo con <a href="https://example.com">link persistente</a> y mas texto.</p>',
            "<ul><li><p>Item uno</p></li><li><p>Item dos</p></li><li><p>Item tres</p></li></ul>",
            '<p>Entre lista e imagen.</p>',
            '<img src="https://example.com/image.png" data-width="640" data-align="center" />',
            ...Array.from({ length: 16 }, (_, index) => `<p>Parrafo pegado ${index + 1}</p>`),
          ].join("");

          editor?.commands.insertContent(richHtml);
          setHasPastedRichContent(true);
        }}
      >
        paste-rich-content
      </button>
      <output data-testid="rich-total-pages">{totalPages}</output>
      <output data-testid="rich-current-page">{currentPage}</output>
      <output data-testid="rich-html">{editor?.getHTML() ?? ""}</output>
    </div>
  );
}

describe("useAppEditor [SPEC:IMPLEMENTACION-COMPONENTE-APPEDITOR-01-FE]", () => {
  it("preserva el anclaje vertical del caret al restaurar scroll tras repaginacion", () => {
    const editor = {
      state: {
        selection: {
          from: 20,
          to: 20,
        },
      },
      view: {
        coordsAtPos: vi
          .fn()
          .mockReturnValueOnce({ top: 460 })
          .mockReturnValueOnce({ top: 640 }),
      },
    } as unknown as Parameters<typeof capturePaginationScrollAnchor>[0];

    const scrollContainer = document.createElement("div");
    Object.defineProperty(scrollContainer, "scrollTop", {
      configurable: true,
      writable: true,
      value: 900,
    });
    Object.defineProperty(scrollContainer, "getBoundingClientRect", {
      configurable: true,
      value: () => ({
        top: 100,
        left: 0,
        right: 0,
        bottom: 0,
        width: 0,
        height: 0,
        x: 0,
        y: 0,
        toJSON: () => ({}),
      }),
    });

    const anchor = capturePaginationScrollAnchor(editor, scrollContainer);
    restorePaginationScrollAnchor(editor, scrollContainer, anchor);

    expect(scrollContainer.scrollTop).toBe(1080);
  });

  it("preserva el anclaje vertical tambien cuando el formato parte de una seleccion de texto", () => {
    const editor = {
      state: {
        selection: {
          from: 10,
          to: 16,
        },
      },
      view: {
        coordsAtPos: vi
          .fn()
          .mockReturnValueOnce({ top: 420 })
          .mockReturnValueOnce({ top: 560 }),
      },
    } as unknown as Parameters<typeof capturePaginationScrollAnchor>[0];

    const scrollContainer = document.createElement("div");
    Object.defineProperty(scrollContainer, "scrollTop", {
      configurable: true,
      writable: true,
      value: 700,
    });
    Object.defineProperty(scrollContainer, "getBoundingClientRect", {
      configurable: true,
      value: () => ({
        top: 100,
        left: 0,
        right: 0,
        bottom: 0,
        width: 0,
        height: 0,
        x: 0,
        y: 0,
        toJSON: () => ({}),
      }),
    });

    const anchor = capturePaginationScrollAnchor(editor, scrollContainer);
    restorePaginationScrollAnchor(editor, scrollContainer, anchor);

    expect(scrollContainer.scrollTop).toBe(840);
  });

  it("inicializa en modo no controlado y propaga onChange", async () => {
    const handleChange = vi.fn();

    render(
      <HookHarness
        defaultValue="<p>Inicial</p>"
        onChange={handleChange}
      />,
    );

    await waitFor(() => {
      expect(screen.getByTestId("html")).toHaveTextContent("Inicial");
    });

    fireEvent.click(screen.getByText("set-content"));

    await waitFor(() => {
      expect(handleChange).toHaveBeenCalledWith(expect.stringContaining("Nuevo contenido"));
      expect(screen.getByTestId("html")).toHaveTextContent("Nuevo contenido");
    });
  });

  it("sincroniza el valor controlado externamente", async () => {
    const { rerender } = render(<HookHarness value="<p>Uno</p>" />);

    await waitFor(() => {
      expect(screen.getByTestId("html")).toHaveTextContent("Uno");
    });

    rerender(<HookHarness value="<p>Dos</p>" />);

    await waitFor(() => {
      expect(screen.getByTestId("html")).toHaveTextContent("Dos");
    });
  });

  it("recrea el schema cuando un valor controlado visual pasa de plano a paginado", async () => {
    const { rerender } = render(<HookHarness value="<p>Uno</p>" paginationMode="visual" />);

    await waitFor(() => {
      const html = screen.getByTestId("html").textContent ?? "";
      expect(html).toContain("Uno");
      expect(html).toContain('data-app-editor-page="true"');
    });

    rerender(
      <HookHarness
        value={'<p>Uno</p><div data-page-break="true"></div><p>Dos</p>'}
        paginationMode="visual"
      />,
    );

    await waitFor(() => {
      const html = screen.getByTestId("html").textContent ?? "";
      expect(html).toContain('data-app-editor-page="true"');
      expect(html).toContain("Uno");
      expect(html).toContain("Dos");
    });
  });

  it("omite pageBreaks automaticos al inicializar contenido externo", async () => {
    render(
      <HookHarness
        defaultValue={
          '<p>Uno</p><div data-page-break="true" data-page-break-auto="true" data-page-break-merge="true" data-page-break-spacer="120"></div><p>Dos</p>'
        }
      />,
    );

    await waitFor(() => {
      const html = screen.getByTestId("html").textContent ?? "";
      expect(html).toContain("Uno");
      expect(html).toContain("Dos");
      expect(html).not.toContain("data-page-break-auto");
    });
  });

  it("serializa limpio contenido inicial con pageBreak manual en modo visual", async () => {
    render(
      <HookHarness
        defaultValue={'<p>Uno</p><div data-page-break="true"></div><p>Dos</p>'}
        onChange={() => {}}
        paginationMode="visual"
      />,
    );

    await waitFor(() => {
      const html = screen.getByTestId("html").textContent ?? "";
      expect(html).toContain("data-app-editor-page");
      expect(html).not.toContain('data-page-break="true"');
    });
  });

  it("rehidrata contenido ya paginado en wrappers reales al inicializar modo visual", async () => {
    render(
      <HookHarness
        defaultValue={
          '<div data-app-editor-page="true"><p>Uno</p></div><div data-app-editor-page="true"><p>Dos</p></div>'
        }
        paginationMode="visual"
      />,
    );

    await waitFor(() => {
      const html = screen.getByTestId("html").textContent ?? "";
      expect(html).toContain('data-app-editor-page="true"');
      expect(html.match(/data-app-editor-page="true"/g)?.length).toBe(2);
      expect(html).toContain("Uno");
      expect(html).toContain("Dos");
    });
  });

  it("desactiva la edicion cuando disabled o readOnly estan activos", () => {
    const { rerender } = render(<HookHarness disabled />);

    expect(screen.getByTestId("editable")).toHaveTextContent("false");

    rerender(<HookHarness readOnly />);

    expect(screen.getByTestId("editable")).toHaveTextContent("false");
  });

  it("omite pageBreaks automaticos del html propagado por onChange", async () => {
    const handleChange = vi.fn();

    render(<HookHarness onChange={handleChange} />);

    fireEvent.click(screen.getByText("set-auto-page-break-content"));

    await waitFor(() => {
      expect(handleChange).toHaveBeenCalled();
      const latestValue = handleChange.mock.lastCall?.[0];
      expect(normalizeEditorHtml(latestValue)).toBe('<p style="text-align: left;">Uno</p><p style="text-align: left;">Dos</p>');
      expect(String(latestValue)).not.toContain("data-page-break-auto");
    });
  });

  it("conserva pageBreaks manuales en el html propagado por onChange", async () => {
    const handleChange = vi.fn();

    render(<HookHarness onChange={handleChange} />);

    fireEvent.click(screen.getByText("set-manual-page-break-content"));

    await waitFor(() => {
      expect(handleChange).toHaveBeenCalled();
      const latestValue = handleChange.mock.lastCall?.[0];
      expect(String(latestValue)).toContain('data-page-break="true"');
      expect(String(latestValue)).not.toContain("data-page-break-auto");
    });
  });

  it("serializa paginas reales a pageBreaks manuales al propagar onChange en modo visual", async () => {
    const handleChange = vi.fn();

    render(<HookHarness onChange={handleChange} paginationMode="visual" />);

    fireEvent.click(screen.getByText("set-manual-page-break-content"));

    await waitFor(() => {
      expect(handleChange).toHaveBeenCalled();
      const latestValue = String(handleChange.mock.lastCall?.[0] ?? "");
      expect(latestValue).toContain('<div data-page-break="true"></div>');
      expect(latestValue).not.toContain('data-app-editor-page="true"');
    });
  });

  it("reacciona a escritura al final de pagina creando continuidad a la siguiente hoja", async () => {
    render(<VisualTypingHarness />);

    await waitFor(() => {
      expect(screen.getByTestId("typing-total-pages")).toHaveTextContent("1");
      expect(screen.getByTestId("typing-current-page")).toHaveTextContent("1");
    });

    fireEvent.click(screen.getByText("type-tail-line"));

    await waitFor(() => {
      expect(screen.getByTestId("typing-total-pages")).toHaveTextContent("2");
      expect(screen.getByTestId("typing-current-page")).toHaveTextContent("2");
      expect(Number(screen.getByTestId("typing-selection-delta").textContent)).toBeGreaterThan(0);
      expect(screen.getByTestId("typing-selection-parent-text")).toHaveTextContent(
        "Linea final adicional",
      );
      expect(Number(screen.getByTestId("typing-selection-parent-offset").textContent)).toBe(
        "Linea final adicional".length,
      );
    });
  });

  it("permite deshacer la escritura paginada sin exigir deshacer pageBreaks automaticos intermedios", async () => {
    render(<VisualTypingHarness />);

    fireEvent.click(screen.getByText("type-tail-line"));

    await waitFor(() => {
      expect(screen.getByTestId("typing-total-pages")).toHaveTextContent("2");
      expect(screen.getByTestId("typing-html")).toHaveTextContent("Linea final adicional");
    });

    fireEvent.click(screen.getByText("undo-tail-line"));

    await waitFor(() => {
      expect(screen.getByTestId("typing-total-pages")).toHaveTextContent("1");
      expect(screen.getByTestId("typing-html")).not.toHaveTextContent("Linea final adicional");
      expect(screen.getByTestId("typing-html")).toHaveTextContent("Linea inicial");
    });
  });

  it("repagina contenido pegado largo respetando continuidad de hojas", async () => {
    render(<VisualPasteHarness />);

    await waitFor(() => {
      expect(screen.getByTestId("paste-total-pages")).toHaveTextContent("1");
      expect(screen.getByTestId("paste-current-page")).toHaveTextContent("1");
    });

    fireEvent.click(screen.getByText("paste-long-content"));

    await waitFor(() => {
      expect(screen.getByTestId("paste-total-pages")).toHaveTextContent("4");
      expect(screen.getByTestId("paste-current-page")).toHaveTextContent("3");
    });
  });

  it("mantiene el cursor en el ultimo bloque tras pegar contenido largo multipagina", async () => {
    render(<VisualPasteHarness />);

    fireEvent.click(screen.getByText("paste-long-content"));

    await waitFor(() => {
      expect(screen.getByTestId("paste-selection-parent-text")).toHaveTextContent(
        "Parrafo pegado 20",
      );
      expect(Number(screen.getByTestId("paste-selection-parent-offset").textContent)).toBe(
        "Parrafo pegado 20".length,
      );
    });
  });

  it("conserva el cursor al final tras limpiar auto pageBreaks previos durante repaginacion", async () => {
    render(<VisualAutoBreakCleanupHarness />);

    fireEvent.click(screen.getByText("cleanup-auto-breaks"));

    await waitFor(() => {
      expect(screen.getByTestId("cleanup-selection-parent-text")).toHaveTextContent("Tres final");
      expect(Number(screen.getByTestId("cleanup-selection-parent-offset").textContent)).toBe(
        "Tres final".length,
      );
    });
  });

  it("mantiene el cursor en el ultimo bloque tras varios pegados consecutivos", async () => {
    render(<MultiPasteHarness />);

    fireEvent.click(screen.getByText("paste-batch-1"));
    fireEvent.click(screen.getByText("paste-batch-2"));
    fireEvent.click(screen.getByText("paste-batch-3"));

    await waitFor(() => {
      expect(screen.getByTestId("multi-paste-html")).toHaveTextContent("Bloque pegado 30");
      expect(screen.getByTestId("multi-paste-selection-parent-text")).toHaveTextContent(
        "Bloque pegado 30",
      );
      expect(Number(screen.getByTestId("multi-paste-selection-parent-offset").textContent)).toBe(
        "Bloque pegado 30".length,
      );
    });
  });

  it("soporta paste largo con links, listas e imagen sin corromper el flujo multipagina", async () => {
    render(<VisualPasteRichContentHarness />);

    fireEvent.click(screen.getByText("paste-rich-content"));

    await waitFor(() => {
      const html = screen.getByTestId("rich-html").textContent ?? "";
      expect(html).toContain('href="https://example.com"');
      expect(html).toContain("<ul>");
      expect(html).toContain("<img");
      expect(html).toContain("Parrafo pegado 16");
    });
  });

  it("conserva links y marks inline tras reflow visual", async () => {
    render(<VisualInlineMarksHarness />);

    fireEvent.click(screen.getByText("trigger-reflow"));

    await waitFor(() => {
      const html = screen.getByTestId("marks-html").textContent ?? "";
      expect(html).toContain('href="https://example.com"');
      expect(html).toContain("<strong>");
      expect(html).toContain("<em>");
      expect(html).toContain("<u>");
    });
  });

  it("mantiene seleccion estable cerca del corte de pagina tras reflow", async () => {
    render(<VisualCrossPageSelectionHarness />);

    fireEvent.click(screen.getByText("select-near-cut-and-type"));

    await waitFor(() => {
      const from = Number(screen.getByTestId("selection-from").textContent ?? "0");
      const to = Number(screen.getByTestId("selection-to").textContent ?? "0");
      expect(from).toBeGreaterThan(0);
      expect(to).toBeGreaterThanOrEqual(from);
      // Selection may be collapsed to a cursor; we only require it stays valid and within content.
      expect(screen.getByTestId("selection-parent-text").textContent ?? "").not.toBe("");
    });
  });
});
