import type { ReactNode } from "react";

type AppGuideTourProviderProps = {
  children: ReactNode;
};

export function AppGuideTourProvider({ children }: AppGuideTourProviderProps) {
  return <>{children}</>;
}
