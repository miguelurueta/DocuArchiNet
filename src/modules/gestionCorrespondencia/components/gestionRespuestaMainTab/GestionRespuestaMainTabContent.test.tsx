import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { GestionRespuestaMainTabContent } from "./GestionRespuestaMainTabContent";

vi.mock("../../hooks/useEstructuraRespuestaIdTarea", () => ({
  useEstructuraRespuestaIdTarea: () => ({
    estrucTuraRespuesta: null,
    loading: false,
    error: null,
    isEmpty: true,
  }),
}));

vi.mock("../../../../app/Components/UI/AppUpload/AppUpload", () => ({
  AppUpload: ({
    onChange,
  }: {
    onChange: (files: unknown[]) => void;
  }) => (
    <button
      type="button"
      onClick={() =>
        onChange([
          {
            uid: "test-file-1",
            name: "soporte.pdf",
            size: 1234,
            status: "done",
          },
        ])
      }
      aria-label="Mock upload"
    >
      Mock upload
    </button>
  ),
}));

describe("GestionRespuestaMainTabContent [SPEC:APP-APPEDITORPDF-05-FE]", () => {
  it("renderiza AppEditorPdf como superficie principal del contenedor", async () => {
    const { container } = render(<GestionRespuestaMainTabContent />);

    expect(screen.queryByText(/Aqui se renderizara el editor de contenido/i)).not.toBeInTheDocument();
    expect(screen.getByLabelText("Editor principal de respuesta")).toHaveAttribute(
      "data-editor-shell",
      "neutral",
    );

    await waitFor(() => {
      expect(
        screen.getByLabelText("Contenido del editor principal de respuesta"),
      ).toBeInTheDocument();
    });

    expect(
      container.querySelector('[data-pagination-mode="visual"]'),
    ).toBeInTheDocument();
  });

  it("mantiene operativo el colapso y expansion del panel lateral", async () => {
    render(<GestionRespuestaMainTabContent />);

    const toggle = screen.getByLabelText("Ocultar panel de herramientas");
    fireEvent.click(toggle);

    expect(
      screen.getAllByRole("button", { name: "Mostrar panel de herramientas" }).length,
    ).toBeGreaterThan(0);

    fireEvent.click(screen.getAllByRole("button", { name: "Mostrar panel de herramientas" })[0]);

    await waitFor(() => {
      expect(screen.getByLabelText("Ocultar panel de herramientas")).toBeInTheDocument();
    });
  });

  it("[SPEC:APP-APPSTEPS-03-FE] integra AppSteps y bloquea Envio hasta tener adjuntos", async () => {
    render(<GestionRespuestaMainTabContent />);

    const getStepButton = (label: string) =>
      screen.getByRole("button", { name: new RegExp(`${label}estado`, "i") });

    fireEvent.click(getStepButton("Adjuntos"));
    await waitFor(() => {
      expect(
        getStepButton("Adjuntos").querySelector('[aria-current="step"]'),
      ).toBeInTheDocument();
    });

    fireEvent.click(getStepButton("Envio"));
    await waitFor(() => {
      expect(
        getStepButton("Adjuntos").querySelector('[aria-current="step"]'),
      ).toBeInTheDocument();
    });

    fireEvent.click(screen.getByLabelText("Mock upload"));
    fireEvent.click(getStepButton("Envio"));

    await waitFor(() => {
      expect(getStepButton("Envio").querySelector('[aria-current="step"]')).toBeInTheDocument();
    });
    expect(
      screen.queryByText(/Para habilitar envio, carga al menos un archivo/i),
    ).not.toBeInTheDocument();
  }, 15000);
});
