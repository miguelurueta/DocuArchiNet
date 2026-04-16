import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppCheckbox, AppCheckboxCheckAll, AppCheckboxGroup } from "./AppCheckbox";
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

describe("AppCheckboxGroup [SPEC:APP-CHECKBOX-002]", () => {
  const options = [
    { label: "Correo", value: "correo" },
    { label: "SMS", value: "sms" },
    { label: "WhatsApp", value: "whatsapp", disabled: true },
  ] as const;

  it("renderiza grupo vertical con label y helperText", () => {
    render(
      <AppCheckboxGroup
        helperText="Seleccion multiple"
        label="Canales"
        onChange={vi.fn()}
        options={options}
        value={["correo"]}
      />,
    );

    expect(screen.getByRole("group")).toHaveClass(styles.groupVertical);
    expect(screen.getByText("Canales")).toBeInTheDocument();
    expect(screen.getByText("Seleccion multiple")).toBeInTheDocument();
    expect(screen.getByRole("checkbox", { name: /Correo/i })).toBeChecked();
  });

  it("renderiza grupo horizontal", () => {
    render(
      <AppCheckboxGroup direction="horizontal" onChange={vi.fn()} options={options} value={[]} />,
    );

    expect(screen.getByRole("group")).toHaveClass(styles.groupHorizontal);
  });

  it("dispara onChange con el nuevo arreglo controlado", () => {
    const handleChange = vi.fn();
    render(<AppCheckboxGroup onChange={handleChange} options={options} value={["correo"]} />);

    fireEvent.click(screen.getByRole("checkbox", { name: /SMS/i }));

    expect(handleChange).toHaveBeenCalledWith(["correo", "sms"]);
  });

  it("remueve un valor seleccionado cuando se desmarca", () => {
    const handleChange = vi.fn();
    render(<AppCheckboxGroup onChange={handleChange} options={options} value={["correo"]} />);

    fireEvent.click(screen.getByRole("checkbox", { name: /Correo/i }));

    expect(handleChange).toHaveBeenCalledWith([]);
  });

  it("respeta disabled a nivel de grupo y por opcion", () => {
    const handleChange = vi.fn();
    const { rerender } = render(
      <AppCheckboxGroup onChange={handleChange} options={options} value={[]} />,
    );

    expect(screen.getByRole("checkbox", { name: /WhatsApp/i })).toBeDisabled();

    rerender(<AppCheckboxGroup disabled onChange={handleChange} options={options} value={[]} />);

    fireEvent.click(screen.getByRole("checkbox", { name: /Correo/i }));
    expect(screen.getByRole("checkbox", { name: /Correo/i })).toBeDisabled();
    expect(handleChange).not.toHaveBeenCalled();
  });

  it("propaga tamanos a los items del grupo", () => {
    render(<AppCheckboxGroup onChange={vi.fn()} options={options} size="lg" value={[]} />);

    expect(screen.getByText("Correo").closest(`.${styles.checkbox}`)).toHaveClass(styles.sizeLG);
  });

  it("renderiza helperText y error del grupo", () => {
    render(
      <AppCheckboxGroup
        error
        helperText="Campo requerido"
        onChange={vi.fn()}
        options={options}
        value={[]}
      />,
    );

    expect(screen.getByText("Campo requerido")).toHaveClass(styles.helperTextError);
    expect(screen.getByRole("group")).toHaveClass(styles.groupError);
  });
});

describe("AppCheckboxCheckAll [SPEC:APP-CHECKBOX-003]", () => {
  const options = [
    { label: "Correo", value: "correo" },
    { label: "SMS", value: "sms" },
    { label: "WhatsApp", value: "whatsapp", disabled: true },
  ] as const;

  it("renderiza el checkbox maestro y el grupo reutilizando la familia shared", () => {
    render(
      <AppCheckboxCheckAll
        checkAllLabel="Todos los canales"
        onChange={vi.fn()}
        options={options}
        value={[]}
      />,
    );

    expect(screen.getByRole("checkbox", { name: /Todos los canales/i })).toBeInTheDocument();
    expect(screen.getByRole("checkbox", { name: /Correo/i })).toBeInTheDocument();
    expect(screen.getByRole("checkbox", { name: /SMS/i })).toBeInTheDocument();
  });

  it("selecciona todos los valores habilitados", () => {
    const handleChange = vi.fn();
    render(
      <AppCheckboxCheckAll
        checkAllLabel="Todos los canales"
        onChange={handleChange}
        options={options}
        value={[]}
      />,
    );

    fireEvent.click(screen.getByRole("checkbox", { name: /Todos los canales/i }));

    expect(handleChange).toHaveBeenCalledWith(["correo", "sms"]);
  });

  it("limpia todos los valores cuando ya estan seleccionados", () => {
    const handleChange = vi.fn();
    render(
      <AppCheckboxCheckAll
        checkAllLabel="Todos los canales"
        onChange={handleChange}
        options={options}
        value={["correo", "sms"]}
      />,
    );

    fireEvent.click(screen.getByRole("checkbox", { name: /Todos los canales/i }));

    expect(handleChange).toHaveBeenCalledWith([]);
  });

  it("muestra indeterminate con seleccion parcial", () => {
    render(
      <AppCheckboxCheckAll
        checkAllLabel="Todos los canales"
        onChange={vi.fn()}
        options={options}
        value={["correo"]}
      />,
    );

    expect(
      screen
        .getByRole("checkbox", { name: /Todos los canales/i })
        .closest(".ant-checkbox-wrapper")
        ?.querySelector(".ant-checkbox-indeterminate"),
    ).toBeInTheDocument();
  });

  it("respeta disabled y no dispara cambios", () => {
    const handleChange = vi.fn();
    render(
      <AppCheckboxCheckAll
        checkAllLabel="Todos los canales"
        disabled
        onChange={handleChange}
        options={options}
        value={[]}
      />,
    );

    fireEvent.click(screen.getByRole("checkbox", { name: /Todos los canales/i }));
    fireEvent.click(screen.getByRole("checkbox", { name: /Correo/i }));

    expect(handleChange).not.toHaveBeenCalled();
    expect(screen.getByRole("checkbox", { name: /Todos los canales/i })).toBeDisabled();
    expect(screen.getByRole("checkbox", { name: /Correo/i })).toBeDisabled();
  });

  it("renderiza helperText del control maestro y mantiene el grupo debajo", () => {
    render(
      <AppCheckboxCheckAll
        checkAllLabel="Todos los canales"
        helperText="Selecciona todos los canales disponibles"
        onChange={vi.fn()}
        options={options}
        value={[]}
      />,
    );

    expect(screen.getByText("Selecciona todos los canales disponibles")).toBeInTheDocument();
    expect(screen.getByText("Correo").closest(`.${styles.checkAllGroup}`)).toBeInTheDocument();
  });
});
