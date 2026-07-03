import { Alert, Progress } from "antd";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { AppButton } from "../AppButton";
import { AppModal } from "../AppModal";
import type {
  AppProgressBatchItemContext,
  AppProgressBatchItemResult,
  AppProgressBatchLifecycle,
  AppProgressBatchProps,
  AppProgressBatchSummary,
} from "./AppProgressBatch.types";
import styles from "./AppProgressBatch.module.css";

const DEFAULT_TITLE = "Proceso por lotes";
const DEFAULT_EMPTY_MESSAGE = "No hay elementos para procesar.";
const DEFAULT_CANCEL_CONFIRM_MESSAGE = "Hay un proceso en curso. Desea cancelarlo?";
const DEFAULT_PHASE = "Preparando ejecucion";
const DEFAULT_COMPLETE_MESSAGE = "Proceso completado.";
const DEFAULT_CANCELLED_MESSAGE = "Proceso cancelado.";
const DEFAULT_FATAL_MESSAGE = "El proceso se detuvo por un error fatal.";
const INVALID_RESULT_MESSAGE = "processItem retorno un resultado invalido.";

type ControlledDecision = "continue" | "cancel";

type InternalBatchState = {
  lifecycle: AppProgressBatchLifecycle;
  runId: number;
  currentIndex: number;
  currentLabel: string;
  currentPhase: string;
  globalPercent: number;
  itemPercent: number;
  message: string | null;
  summary: AppProgressBatchSummary;
  showCancelConfirm: boolean;
  pendingControlledError: Extract<
    AppProgressBatchItemResult,
    { status: "controlled-error" }
  > | null;
  warnings: string[];
  skippedMessages: string[];
  controlledErrorMessages: string[];
  fatalErrorMessage: string | null;
};

const isObjectRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

function isValidBatchItemResult(value: unknown): value is AppProgressBatchItemResult {
  if (!isObjectRecord(value) || typeof value.status !== "string") {
    return false;
  }

  switch (value.status) {
    case "success":
      return true;
    case "warning":
    case "controlled-error":
    case "fatal-error":
      return typeof value.message === "string";
    case "skipped":
      return value.message === undefined || typeof value.message === "string";
    default:
      return false;
  }
}

const createSummary = (total: number): AppProgressBatchSummary => ({
  total,
  processed: 0,
  success: 0,
  warnings: 0,
  skipped: 0,
  controlledErrors: 0,
  fatalErrors: 0,
  cancelled: false,
});

const createInitialState = (): InternalBatchState => ({
  lifecycle: "idle",
  runId: 0,
  currentIndex: 0,
  currentLabel: "",
  currentPhase: DEFAULT_PHASE,
  globalPercent: 0,
  itemPercent: 0,
  message: null,
  summary: createSummary(0),
  showCancelConfirm: false,
  pendingControlledError: null,
  warnings: [],
  skippedMessages: [],
  controlledErrorMessages: [],
  fatalErrorMessage: null,
});

const clampPercent = (percent: number) => {
  if (!Number.isFinite(percent)) {
    return 0;
  }

  return Math.min(100, Math.max(0, Math.round(percent)));
};

const calculateGlobalPercent = (summary: AppProgressBatchSummary) =>
  summary.total === 0 ? 0 : clampPercent((summary.processed / summary.total) * 100);

const isActiveLifecycle = (lifecycle: AppProgressBatchLifecycle) =>
  lifecycle === "running" || lifecycle === "paused" || lifecycle === "cancelling";

const defaultItemLabel = (index: number) => `Elemento ${index + 1}`;

export function AppProgressBatch<TItem>({
  open,
  items,
  onOpenChange,
  processItem,
  title,
  processName,
  autoStart = false,
  confirmOnCancel = true,
  emptyMessage = DEFAULT_EMPTY_MESSAGE,
  closeOnComplete = false,
  getItemLabel,
  onComplete,
  onCancel,
  onError,
}: AppProgressBatchProps<TItem>) {
  const [state, setState] = useState<InternalBatchState>(() => createInitialState());
  const stateRef = useRef(state);
  const summaryRef = useRef(state.summary);
  const activeRunIdRef = useRef<number | null>(null);
  const nextRunIdRef = useRef(0);
  const abortControllerRef = useRef<AbortController | null>(null);
  const mountedRef = useRef(false);
  const autoStartedRef = useRef(false);
  const emptyCompletedRef = useRef(false);
  const controlledDecisionRef = useRef<((decision: ControlledDecision) => void) | null>(
    null,
  );

  const modalTitle = title ?? processName ?? DEFAULT_TITLE;
  const hasItems = items.length > 0;
  const queuedItemLabels = useMemo(
    () =>
      items.map((item, index) =>
        getItemLabel ? getItemLabel(item, index) : defaultItemLabel(index),
      ),
    [getItemLabel, items],
  );
  const visibleQueuedItemLabels = queuedItemLabels.slice(0, 6);
  const hiddenQueuedItems = Math.max(0, queuedItemLabels.length - visibleQueuedItemLabels.length);

  const setGuardedState = useCallback(
    (
      runId: number,
      updater: (previous: InternalBatchState) => InternalBatchState,
    ) => {
      if (!mountedRef.current || activeRunIdRef.current !== runId) {
        return;
      }

      setState((previous) => {
        return updater(previous);
      });
    },
    [],
  );

  const updateSummary = useCallback(
    (
      runId: number,
      updater: (previous: AppProgressBatchSummary) => AppProgressBatchSummary,
    ) => {
      if (!mountedRef.current || activeRunIdRef.current !== runId) {
        return summaryRef.current;
      }

      const nextSummary = updater(summaryRef.current);
      summaryRef.current = nextSummary;

      setGuardedState(runId, (previous) => ({
        ...previous,
        summary: nextSummary,
        globalPercent: calculateGlobalPercent(nextSummary),
      }));

      return nextSummary;
    },
    [setGuardedState],
  );

  const invalidateActiveRun = useCallback(() => {
    activeRunIdRef.current = null;
    controlledDecisionRef.current?.("cancel");
    controlledDecisionRef.current = null;
    abortControllerRef.current?.abort();
    abortControllerRef.current = null;
  }, []);

  const finishCancelledRun = useCallback(
    (countControlledError: boolean) => {
      const runId = activeRunIdRef.current;

      if (runId === null) {
        return;
      }

      setGuardedState(runId, (previous) => ({
        ...previous,
        lifecycle: "cancelling",
        showCancelConfirm: false,
        message: DEFAULT_CANCELLED_MESSAGE,
      }));

      const cancelledSummary = updateSummary(runId, (previous) => ({
        ...previous,
        processed: countControlledError ? previous.processed + 1 : previous.processed,
        controlledErrors: countControlledError
          ? previous.controlledErrors + 1
          : previous.controlledErrors,
        cancelled: true,
      }));

      abortControllerRef.current?.abort();
      controlledDecisionRef.current?.("cancel");
      controlledDecisionRef.current = null;
      activeRunIdRef.current = null;
      abortControllerRef.current = null;

      if (mountedRef.current) {
        setState((previous) => ({
          ...previous,
          runId,
          lifecycle: "completed",
          showCancelConfirm: false,
          pendingControlledError: null,
          message: DEFAULT_CANCELLED_MESSAGE,
          summary: cancelledSummary,
          globalPercent: calculateGlobalPercent(cancelledSummary),
        }));
      }

      onCancel?.(cancelledSummary);
    },
    [onCancel, setGuardedState, updateSummary],
  );

  const requestCancel = useCallback(() => {
    const countControlledError = Boolean(stateRef.current.pendingControlledError);

    if (confirmOnCancel && isActiveLifecycle(stateRef.current.lifecycle)) {
      setState((previous) => ({
        ...previous,
        showCancelConfirm: true,
      }));
      return;
    }

    finishCancelledRun(countControlledError);
  }, [confirmOnCancel, finishCancelledRun]);

  const closeWhenInactive = useCallback(() => {
    onOpenChange(false);
  }, [onOpenChange]);

  const handleModalClose = useCallback(() => {
    if (isActiveLifecycle(stateRef.current.lifecycle)) {
      requestCancel();
      return;
    }

    closeWhenInactive();
  }, [closeWhenInactive, requestCancel]);

  const waitControlledDecision = useCallback(
    () =>
      new Promise<ControlledDecision>((resolve) => {
        controlledDecisionRef.current = resolve;
      }),
    [],
  );

  const continueControlledError = useCallback(() => {
    controlledDecisionRef.current?.("continue");
    controlledDecisionRef.current = null;
  }, []);

  const dismissCancelConfirm = useCallback(() => {
    setState((previous) => ({
      ...previous,
      showCancelConfirm: false,
    }));
  }, []);

  const startBatch = useCallback(async () => {
    if (isActiveLifecycle(stateRef.current.lifecycle)) {
      return;
    }

    if (items.length === 0) {
      if (emptyCompletedRef.current) {
        return;
      }

      emptyCompletedRef.current = true;
      const emptySummary = createSummary(0);
      summaryRef.current = emptySummary;
      setState((previous) => ({
        ...previous,
        lifecycle: "completed",
        currentIndex: 0,
        currentLabel: "",
        currentPhase: "",
        globalPercent: 0,
        itemPercent: 0,
        message: emptyMessage,
        summary: emptySummary,
        warnings: [],
        skippedMessages: [],
        controlledErrorMessages: [],
        fatalErrorMessage: null,
      }));
      onComplete?.(emptySummary);
      return;
    }

    const runId = nextRunIdRef.current + 1;
    nextRunIdRef.current = runId;
    activeRunIdRef.current = runId;
    const runItems = [...items];
    const controller = new AbortController();
    abortControllerRef.current = controller;
    const initialSummary = createSummary(runItems.length);
    summaryRef.current = initialSummary;

    setState({
      ...createInitialState(),
      lifecycle: "running",
      runId,
      currentIndex: 0,
      currentLabel: getItemLabel?.(runItems[0], 0) ?? defaultItemLabel(0),
      currentPhase: DEFAULT_PHASE,
      summary: initialSummary,
    });

    for (let index = 0; index < runItems.length; index += 1) {
      if (activeRunIdRef.current !== runId || controller.signal.aborted) {
        return;
      }

      const initialLabel = getItemLabel?.(runItems[index], index) ?? defaultItemLabel(index);

      setGuardedState(runId, (previous) => ({
        ...previous,
        lifecycle: "running",
        currentIndex: index,
        currentLabel: initialLabel,
        currentPhase: DEFAULT_PHASE,
        itemPercent: 0,
        message: null,
        showCancelConfirm: false,
        pendingControlledError: null,
      }));

      const context: AppProgressBatchItemContext = {
        index,
        total: runItems.length,
        signal: controller.signal,
        setCurrentLabel: (label) => {
          setGuardedState(runId, (previous) => ({
            ...previous,
            currentLabel: label,
          }));
        },
        setItemProgress: (percent) => {
          const normalizedPercent = clampPercent(percent);
          setGuardedState(runId, (previous) =>
            previous.itemPercent === normalizedPercent
              ? previous
              : {
                  ...previous,
                  itemPercent: normalizedPercent,
                },
          );
        },
        setPhase: (phase) => {
          setGuardedState(runId, (previous) => ({
            ...previous,
            currentPhase: phase,
          }));
        },
      };

      let result: AppProgressBatchItemResult;

      try {
        const rawResult = await processItem(runItems[index], context);

        if (activeRunIdRef.current !== runId || controller.signal.aborted) {
          return;
        }

        if (!isValidBatchItemResult(rawResult)) {
          throw new Error(INVALID_RESULT_MESSAGE);
        }

        result = rawResult;
      } catch (error) {
        if (activeRunIdRef.current !== runId || controller.signal.aborted) {
          return;
        }

        const message = error instanceof Error ? error.message : DEFAULT_FATAL_MESSAGE;
        const errorSummary = updateSummary(runId, (previous) => ({
          ...previous,
          fatalErrors: previous.fatalErrors + 1,
        }));

        activeRunIdRef.current = null;
        abortControllerRef.current = null;
        setState((previous) => ({
          ...previous,
          runId,
          lifecycle: "error",
          message,
          fatalErrorMessage: message,
          summary: errorSummary,
          globalPercent: calculateGlobalPercent(errorSummary),
        }));
        onError?.(error);
        return;
      }

      if (result.status === "controlled-error") {
        setGuardedState(runId, (previous) => ({
          ...previous,
          lifecycle: "paused",
          message: result.message,
          pendingControlledError: result,
          controlledErrorMessages: [...previous.controlledErrorMessages, result.message],
        }));

        const decision = await waitControlledDecision();

        if (activeRunIdRef.current !== runId || controller.signal.aborted) {
          return;
        }

        if (decision === "cancel") {
          finishCancelledRun(true);
          return;
        }

        updateSummary(runId, (previous) => ({
          ...previous,
          processed: previous.processed + 1,
          controlledErrors: previous.controlledErrors + 1,
        }));

        setGuardedState(runId, (previous) => ({
          ...previous,
          lifecycle: "running",
          pendingControlledError: null,
          message: null,
          itemPercent: 100,
        }));
        continue;
      }

      if (result.status === "fatal-error") {
        const errorSummary = updateSummary(runId, (previous) => ({
          ...previous,
          fatalErrors: previous.fatalErrors + 1,
        }));

        activeRunIdRef.current = null;
        abortControllerRef.current = null;
        setState((previous) => ({
          ...previous,
          runId,
          lifecycle: "error",
          message: result.message,
          fatalErrorMessage: result.message,
          summary: errorSummary,
          globalPercent: calculateGlobalPercent(errorSummary),
          itemPercent: 100,
        }));
        onError?.(new Error(result.message));
        return;
      }

      if (result.status === "warning") {
        updateSummary(runId, (previous) => ({
          ...previous,
          processed: previous.processed + 1,
          warnings: previous.warnings + 1,
        }));
        setGuardedState(runId, (previous) => ({
          ...previous,
          warnings: [...previous.warnings, result.message],
          itemPercent: 100,
        }));
        continue;
      }

      if (result.status === "skipped") {
        updateSummary(runId, (previous) => ({
          ...previous,
          processed: previous.processed + 1,
          skipped: previous.skipped + 1,
        }));
        setGuardedState(runId, (previous) => ({
          ...previous,
          skippedMessages: result.message
            ? [...previous.skippedMessages, result.message]
            : previous.skippedMessages,
          itemPercent: 100,
        }));
        continue;
      }

      updateSummary(runId, (previous) => ({
        ...previous,
        processed: previous.processed + 1,
        success: previous.success + 1,
      }));
      setGuardedState(runId, (previous) => ({
        ...previous,
        itemPercent: 100,
      }));
    }

    if (activeRunIdRef.current !== runId || controller.signal.aborted) {
      return;
    }

    const completedSummary = summaryRef.current;
    activeRunIdRef.current = null;
    abortControllerRef.current = null;
    setState((previous) => ({
      ...previous,
      runId,
      lifecycle: "completed",
      message: DEFAULT_COMPLETE_MESSAGE,
      summary: completedSummary,
      globalPercent: calculateGlobalPercent(completedSummary),
      itemPercent: 100,
    }));
    onComplete?.(completedSummary);

    if (closeOnComplete) {
      onOpenChange(false);
    }
  }, [
    closeOnComplete,
    emptyMessage,
    finishCancelledRun,
    getItemLabel,
    items,
    onComplete,
    onError,
    onOpenChange,
    processItem,
    setGuardedState,
    updateSummary,
    waitControlledDecision,
  ]);

  useEffect(() => {
    stateRef.current = state;
    summaryRef.current = state.summary;
  }, [state]);

  useEffect(() => {
    mountedRef.current = true;

    return () => {
      mountedRef.current = false;
      invalidateActiveRun();
    };
  }, [invalidateActiveRun]);

  useEffect(() => {
    if (!open) {
      invalidateActiveRun();
      autoStartedRef.current = false;
      emptyCompletedRef.current = false;
      setState(createInitialState());
    }
  }, [invalidateActiveRun, open]);

  useEffect(() => {
    if (!open) {
      return;
    }

    if (items.length === 0 && !emptyCompletedRef.current) {
      void startBatch();
      return;
    }

    if (
      autoStart &&
      items.length > 0 &&
      stateRef.current.lifecycle === "idle" &&
      !autoStartedRef.current
    ) {
      autoStartedRef.current = true;
      void startBatch();
    }
  }, [autoStart, items.length, open, startBatch]);

  const processedCounter = state.summary.total === 0
    ? "0 de 0"
    : `${state.summary.processed} de ${state.summary.total}`;
  const currentCounter = state.summary.total === 0
    ? "0 de 0"
    : `${Math.min(state.currentIndex + 1, state.summary.total)} de ${state.summary.total}`;
  const visibleCounter = state.lifecycle === "running" || state.lifecycle === "paused"
    ? currentCounter
    : processedCounter;

  const showItemProgress =
    state.lifecycle === "running" || state.lifecycle === "paused" || state.itemPercent > 0;
  const canContinueControlled =
    state.pendingControlledError?.canContinue !== false && state.lifecycle === "paused";

  const visibleMessage = useMemo(() => {
    if (state.showCancelConfirm) {
      return (
        <Alert
          type="warning"
          showIcon
          title={DEFAULT_CANCEL_CONFIRM_MESSAGE}
          action={
            <div className={styles.footer}>
              <AppButton size="sm" variant="secondary" onClick={dismissCancelConfirm}>
                Volver
              </AppButton>
              <AppButton
                size="sm"
                variant="danger"
                onClick={() => finishCancelledRun(Boolean(state.pendingControlledError))}
              >
                Confirmar
              </AppButton>
            </div>
          }
        />
      );
    }

    if (state.lifecycle === "error" && state.fatalErrorMessage) {
      return <Alert type="error" showIcon title={state.fatalErrorMessage} />;
    }

    if (state.lifecycle === "paused" && state.message) {
      return <Alert type="warning" showIcon title={state.message} />;
    }

    if (state.summary.total === 0 && state.lifecycle === "completed" && state.message) {
      return <Alert type="info" showIcon title={state.message} />;
    }

    if (state.lifecycle === "completed" && state.summary.cancelled) {
      return <Alert type="warning" showIcon title={state.message ?? DEFAULT_CANCELLED_MESSAGE} />;
    }

    if (state.lifecycle === "completed" && state.message) {
      return <Alert type="success" showIcon title={state.message} />;
    }

    if (state.warnings.length > 0) {
      return <Alert type="warning" showIcon title={state.warnings[state.warnings.length - 1]} />;
    }

    return null;
  }, [dismissCancelConfirm, finishCancelledRun, state]);

  const footer = useMemo(() => {
    if (state.showCancelConfirm) {
      return null;
    }

    if (state.lifecycle === "idle" && hasItems) {
      return (
        <>
          <AppButton variant="secondary" onClick={closeWhenInactive}>
            Cerrar
          </AppButton>
          <AppButton onClick={() => void startBatch()}>Iniciar</AppButton>
        </>
      );
    }

    if (state.lifecycle === "idle" || (state.lifecycle === "completed" && !hasItems)) {
      return (
        <AppButton variant="secondary" onClick={closeWhenInactive}>
          Cerrar
        </AppButton>
      );
    }

    if (state.lifecycle === "running") {
      return (
        <AppButton variant="danger" onClick={requestCancel}>
          Cancelar
        </AppButton>
      );
    }

    if (state.lifecycle === "paused") {
      return (
        <>
          <AppButton variant="danger" onClick={requestCancel}>
            Cancelar
          </AppButton>
          {canContinueControlled ? (
            <AppButton onClick={continueControlledError}>Continuar</AppButton>
          ) : null}
        </>
      );
    }

    if (state.lifecycle === "cancelling") {
      return (
        <AppButton loading disabled>
          Cancelando
        </AppButton>
      );
    }

    return (
      <AppButton variant="secondary" onClick={closeWhenInactive}>
        Cerrar
      </AppButton>
    );
  }, [
    canContinueControlled,
    closeWhenInactive,
    continueControlledError,
    hasItems,
    requestCancel,
    startBatch,
    state.lifecycle,
    state.showCancelConfirm,
  ]);

  return (
    <AppModal
      open={open}
      title={modalTitle}
      onClose={handleModalClose}
      maskClosable={false}
      centered
      closeOnEscape
      hideFooter
    >
      <section className={styles.shell} aria-label={modalTitle}>
        {processName && processName !== modalTitle ? (
          <p className={styles.processName}>{processName}</p>
        ) : null}

        <div className={styles.statusPanel}>
          <div className={styles.statusHeader}>
            <div className={styles.statusText}>
              <span className={styles.eyebrow}>Item actual</span>
              <span className={styles.currentLabel} title={state.currentLabel || "Sin item"}>
                {state.currentLabel || "Sin item activo"}
              </span>
            </div>
            <span className={styles.counter} aria-live="polite">
              {visibleCounter}
            </span>
          </div>
          <p className={styles.phase} title={state.currentPhase}>
            {state.currentPhase}
          </p>
        </div>

        {state.lifecycle === "idle" && hasItems ? (
          <div className={styles.queuePreview} aria-label="Items en cola">
            <div className={styles.queueHeader}>
              <span className={styles.eyebrow}>Items en cola</span>
              <span className={styles.queueCount}>{items.length}</span>
            </div>
            <ul className={styles.queueList}>
              {visibleQueuedItemLabels.map((label, index) => (
                <li className={styles.queueItem} key={`${label}-${index}`}>
                  <span className={styles.queueIndex}>{index + 1}</span>
                  <span className={styles.queueLabel} title={label}>
                    {label}
                  </span>
                </li>
              ))}
            </ul>
            {hiddenQueuedItems > 0 ? (
              <span className={styles.queueMore}>
                +{hiddenQueuedItems} elementos adicionales
              </span>
            ) : null}
          </div>
        ) : null}

        <div className={styles.progressStack}>
          <div className={styles.progressBlock} aria-label="Progreso global">
            <div className={styles.progressLabel}>
              <span>Progreso global</span>
              <span>{state.globalPercent}%</span>
            </div>
            <Progress percent={state.globalPercent} status="active" />
          </div>

          {showItemProgress ? (
            <div className={styles.progressBlock} aria-label="Progreso del item actual">
              <div className={styles.progressLabel}>
                <span>Progreso del item</span>
                <span>{state.itemPercent}%</span>
              </div>
              <Progress percent={state.itemPercent} size="small" />
            </div>
          ) : null}
        </div>

        <div className={styles.messageArea} aria-live="polite">
          {visibleMessage}
        </div>

        {(state.lifecycle === "completed" || state.lifecycle === "error") ? (
          <div className={styles.summary} aria-label="Resumen del proceso">
            <SummaryItem label="Total" value={state.summary.total} />
            <SummaryItem label="Procesados" value={state.summary.processed} />
            <SummaryItem label="Exitosos" value={state.summary.success} />
            <SummaryItem label="Advertencias" value={state.summary.warnings} />
            <SummaryItem label="Omitidos" value={state.summary.skipped} />
            <SummaryItem label="Controlados" value={state.summary.controlledErrors} />
            <SummaryItem label="Fatales" value={state.summary.fatalErrors} />
            <SummaryItem label="Cancelado" value={state.summary.cancelled ? "Si" : "No"} />
          </div>
        ) : null}

        {footer ? <div className={styles.footer}>{footer}</div> : null}
      </section>
    </AppModal>
  );
}

function SummaryItem({ label, value }: { label: string; value: number | string }) {
  return (
    <div className={styles.summaryItem}>
      <span className={styles.summaryValue}>{value}</span>
      <span className={styles.summaryLabel}>{label}</span>
    </div>
  );
}
