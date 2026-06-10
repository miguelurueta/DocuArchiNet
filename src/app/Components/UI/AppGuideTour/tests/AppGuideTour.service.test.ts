import { afterEach, describe, expect, it } from "vitest";

import { filterVisibleGuideTourSteps } from "../AppGuideTour.service";
import type { AppGuideTourStep } from "../AppGuideTour.types";

describe("AppGuideTour.service", () => {
  afterEach(() => {
    document.body.innerHTML = "";
  });

  it("filtra steps sin target DOM", () => {
    document.body.innerHTML = '<button data-guide-tour-id="present">Present</button>';

    const steps: AppGuideTourStep[] = [
      {
        id: "present",
        element: '[data-guide-tour-id="present"]',
        title: "Present",
        description: "Visible target",
      },
      {
        id: "missing",
        element: '[data-guide-tour-id="missing"]',
        title: "Missing",
        description: "Missing target",
      },
    ];

    expect(filterVisibleGuideTourSteps(steps)).toEqual([steps[0]]);
  });
});
