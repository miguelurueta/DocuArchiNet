import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppTabs } from "./AppTabs";

const baseItems = [
  {
    key: "general",
    label: "General",
    children: <div>Contenido general</div>,
  },
  {
    key: "historial",
    label: "Historial",
    children: <div>Contenido historial</div>,
  },
  {
    key: "bloqueado",
    label: "Bloqueado",
    children: <div>Contenido bloqueado</div>,
    disabled: true,
  },
];

describe("AppTabs [SPEC:APP-TABS-001]", () => {
  it("renderiza tabs y panel inicial", () => {
    render(<AppTabs items={baseItems} defaultActiveKey="general" />);

    expect(screen.getAllByRole("tablist").length).toBeGreaterThan(0);
    expect(screen.getByRole("tab", { name: /General/ })).toBeInTheDocument();
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido general");
  });

  it("propaga onChange al seleccionar otra tab", async () => {
    const handleChange = vi.fn();

    render(
      <AppTabs
        items={baseItems}
        defaultActiveKey="general"
        onChange={handleChange}
      />,
    );

    await act(async () => {
      fireEvent.click(screen.getByRole("tab", { name: /Historial/ }));
    });

    await waitFor(() => {
      expect(handleChange).toHaveBeenCalledWith("historial");
    });
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido historial");
  });

  it("respeta la tab activa controlada externamente", () => {
    render(<AppTabs items={baseItems} activeKey="historial" />);

    expect(screen.getByRole("tab", { name: /Historial/ })).toHaveAttribute(
      "aria-selected",
      "true",
    );
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido historial");
  });

  it("impide activar tabs deshabilitadas", () => {
    const handleChange = vi.fn();

    render(
      <AppTabs
        items={baseItems}
        defaultActiveKey="general"
        onChange={handleChange}
      />,
    );

    fireEvent.click(screen.getByRole("tab", { name: /Bloqueado/ }));

    expect(handleChange).not.toHaveBeenCalled();
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido general");
  });

  it("bloquea cambio cuando beforeChange retorna false", async () => {
    const handleChange = vi.fn();

    render(
      <AppTabs
        items={baseItems}
        defaultActiveKey="general"
        onChange={handleChange}
        beforeChange={() => false}
      />,
    );

    fireEvent.click(screen.getByRole("tab", { name: /Historial/ }));

    expect(handleChange).not.toHaveBeenCalled();
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido general");
  });
});
