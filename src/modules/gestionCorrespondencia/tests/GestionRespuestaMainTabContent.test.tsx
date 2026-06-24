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
  }, 15000);

  test("permite colapsar y expandir el panel derecho", () => {
    render(<GestionRespuestaMainTabContent />);

    const toggle = screen
      .getAllByRole("button", { name: /Ocultar/i })
      .find((button) => button.getAttribute("aria-expanded") !== null);
    if (!toggle) {
      throw new Error("No se encontro el toggle del panel de herramientas.");
    }
    expect(toggle).toHaveAttribute("aria-expanded", "true");

    fireEvent.click(toggle);

    expect(screen.getAllByRole("button", { name: /Mostrar/i }).length).toBeGreaterThan(0);
  }, 15000);

  test("mantiene el gate de envio cuando no hay adjuntos", async () => {
    render(<GestionRespuestaMainTabContent />);

    fireEvent.click(screen.getByRole("button", { name: /^Enviar$/i }));

    await waitFor(() => {
      expect(screen.queryByText(/Confirmar envio de respuesta/i)).not.toBeInTheDocument();
    }, { timeout: 10000 });
  }, 15000);
});
