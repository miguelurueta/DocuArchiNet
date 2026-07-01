import { DeleteOutlined, EyeOutlined, ReloadOutlined, UploadOutlined } from "@ant-design/icons";
import { Button, Upload } from "antd";
import type { UploadProps } from "antd";
import type { RcFile } from "antd/es/upload";
import {
  memo,
  forwardRef,
  useCallback,
  useEffect,
  useImperativeHandle,
  useMemo,
  useRef,
  useState,
} from "react";
import type { ReactNode } from "react";
import styles from "./AppUpload.module.css";

type UploadStatus = "queued" | "uploading" | "done" | "error" | "removed";

type UploadTransition = Record<UploadStatus, UploadStatus[]>;

const ALLOWED_TRANSITIONS: UploadTransition = {
  queued: ["uploading", "removed"],
  uploading: ["done", "error", "removed"],
  done: ["removed"],
  error: ["uploading", "removed"],
  removed: [],
};

export type AppUploadFile = {
  uid: string;
  name: string;
  size: number;
  type?: string;
  status: UploadStatus;
  percent?: number;
  url?: string;
  thumbUrl?: string;
  error?: string;
  originFile?: File;
};

export type AppUploadProps = {
  value?: AppUploadFile[];
  defaultValue?: AppUploadFile[];
  className?: string;
  layout?: "grid" | "list";
  accept?: string;
  maxSize?: number;
  validateFile?: (file: File) => boolean | Promise<boolean>;
  maxCount?: number;
  disabled?: boolean;
  size?: "sm" | "md" | "lg";
  drag?: boolean;
  strategy?: "auto" | "manual" | "customRequest";
  beforeUpload?: (file: File, fileList: File[]) => boolean | Promise<boolean>;
  onChange: (files: AppUploadFile[]) => void;
  onRemove?: (file: AppUploadFile) => void;
  onUpload?: () => void;
  onProgress?: (file: AppUploadFile, percent: number) => void;
  onSuccess?: (file: AppUploadFile) => void;
  onError?: (file: AppUploadFile, error: unknown) => void;
  onTelemetry?: (event: AppUploadTelemetryEvent) => void;
  onPreview?: (file: AppUploadFile) => void;
  previewOnClick?: boolean;
  customRequest?: (
    file: AppUploadFile,
    helpers: {
      onProgress: (percent: number) => void;
      onSuccess: () => void;
      onError: (error: unknown) => void;
      onAbort: () => void;
    },
  ) => void | Promise<void>;
  renderItem?: (file: AppUploadFile) => ReactNode;
  renderActions?: (file: AppUploadFile) => ReactNode;
  renderUploadButton?: () => ReactNode;
};

export type AppUploadTelemetryEvent = {
  type:
    | "select"
    | "upload_start"
    | "upload_success"
    | "upload_error"
    | "remove"
    | "preview_open"
    | "cancel";
  file?: AppUploadFile;
  timestamp: string;
  meta?: Record<string, unknown>;
};

export type AppUploadHandle = {
  retry: (file: AppUploadFile) => void;
  abort: (file: AppUploadFile) => void;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const normalizeAccept = (accept?: string) =>
  accept
    ? accept
        .split(",")
        .map((value) => value.trim().toLowerCase())
        .filter(Boolean)
    : [];

const isAcceptedFile = (acceptList: string[], file: File) => {
  if (!acceptList.length) return true;
  const fileName = file.name.toLowerCase();
  const mimeType = file.type.toLowerCase();
  return acceptList.some((entry) => {
    if (entry === "*/*") return true;
    if (entry.endsWith("/*")) {
      const prefix = entry.replace("/*", "");
      return mimeType.startsWith(`${prefix}/`);
    }
    if (entry.startsWith(".")) {
      return fileName.endsWith(entry);
    }
    return mimeType === entry;
  });
};

const canTransition = (from: UploadStatus, to: UploadStatus) =>
  ALLOWED_TRANSITIONS[from]?.includes(to) ?? false;

const createAppUploadFile = (file: RcFile): AppUploadFile => ({
  uid: file.uid,
  name: file.name,
  size: file.size,
  type: file.type,
  status: "queued",
  percent: 0,
  originFile: file as File,
});

const IMAGE_EXTENSIONS = [".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".svg"];

const isImageFile = (file: AppUploadFile) =>
  Boolean(
    file.type?.startsWith("image/") ||
      IMAGE_EXTENSIONS.some((ext) => file.name.toLowerCase().endsWith(ext)),
  );

const FilePreview = ({
  file,
  onPreview,
  previewOnClick,
}: {
  file: AppUploadFile;
  onPreview?: (file: AppUploadFile) => void;
  previewOnClick?: boolean;
}) => {
  const [objectUrl, setObjectUrl] = useState<string | null>(null);
  const previewUrl = file.thumbUrl ?? file.url ?? objectUrl ?? undefined;
  const isImage = isImageFile(file) && Boolean(previewUrl);

  useEffect(() => {
    if (!file.originFile || previewUrl) return undefined;
    const url = URL.createObjectURL(file.originFile);
    setObjectUrl(url);
    return () => {
      URL.revokeObjectURL(url);
    };
  }, [file.originFile, previewUrl]);

  return (
    <button
      type="button"
      className={styles.preview}
      onClick={() => previewOnClick && onPreview?.(file)}
      aria-label={`Preview ${file.name}`}
    >
      {isImage ? (
        <img src={previewUrl} alt={file.name} />
      ) : (
        <span className={styles.previewFallback}>{file.name}</span>
      )}
    </button>
  );
};

const AppUploadItem = memo(function AppUploadItem({
  file,
  layout,
  onRemove,
  onRetry,
  onAbort,
  onPreview,
  previewOnClick,
  renderItem,
  renderActions,
  disabled,
}: {
  file: AppUploadFile;
  layout: "grid" | "list";
  onRemove: (file: AppUploadFile) => void;
  onRetry: (file: AppUploadFile) => void;
  onAbort: (file: AppUploadFile) => void;
  onPreview?: (file: AppUploadFile) => void;
  previewOnClick?: boolean;
  renderItem?: (file: AppUploadFile) => ReactNode;
  renderActions?: (file: AppUploadFile) => ReactNode;
  disabled?: boolean;
}) {
  const showRetry = file.status === "error";
  const showAbort = file.status === "uploading";

  return (
    <div
      className={joinClasses(styles.card, layout === "list" && styles.cardList)}
      data-status={file.status}
      role="listitem"
      tabIndex={0}
      onKeyDown={(event) => {
        if (event.key === "Enter") {
          onPreview?.(file);
        }
        if (event.key === "Delete" || event.key === "Backspace") {
          onRemove(file);
        }
      }}
    >
      {renderItem ? (
        renderItem(file)
      ) : (
        <FilePreview file={file} onPreview={onPreview} previewOnClick={previewOnClick} />
      )}
      <div className={styles.cardMeta}>
        <span className={styles.fileName} title={file.name}>
          {file.name}
        </span>
        {typeof file.percent === "number" ? (
          <span className={styles.filePercent}>{Math.round(file.percent)}%</span>
        ) : null}
      </div>
      <div className={styles.actions}>
        {renderActions ? (
          renderActions(file)
        ) : (
          <>
            {onPreview ? (
              <button
                type="button"
                className={styles.actionButton}
                onClick={() => onPreview(file)}
                aria-label={`Ver ${file.name}`}
                disabled={disabled}
              >
                <EyeOutlined />
              </button>
            ) : null}
            {showRetry ? (
              <button
                type="button"
                className={styles.actionButton}
                onClick={() => onRetry(file)}
                aria-label={`Reintentar ${file.name}`}
                disabled={disabled}
              >
                <ReloadOutlined />
              </button>
            ) : null}
            {showAbort ? (
              <button
                type="button"
                className={styles.actionButton}
                onClick={() => onAbort(file)}
                aria-label={`Cancelar ${file.name}`}
                disabled={disabled}
              >
                <DeleteOutlined />
              </button>
            ) : null}
            <button
              type="button"
              className={styles.removeButton}
              onClick={() => onRemove(file)}
              aria-label={`Eliminar ${file.name}`}
              disabled={disabled}
            >
              <DeleteOutlined />
            </button>
          </>
        )}
      </div>
    </div>
  );
});

export const AppUpload = forwardRef<AppUploadHandle, AppUploadProps>(
  function AppUpload(
    {
      value,
      defaultValue,
      className,
      layout = "grid",
      accept,
      maxSize,
      validateFile,
      maxCount,
      disabled = false,
      size = "md",
      drag = false,
      strategy = "manual",
      beforeUpload,
      onChange,
      onRemove,
      onUpload,
      onProgress,
      onSuccess,
      onError,
      onTelemetry,
      onPreview,
      previewOnClick = true,
      customRequest,
      renderItem,
      renderActions,
      renderUploadButton,
    },
    ref,
  ) {
    const acceptList = useMemo(() => normalizeAccept(accept), [accept]);
    const [internalFiles, setInternalFiles] = useState<AppUploadFile[]>(defaultValue ?? []);
    const files = value ?? internalFiles;
    const filesRef = useRef<AppUploadFile[]>(files);
    const isControlled = value !== undefined;
    const [dragState, setDragState] = useState<"valid" | "invalid" | null>(null);
    const createdThumbUrlsRef = useRef(new Map<string, string>());

    useEffect(() => {
      filesRef.current = files;
    }, [files]);

    useEffect(() => {
      const liveIds = new Set(files.map((file) => file.uid));
      createdThumbUrlsRef.current.forEach((url, uid) => {
        if (!liveIds.has(uid)) {
          URL.revokeObjectURL(url);
          createdThumbUrlsRef.current.delete(uid);
        }
      });
      return () => {
        createdThumbUrlsRef.current.forEach((url) => URL.revokeObjectURL(url));
        createdThumbUrlsRef.current.clear();
      };
    }, [files]);

    const emitTelemetry = useCallback(
      (type: AppUploadTelemetryEvent["type"], file?: AppUploadFile, meta?: Record<string, unknown>) => {
        onTelemetry?.({
          type,
          file,
          meta,
          timestamp: new Date().toISOString(),
        });
      },
      [onTelemetry],
    );

    const handlePreview = useCallback(
      (file: AppUploadFile) => {
        emitTelemetry("preview_open", file);
        if (onPreview) {
          onPreview(file);
          return;
        }
        if (typeof window === "undefined") return;
        const directUrl = file.url ?? file.thumbUrl;
        if (directUrl) {
          window.open(directUrl, "_blank", "noopener");
          return;
        }
        if (file.originFile) {
          const objectUrl = URL.createObjectURL(file.originFile);
          const opened = window.open(objectUrl, "_blank", "noopener");
          if (!opened) {
            const link = document.createElement("a");
            link.href = objectUrl;
            link.target = "_blank";
            link.rel = "noopener";
            link.click();
          }
          window.setTimeout(() => URL.revokeObjectURL(objectUrl), 60_000);
        }
      },
      [emitTelemetry, onPreview],
    );

    const emitChange = useCallback(
      (nextFiles: AppUploadFile[]) => {
        filesRef.current = nextFiles;
        if (!isControlled) {
          setInternalFiles(nextFiles);
        }
        onChange(nextFiles);
      },
      [isControlled, onChange],
    );

    const updateFile = useCallback(
      (uid: string, updater: (file: AppUploadFile) => AppUploadFile) => {
        const currentFiles = filesRef.current;
        emitChange(
          currentFiles.map((file) => (file.uid === uid ? updater(file) : file)),
        );
      },
      [emitChange],
    );

    const updateStatus = useCallback(
      (file: AppUploadFile, nextStatus: UploadStatus, patch?: Partial<AppUploadFile>) => {
        updateFile(file.uid, (current) => {
          if (!canTransition(current.status, nextStatus)) return current;
          return {
            ...current,
            ...patch,
            status: nextStatus,
          };
        });
      },
      [updateFile],
    );

    const handleRemove = useCallback(
      (file: AppUploadFile) => {
        const createdUrl = createdThumbUrlsRef.current.get(file.uid);
        if (createdUrl) {
          URL.revokeObjectURL(createdUrl);
          createdThumbUrlsRef.current.delete(file.uid);
        }
        const nextFiles = files.filter((item) => item.uid !== file.uid);
        emitChange(nextFiles);
        emitTelemetry("remove", file);
        onRemove?.(file);
      },
      [emitChange, emitTelemetry, files, onRemove],
    );

    const handleAbort = useCallback(
      (file: AppUploadFile) => {
        updateStatus(file, "removed");
        emitTelemetry("cancel", file);
        handleRemove(file);
      },
      [emitTelemetry, handleRemove, updateStatus],
    );

    const runUpload = useCallback(
      async (file: AppUploadFile) => {
        updateStatus(file, "uploading", { percent: 0 });
        emitTelemetry("upload_start", file);

        const helpers = {
          onProgress: (percent: number) => {
            const normalized = Math.min(100, Math.max(0, percent));
            updateFile(file.uid, (current) => ({
              ...current,
              percent: normalized,
            }));
            onProgress?.(file, normalized);
          },
          onSuccess: () => {
            updateStatus(file, "done", { percent: 100 });
            emitTelemetry("upload_success", file);
            onSuccess?.(file);
          },
          onError: (error: unknown) => {
            updateStatus(file, "error", { error: String(error ?? "Error") });
            emitTelemetry("upload_error", file, { error });
            onError?.(file, error);
          },
          onAbort: () => {
            updateStatus(file, "removed");
            emitTelemetry("cancel", file);
            handleRemove(file);
          },
        };

        if (!customRequest) {
          helpers.onSuccess();
          return;
        }

        try {
          await customRequest(file, helpers);
        } catch (error) {
          helpers.onError(error);
        }
      },
      [
        customRequest,
        emitTelemetry,
        handleRemove,
        onError,
        onProgress,
        onSuccess,
        updateFile,
        updateStatus,
      ],
    );

    const handleRetry = useCallback(
      (file: AppUploadFile) => {
        if (file.status !== "error") return;
        void runUpload(file);
      },
      [runUpload],
    );

    useImperativeHandle(ref, () => ({
      retry: handleRetry,
      abort: handleAbort,
    }));

    const validateFileInput = useCallback(
      async (file: RcFile, fileList: RcFile[]) => {
        if (disabled) return Upload.LIST_IGNORE;
        if (!isAcceptedFile(acceptList, file)) {
          onError?.(createAppUploadFile(file), new Error("Tipo de archivo no permitido"));
          return Upload.LIST_IGNORE;
        }
        if (maxSize && file.size > maxSize) {
          onError?.(createAppUploadFile(file), new Error("Archivo excede el tamanio permitido"));
          return Upload.LIST_IGNORE;
        }
        if (validateFile) {
          const valid = await validateFile(file);
          if (!valid) {
            onError?.(createAppUploadFile(file), new Error("Archivo invalido"));
            return Upload.LIST_IGNORE;
          }
        }
        if (beforeUpload) {
          const ok = await beforeUpload(file, fileList);
          if (!ok) return Upload.LIST_IGNORE;
        }
        return true;
      },
      [acceptList, beforeUpload, disabled, maxSize, onError, validateFile],
    );

    const handleBeforeUpload: UploadProps["beforeUpload"] = async (file, fileList) => {
      const result = await validateFileInput(file, fileList);
      if (result === Upload.LIST_IGNORE) return Upload.LIST_IGNORE;
      const nextFile = createAppUploadFile(file);
      if (isImageFile(nextFile) && !nextFile.thumbUrl && !nextFile.url) {
        const thumbUrl = URL.createObjectURL(file);
        nextFile.thumbUrl = thumbUrl;
        createdThumbUrlsRef.current.set(nextFile.uid, thumbUrl);
      }
      const nextFiles = [...files, nextFile].slice(0, maxCount ?? files.length + 1);
      emitChange(nextFiles);
      emitTelemetry("select", nextFile, { count: nextFiles.length });
      if (strategy === "auto") {
        void runUpload(nextFile);
      }
      return false;
    };

    const handleUploadClick = () => {
      onUpload?.();
      if (strategy === "manual") {
        files
          .filter((file) => file.status === "queued")
          .forEach((file) => void runUpload(file));
      }
    };

    const showUploadButton = !maxCount || files.length < maxCount;

    const uploadButton = renderUploadButton ? (
      renderUploadButton()
    ) : (
      <Button
        type="primary"
        icon={<UploadOutlined />}
        onClick={handleUploadClick}
        disabled={disabled}
      >
        Cargar archivos
      </Button>
    );

    const UploadComponent = drag ? Upload.Dragger : Upload;

    return (
      <div
        className={joinClasses(
          styles.root,
          styles[`size${size.toUpperCase()}`],
          className,
        )}
        onDragEnter={() => setDragState("valid")}
        onDragLeave={() => setDragState(null)}
        onDragOver={(event) => {
          if (!drag) return;
          const items = Array.from(event.dataTransfer?.items ?? []);
          if (!items.length) {
            setDragState("valid");
            return;
          }
          const invalid = items.some((item) => {
            const fileName = item.getAsFile()?.name ?? "";
            const fileType = item.type ?? "";
            const fakeFile = new File([], fileName, { type: fileType });
            return !isAcceptedFile(acceptList, fakeFile);
          });
          setDragState(invalid ? "invalid" : "valid");
        }}
        onDrop={() => setDragState(null)}
      >
        {showUploadButton ? (
          <UploadComponent
            multiple
            accept={accept}
            disabled={disabled}
            showUploadList={false}
            beforeUpload={handleBeforeUpload}
            className={joinClasses(
              styles.uploader,
              dragState === "valid" && styles.dragValid,
              dragState === "invalid" && styles.dragInvalid,
            )}
          >
            {drag ? (
              <div className={styles.dragLabel}>
                <UploadOutlined className={styles.dragIcon} aria-hidden="true" />
                <span>Agregar archivos</span>
              </div>
            ) : (
              uploadButton
            )}
          </UploadComponent>
        ) : null}

        <div
          className={joinClasses(
            styles.list,
            layout === "grid" ? styles.grid : styles.listView,
          )}
          role="list"
        >
          {files.map((file) => (
            <AppUploadItem
              key={file.uid}
              file={file}
              layout={layout}
              onRemove={handleRemove}
              onRetry={handleRetry}
              onAbort={handleAbort}
              onPreview={handlePreview}
              previewOnClick={previewOnClick}
              renderItem={renderItem}
              renderActions={renderActions}
              disabled={disabled}
            />
          ))}
        </div>
      </div>
    );
  },
);
