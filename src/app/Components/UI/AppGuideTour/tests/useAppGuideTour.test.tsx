import { act, renderHook } from "@testing-library/react";
import { afterEach, describe, expect, it, vi } from "vitest";

import { useAppGuideTour } from "../hooks/useAppGuideTour";
import type { AppGuideTourDriverCallbacks, AppGuideTourDriverFactory, AppGuideTourStep } from "../AppGuideTour.types";

const steps: AppGuideTourStep[] = [
  {
    id: "target",
    element: '[data-guide-tour-id="target"]',
    title: "Target",
    description: "Target description",
  },
];

describe("useAppGuideTour", () => {
  afterEach(() => {
    document.body.innerHTML = "";
  });

  it("registra steps, ejecuta start/stop/refresh y cleanup", () => {
    document.body.innerHTML = '<button data-guide-tour-id="target">Target</button>';

    let callbacks: AppGuideTourDriverCallbacks | null = null;
    const start = vi.fn();
    const stop = vi.fn();
    const refresh = vi.fn();
    const destroy = vi.fn();
    const onEvent = vi.fn();
    const driverFactory: AppGuideTourDriverFactory = (driverCallbacks) => {
      callbacks = driverCallbacks;
      return { start, stop, refresh, destroy };
    };

    const { result, unmount } = renderHook(() =>
      useAppGuideTour({
        tourId: "test-tour",
        steps,
        onEvent,
        driverFactory,
      }),
    );

    act(() => result.current.start());

    expect(start).toHaveBeenCalledWith(steps);
    expect(result.current.isRunning).toBe(true);
    expect(onEvent).toHaveBeenCalledWith(
      expect.objectContaining({
        name: "guide_started",
        tourId: "test-tour",
        totalSteps: 1,
      }),
    );

    act(() => callbacks?.onStepChange(0));
    expect(result.current.currentStepId).toBe("target");

    act(() => result.current.refresh());
    expect(refresh).toHaveBeenCalledTimes(1);

    act(() => result.current.stop());
    expect(stop).toHaveBeenCalledTimes(1);

    unmount();
    expect(destroy).toHaveBeenCalledTimes(1);
  });
});
