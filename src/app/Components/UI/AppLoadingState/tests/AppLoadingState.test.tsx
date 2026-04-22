import { act, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppLoadingState } from "../AppLoadingState";

describe("[SPEC:app-loading-state] AppLoadingState", () => {
  it("no renderiza antes de delayMs", () => {
    vi.useFakeTimers();

    const { container } = render(
      <AppLoadingState loading delayMs={500} title="Cargando" message="Validando" />,
    );

    expect(screen.queryByText("Cargando")).not.toBeInTheDocument();
    expect(container).toBeTruthy();

    vi.advanceTimersByTime(499);
    expect(screen.queryByText("Cargando")).not.toBeInTheDocument();

    vi.useRealTimers();
  });

  it("renderiza despues de delayMs si loading sigue true", async () => {
    vi.useFakeTimers();

    render(<AppLoadingState loading delayMs={200} title="Cargando" message="Validando" />);

    await act(async () => {
      vi.advanceTimersByTime(200);
    });
    expect(screen.getByText("Cargando")).toBeInTheDocument();

    vi.useRealTimers();
  });

  it("se oculta al pasar loading=false", async () => {
    vi.useFakeTimers();

    const { rerender } = render(
      <AppLoadingState loading delayMs={0} title="Cargando" message="Validando" />,
    );

    expect(screen.getByText("Cargando")).toBeInTheDocument();

    rerender(<AppLoadingState loading={false} delayMs={0} title="Cargando" message="Validando" />);
    expect(screen.queryByText("Cargando")).not.toBeInTheDocument();

    vi.useRealTimers();
  });

  it("limpia timers al desmontar", () => {
    vi.useFakeTimers();

    const consoleErrorSpy = vi.spyOn(console, "error").mockImplementation(() => undefined);
    const { unmount } = render(
      <AppLoadingState loading delayMs={500} title="Cargando" message="Validando" />,
    );

    unmount();
    vi.runOnlyPendingTimers();

    expect(consoleErrorSpy).not.toHaveBeenCalled();
    consoleErrorSpy.mockRestore();

    vi.useRealTimers();
  });

  it("wrapper mode muestra children cuando loading=false", () => {
    render(
      <AppLoadingState loading={false} title="Cargando">
        <div>Contenido</div>
      </AppLoadingState>,
    );

    expect(screen.getByText("Contenido")).toBeInTheDocument();
  });
});
