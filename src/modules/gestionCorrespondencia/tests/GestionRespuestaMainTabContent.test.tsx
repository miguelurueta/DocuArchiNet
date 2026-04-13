import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, test } from "vitest";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";

describe("[SCRUMCORE-89] GestionRespuesta main tab workbench", () => {
  test("renderiza toolbar y zona de adjuntos", () => {
    render(<GestionRespuestaMainTabContent />);

    expect(screen.getByText(/Guardar borrador/i)).toBeInTheDocument();
    expect(screen.getByText(/Enviar respuesta/i)).toBeInTheDocument();
    expect(screen.getByText(/Adjuntos/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Arrastra archivos/i })).toBeInTheDocument();
  });

  test("permite colapsar y expandir el panel derecho", () => {
    render(<GestionRespuestaMainTabContent />);

    const toggle = screen
      .getAllByRole("button", { name: /panel de herramientas/i })
      .find((button) => button.getAttribute("aria-expanded") !== null);
    if (!toggle) {
      throw new Error("No se encontro el toggle del panel de herramientas.");
    }
    const workbench = screen.getByTestId("gestion-respuesta-workbench");

    expect(toggle).toHaveAttribute("aria-expanded", "true");
    expect(workbench).toHaveAttribute("data-panel-collapsed", "false");

    fireEvent.click(toggle);

    expect(toggle).toHaveAttribute("aria-expanded", "false");
    expect(workbench).toHaveAttribute("data-panel-collapsed", "true");
  });
});
