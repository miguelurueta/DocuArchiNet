// shared/errors/AppError.ts
export type AppErrorSeverity = "warning" | "error";
export type AppErrorSource = "api" | "ui" | "runtime" | "domain" | "auth" | "unknown";

export interface AppError {
  source: AppErrorSource;
  severity: AppErrorSeverity;
  message: string;
  details?: unknown;
  code?: string;
}
