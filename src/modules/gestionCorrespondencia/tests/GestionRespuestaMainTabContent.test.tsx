import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, test } from "vitest";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";

describe("[SCRUMCORE-89] GestionRespuesta main tab workbench", () => {
  test("renderiza toolbar y zona de adjuntos", () => {
    render(<GestionRespuestaMainTabContent />);

    expect(screen.getByText(/Solicitud de Aprobacion/i)).toBeInTheDocument();
    expect(screen.queryByText(/^Guardar$/i)).not.toBeInTheDocument();
    expect(screen.getByText(/^Enviar$/i)).toBeInTheDocument();
    expect(screen.getByRole("heading", { name: /^Adjuntos$/i })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Arrastra archivos/i })).toBeInTheDocument();
  });

  test("permite colapsar y expandir el panel derecho", () => {
    render(<GestionRespuestaMainTabContent />);

    const toggle = screen
      .getAllByRole("button", { name: /herramientas/i })
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

  test("mantiene el gate de envio cuando no hay adjuntos", async () => {
    render(<GestionRespuestaMainTabContent />);

    fireEvent.click(screen.getByRole("button", { name: /^Enviar$/i }));

    await waitFor(() => {
      expect(
        screen.getByText(/Para habilitar envio, carga al menos un archivo/i),
      ).toBeInTheDocument();
    }, { timeout: 10000 });

    expect(screen.queryByText(/Confirmar envio de respuesta/i)).not.toBeInTheDocument();
  }, 15000);
});
