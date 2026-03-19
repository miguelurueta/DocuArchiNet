import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppModal } from "./AppModal";

describe("AppModal [SPEC:APP-MODAL-001]", () => {
  it("renderiza titulo y contenido cuando open=true", () => {
    render(
      <AppModal open title="Eliminar registro">
        Confirma la eliminacion del registro.
      </AppModal>,
    );

    expect(screen.getByRole("dialog", { name: "Eliminar registro" })).toBeInTheDocument();
    expect(screen.getByText("Confirma la eliminacion del registro.")).toBeInTheDocument();
  });

  it("permanece oculto cuando open=false", () => {
    render(
      <AppModal open={false} title="Oculto">
        Contenido
      </AppModal>,
    );

    expect(screen.queryByRole("dialog")).toBeNull();
  });

  it("ejecuta accion primaria y secundaria", () => {
    const handleConfirm = vi.fn();
    const handleCancel = vi.fn();

    render(
      <AppModal
        open
        title="Confirmacion"
        primaryAction={{ label: "Aceptar", onClick: handleConfirm }}
        secondaryAction={{ label: "Cancelar", onClick: handleCancel }}
      >
        Texto
      </AppModal>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Aceptar" }));
    fireEvent.click(screen.getByRole("button", { name: "Cancelar" }));

    expect(handleConfirm).toHaveBeenCalledTimes(1);
    expect(handleCancel).toHaveBeenCalledTimes(1);
  });

  it("ejecuta onClose con tecla Escape cuando closeOnEscape=true", () => {
    const handleClose = vi.fn();

    render(
      <AppModal open title="Dialogo" onClose={handleClose}>
        Texto
      </AppModal>,
    );

    fireEvent.keyDown(document, { key: "Escape" });

    expect(handleClose).toHaveBeenCalledTimes(1);
  });

  it("oculta el footer cuando hideFooter=true", () => {
    render(
      <AppModal
        open
        title="Sin footer"
        hideFooter
        primaryAction={{ label: "Aceptar", onClick: vi.fn() }}
      >
        Texto
      </AppModal>,
    );

    expect(screen.queryByRole("button", { name: "Aceptar" })).toBeNull();
  });

  it("bloquea acciones cuando estan en loading o disabled", () => {
    const handleConfirm = vi.fn();

    render(
      <AppModal
        open
        title="Procesando"
        primaryAction={{
          label: "Guardar",
          onClick: handleConfirm,
          loading: true,
        }}
        secondaryAction={{
          label: "Cancelar",
          onClick: vi.fn(),
          disabled: true,
        }}
      >
        Texto
      </AppModal>,
    );

    const confirmButton = screen.getByRole("button", { name: /guardar/i });
    const cancelButton = screen.getByRole("button", { name: "Cancelar" });

    fireEvent.click(confirmButton);
    fireEvent.click(cancelButton);

    expect(confirmButton).toBeDisabled();
    expect(cancelButton).toBeDisabled();
    expect(handleConfirm).not.toHaveBeenCalled();
  });
});
