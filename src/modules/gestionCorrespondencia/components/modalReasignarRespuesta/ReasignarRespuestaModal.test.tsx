import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { ReasignarRespuestaModal } from "./ReasignarRespuestaModal";

describe("ReasignarRespuestaModal [SPEC:implementacion-visual-appmodal-gestion-correspondencia-13-fe]", () => {
  it("renderiza estructura base cuando open=true", () => {
    render(
      <ReasignarRespuestaModal
        open
        onClose={vi.fn()}
        radicado="2500056897"
        nota="Buen dia, Angelica..."
        users={["ana@contasoft.com"]}
        onAddUser={vi.fn()}
        onRemoveUser={vi.fn()}
        onRemoveAllUsers={vi.fn()}
        onSubmit={vi.fn()}
      />,
    );

    expect(screen.getByRole("heading", { name: "Reasignar Respuesta" })).toBeInTheDocument();
    expect(screen.getByText("RAD. 2500056897")).toBeInTheDocument();
    expect(screen.getByText("Buen dia, Angelica...")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Cancelar" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Enviar" })).toBeInTheDocument();
  });

  it("ejecuta onClose al pulsar Cancelar", () => {
    const onClose = vi.fn();
    render(
      <ReasignarRespuestaModal
        open
        onClose={onClose}
        radicado="2500056897"
        nota="Nota"
        users={[]}
        onAddUser={vi.fn()}
        onRemoveUser={vi.fn()}
        onRemoveAllUsers={vi.fn()}
        onSubmit={vi.fn()}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Cancelar" }));
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("ejecuta onSubmit al pulsar Enviar", () => {
    const onSubmit = vi.fn();
    render(
      <ReasignarRespuestaModal
        open
        onClose={vi.fn()}
        radicado="2500056897"
        nota="Nota"
        users={[]}
        onAddUser={vi.fn()}
        onRemoveUser={vi.fn()}
        onRemoveAllUsers={vi.fn()}
        onSubmit={onSubmit}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Enviar" }));
    expect(onSubmit).toHaveBeenCalledTimes(1);
  });

  it("permite eliminar tags y dispara callback", () => {
    const onRemoveUser = vi.fn();
    render(
      <ReasignarRespuestaModal
        open
        onClose={vi.fn()}
        radicado="2500056897"
        nota="Nota"
        users={["ana@contasoft.com"]}
        onAddUser={vi.fn()}
        onRemoveUser={onRemoveUser}
        onRemoveAllUsers={vi.fn()}
        onSubmit={vi.fn()}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Eliminar ana@contasoft.com" }));
    expect(onRemoveUser).toHaveBeenCalledWith("ana@contasoft.com");
  });
});

