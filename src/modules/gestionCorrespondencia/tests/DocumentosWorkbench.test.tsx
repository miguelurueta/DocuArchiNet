import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { useEffect } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { DocumentosWorkbench } from "../components/documentosWorkbench/DocumentosWorkbench";

const appTreeTableSpy = vi.fn();
const visualizarDocumentoSpy = vi.fn();
const documentViewerResetSpy = vi.fn();
const exportAnnotatedPdfPagesSpy = vi.fn();
const getOriginalPdfPasswordSpy = vi.fn();
const markAnnotatedPagesPersistedSpy = vi.fn();
const initUploadTemporalPdfAnotadoSpy = vi.fn();
const uploadTemporalChunkSpy = vi.fn();
const completeUploadTemporalSpy = vi.fn();
const statusUploadTemporalSpy = vi.fn();
const reemplazarPaginasPdfAnotadasSpy = vi.fn();
const cancelUploadTemporalSpy = vi.fn();
let mockDocumentoActivo: unknown = null;
let mockDocumentosRefreshKey = 0;

type MockTableApi = {
  load: () => Promise<unknown>;
  loadChildren: () => Promise<unknown>;
  onSelectRow: (
    rowId: string,
  ) => Promise<{ documentResolveRequest: { IdDocumento: number; NombreGabinete: string }; rowId: string } | null>;
  onActionTriggered: (
    params: { actionId: string; rowId: string },
  ) => Promise<
    | { documentResolveRequest: { IdDocumento: number; NombreGabinete: string }; rowId: string }
    | { success: boolean; severity?: "success" | "warning" | "error"; message?: string; requestId?: string }
    | null
  >;
  onSelectionChanged: (rowIds: string[]) => void;
  getTableColumns: () => undefined | Array<{ headerName?: string; field?: string }>;
  getColumns: () => undefined;
  totalDocumentsCount: number;
  selectedDocumentsCount: number;
  getWorkbenchContext: () => { nombreGabinete?: string; radicado?: string };
};

let mockTableApi: MockTableApi;

vi.mock("../hooks/useGestionRespuestaDocumentosTable", () => ({
  useGestionRespuestaDocumentosTable: () => {
    return mockTableApi;
  },
}));

vi.mock("../hooks/useGestionRespuestaDocumentos", () => ({
  useGestionRespuestaDocumentos: () => ({
    documentosRefreshKey: mockDocumentosRefreshKey,
  }),
}));

vi.mock("../../../app/Components/UI/AppDocumentViewerOrchestrator", () => ({
  useDocumentViewerOrchestrator: () => ({
    visualizarDocumento: (input: unknown) => visualizarDocumentoSpy(input),
    documentoActivo: mockDocumentoActivo,
    loading: false,
    error: null,
    reset: () => documentViewerResetSpy(),
    cancelCurrentRequest: () => {},
  }),
}));

vi.mock("../../../app/Components/UI/AppVisorEmbedPdf", async () => {
  const ReactRuntime = await import("react");

  return {
    AppVisorEmbedPdf: ReactRuntime.forwardRef(
      (
        props: {
          fileUrl?: string;
          onEmptyDocumentHintRequest?: () => void;
          onSaveAnnotatedPages?: () => void;
          isSaveAnnotatedPagesDisabled?: boolean;
          saveAnnotatedPagesProgress?: number;
        },
        ref,
      ) => {
        ReactRuntime.useImperativeHandle(ref, () => ({
          load: vi.fn(async () => ({
            ok: true,
            fileUrl: props.fileUrl ?? null,
            permissionsRaw: {},
            permissionsEffective: {},
            isElectronicallySigned: false,
            permissionStatus: "not_required",
            errors: [],
          })),
          reset: vi.fn(),
          cancelCurrentLoad: vi.fn(),
          getOriginalPdfPassword: getOriginalPdfPasswordSpy,
          markAnnotatedPagesPersisted: markAnnotatedPagesPersistedSpy,
          exportAnnotatedPdfPages: exportAnnotatedPdfPagesSpy,
        }));

        return (
          <div
            role="status"
            aria-label="Zona de documento"
            data-testid="app-visor-embedpdf-mock"
            data-file-url={props.fileUrl ?? ""}
          >
            <button type="button" onClick={props.onEmptyDocumentHintRequest}>
              Resaltar listado de documentos
            </button>
            <button
              type="button"
              disabled={props.isSaveAnnotatedPagesDisabled}
              onClick={props.onSaveAnnotatedPages}
              data-progress={props.saveAnnotatedPagesProgress ?? ""}
            >
              Guardar paginas anotadas
            </button>
          </div>
        );
      },
    ),
  };
});

vi.mock("../../../app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service", () => ({
  initUploadTemporalPdfAnotado: (...args: unknown[]) => initUploadTemporalPdfAnotadoSpy(...args),
  uploadTemporalChunk: (...args: unknown[]) => uploadTemporalChunkSpy(...args),
  completeUploadTemporal: (...args: unknown[]) => completeUploadTemporalSpy(...args),
  statusUploadTemporal: (...args: unknown[]) => statusUploadTemporalSpy(...args),
  reemplazarPaginasPdfAnotadas: (...args: unknown[]) => reemplazarPaginasPdfAnotadasSpy(...args),
  cancelUploadTemporal: (...args: unknown[]) => cancelUploadTemporalSpy(...args),
}));

vi.mock("../../../app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.types", () => ({
  ReemplazoPaginasPdfAnotadasError: class ReemplazoPaginasPdfAnotadasError extends Error {
    field?: string;
  },
}));

const toastErrorSpy = vi.fn();
const toastWarningSpy = vi.fn();
const toastSuccessSpy = vi.fn();

vi.mock("react-toastify", () => ({
  toast: {
    warning: (message: unknown, opts?: unknown) => toastWarningSpy(message, opts),
    success: (message: unknown, opts?: unknown) => toastSuccessSpy(message, opts),
    error: (message: unknown, opts?: unknown) => toastErrorSpy(message, opts),
  },
}));

vi.mock("../../../app/Components/UI/AppTreeTable", () => ({
  AppTreeTable: (props: {
    load?: () => Promise<unknown>;
    onSelectRow?: (rowId: string) => void;
    onActionTriggered?: (params: { actionId: string; rowId: string }) => void;
    onSelectionChanged?: (rowIds: string[]) => void;
    tableLayoutMode?: string;
    tableColumns?: Array<{ headerName?: string; field?: string }>;
  }) => {
    useEffect(() => {
      void props.load?.();
      // Solo al montar: el test valida remount por key, no rerender normal.
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, []);

    appTreeTableSpy(props);
    return (
      <div data-testid="app-tree-table-mock">
        <button type="button" onClick={() => props.onSelectRow?.("r1")}>
          Select r1
        </button>
        <button type="button" onClick={() => props.onActionTriggered?.({ actionId: "ver_documento", rowId: "r1" })}>
          Action ver_documento
        </button>
        <button type="button" onClick={() => props.onActionTriggered?.({ actionId: "eliminar_item", rowId: "r1" })}>
          Action eliminar_item
        </button>
        <button type="button" onClick={() => props.onSelectionChanged?.(["r1", "r2"])}>
          Select rows
        </button>
      </div>
    );
  },
}));

const TABLET_QUERY = "(max-width: 1024px)";
const MOBILE_QUERY = "(max-width: 768px)";

type MatchMediaMap = Record<string, boolean>;

const createMatchMedia = (matches: MatchMediaMap) => (query: string) => ({
  matches: matches[query] ?? false,
  media: query,
  onchange: null,
  addEventListener: () => {},
  removeEventListener: () => {},
  dispatchEvent: () => false,
});

describe("[SPEC:APPTREETABLE-217] DocumentosWorkbench", () => {
  beforeEach(() => {
    appTreeTableSpy.mockClear();
    visualizarDocumentoSpy.mockClear();
    documentViewerResetSpy.mockClear();
    exportAnnotatedPdfPagesSpy.mockReset();
    getOriginalPdfPasswordSpy.mockReset();
    markAnnotatedPagesPersistedSpy.mockReset();
    initUploadTemporalPdfAnotadoSpy.mockReset();
    uploadTemporalChunkSpy.mockReset();
    completeUploadTemporalSpy.mockReset();
    statusUploadTemporalSpy.mockReset();
    reemplazarPaginasPdfAnotadasSpy.mockReset();
    cancelUploadTemporalSpy.mockReset();
    toastErrorSpy.mockClear();
    toastWarningSpy.mockClear();
    toastSuccessSpy.mockClear();
    mockDocumentoActivo = null;
    mockDocumentosRefreshKey = 0;
    getOriginalPdfPasswordSpy.mockReturnValue(undefined);
    exportAnnotatedPdfPagesSpy.mockResolvedValue({
      hasAnnotations: true,
      annotatedPages: [2],
      pageNumbers: [2],
      pages: [
        {
          pageNumber: 2,
          fileName: "document-10-page-2-annotated.pdf",
          blob: new Blob(["pdf"], { type: "application/pdf" }),
          sizeBytes: 3,
          hashSha256: "hash-page-2",
        },
      ],
    });
    initUploadTemporalPdfAnotadoSpy.mockResolvedValue({
      RutaTemporalId: "usr_page_2",
      ArchivoTemporalId: "af_page_2.pdf",
      ChunkSizeBytes: 1048576,
      Estado: "IN_PROGRESS",
    });
    uploadTemporalChunkSpy.mockResolvedValue({ chunkIndex: 0 });
    completeUploadTemporalSpy.mockResolvedValue({ Estado: "COMPLETED" });
    statusUploadTemporalSpy.mockResolvedValue({
      Estado: "COMPLETED",
      ChunksRecibidos: 1,
      ChunksPendientes: 0,
      TamanoRecibidoBytes: 3,
    });
    reemplazarPaginasPdfAnotadasSpy.mockResolvedValue({
      IdDocumento: 10,
      NombreGabinete: "G",
      PaginasReemplazadas: [2],
      RutaArchivoFinal: "D:/x.pdf",
      RutaRespaldo: "D:/backup.pdf",
      TamanoAnteriorBytes: 1,
      TamanoNuevoBytes: 2,
      HashAnteriorSha256: "old",
      HashNuevoSha256: "new",
      RequestId: "req-1",
    });
    mockTableApi = {
      load: vi.fn(async () => ({ ok: true, rows: [] })),
      loadChildren: vi.fn(async () => ({ ok: true, rows: [] })),
      onSelectRow: vi.fn(async () => ({
        documentResolveRequest: { IdDocumento: 10, NombreGabinete: "G" },
        rowId: "r1",
      })),
      onActionTriggered: vi.fn(async ({ actionId }: { actionId: string; rowId: string }) =>
        actionId === "eliminar_item"
          ? {
              success: true,
              message: "Documento eliminado correctamente.",
              severity: "success",
              requestId: "req-delete",
            }
          : {
              documentResolveRequest: { IdDocumento: 11, NombreGabinete: "G" },
              rowId: "r1",
            },
      ),
      onSelectionChanged: vi.fn(),
      getTableColumns: () => undefined,
      getColumns: () => undefined,
      getWorkbenchContext: () => ({ nombreGabinete: "G", radicado: "RAD-1" }),
      totalDocumentsCount: 25,
      selectedDocumentsCount: 0,
    };

    window.matchMedia = createMatchMedia({
      [TABLET_QUERY]: false,
      [MOBILE_QUERY]: false,
    }) as unknown as typeof window.matchMedia;

    Object.defineProperty(window, "innerWidth", { value: 1440, configurable: true });
    Object.defineProperty(navigator, "maxTouchPoints", { value: 0, configurable: true });
  });

  it("[SPEC:SCRUMCORE-202] renderiza estructura base con visor embebido", () => {
    render(<DocumentosWorkbench />);

    expect(screen.getByTestId("documentos-workbench")).toBeInTheDocument();
    expect(screen.getByRole("status", { name: "Zona de documento" })).toBeInTheDocument();
    expect(screen.getByTestId("app-visor-embedpdf-mock")).toBeInTheDocument();
    expect(screen.getAllByRole("button", { name: /Ocultar documentos/i }).length).toBeGreaterThan(0);
    expect(screen.getByTestId("app-tree-table-mock")).toBeInTheDocument();
    expect(appTreeTableSpy).toHaveBeenCalledWith(expect.objectContaining({ tableLayoutMode: "fill" }));
  });

  it("remonta el AppTreeTable cuando cambia documentosRefreshKey para refrescar el listado", async () => {
    const view = render(<DocumentosWorkbench idTareaWf={933} />);

    await waitFor(() => {
      expect(mockTableApi.load).toHaveBeenCalledTimes(1);
    });

    mockDocumentosRefreshKey = 1;
    view.rerender(<DocumentosWorkbench idTareaWf={933} />);

    await waitFor(() => {
      expect(mockTableApi.load).toHaveBeenCalledTimes(2);
    });
  });

  it("no remonta el AppTreeTable despues de una accion de documento", async () => {
    render(<DocumentosWorkbench idTareaWf={933} />);

    await waitFor(() => {
      expect(mockTableApi.load).toHaveBeenCalledTimes(1);
    });

    fireEvent.click(screen.getByRole("button", { name: "Action eliminar_item" }));

    await waitFor(() => {
      expect(mockTableApi.onActionTriggered).toHaveBeenCalledWith({ actionId: "eliminar_item", rowId: "r1" });
    });
    expect(mockTableApi.load).toHaveBeenCalledTimes(1);
  });

  it("limpia el visor y el row activo cuando se elimina la fila abierta", async () => {
    render(<DocumentosWorkbench idTareaWf={933} />);

    await waitFor(() => {
      expect(mockTableApi.load).toHaveBeenCalledTimes(1);
    });

    fireEvent.click(screen.getByRole("button", { name: "Select r1" }));

    await waitFor(() => {
      expect(visualizarDocumentoSpy).toHaveBeenCalledWith(
        expect.objectContaining({ documentId: 10, nombreGabinete: "G" }),
      );
    });

    fireEvent.click(screen.getByRole("button", { name: "Action eliminar_item" }));

    await waitFor(() => {
      expect(documentViewerResetSpy).toHaveBeenCalledTimes(1);
      expect(mockTableApi.onActionTriggered).toHaveBeenCalledWith({ actionId: "eliminar_item", rowId: "r1" });
    });

    await waitFor(() => {
      expect(appTreeTableSpy).toHaveBeenCalledWith(expect.objectContaining({ activeRowId: undefined }));
    });
  });

  it("muestra warning amigable cuando el backend deshabilita el delete", async () => {
    mockTableApi.onActionTriggered = vi.fn(async () => ({
      success: false,
      severity: "warning",
      message: "Delete feature is disabled: DELETE_STORAGE_ENGINE is disabled for this environment.",
      requestId: "req-delete",
    }));

    render(<DocumentosWorkbench idTareaWf={933} />);

    await waitFor(() => {
      expect(mockTableApi.load).toHaveBeenCalledTimes(1);
    });

    fireEvent.click(screen.getByRole("button", { name: "Action eliminar_item" }));

    await waitFor(() => {
      expect(toastWarningSpy).toHaveBeenCalledWith(
        "No es posible eliminar este documento en este momento.",
        expect.objectContaining({
          position: "top-right",
          autoClose: 6000,
          closeOnClick: true,
        }),
      );
    });
    expect(toastErrorSpy).not.toHaveBeenCalled();
    expect(toastSuccessSpy).not.toHaveBeenCalled();
  });

  it("resalta temporalmente el listado cuando el visor vacio solicita ayuda de seleccion", async () => {
    render(<DocumentosWorkbench />);

    fireEvent.click(screen.getByRole("button", { name: "Resaltar listado de documentos" }));

    await waitFor(() => {
      expect(screen.getByLabelText("Listado de documentos")).toHaveAttribute("data-document-hint-active", "true");
    });
  });

  it("[SPEC:SCRUMCORE-227] row_click invoca visualizarDocumento usando DocumentResolveRequest (handler unificado)", async () => {
    render(<DocumentosWorkbench />);

    fireEvent.click(screen.getByRole("button", { name: "Select r1" }));

    expect(mockTableApi.onSelectRow).toHaveBeenCalledWith("r1");
    await waitFor(() => {
      expect(visualizarDocumentoSpy).toHaveBeenCalledWith(
        expect.objectContaining({ documentId: 10, nombreGabinete: "G" }),
      );
    });
  });

  it("[SPEC:SCRUMCORE-227] si falla action/ver_documento, no invoca visualizarDocumento y preserva activeRowId", async () => {
    mockTableApi.onSelectRow.mockResolvedValueOnce(null);

    render(<DocumentosWorkbench />);

    fireEvent.click(screen.getByRole("button", { name: "Select r1" }));

    await waitFor(() => {
      expect(visualizarDocumentoSpy).not.toHaveBeenCalled();
    });

    // El rowId activo no debe actualizarse en fallas de action.
    expect(appTreeTableSpy).toHaveBeenCalledWith(expect.objectContaining({ activeRowId: undefined }));
  });

  it("[SPEC:SCRUMCORE-227] menu_action ver_documento converge al mismo flujo y llama visualizarDocumento", async () => {
    render(<DocumentosWorkbench />);

    fireEvent.click(screen.getByRole("button", { name: "Action ver_documento" }));

    expect(mockTableApi.onSelectRow).toHaveBeenCalledWith("r1");
    await waitFor(() => {
      expect(visualizarDocumentoSpy).toHaveBeenCalledWith(
        expect.objectContaining({ documentId: 10, nombreGabinete: "G" }),
      );
    });
  });

  it("[SPEC:SCRUMCORE-227] notifica error de resolve por sistema global de notificaciones (toast)", async () => {
    const view = render(<DocumentosWorkbench />);

    fireEvent.click(screen.getByRole("button", { name: "Select r1" }));

    await waitFor(() => {
      expect(visualizarDocumentoSpy).toHaveBeenCalled();
    });

    mockDocumentoActivo = {
      documentId: 10,
      nombreGabinete: "G",
      fileUrl: null,
      contentType: null,
      isPdf: false,
      isElectronicallySigned: null,
      firmaCheckStatus: "not_required",
      resolveStatus: "failed",
      errors: ["No existe carpeta física del documento"],
    };

    view.rerender(<DocumentosWorkbench />);

    expect(toastErrorSpy).toHaveBeenCalledWith(
      "No existe carpeta física del documento",
      expect.objectContaining({ autoClose: false }),
    );
  });

  it("[SPEC:SCRUMCORE-238] guarda paginas anotadas sin recargar documento activo", async () => {
    mockDocumentoActivo = {
      documentId: 10,
      nombreGabinete: "G",
      fileUrl: "/tmp/doc.pdf",
      contentType: "application/pdf",
      viewerKind: "pdf",
      isPdf: true,
      isElectronicallySigned: false,
      firmaCheckStatus: "resolved",
      resolveStatus: "resolved",
      errors: [],
      documentKey: "G:10",
      attemptId: 1,
    };

    render(<DocumentosWorkbench idTareaWf={123} />);
    expect(screen.getByTestId("app-visor-embedpdf-mock")).toHaveAttribute(
      "data-file-url",
      "/tmp/doc.pdf?_dvAttempt=1",
    );

    fireEvent.click(screen.getByRole("button", { name: "Guardar paginas anotadas" }));

    await waitFor(() => {
      expect(reemplazarPaginasPdfAnotadasSpy).toHaveBeenCalledWith(
        expect.objectContaining({
          NombreGabinete: "G",
          IdDocumento: 10,
          RutaTemporalId: "usr_page_2",
          Radicado: "RAD-1",
          IdTareaWorkflow: 123,
          Paginas: [
            expect.objectContaining({
              PageNumber: 2,
              RutaTemporalId: "usr_page_2",
              ArchivoTemporalId: "af_page_2.pdf",
              ContentType: "application/pdf",
              HashSha256Esperado: "hash-page-2",
            }),
          ],
        }),
        expect.objectContaining({ signal: expect.any(AbortSignal) }),
      );
    });

    expect(uploadTemporalChunkSpy).toHaveBeenCalledWith(
      expect.objectContaining({ chunkIndex: 0, totalChunks: 1, chunk: expect.any(Blob) }),
      expect.objectContaining({ signal: expect.any(AbortSignal) }),
    );
    expect(markAnnotatedPagesPersistedSpy).toHaveBeenCalledTimes(1);
    expect(cancelUploadTemporalSpy).not.toHaveBeenCalled();
    expect(visualizarDocumentoSpy).not.toHaveBeenCalled();
  });

  it("[SPEC:SCRUMCORE-238] limita chunks frontend a 768KB para paginas anotadas grandes", async () => {
    const largePdf = new Blob([new Uint8Array(1_639_741)], { type: "application/pdf" });
    exportAnnotatedPdfPagesSpy.mockResolvedValueOnce({
      hasAnnotations: true,
      annotatedPages: [1],
      pageNumbers: [1],
      pages: [
        {
          pageNumber: 1,
          fileName: "document-10-page-1-annotated.pdf",
          blob: largePdf,
          sizeBytes: largePdf.size,
          hashSha256: "hash-page-1",
        },
      ],
    });
    mockDocumentoActivo = {
      documentId: 10,
      nombreGabinete: "G",
      fileUrl: "/tmp/doc.pdf",
      contentType: "application/pdf",
      viewerKind: "pdf",
      isPdf: true,
      isElectronicallySigned: false,
      firmaCheckStatus: "resolved",
      resolveStatus: "resolved",
      errors: [],
      documentKey: "G:10",
      attemptId: 1,
    };

    render(<DocumentosWorkbench idTareaWf={123} />);
    fireEvent.click(screen.getByRole("button", { name: "Guardar paginas anotadas" }));

    await waitFor(() => {
      expect(uploadTemporalChunkSpy).toHaveBeenCalledTimes(3);
    });

    expect(initUploadTemporalPdfAnotadoSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        NombreOriginal: "document-10-page-1-annotated.pdf",
        TamanoBytes: 1_639_741,
        NumeroChunks: 3,
      }),
      expect.objectContaining({ signal: expect.any(AbortSignal) }),
    );
    expect(uploadTemporalChunkSpy).toHaveBeenNthCalledWith(
      1,
      expect.objectContaining({ chunkIndex: 0, totalChunks: 3, chunk: expect.objectContaining({ size: 786_432 }) }),
      expect.objectContaining({ signal: expect.any(AbortSignal) }),
    );
    expect(uploadTemporalChunkSpy).toHaveBeenNthCalledWith(
      3,
      expect.objectContaining({ chunkIndex: 2, totalChunks: 3, chunk: expect.objectContaining({ size: 66_877 }) }),
      expect.objectContaining({ signal: expect.any(AbortSignal) }),
    );
  });

  it("[SPEC:SCRUMCORE-238] envia OriginalPdfPassword solo en reemplazo final cuando existe en memoria", async () => {
    getOriginalPdfPasswordSpy.mockReturnValue("secret");
    mockDocumentoActivo = {
      documentId: 10,
      nombreGabinete: "G",
      fileUrl: "/tmp/doc.pdf",
      contentType: "application/pdf",
      viewerKind: "pdf",
      isPdf: true,
      isElectronicallySigned: false,
      firmaCheckStatus: "resolved",
      resolveStatus: "resolved",
      errors: [],
      documentKey: "G:10",
      attemptId: 1,
    };

    render(<DocumentosWorkbench idTareaWf={123} />);

    fireEvent.click(screen.getByRole("button", { name: "Guardar paginas anotadas" }));

    await waitFor(() => {
      expect(reemplazarPaginasPdfAnotadasSpy).toHaveBeenCalledWith(
        expect.objectContaining({ OriginalPdfPassword: "secret" }),
        expect.anything(),
      );
    });
    expect(initUploadTemporalPdfAnotadoSpy).not.toHaveBeenCalledWith(
      expect.objectContaining({ OriginalPdfPassword: expect.anything() }),
      expect.anything(),
    );
  });

  it("[SPEC:SCRUMCORE-238] bloquea documento firmado sin exportar ni llamar APIs", async () => {
    mockDocumentoActivo = {
      documentId: 10,
      nombreGabinete: "G",
      fileUrl: "/tmp/doc.pdf",
      contentType: "application/pdf",
      viewerKind: "pdf",
      isPdf: true,
      isElectronicallySigned: true,
      firmaCheckStatus: "resolved",
      resolveStatus: "resolved",
      errors: [],
      documentKey: "G:10",
      attemptId: 1,
    };

    render(<DocumentosWorkbench idTareaWf={123} />);

    expect(screen.getByRole("button", { name: "Guardar paginas anotadas" })).toBeDisabled();
    expect(exportAnnotatedPdfPagesSpy).not.toHaveBeenCalled();
    expect(reemplazarPaginasPdfAnotadasSpy).not.toHaveBeenCalled();
  });

  it("[SPEC:SCRUMCORE-238] no llama APIs si no hay anotaciones", async () => {
    exportAnnotatedPdfPagesSpy.mockResolvedValueOnce({
      hasAnnotations: false,
      annotatedPages: [],
      pageNumbers: [],
      pages: [],
    });
    mockDocumentoActivo = {
      documentId: 10,
      nombreGabinete: "G",
      fileUrl: "/tmp/doc.pdf",
      contentType: "application/pdf",
      viewerKind: "pdf",
      isPdf: true,
      isElectronicallySigned: false,
      firmaCheckStatus: "resolved",
      resolveStatus: "resolved",
      errors: [],
      documentKey: "G:10",
      attemptId: 1,
    };

    render(<DocumentosWorkbench idTareaWf={123} />);

    fireEvent.click(screen.getByRole("button", { name: "Guardar paginas anotadas" }));

    await waitFor(() => {
      expect(toastErrorSpy).toHaveBeenCalledWith("No hay paginas anotadas para reemplazar.", undefined);
    });
    expect(initUploadTemporalPdfAnotadoSpy).not.toHaveBeenCalled();
    expect(reemplazarPaginasPdfAnotadasSpy).not.toHaveBeenCalled();
  });

  it("[SPEC:SCRUMCORE-238] limpia temporal best-effort si falla reemplazo final", async () => {
    reemplazarPaginasPdfAnotadasSpy.mockRejectedValueOnce(new Error("fallo final"));
    mockDocumentoActivo = {
      documentId: 10,
      nombreGabinete: "G",
      fileUrl: "/tmp/doc.pdf",
      contentType: "application/pdf",
      viewerKind: "pdf",
      isPdf: true,
      isElectronicallySigned: false,
      firmaCheckStatus: "resolved",
      resolveStatus: "resolved",
      errors: [],
      documentKey: "G:10",
      attemptId: 1,
    };

    render(<DocumentosWorkbench idTareaWf={123} />);

    fireEvent.click(screen.getByRole("button", { name: "Guardar paginas anotadas" }));

    await waitFor(() => {
      expect(cancelUploadTemporalSpy).toHaveBeenCalledWith({
        rutaTemporalId: "usr_page_2",
        archivoTemporalId: "af_page_2.pdf",
      });
    });
    expect(toastErrorSpy).toHaveBeenCalledWith("fallo final", undefined);
  });
});
