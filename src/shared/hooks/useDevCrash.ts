import type { AppError, AppErrorSource, AppErrorSeverity } from "../errors/AppError";
import { useAppErrorNotifier } from "./useAppErrorNotifier";

type DevCrashOptions = {
  enabled: boolean;
  source: AppErrorSource;
  severity?: AppErrorSeverity;
  message?: string;
  code?: string;
  details?: unknown;
};

export function useDevCrash(options?: DevCrashOptions) {
  const notifyError = useAppErrorNotifier();

  if (!import.meta.env.DEV) return;
  if (!options?.enabled) return;

  const {
    source,
    severity = "error",
    message = "💥 Dev simulated error",
    code,
    details,
  } = options;

  // 🔴 UI / Runtime → throw (ErrorBoundary)
  if (source === "ui" || source === "runtime") {
    throw new Error(message);
  }

  // 🟡 Dominio / API / Auth / Unknown → notifier
  const appError: AppError = {
    source,
    severity,
    message,
    code,
    details,
  };

  notifyError(appError);
}
