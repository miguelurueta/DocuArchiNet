import type { AppGuideTourEventName } from "./AppGuideTour.types";

export const APP_GUIDE_TOUR_EVENTS = {
  STARTED: "guide_started",
  COMPLETED: "guide_completed",
  CANCELLED: "guide_cancelled",
  STEP_CHANGED: "guide_step_changed",
  ERROR: "guide_error",
} as const satisfies Record<string, AppGuideTourEventName>;

export const APP_GUIDE_TOUR_EMPTY_TARGETS_REASON = "no_valid_targets";
