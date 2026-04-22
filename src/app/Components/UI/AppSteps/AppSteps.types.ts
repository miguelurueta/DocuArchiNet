import type { ReactNode } from "react";

export type AppStepsVariant = "default" | "form" | "progress" | "timeline";

export type AppStepStatus = "wait" | "process" | "finish" | "error";

export type AppStepItem<TData = unknown> = {
  key: string;
  title: ReactNode;
  description?: ReactNode;
  icon?: ReactNode;
  status?: AppStepStatus;
  disabled?: boolean;
  data?: TData;
  formFields?: string[];
  timestamp?: string;
};

export type AppStepsProps<TData = unknown> = {
  items: AppStepItem<TData>[];
  current?: number;
  defaultCurrent?: number;
  variant?: AppStepsVariant;
  direction?: "horizontal" | "vertical";
  size?: "sm" | "md" | "lg";
  responsive?: boolean;
  validateStep?: (stepIndex: number) => boolean | Promise<boolean>;
  progressPercent?: number;
  onChange?: (stepIndex: number) => void;
  className?: string;
};
