import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
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

describe("AppTabs [SPEC:APP-TABS-002]", () => {
  it("renderiza iconos y badges", () => {
    render(
      <AppTabs
        items={[
          {
            key: "alertas",
            label: "Alertas",
            icon: <span data-testid="tab-icon">!</span>,
            badge: 3,
            children: <div>Contenido alertas</div>,
          },
        ]}
        defaultActiveKey="alertas"
      />,
    );

    expect(screen.getByTestId("tab-icon")).toBeInTheDocument();
    expect(screen.getByText("3")).toBeInTheDocument();
  });

  it("aplica clase customTabs", () => {
    const { container } = render(<AppTabs items={baseItems} />);

    expect(container.querySelector(".customTabs")).toBeInTheDocument();
  });

  it("marca estado visual disabled", () => {
    render(<AppTabs items={baseItems} defaultActiveKey="general" />);

    const disabledTabBtn = screen.getByRole("tab", { name: /Bloqueado/ });
    const disabledTab = disabledTabBtn.closest(".ant-tabs-tab");

    expect(disabledTab).toHaveClass("ant-tabs-tab-disabled");
    expect(disabledTabBtn).toHaveAttribute("aria-disabled", "true");
  });
});

describe("AppTabs [SPEC:APP-TABS-003]", () => {
  it("sincroniza tab con query param", () => {
    render(
      <MemoryRouter initialEntries={["/tabs?tab=historial"]}>
        <AppTabs items={baseItems} syncWithRouter />
      </MemoryRouter>,
    );

    expect(screen.getByRole("tab", { name: /Historial/ })).toHaveAttribute(
      "aria-selected",
      "true",
    );
  });

  it("sincroniza tab con path segment", () => {
    render(
      <MemoryRouter initialEntries={["/tabs/historial"]}>
        <AppTabs items={baseItems} syncWithRouter />
      </MemoryRouter>,
    );

    expect(screen.getByRole("tab", { name: /Historial/ })).toHaveAttribute(
      "aria-selected",
      "true",
    );
  });

  it("hace fallback cuando el key del router no existe", () => {
    render(
      <MemoryRouter initialEntries={["/tabs/inexistente"]}>
        <AppTabs items={baseItems} syncWithRouter />
      </MemoryRouter>,
    );

    expect(screen.getByRole("tab", { name: /General/ })).toHaveAttribute(
      "aria-selected",
      "true",
    );
  });

  it("router gana sobre activeKey", () => {
    render(
      <MemoryRouter initialEntries={["/tabs?tab=historial"]}>
        <AppTabs items={baseItems} activeKey="general" syncWithRouter />
      </MemoryRouter>,
    );

    expect(screen.getByRole("tab", { name: /Historial/ })).toHaveAttribute(
      "aria-selected",
      "true",
    );
  });

  it("lazy rendering renderiza contenido solo al activar tab", async () => {
    render(<AppTabs items={baseItems} defaultActiveKey="general" lazy />);

    expect(screen.queryByText("Contenido historial")).not.toBeInTheDocument();

    await act(async () => {
      fireEvent.click(screen.getByRole("tab", { name: /Historial/ }));
    });

    expect(screen.getByText("Contenido historial")).toBeInTheDocument();
  });

  it("dispara onTabVisible cuando una tab se vuelve visible", async () => {
    const handleVisible = vi.fn();

    render(
      <AppTabs
        items={baseItems}
        defaultActiveKey="general"
        onTabVisible={handleVisible}
      />,
    );

    expect(handleVisible).toHaveBeenCalledWith("general");

    await act(async () => {
      fireEvent.click(screen.getByRole("tab", { name: /Historial/ }));
    });

    expect(handleVisible).toHaveBeenCalledWith("historial");
    expect(handleVisible).toHaveBeenCalledTimes(2);
  });
});
