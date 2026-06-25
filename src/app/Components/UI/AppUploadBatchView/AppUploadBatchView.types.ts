import type { ReactNode } from "react";

export type AppUploadBatchFileState =
  | "queued"
  | "validating"
  | "ready"
  | "uploading"
  | "completing"
  | "storing"
  | "done"
  | "warning"
  | "error"
  | "cancelled"
  | "removed";

export type AppUploadBatchFileItem<TMetadata = unknown> = {
  uid: string;
  file: File;
  name: string;
  size: number;
  extension: string;
  state: AppUploadBatchFileState;
  progress?: number;
  phaseLabel?: string;
  error?: string;
  warning?: string;
  metadata?: TMetadata;
  previewUrl?: string;
  selected?: boolean;
  disabled?: boolean;
};

export type AppUploadBatchSummary = {
  total: number;
  queued: number;
  ready: number;
  uploading: number;
  done: number;
  warning: number;
  error: number;
  cancelled: number;
};

export type AppUploadBatchViewProps<TMetadata = unknown> = {
  title?: string;
  description?: string;
  files: ReadonlyArray<AppUploadBatchFileItem<TMetadata>>;
  selectedUid?: string;
  accept?: string;
  maxSize?: number;
  multiple?: boolean;
  drag?: boolean;
  disabled?: boolean;
  loading?: boolean;
  canSaveAll?: boolean;
  canClearAll?: boolean;
  canAddFiles?: boolean;
  canPreview?: boolean;
  canSaveOne?: boolean;
  emptyMessage?: string;
  summary?: AppUploadBatchSummary;
  onFilesSelected?: (files: File[]) => void;
  onSelectFile?: (uid: string) => void;
  onPreviewFile?: (uid: string) => void;
  onRemoveFile?: (uid: string) => void;
  onSaveFile?: (uid: string) => void;
  onSaveAll?: () => void;
  onClearAll?: () => void;
  onClosePreview?: () => void;
  renderMetadata?: (args: {
    item: AppUploadBatchFileItem<TMetadata>;
    disabled: boolean;
  }) => ReactNode;
  renderPreview?: (args: {
    item: AppUploadBatchFileItem<TMetadata>;
    previewUrl?: string;
    onClose: () => void;
  }) => ReactNode;
  renderFileName?: (item: AppUploadBatchFileItem<TMetadata>) => ReactNode;
  renderFooterExtra?: (summary: AppUploadBatchSummary) => ReactNode;
};
