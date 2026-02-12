import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router";
import { describe, expect, it, vi } from "vitest";
import CambiarPasswordForm from "./CambiarPasswordForm";

const navigateMock = vi.fn();

vi.mock("react-router", async importOriginal => {
  const actual = await importOriginal<typeof import("react-router")>();
  return {
    ...actual,
    useNavigate: () => navigateMock,
  };
});

describe("CambiarPasswordForm", () => {
  it("[SPEC:RST-003] Validaciones UI de password", () => {
    const onSubmit = vi.fn();

    const { container } = render(
      <MemoryRouter>
        <CambiarPasswordForm onSubmit={onSubmit} />
      </MemoryRouter>
    );

    fireEvent.click(screen.getByRole("button", { name: "Actualizar contraseña" }));

    expect(onSubmit).not.toHaveBeenCalled();
    expect(screen.getByText("Debe informar la contraseña")).toBeInTheDocument();
    expect(screen.getByText("Debe confirmar la contraseña")).toBeInTheDocument();

    const passwordInputs = Array.from(
      container.querySelectorAll<HTMLInputElement>('input[type="password"]')
    );
    fireEvent.change(passwordInputs[0], {
      target: { value: "ClaveSegura123" },
    });
    fireEvent.change(passwordInputs[1], {
      target: { value: "OtraClave456" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Actualizar contraseña" }));

    expect(screen.getByText("Las contraseñas no coinciden")).toBeInTheDocument();
    expect(onSubmit).not.toHaveBeenCalled();
  });

  it("[SPEC:RST-004] Navegación SPA correcta (sin recarga)", () => {
    const onSubmit = vi.fn();

    render(
      <MemoryRouter>
        <CambiarPasswordForm onSubmit={onSubmit} />
      </MemoryRouter>
    );

    fireEvent.click(screen.getByRole("link", { name: "Volver" }));

    expect(navigateMock).toHaveBeenCalledWith("/");
  });
});
