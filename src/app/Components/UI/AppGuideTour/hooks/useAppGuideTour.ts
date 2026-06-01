import { useCallback, useEffect, useMemo, useRef, useState } from "react";

import { defaultAppGuideTourDriverFactory } from "../AppGuideTour.adapter";
import { APP_GUIDE_TOUR_EMPTY_TARGETS_REASON, APP_GUIDE_TOUR_EVENTS } from "../AppGuideTour.constants";
import { createGuideTourEvent, filterVisibleGuideTourSteps } from "../AppGuideTour.service";
import type {
  AppGuideTourDriver,
  AppGuideTourDriverCallbacks,
  AppGuideTourState,
  AppGuideTourStep,
  UseAppGuideTourParams,
  UseAppGuideTourResult,
} from "../AppGuideTour.types";

export function useAppGuideTour({
  tourId,
  steps,
  autoStart = false,
  onEvent,
  driverFactory = defaultAppGuideTourDriverFactory,
}: UseAppGuideTourParams): UseAppGuideTourResult {
  const [state, setState] = useState<AppGuideTourState>("idle");
  const [currentStepId, setCurrentStepId] = useState<string | null>(null);
  const validStepsRef = useRef<AppGuideTourStep[]>([]);
  const driverRef = useRef<AppGuideTourDriver | null>(null);
  const onEventRef = useRef(onEvent);

  useEffect(() => {
    onEventRef.current = onEvent;
  }, [onEvent]);

  const emit = useCallback(
    (event: Parameters<NonNullable<typeof onEventRef.current>>[0]) => {
      onEventRef.current?.(createGuideTourEvent(event));
    },
    [],
  );

  const callbacks = useMemo<AppGuideTourDriverCallbacks>(
    () => ({
      onStepChange: (stepIndex) => {
        const validSteps = validStepsRef.current;
        const step = validSteps[stepIndex];
        setCurrentStepId(step?.id ?? null);
        emit({
          name: APP_GUIDE_TOUR_EVENTS.STEP_CHANGED,
          tourId,
          stepId: step?.id,
          stepIndex,
          totalSteps: validSteps.length,
        });
      },
      onCompleted: () => {
        const validSteps = validStepsRef.current;
        setState("completed");
        emit({
          name: APP_GUIDE_TOUR_EVENTS.COMPLETED,
          tourId,
          totalSteps: validSteps.length,
        });
        setCurrentStepId(null);
        setState("idle");
      },
      onCancelled: () => {
        const validSteps = validStepsRef.current;
        setState("cancelled");
        emit({
          name: APP_GUIDE_TOUR_EVENTS.CANCELLED,
          tourId,
          totalSteps: validSteps.length,
        });
        setCurrentStepId(null);
        setState("idle");
      },
      onError: (reason) => {
        setState("error");
        emit({
          name: APP_GUIDE_TOUR_EVENTS.ERROR,
          tourId,
          reason,
        });
      },
    }),
    [emit, tourId],
  );

  const getDriver = useCallback(() => {
    if (!driverRef.current) {
      driverRef.current = driverFactory(callbacks);
    }

    return driverRef.current;
  }, [callbacks, driverFactory]);

  const start = useCallback(() => {
    setState("loading");
    const validSteps = filterVisibleGuideTourSteps(steps);
    validStepsRef.current = validSteps;

    if (validSteps.length === 0) {
      setState("error");
      emit({
        name: APP_GUIDE_TOUR_EVENTS.ERROR,
        tourId,
        reason: APP_GUIDE_TOUR_EMPTY_TARGETS_REASON,
      });
      return;
    }

    setCurrentStepId(validSteps[0]?.id ?? null);
    emit({
      name: APP_GUIDE_TOUR_EVENTS.STARTED,
      tourId,
      stepId: validSteps[0]?.id,
      stepIndex: 0,
      totalSteps: validSteps.length,
    });
    setState("running");
    getDriver().start(validSteps);
  }, [emit, getDriver, steps, tourId]);

  const stop = useCallback(() => {
    getDriver().stop();
  }, [getDriver]);

  const refresh = useCallback(() => {
    getDriver().refresh();
  }, [getDriver]);

  useEffect(() => {
    if (!autoStart) return;
    start();
  }, [autoStart, start]);

  useEffect(() => {
    return () => {
      driverRef.current?.destroy();
      driverRef.current = null;
    };
  }, []);

  return {
    state,
    currentStepId,
    isRunning: state === "running",
    start,
    stop,
    refresh,
  };
}
