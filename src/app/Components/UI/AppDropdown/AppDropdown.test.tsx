import { fireEvent, render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AppButton, AppIconActionButton } from "../AppButton";
import { AppDropdown } from "./AppDropdown";

let breakpointState = { md: true };

vi.mock("antd", async () => {
  const actual = await vi.importActual<typeof import("antd")>("antd");
  return {
    ...actual,
    Grid: {
      ...actual.Grid,
      useBreakpoint: () => breakpointState,
    },
  };
});

describe("AppDropdown [SPEC:APP-DROPDOWN-001]", () => {
  beforeEach(() => {
    breakpointState = { md: true };
  });

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

  it("renderiza submenus jerarquicos con iconografia usando AppButton como trigger", async () => {
    render(
      <AppDropdown
        trigger={<AppButton>Exportar</AppButton>}
        items={[
          {
            key: "excel",
            label: "Exportar en Excel",
            leftIcon: <span data-testid="excel-icon">X</span>,
            children: [
              { key: "excel-all", label: "Exportar Todo" },
              { key: "excel-selected", label: "Exportar Seleccionados" },
            ],
          },
        ]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));

    expect(await screen.findByText("Exportar en Excel")).toBeInTheDocument();
    expect(screen.getByTestId("excel-icon")).toBeInTheDocument();
  });

  it("en mobile renderiza children en modo inline para abrirlos debajo del item padre", async () => {
    breakpointState = { md: false };

    render(
      <AppDropdown
        trigger={<AppButton>Exportar</AppButton>}
        items={[
          {
            key: "excel",
            label: "Exportar en Excel",
            children: [{ key: "excel-all", label: "Exportar Todo" }],
          },
        ]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));

    expect(await screen.findByText("Exportar en Excel")).toBeInTheDocument();
    expect(await screen.findByText("Exportar Todo")).toBeInTheDocument();
  });

  it("en mobile preserva nietos al aplanar submenus anidados", async () => {
    breakpointState = { md: false };

    render(
      <AppDropdown
        trigger={<AppButton>Exportar</AppButton>}
        items={[
          {
            key: "excel",
            label: "Exportar en Excel",
            children: [
              {
                key: "excel-group",
                label: "Opciones avanzadas",
                children: [{ key: "excel-all", label: "Exportar Todo" }],
              },
            ],
          },
        ]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));

    expect(await screen.findByText("Exportar en Excel")).toBeInTheDocument();
    expect(await screen.findByText("Opciones avanzadas")).toBeInTheDocument();
    expect(await screen.findByText("Exportar Todo")).toBeInTheDocument();
  });

  it("soporta divisores sin romper items ni submenus existentes", async () => {
    render(
      <AppDropdown
        trigger={<AppButton>Acciones</AppButton>}
        items={[
          { key: "export", label: "Exportar" },
          { key: "divider-1", type: "divider" },
          { key: "share", label: "Compartir" },
        ]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Acciones" }));

    expect(await screen.findByText("Exportar")).toBeInTheDocument();
    expect(screen.getByText("Compartir")).toBeInTheDocument();
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

  it("acepta AppIconActionButton como trigger compatible", async () => {
    render(
      <AppDropdown
        trigger={
          <AppIconActionButton
            icon={<span aria-hidden="true">...</span>}
            aria-label="Abrir acciones"
            tooltip="Abrir acciones"
          />
        }
        items={[{ key: "export", label: "Exportar" }]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Abrir acciones" }));

    expect(await screen.findByText("Exportar")).toBeInTheDocument();
  });
});
