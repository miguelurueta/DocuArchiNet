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
const requestZoomByMock = vi.fn();
vi.mock("@embedpdf/plugin-zoom/react", () => ({
  useZoom: () => ({
    state: { zoomLevel: 1, currentZoomLevel: 1, isMarqueeZoomActive: false },
    provides: {
      zoomIn: zoomInMock,
      zoomOut: zoomOutMock,
      requestZoom: requestZoomMock,
      requestZoomBy: requestZoomByMock,
    },
  }),
}));

const scrollToPageMock = vi.fn();
vi.mock("@embedpdf/plugin-scroll/react", () => ({
  useScroll: () => ({
    provides: { scrollToPage: scrollToPageMock },
    state: { currentPage: 1, totalPages: 3 },
  }),
}));

const rotateForwardMock = vi.fn();
const rotateBackwardMock = vi.fn();
const setRotationMock = vi.fn();
vi.mock("@embedpdf/plugin-rotate/react", () => ({
  useRotate: () => ({
    rotation: 0,
    provides: {
      rotateForward: rotateForwardMock,
      rotateBackward: rotateBackwardMock,
      setRotation: setRotationMock,
    },
  }),
  Rotate: ({ children }: { children: React.ReactNode }) => <div data-testid="rotate">{children}</div>,
}));

const printMock = vi.fn();
let printProvides: { print: typeof printMock } | null = { print: printMock };
vi.mock("@embedpdf/plugin-print/react", () => ({
  __setPrintProvides: (next: typeof printProvides) => {
    printProvides = next;
  },
  usePrint: () => ({
    provides: printProvides,
  }),
}));

const downloadMock = vi.fn();
let exportProvides: { download: typeof downloadMock } | null = {
  download: downloadMock,
};
vi.mock("@embedpdf/plugin-export/react", () => ({
  __setExportProvides: (next: typeof exportProvides) => {
    exportProvides = next;
  },
  useExport: () => ({
    provides: exportProvides,
  }),
}));

vi.mock("@embedpdf/plugin-thumbnail/react", () => ({
  ThumbnailsPane: ({
    documentId,
    children,
  }: {
    documentId: string;
    children: (meta: { pageIndex: number; top: number; wrapperHeight: number }) => React.ReactNode;
  }) => (
    <div data-testid="thumbnails-pane" data-document-id={documentId}>
      {children({ pageIndex: 0, top: 0, wrapperHeight: 100 })}
    </div>
  ),
  ThumbImg: ({ meta }: { meta: { pageIndex: number } }) => (
    <img data-testid="thumb-img" alt={`thumb-${meta.pageIndex}`} />
  ),
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
    requestZoomByMock.mockClear();

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    expect(screen.getByRole("toolbar", { name: /toolbar pdf/i })).toBeInTheDocument();

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /zoom in/i }));
    await user.click(screen.getByRole("button", { name: /zoom out/i }));
    await user.click(screen.getByRole("button", { name: /reset zoom/i }));

    expect(requestZoomByMock).toHaveBeenCalledTimes(2);
    expect(requestZoomMock).toHaveBeenCalledWith(1, undefined);
  });

  it("[SPEC:SCRUMCORE-205] toggle thumbnails abre/cierra panel sin romper el visor", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    expect(screen.queryByTestId("thumbnails-pane")).not.toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /abrir thumbnails/i }));
    expect(screen.getByTestId("thumbnails-pane")).toBeInTheDocument();

    scrollToPageMock.mockClear();
    await user.click(screen.getByRole("button", { name: /ir a página 1/i }));
    expect(scrollToPageMock).toHaveBeenCalledWith({ pageNumber: 1, behavior: "smooth", alignY: 0 });

    await user.click(screen.getByRole("button", { name: /abrir thumbnails/i }));
    expect(screen.queryByTestId("thumbnails-pane")).not.toBeInTheDocument();

    expect(screen.getByTestId("render-layer")).toBeInTheDocument();
  });

  it("[SPEC:SCRUMCORE-206] toolbar permite rotar derecha/izquierda y reset", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    rotateForwardMock.mockClear();
    rotateBackwardMock.mockClear();
    setRotationMock.mockClear();

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /rotar izquierda/i }));
    await user.click(screen.getByRole("button", { name: /rotar derecha/i }));

    expect(rotateBackwardMock).toHaveBeenCalledTimes(1);
    expect(rotateForwardMock).toHaveBeenCalledTimes(1);
  });

  it("[SPEC:SCRUMCORE-207] toolbar permite print/export cuando plugins exponen provides", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    printMock.mockClear();
    downloadMock.mockClear();
    printProvides = { print: printMock };
    exportProvides = { download: downloadMock };

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /print/i }));
    await user.click(screen.getByRole("button", { name: /export/i }));

    expect(printMock).toHaveBeenCalledTimes(1);
    expect(downloadMock).toHaveBeenCalledTimes(1);
  });

  it("[SPEC:SCRUMCORE-207] no crashea cuando print/export provides es null", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    printMock.mockClear();
    downloadMock.mockClear();
    printProvides = null;
    exportProvides = null;

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /print/i }));
    await user.click(screen.getByRole("button", { name: /export/i }));

    expect(printMock).not.toHaveBeenCalled();
    expect(downloadMock).not.toHaveBeenCalled();
    expect(screen.getByTestId("render-layer")).toBeInTheDocument();
  });

  it("permite consumo desde index sin exponer detalles del engine", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateLoading);
    render(<AppVisorEmbedPdfFromIndex fileUrl="/demo/Radicado_2026_0413.pdf" />);
    expect(screen.getByText(/cargando motor pdf/i)).toBeInTheDocument();
  });
});
