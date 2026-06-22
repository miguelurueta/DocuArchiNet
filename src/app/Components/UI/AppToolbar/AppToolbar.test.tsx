import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import { afterEach, describe, expect, it, vi } from "vitest";
import { AppToolbar } from "./AppToolbar";

const originalMatchMedia = window.matchMedia;

function mockMatchMedia(matches: boolean) {
  window.matchMedia = vi.fn().mockImplementation((query: string) => ({
    matches,
    media: query,
    onchange: null,
    addListener: () => {},
    removeListener: () => {},
    addEventListener: () => {},
    removeEventListener: () => {},
    dispatchEvent: () => false,
  }));
}

afterEach(() => {
  window.matchMedia = originalMatchMedia;
});

describe("AppToolbar [SPEC:APP-TOOLBAR-001]", () => {
  it("renderiza encabezado, breadcrumbs y accion primaria", () => {
    render(
      <MemoryRouter>
        <AppToolbar
          title="Gestion de correspondencia"
          subtitle="Centro operativo"
          description="Espacio para bandejas y acciones contextuales."
          breadcrumbs={[
            { key: "home", label: "Dashboard", to: "/dashboard" },
            { key: "current", label: "Gestion", current: true },
          ]}
          primaryAction={{ key: "create", label: "Nueva respuesta", variant: "primary" }}
        />
      </MemoryRouter>,
    );

    expect(screen.getByRole("heading", { name: /Gestion de correspondencia/i })).toBeInTheDocument();
    expect(screen.getByText("Centro operativo")).toBeInTheDocument();
    expect(screen.getByRole("navigation", { name: /breadcrumb/i })).toBeInTheDocument();
    expect(screen.getByRole("link", { name: "Dashboard" })).toHaveAttribute("href", "/dashboard");
    expect(screen.getByRole("button", { name: "Nueva respuesta" })).toBeInTheDocument();
  });

  it("renderiza regiones opcionales y acciones secundarias visibles en desktop", () => {
    mockMatchMedia(false);

    const { container } = render(
      <MemoryRouter>
        <AppToolbar
          title="Titulo"
          extra={<span>Indicador</span>}
          actions={[{ key: "refresh", label: "Actualizar" }]}
          secondaryActions={[
            { key: "export", label: "Exportar" },
            { key: "share", label: "Compartir" },
          ]}
        />
      </MemoryRouter>,
    );

    expect(screen.getByText("Indicador")).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Actualizar" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Exportar" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Compartir" })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: /more actions/i })).not.toBeInTheDocument();
    expect(container.querySelector("section")?.className).not.toContain("compact");
  });

  it("colapsa acciones secundarias en overflow cuando el viewport es compacto", async () => {
    mockMatchMedia(true);
    const onShare = vi.fn();

    render(
      <MemoryRouter>
        <AppToolbar
          title="Toolbar compacta"
          primaryAction={{ key: "new", label: "Nueva" }}
          secondaryActions={[
            { key: "export", label: "Exportar" },
            { key: "share", label: "Compartir", onClick: onShare },
          ]}
        />
      </MemoryRouter>,
    );

    expect(screen.getByRole("button", { name: "Nueva" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: /more actions/i })).toBeInTheDocument();
    expect(screen.queryByRole("button", { name: "Exportar" })).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /more actions/i }));

    expect(await screen.findByText("Exportar")).toBeInTheDocument();
    expect(screen.getByText("Compartir")).toBeInTheDocument();

    fireEvent.click(screen.getByText("Compartir"));

    expect(onShare).toHaveBeenCalledTimes(1);
  });

  it("mantiene nombre accesible para acciones icon-only", () => {
    render(
      <MemoryRouter>
        <AppToolbar
          title="Icon actions"
          actions={[
            {
              key: "filters",
              label: "",
              icon: <span data-testid="filter-icon">F</span>,
              ariaLabel: "Abrir filtros",
            },
          ]}
        />
      </MemoryRouter>,
    );

    expect(screen.getByRole("button", { name: "Abrir filtros" })).toBeInTheDocument();
    expect(screen.getByTestId("filter-icon")).toBeInTheDocument();
  });
});
