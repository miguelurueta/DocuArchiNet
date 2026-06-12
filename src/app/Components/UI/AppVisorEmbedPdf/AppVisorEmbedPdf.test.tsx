import type React from "react";
import { createRef } from "react";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { PdfErrorCode } from "@embedpdf/models";

import { AppVisorEmbedPdf } from "./AppVisorEmbedPdf";
import type { AppVisorEmbedPdfRef } from "./AppVisorEmbedPdf.types";
import { AppVisorEmbedPdf as AppVisorEmbedPdfFromIndex } from "./index";

const appGuideTourMock = vi.hoisted(() => ({
  start: vi.fn(),
  stop: vi.fn(),
  refresh: vi.fn(),
}));

const pdfReplacementUtilsMock = vi.hoisted(() => ({
  extractSinglePagePdfs: vi.fn(async (_sourcePdf: Blob, pageNumbers: number[]) =>
    pageNumbers.map((pageNumber) => ({
      pageNumber,
      blob: new Blob([`page-${pageNumber}`], { type: "application/pdf" }),
    })),
  ),
  calculateBlobSha256: vi.fn(async (blob: Blob) => `hash-${blob.size}`),
}));

vi.mock("./utils/pdfSinglePageExtraction", () => ({
  extractSinglePagePdfs: pdfReplacementUtilsMock.extractSinglePagePdfs,
}));

vi.mock("./utils/hashSha256", () => ({
  calculateBlobSha256: pdfReplacementUtilsMock.calculateBlobSha256,
}));

type MockPersonalSignatureStatus = "idle" | "loading" | "ready" | "error" | "empty";

const workflowPersonalSignatureMock = vi.hoisted(() => {
  const load = vi.fn();
  const reload = vi.fn();
  const clear = vi.fn();
  return {
    load,
    reload,
    clear,
    state: {
      status: "idle" as MockPersonalSignatureStatus,
      meta: null as { fileName: string; contentType: string; expiresAt: string; urlTemporal: string } | null,
      blobUrl: null as string | null,
      imageData: null as ArrayBuffer | null,
      errorMessage: null as string | null,
      load,
      reload,
      clear,
    },
  };
});

vi.mock("../AppGuideTour", async () => {
  const ReactRuntime = await import("react");

  return {
    AppGuideTour: ({ ref }: { ref?: React.Ref<{ start: () => void; stop: () => void; refresh: () => void }> }) => {
      ReactRuntime.useImperativeHandle(ref, () => appGuideTourMock);
      return <div data-testid="app-guide-tour" />;
    },
  };
});

const clienteApiGetMock = vi.fn();
vi.mock("../../../../../api/Clienteaxios", () => ({
  default: {
    defaults: { baseURL: "https://localhost:7101" },
    get: (...args: unknown[]) => clienteApiGetMock(...args),
  },
}));

const engineStateLoading = { status: "loading" as const };
const engineStateError = { status: "error" as const, error: new Error("boom") };
const extractPagesMock = vi.fn((_: unknown, pageIndexes: number[]) => ({
  wait: (resolved: (value: ArrayBuffer) => void) => {
    const text = `page-${pageIndexes.join("-")}`;
    resolved(new TextEncoder().encode(text).buffer);
  },
}));
const engineStateReady = {
  status: "ready" as const,
  engine: {
    extractPages: extractPagesMock,
  } as unknown,
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

vi.mock("./hooks/useWorkflowPersonalSignature", () => ({
  useWorkflowPersonalSignature: () => workflowPersonalSignatureMock.state,
}));

// JSDOM no soporta bien cargar recursos `blob:` en <img> en unit tests.
// Evitamos que el ResourceLoader intente "navegar" el blobUrl.
let originalImageCtor: unknown = null;
beforeEach(() => {
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  originalImageCtor = (globalThis as any).Image;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  (globalThis as any).Image = class MockImage {
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onload: any = null;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    onerror: any = null;
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
    set src(_v: any) {
      // noop
    }
    get src() {
      return "";
    }
  };
  activeDocumentIdState = "doc-1";
  workflowPersonalSignatureMock.load.mockReset();
  workflowPersonalSignatureMock.reload.mockReset();
  workflowPersonalSignatureMock.clear.mockReset();
  workflowPersonalSignatureMock.state.status = "idle";
  workflowPersonalSignatureMock.state.meta = null;
  workflowPersonalSignatureMock.state.blobUrl = null;
  workflowPersonalSignatureMock.state.imageData = null;
  workflowPersonalSignatureMock.state.errorMessage = null;
  annotationPagesState = {};
  annotationCommitMock.mockClear();
  extractPagesMock.mockClear();
  saveAsCopyMock.mockClear();
  pdfReplacementUtilsMock.extractSinglePagePdfs.mockClear();
  pdfReplacementUtilsMock.calculateBlobSha256.mockClear();
  exportProvides = {
    download: downloadMock,
    saveAsCopy: saveAsCopyMock,
  };
});

afterEach(() => {
  if (!originalImageCtor) return;
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  (globalThis as any).Image = originalImageCtor;
  originalImageCtor = null;
});

let lastDocumentContentSrc: string | undefined;
let documentContentRenderState: { isLoaded: boolean; isError: boolean; isLoading: boolean } = {
  isLoaded: true,
  isError: false,
  isLoading: false,
};

let activeDocumentIdState: string | null = "doc-1";

const openDocumentUrlMock = vi.fn<
  [
    {
      url: string;
      name?: string;
      autoActivate?: boolean;
      password?: string;
    },
  ],
  {
    wait: (
      resolved: (v: { documentId: string; task: { wait: (r: (v: unknown) => void, e: (x: unknown) => void) => void } }) => void,
      rejected: (e: unknown) => void,
    ) => void;
  }
>(() => ({
  wait: (resolved) =>
    resolved({
      documentId: "doc-1",
      task: {
        wait: (r) => r({}),
      },
    }),
}));

const retryDocumentMock = vi.fn<
  [string, { password?: string }],
  {
    wait: (
      resolved: (v: { documentId: string; task: { wait: (r: (v: unknown) => void, e: (x: unknown) => void) => void } }) => void,
      rejected: (e: unknown) => void,
    ) => void;
  }
>(() => ({
  wait: (resolved) =>
    resolved({
      documentId: "doc-1",
      task: {
        wait: (r) => r({}),
      },
    }),
}));

const onDocumentErrorMock = vi.fn();

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
const scrollToNextPageMock = vi.fn();
const scrollToPreviousPageMock = vi.fn();
let scrollProvides: {
  scrollToPage: typeof scrollToPageMock;
  scrollToNextPage: typeof scrollToNextPageMock;
  scrollToPreviousPage: typeof scrollToPreviousPageMock;
} | null = {
  scrollToPage: scrollToPageMock,
  scrollToNextPage: scrollToNextPageMock,
  scrollToPreviousPage: scrollToPreviousPageMock,
};
let scrollState: { currentPage: number; totalPages: number } = {
  currentPage: 1,
  totalPages: 3,
};
vi.mock("@embedpdf/plugin-scroll/react", () => ({
  useScroll: () => ({
    provides: scrollProvides,
    state: scrollState,
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

vi.mock("@embedpdf/plugin-selection/react", () => ({
  SelectionLayer: ({ documentId, pageIndex }: { documentId: string; pageIndex: number }) => (
    <div data-testid="selection-layer" data-document-id={documentId} data-page-index={pageIndex} />
  ),
  useSelectionCapability: () => ({ provides: null }),
}));

const annotationCommitMock = vi.fn(() => ({
  wait: (resolved: (value: boolean) => void) => resolved(true),
}));
let annotationPagesState: Record<string, string[]> = {};

vi.mock("@embedpdf/plugin-annotation/react", () => ({
  AnnotationLayer: ({ documentId, pageIndex }: { documentId: string; pageIndex: number }) => (
    <div data-testid="annotation-layer" data-document-id={documentId} data-page-index={pageIndex} />
  ),
  useAnnotation: () => ({
    state: { selectedUids: [], pages: annotationPagesState },
    provides: {
      deleteAnnotation: vi.fn(),
    },
  }),
  useAnnotationCapability: () => ({
    provides: {
      commit: annotationCommitMock,
    },
  }),
}));

const signatureAddEntryMock = vi.fn(() => "sig-1");
const activateSignaturePlacementMock = vi.fn();
const loadEntriesMock = vi.fn();
let signatureEntriesState: unknown[] = [];
let signatureProvides: {
  addEntry: typeof signatureAddEntryMock;
  loadEntries: typeof loadEntriesMock;
  forDocument: (documentId: string) => { activateSignaturePlacement: typeof activateSignaturePlacementMock };
} | null = {
  addEntry: signatureAddEntryMock,
  loadEntries: loadEntriesMock,
  forDocument: () => ({ activateSignaturePlacement: activateSignaturePlacementMock }),
};

vi.mock("@embedpdf/plugin-signature/react", () => ({
  // hooks
  useSignatureCapability: () => ({ provides: signatureProvides, ready: Promise.resolve() }),
  useSignatureEntries: () => ({ entries: signatureEntriesState, provides: signatureProvides }),
  useActivePlacement: () => null,
  // helpers
  serializeEntries: (entries: unknown[]) => entries,
  deserializeEntries: (data: unknown[]) => data,
  // components used by modal (render-only in tests)
  SignatureDrawPad: ({ onResult }: { onResult: (r: unknown) => void }) => (
    <button type="button" onClick={() => onResult({ creationType: "draw" })}>
      draw-pad
    </button>
  ),
  SignatureTypePad: ({ onResult }: { onResult: (r: unknown) => void }) => (
    <button type="button" onClick={() => onResult({ creationType: "type" })}>
      type-pad
    </button>
  ),
  useSignatureUpload: ({ onResult }: { onResult: (r: unknown) => void }) => ({
    inputRef: { current: null },
    openFilePicker: () => onResult({ creationType: "upload" }),
    processFile: vi.fn(),
    handleFileInputChange: vi.fn(),
    handleDrop: vi.fn(),
    handleDragOver: vi.fn(),
    handleDragLeave: vi.fn(),
    previewUrl: null,
    isDragging: false,
    clear: vi.fn(),
    accept: "image/png,image/jpeg,image/svg+xml",
  }),
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
const saveAsCopyMock = vi.fn(() => ({
  wait: (resolved: (value: ArrayBuffer) => void) => resolved(new ArrayBuffer(8)),
}));
let exportProvides: { download?: typeof downloadMock; saveAsCopy?: typeof saveAsCopyMock } | null = {
  download: downloadMock,
  saveAsCopy: saveAsCopyMock,
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
  __setActiveDocumentId: (next: string | null) => {
    activeDocumentIdState = next;
  },
  useActiveDocument: () => ({ activeDocumentId: activeDocumentIdState }),
  useDocumentState: (documentId: string | null) =>
    documentId
      ? {
          document: {
            id: documentId,
            pageCount: 5,
            pages: [
              { index: 0, size: { width: 612, height: 792 }, rotation: 0 },
              { index: 1, size: { width: 612, height: 792 }, rotation: 0 },
              { index: 2, size: { width: 612, height: 792 }, rotation: 0 },
              { index: 3, size: { width: 612, height: 792 }, rotation: 0 },
              { index: 4, size: { width: 612, height: 792 }, rotation: 0 },
            ],
          },
        }
      : null,
  useDocumentManagerCapability: () => ({
    provides: {
      openDocumentUrl: openDocumentUrlMock,
      retryDocument: retryDocumentMock,
      onDocumentError: (cb: (evt: unknown) => void) => {
        onDocumentErrorMock.mockImplementation(cb);
        return () => onDocumentErrorMock.mockReset();
      },
    },
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
        {children(documentContentRenderState)}
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
    renderPage: (props: {
      pageIndex: number;
      width: number;
      height: number;
      rotatedWidth: number;
      rotatedHeight: number;
    }) => React.ReactNode;
  }) => (
    <div data-testid="scroller">
      {renderPage({
        pageIndex: 0,
        width: 612,
        height: 792,
        rotatedWidth: 612,
        rotatedHeight: 792,
      })}
    </div>
  ),
  RenderLayer: ({ pageIndex }: { pageIndex: number }) => (
    <div data-testid="render-layer">page:{pageIndex}</div>
  ),
  ScrollStrategy: { Vertical: "Vertical" },
}));

describe("AppVisorEmbedPdf [SPEC:SCRUMCORE-201]", () => {
  beforeEach(() => {
    documentContentRenderState = { isLoaded: true, isError: false, isLoading: false };
  });

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

  it("usa el demo pdf cuando fileUrl no existe", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    lastDocumentContentSrc = undefined;

    render(<AppVisorEmbedPdf />);

    expect(useDemoPdfUrlMock).toHaveBeenCalled();
    await waitFor(() => {
      expect(screen.getByTestId("render-layer")).toBeInTheDocument();
    });
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
    scrollToPageMock.mockClear();
    scrollToNextPageMock.mockClear();
    scrollToPreviousPageMock.mockClear();
    scrollProvides = {
      scrollToPage: scrollToPageMock,
      scrollToNextPage: scrollToNextPageMock,
      scrollToPreviousPage: scrollToPreviousPageMock,
    };
    scrollState = { currentPage: 1, totalPages: 3 };
    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    expect(screen.queryByTestId("thumbnails-pane")).not.toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /abrir thumbnails/i }));
    expect(screen.getByTestId("thumbnails-pane")).toBeInTheDocument();

    scrollToPageMock.mockClear();
    await user.click(screen.getByRole("button", { name: /ir a pÃ¡gina 1/i }));
    expect(scrollToPageMock).toHaveBeenCalledWith({ pageNumber: 1, behavior: "smooth", alignY: 0 });

    await user.click(screen.getByRole("button", { name: /abrir thumbnails/i }));
    expect(screen.queryByTestId("thumbnails-pane")).not.toBeInTheDocument();

    expect(screen.getByTestId("render-layer")).toBeInTheDocument();
  });

  it("[SPEC:SCRUMCORE-208] paginaciÃ³n usa scroll plugin (prev/next) y muestra indicador", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    scrollToNextPageMock.mockClear();
    scrollToPreviousPageMock.mockClear();
    scrollProvides = {
      scrollToPage: scrollToPageMock,
      scrollToNextPage: scrollToNextPageMock,
      scrollToPreviousPage: scrollToPreviousPageMock,
    };
    scrollState = { currentPage: 4, totalPages: 20 };

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    expect(screen.getByLabelText(/pÃ¡gina 4 de 20/i)).toBeInTheDocument();

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /pÃ¡gina anterior/i }));
    await user.click(screen.getByRole("button", { name: /pÃ¡gina siguiente/i }));

    expect(scrollToPreviousPageMock).toHaveBeenCalledTimes(1);
    expect(scrollToNextPageMock).toHaveBeenCalledTimes(1);
  });

  it("[SPEC:SCRUMCORE-209] muestra password prompt cuando el documento requiere contraseÃƒÆ’Ã‚Â±a", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    documentContentRenderState = { isLoaded: false, isError: true, isLoading: false };

    render(<AppVisorEmbedPdf fileUrl="/demo/protected.pdf" />);

    expect(screen.getByRole("dialog", { name: /documento protegido/i })).toBeInTheDocument();
    expect(screen.getByLabelText(/contraseña del documento/i)).toBeInTheDocument();
  });

  it("[SPEC:SCRUMCORE-233] no muestra password prompt en OPEN_FAILED (evita falso 'Documento protegido')", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    documentContentRenderState = { isLoaded: false, isError: false, isLoading: true };

    const { __setActiveDocumentId } = await import("./engine/embedPdfAdapter");
    __setActiveDocumentId(null);

    openDocumentUrlMock.mockImplementationOnce(() => ({
      wait: (resolved) =>
        resolved({
          documentId: "doc-1",
          task: {
            wait: (_r, e) => e(new Error("boom")),
          },
        }),
    }));

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    await waitFor(() => {
      expect(screen.queryByRole("dialog", { name: /documento protegido/i })).not.toBeInTheDocument();
    });

    __setActiveDocumentId("doc-1");
  });

  it("[SPEC:SCRUMCORE-238] no abre prompt si PDFium reporta Password pero el PDF no esta cifrado", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    documentContentRenderState = { isLoaded: false, isError: false, isLoading: true };
    const pdfUrl = URL.createObjectURL(new Blob(["%PDF-1.7\n1 0 obj\n<<>>\nendobj\n%%EOF"], { type: "application/pdf" }));

    render(<AppVisorEmbedPdf fileUrl={pdfUrl} />);

    onDocumentErrorMock({
      documentId: "doc-1",
      reason: { code: PdfErrorCode.Password },
    });

    await waitFor(() => {
      expect(screen.queryByRole("dialog", { name: /documento protegido/i })).not.toBeInTheDocument();
    });

    URL.revokeObjectURL(pdfUrl);
  });

  it("[SPEC:SCRUMCORE-208] no crashea cuando scroll.provides es null", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    scrollToNextPageMock.mockClear();
    scrollToPreviousPageMock.mockClear();
    scrollProvides = null;
    scrollState = { currentPage: 1, totalPages: 1 };

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /pÃ¡gina anterior/i }));
    await user.click(screen.getByRole("button", { name: /pÃ¡gina siguiente/i }));

    expect(scrollToPreviousPageMock).not.toHaveBeenCalled();
    expect(scrollToNextPageMock).not.toHaveBeenCalled();
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

  it("[SPEC:SCRUMCORE-235] muestra ayuda y conecta inicio de guia interactiva sin cambiar toolbar", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    appGuideTourMock.start.mockClear();

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    expect(screen.getByTestId("app-guide-tour")).toBeInTheDocument();
    expect(screen.getByRole("toolbar", { name: /toolbar pdf/i })).toHaveAttribute(
      "data-guide-tour-id",
      "pdf-toolbar",
    );

    const helpButton = screen.getByRole("button", { name: /guia interactiva/i });
    expect(helpButton).toHaveAttribute("data-guide-tour-id", "pdf-help");

    const user = userEvent.setup();
    await user.click(helpButton);

    expect(appGuideTourMock.start).toHaveBeenCalledTimes(1);
    expect(screen.queryByRole("button", { name: /buscar texto/i })).not.toBeInTheDocument();
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

  it("[SPEC:SCRUMCORE-210] toolbar abre modal de firmas y activa placement oficial", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    signatureAddEntryMock.mockClear();
    activateSignaturePlacementMock.mockClear();
    clienteApiGetMock.mockReset();

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /signature/i }));

    expect(screen.getByRole("dialog", { name: /firmas/i })).toBeInTheDocument();

    // Simular elección de firma y uso (modal emite SignatureFieldDefinition).
    await user.click(screen.getByRole("button", { name: /draw-pad/i }));
    await user.click(screen.getByRole("button", { name: /usar firma/i }));

    expect(signatureAddEntryMock).toHaveBeenCalledTimes(1);
    expect(activateSignaturePlacementMock).toHaveBeenCalledTimes(1);
  });

  it("[SPEC:SCRUMCORE-211] pestaña Firma personal renderiza y permite usar firma descargada", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    signatureAddEntryMock.mockClear();
    activateSignaturePlacementMock.mockClear();
    workflowPersonalSignatureMock.state.status = "ready";
    workflowPersonalSignatureMock.state.meta = {
      fileName: "firma.png",
      contentType: "image/png",
      expiresAt: "2026-05-15T00:00:00Z",
      urlTemporal: "/api/workflow/usuarios/firma-temporal/download/tok-1",
    };
    workflowPersonalSignatureMock.state.blobUrl = "data:image/png;base64,cG5n";
    workflowPersonalSignatureMock.state.imageData = new ArrayBuffer(3);

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /signature/i }));

    expect(screen.getByRole("dialog", { name: /firmas/i })).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /firma personal/i }));

    expect(workflowPersonalSignatureMock.load).toHaveBeenCalledTimes(1);
    expect(screen.getByLabelText(/vista previa firma personal/i)).toBeInTheDocument();
    expect(screen.getByText("firma.png")).toBeInTheDocument();

    expect(screen.queryByRole("button", { name: /usar firma personal/i })).not.toBeInTheDocument();
    expect(screen.queryByText(/data:image/i)).not.toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /usar firma/i }));

    expect(signatureAddEntryMock).toHaveBeenCalledTimes(1);
    expect(signatureAddEntryMock).toHaveBeenCalledWith({
      signature: expect.objectContaining({
        previewDataUrl: "data:image/png;base64,cG5n",
        imageMimeType: "image/png",
        imageData: workflowPersonalSignatureMock.state.imageData,
      }),
    });
    expect(activateSignaturePlacementMock).toHaveBeenCalledTimes(1);
    expect(workflowPersonalSignatureMock.clear).toHaveBeenCalled();
  });

  it("[SPEC:SCRUMCORE-211] pestaña Firma personal muestra estado de carga y pide load al entrar", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    workflowPersonalSignatureMock.state.status = "loading";

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /signature/i }));
    await user.click(screen.getByRole("button", { name: /firma personal/i }));

    expect(workflowPersonalSignatureMock.load).toHaveBeenCalledTimes(1);
    expect(screen.getByLabelText(/cargando firma personal/i)).toBeInTheDocument();
  });

  it("[SPEC:SCRUMCORE-213] paginaci\u00f3n permite escribir n\u00famero y navega con scrollToPage", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    scrollToPageMock.mockClear();
    scrollProvides = {
      scrollToPage: scrollToPageMock,
      scrollToNextPage: scrollToNextPageMock,
      scrollToPreviousPage: scrollToPreviousPageMock,
    };
    scrollState = { currentPage: 4, totalPages: 20 };

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByText("4/20"));

    const input = screen.getByRole("textbox");
    await user.clear(input);
    await user.type(input, "10{enter}");

    expect(scrollToPageMock).toHaveBeenCalledWith({ pageNumber: 10, behavior: "smooth", alignY: 0 });
  });

  it("[SPEC:SCRUMCORE-238] exportAnnotatedPdfPages retorna vacio sin anotaciones", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    const ref = createRef<AppVisorEmbedPdfRef>();

    render(<AppVisorEmbedPdf ref={ref} fileUrl="/demo/Radicado_2026_0413.pdf" />);

    await waitFor(() => expect(ref.current).not.toBeNull());

    await expect(ref.current!.exportAnnotatedPdfPages()).resolves.toEqual({
      hasAnnotations: false,
      annotatedPages: [],
      pageNumbers: [],
      pages: [],
    });
    expect(annotationCommitMock).not.toHaveBeenCalled();
    expect(saveAsCopyMock).not.toHaveBeenCalled();
  });

  it("[SPEC:SCRUMCORE-238] exportAnnotatedPdfPages hace commit y exporta PDFs por pagina anotada", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    annotationPagesState = {
      "1": ["ann-page-2"],
      "4": ["ann-page-5"],
      "3": [],
    };
    const ref = createRef<AppVisorEmbedPdfRef>();

    render(<AppVisorEmbedPdf ref={ref} fileUrl="/demo/Radicado_2026_0413.pdf" />);

    await waitFor(() => expect(ref.current).not.toBeNull());
    const result = await ref.current!.exportAnnotatedPdfPages({ calculateHashSha256: true });

    expect(annotationCommitMock).toHaveBeenCalledTimes(1);
    expect(saveAsCopyMock).not.toHaveBeenCalled();
    expect(pdfReplacementUtilsMock.extractSinglePagePdfs).not.toHaveBeenCalled();
    expect(extractPagesMock).toHaveBeenCalledTimes(2);
    expect(extractPagesMock).toHaveBeenNthCalledWith(1, expect.any(Object), [1]);
    expect(extractPagesMock).toHaveBeenNthCalledWith(2, expect.any(Object), [4]);
    expect(result.pageNumbers).toEqual([2, 5]);
    expect(result.hasAnnotations).toBe(true);
    expect(result.annotatedPages).toEqual([2, 5]);
    expect(result.pages).toEqual([
      expect.objectContaining({
        pageNumber: 2,
        fileName: "document-doc-1-page-2-annotated.pdf",
        sizeBytes: 6,
        hashSha256: "hash-6",
      }),
      expect.objectContaining({
        pageNumber: 5,
        fileName: "document-doc-1-page-5-annotated.pdf",
        sizeBytes: 6,
        hashSha256: "hash-6",
      }),
    ]);
  });

  it("[SPEC:SCRUMCORE-238] conserva OriginalPdfPassword solo tras password valida", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    documentContentRenderState = { isLoaded: false, isError: true, isLoading: false };
    const ref = createRef<AppVisorEmbedPdfRef>();

    render(<AppVisorEmbedPdf ref={ref} fileUrl="/demo/protected.pdf" />);

    const user = userEvent.setup();
    await user.type(screen.getByLabelText(/contraseña del documento/i), "secret");
    await user.click(screen.getByRole("button", { name: /continuar/i }));

    await waitFor(() => {
      expect(ref.current?.getOriginalPdfPassword()).toBe("secret");
    });
  });

  it("permite consumo desde index sin exponer detalles del engine", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateLoading);
    render(<AppVisorEmbedPdfFromIndex fileUrl="/demo/Radicado_2026_0413.pdf" />);
    expect(screen.getByText(/cargando motor pdf/i)).toBeInTheDocument();
  });
});
