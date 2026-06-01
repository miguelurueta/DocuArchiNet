import type { Ref } from "react";

export type AppGuideTourSide = "top" | "bottom" | "left" | "right";

export type AppGuideTourState =
  | "idle"
  | "loading"
  | "running"
  | "paused"
  | "completed"
  | "cancelled"
  | "error";

export type AppGuideTourStep = {
  id: string;
  element: string;
  title: string;
  description: string;
  side?: AppGuideTourSide;
};

export type AppGuideTourEventName =
  | "guide_started"
  | "guide_completed"
  | "guide_cancelled"
  | "guide_step_changed"
  | "guide_error";

export type AppGuideTourEvent = {
  name: AppGuideTourEventName;
  tourId: string;
  stepId?: string;
  stepIndex?: number;
  totalSteps?: number;
  reason?: string;
};

export type AppGuideTourProps = {
  tourId: string;
  steps: AppGuideTourStep[];
  autoStart?: boolean;
  onEvent?: (event: AppGuideTourEvent) => void;
  driverFactory?: AppGuideTourDriverFactory;
};

export type AppGuideTourRef = {
  start: () => void;
  stop: () => void;
  refresh: () => void;
};

export type AppGuideTourComponentProps = AppGuideTourProps & {
  ref?: Ref<AppGuideTourRef>;
};

export type AppGuideTourDriverCallbacks = {
  onStepChange: (stepIndex: number) => void;
  onCompleted: () => void;
  onCancelled: () => void;
  onError: (reason: string) => void;
};

export type AppGuideTourDriver = {
  start: (steps: AppGuideTourStep[]) => void;
  stop: () => void;
  refresh: () => void;
  destroy: () => void;
};

export type AppGuideTourDriverFactory = (
  callbacks: AppGuideTourDriverCallbacks,
) => AppGuideTourDriver;

export type UseAppGuideTourParams = {
  tourId: string;
  steps: AppGuideTourStep[];
  autoStart?: boolean;
  onEvent?: (event: AppGuideTourEvent) => void;
  driverFactory?: AppGuideTourDriverFactory;
};

export type UseAppGuideTourResult = {
  state: AppGuideTourState;
  currentStepId: string | null;
  isRunning: boolean;
  start: () => void;
  stop: () => void;
  refresh: () => void;
};
