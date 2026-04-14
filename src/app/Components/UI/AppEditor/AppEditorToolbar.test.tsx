import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi, afterEach } from "vitest";
import { AppEditorToolbar } from "./presentation/AppEditorToolbar";

function createChainMock() {
  const chain = {
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
    toggleHeading: vi.fn(() => chain),
    setParagraph: vi.fn(() => chain),
    run: vi.fn(() => true),
  };

  return chain;
}

function createEditorMock() {
  const actionChain = createChainMock();
  const canChain = createChainMock();

  return {
    isActive: vi.fn((name: unknown) => name === "bold"),
    can: vi.fn(() => ({
      chain: vi.fn(() => canChain),
    })),
    chain: vi.fn(() => actionChain),
    getAttributes: vi.fn(() => ({ href: "https://openai.com" })),
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

  it("solicita URL y configura enlaces e imagenes", () => {
    const editor = createEditorMock();
    const promptSpy = vi
      .spyOn(window, "prompt")
      .mockReturnValueOnce("docs.openai.com")
      .mockReturnValueOnce("cdn.example.com/image.png");

    render(<AppEditorToolbar editor={editor as never} />);

    fireEvent.click(screen.getByLabelText("Insertar enlace"));
    fireEvent.click(screen.getByLabelText("Insertar imagen"));

    expect(promptSpy).toHaveBeenCalledTimes(2);
    expect(editor.__actionChain.setLink).toHaveBeenCalledWith({
      href: "https://docs.openai.com",
    });
    expect(editor.__actionChain.setImage).toHaveBeenCalledWith({
      src: "https://cdn.example.com/image.png",
    });
  });
});
