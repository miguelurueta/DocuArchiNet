import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type { Dispatch, SetStateAction } from "react";
import type {
  AppUploadBatchFileItem,
  AppUploadBatchSummary,
} from "../../../../../app/Components/UI/AppUploadBatchView";
import type {
  AppUploadDocumentalProps,
  TipoDocumentalOption,
  UploadDocumentalConfig,
  UploadDocumentalFileMetadata,
} from "../AppUploadDocumental.types";
import { normalizeUploadDocumentalConfig } from "../../../services/uploadConfig.service";
import { normalizeTipoDocumentalOptions } from "../../../services/tipoDocumental.service";
import { normalizeFileExtension } from "../../../utils/storageFile.utils";
import {
  applyTipoDocumentalSuggestion,
  prepareTipoDocumentalOptions,
  suggestTipoDocumentalFromPrepared,
} from "../../../utils/tipoDocumentalSuggestion.utils";

export type UploadDocumentalFileItem = AppUploadBatchFileItem<UploadDocumentalFileMetadata>;

export type UseAppUploadDocumentalStateResult = {
  files: UploadDocumentalFileItem[];
  selectedUid?: string;
  config?: UploadDocumentalConfig;
  tiposDocumentales: TipoDocumentalOption[];
  loading: boolean;
  loaderError: string | null;
  selectionDisabled: boolean;
  summary: AppUploadBatchSummary;
  operationId: number;
  setFiles: Dispatch<SetStateAction<UploadDocumentalFileItem[]>>;
  setSelectedUid: Dispatch<SetStateAction<string | undefined>>;
  handleFilesSelected: (selectedFiles: File[]) => void;
  updateMetadata: (uid: string, metadata: Partial<UploadDocumentalFileMetadata>, manual?: boolean) => void;
  removeFile: (uid: string) => void;
  clearFiles: () => void;
  validateFileForStore: (uid: string) => string | null;
  markFile: (
    uid: string,
    patch: Partial<Omit<UploadDocumentalFileItem, "uid" | "file" | "name" | "size" | "extension">>,
  ) => void;
};

const EMPTY_SUMMARY: AppUploadBatchSummary = {
  total: 0,
  queued: 0,
  ready: 0,
  uploading: 0,
  done: 0,
  warning: 0,
  error: 0,
  cancelled: 0,
};

export function useAppUploadDocumentalState({
  proceso,
  context,
  modoDocumento,
  loadConfig,
  loadTiposDocumentales,
  tipologiaObligatoria,
  autoSuggestTipologia = true,
  requiereFechaCarga,
  fechaCargaObligatoria,
  validationMode,
  onError,
}: AppUploadDocumentalProps): UseAppUploadDocumentalStateResult {
  const [files, setFiles] = useState<UploadDocumentalFileItem[]>([]);
  const [selectedUid, setSelectedUid] = useState<string | undefined>();
  const [config, setConfig] = useState<UploadDocumentalConfig>();
  const [tiposDocumentales, setTiposDocumentales] = useState<TipoDocumentalOption[]>([]);
  const [loading, setLoading] = useState(false);
  const [loaderError, setLoaderError] = useState<string | null>(null);
  const operationIdRef = useRef(0);

  const contextKey = useMemo(() => JSON.stringify(context), [context]);

  useEffect(() => {
    const operationId = operationIdRef.current + 1;
    operationIdRef.current = operationId;
    let cancelled = false;

    async function load() {
      if (!context.nombreGabinete.trim()) {
        setConfig(undefined);
        setTiposDocumentales([]);
        setLoaderError("El nombre de gabinete es obligatorio.");
        return;
      }

      setLoading(true);
      setLoaderError(null);

      try {
        const [loadedConfig, loadedTipos] = await Promise.all([
          loadConfig({ proceso, context, modoDocumento }),
          loadTiposDocumentales({ proceso, context }),
        ]);

        if (cancelled || operationIdRef.current !== operationId) {
          return;
        }

        setConfig(normalizeUploadDocumentalConfig(loadedConfig));
        setTiposDocumentales(normalizeTipoDocumentalOptions(loadedTipos));
      } catch (error) {
        if (cancelled || operationIdRef.current !== operationId) {
          return;
        }

        setConfig(undefined);
        setTiposDocumentales([]);
        setLoaderError("No fue posible cargar la configuracion documental.");
        onError?.(error);
      } finally {
        if (!cancelled && operationIdRef.current === operationId) {
          setLoading(false);
        }
      }
    }

    void load();

    return () => {
      cancelled = true;
      operationIdRef.current += 1;
    };
  }, [context, contextKey, loadConfig, loadTiposDocumentales, modoDocumento, onError, proceso]);

  const effectiveValidationMode = validationMode ?? config?.validationMode ?? "reject";
  const requiresTypology = Boolean(tipologiaObligatoria ?? config?.requiereTipologia);
  const requiresDate = Boolean(requiereFechaCarga ?? config?.requiereFechaCarga);
  const requiresDateValue = Boolean(fechaCargaObligatoria ?? config?.fechaCargaObligatoria ?? requiresDate);
  const preparedTipoDocumentalOptions = useMemo(
    () => prepareTipoDocumentalOptions(tiposDocumentales),
    [tiposDocumentales],
  );

  const selectionDisabled = loading || Boolean(loaderError) || !config || !context.nombreGabinete.trim();

  const summary = useMemo(
    () =>
      files.reduce<AppUploadBatchSummary>((current, item) => {
        const next = { ...current, total: current.total + 1 };
        if (item.state === "ready") next.ready += 1;
        if (item.state === "done") next.done += 1;
        if (item.state === "warning") next.warning += 1;
        if (item.state === "error") next.error += 1;
        if (item.state === "cancelled") next.cancelled += 1;
        if (item.state === "queued" || item.state === "validating") next.queued += 1;
        if (item.state === "uploading" || item.state === "completing" || item.state === "storing") {
          next.uploading += 1;
        }
        return next;
      }, EMPTY_SUMMARY),
    [files],
  );

  const validateSelectedFile = useCallback(
    (file: File): string | null => {
      if (!config) {
        return "La configuracion documental no esta disponible.";
      }

      const extension = normalizeFileExtension(file.name);
      if (!extension || !config.allowedExtensions.includes(extension)) {
        return `No se puede guardar: la extension ${extension || "sin extension"} no esta permitida.`;
      }

      if (file.size <= 0) {
        return "No se puede guardar: el archivo esta vacio.";
      }

      if (file.size > config.maxSizeBytes) {
        return "No se puede guardar: el archivo supera el tamano maximo permitido.";
      }

      return null;
    },
    [config],
  );

  const handleFilesSelected = useCallback(
    (selectedFiles: File[]) => {
      if (!config) {
        return;
      }

      setFiles((current) => {
        const next = config.multiple ? [...current] : [];

        for (const file of selectedFiles) {
          const extension = normalizeFileExtension(file.name);
          const validationError = validateSelectedFile(file);

          if (validationError && effectiveValidationMode === "reject") {
            continue;
          }

          const baseMetadata: UploadDocumentalFileMetadata = validationError
            ? { error: validationError }
            : {};
          const suggestedMetadata = autoSuggestTipologia
            ? applyTipoDocumentalSuggestion(
                baseMetadata,
                suggestTipoDocumentalFromPrepared({
                  fileName: file.name,
                  preparedOptions: preparedTipoDocumentalOptions,
                }),
              )
            : baseMetadata;

          next.push({
            uid: createFileUid(file),
            file,
            name: file.name,
            size: file.size,
            extension,
            state: validationError ? "error" : "ready",
            error: validationError ?? undefined,
            metadata: suggestedMetadata,
          });
        }

        return next;
      });
    },
    [
      autoSuggestTipologia,
      config,
      effectiveValidationMode,
      preparedTipoDocumentalOptions,
      validateSelectedFile,
    ],
  );

  const updateMetadata = useCallback(
    (uid: string, metadata: Partial<UploadDocumentalFileMetadata>, manual = false) => {
      setFiles((current) =>
        current.map((item) =>
          item.uid === uid
            ? {
                ...item,
                metadata: {
                  ...item.metadata,
                  ...metadata,
                  tipologiaManual: manual ? true : item.metadata?.tipologiaManual,
                  error: undefined,
                },
                error: undefined,
                state: item.state === "error" ? "ready" : item.state,
              }
            : item,
        ),
      );
    },
    [],
  );

  const removeFile = useCallback((uid: string) => {
    setFiles((current) => current.filter((item) => item.uid !== uid));
    setSelectedUid((current) => (current === uid ? undefined : current));
  }, []);

  const clearFiles = useCallback(() => {
    setFiles([]);
    setSelectedUid(undefined);
  }, []);

  const validateFileForStore = useCallback(
    (uid: string): string | null => {
      const item = files.find((file) => file.uid === uid);
      if (!item) {
        return "El archivo no existe en la cola.";
      }

      const fileError = validateSelectedFile(item.file);
      if (fileError) {
        return fileError;
      }

      if (requiresTypology && !item.metadata?.idTipoDocumento) {
        return "No se puede guardar: selecciona la tipologia documental del archivo.";
      }

      const date = item.metadata?.fechaCarga;
      if (requiresDateValue && !date) {
        return "No se puede guardar: ingresa la fecha documental del archivo.";
      }

      if (date && !isValidDate(date)) {
        return "No se puede guardar: la fecha documental debe ser real, no futura y usar formato AAAA-MM-DD.";
      }

      return null;
    },
    [files, requiresDateValue, requiresTypology, validateSelectedFile],
  );

  const markFile = useCallback<UseAppUploadDocumentalStateResult["markFile"]>((uid, patch) => {
    setFiles((current) =>
      current.map((item) =>
        item.uid === uid
          ? {
              ...item,
              ...patch,
              metadata: patch.metadata ? { ...item.metadata, ...patch.metadata } : item.metadata,
            }
          : item,
      ),
    );
  }, []);

  return {
    files,
    selectedUid,
    config,
    tiposDocumentales,
    loading,
    loaderError,
    selectionDisabled,
    summary,
    operationId: operationIdRef.current,
    setFiles,
    setSelectedUid,
    handleFilesSelected,
    updateMetadata,
    removeFile,
    clearFiles,
    validateFileForStore,
    markFile,
  };
}

function createFileUid(file: File): string {
  return `${file.name}-${file.size}-${file.lastModified}-${Math.random().toString(36).slice(2, 10)}`;
}

function isValidDate(value: string): boolean {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
    return false;
  }

  const [year, month, day] = value.split("-").map(Number);
  const date = new Date(Date.UTC(year, month - 1, day));

  return (
    date.getUTCFullYear() === year &&
    date.getUTCMonth() === month - 1 &&
    date.getUTCDate() === day &&
    year <= new Date().getFullYear()
  );
}
