import {
  ClearOutlined,
  CloseOutlined,
  DeleteOutlined,
  EyeOutlined,
  FileOutlined,
  SaveOutlined,
} from "@ant-design/icons";
import { Progress, Tooltip } from "antd";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { AppButton } from "../AppButton";
import { AppUpload } from "../AppUpload";
import type { AppUploadFile } from "../AppUpload";
import styles from "./AppUploadBatchView.module.css";
import type {
  AppUploadBatchFileItem,
  AppUploadBatchFileState,
  AppUploadBatchSummary,
  AppUploadBatchViewProps,
} from "./AppUploadBatchView.types";

const DEFAULT_TITLE = "Carga de archivos";
const DEFAULT_EMPTY_MESSAGE = "No hay archivos en la cola.";
const PREVIEW_EXIT_ANIMATION_MS = 180;

const ACTIVE_STATES: AppUploadBatchFileState[] = ["uploading", "completing", "storing"];
const IMAGE_EXTENSIONS = [".png", ".jpg", ".jpeg", ".gif", ".webp", ".bmp", ".svg"];
type PreviewVisibilityState = "closed" | "open" | "closing";

const STATUS_LABELS: Record<AppUploadBatchFileState, string> = {
  queued: "En cola",
  validating: "Validando",
  ready: "Listo",
  uploading: "Cargando",
  completing: "Completando",
  storing: "Guardando",
  done: "Guardado",
  warning: "Advertencia",
  error: "Error",
  cancelled: "Cancelado",
  removed: "Removido",
};

const clampPercent = (value?: number) => {
  if (typeof value !== "number" || Number.isNaN(value)) return 0;
  return Math.min(100, Math.max(0, Math.round(value)));
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const formatBytes = (size: number) => {
  if (!Number.isFinite(size) || size <= 0) return "0 B";
  const units = ["B", "KB", "MB", "GB"];
  const index = Math.min(Math.floor(Math.log(size) / Math.log(1024)), units.length - 1);
  const value = size / 1024 ** index;
  return `${value >= 10 || index === 0 ? value.toFixed(0) : value.toFixed(1)} ${units[index]}`;
};

const getNormalizedExtension = (item: AppUploadBatchFileItem) => {
  const extension = item.extension || item.name.split(".").pop() || "";
  return extension.startsWith(".") ? extension.toLowerCase() : `.${extension.toLowerCase()}`;
};

const isPdf = (item: AppUploadBatchFileItem) =>
  item.file.type === "application/pdf" || getNormalizedExtension(item) === ".pdf";

const isImage = (item: AppUploadBatchFileItem) =>
  item.file.type.startsWith("image/") || IMAGE_EXTENSIONS.includes(getNormalizedExtension(item));

const buildSummary = (files: ReadonlyArray<AppUploadBatchFileItem>): AppUploadBatchSummary =>
  files.reduce<AppUploadBatchSummary>(
    (current, item) => {
      const next = { ...current, total: current.total + 1 };
      if (item.state === "ready") next.ready += 1;
      if (item.state === "done") next.done += 1;
      if (item.state === "warning") next.warning += 1;
      if (item.state === "error") next.error += 1;
      if (item.state === "cancelled") next.cancelled += 1;
      if (item.state === "queued" || item.state === "validating") next.queued += 1;
      if (ACTIVE_STATES.includes(item.state)) next.uploading += 1;
      return next;
    },
    {
      total: 0,
      queued: 0,
      ready: 0,
      uploading: 0,
      done: 0,
      warning: 0,
      error: 0,
      cancelled: 0,
    },
  );

const isFile = (value: File | undefined): value is File => value instanceof File;

export const AppUploadBatchView = <TMetadata = unknown,>({
  title = DEFAULT_TITLE,
  description,
  files,
  selectedUid,
  accept,
  maxSize,
  multiple = true,
  drag = true,
  disabled = false,
  loading = false,
  canSaveAll = true,
  canClearAll = true,
  canAddFiles = true,
  canPreview = true,
  canSaveOne = false,
  emptyMessage = DEFAULT_EMPTY_MESSAGE,
  summary,
  onFilesSelected,
  onSelectFile,
  onPreviewFile,
  onRemoveFile,
  onSaveFile,
  onSaveAll,
  onClearAll,
  onClosePreview,
  renderMetadata,
  renderPreview,
  renderFileName,
  renderFooterExtra,
}: AppUploadBatchViewProps<TMetadata>) => {
  const [previewVisibility, setPreviewVisibility] = useState<PreviewVisibilityState>("closed");
  const [removingUids, setRemovingUids] = useState<ReadonlySet<string>>(() => new Set());
  const removeTimersRef = useRef(new Map<string, number>());
  const previewCloseTimerRef = useRef<number | undefined>(undefined);
  const resolvedSummary = useMemo(() => summary ?? buildSummary(files), [files, summary]);
  const isPreviewMounted = previewVisibility !== "closed";
  const selectedItem = useMemo(() => {
    const explicit = selectedUid ? files.find((item) => item.uid === selectedUid) : undefined;
    return explicit ?? files.find((item) => item.selected) ?? files[0];
  }, [files, selectedUid]);

  const generatedPreviewUrl = useMemo(() => {
    if (!isPreviewMounted) return undefined;
    if (!selectedItem || selectedItem.previewUrl) return undefined;
    return URL.createObjectURL(selectedItem.file);
  }, [isPreviewMounted, selectedItem]);

  useEffect(
    () => () => {
      if (generatedPreviewUrl) {
        URL.revokeObjectURL(generatedPreviewUrl);
      }
    },
    [generatedPreviewUrl],
  );

  useEffect(
    () => () => {
      removeTimersRef.current.forEach((timerId) => window.clearTimeout(timerId));
      removeTimersRef.current.clear();
      if (previewCloseTimerRef.current) {
        window.clearTimeout(previewCloseTimerRef.current);
      }
    },
    [],
  );

  const previewUrl = selectedItem?.previewUrl ?? generatedPreviewUrl;
  const isBlocked = disabled || loading;

  const handleFilesChange = useCallback(
    (uploadFiles: AppUploadFile[]) => {
      const selectedFiles = uploadFiles.map((item) => item.originFile).filter(isFile);
      if (selectedFiles.length > 0) {
        onFilesSelected?.(selectedFiles);
      }
    },
    [onFilesSelected],
  );

  const handleBeforeUpload = useCallback(
    (file: File, fileList: File[]) => {
      if (fileList[0] === file) {
        onFilesSelected?.(fileList);
      }
      return false;
    },
    [onFilesSelected],
  );

  const handleClosePreview = useCallback(() => {
    if (previewCloseTimerRef.current) {
      window.clearTimeout(previewCloseTimerRef.current);
    }
    setPreviewVisibility("closing");
    previewCloseTimerRef.current = window.setTimeout(() => {
      previewCloseTimerRef.current = undefined;
      setPreviewVisibility("closed");
    }, PREVIEW_EXIT_ANIMATION_MS);
    onClosePreview?.();
  }, [onClosePreview]);

  const handleOpenPreview = useCallback(
    (uid: string) => {
      if (previewCloseTimerRef.current) {
        window.clearTimeout(previewCloseTimerRef.current);
        previewCloseTimerRef.current = undefined;
      }
      onSelectFile?.(uid);
      onPreviewFile?.(uid);
      setPreviewVisibility("open");
    },
    [onPreviewFile, onSelectFile],
  );

  const handleRemoveFile = useCallback(
    (uid: string) => {
      if (removeTimersRef.current.has(uid)) {
        return;
      }

      setRemovingUids((current) => {
        const next = new Set(current);
        next.add(uid);
        return next;
      });

      const timerId = window.setTimeout(() => {
        removeTimersRef.current.delete(uid);
        setRemovingUids((current) => {
          const next = new Set(current);
          next.delete(uid);
          return next;
        });
        onRemoveFile?.(uid);
      }, 220);

      removeTimersRef.current.set(uid, timerId);
    },
    [onRemoveFile],
  );

  const renderDefaultPreview = () => {
    if (!selectedItem) {
      return (
        <div className={styles.previewEmpty}>
          <FileOutlined aria-hidden="true" />
          <span>Selecciona un archivo para ver la vista previa.</span>
        </div>
      );
    }

    if (isPdf(selectedItem) && previewUrl) {
      return (
        <iframe
          className={styles.previewFrame}
          src={previewUrl}
          title={`Vista previa de ${selectedItem.name}`}
        />
      );
    }

    if (isImage(selectedItem) && previewUrl) {
      return <img className={styles.previewImage} src={previewUrl} alt={selectedItem.name} />;
    }

    return (
      <div className={styles.previewFallback}>
        <FileOutlined aria-hidden="true" />
        <strong>{selectedItem.name}</strong>
        <span>
          {getNormalizedExtension(selectedItem).toUpperCase()} | {formatBytes(selectedItem.size)}
        </span>
      </div>
    );
  };

  return (
    <section className={styles.root} aria-label={title} data-preview-open={isPreviewMounted ? "true" : "false"}>
      <header className={styles.header}>
        <div className={styles.heading}>
          <h2 className={styles.title}>{title}</h2>
          {description ? <p className={styles.description}>{description}</p> : null}
        </div>
        <div className={styles.summary} aria-live="polite">
          <span>{resolvedSummary.total} archivo(s)</span>
          <span>{resolvedSummary.done} guardado(s)</span>
          {resolvedSummary.error > 0 ? <span>{resolvedSummary.error} error(es)</span> : null}
        </div>
      </header>

      <div className={styles.toolbar} aria-label="Acciones globales de carga">
        {canAddFiles ? (
          <div className={styles.uploadSlot}>
            <AppUpload
              value={[]}
              layout="list"
              accept={accept}
              maxSize={maxSize}
              maxCount={multiple ? undefined : 1}
              disabled={isBlocked}
              size="sm"
              drag={drag}
              strategy="manual"
              beforeUpload={handleBeforeUpload}
              onChange={handleFilesChange}
            />
          </div>
        ) : null}

        <div className={styles.globalActions}>
          <AppButton
            variant="primary"
            size="sm"
            leftIcon={<SaveOutlined />}
            disabled={isBlocked || !canSaveAll || files.length === 0}
            onClick={onSaveAll}
          >
            Guardar todo
          </AppButton>
          <AppButton
            variant="danger"
            size="sm"
            leftIcon={<ClearOutlined />}
            disabled={isBlocked || !canClearAll || files.length === 0}
            onClick={onClearAll}
          >
            Limpiar todo
          </AppButton>
        </div>
      </div>

      <div className={styles.workspace}>
        <div className={styles.fileListPanel}>
          <div className={styles.panelHeader}>
            <span>Cola de archivos</span>
            <span>{resolvedSummary.total} archivo(s)</span>
          </div>

          {files.length === 0 ? (
            <div className={styles.emptyState}>{emptyMessage}</div>
          ) : (
            <div className={styles.fileList} role="list">
              {files.map((item) => {
                const selected = selectedItem?.uid === item.uid;
                const itemDisabled = isBlocked || Boolean(item.disabled);
                const isRemoving = removingUids.has(item.uid);
                const itemProgress = clampPercent(item.progress);
                const canUsePreview = canPreview && !itemDisabled && !isRemoving;
                const canUseRemove = !itemDisabled && !isRemoving && item.state !== "done";
                const canUseSaveOne = canSaveOne && !itemDisabled && !isRemoving;

                return (
                  <article
                    key={item.uid}
                    className={joinClasses(styles.fileRow, selected && styles.fileRowActive)}
                    data-state={item.state}
                    data-removing={isRemoving ? "true" : "false"}
                    role="listitem"
                  >
                    <button
                      type="button"
                      className={styles.fileMain}
                      onClick={() => onSelectFile?.(item.uid)}
                      disabled={itemDisabled}
                      aria-pressed={selected}
                    >
                      <span className={styles.fileName} title={item.name}>
                        {renderFileName ? renderFileName(item) : item.name}
                      </span>
                      <span className={styles.fileMeta}>
                        <span>{formatBytes(item.size)}</span>
                        <span className={styles.statusBadge} data-state={item.state}>
                          {STATUS_LABELS[item.state]}
                        </span>
                      </span>
                      {item.phaseLabel ? (
                        <span className={styles.phaseLabel}>{item.phaseLabel}</span>
                      ) : null}
                    </button>

                    {ACTIVE_STATES.includes(item.state) ? (
                      <Progress
                        className={styles.fileProgress}
                        aria-label={`Progreso de ${item.name}`}
                        percent={itemProgress}
                        size="small"
                        showInfo={false}
                      />
                    ) : null}

                    {item.warning ? <p className={styles.warningText}>{item.warning}</p> : null}
                    {item.error ? <p className={styles.errorText}>{item.error}</p> : null}

                    {renderMetadata ? (
                      <div className={styles.metadataSlot}>
                        {renderMetadata({ item, disabled: itemDisabled })}
                      </div>
                    ) : null}

                    <div className={styles.rowActions}>
                      <AppButton
                        variant="secondary"
                        size="sm"
                        icon={<EyeOutlined />}
                        tooltip="Ver archivo"
                        aria-label={`Ver ${item.name}`}
                        disabled={!canUsePreview}
                        onClick={() => handleOpenPreview(item.uid)}
                      />
                      {canSaveOne ? (
                        <AppButton
                          variant="primary"
                          size="sm"
                          icon={<SaveOutlined />}
                          tooltip="Guardar archivo"
                          aria-label={`Guardar ${item.name}`}
                          disabled={!canUseSaveOne}
                          onClick={() => onSaveFile?.(item.uid)}
                        />
                      ) : null}
                      <AppButton
                        variant="danger"
                        size="sm"
                        icon={<DeleteOutlined />}
                        tooltip="Eliminar archivo"
                        aria-label={`Eliminar ${item.name}`}
                        disabled={!canUseRemove}
                        onClick={() => handleRemoveFile(item.uid)}
                      />
                    </div>
                  </article>
                );
              })}
            </div>
          )}
        </div>

        {canPreview && isPreviewMounted ? (
          <aside
            className={styles.previewPanel}
            data-preview-state={previewVisibility}
            aria-label="Vista previa del archivo activo"
          >
            <div className={styles.panelHeader}>
              <Tooltip title={selectedItem?.name}>
                <span className={styles.previewTitle}>
                  {selectedItem ? selectedItem.name : "Vista previa"}
                </span>
              </Tooltip>
              <AppButton
                variant="ghost"
                size="sm"
                icon={<CloseOutlined />}
                tooltip="Cerrar vista previa"
                aria-label="Cerrar vista previa"
                disabled={isBlocked}
                onClick={handleClosePreview}
              />
            </div>
            <div className={styles.previewSurface}>
              {selectedItem && renderPreview
                ? renderPreview({ item: selectedItem, previewUrl, onClose: handleClosePreview })
                : renderDefaultPreview()}
            </div>
          </aside>
        ) : null}
      </div>

      <footer className={styles.footer}>
        <div className={styles.footerSummary} aria-live="polite">
          <span>Total: {resolvedSummary.total}</span>
          <span>Listos: {resolvedSummary.ready}</span>
          <span>En proceso: {resolvedSummary.uploading}</span>
          <span>Guardados: {resolvedSummary.done}</span>
          <span>Advertencias: {resolvedSummary.warning}</span>
          <span>Errores: {resolvedSummary.error}</span>
          <span>Cancelados: {resolvedSummary.cancelled}</span>
        </div>
        {renderFooterExtra ? (
          <div className={styles.footerExtra}>{renderFooterExtra(resolvedSummary)}</div>
        ) : null}
      </footer>
    </section>
  );
};
