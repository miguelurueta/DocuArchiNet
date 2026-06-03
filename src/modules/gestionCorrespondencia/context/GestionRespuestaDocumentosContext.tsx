import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import type { ReactNode } from "react";
import type { AppUploadFile } from "../../../app/Components/UI/AppUpload/AppUpload";
import { getSolicitaGabinetePorTareaWorkflow } from "../services/solicitaGabineteRadicadoWorkflow.service";

export type GestionRespuestaDocumentosState = {
  idTareaWf?: number;
  radicado?: string;
  idRespuestaRadicado?: string | number;
  nombreGabinete?: string;
  gabineteLoading: boolean;
  gabineteError?: string;
  reloadGabinete: () => Promise<void>;
  files: AppUploadFile[];
  setFiles: (files: AppUploadFile[]) => void;
};

type GestionRespuestaDocumentosProviderProps = {
  idTareaWf?: number;
  radicado?: string;
  idRespuestaRadicado?: string | number;
  children: ReactNode;
};

type GabineteState = {
  nombreGabinete?: string;
  loading: boolean;
  error?: string;
};

export const GestionRespuestaDocumentosContext =
  createContext<GestionRespuestaDocumentosState | null>(null);

const normalizeOptionalString = (value?: string): string | undefined => {
  const normalized = value?.trim();
  return normalized ? normalized : undefined;
};

const isEstadoExistenciaNo = (value: unknown): boolean =>
  typeof value === "string" && value.trim().toUpperCase() === "NO";

const isValidTaskId = (value?: number): value is number =>
  typeof value === "number" && Number.isFinite(value) && value > 0;

const isRecord = (value: unknown): value is Record<string, unknown> =>
  !!value && typeof value === "object";

const readErrorMessage = (error: unknown): string => {
  if (isRecord(error)) {
    const message = typeof error.message === "string" ? error.message : String(error);
    const response = isRecord(error.response) ? error.response : undefined;
    const status = typeof response?.status === "number" ? response.status : undefined;
    return status ? `HTTP ${status}: ${message}` : message;
  }

  return error instanceof Error ? error.message : String(error);
};

const readApiErrorMessage = (error: unknown): string | undefined => {
  if (!error || typeof error !== "object") return undefined;
  const source = error as Record<string, unknown>;
  const value = source.errorMessage ?? source.Message;
  return typeof value === "string" && value.trim().length > 0 ? value.trim() : undefined;
};

const isAbortError = (error: unknown): boolean =>
  (error instanceof DOMException && error.name === "AbortError") ||
  (isRecord(error) && error.code === "ERR_CANCELED");

export function GestionRespuestaDocumentosProvider({
  idTareaWf,
  radicado,
  idRespuestaRadicado,
  children,
}: GestionRespuestaDocumentosProviderProps) {
  const [files, setFiles] = useState<AppUploadFile[]>([]);
  const [gabineteState, setGabineteState] = useState<GabineteState>({
    nombreGabinete: undefined,
    loading: false,
    error: undefined,
  });
  const abortRef = useRef<AbortController | null>(null);
  const requestSeqRef = useRef(0);
  const loadedTaskRef = useRef<number | null>(null);
  const idTareaWfRef = useRef<number | undefined>(idTareaWf);

  useEffect(() => {
    idTareaWfRef.current = idTareaWf;
  }, [idTareaWf]);

  const loadGabinete = useCallback(async (force: boolean): Promise<void> => {
    const currentTaskId = idTareaWfRef.current;

    if (!isValidTaskId(currentTaskId)) {
      abortRef.current?.abort();
      abortRef.current = null;
      loadedTaskRef.current = null;
      setGabineteState({ nombreGabinete: undefined, loading: false, error: undefined });
      return;
    }

    if (!force && loadedTaskRef.current === currentTaskId) {
      return;
    }

    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;
    const requestSeq = requestSeqRef.current + 1;
    requestSeqRef.current = requestSeq;

    setGabineteState((prev) => ({
      nombreGabinete: force ? prev.nombreGabinete : undefined,
      loading: true,
      error: undefined,
    }));

    try {
      const response = await getSolicitaGabinetePorTareaWorkflow(currentTaskId, {
        signal: controller.signal,
      });
      if (requestSeq !== requestSeqRef.current || controller.signal.aborted) return;

      loadedTaskRef.current = currentTaskId;

      if (!response.success) {
        const message =
          readApiErrorMessage(response.errors?.[0]) ??
          response.message ??
          "No fue posible resolver el gabinete del radicado.";

        setGabineteState({
          nombreGabinete: undefined,
          loading: false,
          error: message,
        });
        return;
      }

      if (isEstadoExistenciaNo(response.data?.EstadoExistenciaRadicado)) {
        setGabineteState({
          nombreGabinete: undefined,
          loading: false,
          error: "No fue posible cargar documentos: el radicado no existe para la tarea.",
        });
        return;
      }

      setGabineteState({
        nombreGabinete: normalizeOptionalString(response.data?.NombreGabinete),
        loading: false,
        error: undefined,
      });
    } catch (error) {
      if (isAbortError(error)) return;
      if (requestSeq !== requestSeqRef.current) return;

      loadedTaskRef.current = currentTaskId;
      setGabineteState({
        nombreGabinete: undefined,
        loading: false,
        error: readErrorMessage(error),
      });
    }
  }, []);

  const reloadGabinete = useCallback(() => loadGabinete(true), [loadGabinete]);

  useEffect(() => {
    void loadGabinete(false);

    return () => {
      abortRef.current?.abort();
    };
  }, [idTareaWf, loadGabinete]);

  const value = useMemo<GestionRespuestaDocumentosState>(
    () => ({
      idTareaWf,
      radicado: normalizeOptionalString(radicado),
      idRespuestaRadicado,
      nombreGabinete: gabineteState.nombreGabinete,
      gabineteLoading: gabineteState.loading,
      gabineteError: gabineteState.error,
      reloadGabinete,
      files,
      setFiles,
    }),
    [
      files,
      gabineteState.error,
      gabineteState.loading,
      gabineteState.nombreGabinete,
      idRespuestaRadicado,
      idTareaWf,
      radicado,
      reloadGabinete,
    ],
  );

  return (
    <GestionRespuestaDocumentosContext.Provider value={value}>
      {children}
    </GestionRespuestaDocumentosContext.Provider>
  );
}
