import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type {
  AppProgressBatchItemContext,
  AppProgressBatchItemResult,
  AppProgressBatchSummary,
} from "../../../../../app/Components/UI/AppProgressBatch";
import type {
  AlmacenamientoDocumentalUploadErrorCode,
  AlmacenarDocumentoRequest,
  UploadStorageProgress,
} from "../../../types/almacenamientoDocumental.types";
import { uploadAndStoreOneDocument } from "../../../services/almacenamientoDocumentalUpload.service";
import { buildUploadDocumentalInterfaceRegistration } from "../../../services/uploadDocumentalInterfaceRegistration.mapper";
import { createStorageRequestId, normalizeFileExtension } from "../../../utils/storageFile.utils";
import type {
  AlmacenarDocumentoStoredResult,
  AppUploadDocumentalProps,
  UploadDocumentalBatchSummary,
  UploadDocumentalFileMetadata,
} from "../AppUploadDocumental.types";
import type {
  UploadDocumentalFileItem,
  UseAppUploadDocumentalStateResult,
} from "./useAppUploadDocumentalState";

export type UseAppUploadDocumentalActionsInput = Pick<
  AppUploadDocumentalProps,
  | "context"
  | "proceso"
  | "modoDocumento"
  | "buildStoreRequest"
  | "storageOptions"
  | "saveAllMode"
  | "onStored"
  | "onInterfaceRegistration"
  | "onBatchComplete"
  | "onError"
> &
  Pick<
    UseAppUploadDocumentalStateResult,
    "files" | "config" | "operationId" | "validateFileForStore" | "markFile" | "setFiles"
  >;

export type UseAppUploadDocumentalActionsResult = {
  batchOpen: boolean;
  batchItems: UploadDocumentalFileItem[];
  setBatchOpen: (open: boolean) => void;
  saveOne: (uid: string) => Promise<void>;
  saveAll: () => Promise<void>;
  processBatchItem: (
    item: UploadDocumentalFileItem,
    context: AppProgressBatchItemContext,
  ) => Promise<AppProgressBatchItemResult>;
  handleBatchComplete: (summary: AppProgressBatchSummary) => void;
  cancelAll: () => void;
  cancelFile: (uid: string) => void;
  canSaveAll: boolean;
  isSavingAll: boolean;
};

export function useAppUploadDocumentalActions({
  files,
  config,
  context,
  proceso,
  modoDocumento,
  buildStoreRequest: buildStoreRequestOverride,
  storageOptions,
  saveAllMode = "progress-modal",
  operationId,
  validateFileForStore,
  markFile,
  setFiles,
  onStored,
  onInterfaceRegistration,
  onBatchComplete,
  onError,
}: UseAppUploadDocumentalActionsInput): UseAppUploadDocumentalActionsResult {
  const [batchOpen, setBatchOpen] = useState(false);
  const [batchItems, setBatchItems] = useState<UploadDocumentalFileItem[]>([]);
  const [isSavingAll, setIsSavingAll] = useState(false);
  const [, setStoredResults] = useState<AlmacenarDocumentoStoredResult[]>([]);
  const storedResultsRef = useRef<AlmacenarDocumentoStoredResult[]>([]);
  const currentOperationIdRef = useRef(operationId);
  const activeControllersRef = useRef<Set<AbortController>>(new Set());
  const activeFileControllersRef = useRef<Map<string, AbortController>>(new Map());
  const globalCancelRequestedRef = useRef(false);
  const preValidationFailedCountRef = useRef(0);

  useEffect(() => {
    if (currentOperationIdRef.current !== operationId) {
      currentOperationIdRef.current = operationId;
      activeControllersRef.current.forEach((controller) => controller.abort());
      activeControllersRef.current.clear();
      activeFileControllersRef.current.clear();
      globalCancelRequestedRef.current = false;
      setBatchOpen(false);
      setBatchItems([]);
      setIsSavingAll(false);
      setStoredResults([]);
      storedResultsRef.current = [];
      preValidationFailedCountRef.current = 0;
    }
  }, [operationId]);

  useEffect(
    () => () => {
      activeControllersRef.current.forEach((controller) => controller.abort());
      activeControllersRef.current.clear();
      activeFileControllersRef.current.clear();
    },
    [],
  );

  const canSaveAll = useMemo(
    () =>
      Boolean(config) &&
      !isSavingAll &&
      files.some((item) => item.state === "ready" || item.state === "warning" || item.state === "cancelled"),
    [config, files, isSavingAll],
  );

  const registerController = useCallback((uid: string, controller: AbortController) => {
    activeControllersRef.current.add(controller);
    activeFileControllersRef.current.set(uid, controller);
  }, []);

  const unregisterController = useCallback((uid: string, controller: AbortController) => {
    activeControllersRef.current.delete(controller);
    if (activeFileControllersRef.current.get(uid) === controller) {
      activeFileControllersRef.current.delete(uid);
    }
  }, []);

  const cancelFile = useCallback((uid: string) => {
    activeFileControllersRef.current.get(uid)?.abort();
  }, []);

  const cancelAll = useCallback(() => {
    globalCancelRequestedRef.current = true;
    activeControllersRef.current.forEach((controller) => controller.abort());
  }, []);

  const storeFile = useCallback(
    async (
      item: UploadDocumentalFileItem,
      signal: AbortSignal | undefined,
      progressContext?: AppProgressBatchItemContext,
      storedSource: "single" | "batch" = progressContext ? "batch" : "single",
      remainingFilesAfterStore?: number,
    ): Promise<AlmacenarDocumentoStoredResult> => {
      const validationError = validateFileForStore(item.uid);
      if (validationError) {
        markFile(item.uid, {
          state: "error",
          error: undefined,
          metadata: { error: validationError },
        });
        throw new UploadDocumentalValidationError(validationError);
      }

      const runOperationId = operationId;
      const requestId = createStorageRequestId("documental");
      const metadata = item.metadata ?? {};

      markFile(item.uid, { state: "uploading", progress: 0, phaseLabel: "Inicializando" });

      const result = await uploadAndStoreOneDocument({
        fileUid: item.uid,
        file: item.file,
        initialChunkSizeBytes: config?.preferredChunkSizeBytes,
        maxChunkSizeBytes: storageOptions?.maxChunkSizeBytes,
        backendPayloadCase: storageOptions?.backendPayloadCase,
        validateStatusBeforeComplete: storageOptions?.validateStatusBeforeComplete,
        signal,
        request: (buildStoreRequestOverride ?? buildStoreRequest)({
          context,
          metadata,
          fileName: item.name,
          requestId,
        }),
        onProgress: (progress) => {
          applyProgress(item.uid, progress, markFile, progressContext);
        },
      });

      if (runOperationId !== currentOperationIdRef.current) {
        throw new Error("Resultado obsoleto ignorado.");
      }

      const storedWithoutEvents: AlmacenarDocumentoStoredResult = {
        ...result.response,
        fileUid: item.uid,
        fileName: item.name,
        metadata,
        rawBackendResult: result.rawBackendResult,
      };
      const interfaceRegistration = buildUploadDocumentalInterfaceRegistration({
        stored: storedWithoutEvents,
        rawBackendResult: result.rawBackendResult,
        context,
        metadata,
        proceso,
        modoDocumento,
      });
      const stored: AlmacenarDocumentoStoredResult = {
        ...storedWithoutEvents,
        interfaceRegistration: interfaceRegistration.length > 0 ? interfaceRegistration : undefined,
      };

      markFile(item.uid, {
        state: "storing",
        progress: 99,
        phaseLabel: "Actualizando documentos",
        error: undefined,
      });
      progressContext?.setPhase("Actualizando documentos");
      progressContext?.setItemProgress(99);

      storedResultsRef.current = [...storedResultsRef.current, stored];
      setStoredResults(storedResultsRef.current);
      onStored?.(stored, {
        source: storedSource,
        remainingFiles: remainingFilesAfterStore ?? countRemainingFilesAfterStore(files, item.uid),
      });
      if (interfaceRegistration.length > 0) {
        onInterfaceRegistration?.(interfaceRegistration);
      }

      markFile(item.uid, { state: "done", progress: 100, phaseLabel: "Guardado", error: undefined });
      progressContext?.setPhase("Guardado");
      progressContext?.setItemProgress(100);

      return stored;
    },
    [
      config?.preferredChunkSizeBytes,
      context,
      buildStoreRequestOverride,
      markFile,
      modoDocumento,
      onInterfaceRegistration,
      onStored,
      operationId,
      proceso,
      storageOptions?.backendPayloadCase,
      storageOptions?.maxChunkSizeBytes,
      storageOptions?.validateStatusBeforeComplete,
      validateFileForStore,
      files,
    ],
  );

  const removeStoredFilesFromQueue = useCallback(
    (results: AlmacenarDocumentoStoredResult[]) => {
      if (results.length === 0) {
        return;
      }

      const storedUids = new Set(results.map((result) => result.fileUid));
      setFiles((current) => current.filter((item) => !storedUids.has(item.uid)));
    },
    [setFiles],
  );

  const saveOne = useCallback(
    async (uid: string) => {
      if (activeFileControllersRef.current.has(uid)) {
        return;
      }

      const item = files.find((file) => file.uid === uid);
      if (!item || isStoreActiveFileState(item.state) || item.state === "done" || item.state === "removed") {
        return;
      }

      const controller = new AbortController();
      registerController(item.uid, controller);
      try {
        const stored = await storeFile(item, controller.signal);
        removeStoredFilesFromQueue([stored]);
      } catch (error) {
        if (shouldSuppressTypologyBackendMessage(error, item)) {
          markFile(uid, {
            state: "error",
            error: undefined,
            metadata: { error: "missing_typology" },
          });
          return;
        }

        const message = getFunctionalSaveErrorMessage(error, controller.signal.aborted);
        markFile(uid, {
          state: controller.signal.aborted ? "cancelled" : "error",
          error: isUploadDocumentalValidationError(error) ? undefined : message,
          metadata: { error: message },
        });
        if (shouldNotifyGlobalError(error)) {
          onError?.(error);
        }
      } finally {
        unregisterController(item.uid, controller);
      }
    },
    [files, markFile, onError, registerController, removeStoredFilesFromQueue, storeFile, unregisterController],
  );

  const saveAll = useCallback(async () => {
    const runnable = files.filter(
      (item) => item.state === "ready" || item.state === "warning" || item.state === "cancelled",
    );
    const validationCandidates = files.filter(
      (item) =>
        item.state === "ready" ||
        item.state === "warning" ||
        item.state === "cancelled" ||
        item.state === "error",
    );
    const invalidItems = validationCandidates
      .map((item) => ({
        item,
        validationError: validateFileForStore(item.uid),
      }))
      .filter((entry): entry is { item: UploadDocumentalFileItem; validationError: string } =>
        Boolean(entry.validationError),
      );

    invalidItems.forEach(({ item, validationError }) => {
      markFile(item.uid, {
        state: "error",
        error: undefined,
        metadata: { error: validationError },
      });
    });

    const invalidUids = new Set(invalidItems.map(({ item }) => item.uid));
    const validItems = runnable.filter((item) => !invalidUids.has(item.uid));

    if (validItems.length === 0) {
      preValidationFailedCountRef.current = invalidItems.length;
      return;
    }

    preValidationFailedCountRef.current = invalidItems.length;

    if (saveAllMode === "inline") {
      setIsSavingAll(true);
      globalCancelRequestedRef.current = false;
      setStoredResults([]);
      storedResultsRef.current = [];

      let failed = invalidItems.length;
      let skipped = 0;
      let cancelled = 0;
      let processed = 0;

      try {
        for (const item of validItems) {
          if (globalCancelRequestedRef.current) {
            skipped += validItems.length - processed;
            break;
          }

          const controller = new AbortController();
          registerController(item.uid, controller);

          try {
            await storeFile(
              item,
              controller.signal,
              undefined,
              "batch",
              countRemainingFilesAfterStore(files, item.uid),
            );
          } catch (error) {
            const aborted = controller.signal.aborted;
            const message = getFunctionalSaveErrorMessage(error, aborted);
            markFile(item.uid, {
              state: aborted ? "cancelled" : "error",
              error: isUploadDocumentalValidationError(error) ? undefined : message,
              metadata: { error: message },
            });

            if (aborted) {
              skipped += 1;
              cancelled += 1;
              if (globalCancelRequestedRef.current) {
                skipped += validItems.length - processed - 1;
                break;
              }
            } else {
              failed += 1;
            }

            if (shouldNotifyGlobalError(error)) {
              onError?.(error);
            }
          } finally {
            processed += 1;
            unregisterController(item.uid, controller);
          }
        }
      } finally {
        const results = storedResultsRef.current;
        const remainingFiles = countRemainingFilesAfterBatch(files, results);
        removeStoredFilesFromQueue(results);
        preValidationFailedCountRef.current = 0;
        const wasCancelled = globalCancelRequestedRef.current;
        globalCancelRequestedRef.current = false;
        setIsSavingAll(false);
        onBatchComplete?.({
          total: validItems.length + invalidItems.length,
          stored: results.length,
          failed,
          skipped,
          cancelled: wasCancelled ? Math.max(cancelled, 1) : cancelled,
          remainingFiles,
          results,
        });
        return;
      }
    }

    setStoredResults([]);
    storedResultsRef.current = [];
    setBatchItems(validItems);
    setBatchOpen(true);
  }, [
    files,
    markFile,
    onBatchComplete,
    onError,
    registerController,
    removeStoredFilesFromQueue,
    saveAllMode,
    storeFile,
    unregisterController,
    validateFileForStore,
  ]);

  const processBatchItem = useCallback(
    async (
      item: UploadDocumentalFileItem,
      batchContext: AppProgressBatchItemContext,
    ): Promise<AppProgressBatchItemResult> => {
      try {
        batchContext.setCurrentLabel(item.name);
        await storeFile(item, batchContext.signal, batchContext);
        return { status: "success" };
      } catch (error) {
        const aborted = batchContext.signal.aborted;
        if (shouldSuppressTypologyBackendMessage(error, item)) {
          markFile(item.uid, {
            state: "error",
            error: undefined,
            metadata: { error: "missing_typology" },
          });
          return { status: "controlled-error", canContinue: true };
        }

        const message = getFunctionalSaveErrorMessage(error, aborted);
        markFile(item.uid, {
          state: aborted ? "cancelled" : "error",
          error: isUploadDocumentalValidationError(error) ? undefined : message,
          metadata: { error: message },
        });
        if (shouldNotifyGlobalError(error)) {
          onError?.(error);
        }
        return { status: aborted ? "skipped" : "controlled-error", message, canContinue: true };
      }
    },
    [markFile, onError, storeFile],
  );

  const handleBatchComplete = useCallback(
    (summary: AppProgressBatchSummary) => {
      const preValidationFailed = preValidationFailedCountRef.current;
      const failed = summary.controlledErrors + summary.fatalErrors + preValidationFailed;
      const results = storedResultsRef.current;
      const remainingFiles = countRemainingFilesAfterBatch(files, results);
      removeStoredFilesFromQueue(results);
      preValidationFailedCountRef.current = 0;

      const batchSummary: UploadDocumentalBatchSummary = {
        total: summary.total + preValidationFailed,
        stored: summary.success,
        failed,
        skipped: summary.skipped,
        cancelled: summary.cancelled ? 1 : 0,
        remainingFiles,
        results,
      };
      onBatchComplete?.(batchSummary);
    },
    [files, onBatchComplete, removeStoredFilesFromQueue],
  );

  return {
    batchOpen,
    batchItems,
    setBatchOpen,
    saveOne,
    saveAll,
    processBatchItem,
    handleBatchComplete,
    cancelAll,
    cancelFile,
    canSaveAll,
    isSavingAll,
  };
}

function buildStoreRequest(input: {
  context: AppUploadDocumentalProps["context"];
  metadata: UploadDocumentalFileMetadata;
  fileName: string;
  requestId: string;
}): Omit<AlmacenarDocumentoRequest, "rutaTemporalId" | "documentos"> & {
  documento?: AlmacenarDocumentoRequest["documentos"][number];
} {
  const camposIndexacion = [
    ...(input.context.camposIndexacion?.map((field) => ({
      nombreCampo: field.nombreCampo,
      valor: field.valor ?? null,
      esObligatorio: field.esObligatorio ?? null,
    })) ?? []),
    ...(input.metadata.fechaCarga
      ? [
          {
            nombreCampo: "fechaCarga",
            valor: input.metadata.fechaCarga,
            esObligatorio: true,
          },
        ]
      : []),
  ];

  return {
    nombreGabinete: input.context.nombreGabinete,
    nombreDocumento: input.fileName,
    requestId: input.requestId,
    camposIndexacion: camposIndexacion.length > 0 ? camposIndexacion : null,
    trd:
      input.metadata.idTipoDocumento || input.metadata.nombreTipoDocumento
        ? {
            idTipoDocumento: input.metadata.idTipoDocumento ?? null,
            nombreTipoDocumento: input.metadata.nombreTipoDocumento ?? null,
          }
        : null,
    expediente:
      input.context.idExpediente || input.context.idTipoExpediente
        ? {
            idExpediente: input.context.idExpediente ?? null,
            idTipoExpediente: input.context.idTipoExpediente ?? null,
          }
        : null,
    workflow:
      input.context.idTareaWorkflow || input.context.idRutaWorkflow
        ? {
            idTareaWorkflow: input.context.idTareaWorkflow ?? null,
            idRutaWorkflow: input.context.idRutaWorkflow ?? null,
          }
        : null,
    numeroPaginasDeclaradas: input.metadata.numeroPaginas ?? null,
    documento: {
      archivoTemporalId: "pending",
      nombreOriginal: input.fileName,
      extension: normalizeFileExtension(input.fileName),
      numeroPaginas: input.metadata.numeroPaginas ?? null,
    },
  };
}

function applyProgress(
  uid: string,
  progress: UploadStorageProgress,
  markFile: UseAppUploadDocumentalStateResult["markFile"],
  context?: AppProgressBatchItemContext,
): void {
  const phaseLabel = getPhaseLabel(progress);
  const visualState =
    progress.phase === "initializing" ? "uploading" : progress.phase === "uploading" ? "uploading" : progress.phase;

  markFile(uid, {
    state: visualState,
    progress: mapStorageProgressPercent(progress),
    phaseLabel,
  });
  context?.setPhase(phaseLabel);
  context?.setItemProgress(mapStorageProgressPercent(progress));
}

function mapStorageProgressPercent(progress: UploadStorageProgress): number {
  const normalized = Math.min(100, Math.max(0, progress.percent));

  if (progress.phase === "initializing") {
    return normalized > 0 ? Math.min(5, normalized) : 2;
  }

  if (progress.phase === "uploading") {
    return 5 + normalized * 0.77;
  }

  if (progress.phase === "completing") {
    return 84 + normalized * 0.08;
  }

  return 94 + normalized * 0.04;
}

function getPhaseLabel(progress: UploadStorageProgress): string {
  if (progress.phase === "uploading" && progress.totalChunks !== undefined && progress.chunkIndex !== undefined) {
    return `Subiendo chunk ${progress.chunkIndex + 1} de ${progress.totalChunks}`;
  }

  const labels: Record<UploadStorageProgress["phase"], string> = {
    initializing: "Inicializando carga",
    uploading: "Subiendo archivo",
    completing: "Completando temporal",
    storing: "Registrando documento",
  };

  return labels[progress.phase];
}

function getFunctionalSaveErrorMessage(error: unknown, aborted: boolean): string {
  if (aborted || readStorageErrorCode(error) === "storage_aborted") {
    return "Carga cancelada. El archivo no fue almacenado.";
  }

  const code = readStorageErrorCode(error);
  if (code) {
    if (code === "storage_store_error" && error instanceof Error && error.message.trim()) {
      return error.message;
    }

    const messages: Record<AlmacenamientoDocumentalUploadErrorCode, string> = {
      storage_aborted: "Carga cancelada. El archivo no fue almacenado.",
      storage_contract_error:
        "No se puede confirmar el guardado porque la respuesta del servidor no tiene el formato esperado.",
      storage_init_error: "No fue posible iniciar la carga temporal del archivo. Intenta nuevamente.",
      storage_chunk_error: "No fue posible subir el archivo completo. Intenta guardar nuevamente.",
      storage_status_error: "No fue posible validar los chunks cargados. Intenta guardar nuevamente.",
      storage_complete_error: "No fue posible completar la carga temporal del archivo. Intenta nuevamente.",
      storage_cancel_error: "No fue posible cancelar la carga temporal en el servidor.",
      storage_store_error:
        "El archivo se cargo, pero no fue posible registrar el documento. Revisa los datos e intenta nuevamente.",
    };

    return messages[code];
  }

  if (error instanceof Error && error.message.trim()) {
    return error.message;
  }

  return "No fue posible guardar el archivo. Intenta nuevamente.";
}

function readStorageErrorCode(error: unknown): AlmacenamientoDocumentalUploadErrorCode | undefined {
  if (!error || typeof error !== "object") {
    return undefined;
  }

  const code = (error as { code?: unknown }).code;
  const validCodes: AlmacenamientoDocumentalUploadErrorCode[] = [
    "storage_contract_error",
    "storage_init_error",
    "storage_chunk_error",
    "storage_status_error",
    "storage_complete_error",
    "storage_cancel_error",
    "storage_store_error",
    "storage_aborted",
  ];

  return validCodes.find((value) => value === code);
}

function shouldSuppressTypologyBackendMessage(error: unknown, item: UploadDocumentalFileItem): boolean {
  if (item.metadata?.idTipoDocumento) {
    return false;
  }

  if (readStorageErrorCode(error) !== "storage_store_error") {
    return false;
  }

  const searchable = normalizeForErrorSearch([
    error instanceof Error ? error.message : undefined,
    readErrorDetails(error),
  ]);

  return (
    searchable.includes("tipologia") ||
    searchable.includes("tipologias") ||
    searchable.includes("tipo documental") ||
    searchable.includes("tipo documento") ||
    searchable.includes("trd")
  );
}

function shouldNotifyGlobalError(error: unknown): boolean {
  return !isUploadDocumentalValidationError(error) && !isStorageAbortError(error);
}

function isStorageAbortError(error: unknown): boolean {
  if (readStorageErrorCode(error) === "storage_aborted") {
    return true;
  }

  return Boolean(error && typeof error === "object" && (error as { name?: unknown }).name === "AbortError");
}

function countRemainingFilesAfterStore(files: UploadDocumentalFileItem[], storedUid: string): number {
  return files.filter((item) => item.uid !== storedUid && item.state !== "done" && item.state !== "removed").length;
}

function countRemainingFilesAfterBatch(
  files: UploadDocumentalFileItem[],
  results: AlmacenarDocumentoStoredResult[],
): number {
  const storedUids = new Set(results.map((result) => result.fileUid));
  return files.filter(
    (item) => !storedUids.has(item.uid) && item.state !== "done" && item.state !== "removed",
  ).length;
}

function isStoreActiveFileState(state: UploadDocumentalFileItem["state"]): boolean {
  return state === "uploading" || state === "completing" || state === "storing" || state === "validating";
}

function readErrorDetails(error: unknown): unknown {
  if (!error || typeof error !== "object") {
    return undefined;
  }

  return (error as { details?: unknown }).details;
}

function normalizeForErrorSearch(value: unknown): string {
  try {
    return JSON.stringify(value ?? "")
      .normalize("NFD")
      .replace(/[\u0300-\u036f]/g, "")
      .toLowerCase();
  } catch {
    return "";
  }
}

class UploadDocumentalValidationError extends Error {
  public constructor(message: string) {
    super(message);
    this.name = "UploadDocumentalValidationError";
  }
}

function isUploadDocumentalValidationError(error: unknown): error is UploadDocumentalValidationError {
  return error instanceof Error && error.name === "UploadDocumentalValidationError";
}
