import { useEffect, useRef, useState } from "react";
import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { normalizeEditorHtml } from "./application/normalizeEditorHtml";
import { useAppEditor } from "./application/useAppEditor";
import { usePageContext } from "./application/usePageContext";
import { usePaginationMetrics } from "./application/usePaginationMetrics";
import { TiptapEditorContent } from "./infrastructure/TiptapEditorContent";

type HarnessProps = {
  value?: string;
  defaultValue?: string;
  placeholder?: string;
  disabled?: boolean;
  readOnly?: boolean;
  onChange?: (value: string) => void;
};

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

    proseMirror.dispatchEvent(
      new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
    );

    if (canvas instanceof HTMLElement && hasTypedTailLine) {
      canvas.dispatchEvent(new Event("scroll"));
    }
  }, [editor, hasTypedTailLine]);

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
      <output data-testid="typing-total-pages">{totalPages}</output>
      <output data-testid="typing-current-page">{currentPage}</output>
      <output data-testid="typing-selection-delta">
        {String((window as typeof window & { __appEditorSelectionDelta?: number }).__appEditorSelectionDelta ?? 0)}
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

    proseMirror.dispatchEvent(
      new CustomEvent("app-editor-pagination-updated", { bubbles: true }),
    );

    if (canvas instanceof HTMLElement && hasPastedLongContent) {
      canvas.dispatchEvent(new Event("scroll"));
    }
  }, [editor, hasPastedLongContent]);

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
          editor?.commands.insertContent(
            Array.from({ length: 20 }, (_, index) => `<p>Parrafo pegado ${index + 1}</p>`).join(""),
          );
          setHasPastedLongContent(true);
        }}
      >
        paste-long-content
      </button>
      <output data-testid="paste-total-pages">{totalPages}</output>
      <output data-testid="paste-current-page">{currentPage}</output>
    </div>
  );
}

describe("useAppEditor [SPEC:IMPLEMENTACION-COMPONENTE-APPEDITOR-01-FE]", () => {
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
});
