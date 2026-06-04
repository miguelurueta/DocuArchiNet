import { useImperativeHandle } from "react";

import { useAppGuideTour } from "./hooks/useAppGuideTour";
import type { AppGuideTourComponentProps } from "./AppGuideTour.types";

export function AppGuideTour({
  ref,
  tourId,
  steps,
  autoStart = false,
  onEvent,
  driverFactory,
}: AppGuideTourComponentProps) {
  const guideTour = useAppGuideTour({
    tourId,
    steps,
    autoStart,
    onEvent,
    driverFactory,
  });

  useImperativeHandle(
    ref,
    () => ({
      start: guideTour.start,
      stop: guideTour.stop,
      refresh: guideTour.refresh,
    }),
    [guideTour.refresh, guideTour.start, guideTour.stop],
  );

  return null;
}
