import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { DocumentosWorkbench } from "../components/documentosWorkbench/DocumentosWorkbench";

const appTreeTableSpy = vi.fn();

type MockTableApi = {
  load: () => Promise<unknown>;
  loadChildren: () => Promise<unknown>;
  onSelectRow: (rowId: string) => Promise<{ fileUrl?: string; rowId: string } | null>;
  onActionTriggered: (params: {
    actionId: string;
    rowId: string;
  }) => Promise<{ fileUrl?: string; rowId: string } | null>;
  onSelectionChanged: (rowIds: string[]) => void;
  getTableColumns: () => undefined;
  getColumns: () => undefined;
  totalDocumentsCount: number;
  selectedDocumentsCount: number;
};

let mockTableApi: MockTableApi;

vi.mock("../hooks/useGestionRespuestaDocumentosTable", () => ({
  useGestionRespuestaDocumentosTable: () => mockTableApi,
}));

vi.mock("../../../app/Components/UI/AppVisorEmbedPdf", () => ({
  AppVisorEmbedPdf: (props: { fileUrl?: string }) => (
    <div
      role="status"
      aria-label="Zona de documento"
      data-testid="app-visor-embedpdf-mock"
      data-file-url={props.fileUrl ?? ""}
    />
  ),
}));

vi.mock("../../../app/Components/UI/AppTreeTable", () => ({
  AppTreeTable: (props: {
    onSelectRow?: (rowId: string) => void;
    onActionTriggered?: (params: { actionId: string; rowId: string }) => void;
    onSelectionChanged?: (rowIds: string[]) => void;
    tableLayoutMode?: string;
  }) => {
    appTreeTableSpy(props);
    return (
      <div data-testid="app-tree-table-mock">
        <button type="button" onClick={() => props.onSelectRow?.("r1")}>
          Select r1
        </button>
        <button
          type="button"
          onClick={() => props.onActionTriggered?.({ actionId: "ver_documento", rowId: "r1" })}
        >
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
    mockTableApi = {
      load: vi.fn(async () => ({ ok: true, rows: [] })),
      loadChildren: vi.fn(async () => ({ ok: true, rows: [] })),
      onSelectRow: vi.fn(async () => ({ fileUrl: "http://example.test/doc.pdf", rowId: "r1" })),
      onActionTriggered: vi.fn(async () => ({ fileUrl: "http://example.test/doc2.pdf", rowId: "r1" })),
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
    expect(screen.getByRole("button", { name: /Ocultar Visualizar documentos/i })).toBeInTheDocument();
    expect(screen.getByTestId("app-tree-table-mock")).toBeInTheDocument();
    expect(appTreeTableSpy).toHaveBeenCalledWith(
      expect.objectContaining({ tableLayoutMode: "fill" }),
    );
  });

  it("muestra contador documental total", () => {
    render(<DocumentosWorkbench />);
    expect(screen.getByText("Documentos (25)")).toBeInTheDocument();
  });

  it("muestra contador de seleccionados cuando hay seleccion", () => {
    mockTableApi.selectedDocumentsCount = 2;
    render(<DocumentosWorkbench />);
    expect(screen.getByText("Documentos (25) · Seleccionados (2)")).toBeInTheDocument();
  });

  it("propaga el cambio de seleccion desde AppTreeTable al hook", () => {
    render(<DocumentosWorkbench />);
    fireEvent.click(screen.getByRole("button", { name: "Select rows" }));
    expect(mockTableApi.onSelectionChanged).toHaveBeenCalledWith(["r1", "r2"]);
  });

  it("actualiza fileUrl del visor al seleccionar una fila", async () => {
    render(<DocumentosWorkbench />);

    fireEvent.click(screen.getByRole("button", { name: "Select r1" }));

    const visor = await screen.findByTestId("app-visor-embedpdf-mock");
    expect(visor).toHaveAttribute("data-file-url", "http://example.test/doc.pdf");
  });

  it("actualiza fileUrl del visor al disparar accion ver_documento", async () => {
    render(<DocumentosWorkbench />);

    fireEvent.click(screen.getByRole("button", { name: "Action ver_documento" }));

    const visor = await screen.findByTestId("app-visor-embedpdf-mock");
    expect(visor).toHaveAttribute("data-file-url", "http://example.test/doc2.pdf");
  });

  it("permite colapsar el rail", () => {
    render(<DocumentosWorkbench />);
    const toggle = screen.getByRole("button", {
      name: /Ocultar Visualizar documentos/i,
    });
    fireEvent.click(toggle);
    expect(screen.getAllByRole("button", { name: /Mostrar Visualizar documentos/i }).length).toBeGreaterThan(
      0,
    );
  });

  it("en mobile usa variant overlay", () => {
    window.matchMedia = createMatchMedia({
      [TABLET_QUERY]: true,
      [MOBILE_QUERY]: true,
    }) as unknown as typeof window.matchMedia;

    Object.defineProperty(window, "innerWidth", { value: 500, configurable: true });
    Object.defineProperty(navigator, "maxTouchPoints", { value: 5, configurable: true });

    render(<DocumentosWorkbench />);
    expect(screen.getByTestId("documentos-workbench")).toHaveAttribute("data-variant", "overlay");
  });

  it("en iPad Pro usa variant overlay (touch + 1024..1366px)", () => {
    window.matchMedia = createMatchMedia({
      [TABLET_QUERY]: false,
      [MOBILE_QUERY]: false,
    }) as unknown as typeof window.matchMedia;

    Object.defineProperty(window, "innerWidth", { value: 1024, configurable: true });
    Object.defineProperty(navigator, "maxTouchPoints", { value: 5, configurable: true });
    window.dispatchEvent(new Event("resize"));

    render(<DocumentosWorkbench />);
    expect(screen.getByTestId("documentos-workbench")).toHaveAttribute("data-variant", "overlay");
  });
});

