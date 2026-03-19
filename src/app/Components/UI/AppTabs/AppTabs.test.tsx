import { fireEvent, render, screen } from "@testing-library/react";
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

    expect(screen.getByRole("tablist")).toBeInTheDocument();
    expect(screen.getByRole("tab", { name: "General" })).toBeInTheDocument();
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido general");
  });

  it("propaga onChange al seleccionar otra tab", () => {
    const handleChange = vi.fn();

    render(
      <AppTabs
        items={baseItems}
        defaultActiveKey="general"
        onChange={handleChange}
      />,
    );

    fireEvent.click(screen.getByRole("tab", { name: "Historial" }));

    expect(handleChange).toHaveBeenCalledWith("historial");
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido historial");
  });

  it("respeta la tab activa controlada externamente", () => {
    render(<AppTabs items={baseItems} activeKey="historial" />);

    expect(screen.getByRole("tab", { name: "Historial" })).toHaveAttribute(
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

    fireEvent.click(screen.getByRole("tab", { name: "Bloqueado" }));

    expect(handleChange).not.toHaveBeenCalled();
    expect(screen.getByRole("tabpanel")).toHaveTextContent("Contenido general");
  });

  it("soporta orientacion vertical", () => {
    const { container } = render(
      <AppTabs items={baseItems} defaultActiveKey="general" orientation="vertical" />,
    );

    expect((container.firstElementChild as HTMLElement | null)?.className).toMatch(
      /orientationVertical/,
    );
  });

  it("mantiene la relacion accesible entre tab activa y panel", () => {
    render(<AppTabs items={baseItems} defaultActiveKey="general" />);

    const selectedTab = screen.getByRole("tab", { name: "General" });
    const panel = screen.getByRole("tabpanel");
    const controls = selectedTab.getAttribute("aria-controls");

    expect(selectedTab).toHaveAttribute("aria-selected", "true");
    expect(controls).toBeTruthy();
    expect(panel).toHaveAttribute("id", controls);
  });
});
