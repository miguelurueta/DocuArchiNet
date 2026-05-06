import type React from "react";
import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";

import { AppVisorEmbedPdf } from "./AppVisorEmbedPdf";
import { AppVisorEmbedPdf as AppVisorEmbedPdfFromIndex } from "./index";

const engineStateLoading = { status: "loading" as const };
const engineStateError = { status: "error" as const, error: new Error("boom") };
const engineStateReady = {
  status: "ready" as const,
  engine: {} as unknown,
};

const useEmbedPdfEngineMock = vi.fn();
vi.mock("./engine/useEmbedPdfEngine", () => ({
  useEmbedPdfEngine: () => useEmbedPdfEngineMock(),
}));

const createBasicPluginRegistrationMock = vi.fn(() => ({}));
vi.mock("./plugins/pluginRegistration", () => ({
  createBasicPluginRegistration: () => createBasicPluginRegistrationMock(),
}));

const useDemoPdfUrlMock = vi.fn(() => "/demo/Radicado_2026_0413.pdf");
vi.mock("./hooks/useDemoPdfUrl", () => ({
  useDemoPdfUrl: () => useDemoPdfUrlMock(),
}));

let lastDocumentContentSrc: string | undefined;

const zoomInMock = vi.fn();
const zoomOutMock = vi.fn();
const requestZoomMock = vi.fn();
vi.mock("@embedpdf/plugin-zoom/react", () => ({
  useZoom: () => ({
    state: { zoomLevel: 1, currentZoomLevel: 1, isMarqueeZoomActive: false },
    provides: { zoomIn: zoomInMock, zoomOut: zoomOutMock, requestZoom: requestZoomMock },
  }),
}));

vi.mock("./engine/embedPdfAdapter", () => ({
  EmbedPDF: ({ children }: { children: React.ReactNode }) => (
    <div data-testid="embedpdf">{children}</div>
  ),
  useActiveDocument: () => ({ activeDocumentId: "doc-1" }),
  useDocumentManagerCapability: () => ({
    provides: { openDocumentUrl: vi.fn() },
  }),
  DocumentContent: ({
    documentId,
    children,
  }: {
    documentId: string;
    children: (props: { isLoaded: boolean; isError: boolean; isLoading: boolean }) => React.ReactNode;
  }) => {
    lastDocumentContentSrc = documentId;
    return (
      <div data-testid="document-content">
        {children({ isLoaded: true, isError: false, isLoading: false })}
      </div>
    );
  },
  Viewport: ({
    children,
    className,
  }: {
    children: React.ReactNode;
    className?: string;
  }) => (
    <div data-testid="viewport" className={className}>
      {children}
    </div>
  ),
  Scroller: ({
    renderPage,
  }: {
    renderPage: (props: { pageIndex: number }) => React.ReactNode;
  }) => <div data-testid="scroller">{renderPage({ pageIndex: 0 })}</div>,
  RenderLayer: ({ pageIndex }: { pageIndex: number }) => (
    <div data-testid="render-layer">page:{pageIndex}</div>
  ),
  ScrollStrategy: { Vertical: "Vertical" },
}));

describe("AppVisorEmbedPdf [SPEC:SCRUMCORE-201]", () => {
  it("muestra loader mientras carga el engine", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateLoading);

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    expect(screen.getByText(/cargando motor pdf/i)).toBeInTheDocument();
  });

  it("muestra error state si falla el engine", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateError);

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    expect(
      screen.getByText(/no fue posible cargar el documento/i)
    ).toBeInTheDocument();
  });

  it("usa el demo pdf cuando fileUrl no existe", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    lastDocumentContentSrc = undefined;

    render(<AppVisorEmbedPdf />);

    expect(useDemoPdfUrlMock).toHaveBeenCalled();
    expect(screen.getByTestId("render-layer")).toBeInTheDocument();
  });

  it("[SPEC:SCRUMCORE-204] renderiza toolbar y permite zoom in/out/reset", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    zoomInMock.mockClear();
    zoomOutMock.mockClear();
    requestZoomMock.mockClear();

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    expect(screen.getByRole("toolbar", { name: /toolbar pdf/i })).toBeInTheDocument();

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /zoom in/i }));
    await user.click(screen.getByRole("button", { name: /zoom out/i }));
    await user.click(screen.getByRole("button", { name: /reset zoom/i }));

    expect(zoomInMock).toHaveBeenCalledTimes(1);
    expect(zoomOutMock).toHaveBeenCalledTimes(1);
    expect(requestZoomMock).toHaveBeenCalledWith(1);
  });

  it("permite consumo desde index sin exponer detalles del engine", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateLoading);
    render(<AppVisorEmbedPdfFromIndex fileUrl="/demo/Radicado_2026_0413.pdf" />);
    expect(screen.getByText(/cargando motor pdf/i)).toBeInTheDocument();
  });
});
