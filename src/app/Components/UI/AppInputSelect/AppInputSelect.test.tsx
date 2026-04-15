import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppInputSelect } from "./AppInputSelect";
import styles from "./AppInputSelect.module.css";

describe("AppInputSelect [SPEC:app-input-select]", () => {
  it("renderiza placeholder, label y opciones locales", async () => {
    render(
      <AppInputSelect
        label="Tipo"
        placeholder="Seleccione una opcion"
        options={[
          { label: "Radicado", value: "radicado" },
          { label: "Expediente", value: "expediente" },
        ]}
      />,
    );

    expect(screen.getByText("Tipo")).toBeInTheDocument();
    expect(screen.getByText("Seleccione una opcion")).toBeInTheDocument();

    fireEvent.mouseDown(screen.getByRole("combobox"));

    expect(await screen.findByText("Radicado")).toBeInTheDocument();
    expect(screen.getByText("Expediente")).toBeInTheDocument();
  });

  it("propaga onChange con el valor seleccionado", async () => {
    const handleChange = vi.fn();

    render(
      <AppInputSelect
        aria-label="Tipo de documento"
        options={[
          { label: "Radicado", value: "radicado" },
          { label: "Expediente", value: "expediente" },
        ]}
        onChange={handleChange}
      />,
    );

    fireEvent.mouseDown(screen.getByRole("combobox", { name: "Tipo de documento" }));
    fireEvent.click(await screen.findByText("Expediente"));

    expect(handleChange).toHaveBeenCalledWith("expediente", expect.any(Object));
  });

  it("usa fetchOptions y renderiza estado vacio custom", async () => {
    const fetchOptions = vi.fn().mockResolvedValue({ options: [] });

    render(
      <AppInputSelect
        aria-label="Buscar tercero"
        searchable
        fetchOptions={fetchOptions}
        noDataText="Sin resultados"
      />,
    );

    fireEvent.mouseDown(screen.getByRole("combobox", { name: "Buscar tercero" }));
    fireEvent.change(screen.getByRole("combobox", { name: "Buscar tercero" }), {
      target: { value: "miguel" },
    });

    await waitFor(() => {
      expect(fetchOptions).toHaveBeenCalledWith("miguel");
    });

    fireEvent.mouseDown(screen.getByRole("combobox", { name: "Buscar tercero" }));
    expect(await screen.findByText("Sin resultados")).toBeInTheDocument();
  });

  it("respeta loading, disabled y helperText de error", () => {
    render(
      <AppInputSelect
        aria-label="Dependencia"
        disabled
        loading
        error
        helperText="Seleccion obligatoria"
      />,
    );

    const combobox = screen.getByRole("combobox", { name: "Dependencia" });
    const wrapper = combobox.closest(".ant-select");
    expect(wrapper).toHaveClass(styles.selectError);
    expect(screen.getByText("Seleccion obligatoria")).toHaveClass(styles.helperTextError);
    expect(document.querySelector(".ant-select-disabled")).toBeInTheDocument();
  });

  it("aplica clases de tamano shared", () => {
    render(<AppInputSelect aria-label="Rol" size="sm" />);

    const wrapper = screen.getByRole("combobox", { name: "Rol" }).closest(".ant-select");
    expect(wrapper).toHaveClass(styles.select);
    expect(wrapper).toHaveClass(styles.sm);
  });
});
