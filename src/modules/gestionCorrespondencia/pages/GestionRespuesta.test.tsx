import { fireEvent, render, screen } from "@testing-library/react";
import type { ReactNode } from "react";
import { describe, expect, it, vi } from "vitest";
import GestionRespuesta from "./GestionRespuesta";

const { documentosWorkbenchRenderSpy } = vi.hoisted(() => ({
  documentosWorkbenchRenderSpy: vi.fn(),
}));

vi.mock("@ant-design/icons", () => ({
  CloseOutlined: () => <span aria-hidden="true" />,
  ColumnWidthOutlined: () => <span aria-hidden="true" />,
  FileTextOutlined: () => <span aria-hidden="true" />,
  InfoCircleOutlined: () => <span aria-hidden="true" />,
  RobotOutlined: () => <span aria-hidden="true" />,
  SendOutlined: () => <span aria-hidden="true" />,
}));

vi.mock("antd", () => ({
  Switch: ({
    checked,
    disabled,
    onChange,
    "aria-label": ariaLabel,
    "aria-pressed": ariaPressed,
  }: {
    checked?: boolean;
    disabled?: boolean;
    onChange?: (checked: boolean) => void;
    "aria-label"?: string;
    "aria-pressed"?: boolean;
  }) => (
    <button
      type="button"
      role="switch"
      aria-label={ariaLabel}
      aria-checked={checked}
      aria-pressed={ariaPressed}
      disabled={disabled}
      onClick={() => onChange?.(!checked)}
    />
  ),
}));

vi.mock("react-router-dom", async (importOriginal) => {
  const actual = await importOriginal<typeof import("react-router-dom")>();
  return {
    ...actual,
    useParams: () => ({ id: "123" }),
  };
});

vi.mock("../../../app/Components/UI/AppTabs", () => ({
  AppTabs: ({
    items,
    tabBarExtraContent,
  }: {
    items: Array<{ key: string; label: ReactNode; children: ReactNode }>;
    tabBarExtraContent?: { right?: ReactNode };
  }) => (
    <div>
      <div role="tablist">
        {items.map((item) => (
          <button type="button" role="tab" key={item.key}>
            {item.label}
          </button>
        ))}
        <div>{tabBarExtraContent?.right}</div>
      </div>
      <div>{items[0]?.children}</div>
    </div>
  ),
}));

vi.mock("../context/GestionRespuestaDocumentosContext", () => ({
  GestionRespuestaDocumentosProvider: ({ children }: { children: ReactNode }) => (
    <div>{children}</div>
  ),
}));

vi.mock("../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent", () => ({
  GestionRespuestaMainTabContent: () => <div>Mock Gestion</div>,
}));

vi.mock("../components/documentosWorkbench", () => ({
  DocumentosWorkbench: () => {
    documentosWorkbenchRenderSpy();
    return (
      <div>
        <span>Mock Documentos</span>
        <span>Documento seleccionado: contrato.pdf</span>
      </div>
    );
  },
}));

const mockMatchMedia = (matches: boolean) => {
  Object.defineProperty(window, "matchMedia", {
    writable: true,
    value: vi.fn().mockImplementation((query: string) => ({
      matches,
      media: query,
      onchange: null,
      addEventListener: vi.fn(),
      removeEventListener: vi.fn(),
      dispatchEvent: vi.fn(),
    })),
  });
};

describe("[SCRUMCORE-251] GestionRespuesta parallel tabs", () => {
  beforeEach(() => {
    documentosWorkbenchRenderSpy.mockClear();
  });

  it("renderiza tabs normales por defecto y expone boton opt-in", () => {
    mockMatchMedia(true);

    render(<GestionRespuesta />);

    const toggle = screen.getByRole("switch", { name: /Vista paralela/i });
    expect(toggle).toHaveAttribute("aria-pressed", "false");
    expect(toggle).toHaveAttribute("aria-checked", "false");
    expect(screen.getByText("Vista paralela").closest("label")).toHaveAttribute(
      "data-layout-state",
      "inactive",
    );
    expect(screen.getByRole("tab", { name: /Gestion/i })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: /Documentos/i })).toBeInTheDocument();
  });

  it("activa y desactiva la vista paralela sin perder Gestion ni Documentos", () => {
    mockMatchMedia(true);

    render(<GestionRespuesta />);

    const toggle = screen.getByRole("switch", { name: /Vista paralela/i });
    fireEvent.click(toggle);

    expect(screen.getByRole("switch", { name: /Vista paralela/i })).toHaveAttribute(
      "aria-pressed",
      "true",
    );
    expect(screen.getByRole("switch", { name: /Vista paralela/i })).toHaveAttribute(
      "aria-checked",
      "true",
    );
    expect(screen.getByText("Vista paralela").closest("label")).toHaveAttribute(
      "data-layout-state",
      "active",
    );
    expect(screen.getByLabelText("Gestion")).toHaveTextContent("Mock Gestion");
    expect(screen.getByLabelText("Documentos")).toHaveTextContent("Mock Documentos");

    fireEvent.click(screen.getByRole("switch", { name: /Vista paralela/i }));

    expect(screen.getByRole("switch", { name: /Vista paralela/i })).toHaveAttribute(
      "aria-pressed",
      "false",
    );
    expect(screen.getByRole("tab", { name: /Gestion/i })).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: /Documentos/i })).toBeInTheDocument();
  });

  it("mantiene visible el documento seleccionado al alternar modo", () => {
    mockMatchMedia(true);

    render(<GestionRespuesta />);

    fireEvent.click(screen.getByRole("switch", { name: /Vista paralela/i }));
    expect(screen.getByText("Documento seleccionado: contrato.pdf")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("switch", { name: /Vista paralela/i }));
    fireEvent.click(screen.getByRole("switch", { name: /Vista paralela/i }));

    expect(screen.getByLabelText("Documentos")).toHaveTextContent(
      "Documento seleccionado: contrato.pdf",
    );
  });

  it("no duplica la instancia visible de Documentos al alternar modo", () => {
    mockMatchMedia(true);

    render(<GestionRespuesta />);

    fireEvent.click(screen.getByRole("switch", { name: /Vista paralela/i }));
    expect(screen.getAllByText("Mock Documentos")).toHaveLength(1);

    fireEvent.click(screen.getByRole("switch", { name: /Vista paralela/i }));
    fireEvent.click(screen.getByRole("switch", { name: /Vista paralela/i }));

    expect(screen.getAllByText("Mock Documentos")).toHaveLength(1);
    expect(documentosWorkbenchRenderSpy).toHaveBeenCalledTimes(2);
  });

  it("mantiene deshabilitada la vista paralela en ancho reducido", () => {
    mockMatchMedia(false);

    render(<GestionRespuesta />);

    expect(screen.getByRole("switch", { name: /Vista paralela/i })).toBeDisabled();
  });
});
