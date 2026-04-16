import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, test } from "vitest";
import { GestionRespuestaMainTabContent } from "../components/gestionRespuestaMainTab/GestionRespuestaMainTabContent";

describe("[SCRUMCORE-89] GestionRespuesta main tab workbench", () => {
  test("renderiza toolbar y zona de adjuntos", () => {
    render(<GestionRespuestaMainTabContent />);

    expect(screen.getByText(/Solicitud de Aprobacion/i)).toBeInTheDocument();
    expect(screen.getByText(/^Guardar$/i)).toBeInTheDocument();
    expect(screen.getByText(/^Enviar$/i)).toBeInTheDocument();
    expect(screen.getByText(/Adjuntos/i)).toBeInTheDocument();
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

  test("abre y cierra el modal de solicitud de aprobacion", async () => {
    render(<GestionRespuestaMainTabContent />);

    fireEvent.click(screen.getByRole("button", { name: /Solicitud de Aprobacion/i }));

    expect(screen.getByText(/Gestionar Documento/i)).toBeInTheDocument();
    expect(screen.getByText(/Solicitud de aprobacion documental/i)).toBeInTheDocument();
    expect(screen.getByText(/^Tipo de documento$/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/Requiere firma del responsable/i)).toBeInTheDocument();
    expect(screen.getByText(/^Etiquetas de gestion$/i)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /Cancelar/i }));

    await waitFor(() => {
      expect(screen.queryByText(/Gestionar Documento/i)).not.toBeInTheDocument();
    });
  });
});
