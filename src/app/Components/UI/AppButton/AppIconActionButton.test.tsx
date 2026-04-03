import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppIconActionButton } from "./AppIconActionButton";

describe("AppIconActionButton", () => {
  it("renderiza un boton icon-only reutilizando AppButton", () => {
    render(
      <AppIconActionButton
        icon={<span data-testid="action-icon">I</span>}
        aria-label="Actualizar"
      />,
    );

    expect(screen.getByRole("button", { name: "Actualizar" })).toBeInTheDocument();
    expect(screen.getByTestId("action-icon")).toBeInTheDocument();
  });

  it("respeta disabled y loading sin ejecutar onClick", () => {
    const handleClick = vi.fn();
    const { rerender } = render(
      <AppIconActionButton
        icon={<span aria-hidden="true">I</span>}
        aria-label="Refrescar"
        disabled
        onClick={handleClick}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Refrescar" }));
    expect(handleClick).not.toHaveBeenCalled();

    rerender(
      <AppIconActionButton
        icon={<span aria-hidden="true">I</span>}
        aria-label="Refrescar"
        loading
        onClick={handleClick}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /refrescar/i }));
    expect(handleClick).not.toHaveBeenCalled();
  });

  it("muestra tooltip cuando se provee", async () => {
    render(
      <AppIconActionButton
        icon={<span aria-hidden="true">I</span>}
        aria-label="Sincronizar"
        tooltip="Sincronizar datos"
      />,
    );

    fireEvent.mouseOver(screen.getByRole("button", { name: "Sincronizar" }));

    expect(await screen.findByText("Sincronizar datos")).toBeInTheDocument();
  });
});
