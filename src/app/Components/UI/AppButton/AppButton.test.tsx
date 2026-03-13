import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppButton } from "./AppButton";
import styles from "./AppButton.module.css";

describe("AppButton [SPEC:APP-BUTTON-001]", () => {
  it("renderiza children y ejecuta onClick cuando esta habilitado", () => {
    const handleClick = vi.fn();

    render(<AppButton onClick={handleClick}>Guardar</AppButton>);

    const button = screen.getByRole("button", { name: "Guardar" });
    fireEvent.click(button);

    expect(button).toBeInTheDocument();
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it("no ejecuta onClick cuando disabled=true", () => {
    const handleClick = vi.fn();

    render(
      <AppButton disabled onClick={handleClick}>
        Deshabilitado
      </AppButton>,
    );

    const button = screen.getByRole("button", { name: "Deshabilitado" });
    fireEvent.click(button);

    expect(button).toBeDisabled();
    expect(button).toHaveAttribute("aria-disabled", "true");
    expect(handleClick).not.toHaveBeenCalled();
  });

  it("no ejecuta onClick cuando loading=true", () => {
    const handleClick = vi.fn();

    render(
      <AppButton loading onClick={handleClick}>
        Cargando
      </AppButton>,
    );

    const button = screen.getByRole("button", { name: /cargando/i });
    fireEvent.click(button);

    expect(button).toBeDisabled();
    expect(button).toHaveAttribute("aria-disabled", "true");
    expect(handleClick).not.toHaveBeenCalled();
  });

  it("usa htmlType='button' por defecto", () => {
    render(<AppButton>Default</AppButton>);

    expect(screen.getByRole("button", { name: "Default" })).toHaveAttribute(
      "type",
      "button",
    );
  });

  it("renderiza leftIcon y rightIcon junto al texto", () => {
    render(
      <AppButton
        leftIcon={<span data-testid="left-icon">L</span>}
        rightIcon={<span data-testid="right-icon">R</span>}
      >
        Accion
      </AppButton>,
    );

    expect(screen.getByTestId("left-icon")).toBeInTheDocument();
    expect(screen.getByTestId("right-icon")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Accion" })).toBeInTheDocument();
  });

  it("renderiza icon-only cuando solo se pasa icon", () => {
    render(
      <AppButton icon={<span data-testid="only-icon">I</span>} aria-label="Editar" />,
    );

    const button = screen.getByRole("button", { name: "Editar" });
    expect(button).toHaveClass(styles.iconOnly);
    expect(screen.getByTestId("only-icon")).toBeInTheDocument();
  });

  it("exige aria-label en modo icon-only", () => {
    expect(() =>
      render(<AppButton icon={<span data-testid="missing-label">I</span>} />),
    ).toThrow("AppButton icon-only requiere `aria-label`.");
  });

  it("renderiza tooltip cuando se pasa la prop", async () => {
    render(<AppButton tooltip="Accion segura">Tooltip</AppButton>);

    fireEvent.mouseOver(screen.getByRole("button", { name: "Tooltip" }));

    expect(await screen.findByText("Accion segura")).toBeInTheDocument();
  });

  it("aplica clases de variant y fullWidth", () => {
    render(
      <AppButton variant="danger" fullWidth>
        Eliminar
      </AppButton>,
    );

    const button = screen.getByRole("button", { name: "Eliminar" });
    expect(button).toHaveClass(styles.variantDanger);
    expect(button).toHaveClass(styles.fullWidth);
  });

  it("respeta tamanos sm, md y lg", () => {
    const { rerender } = render(<AppButton size="sm">Small</AppButton>);
    expect(screen.getByRole("button", { name: "Small" })).toHaveClass(styles.sizeSm);

    rerender(<AppButton size="md">Medium</AppButton>);
    expect(screen.getByRole("button", { name: "Medium" })).toHaveClass(styles.sizeMd);

    rerender(<AppButton size="lg">Large</AppButton>);
    expect(screen.getByRole("button", { name: "Large" })).toHaveClass(styles.sizeLg);
  });
});
