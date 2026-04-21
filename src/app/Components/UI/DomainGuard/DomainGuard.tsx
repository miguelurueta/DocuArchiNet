import type { ReactNode } from "react";

export type DomainGuardProps = {
  isBlocked: boolean;
  fallback: ReactNode;
  children: ReactNode;
};

export function DomainGuard({ isBlocked, fallback, children }: DomainGuardProps) {
  if (isBlocked) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}

