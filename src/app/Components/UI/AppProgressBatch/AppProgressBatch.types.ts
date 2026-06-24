export type AppProgressBatchLifecycle =
  | "idle"
  | "running"
  | "paused"
  | "cancelling"
  | "completed"
  | "error";

export type AppProgressBatchItemResult =
  | { status: "success" }
  | { status: "warning"; message: string }
  | { status: "skipped"; message?: string }
  | { status: "controlled-error"; message: string; canContinue?: boolean }
  | { status: "fatal-error"; message: string };

export type AppProgressBatchSummary = {
  total: number;
  processed: number;
  success: number;
  warnings: number;
  skipped: number;
  controlledErrors: number;
  fatalErrors: number;
  cancelled: boolean;
};

export type AppProgressBatchItemContext = {
  index: number;
  total: number;
  signal: AbortSignal;
  setCurrentLabel: (label: string) => void;
  setItemProgress: (percent: number) => void;
  setPhase: (phase: string) => void;
};

export type AppProgressBatchProps<TItem> = {
  open: boolean;
  items: TItem[];
  onOpenChange: (open: boolean) => void;
  processItem: (
    item: TItem,
    context: AppProgressBatchItemContext,
  ) => Promise<AppProgressBatchItemResult>;
  title?: string;
  processName?: string;
  autoStart?: boolean;
  confirmOnCancel?: boolean;
  emptyMessage?: string;
  closeOnComplete?: boolean;
  getItemLabel?: (item: TItem, index: number) => string;
  onComplete?: (summary: AppProgressBatchSummary) => void;
  onCancel?: (summary: AppProgressBatchSummary) => void;
  onError?: (error: unknown) => void;
};
