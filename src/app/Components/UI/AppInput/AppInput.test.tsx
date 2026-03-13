import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppInput } from "./AppInput";
import styles from "./AppInput.module.css";

describe("AppInput [SPEC:APP-INPUT-001]", () => {
  it("renderiza label, helperText y placeholder", () => {
    render(
      <AppInput
        label="Correo"
        helperText="Ingresa el correo corporativo"
        placeholder="nombre@empresa.com"
      />,
    );

    expect(screen.getByLabelText("Correo")).toBeInTheDocument();
    expect(screen.getByText("Ingresa el correo corporativo")).toBeInTheDocument();
    expect(screen.getByPlaceholderText("nombre@empresa.com")).toBeInTheDocument();
  });

  it("propaga onChange y sincroniza valor controlado", () => {
    const handleChange = vi.fn();

    const { rerender } = render(
      <AppInput label="Nombre" value="Ana" onChange={handleChange} />,
    );

    const input = screen.getByLabelText("Nombre");
    fireEvent.change(input, { target: { value: "Ana Maria" } });

    expect(handleChange).toHaveBeenCalledTimes(1);

    rerender(<AppInput label="Nombre" value="Ana Maria" onChange={handleChange} />);
    expect(screen.getByDisplayValue("Ana Maria")).toBeInTheDocument();
  });

  it("soporta defaultValue en modo no controlado", () => {
    render(<AppInput label="Ciudad" defaultValue="Bogota" />);

    expect(screen.getByDisplayValue("Bogota")).toBeInTheDocument();
  });

  it("respeta disabled y expone semantica accesible", () => {
    render(<AppInput label="Documento" disabled />);

    const input = screen.getByLabelText("Documento");
    expect(input).toBeDisabled();
    expect(input).toHaveClass(styles.inputDisabled);
  });

  it("renderiza estado de error con helperText asociado", () => {
    render(
      <AppInput
        label="Telefono"
        error
        helperText="El telefono es obligatorio"
      />,
    );

    const input = screen.getByLabelText("Telefono");
    const helper = screen.getByText("El telefono es obligatorio");

    expect(input).toHaveAttribute("aria-invalid", "true");
    expect(input).toHaveClass(styles.inputError);
    expect(helper).toHaveClass(styles.helperTextError);
    expect(input).toHaveAttribute("aria-describedby", helper.getAttribute("id") ?? "");
  });

  it("permite composicion segura con className externa", () => {
    render(<AppInput label="Filtro" className="custom-input" />);

    expect(screen.getByLabelText("Filtro")).toHaveClass("custom-input");
    expect(screen.getByLabelText("Filtro")).toHaveClass(styles.input);
  });
});
