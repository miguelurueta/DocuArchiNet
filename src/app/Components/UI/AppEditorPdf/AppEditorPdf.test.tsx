import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AppEditorPdf } from "./AppEditorPdf";
import styles from "./AppEditorPdf.module.css";

const appEditorMock = vi.fn(() => <div data-testid="app-editor-mock" />);

vi.mock("../AppEditor", () => ({
  AppEditor: (props: unknown) => appEditorMock(props),
}));

describe("AppEditorPdf [SPEC:APP-APPEDITORPDF-07-FE]", () => {
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
});
