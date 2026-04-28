import { fireEvent, render, screen } from "@testing-library/react";
import { DocumentosWorkbench } from "../components/documentosWorkbench/DocumentosWorkbench";

jest.mock(
  "../components/documentosWorkbench/PdfDocumentViewer",
  () => ({
    PdfDocumentViewer: () => <div data-testid="pdf-document-viewer" />,
  }),
);

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
      });
    });

  it("renderiza el workbench y deja el panel expandido en desktop", () => {
    render(<DocumentosWorkbench />);

    expect(screen.getByTestId("documentos-workbench")).toBeInTheDocument();
    expect(
      screen.getByRole("button", { name: /Ocultar Visualizar documentos/i }),
    ).toBeInTheDocument();
  });

  it("permite colapsar el panel desde el toggle", () => {
    render(<DocumentosWorkbench />);

    const toggle = screen.getByRole("button", {
      name: /Ocultar Visualizar documentos/i,
    });
    fireEvent.click(toggle);

    const collapsedButtons = screen.getAllByRole("button", {
      name: /Mostrar Visualizar documentos/i,
    });
    expect(collapsedButtons.length).toBeGreaterThan(0);
  });

  it("mantiene el contenido del panel montado al colapsar", () => {
    render(<DocumentosWorkbench />);

    const toggle = screen.getByRole("button", {
      name: /Ocultar Visualizar documentos/i,
    });
    fireEvent.click(toggle);

    expect(screen.getByText("Radicado_2026_0413.pdf")).toBeInTheDocument();
  });

  it("permite enfocar el toggle del panel con teclado", () => {
    render(<DocumentosWorkbench />);

    const toggle = screen.getByRole("button", {
      name: /Ocultar Visualizar documentos/i,
    });
    toggle.focus();
    expect(toggle).toHaveFocus();
  });

  it("aplica variant overlay en mobile", () => {
    window.matchMedia = createMatchMedia({
      [TABLET_QUERY]: true,
      [MOBILE_QUERY]: true,
    });

    render(<DocumentosWorkbench />);

    const panel = screen.getByLabelText("Visualizar documentos");
    expect(panel).toHaveAttribute("data-variant", "overlay");
  });

  it("muestra el visor PDF en el contenido principal", () => {
    render(<DocumentosWorkbench />);

    expect(screen.getByTestId("pdf-document-viewer")).toBeInTheDocument();
  });
  },
);
