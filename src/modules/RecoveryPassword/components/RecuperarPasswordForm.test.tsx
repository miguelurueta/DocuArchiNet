import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it, vi } from "vitest";
import RecuperarPasswordForm from "./RecuperarPasswordForm";

const navigateMock = vi.fn();

vi.mock("react-router", async importOriginal => {
  const actual = await importOriginal<typeof import("react-router")>();
  return {
    ...actual,
    useNavigate: () => navigateMock,
  };
});

describe("RecuperarPasswordForm", () => {
  it("[SPEC:REC-001] Solicitud de recuperación SUCCESS", () => {
    const onSubmit = vi.fn();

    render(
      <MemoryRouter>
        <RecuperarPasswordForm
          onSubmit={onSubmit}
          idModulo={7}
          idEmpresa={23}
          loginUsuario=""
        />
      </MemoryRouter>
    );

    fireEvent.change(screen.getByRole("textbox"), {
      target: { value: "usuario.demo" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Continuar" }));

    expect(onSubmit).toHaveBeenCalledTimes(1);
    expect(onSubmit).toHaveBeenCalledWith({
      user: "usuario.demo",
      idModule: 7,
      IdEmpresa: 23,
    });
  });

  it("[SPEC:REC-002] Email inválido / usuario vacío no envía formulario", () => {
    const onSubmit = vi.fn();

    render(
      <MemoryRouter>
        <RecuperarPasswordForm onSubmit={onSubmit} idModulo={7} idEmpresa={23} />
      </MemoryRouter>
    );

    fireEvent.click(screen.getByRole("button", { name: "Continuar" }));

    expect(onSubmit).not.toHaveBeenCalled();
    expect(screen.getByText("Debe informar el usuario")).toBeInTheDocument();
  });

});
