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

  test("abre y cierra el modal desde el boton enviar", async () => {
    render(<GestionRespuestaMainTabContent />);

    fireEvent.click(screen.getByRole("button", { name: /^Enviar$/i }));

    expect(screen.getByText(/Confirmar envio de respuesta/i)).toBeInTheDocument();
    expect(screen.getByText(/^Firma de la respuesta$/i)).toBeInTheDocument();
    expect(screen.getByText(/^Tipo de respuesta$/i)).toBeInTheDocument();
    expect(
      screen.getByLabelText(
        /Solicita al centro de envio de correspondencia el envio de la respuesta/i,
      ),
    ).toBeInTheDocument();
    expect(
      screen.getByLabelText(/Confirma respuesta al correo electronico del peticionario/i),
    ).toBeInTheDocument();
    expect(
      screen.getByLabelText(/Certificar digitalmente el documento de respuesta/i),
    ).toBeInTheDocument();
    expect(screen.getByText(/^Direccion de correos$/i)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /Cancelar/i }));

    await waitFor(() => {
      expect(screen.queryByText(/Confirmar envio de respuesta/i)).not.toBeInTheDocument();
    });
  });
});
