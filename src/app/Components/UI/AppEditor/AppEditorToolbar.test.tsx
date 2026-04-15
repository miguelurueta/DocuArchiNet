import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";
import { AppEditorToolbar } from "./presentation/AppEditorToolbar";

function createChainMock() {
  const chain: Record<string, ReturnType<typeof vi.fn> | unknown> = {
    focus: vi.fn(() => chain),
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
    toggleHeading: vi.fn(() => chain),
    setParagraph: vi.fn(() => chain),
    run: vi.fn(() => true),
  };

  return chain as {
    focus: ReturnType<typeof vi.fn>;
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
    toggleHeading: ReturnType<typeof vi.fn>;
    setParagraph: ReturnType<typeof vi.fn>;
    insertPageBreak?: ReturnType<typeof vi.fn>;
    run: ReturnType<typeof vi.fn>;
  };
}

function createEditorMock() {
  const actionChain = createChainMock();
  const canChain = createChainMock();
  canChain.insertPageBreak = vi.fn(() => canChain);
  actionChain.insertPageBreak = vi.fn(() => actionChain);

  return {
    isActive: vi.fn((name: unknown) => Boolean(name === "bold")),
    can: vi.fn(() => ({
      chain: vi.fn(() => canChain),
    })),
    chain: vi.fn(() => actionChain),
    getAttributes: vi.fn((name: unknown) => {
      if (name === "image") {
        return { width: "50%" };
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
      expect(editor.__actionChain.setImage).toHaveBeenCalledWith({
        src: "https://cdn.example.com/image.png",
      });
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
});
