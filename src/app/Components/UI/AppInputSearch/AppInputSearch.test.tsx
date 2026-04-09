import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi, afterEach } from "vitest";
import { AppInputSearch } from "./AppInputSearch";
import styles from "./AppInputSearch.module.css";

describe("AppInputSearch [SPEC:app-input-search]", () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it("renderiza valor controlado, placeholder y nombre accesible", () => {
    const { rerender } = render(
      <AppInputSearch
        aria-label="Buscar documentos"
        placeholder="Buscar por radicado"
        value="ABC"
      />,
    );

    const input = screen.getByRole("combobox", { name: "Buscar documentos" });
    expect(input).toHaveValue("ABC");
    expect(input).toHaveAttribute("placeholder", "Buscar por radicado");

    rerender(
      <AppInputSearch
        aria-label="Buscar documentos"
        placeholder="Buscar por radicado"
        value="XYZ"
      />,
    );

    expect(screen.getByRole("combobox", { name: "Buscar documentos" })).toHaveValue("XYZ");
  });

  it("usa defaultValue en modo no controlado", () => {
    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        defaultValue="Inicial"
      />,
    );

    expect(screen.getByRole("combobox", { name: "Buscar documentos" })).toHaveValue(
      "Inicial",
    );
  });

  it("notifica onChange con el valor en cada cambio", () => {
    const handleChange = vi.fn();

    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        value=""
        onChange={handleChange}
      />,
    );

    fireEvent.change(screen.getByRole("combobox", { name: "Buscar documentos" }), {
      target: { value: "radicado" },
    });

    expect(handleChange).toHaveBeenCalledWith("radicado");
  });

  it("ejecuta onSearch por Enter y por click en el icono", () => {
    const handleSearch = vi.fn();

    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        value="radicado"
        onSearch={handleSearch}
      />,
    );

    fireEvent.keyDown(screen.getByRole("combobox", { name: "Buscar documentos" }), {
      key: "Enter",
    });
    fireEvent.click(screen.getByRole("button", { name: "Buscar" }));

    expect(handleSearch).toHaveBeenNthCalledWith(1, "radicado");
    expect(handleSearch).toHaveBeenNthCalledWith(2, "radicado");
  });

  it("ejecuta onSearch por debounce y Enter cancela el debounce pendiente", () => {
    vi.useFakeTimers();
    const handleChange = vi.fn();
    const handleSearch = vi.fn();

    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        debounceMs={300}
        onChange={handleChange}
        onSearch={handleSearch}
      />,
    );

    fireEvent.change(screen.getByRole("combobox", { name: "Buscar documentos" }), {
      target: { value: "radicado" },
    });
    fireEvent.keyDown(screen.getByRole("combobox", { name: "Buscar documentos" }), {
      key: "Enter",
    });

    expect(handleSearch).toHaveBeenCalledWith("radicado");

    vi.advanceTimersByTime(300);

    expect(handleSearch).toHaveBeenCalledTimes(1);
  });

  it("respeta minLength y desactiva debounce cuando debounceMs es 0", () => {
    vi.useFakeTimers();
    const handleSearch = vi.fn();

    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        debounceMs={0}
        minLength={3}
        onSearch={handleSearch}
      />,
    );

    const input = screen.getByRole("combobox", { name: "Buscar documentos" });
    fireEvent.change(input, { target: { value: "ra" } });
    fireEvent.keyDown(input, { key: "Enter" });
    fireEvent.click(screen.getByRole("button", { name: "Buscar" }));
    vi.advanceTimersByTime(300);

    expect(handleSearch).not.toHaveBeenCalled();
  });

  it("limpia con botón y Escape sin disparar onSearch vacío", () => {
    const handleChange = vi.fn();
    const handleClear = vi.fn();
    const handleSearch = vi.fn();

    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        value="radicado"
        clearOnEscape
        onChange={handleChange}
        onClear={handleClear}
        onSearch={handleSearch}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Limpiar" }));

    expect(handleChange).toHaveBeenCalledWith("");
    expect(handleClear).toHaveBeenCalledTimes(1);
    expect(handleSearch).not.toHaveBeenCalledWith("");
  });

  it("Escape limpia solo si clearOnEscape está habilitado", () => {
    const handleChange = vi.fn();

    const { rerender } = render(
      <AppInputSearch
        aria-label="Buscar documentos"
        value="radicado"
        onChange={handleChange}
      />,
    );

    fireEvent.keyDown(screen.getByRole("combobox", { name: "Buscar documentos" }), {
      key: "Escape",
    });
    expect(handleChange).not.toHaveBeenCalled();

    rerender(
      <AppInputSearch
        aria-label="Buscar documentos"
        value="radicado"
        clearOnEscape
        onChange={handleChange}
      />,
    );

    fireEvent.keyDown(screen.getByRole("combobox", { name: "Buscar documentos" }), {
      key: "Escape",
    });
    expect(handleChange).toHaveBeenCalledWith("");
  });

  it("mantiene loading editable, foco y estados error/disabled", () => {
    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        loading
        error
        helperText="Busqueda no disponible"
      />,
    );

    const input = screen.getByRole("combobox", { name: "Buscar documentos" });
    fireEvent.focus(input);

    expect(input).not.toBeDisabled();
    expect(document.querySelector(".ant-select-focused")).toBeInTheDocument();
    expect(input).toHaveAttribute("aria-invalid", "true");
    expect(screen.getByText("Busqueda no disponible")).toBeInTheDocument();
    expect(document.querySelector(".anticon-loading")).toBeInTheDocument();
  });

  it("disabled tiene prioridad sobre loading y oculta clear", () => {
    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        disabled
        loading
        value="radicado"
      />,
    );

    expect(screen.getByRole("combobox", { name: "Buscar documentos" })).toBeDisabled();
    expect(screen.queryByRole("button", { name: "Limpiar" })).not.toBeInTheDocument();
  });

  it("renderiza opciones sin mutarlas y aplica clases de tamaño", () => {
    const options = [{ value: "radicado", label: "Radicado" }];

    render(
      <AppInputSearch
        aria-label="Buscar documentos"
        options={options}
        size="sm"
      />,
    );

    const input = screen.getByRole("combobox", { name: "Buscar documentos" });
    const inputWrapper = input.closest(".ant-input-affix-wrapper");
    expect(inputWrapper).toHaveClass(styles.input);
    expect(inputWrapper).toHaveClass(styles.sm);
    expect(options).toEqual([{ value: "radicado", label: "Radicado" }]);
  });
});
