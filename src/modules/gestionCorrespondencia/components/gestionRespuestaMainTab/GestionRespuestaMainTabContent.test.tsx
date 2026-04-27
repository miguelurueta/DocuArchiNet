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

describe("GestionRespuestaMainTabContent", () => {
  it("renderiza AppEditor como superficie principal del contenedor", async () => {
    const { container } = render(<GestionRespuestaMainTabContent />);

    expect(screen.queryByText(/Aqui se renderizara el editor de contenido/i)).not.toBeInTheDocument();

    await waitFor(() => {
      expect(
        screen.getByLabelText("Contenido del editor principal de respuesta"),
      ).toBeInTheDocument();
    });

    expect(container.querySelector('[aria-label="Contenido del editor principal de respuesta"]')).toBeInTheDocument();
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
  }, 20000);
});
