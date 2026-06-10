import { useCallback, useEffect, useRef, useState } from "react";
import { uploadPdfTemporal } from "../services/digitalizacionUploadTemporal.api";
import {
  createDigitalizacionApiError,
  toDigitalizacionApiError,
} from "../services/digitalizacionApiClient";
import type {
  DigitalizacionApiError,
  UploadTemporalPdfProgress,
  UploadTemporalReferencia,
} from "../types/digitalizacionApi.types";

type UploadTemporalPdfState = {
  loading: boolean;
  data: UploadTemporalReferencia | null;
  error: DigitalizacionApiError | null;
  progress: UploadTemporalPdfProgress | null;
};

export function useUploadTemporalPdf() {
  const generationRef = useRef(0);
  const loadingRef = useRef(false);
  const controllerRef = useRef<AbortController | null>(null);
  const [state, setState] = useState<UploadTemporalPdfState>({
    loading: false,
    data: null,
    error: null,
    progress: null,
  });

  const cancel = useCallback(() => {
    generationRef.current += 1;
    controllerRef.current?.abort();
    controllerRef.current = null;
    loadingRef.current = false;
    setState((current) => ({ ...current, loading: false }));
  }, []);

  const upload = useCallback(
    async (file: File, options: { chunkSizeBytes?: number; requestId?: string } = {}) => {
      if (loadingRef.current) {
        const error = createDigitalizacionApiError(
          "UPLOAD_ALREADY_IN_PROGRESS",
          "Ya existe un upload activo.",
          "validation",
        );
        setState((current) => ({ ...current, error: error.detail }));
        throw error;
      }

      const generation = generationRef.current + 1;
      generationRef.current = generation;
      const controller = new AbortController();
      controllerRef.current = controller;
      loadingRef.current = true;
      setState({ loading: true, data: null, error: null, progress: null });

      try {
        const reference = await uploadPdfTemporal(file, {
          ...options,
          signal: controller.signal,
          onProgress: (progress) => {
            if (generation === generationRef.current && !controller.signal.aborted) {
              setState((current) => ({ ...current, progress }));
            }
          },
        });
        if (generation === generationRef.current && !controller.signal.aborted) {
          setState((current) => ({ ...current, loading: false, data: reference, error: null }));
        }
        return reference;
      } catch (error) {
        const apiError = toDigitalizacionApiError(error);
        if (generation === generationRef.current) {
          setState((current) => ({ ...current, loading: false, error: apiError }));
        }
        throw error;
      } finally {
        if (generation === generationRef.current) {
          loadingRef.current = false;
          controllerRef.current = null;
        }
      }
    },
    [],
  );

  useEffect(() => cancel, [cancel]);

  return {
    ...state,
    upload,
    cancel,
    reset: useCallback(() => {
      setState({ loading: false, data: null, error: null, progress: null });
    }, []),
  };
}
