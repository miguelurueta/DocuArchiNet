import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppTableQueryWrapper } from "../AppTableQueryWrapper";
import type { AppTableQueryState } from "../types/appTableQueryState.types";

const createQueryState = (
  overrides: Partial<AppTableQueryState> = {},
): AppTableQueryState => ({
  page: 2,
  pageSize: 25,
  search: "",
  structuredFilters: [],
  ...overrides,
});

describe("AppTableQueryWrapper", () => {
  it("renderiza la estructura completa con rango visible y children", () => {
    render(
      <AppTableQueryWrapper
        queryState={createQueryState()}
        onQueryChange={vi.fn()}
        total={87}
        headerActions={<button type="button">Acción extra</button>}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    expect(screen.getByTestId("app-table-query-wrapper")).toBeInTheDocument();
    expect(screen.getByText("Tabla mock")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Acción extra" })).toBeInTheDocument();
    expect(screen.getByTestId("app-table-query-range")).toHaveTextContent("26-50 de 87");
  });

  it("emite patches simples al cambiar la búsqueda y navegar páginas", () => {
    const onQueryChange = vi.fn();

    render(
      <AppTableQueryWrapper queryState={createQueryState()} onQueryChange={onQueryChange} total={87}>
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    fireEvent.change(screen.getByRole("textbox", { name: "Buscar en la tabla" }), {
      target: { value: "radicado" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Página anterior" }));
    fireEvent.click(screen.getByRole("button", { name: "Página siguiente" }));

    expect(onQueryChange).toHaveBeenNthCalledWith(1, { search: "radicado" });
    expect(onQueryChange).toHaveBeenNthCalledWith(2, { page: 1 });
    expect(onQueryChange).toHaveBeenNthCalledWith(3, { page: 3 });
  });

  it("ejecuta refresh sin alterar el query state y permite cambiar page size", async () => {
    const onQueryChange = vi.fn();
    const onRefresh = vi.fn();

    render(
      <AppTableQueryWrapper
        queryState={createQueryState({ page: 1 })}
        onQueryChange={onQueryChange}
        onRefresh={onRefresh}
        total={87}
      >
        <div>Tabla mock</div>
      </AppTableQueryWrapper>,
    );

    fireEvent.click(screen.getByRole("button", { name: "Actualizar tabla" }));
    expect(onRefresh).toHaveBeenCalledTimes(1);
    expect(onQueryChange).not.toHaveBeenCalled();

    fireEvent.mouseDown(screen.getByLabelText("Cantidad de registros por página"));
    fireEvent.click(await screen.findByText("50 por página"));

    expect(onQueryChange).toHaveBeenCalledWith({ pageSize: 50 });
  });
});
