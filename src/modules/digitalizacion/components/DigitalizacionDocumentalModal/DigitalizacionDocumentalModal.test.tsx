import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { DigitalizacionDocumentalModal } from "./DigitalizacionDocumentalModal";
import type { DigitalizacionContext } from "../../types/digitalizacion.types";

const baseProps = {
  open: true,
  onClose: vi.fn(),
  onCompleted: vi.fn(),
};

const crearContext: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "Gestion",
  radicado: "RAD-2026",
};

const adjuntarContext: DigitalizacionContext = {
  modo: "adjuntar",
  nombreGabinete: "Gestion",
  radicado: "RAD-2026",
  idDocumentoDestino: 321,
};

describe("[SPEC:SCRUMCORE-239] DigitalizacionDocumentalModal", () => {
  it("renders crear mode", () => {
    render(<DigitalizacionDocumentalModal {...baseProps} context={crearContext} />);

    expect(screen.getByTestId("digitalizacion-modal")).toBeInTheDocument();
    expect(screen.getByText("crear")).toBeInTheDocument();
    expect(screen.getAllByText("Guardar documento")).toHaveLength(2);
    expect(screen.getByText("RAD-2026")).toBeInTheDocument();
  });

  it("renders adjuntar mode", () => {
    render(<DigitalizacionDocumentalModal {...baseProps} context={adjuntarContext} />);

    expect(screen.getByText("adjuntar")).toBeInTheDocument();
    expect(screen.getAllByText("Adjuntar digitalizacion").length).toBeGreaterThanOrEqual(2);
    expect(screen.getByText("321")).toBeInTheDocument();
  });

  it("shows controlled error for null context", () => {
    const onError = vi.fn();

    render(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={null}
        onError={onError}
      />,
    );

    expect(screen.getByRole("alert")).toHaveTextContent(
      "El contexto documental es obligatorio.",
    );
    expect(onError).toHaveBeenCalledWith(
      expect.objectContaining({ code: "CONTEXT_REQUIRED" }),
    );
  });

  it("shows required idDocumentoDestino for adjuntar", () => {
    render(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={{ modo: "adjuntar", nombreGabinete: "Gestion" }}
      />,
    );

    expect(screen.getByRole("alert")).toHaveTextContent(
      "idDocumentoDestino es obligatorio para modo adjuntar.",
    );
  });

  it("cancel completes with cancelado and closes", () => {
    const onClose = vi.fn();
    const onCompleted = vi.fn();

    render(
      <DigitalizacionDocumentalModal
        open
        context={crearContext}
        onClose={onClose}
        onCompleted={onCompleted}
      />,
    );

    fireEvent.click(screen.getByText("Cancelar"));

    expect(onCompleted).toHaveBeenCalledWith({ accion: "cancelado" });
    expect(onClose).toHaveBeenCalled();
  });

  it("clears previous context data when context changes", () => {
    const { rerender } = render(
      <DigitalizacionDocumentalModal {...baseProps} context={crearContext} />,
    );

    expect(screen.getByText("RAD-2026")).toBeInTheDocument();

    rerender(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={{
          modo: "crear",
          nombreGabinete: "Archivo",
          radicado: "RAD-NEW",
        }}
      />,
    );

    expect(screen.queryByText("RAD-2026")).not.toBeInTheDocument();
    expect(screen.getByText("RAD-NEW")).toBeInTheDocument();
  });
});
