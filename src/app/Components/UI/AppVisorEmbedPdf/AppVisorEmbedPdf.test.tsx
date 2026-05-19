import type React from "react";
import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it, vi } from "vitest";

import { AppVisorEmbedPdf } from "./AppVisorEmbedPdf";
import { AppVisorEmbedPdf as AppVisorEmbedPdfFromIndex } from "./index";

const clienteApiGetMock = vi.fn();
vi.mock("../../../../../api/Clienteaxios", () => ({
  default: {
    defaults: { baseURL: "https://localhost:7101" },
    get: (...args: unknown[]) => clienteApiGetMock(...args),
  },
}));

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
let documentContentRenderState: { isLoaded: boolean; isError: boolean; isLoading: boolean } = {
  isLoaded: true,
  isError: false,
  isLoading: false,
};

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
}));

vi.mock("@embedpdf/plugin-annotation/react", () => ({
  AnnotationLayer: ({ documentId, pageIndex }: { documentId: string; pageIndex: number }) => (
    <div data-testid="annotation-layer" data-document-id={documentId} data-page-index={pageIndex} />
  ),
  useAnnotation: () => ({
    state: { selectedUids: [] },
    provides: {
      deleteAnnotation: vi.fn(),
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
    renderPage: (props: { pageIndex: number }) => React.ReactNode;
  }) => <div data-testid="scroller">{renderPage({ pageIndex: 0 })}</div>,
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
    clienteApiGetMock.mockReset();

    const createObjectUrlSpy = vi
      .spyOn(URL, "createObjectURL")
      .mockImplementation(() => "blob:personal-sig");
    const revokeObjectUrlSpy = vi.spyOn(URL, "revokeObjectURL").mockImplementation(() => undefined);

    clienteApiGetMock.mockImplementation((url: string) => {
      if (url === "/api/workflow/usuarios/firma-temporal") {
        return Promise.resolve({
          data: {
            success: true,
            message: "YES",
            data: {
              IdUsuarioWorkflow: 141,
              FileName: "firma.png",
              ContentType: "image/png",
              RelativePath: "signatures/firma.png",
              UrlTemporal: "/api/workflow/usuarios/firma-temporal/download/tok-1",
              ExpiresAt: "2026-05-15T00:00:00Z",
            },
            errors: [],
          },
        });
      }

      if (String(url).includes("/api/workflow/usuarios/firma-temporal/download/")) {
        return Promise.resolve({
          data: new Blob(["png"], { type: "image/png" }),
        });
      }

      throw new Error(`Unexpected GET: ${url}`);
    });

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /signature/i }));

    expect(screen.getByRole("dialog", { name: /firmas/i })).toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /firma personal/i }));

    await waitFor(() =>
      expect(screen.getByRole("img", { name: /firma personal/i })).toBeInTheDocument()
    );

    expect(screen.queryByRole("button", { name: /usar firma personal/i })).not.toBeInTheDocument();
    expect(screen.queryByText(/blob:/i)).not.toBeInTheDocument();

    await user.click(screen.getByRole("button", { name: /usar firma/i }));

    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(signatureAddEntryMock).toHaveBeenCalledTimes(1);
    expect(activateSignaturePlacementMock).toHaveBeenCalledTimes(1);

    // Cleanup al usar y resetear modal (revoca objectURL y limpia estado)
    expect(revokeObjectUrlSpy).toHaveBeenCalled();

    createObjectUrlSpy.mockRestore();
    revokeObjectUrlSpy.mockRestore();
  });

  it("[SPEC:SCRUMCORE-211] download 404 reintenta metadata y descarga una vez", async () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateReady);
    clienteApiGetMock.mockReset();

    const createObjectUrlSpy = vi
      .spyOn(URL, "createObjectURL")
      .mockImplementation(() => "blob:personal-sig");
    const revokeObjectUrlSpy = vi.spyOn(URL, "revokeObjectURL").mockImplementation(() => undefined);

    let metaCall = 0;
    let downloadCall = 0;

    clienteApiGetMock.mockImplementation((url: string) => {
      if (url === "/api/workflow/usuarios/firma-temporal") {
        metaCall += 1;
        const tok = metaCall === 1 ? "tok-1" : "tok-2";
        return Promise.resolve({
          data: {
            success: true,
            message: "YES",
            data: {
              IdUsuarioWorkflow: 141,
              FileName: `firma-${tok}.png`,
              ContentType: "image/png",
              RelativePath: `signatures/firma-${tok}.png`,
              UrlTemporal: `/api/workflow/usuarios/firma-temporal/download/${tok}`,
              ExpiresAt: "2026-05-15T00:00:00Z",
            },
            errors: [],
          },
        });
      }

      if (String(url).includes("/api/workflow/usuarios/firma-temporal/download/")) {
        downloadCall += 1;
        if (downloadCall === 1) {
          return Promise.reject({ response: { status: 404 } });
        }
        return Promise.resolve({
          data: new Blob(["png"], { type: "image/png" }),
        });
      }

      throw new Error(`Unexpected GET: ${url}`);
    });

    render(<AppVisorEmbedPdf fileUrl="/demo/Radicado_2026_0413.pdf" />);

    const user = userEvent.setup();
    await user.click(screen.getByRole("button", { name: /signature/i }));
    await user.click(screen.getByRole("button", { name: /firma personal/i }));

    await waitFor(() =>
      expect(screen.getByRole("button", { name: /usar firma/i })).toBeInTheDocument()
    );

    expect(metaCall).toBe(2);
    expect(downloadCall).toBe(2);
    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);

    // Cleanup en cierre del modal
    await user.click(screen.getByRole("button", { name: /cerrar modal de firmas/i }));
    expect(revokeObjectUrlSpy).toHaveBeenCalled();

    createObjectUrlSpy.mockRestore();
    revokeObjectUrlSpy.mockRestore();
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

  it("permite consumo desde index sin exponer detalles del engine", () => {
    useEmbedPdfEngineMock.mockReturnValue(engineStateLoading);
    render(<AppVisorEmbedPdfFromIndex fileUrl="/demo/Radicado_2026_0413.pdf" />);
    expect(screen.getByText(/cargando motor pdf/i)).toBeInTheDocument();
  });
});
