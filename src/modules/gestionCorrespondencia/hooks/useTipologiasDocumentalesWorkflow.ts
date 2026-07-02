import { useCallback, useEffect, useRef, useState } from "react";
import {
  getTipologiasDocumentalesWorkflow,
  TipologiasDocumentalesWorkflowError,
} from "../services/tipologiasDocumentalesWorkflow.service";
import type { TipologiaDocumentalWorkflowOption } from "../types/tipologiasDocumentalesWorkflow.types";

export type UseTipologiasDocumentalesWorkflowInput = {
  idTareaWf?: number;
  idRutaWf?: number;
  enabled?: boolean;
};

export type UseTipologiasDocumentalesWorkflowResult = {
  options: TipologiaDocumentalWorkflowOption[];
  loading: boolean;
  error?: string;
  empty: boolean;
  reload: () => Promise<void>;
};

const isPositiveNumber = (value?: number): value is number =>
  typeof value === "number" && Number.isFinite(value) && value > 0;

const isAbortError = (error: unknown): boolean =>
  (error instanceof DOMException && error.name === "AbortError") ||
  (isRecord(error) && error.code === "ERR_CANCELED");

export function useTipologiasDocumentalesWorkflow({
  idTareaWf,
  idRutaWf,
  enabled = true,
}: UseTipologiasDocumentalesWorkflowInput): UseTipologiasDocumentalesWorkflowResult {
  const [options, setOptions] = useState<TipologiaDocumentalWorkflowOption[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | undefined>();
  const [empty, setEmpty] = useState(false);
  const requestSeqRef = useRef(0);
  const abortRef = useRef<AbortController | null>(null);
  const canLoad = enabled && isPositiveNumber(idTareaWf) && isPositiveNumber(idRutaWf);

  const load = useCallback(async (): Promise<void> => {
    if (!canLoad) {
      abortRef.current?.abort();
      abortRef.current = null;
      setOptions([]);
      setLoading(false);
      setError(undefined);
      setEmpty(false);
      return;
    }

    abortRef.current?.abort();
    const controller = new AbortController();
    abortRef.current = controller;
    const requestSeq = requestSeqRef.current + 1;
    requestSeqRef.current = requestSeq;

    setLoading(true);
    setError(undefined);

    try {
      const loadedOptions = await getTipologiasDocumentalesWorkflow(
        { idTareaWf, idRutaWf },
        { signal: controller.signal },
      );

      if (controller.signal.aborted || requestSeq !== requestSeqRef.current) return;

      setOptions(loadedOptions);
      setEmpty(loadedOptions.length === 0);
      setError(undefined);
    } catch (caughtError) {
      if (isAbortError(caughtError) || requestSeq !== requestSeqRef.current) return;

      setOptions([]);
      setEmpty(false);
      setError(readErrorMessage(caughtError));
    } finally {
      if (!controller.signal.aborted && requestSeq === requestSeqRef.current) {
        setLoading(false);
      }
    }
  }, [canLoad, idRutaWf, idTareaWf]);

  useEffect(() => {
    void load();

    return () => {
      abortRef.current?.abort();
      requestSeqRef.current += 1;
    };
  }, [load]);

  return {
    options,
    loading,
    error,
    empty,
    reload: load,
  };
}

function readErrorMessage(error: unknown): string {
  if (error instanceof TipologiasDocumentalesWorkflowError && error.message.trim()) {
    return error.message;
  }

  if (error instanceof Error && error.message.trim()) {
    return error.message;
  }

  return "No fue posible cargar las tipologias documentales del workflow.";
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return !!value && typeof value === "object" && !Array.isArray(value);
}
