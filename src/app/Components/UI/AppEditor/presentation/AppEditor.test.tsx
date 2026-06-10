import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AppEditor } from "./AppEditor";

const mocks = vi.hoisted(() => {
  const editor = {
    state: {
      selection: {
        from: 4,
        to: 9,
        head: 9,
      },
    },
    view: {
      coordsAtPos: vi.fn(() => ({
        top: 0,
        bottom: 20,
        left: 0,
        right: 0,
      })),
    },
    commands: {
      setTextSelection: vi.fn(),
      focus: vi.fn(),
    },
    getHTML: vi.fn(() => "<p>Contenido editable</p>"),
    on: vi.fn(),
    off: vi.fn(),
  };

  return {
    editor,
  };
});

vi.mock("../application/useAppEditor", () => ({
  useAppEditor: () => ({
    editor: mocks.editor,
    isEditable: true,
    insertLocalImage: vi.fn(),
  }),
}));

vi.mock("../application/usePaginationMetrics", () => ({
  usePaginationMetrics: () => ({
    totalPages: 3,
    visualContentHeight: 3168,
    pages: [
      {
        pageNumber: 1,
        top: 0,
        bottom: 1056,
        contentTop: 96,
        contentBottom: 960,
        startBlockIndex: 0,
        endBlockIndex: 0,
      },
      {
        pageNumber: 2,
        top: 1088,
        bottom: 2144,
        contentTop: 1184,
        contentBottom: 2048,
        startBlockIndex: 1,
        endBlockIndex: 1,
      },
      {
        pageNumber: 3,
        top: 2176,
        bottom: 3232,
        contentTop: 2272,
        contentBottom: 3136,
        startBlockIndex: 2,
        endBlockIndex: 2,
      },
    ],
  }),
}));

vi.mock("./AppEditorToolbar", () => ({
  AppEditorToolbar: ({
    toolbarActions,
    trailingContent,
  }: {
    toolbarActions?: React.ReactNode;
    trailingContent?: React.ReactNode;
  }) => (
    <div data-testid="toolbar">
      {toolbarActions}
      {trailingContent}
    </div>
  ),
}));

vi.mock("../infrastructure/TiptapEditorContent", () => ({
  TiptapEditorContent: ({ className }: { className?: string }) => (
    <div data-testid="editor-content" className={className}>
      <div className="ProseMirror">
        <p>Bloque uno</p>
        <p>Bloque dos</p>
        <p>Bloque tres</p>
      </div>
    </div>
  ),
}));

vi.mock("./AppEditorPreview", async () => {
  const React = await vi.importActual<typeof import("react")>("react");

  return {
    AppEditorPreview: React.forwardRef<
    { goToPage: (pageNumber: number) => void },
    {
      html: string;
      onPageCountChange?: (pageCount: number) => void;
      onCurrentPageChange?: (pageNumber: number) => void;
    }
  >(function MockAppEditorPreview({ html, onCurrentPageChange, onPageCountChange }, ref) {
    React.useImperativeHandle(ref, () => ({
      goToPage: (pageNumber: number) => {
        onCurrentPageChange?.(pageNumber);
      },
    }));

    React.useLayoutEffect(() => {
      onPageCountChange?.(11);
      onCurrentPageChange?.(1);
    }, [onCurrentPageChange, onPageCountChange]);

    return (
      <div className="previewViewport" data-testid="preview-viewport">
        <div dangerouslySetInnerHTML={{ __html: html }} />
      </div>
    );
  }),
  };
});

describe("AppEditor preview mode", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("alterna entre edicion y vista previa sin perder snapshot ni seleccion", async () => {
    render(<AppEditor defaultValue="<p>Contenido editable</p>" paginationMode="visual" />);

    expect(screen.getByTestId("editor-content")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Vista previa" }));

    expect(screen.queryByTestId("editor-content")).not.toBeInTheDocument();
    expect(screen.getAllByText("Contenido editable").length).toBeGreaterThan(0);

    fireEvent.click(screen.getByRole("button", { name: "Editar" }));

    expect(screen.getByTestId("editor-content")).toBeInTheDocument();
    expect(mocks.editor.commands.setTextSelection).toHaveBeenCalledWith({
      from: 4,
      to: 9,
    });
    expect(mocks.editor.commands.focus).toHaveBeenCalled();
  });

  it("muestra pildoras sutiles de pagina sin separadores ni lineas", async () => {
    const { container } = render(
      <AppEditor defaultValue="<p>Contenido editable</p>" paginationMode="visual" />,
    );

    expect(container.querySelector('[class*="pageStack"]')).not.toBeInTheDocument();
    expect(container.querySelector('[class*="pageShell"]')).not.toBeInTheDocument();
    expect(container.querySelector('[class*="pageSeparatorLayer"]')).not.toBeInTheDocument();
    expect(container.querySelector('[class*="pageSeparator"]')).not.toBeInTheDocument();
    expect(container.querySelector('[class*="pageIndicatorLayer"]')).toBeInTheDocument();
    expect(screen.getAllByRole("button", { name: /^Ir a pagina \d+$/ })).toHaveLength(3);
    expect(screen.getByText("Pagina 2")).toBeInTheDocument();
    expect(screen.getByText("Pagina 3")).toBeInTheDocument();
    expect(screen.getByText("Bloque dos")).not.toHaveAttribute(
      "data-app-editor-page-separator",
    );
    expect(screen.getByText("Bloque tres")).not.toHaveAttribute(
      "data-app-editor-page-separator",
    );
  });

  it("activa presentacion como submodo visual sin regenerar previewHtml ni duplicar preview", async () => {
    const { container } = render(
      <AppEditor defaultValue="<p>Contenido editable</p>" paginationMode="visual" />,
    );

    expect(screen.getByRole("button", { name: /Modo presentaci/i })).toBeDisabled();

    fireEvent.click(screen.getByRole("button", { name: "Vista previa" }));
    expect(await screen.findByRole("button", { name: "Ir a pagina 11" })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Ir a pagina 12" })).not.toBeInTheDocument();
    mocks.editor.getHTML.mockClear();

    fireEvent.click(screen.getByRole("button", { name: /Modo presentaci/i }));

    expect(mocks.editor.getHTML).not.toHaveBeenCalled();
    expect(document.body.querySelectorAll('[class*="previewViewport"]')).toHaveLength(1);
    expect(document.body.querySelectorAll('[data-app-editor-preview-stage-host]')).toHaveLength(1);
    expect(document.body.querySelector('[class*="previewStageSlotPresentation"]')).toBeInTheDocument();
    expect(container.querySelector('[class*="documentWorkspace"]')).toHaveAttribute(
      "data-presentation-active",
      "true",
    );
    expect(document.body.querySelector('[class*="presentationOverlay"]')).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Salir" })).toBeInTheDocument();
    expect(document.body.querySelector(".ant-tooltip")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Salir" }));

    expect(document.body.querySelector('[class*="presentationOverlay"]')).not.toBeInTheDocument();
    expect(document.body.querySelector('[class*="previewStageSlotPresentation"]')).not.toBeInTheDocument();
    expect(document.body.querySelectorAll('[class*="previewViewport"]')).toHaveLength(1);
  });
});

