import { useEffect, useState } from "react";

const DEFAULT_LOADING_VEIL_DELAY_MS = 140;

export const useDeferredLoadingVeil = (
  enabled: boolean,
  delayMs = DEFAULT_LOADING_VEIL_DELAY_MS,
) => {
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    if (!enabled) {
      setVisible(false);
      return undefined;
    }

    const timeoutId = window.setTimeout(() => {
      setVisible(true);
    }, delayMs);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [delayMs, enabled]);

  return visible;
};
