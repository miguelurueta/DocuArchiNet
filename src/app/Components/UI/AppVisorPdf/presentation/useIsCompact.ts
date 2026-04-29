import { useEffect, useState } from "react";

export function useIsCompact(breakpointPx = 768) {
  const [isCompact, setIsCompact] = useState(() => {
    if (typeof window === "undefined") return false;
    return window.innerWidth <= breakpointPx;
  });

  useEffect(() => {
    const handler = () => setIsCompact(window.innerWidth <= breakpointPx);
    window.addEventListener("resize", handler);
    return () => window.removeEventListener("resize", handler);
  }, [breakpointPx]);

  return isCompact;
}

