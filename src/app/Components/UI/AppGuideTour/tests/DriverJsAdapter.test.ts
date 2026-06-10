import { beforeEach, describe, expect, it, vi } from "vitest";

import { DriverJsAdapter } from "../drivers/DriverJsAdapter";
import type { AppGuideTourDriverCallbacks, AppGuideTourStep } from "../AppGuideTour.types";

const drive = vi.fn();
const setConfig = vi.fn();
const refresh = vi.fn();
const destroy = vi.fn();
const getActiveIndex = vi.fn(() => 0);
const driverMock = vi.fn(() => ({
  drive,
  setConfig,
  refresh,
  destroy,
  getActiveIndex,
}));

vi.mock("driver.js", () => ({
  driver: (config: unknown) => driverMock(config),
}));

const steps: AppGuideTourStep[] = [
  {
    id: "target",
    element: '[data-guide-tour-id="target"]',
    title: "Target",
    description: "Target description",
    side: "right",
  },
];

describe("DriverJsAdapter", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("mapea steps a Driver.js y destruye instancia", () => {
    const callbacks: AppGuideTourDriverCallbacks = {
      onStepChange: vi.fn(),
      onCompleted: vi.fn(),
      onCancelled: vi.fn(),
      onError: vi.fn(),
    };
    const adapter = new DriverJsAdapter(callbacks);

    adapter.start(steps);
    adapter.refresh();
    adapter.destroy();

    expect(driverMock).toHaveBeenCalledWith(
      expect.objectContaining({
        steps: [
          expect.objectContaining({
            element: steps[0].element,
            popover: expect.objectContaining({
              title: steps[0].title,
              description: steps[0].description,
              side: "right",
            }),
          }),
        ],
      }),
    );
    expect(drive).toHaveBeenCalledTimes(1);
    expect(refresh).toHaveBeenCalledTimes(1);
    expect(destroy).toHaveBeenCalledTimes(1);
  });
});
