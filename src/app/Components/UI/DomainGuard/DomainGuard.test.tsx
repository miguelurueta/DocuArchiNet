import { render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useEffect } from "react";
import { DomainGuard } from "./DomainGuard";

describe("DomainGuard", () => {
  it("renderiza children cuando no esta bloqueado", () => {
    render(
      <DomainGuard isBlocked={false} fallback={<div>Fallback</div>}>
        <div>Contenido</div>
      </DomainGuard>,
    );

    expect(screen.getByText("Contenido")).toBeInTheDocument();
    expect(screen.queryByText("Fallback")).not.toBeInTheDocument();
  });

  it("renderiza fallback cuando esta bloqueado", () => {
    render(
      <DomainGuard isBlocked fallback={<div role="alert">Bloqueado</div>}>
        <div>Contenido</div>
      </DomainGuard>,
    );

    expect(screen.getByRole("alert")).toHaveTextContent("Bloqueado");
    expect(screen.queryByText("Contenido")).not.toBeInTheDocument();
  });

  it("no monta children cuando esta bloqueado", () => {
    const onMount = vi.fn();

    function Child() {
      useEffect(() => {
        onMount();
      }, []);
      return <div>Child</div>;
    }

    render(
      <DomainGuard isBlocked fallback={<div>Fallback</div>}>
        <Child />
      </DomainGuard>,
    );

    expect(onMount).not.toHaveBeenCalled();
    expect(screen.queryByText("Child")).not.toBeInTheDocument();
  });
});

