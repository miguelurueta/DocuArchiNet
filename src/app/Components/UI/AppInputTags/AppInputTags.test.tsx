import { act, fireEvent, render, screen, within } from "@testing-library/react";
import type { ComponentProps } from "react";
import { describe, expect, it, vi, afterEach } from "vitest";
import { AppInputTags } from "./AppInputTags";
import styles from "./AppInputTags.module.css";

const renderTags = (props: Partial<ComponentProps<typeof AppInputTags>> = {}) =>
  render(
    <AppInputTags
      aria-label="Destinatarios"
      onAddTag={vi.fn()}
      onRemoveAll={vi.fn()}
      onRemoveTag={vi.fn()}
      onSearch={vi.fn()}
      placeholder="Buscar destinatario"
      {...props}
    />,
  );

describe("AppInputTags [SPEC:app-input-tags]", () => {
  afterEach(() => {
    vi.useRealTimers();
  });

  it("renderiza el componente reusable", () => {
    renderTags();

    expect(screen.getByLabelText("Destinatarios")).toBeInTheDocument();
  });

  it("renderiza valor controlado y no invoca servicios externos", () => {
    renderTags({ value: ["Ana"] });

    expect(screen.getByText("Ana")).toBeInTheDocument();
    expect(screen.getByLabelText("Destinatarios")).toHaveAttribute(
      "placeholder",
      "Buscar destinatario",
    );
  });

  it("usa defaultValue en modo no controlado", () => {
    renderTags({ defaultValue: ["Ana"] });

    expect(screen.getByText("Ana")).toBeInTheDocument();
  });

  it("en modo single reemplaza el tag visible al confirmar", () => {
    const onAddTag = vi.fn();
    renderTags({ defaultValue: ["Ana"], mode: "single", onAddTag });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Luis" } });
    fireEvent.keyDown(input, { key: "Enter" });

    expect(onAddTag).toHaveBeenCalledWith("Luis");
    expect(screen.queryByText("Ana")).not.toBeInTheDocument();
    expect(screen.getByText("Luis")).toBeInTheDocument();
  });

  it("en modo multiple acumula tags sin duplicar", () => {
    const onAddTag = vi.fn();
    renderTags({ defaultValue: ["Ana"], mode: "multiple", onAddTag });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Luis" } });
    fireEvent.keyDown(input, { key: "Enter" });
    fireEvent.change(input, { target: { value: "Luis" } });
    fireEvent.keyDown(input, { key: "Enter" });

    expect(onAddTag).toHaveBeenCalledTimes(2);
    expect(screen.getByText("Ana")).toBeInTheDocument();
    expect(screen.getAllByText("Luis")).toHaveLength(1);
  });

  it("elimina un tag y elimina todos con acciones accesibles", () => {
    const onRemoveTag = vi.fn();
    const onRemoveAll = vi.fn();
    renderTags({
      defaultValue: ["Ana", "Luis"],
      onRemoveAll,
      onRemoveTag,
    });

    fireEvent.click(screen.getByRole("button", { name: "Eliminar Ana" }));
    expect(onRemoveTag).toHaveBeenCalledWith("Ana");
    expect(screen.queryByText("Ana")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Eliminar todos" }));
    expect(onRemoveAll).toHaveBeenCalledTimes(1);
    expect(screen.queryByText("Luis")).not.toBeInTheDocument();
  });

  it("dispara busqueda con debounce y cancela el pendiente con Enter", () => {
    vi.useFakeTimers();
    const onSearch = vi.fn();
    renderTags({ debounceMs: 250, minLength: 2, onSearch });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "An" } });
    fireEvent.keyDown(input, { key: "Enter" });

    expect(onSearch).toHaveBeenCalledWith("An");
    expect(onSearch).toHaveBeenCalledTimes(1);

    act(() => {
      vi.advanceTimersByTime(250);
    });

    expect(onSearch).toHaveBeenCalledTimes(1);
  });

  it("bloquea busquedas menores a minLength y debounceMs 0 dispara sin delay", () => {
    const onSearch = vi.fn();
    renderTags({ debounceMs: 0, minLength: 3, onSearch });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "An" } });
    expect(onSearch).not.toHaveBeenCalled();

    fireEvent.change(input, { target: { value: "Ana" } });
    expect(onSearch).toHaveBeenCalledWith("Ana");
  });

  it("el click en icono dispara busqueda inmediata sin duplicados", () => {
    vi.useFakeTimers();
    const onSearch = vi.fn();
    renderTags({ debounceMs: 250, onSearch });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Ana" } });
    fireEvent.click(screen.getByRole("button", { name: "Buscar" }));

    expect(onSearch).toHaveBeenCalledWith("Ana");
    expect(onSearch).toHaveBeenCalledTimes(1);

    act(() => {
      vi.advanceTimersByTime(250);
    });

    expect(onSearch).toHaveBeenCalledTimes(1);
  });

  it("limpia con boton y Escape sin disparar onSearch vacio", () => {
    const onSearch = vi.fn();
    renderTags({ clearOnEscape: true, onSearch });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Ana" } });
    fireEvent.click(screen.getByRole("button", { name: "Limpiar" }));
    expect(input).toHaveValue("");
    expect(onSearch).not.toHaveBeenCalledWith("");

    fireEvent.change(input, { target: { value: "Luis" } });
    fireEvent.keyDown(input, { key: "Escape" });
    expect(input).toHaveValue("");
    expect(onSearch).not.toHaveBeenCalledWith("");
  });

  it("loading no bloquea el input y disabled si bloquea interacciones", () => {
    const { rerender } = renderTags({ loading: true });

    const input = screen.getByLabelText("Destinatarios");
    expect(screen.getByLabelText("Cargando")).toBeInTheDocument();
    expect(input).not.toBeDisabled();

    rerender(
      <AppInputTags
        aria-label="Destinatarios"
        defaultValue={["Ana"]}
        loading
        onAddTag={vi.fn()}
        onRemoveAll={vi.fn()}
        onRemoveTag={vi.fn()}
        onSearch={vi.fn()}
        selectDisabled
      />,
    );

    expect(screen.getByLabelText("Destinatarios")).toBeDisabled();
    expect(screen.queryByRole("button", { name: "Eliminar Ana" })).not.toBeInTheDocument();
  });

  it("renderiza sugerencias, permite seleccionarlas y mantiene accesibilidad", () => {
    const onAddTag = vi.fn();
    renderTags({
      onAddTag,
      options: [{ label: "Ana Perez", value: "ana" }],
    });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Ana" } });
    fireEvent.keyDown(input, { key: "ArrowDown" });

    const option = screen.getByText("Ana Perez");
    fireEvent.click(option);

    expect(onAddTag).toHaveBeenCalledWith("ana");
  });

  it("renderiza opciones con metadata sin interpretar datos de dominio", () => {
    const onAddTag = vi.fn();
    const options = [
      {
        label: "Ana Perez",
        value: "ana",
        id: 7,
        meta: { endpoint: "usuarios", rawId: 7 },
      },
    ];

    renderTags({ onAddTag, options });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Ana" } });
    fireEvent.click(screen.getByText("Ana Perez"));

    expect(onAddTag).toHaveBeenCalledWith("ana");
    expect(options[0]).toEqual({
      label: "Ana Perez",
      value: "ana",
      id: 7,
      meta: { endpoint: "usuarios", rawId: 7 },
    });
  });

  it("mantiene AppInputTags desacoplado de hooks y servicios de dominio", async () => {
    const source = await import("./AppInputTags?raw");

    expect(source.default).not.toContain("useAutocompleteCamposPlantilla");
    expect(source.default).not.toContain("clienteApi");
    expect(source.default).not.toContain("axios");
    expect(source.default).not.toContain("src/modules");
  });

  it("renderiza acciones secundarias por slot sin bloquear autocomplete", () => {
    const onSearch = vi.fn();
    renderTags({
      loading: true,
      onSearch,
      toolbar: {
        render: () => <button type="button">Accion secundaria</button>,
      },
    });

    const input = screen.getByLabelText("Destinatarios");
    expect(screen.getByRole("button", { name: "Accion secundaria" })).toBeInTheDocument();
    expect(input).not.toBeDisabled();

    fireEvent.change(input, { target: { value: "Ana" } });

    expect(onSearch).toHaveBeenCalledWith("Ana");
  });

  it("aplica clases de size, error, helperText y className externa", () => {
    renderTags({
      className: "custom-tags",
      error: true,
      helperText: "Campo requerido",
      size: "lg",
    });

    expect(screen.getByText("Campo requerido")).toBeInTheDocument();
    expect(screen.getByLabelText("Destinatarios")).toHaveAttribute("aria-invalid", "true");

    const field = screen.getByText("Campo requerido").closest(`.${styles.field}`);
    expect(field).toHaveClass("custom-tags");
  });

  it("agrupa las tags seleccionadas en una lista accesible", () => {
    renderTags({ defaultValue: ["Ana"] });

    const list = screen.getByRole("list", { name: "Etiquetas seleccionadas" });
    expect(within(list).getByRole("listitem")).toHaveTextContent("Ana");
  });
});
