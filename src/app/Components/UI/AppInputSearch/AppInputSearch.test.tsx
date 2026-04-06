import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppInputSearch } from "./AppInputSearch";
import styles from "./AppInputSearch.module.css";

describe("AppInputSearch [SPEC:app-input-search]", () => {
  it("renderiza valor controlado, placeholder y nombre accesible", () => {
    const { rerender } = render(
      <AppInputSearch
        aria-label="Buscar documentos"
        placeholder="Buscar por radicado"
        value="ABC"
      />,
    );

    const input = screen.getByRole("textbox", { name: "Buscar documentos" });
    expect(input).toHaveValue("ABC");
    expect(input).toHaveAttribute("placeholder", "Buscar por radicado");

    rerender(
      <AppInputSearch
        aria-label="Buscar documentos"
        placeholder="Buscar por radicado"
        value="XYZ"
      />,
    );

    expect(screen.getByRole("textbox", { name: "Buscar documentos" })).toHaveValue("XYZ");
  });

  it("notifica cambios sin administrar el estado de busqueda internamente", () => {
    const handleChange = vi.fn();

    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        value=""
        onChange={handleChange}
      />,
    );

    fireEvent.change(screen.getByRole("textbox", { name: "Buscar documentos" }), {
      target: { value: "radicado" },
    });

    expect(handleChange).toHaveBeenCalledTimes(1);
  });

  it("preserva estados disabled y error delegados por AppInput", () => {
    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        disabled
        error
        helperText="Busqueda no disponible"
      />,
    );

    const input = screen.getByRole("textbox", { name: "Buscar documentos" });
    expect(input).toBeDisabled();
    expect(input).toHaveAttribute("aria-invalid", "true");
    expect(screen.getByText("Busqueda no disponible")).toBeInTheDocument();
  });

  it("mantiene el icono de busqueda como decorativo", () => {
    const { container } = render(
      <AppInputSearch aria-label="Buscar documentos" value="" onChange={vi.fn()} />,
    );

    expect(screen.getByRole("textbox", { name: "Buscar documentos" })).toBeInTheDocument();
    expect(container.querySelector(".anticon-search")).toBeInTheDocument();
    expect(screen.queryByRole("img", { name: /search/i })).not.toBeInTheDocument();
  });

  it("permite ocultar el icono y componer className externa", () => {
    const { container } = render(
      <AppInputSearch
        aria-label="Buscar documentos"
        showIcon={false}
        className="custom-search"
      />,
    );

    const input = screen.getByRole("textbox", { name: "Buscar documentos" });
    expect(input).toHaveClass(styles.input);
    expect(input).toHaveClass("custom-search");
    expect(container.querySelector(".anticon-search")).not.toBeInTheDocument();
  });
});
