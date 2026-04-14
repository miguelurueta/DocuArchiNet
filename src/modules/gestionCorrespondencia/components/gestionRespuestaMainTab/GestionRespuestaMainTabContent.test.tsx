import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { GestionRespuestaMainTabContent } from "./GestionRespuestaMainTabContent";

vi.mock("../../../../app/Components/UI/AppUpload/AppUpload", () => ({
  AppUpload: ({
    value,
    onChange,
  }: {
    value: unknown[];
    onChange: (files: unknown[]) => void;
  }) => (
    <button type="button" onClick={() => onChange(value)} aria-label="Mock upload">
      Mock upload
    </button>
  ),
}));

describe("GestionRespuestaMainTabContent [SPEC:IMPLEMENTACION-APPEDITOR-GESTION-RESPUESTA-EDITORCONTAINER-04-FE]", () => {
  it("reemplaza el placeholder del editor por AppEditor dentro de editorSurface", async () => {
    render(<GestionRespuestaMainTabContent />);

    expect(screen.queryByText(/Aqui se renderizara el editor de contenido/i)).not.toBeInTheDocument();
    expect(screen.getByText("Editor principal")).toBeInTheDocument();
    expect(screen.getByText("Zona dominante del workspace para construir la respuesta.")).toBeInTheDocument();
    expect(screen.getByLabelText("Editor principal de respuesta")).toBeInTheDocument();

    await waitFor(() => {
      expect(
        screen.getByLabelText("Contenido del editor principal de respuesta"),
      ).toBeInTheDocument();
    });
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
});
