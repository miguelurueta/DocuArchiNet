import { useCallback, useEffect, useRef, useState } from "react";
import {
  ConfiguracionUploadCorrespondenciaError,
  getConfiguracionUploadCorrespondencia,
} from "../services/configuracionUploadCorrespondencia.service";
import type { ConfiguracionUploadCorrespondencia } from "../types/configuracionUploadCorrespondencia.types";

export type UseConfiguracionUploadCorrespondenciaInput = {
  enabled?: boolean;
};

export type UseConfiguracionUploadCorrespondenciaResult = {
  config?: ConfiguracionUploadCorrespondencia;
  loading: boolean;
  error?: string;
  empty: boolean;
  reload: () => Promise<void>;
};

const isAbortError = (error: unknown): boolean =>
  (error instanceof DOMException && error.name === "AbortError") ||
  (isRecord(error) && error.code === "ERR_CANCELED");

export function useConfiguracionUploadCorrespondencia({
  enabled = true,
}: UseConfiguracionUploadCorrespondenciaInput = {}): UseConfiguracionUploadCorrespondenciaResult {
  const [config, setConfig] = useState<ConfiguracionUploadCorrespondencia | undefined>();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | undefined>();
  const [empty, setEmpty] = useState(false);
  const requestSeqRef = useRef(0);
  const abortRef = useRef<AbortController | null>(null);

  const load = useCallback(async (): Promise<void> => {
    if (!enabled) {
      abortRef.current?.abort();
      abortRef.current = null;
      setConfig(undefined);
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
    setEmpty(false);

    try {
      const loadedConfig = await getConfiguracionUploadCorrespondencia({
        signal: controller.signal,
      });

      if (controller.signal.aborted || requestSeq !== requestSeqRef.current) return;

      setConfig(loadedConfig);
      setError(undefined);
      setEmpty(false);
    } catch (caughtError) {
      if (isAbortError(caughtError) || requestSeq !== requestSeqRef.current) return;

      setConfig(undefined);
      setError(readErrorMessage(caughtError));
      setEmpty(isEmptyConfigurationError(caughtError));
    } finally {
      if (!controller.signal.aborted && requestSeq === requestSeqRef.current) {
        setLoading(false);
      }
    }
  }, [enabled]);

  useEffect(() => {
    void load();

    return () => {
      abortRef.current?.abort();
      requestSeqRef.current += 1;
    };
  }, [load]);

  return {
    config,
    loading,
    error,
    empty,
    reload: load,
  };
}

function readErrorMessage(error: unknown): string {
  if (error instanceof ConfiguracionUploadCorrespondenciaError && error.message.trim()) {
    return error.message;
  }

  if (error instanceof Error && error.message.trim()) {
    return error.message;
  }

  return "No fue posible cargar la configuracion de adjuntos para CORRESPO.";
}

function isEmptyConfigurationError(error: unknown): boolean {
  return (
    error instanceof ConfiguracionUploadCorrespondenciaError &&
    error.message.includes("No hay configuracion de adjuntos")
  );
}

function isRecord(value: unknown): value is Record<string, unknown> {
  return !!value && typeof value === "object" && !Array.isArray(value);
}

