import { act, fireEvent, render, screen } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AppEditorPdf } from "./AppEditorPdf";
import styles from "./AppEditorPdf.module.css";

const appEditorMock = vi.fn((props: unknown) => {
  const toolbarActions = (props as { toolbarActions?: ReactNode }).toolbarActions;

  return (
    <div data-testid="app-editor-mock">
      <div data-testid="app-editor-toolbar-actions">{toolbarActions}</div>
    </div>
  );
});

vi.mock("../AppEditor", () => ({
  AppEditor: (props: unknown) => appEditorMock(props),
}));

function getLatestAppEditorProps() {
  const latestCall = appEditorMock.mock.calls.at(-1);
  return (latestCall?.[0] ?? {}) as Record<string, unknown>;
}

function emitPageContextChange(context: {
  currentPage: number;
  totalPages: number;
  source: "cursor" | "scroll";
}) {
  const onPageContextChange = getLatestAppEditorProps()
    .onPageContextChange as ((value: typeof context) => void) | undefined;
  onPageContextChange?.(context);
}

function emitPageBreakCommandReady(command: (() => boolean) | null) {
  const onPageBreakCommandReady = getLatestAppEditorProps()
    .onPageBreakCommandReady as ((value: typeof command) => void) | undefined;
  onPageBreakCommandReady?.(command);
}

describe(
  "AppEditorPdf [SPEC:APP-APPEDITORPDF-07-FE] [SPEC:APP-APPEDITORPDF-08-FE] [SPEC:APP-APPEDITORPDF-09-FE] [SPEC:APP-APPEDITORPDF-10-FE]",
  () => {
  beforeEach(() => {
    appEditorMock.mockClear();
  });

  it("renderiza usando AppEditor como engine shared", () => {
    render(<AppEditorPdf label="Editor PDF" defaultValue="<p>Inicial</p>" />);

    expect(screen.getByTestId("app-editor-mock")).toBeInTheDocument();
    expect(appEditorMock).toHaveBeenCalledTimes(1);
  });

  it("pasa contrato controlado al AppEditor subyacente", () => {
    const onChange = vi.fn();

    render(
      <AppEditorPdf
        label="Editor PDF controlado"
        value="<p>Controlado</p>"
        onChange={onChange}
        readOnly
      />,
    );

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        label: "Editor PDF controlado",
        value: "<p>Controlado</p>",
        onChange,
        readOnly: true,
      }),
    );
  });

  it("compone className responsive propio con className externo", () => {
    render(<AppEditorPdf label="Editor con clase" className="custom-shell" />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        className: `${styles.root} custom-shell`,
      }),
    );
  });

  it("prioriza aria-label explicito cuando esta presente", () => {
    render(
      <AppEditorPdf
        label="Label visible"
        aria-label="Editor PDF accesible"
      />,
    );

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        label: "Label visible",
        "aria-label": "Editor PDF accesible",
      }),
    );
  });

  it("usa label string como aria-label cuando no se provee uno explicito", () => {
    render(<AppEditorPdf label="Label como aria" />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        label: "Label como aria",
        "aria-label": "Label como aria",
      }),
    );
  });

  it("aplica fallback accesible cuando no hay label string ni aria-label", () => {
    render(<AppEditorPdf />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        "aria-label": "Editor PDF",
      }),
    );
  });

  it("aplica paginacion visual base por defecto", () => {
    render(<AppEditorPdf />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        paginationMode: "visual",
        pageFormat: "A4",
        pageOrientation: "portrait",
        pageMargins: {
          top: 96,
          right: 72,
          bottom: 96,
          left: 72,
        },
      }),
    );
  });

  it("permite override explicito del contrato de paginacion", () => {
    render(
      <AppEditorPdf
        paginationMode="none"
        pageOrientation="landscape"
        pageMargins={{ top: 40 }}
      />,
    );

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        paginationMode: "none",
        pageFormat: "A4",
        pageOrientation: "landscape",
        pageMargins: {
          top: 40,
          right: 72,
          bottom: 96,
          left: 72,
        },
      }),
    );
  });

  it("muestra guias visuales cuando la paginacion visual esta activa", () => {
    render(<AppEditorPdf paginationMode="visual" />);

    expect(screen.getByTestId("app-editor-pdf-page-boundary-guide")).toBeInTheDocument();
    expect(screen.getByTestId("app-editor-pdf-reading-frame-guide")).toBeInTheDocument();
  });

  it("publica metricas cuando cambia documento, pagina o zoom", () => {
    const onMetricsChange = vi.fn();
    const { rerender } = render(
      <AppEditorPdf
        paginationMode="visual"
        documentSource="doc-a"
        activePage={1}
        totalPages={3}
        zoomLevel={1}
        onMetricsChange={onMetricsChange}
      />,
    );

    rerender(
      <AppEditorPdf
        paginationMode="visual"
        documentSource="doc-b"
        activePage={2}
        totalPages={3}
        zoomLevel={1.25}
        onMetricsChange={onMetricsChange}
      />,
    );

    expect(onMetricsChange).toHaveBeenCalled();
    expect(onMetricsChange).toHaveBeenLastCalledWith(
      expect.objectContaining({
        documentSource: "doc-b",
        currentPage: 2,
        totalPages: 3,
        zoomLevel: 1.25,
      }),
    );
  });

  it("propaga defaultZoomLevel como zoomLevel efectivo cuando no se controla zoomLevel", () => {
    render(<AppEditorPdf paginationMode="visual" defaultZoomLevel={1.3} />);

    expect(appEditorMock).toHaveBeenCalledWith(
      expect.objectContaining({
        zoomLevel: 1.3,
        defaultZoomLevel: 1.3,
      }),
    );
  });

  it("escala guias visuales usando el zoom efectivo", () => {
    render(<AppEditorPdf paginationMode="visual" zoomLevel={1.25} />);

    // Any guide implies the wrapper was rendered.
    expect(screen.getByTestId("app-editor-pdf-page-boundary-guide")).toBeInTheDocument();

    const guidesWrapper = screen.getByTestId("app-editor-pdf-page-boundary-guide").parentElement;
    expect(guidesWrapper).toHaveStyle({ "--app-editor-pdf-zoom": "1.25" });
  });

  it("notifica cambio de pagina al navegar con controles internos", () => {
    const onActivePageChange = vi.fn();

    render(
      <AppEditorPdf
        paginationMode="visual"
        totalPages={3}
        activePage={2}
        onActivePageChange={onActivePageChange}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Pagina siguiente" }));
    expect(onActivePageChange).toHaveBeenCalledWith(3);

    fireEvent.click(screen.getByRole("button", { name: "Pagina anterior" }));
    expect(onActivePageChange).toHaveBeenCalledWith(1);
  });

  it("usa contexto de pagina del editor con prioridad de cursor para el contador FE-08", () => {
    render(<AppEditorPdf paginationMode="visual" totalPages={2} defaultActivePage={1} />);

    expect(screen.getByText("Pagina 1 de 2")).toBeInTheDocument();

    act(() => {
      emitPageContextChange({
        currentPage: 3,
        totalPages: 5,
        source: "cursor",
      });
    });

    expect(screen.getByText("Pagina 3 de 5")).toBeInTheDocument();
  });

  it("publica callback opcional de contexto de pagina para consumidores avanzados", () => {
    const onPageContextChange = vi.fn();

    render(
      <AppEditorPdf
        paginationMode="visual"
        totalPages={4}
        onPageContextChange={onPageContextChange}
      />,
    );

    act(() => {
      emitPageContextChange({
        currentPage: 2,
        totalPages: 4,
        source: "scroll",
      });
    });

    expect(onPageContextChange).toHaveBeenCalledWith({
      currentPage: 2,
      totalPages: 4,
      source: "scroll",
    });
  });

  it("preserva pagina controlada por props mientras actualiza totalPages desde contexto visual", () => {
    render(<AppEditorPdf paginationMode="visual" activePage={2} totalPages={3} />);

    act(() => {
      emitPageContextChange({
        currentPage: 4,
        totalPages: 5,
        source: "cursor",
      });
    });

    expect(screen.getByText("Pagina 2 de 5")).toBeInTheDocument();
  });

  it("renderiza accion manual de salto de pagina cuando se habilita la toolbar FE-09", () => {
    const insertPageBreak = vi.fn(() => true);

    render(<AppEditorPdf paginationMode="visual" showPageBreakAction />);

    expect(screen.getByTestId("app-editor-pdf-page-break-action")).toBeDisabled();

    act(() => {
      emitPageBreakCommandReady(insertPageBreak);
    });

    fireEvent.click(screen.getByTestId("app-editor-pdf-page-break-action"));

    expect(insertPageBreak).toHaveBeenCalledTimes(1);
  });

  it("no expone accion manual cuando el consumidor no la solicita", () => {
    render(<AppEditorPdf paginationMode="visual" />);

    expect(screen.queryByTestId("app-editor-pdf-page-break-action")).not.toBeInTheDocument();
  });

  it("mantiene la accion manual como control opcional aunque el editor subyacente entregue el comando", () => {
    const insertPageBreak = vi.fn(() => true);

    render(<AppEditorPdf paginationMode="visual" showPageBreakAction={false} />);

    act(() => {
      emitPageBreakCommandReady(insertPageBreak);
    });

    expect(screen.queryByTestId("app-editor-pdf-page-break-action")).not.toBeInTheDocument();
  });

  it("compone la accion de salto manual con toolbarActions externas", () => {
    const insertPageBreak = vi.fn(() => true);

    render(
      <AppEditorPdf
        paginationMode="visual"
        showPageBreakAction
        toolbarActions={<button type="button">Accion externa</button>}
      />,
    );

    act(() => {
      emitPageBreakCommandReady(insertPageBreak);
    });

    expect(screen.getByRole("button", { name: "Accion externa" })).toBeInTheDocument();
    expect(screen.getByTestId("app-editor-pdf-page-break-action")).toBeInTheDocument();
  });

  it("no expone toggle de tema en la toolbar por defecto (FE-10)", () => {
    render(<AppEditorPdf paginationMode="visual" />);

    expect(screen.queryByRole("button", { name: /tema/i })).not.toBeInTheDocument();
  });
  },
);
