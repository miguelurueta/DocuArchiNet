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
        users={["ana@contasoft.com"]}
        onAddUser={vi.fn()}
        onRemoveUser={vi.fn()}
        onRemoveAllUsers={vi.fn()}
        onSubmit={onSubmit}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Enviar" }));
    expect(onSubmit).toHaveBeenCalledTimes(1);
  });

  it("no envia y muestra error cuando no hay responsables", () => {
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
    expect(onSubmit).not.toHaveBeenCalled();
    expect(screen.getByText("Debe seleccionar al menos un responsable.")).toBeInTheDocument();
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

  it("[SPEC:SCRUMCORE-148] abre confirmacion de tramite reasignado al enviar valido", () => {
    render(
      <ReasignarRespuestaModal
        open
        onClose={vi.fn()}
        radicado="2500056897"
        nota="Nota"
        users={["angelica.torres@contasoft.com"]}
        onAddUser={vi.fn()}
        onRemoveUser={vi.fn()}
        onRemoveAllUsers={vi.fn()}
        onSubmit={vi.fn()}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Enviar" }));

    expect(screen.getByRole("heading", { name: /Trámite Reasignado/i })).toBeInTheDocument();
    expect(screen.getByText(/Usuario Asignado:/i)).toBeInTheDocument();
    expect(screen.getByText("Angelica Torres")).toBeInTheDocument();
    expect(screen.getByText(/Radicado:/i)).toBeInTheDocument();
    expect(screen.getByText("2500056897")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Aceptar" })).toBeInTheDocument();
  });

  it("[SPEC:SCRUMCORE-148] bloquea escritura cuando ya existe un responsable seleccionado", () => {
    render(
      <ReasignarRespuestaModal
        open
        onClose={vi.fn()}
        radicado="2500056897"
        nota="Nota"
        users={["angelica.torres@contasoft.com"]}
        onAddUser={vi.fn()}
        onRemoveUser={vi.fn()}
        onRemoveAllUsers={vi.fn()}
        onSubmit={vi.fn()}
      />,
    );

    const input = screen.getByRole("combobox");
    expect(input).toHaveAttribute("readonly");

    fireEvent.change(input, { target: { value: "nuevo@contasoft.com" } });
    expect(input).toHaveValue("");
  });
});
