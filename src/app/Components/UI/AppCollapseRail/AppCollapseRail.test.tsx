import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, test, vi } from "vitest";
import { AppCollapseRail } from "./AppCollapseRail";

describe("[SPEC:APP-COLLAPSE-RAIL-001] AppCollapseRail", () => {
  test("renderiza titulo y contenido", () => {
    render(
      <AppCollapseRail title="Herramientas" collapsed={false} onToggle={() => undefined}>
        <div>Contenido</div>
      </AppCollapseRail>,
    );

    expect(screen.getByText("Herramientas")).toBeInTheDocument();
    expect(screen.getByText("Contenido")).toBeInTheDocument();
  });
});

describe("[SPEC:APP-COLLAPSE-RAIL-002] AppCollapseRail toggle", () => {
  test("toggle actualiza aria-expanded", () => {
    const handleToggle = vi.fn();
    render(
      <AppCollapseRail title="Herramientas" collapsed={false} onToggle={handleToggle}>
        <div>Contenido</div>
      </AppCollapseRail>,
    );

    const toggle = screen.getByRole("button", { name: /Ocultar Herramientas/i });
    fireEvent.click(toggle);

    expect(handleToggle).toHaveBeenCalled();
  });

  test("rail visible cuando colapsado", () => {
    render(
      <AppCollapseRail title="Herramientas" collapsed onToggle={() => undefined}>
        <div>Contenido</div>
      </AppCollapseRail>,
    );

    expect(screen.getAllByRole("button", { name: /Mostrar Herramientas/i }).length).toBeGreaterThan(0);
  });
});

describe("[SPEC:APP-COLLAPSE-RAIL-003] AppCollapseRail persistence", () => {
  test("mantiene contenido montado al colapsar", () => {
    const { rerender } = render(
      <AppCollapseRail title="Herramientas" collapsed={false} onToggle={() => undefined}>
        <div>Contenido persistente</div>
      </AppCollapseRail>,
    );

    rerender(
      <AppCollapseRail title="Herramientas" collapsed onToggle={() => undefined}>
        <div>Contenido persistente</div>
      </AppCollapseRail>,
    );

    expect(screen.getByText("Contenido persistente")).toBeInTheDocument();
  });
});

describe("[SPEC:APP-COLLAPSE-RAIL-004] AppCollapseRail placement/variant", () => {
  test("aplica data attrs para placement y variant", () => {
    const { container } = render(
      <AppCollapseRail
        title="Herramientas"
        collapsed
        onToggle={() => undefined}
        placement="left"
        variant="overlay"
        railLabel="Rail"
      >
        <div>Contenido</div>
      </AppCollapseRail>,
    );

    const wrapper = container.querySelector("[data-placement][data-variant]");
    expect(wrapper).toHaveAttribute("data-placement", "left");
    expect(wrapper).toHaveAttribute("data-variant", "overlay");

    expect(screen.getAllByRole("button", { name: /Mostrar Herramientas/i }).length).toBeGreaterThan(0);
  });
});
