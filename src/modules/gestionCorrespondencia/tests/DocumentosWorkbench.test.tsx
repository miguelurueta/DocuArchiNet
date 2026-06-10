import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { DocumentosWorkbench } from "../components/documentosWorkbench/DocumentosWorkbench";

const appTreeTableSpy = vi.fn();
const visualizarDocumentoSpy = vi.fn();
let mockDocumentoActivo: unknown = null;

type MockTableApi = {
  load: () => Promise<unknown>;
  loadChildren: () => Promise<unknown>;
  onSelectRow: (
    rowId: string,
  ) => Promise<{ documentResolveRequest: { IdDocumento: number; NombreGabinete: string }; rowId: string } | null>;
  onActionTriggered: (
    params: { actionId: string; rowId: string },
  ) => Promise<{ documentResolveRequest: { IdDocumento: number; NombreGabinete: string }; rowId: string } | null>;
  onSelectionChanged: (rowIds: string[]) => void;
  getTableColumns: () => undefined | Array<{ headerName?: string; field?: string }>;
  getColumns: () => undefined;
  totalDocumentsCount: number;
  selectedDocumentsCount: number;
};

let mockTableApi: MockTableApi;

vi.mock("../hooks/useGestionRespuestaDocumentosTable", () => ({
  useGestionRespuestaDocumentosTable: () => {
    return mockTableApi;
  },
}));

vi.mock("../../../app/Components/UI/AppDocumentViewerOrchestrator", () => ({
  useDocumentViewerOrchestrator: () => ({
    visualizarDocumento: (input: unknown) => visualizarDocumentoSpy(input),
    documentoActivo: mockDocumentoActivo,
    loading: false,
    error: null,
    reset: () => {},
    cancelCurrentRequest: () => {},
  }),
}));

vi.mock("../../../app/Components/UI/AppVisorEmbedPdf", () => ({
  AppVisorEmbedPdf: (props: { fileUrl?: string; onEmptyDocumentHintRequest?: () => void }) => (
    <div
      role="status"
      aria-label="Zona de documento"
      data-testid="app-visor-embedpdf-mock"
      data-file-url={props.fileUrl ?? ""}
    >
      <button type="button" onClick={props.onEmptyDocumentHintRequest}>
        Resaltar listado de documentos
      </button>
    </div>
  ),
}));

const toastErrorSpy = vi.fn();

vi.mock("react-toastify", () => ({
  toast: {
    warning: vi.fn(),
    error: (message: unknown, opts?: unknown) => toastErrorSpy(message, opts),
  },
}));

vi.mock("../../../app/Components/UI/AppTreeTable", () => ({
  AppTreeTable: (props: {
    onSelectRow?: (rowId: string) => void;
    onActionTriggered?: (params: { actionId: string; rowId: string }) => void;
    onSelectionChanged?: (rowIds: string[]) => void;
    tableLayoutMode?: string;
    tableColumns?: Array<{ headerName?: string; field?: string }>;
  }) => {
    appTreeTableSpy(props);
    return (
      <div data-testid="app-tree-table-mock">
        <button type="button" onClick={() => props.onSelectRow?.("r1")}>
          Select r1
        </button>
        <button type="button" onClick={() => props.onActionTriggered?.({ actionId: "ver_documento", rowId: "r1" })}>
          Action ver_documento
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
    toastErrorSpy.mockClear();
    mockDocumentoActivo = null;
    mockTableApi = {
      load: vi.fn(async () => ({ ok: true, rows: [] })),
      loadChildren: vi.fn(async () => ({ ok: true, rows: [] })),
      onSelectRow: vi.fn(async () => ({
        documentResolveRequest: { IdDocumento: 10, NombreGabinete: "G" },
        rowId: "r1",
      })),
      onActionTriggered: vi.fn(async () => ({
        documentResolveRequest: { IdDocumento: 11, NombreGabinete: "G" },
        rowId: "r1",
      })),
      onSelectionChanged: vi.fn(),
      getTableColumns: () => undefined,
      getColumns: () => undefined,
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
});
