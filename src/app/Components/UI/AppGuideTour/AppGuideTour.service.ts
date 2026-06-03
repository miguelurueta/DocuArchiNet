import type { AppGuideTourEvent, AppGuideTourStep } from "./AppGuideTour.types";

export function resolveGuideTourElement(selector: string): Element | null {
  if (typeof document === "undefined") return null;
  return document.querySelector(selector);
}

export function filterVisibleGuideTourSteps(steps: AppGuideTourStep[]): AppGuideTourStep[] {
  return steps.filter((step) => Boolean(resolveGuideTourElement(step.element)));
}

export function createGuideTourEvent(params: AppGuideTourEvent): AppGuideTourEvent {
  const { name, tourId, stepId, stepIndex, totalSteps, reason } = params;

  return {
    name,
    tourId,
    ...(stepId ? { stepId } : {}),
    ...(typeof stepIndex === "number" ? { stepIndex } : {}),
    ...(typeof totalSteps === "number" ? { totalSteps } : {}),
    ...(reason ? { reason } : {}),
  };
}
