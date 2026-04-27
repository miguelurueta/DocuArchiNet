import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { AppEditorToolbar } from "./presentation/AppEditorToolbar";

function createChainMock() {
  const chain: Record<string, ReturnType<typeof vi.fn> | unknown> = {
    focus: vi.fn(() => chain),
    setTextSelection: vi.fn(() => chain),
    toggleBold: vi.fn(() => chain),
    toggleItalic: vi.fn(() => chain),
    toggleUnderline: vi.fn(() => chain),
    toggleBulletList: vi.fn(() => chain),
    toggleOrderedList: vi.fn(() => chain),
    toggleTaskList: vi.fn(() => chain),
    setTextAlign: vi.fn(() => chain),
    undo: vi.fn(() => chain),
    redo: vi.fn(() => chain),
    extendMarkRange: vi.fn(() => chain),
    setLink: vi.fn(() => chain),
    unsetLink: vi.fn(() => chain),
    setImage: vi.fn(() => chain),
    updateAttributes: vi.fn(() => chain),
    setImageAlign: vi.fn(() => chain),
    toggleHeading: vi.fn(() => chain),
    setParagraph: vi.fn(() => chain),
    run: vi.fn(() => true),
  };

  return chain as {
    focus: ReturnType<typeof vi.fn>;
    setTextSelection: ReturnType<typeof vi.fn>;
    toggleBold: ReturnType<typeof vi.fn>;
    toggleItalic: ReturnType<typeof vi.fn>;
    toggleUnderline: ReturnType<typeof vi.fn>;
    toggleBulletList: ReturnType<typeof vi.fn>;
    toggleOrderedList: ReturnType<typeof vi.fn>;
    toggleTaskList: ReturnType<typeof vi.fn>;
    setTextAlign: ReturnType<typeof vi.fn>;
    undo: ReturnType<typeof vi.fn>;
    redo: ReturnType<typeof vi.fn>;
    extendMarkRange: ReturnType<typeof vi.fn>;
    setLink: ReturnType<typeof vi.fn>;
    unsetLink: ReturnType<typeof vi.fn>;
    setImage: ReturnType<typeof vi.fn>;
    updateAttributes: ReturnType<typeof vi.fn>;
    setImageAlign: ReturnType<typeof vi.fn>;
    toggleHeading: ReturnType<typeof vi.fn>;
    setParagraph: ReturnType<typeof vi.fn>;
    run: ReturnType<typeof vi.fn>;
  };
}

function createEditorMock() {
  const actionChain = createChainMock();
  const canChain = createChainMock();

  return {
    isFocused: false,
    state: {
      doc: {
        content: {
          size: 100,
        },
        descendants: vi.fn(),
      },
      selection: {
        from: 3,
        to: 8,
        node: null,
      },
    },
    isActive: vi.fn((name: unknown) => Boolean(name === "bold")),
    can: vi.fn(() => ({
      chain: vi.fn(() => canChain),
    })),
    chain: vi.fn(() => actionChain),
    getAttributes: vi.fn((name: unknown) => {
      if (name === "image") {
        return { width: "50%", align: "left" };
      }

      return { href: "https://openai.com" };
    }),
    __actionChain: actionChain,
  };
}

describe("AppEditorToolbar [SPEC:IMPLEMENTACION-COMPONENTE-APPEDITOR-01-FE]", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("renderiza controles deshabilitados cuando no hay editor", () => {
    render(<AppEditorToolbar editor={null} disabled />);

    expect(screen.getByRole("toolbar")).toBeInTheDocument();
    expect(screen.getByLabelText("Negrita")).toBeDisabled();
    expect(screen.getByLabelText("Insertar imagen")).toBeDisabled();
    expect(screen.getByLabelText("Deshacer")).toBeDisabled();
    expect(screen.getByLabelText("Rehacer")).toBeDisabled();
    expect(screen.queryByRole("button", { name: /Tema .* activo/i })).not.toBeInTheDocument();
    expect(screen.getByRole("group", { name: "Formato de texto" })).toBeInTheDocument();
    expect(screen.getByRole("group", { name: "Insercion de contenido" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Nivel de encabezado" })).toBeInTheDocument();
  });

  it("ejecuta comandos de formato basico", () => {
    const editor = createEditorMock();

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.click(screen.getByLabelText("Negrita"));
    fireEvent.click(screen.getByLabelText("Deshacer"));

    expect(editor.__actionChain.toggleBold).toHaveBeenCalledTimes(1);
    expect(editor.__actionChain.undo).toHaveBeenCalledTimes(1);
    expect(editor.__actionChain.run).toHaveBeenCalled();
  });

  it("mantiene la seleccion para aplicar formato combinado consecutivo", () => {
    const editor = createEditorMock();

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.mouseDown(screen.getByLabelText("Negrita"));
    fireEvent.click(screen.getByLabelText("Negrita"));
    editor.state.selection = {
      from: 3,
      to: 3,
      node: null,
    };

    fireEvent.mouseDown(screen.getByLabelText("Cursiva"));
    fireEvent.click(screen.getByLabelText("Cursiva"));

    expect(editor.__actionChain.setTextSelection).toHaveBeenCalledWith({ from: 3, to: 8 });
    expect(editor.__actionChain.toggleBold).toHaveBeenCalledTimes(1);
    expect(editor.__actionChain.toggleItalic).toHaveBeenCalledTimes(1);
  });

  it("renderiza acciones custom junto a la seccion de insercion", () => {
    const editor = createEditorMock();

    render(
      <AppEditorToolbar
        editor={editor as never}
        toolbarActions={<button type="button" aria-label="Guardar en toolbar" />}
      />,
    );

    expect(screen.getByRole("button", { name: "Guardar en toolbar" })).toBeInTheDocument();
    expect(screen.getByRole("group", { name: "Acciones del editor" })).toBeInTheDocument();
  });

  it("agrupa la alineacion de texto en un dropdown compacto", async () => {
    const editor = createEditorMock();

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.click(screen.getByRole("button", { name: "Alineacion de texto" }));
    fireEvent.click(screen.getByText("Derecha"));

    await waitFor(() => {
      expect(editor.__actionChain.setTextAlign).toHaveBeenCalledWith("right");
    });
  });

  it("compacta la toolbar y colapsa estructura en tablet/mobile", async () => {
    const previousWidth = window.innerWidth;
    Object.defineProperty(window, "innerWidth", {
      configurable: true,
      writable: true,
      value: 896,
    });

    const editor = createEditorMock();
    const { container } = render(<AppEditorToolbar editor={editor as never} />);

    fireEvent(window, new Event("resize"));

    expect(container.querySelector('[data-toolbar-mode="compact"]')).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Estructura de contenido" })).toBeInTheDocument();
    fireEvent.click(screen.getByRole("button", { name: "Estructura de contenido" }));
    fireEvent.click(await screen.findByText("Lista numerada"));

    await waitFor(() => {
      expect(editor.__actionChain.toggleOrderedList).toHaveBeenCalledTimes(1);
    });

    Object.defineProperty(window, "innerWidth", {
      configurable: true,
      writable: true,
      value: previousWidth,
    });
    fireEvent(window, new Event("resize"));
  });

  it("compacta la toolbar segun el ancho real del contenedor (ResizeObserver)", async () => {
    const previousResizeObserver = window.ResizeObserver;
    const triggers: Array<(width: number) => void> = [];

    window.ResizeObserver = class ResizeObserverMock {
      private readonly cb: ResizeObserverCallback;

      constructor(cb: ResizeObserverCallback) {
        this.cb = cb;
        triggers.push((width: number) => {
          this.cb(
            [
              {
                contentRect: { width } as DOMRectReadOnly,
              } as ResizeObserverEntry,
            ],
            this as unknown as ResizeObserver,
          );
        });
      }

      observe() {}
      unobserve() {}
      disconnect() {}
    } as unknown as typeof ResizeObserver;

    const editor = createEditorMock();
    const { container } = render(<AppEditorToolbar editor={editor as never} />);

    // Starts in default mode (window-based), then container observation flips it.
    expect(container.querySelector('[data-toolbar-mode="default"]')).toBeInTheDocument();
    triggers[0]?.(480);

    await waitFor(() => {
      expect(container.querySelector('[data-toolbar-mode="compact"]')).toBeInTheDocument();
    });

    window.ResizeObserver = previousResizeObserver;
  });

  it("abre el formulario de enlace y aplica la URL normalizada", async () => {
    const editor = createEditorMock();

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.click(screen.getByLabelText("Insertar enlace"));
    fireEvent.change(await screen.findByLabelText("URL del enlace"), {
      target: { value: "docs.openai.com" },
    });
    fireEvent.click(screen.getByRole("button", { name: /^Aplicar$/ }));

    await waitFor(() => {
      expect(editor.__actionChain.setLink).toHaveBeenCalledWith({
        href: "https://docs.openai.com",
      });
    });
  });

  it("abre el formulario de imagen y aplica insercion con ancho persistido", async () => {
    const editor = createEditorMock();

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.click(screen.getByLabelText("Insertar imagen"));
    fireEvent.change(await screen.findByLabelText("URL de la imagen"), {
      target: { value: "cdn.example.com/image.png" },
    });
    fireEvent.click(screen.getByRole("button", { name: "75%" }));
    fireEvent.click(screen.getByRole("button", { name: /^Insertar$/ }));

    await waitFor(() => {
      expect(editor.__actionChain.setImage).toHaveBeenCalledWith(
        expect.objectContaining({
          src: "https://cdn.example.com/image.png",
        }),
      );
      expect(editor.__actionChain.updateAttributes).toHaveBeenCalledWith("image", {
        width: "75%",
      });
    });
  });

  it("permite aplicar tamano persistido a una imagen seleccionada", async () => {
    const editor = createEditorMock();
    editor.isActive = vi.fn((name: unknown) => name === "image");

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.click(screen.getByLabelText("Insertar imagen"));
    fireEvent.click(screen.getByRole("button", { name: "100%" }));
    fireEvent.click(screen.getByRole("button", { name: "Aplicar tamaño" }));

    await waitFor(() => {
      expect(editor.__actionChain.updateAttributes).toHaveBeenCalledWith("image", {
        width: "100%",
      });
    });
  });

  it("unifica la alineacion de imagen dentro del popover de imagen", async () => {
    const editor = createEditorMock();
    editor.isActive = vi.fn((name: unknown) => name === "image");

    render(<AppEditorToolbar editor={editor as never} />);

    expect(screen.queryByRole("button", { name: "Alineacion de imagen" })).not.toBeInTheDocument();

    fireEvent.click(screen.getByLabelText("Insertar imagen"));
    fireEvent.click(await screen.findByRole("button", { name: "Centro" }));

    await waitFor(() => {
      expect(editor.__actionChain.setImageAlign).toHaveBeenCalledWith("center");
    });
  });

  it("cubre contrato FE-11: alinear imagen horizontalmente sin perder atributos", async () => {
    const editor = createEditorMock();
    editor.isActive = vi.fn((name: unknown) => name === "image");

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.click(screen.getByLabelText("Insertar imagen"));
    fireEvent.click(await screen.findByRole("button", { name: "Derecha" }));

    await waitFor(() => {
      expect(editor.__actionChain.setImageAlign).toHaveBeenCalledWith("right");
    });
  });

  it("delegates local image insertion to the provided handler", async () => {
    const editor = createEditorMock();
    const handleInsertLocalImage = vi.fn(() => Promise.resolve());
    render(<AppEditorToolbar editor={editor as never} onInsertLocalImage={handleInsertLocalImage} />);

    fireEvent.click(screen.getByLabelText("Insertar imagen"));

    const input = document.querySelector('input[type="file"]') as HTMLInputElement;
    const file = new File(["img"], "logo.png", { type: "image/png" });
    fireEvent.change(input, {
      target: { files: [file] },
    });

    await waitFor(() => {
      expect(handleInsertLocalImage).toHaveBeenCalledWith(file, "50%");
    });
  });
});
