import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppButton } from "../AppButton";
import { AppDropdown } from "./AppDropdown";

describe("AppDropdown [SPEC:APP-DROPDOWN-001]", () => {
  it("renderiza items y ejecuta callbacks al seleccionar una accion", async () => {
    const onSelect = vi.fn();

    render(
      <AppDropdown
        trigger={<AppButton>Acciones</AppButton>}
        items={[
          { key: "export", label: "Exportar", onSelect },
          { key: "share", label: "Compartir" },
        ]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Acciones" }));

    expect(await screen.findByText("Exportar")).toBeInTheDocument();
    expect(screen.getByText("Compartir")).toBeInTheDocument();

    fireEvent.click(screen.getByText("Exportar"));

    expect(onSelect).toHaveBeenCalledTimes(1);
  });

  it("preserva metadata visual y evita ejecutar items deshabilitados", async () => {
    const onDelete = vi.fn();

    render(
      <AppDropdown
        trigger={<AppButton>Mas</AppButton>}
        items={[
          {
            key: "delete",
            label: "Eliminar",
            icon: <span data-testid="delete-icon">!</span>,
            danger: true,
            disabled: true,
            onSelect: onDelete,
          },
        ]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Mas" }));

    expect(await screen.findByText("Eliminar")).toBeInTheDocument();
    expect(screen.getByTestId("delete-icon")).toBeInTheDocument();

    fireEvent.click(screen.getByText("Eliminar"));

    expect(onDelete).not.toHaveBeenCalled();
  });
});

describe("AppDropdown [SPEC:APP-DROPDOWN-002]", () => {
  it("impide apertura cuando el dropdown esta deshabilitado", () => {
    render(
      <AppDropdown
        disabled
        trigger={<AppButton>Bloqueado</AppButton>}
        items={[{ key: "export", label: "Exportar" }]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Bloqueado" }));

    expect(screen.queryByText("Exportar")).not.toBeInTheDocument();
  });

  it("soporta apertura controlada e informa cambios de visibilidad", async () => {
    const onOpenChange = vi.fn();

    const { rerender } = render(
      <AppDropdown
        trigger={<AppButton>Controlado</AppButton>}
        items={[{ key: "export", label: "Exportar" }]}
        open={false}
        onOpenChange={onOpenChange}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Controlado" }));

    expect(onOpenChange).toHaveBeenCalledWith(true);
    expect(screen.queryByText("Exportar")).not.toBeInTheDocument();

    rerender(
      <AppDropdown
        trigger={<AppButton>Controlado</AppButton>}
        items={[{ key: "export", label: "Exportar" }]}
        open
        onOpenChange={onOpenChange}
      />,
    );

    expect(await screen.findByText("Exportar")).toBeInTheDocument();
  });

  it("requiere nombre accesible para triggers icon-only", () => {
    expect(() =>
      render(
        <AppDropdown
          trigger={<AppButton icon={<span aria-hidden="true">...</span>} />}
          items={[{ key: "export", label: "Exportar" }]}
        />,
      ),
    ).toThrow(/nombre accesible/i);
  });
});
