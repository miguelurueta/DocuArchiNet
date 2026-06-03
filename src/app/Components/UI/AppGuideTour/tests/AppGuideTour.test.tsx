import { render } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";

import { AppGuideTour } from "../AppGuideTour";
import type { AppGuideTourDriverFactory, AppGuideTourStep } from "../AppGuideTour.types";

const steps: AppGuideTourStep[] = [
  {
    id: "help",
    element: '[data-guide-tour-id="help"]',
    title: "Help",
    description: "Help target",
  },
];

describe("AppGuideTour", () => {
  it("renderiza sin auto-start por defecto", () => {
    const start = vi.fn();
    const driverFactory: AppGuideTourDriverFactory = () => ({
      start,
      stop: vi.fn(),
      refresh: vi.fn(),
      destroy: vi.fn(),
    });

    render(<AppGuideTour tourId="test-tour" steps={steps} driverFactory={driverFactory} />);

    expect(start).not.toHaveBeenCalled();
  });
});
