import { useCallback, useEffect, useRef, useState } from "react";
import type {
  DigitalizacionApiError,
  DigitalizacionApiOperationState,
  DigitalizacionApiRequestOptions,
} from "../types/digitalizacionApi.types";
import {
  createDigitalizacionApiError,
  toDigitalizacionApiError,
} from "../services/digitalizacionApiClient";

type Operation<TInput, TData> = (
  input: TInput,
  options: DigitalizacionApiRequestOptions,
) => Promise<TData>;

export function useDigitalizacionApiOperation<TInput, TData>({
  operation,
  concurrentErrorCode,
}: {
  operation: Operation<TInput, TData>;
  concurrentErrorCode: string;
}) {
  const generationRef = useRef(0);
  const loadingRef = useRef(false);
  const controllerRef = useRef<AbortController | null>(null);
  const [state, setState] = useState<DigitalizacionApiOperationState<TData>>({
    loading: false,
    data: null,
    error: null,
  });

  const cancel = useCallback(() => {
    generationRef.current += 1;
    controllerRef.current?.abort();
    controllerRef.current = null;
    loadingRef.current = false;
    setState((current) => ({ ...current, loading: false }));
  }, []);

  const run = useCallback(
    async (input: TInput) => {
      if (loadingRef.current) {
        const concurrentError = createDigitalizacionApiError(
          concurrentErrorCode,
          "Ya existe una operacion activa.",
          "validation",
        );
        setState((current) => ({ ...current, error: concurrentError.detail }));
        throw concurrentError;
      }

      const generation = generationRef.current + 1;
      generationRef.current = generation;
      const controller = new AbortController();
      controllerRef.current = controller;
      loadingRef.current = true;
      setState({ loading: true, data: null, error: null });

      try {
        const data = await operation(input, { signal: controller.signal });
        if (generation === generationRef.current && !controller.signal.aborted) {
          setState({ loading: false, data, error: null });
        }
        return data;
      } catch (error) {
        const apiError = toDigitalizacionApiError(error);
        if (generation === generationRef.current) {
          setState({ loading: false, data: null, error: apiError });
        }
        throw error;
      } finally {
        if (generation === generationRef.current) {
          loadingRef.current = false;
          controllerRef.current = null;
        }
      }
    },
    [concurrentErrorCode, operation],
  );

  useEffect(() => cancel, [cancel]);

  return {
    ...state,
    run,
    cancel,
    reset: useCallback(() => {
      setState({ loading: false, data: null, error: null });
    }, []),
  };
}

export type DigitalizacionApiOperationHook<TInput, TData> = ReturnType<
  typeof useDigitalizacionApiOperation<TInput, TData>
> & {
  error: DigitalizacionApiError | null;
};
