import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type {
  AppProgressBatchItemContext,
  AppProgressBatchItemResult,
  AppProgressBatchSummary,
} from "../../../../../app/Components/UI/AppProgressBatch";
import type {
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
  | "onStored"
  | "onInterfaceRegistration"
  | "onBatchComplete"
  | "onError"
> &
  Pick<
    UseAppUploadDocumentalStateResult,
    "files" | "config" | "operationId" | "validateFileForStore" | "markFile"
  >;

export type UseAppUploadDocumentalActionsResult = {
  batchOpen: boolean;
  batchItems: UploadDocumentalFileItem[];
  setBatchOpen: (open: boolean) => void;
  saveOne: (uid: string) => Promise<void>;
  saveAll: () => void;
  processBatchItem: (
    item: UploadDocumentalFileItem,
    context: AppProgressBatchItemContext,
  ) => Promise<AppProgressBatchItemResult>;
  handleBatchComplete: (summary: AppProgressBatchSummary) => void;
  canSaveAll: boolean;
};

export function useAppUploadDocumentalActions({
  files,
  config,
  context,
  proceso,
  modoDocumento,
  operationId,
  validateFileForStore,
  markFile,
  onStored,
  onInterfaceRegistration,
  onBatchComplete,
  onError,
}: UseAppUploadDocumentalActionsInput): UseAppUploadDocumentalActionsResult {
  const [batchOpen, setBatchOpen] = useState(false);
  const [batchItems, setBatchItems] = useState<UploadDocumentalFileItem[]>([]);
  const [storedResults, setStoredResults] = useState<AlmacenarDocumentoStoredResult[]>([]);
  const currentOperationIdRef = useRef(operationId);
  const activeControllersRef = useRef<Set<AbortController>>(new Set());

  useEffect(() => {
    if (currentOperationIdRef.current !== operationId) {
      currentOperationIdRef.current = operationId;
      activeControllersRef.current.forEach((controller) => controller.abort());
      activeControllersRef.current.clear();
      setBatchOpen(false);
      setBatchItems([]);
      setStoredResults([]);
    }
  }, [operationId]);

  useEffect(
    () => () => {
      activeControllersRef.current.forEach((controller) => controller.abort());
      activeControllersRef.current.clear();
    },
    [],
  );

  const canSaveAll = useMemo(
    () => Boolean(config) && files.some((item) => item.state === "ready" || item.state === "warning"),
    [config, files],
  );

  const storeFile = useCallback(
    async (
      item: UploadDocumentalFileItem,
      signal: AbortSignal | undefined,
      progressContext?: AppProgressBatchItemContext,
    ): Promise<AlmacenarDocumentoStoredResult> => {
      const validationError = validateFileForStore(item.uid);
      if (validationError) {
        markFile(item.uid, {
          state: "error",
          error: validationError,
          metadata: { error: validationError },
        });
        throw new Error(validationError);
      }

      const runOperationId = operationId;
      const requestId = createStorageRequestId("documental");
      const metadata = item.metadata ?? {};

      markFile(item.uid, { state: "uploading", progress: 0, phaseLabel: "Inicializando" });

      const result = await uploadAndStoreOneDocument({
        fileUid: item.uid,
        file: item.file,
        initialChunkSizeBytes: config?.preferredChunkSizeBytes,
        signal,
        request: buildStoreRequest({
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

      markFile(item.uid, { state: "done", progress: 100, phaseLabel: "Guardado", error: undefined });
      setStoredResults((current) => [...current, stored]);
      onStored?.(stored);
      if (interfaceRegistration.length > 0) {
        onInterfaceRegistration?.(interfaceRegistration);
      }

      return stored;
    },
    [
      config?.preferredChunkSizeBytes,
      context,
      markFile,
      modoDocumento,
      onInterfaceRegistration,
      onStored,
      operationId,
      proceso,
      validateFileForStore,
    ],
  );

  const saveOne = useCallback(
    async (uid: string) => {
      const item = files.find((file) => file.uid === uid);
      if (!item) {
        return;
      }

      const controller = new AbortController();
      activeControllersRef.current.add(controller);
      try {
        await storeFile(item, controller.signal);
      } catch (error) {
        const message = error instanceof Error ? error.message : "No fue posible guardar el archivo.";
        markFile(uid, { state: controller.signal.aborted ? "cancelled" : "error", error: message });
        onError?.(error);
      } finally {
        activeControllersRef.current.delete(controller);
      }
    },
    [files, markFile, onError, storeFile],
  );

  const saveAll = useCallback(() => {
    const runnable = files.filter((item) => item.state === "ready" || item.state === "warning");
    setStoredResults([]);
    setBatchItems(runnable);
    setBatchOpen(true);
  }, [files]);

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
        const message = error instanceof Error ? error.message : "No fue posible guardar el archivo.";
        markFile(item.uid, { state: aborted ? "cancelled" : "error", error: message });
        onError?.(error);
        return { status: aborted ? "skipped" : "controlled-error", message, canContinue: true };
      }
    },
    [markFile, onError, storeFile],
  );

  const handleBatchComplete = useCallback(
    (summary: AppProgressBatchSummary) => {
      const failed = summary.controlledErrors + summary.fatalErrors;
      const batchSummary: UploadDocumentalBatchSummary = {
        total: summary.total,
        stored: summary.success,
        failed,
        skipped: summary.skipped,
        cancelled: summary.cancelled ? 1 : 0,
        results: storedResults,
      };
      onBatchComplete?.(batchSummary);
    },
    [onBatchComplete, storedResults],
  );

  return {
    batchOpen,
    batchItems,
    setBatchOpen,
    saveOne,
    saveAll,
    processBatchItem,
    handleBatchComplete,
    canSaveAll,
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
    progress: progress.percent,
    phaseLabel,
  });
  context?.setPhase(phaseLabel);
  context?.setItemProgress(progress.percent);
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
