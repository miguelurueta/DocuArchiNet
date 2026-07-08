import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { AppHorizontalScroller } from "./AppHorizontalScroller";
import styles from "./AppHorizontalScroller.module.css";

describe("AppHorizontalScroller [SPEC:app-horizontal-scroller]", () => {
  it("renderiza children en una region accesible con testId", () => {
    render(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        testId="horizontal-scroller"
      >
        <button type="button">Elemento 1</button>
        <button type="button">Elemento 2</button>
      </AppHorizontalScroller>,
    );

    const region = screen.getByRole("region", { name: "Listado horizontal" });
    expect(region).toHaveAttribute("data-testid", "horizontal-scroller");
    expect(screen.getByRole("button", { name: "Elemento 1" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Elemento 2" })).toBeInTheDocument();
  });

  it("aplica clases personalizadas de root, viewport y content", () => {
    const { container } = render(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        className="custom-root"
        viewportClassName="custom-viewport"
        contentClassName="custom-content"
      >
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    expect(container.firstElementChild).toHaveClass("custom-root");
    expect(screen.getByRole("region", { name: "Listado horizontal" })).toHaveClass(
      "custom-viewport",
    );
    expect(container.querySelector(".custom-content")).toHaveClass("custom-content");
  });

  it("usa comfortable y md por defecto", () => {
    const { container } = render(
      <AppHorizontalScroller ariaLabel="Listado horizontal">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    expect(screen.getByRole("region", { name: "Listado horizontal" })).toHaveClass(
      styles.densityComfortable,
    );
    expect(container.querySelector(`.${styles.content}`)).toHaveClass(styles.gapMD);
  });

  it("respeta densidades compact y comfortable", () => {
    const { rerender } = render(
      <AppHorizontalScroller ariaLabel="Listado horizontal" density="compact">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    expect(screen.getByRole("region", { name: "Listado horizontal" })).toHaveClass(
      styles.densityCompact,
    );

    rerender(
      <AppHorizontalScroller ariaLabel="Listado horizontal" density="comfortable">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    expect(screen.getByRole("region", { name: "Listado horizontal" })).toHaveClass(
      styles.densityComfortable,
    );
  });

  it("respeta gaps xs, sm, md y lg", () => {
    const { container, rerender } = render(
      <AppHorizontalScroller ariaLabel="Listado horizontal" gap="xs">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    const content = () => container.querySelector(`.${styles.content}`);
    expect(content()).toHaveClass(styles.gapXS);

    rerender(
      <AppHorizontalScroller ariaLabel="Listado horizontal" gap="sm">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content()).toHaveClass(styles.gapSM);

    rerender(
      <AppHorizontalScroller ariaLabel="Listado horizontal" gap="md">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content()).toHaveClass(styles.gapMD);

    rerender(
      <AppHorizontalScroller ariaLabel="Listado horizontal" gap="lg">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content()).toHaveClass(styles.gapLG);
  });

  it("convierte dimensiones numericas a px", () => {
    const { container } = render(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        itemMinWidth={220}
        itemMaxWidth={320}
      >
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    const content = container.querySelector(`.${styles.content}`);
    expect(content).toHaveStyle({
      "--app-horizontal-scroller-item-min-width": "220px",
      "--app-horizontal-scroller-item-max-width": "320px",
    });
  });

  it("acepta dimensiones string no vacias", () => {
    const { container } = render(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        itemMinWidth="14rem"
        itemMaxWidth="20rem"
      >
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    const content = container.querySelector(`.${styles.content}`);
    expect(content).toHaveStyle({
      "--app-horizontal-scroller-item-min-width": "14rem",
      "--app-horizontal-scroller-item-max-width": "20rem",
    });
  });

  it("ignora dimensiones invalidas", () => {
    const { container, rerender } = render(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        itemMinWidth=""
        itemMaxWidth=" "
      >
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    const content = () => container.querySelector(`.${styles.content}`) as HTMLElement;
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-min-width")).toBe("");
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-max-width")).toBe("");

    rerender(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        itemMinWidth={0}
        itemMaxWidth={-1}
      >
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-min-width")).toBe("");
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-max-width")).toBe("");

    rerender(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        itemMinWidth={Number.NaN}
        itemMaxWidth={Number.POSITIVE_INFINITY}
      >
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-min-width")).toBe("");
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-max-width")).toBe("");

    rerender(
      <AppHorizontalScroller
        ariaLabel="Listado horizontal"
        itemMinWidth="-1px"
        itemMaxWidth="-10rem"
      >
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-min-width")).toBe("");
    expect(content().style.getPropertyValue("--app-horizontal-scroller-item-max-width")).toBe("");
  });

  it("no muta ni clona los hijos para inyectar props", () => {
    render(
      <AppHorizontalScroller ariaLabel="Listado horizontal">
        <button type="button">Elemento</button>
      </AppHorizontalScroller>,
    );

    const child = screen.getByRole("button", { name: "Elemento" });
    expect(child).not.toHaveAttribute("style");
    expect(child).not.toHaveAttribute("data-testid");
  });

  it("aplica scroll snap opcional", () => {
    const { container, rerender } = render(
      <AppHorizontalScroller ariaLabel="Listado horizontal">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );

    const content = () => container.querySelector(`.${styles.content}`);
    expect(content()).not.toHaveClass(styles.snap);
    expect(content()).not.toHaveClass(styles.snapStart);
    expect(content()).not.toHaveClass(styles.snapCenter);

    rerender(
      <AppHorizontalScroller ariaLabel="Listado horizontal" scrollSnap="start">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content()).toHaveClass(styles.snap);
    expect(content()).toHaveClass(styles.snapStart);

    rerender(
      <AppHorizontalScroller ariaLabel="Listado horizontal" scrollSnap="center">
        <span>Contenido</span>
      </AppHorizontalScroller>,
    );
    expect(content()).toHaveClass(styles.snap);
    expect(content()).toHaveClass(styles.snapCenter);
  });

  it("activa edge fade sin bloquear interaccion", () => {
    const { container } = render(
      <AppHorizontalScroller ariaLabel="Listado horizontal" edgeFade>
        <button type="button">Elemento</button>
      </AppHorizontalScroller>,
    );

    expect(container.firstElementChild).toHaveClass(styles.edgeFade);
    expect(styles.edgeFade).toBeDefined();
    expect(screen.getByRole("button", { name: "Elemento" })).toBeInTheDocument();
  });

  it("renderiza sin fallar con children null", () => {
    render(
      <AppHorizontalScroller ariaLabel="Listado horizontal">
        {null}
      </AppHorizontalScroller>,
    );

    expect(screen.getByRole("region", { name: "Listado horizontal" })).toBeInTheDocument();
  });

  it("no agrega tabIndex al viewport", () => {
    render(
      <AppHorizontalScroller ariaLabel="Listado horizontal">
        <button type="button">Elemento</button>
      </AppHorizontalScroller>,
    );

    expect(screen.getByRole("region", { name: "Listado horizontal" })).not.toHaveAttribute(
      "tabindex",
    );
  });

  it("mantiene clases base necesarias para responsive y scroll contenido", () => {
    const { container } = render(
      <AppHorizontalScroller ariaLabel="Listado horizontal">
        <span>Elemento 1</span>
        <span>Elemento 2</span>
      </AppHorizontalScroller>,
    );

    expect(container.firstElementChild).toHaveClass(styles.root);
    expect(screen.getByRole("region", { name: "Listado horizontal" })).toHaveClass(
      styles.viewport,
    );
    expect(container.querySelector(`.${styles.content}`)).toHaveClass(styles.content);
  });
});
