import { fireEvent, render, screen } from "@testing-library/react";
import { vi } from "vitest";
import { DocumentosWorkbench } from "../components/documentosWorkbench/DocumentosWorkbench";

vi.mock("../hooks/useListaDocumentosRadicadosTreeTable", () => ({
  useListaDocumentosRadicadosTreeTable: () => ({
    load: vi.fn(),
    loadChildren: vi.fn(),
    onSelectRow: vi.fn(),
    columns: [],
  }),
}));

vi.mock("../../../app/Components/UI/AppVisorEmbedPdf", () => ({
  AppVisorEmbedPdf: () => (
    <div role="status" aria-label="Zona de documento" data-testid="app-visor-embedpdf-mock" />
  ),
}));

vi.mock("../../../app/Components/UI/AppTreeTable", () => ({
  AppTreeTable: () => <div data-testid="app-tree-table-mock" />,
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

describe(
  "[SPEC:documentos-workbench-tab] [SPEC:implementacion-visual-tab-documentos-03-fe] Documentos workbench",
  () => {
    beforeEach(() => {
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
    expect(
      screen.getByRole("button", { name: /Ocultar Visualizar documentos/i }),
    ).toBeInTheDocument();
    expect(screen.getByTestId("app-tree-table-mock")).toBeInTheDocument();
  });

  it("permite colapsar el rail", () => {
    render(<DocumentosWorkbench />);
    const toggle = screen.getByRole("button", {
      name: /Ocultar Visualizar documentos/i,
    });
    fireEvent.click(toggle);
    expect(
      screen.getAllByRole("button", { name: /Mostrar Visualizar documentos/i }).length,
    ).toBeGreaterThan(0);
  });

  it("en mobile usa variant overlay", () => {
    window.matchMedia = createMatchMedia({
      [TABLET_QUERY]: true,
      [MOBILE_QUERY]: true,
    }) as unknown as typeof window.matchMedia;

    Object.defineProperty(window, "innerWidth", { value: 500, configurable: true });
    Object.defineProperty(navigator, "maxTouchPoints", { value: 5, configurable: true });

    render(<DocumentosWorkbench />);
    expect(screen.getByTestId("documentos-workbench")).toHaveAttribute(
      "data-variant",
      "overlay",
    );
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
    expect(screen.getByTestId("documentos-workbench")).toHaveAttribute(
      "data-variant",
      "overlay",
    );
  });
  },
);
