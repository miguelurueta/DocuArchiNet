export type DomainGuardReason = "empty" | "error" | "condition" | "unknown";

export type UseDomainGuardInput = {
  isEmpty?: boolean;
  error?: Error | null;
  condition?: boolean;
};

export type UseDomainGuardResult = {
  isBlocked: boolean;
  reason: DomainGuardReason;
};

export function useDomainGuard({
  isEmpty,
  error,
  condition,
}: UseDomainGuardInput): UseDomainGuardResult {
  if (error) {
    return { isBlocked: true, reason: "error" };
  }

  if (isEmpty) {
    return { isBlocked: true, reason: "empty" };
  }

  if (condition) {
    return { isBlocked: true, reason: "condition" };
  }

  return { isBlocked: false, reason: "unknown" };
}

