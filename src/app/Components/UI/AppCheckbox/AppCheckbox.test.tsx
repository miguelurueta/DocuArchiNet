import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppCheckbox } from "./AppCheckbox";
import styles from "./AppCheckbox.module.css";

describe("AppCheckbox [SPEC:APP-CHECKBOX-001]", () => {
  it("renderiza label y helperText", () => {
    render(<AppCheckbox label="Acepto terminos" helperText="Requerido" />);

    expect(screen.getByRole("checkbox", { name: /Acepto terminos/i })).toBeInTheDocument();
    expect(screen.getByText("Requerido")).toBeInTheDocument();
  });

  it("respeta checked en modo controlado", () => {
    render(<AppCheckbox label="Controlado" checked onChange={vi.fn()} />);

    expect(screen.getByRole("checkbox", { name: /Controlado/i })).toBeChecked();
  });

  it("respeta defaultChecked en modo no controlado", () => {
    render(<AppCheckbox label="Inicial" defaultChecked />);

    const checkbox = screen.getByRole("checkbox", { name: /Inicial/i });
    expect(checkbox).toBeChecked();

    fireEvent.click(checkbox);
    expect(checkbox).not.toBeChecked();
  });

  it("dispara onChange con checked y event", () => {
    const handleChange = vi.fn();
    render(<AppCheckbox label="Cambio" onChange={handleChange} />);

    fireEvent.click(screen.getByRole("checkbox", { name: /Cambio/i }));

    expect(handleChange).toHaveBeenCalledTimes(1);
    expect(handleChange).toHaveBeenCalledWith(
      true,
      expect.objectContaining({ target: expect.objectContaining({ checked: true }) }),
    );
  });

  it("respeta disabled", () => {
    const handleChange = vi.fn();
    render(<AppCheckbox label="Bloqueado" disabled onChange={handleChange} />);

    const checkbox = screen.getByRole("checkbox", { name: /Bloqueado/i });
    fireEvent.click(checkbox);

    expect(checkbox).toBeDisabled();
    expect(handleChange).not.toHaveBeenCalled();
  });

  it("renderiza estado indeterminate", () => {
    render(<AppCheckbox label="Parcial" indeterminate />);

    expect(
      screen
        .getByRole("checkbox", { name: /Parcial/i })
        .closest(".ant-checkbox-wrapper")
        ?.querySelector(".ant-checkbox-indeterminate"),
    ).toBeInTheDocument();
  });

  it("respeta tamanos sm, md y lg", () => {
    const { rerender } = render(<AppCheckbox label="Small" size="sm" />);
    expect(screen.getByText("Small").closest(`.${styles.checkbox}`)).toHaveClass(styles.sizeSM);

    rerender(<AppCheckbox label="Medium" size="md" />);
    expect(screen.getByText("Medium").closest(`.${styles.checkbox}`)).toHaveClass(
      styles.sizeMD,
    );

    rerender(<AppCheckbox label="Large" size="lg" />);
    expect(screen.getByText("Large").closest(`.${styles.checkbox}`)).toHaveClass(styles.sizeLG);
  });

  it("reenvia atributos aria al control", () => {
    render(
      <AppCheckbox
        aria-describedby="checkbox-help"
        aria-label="Solo checkbox"
        helperText="Texto"
      />,
    );

    expect(screen.getByRole("checkbox", { name: "Solo checkbox" })).toHaveAttribute(
      "aria-describedby",
      expect.stringContaining("checkbox-help"),
    );
  });
});
