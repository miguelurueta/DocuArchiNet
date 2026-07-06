import type { ReactNode } from "react";

interface RadicacionStartupGuardProps {
  children: ReactNode;
}

export function RadicacionStartupGuard({
  children,
}: RadicacionStartupGuardProps) {
  return <>{children}</>;
}
