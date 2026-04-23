import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { TramiteReasignadoModal } from "./TramiteReasignadoModal";

describe("[SPEC:SCRUMCORE-148] TramiteReasignadoModal", () => {
  it("renderiza contenido cuando open=true", () => {
    render(
      <TramiteReasignadoModal
        open
        usuarioAsignado="Angelica Pedraza"
        radicado="2500056897"
        onClose={vi.fn()}
      />,
    );

    expect(screen.getByRole("heading", { name: /Trámite Reasignado/i })).toBeInTheDocument();
    expect(screen.getByText(/Usuario Asignado:/i)).toBeInTheDocument();
    expect(screen.getByText(/Angelica Pedraza/i)).toBeInTheDocument();
    expect(screen.getByText(/Radicado:/i)).toBeInTheDocument();
    expect(screen.getByText(/2500056897/i)).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Aceptar" })).toBeInTheDocument();
  });

  it("ejecuta onClose al pulsar Aceptar", () => {
    const onClose = vi.fn();
    render(
      <TramiteReasignadoModal
        open
        usuarioAsignado="Angelica Pedraza"
        radicado="2500056897"
        onClose={onClose}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Aceptar" }));
    expect(onClose).toHaveBeenCalledTimes(1);
  });
});
