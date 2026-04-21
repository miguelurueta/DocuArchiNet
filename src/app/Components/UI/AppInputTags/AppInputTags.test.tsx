import { act, fireEvent, render, screen, waitFor, within } from "@testing-library/react";
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
    expect(screen.getByLabelText("Destinatarios")).not.toHaveAttribute("placeholder");
  });

  it("usa defaultValue en modo no controlado", () => {
    renderTags({ defaultValue: ["Ana"] });

    expect(screen.getByText("Ana")).toBeInTheDocument();
  });

  it("en modo single reemplaza el tag visible al confirmar", async () => {
    const onAddTag = vi.fn();
    renderTags({ defaultValue: ["Ana"], mode: "single", onAddTag });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Luis" } });
    fireEvent.keyDown(input, { key: "Enter" });

    await waitFor(() => {
      expect(onAddTag).toHaveBeenCalledWith("Luis");
    });
    expect(screen.queryByText("Ana")).not.toBeInTheDocument();
    expect(screen.getByText("Luis")).toBeInTheDocument();
  });

  it("en modo multiple acumula tags sin duplicar", async () => {
    const onAddTag = vi.fn();
    renderTags({ defaultValue: ["Ana"], mode: "multiple", onAddTag });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Luis" } });
    fireEvent.keyDown(input, { key: "Enter" });
    fireEvent.change(input, { target: { value: "Luis" } });
    fireEvent.keyDown(input, { key: "Enter" });

    await waitFor(() => {
      expect(onAddTag).toHaveBeenCalledTimes(2);
    });
    expect(screen.getByText("Ana")).toBeInTheDocument();
    expect(screen.getAllByText("Luis")).toHaveLength(1);
  });

  it("variant email agrega correos separados por coma y normaliza a lowercase", async () => {
    const onAddTag = vi.fn();
    renderTags({ variant: "email", onAddTag });

    const input = screen.getByLabelText("Destinatarios");
    expect(input).toHaveAttribute("type", "text");
    expect(input).toHaveAttribute("inputmode", "email");
    expect(input).toHaveAttribute("autocomplete", "email");

    fireEvent.change(input, { target: { value: "TEST@EXAMPLE.COM, otra@dom.com" } });
    fireEvent.keyDown(input, { key: "Enter" });

    await waitFor(() => {
      expect(onAddTag).toHaveBeenCalledWith("test@example.com");
    });
    expect(onAddTag).toHaveBeenCalledWith("otra@dom.com");
  });

  it("variant email valida formato y muestra helperText cuando es invalido", async () => {
    const onAddTag = vi.fn();
    renderTags({ variant: "email", onAddTag });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "no-es-correo" } });
    fireEvent.keyDown(input, { key: "Enter" });

    await waitFor(() => {
      expect(screen.getByText(/Correo inválido/i)).toBeInTheDocument();
    });
    expect(onAddTag).not.toHaveBeenCalled();
    expect(input).toHaveAttribute("aria-invalid", "true");
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

  it("limpia con Escape sin disparar onSearch vacio y no renderiza boton de limpiar", () => {
    const onSearch = vi.fn();
    renderTags({ clearOnEscape: true, onSearch });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Ana" } });
    expect(screen.queryByRole("button", { name: "Limpiar" })).toBeNull();

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

  it("no duplica el tag cuando Enter confirma una sugerencia del autocomplete", async () => {
    const onAddTag = vi.fn();
    renderTags({
      onAddTag,
      options: [{ label: "Ana", value: "ana" }],
    });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.change(input, { target: { value: "Ana" } });
    fireEvent.keyDown(input, { key: "Enter" });

    await waitFor(() => {
      expect(onAddTag).toHaveBeenCalledTimes(1);
    });
    expect(onAddTag).toHaveBeenCalledWith("ana");
    expect(input).toHaveValue("");
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

  it("renderiza las tags dentro del campo sin icono de busqueda", () => {
    renderTags({ defaultValue: ["Ana"] });

    const input = screen.getByLabelText("Destinatarios");
    const wrapper = input.closest(".ant-input-affix-wrapper");
    const list = screen.getByRole("list", { name: "Etiquetas seleccionadas" });

    expect(wrapper).toContainElement(list);
    expect(screen.queryByRole("button", { name: "Buscar" })).toBeNull();
  });

  it("no abre autocomplete al enfocar vacio y solo muestra opciones al escribir", () => {
    renderTags({
      options: [{ label: "Ana Perez", value: "ana" }],
    });

    const input = screen.getByLabelText("Destinatarios");
    fireEvent.focus(input);

    expect(screen.queryByText("Ana Perez")).toBeNull();

    fireEvent.change(input, { target: { value: "Ana" } });

    expect(screen.getByText("Ana Perez")).toBeInTheDocument();
  });

  it("oculta el placeholder al enfocar y lo restaura al salir", () => {
    renderTags();

    const input = screen.getByLabelText("Destinatarios");
    expect(input).toHaveAttribute("placeholder", "Buscar destinatario");

    fireEvent.focus(input);
    expect(input).not.toHaveAttribute("placeholder");

    fireEvent.blur(input);
    expect(input).toHaveAttribute("placeholder", "Buscar destinatario");
  });

  it("oculta el placeholder cuando ya existen tags seleccionadas", () => {
    renderTags({ defaultValue: ["Ana"] });

    const input = screen.getByLabelText("Destinatarios");
    expect(input).not.toHaveAttribute("placeholder");
  });

  it("mueve eliminar todos dentro del input", () => {
    renderTags({ defaultValue: ["Ana", "Luis"] });

    const input = screen.getByLabelText("Destinatarios");
    const controlRow = input.closest(`.${styles.controlRow}`);
    const removeAll = screen.getByRole("button", { name: "Eliminar todos" });

    expect(controlRow).toContainElement(removeAll);
  });
});
