import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import {
  buildDigitalizacionContextSignature,
  validateDigitalizacionContext,
} from "../services/digitalizacionContract";
import type {
  DigitalizacionContext,
  DigitalizacionFunctionalError,
  DigitalizacionOperationState,
  DigitalizacionScannedPage,
  DigitalizacionState,
} from "../types/digitalizacion.types";

const createInitialScannerState = () => ({
  selectedScannerId: null,
  runtimeAvailable: false,
  pages: [] as DigitalizacionScannedPage[],
  generatedPdf: null,
});

const createInitialState = (
  context: DigitalizacionContext | null,
  generation: number,
): DigitalizacionState => {
  const validationError = validateDigitalizacionContext(context);

  return {
    context: validationError ? null : context,
    validationError,
    scanner: createInitialScannerState(),
    metadata: {
      required: Boolean(context?.requiereMetadata) && !validationError,
      checklistReady: false,
      trd: null,
      errors: [],
    },
    operation: validationError
      ? { status: "error", error: validationError }
      : { status: "idle" },
    generation,
  };
};

export const useDigitalizacionDocumentalState = ({
  open,
  context,
  onInvalidContext,
}: {
  open: boolean;
  context: DigitalizacionContext | null;
  onInvalidContext?: (error: DigitalizacionFunctionalError) => void;
}) => {
  const generationRef = useRef(0);
  const contextSignature = useMemo(
    () => buildDigitalizacionContextSignature(context),
    [context],
  );
  const [state, setState] = useState<DigitalizacionState>(() =>
    createInitialState(context, 0),
  );

  useEffect(() => {
    generationRef.current += 1;
    const nextState = createInitialState(context, generationRef.current);
    setState(nextState);

    if (open && nextState.validationError) {
      onInvalidContext?.(nextState.validationError);
    }
  }, [contextSignature, open, context, onInvalidContext]);

  const clear = useCallback(() => {
    generationRef.current += 1;
    setState(createInitialState(context, generationRef.current));
  }, [context]);

  const setOperation = useCallback((operation: DigitalizacionOperationState) => {
    setState((current) => ({
      ...current,
      operation,
    }));
  }, []);

  const clearPages = useCallback(() => {
    setState((current) => ({
      ...current,
      scanner: createInitialScannerState(),
      metadata: {
        ...current.metadata,
        trd: null,
        errors: [],
      },
      operation: current.validationError
        ? { status: "error", error: current.validationError }
        : { status: "idle" },
    }));
  }, []);

  const isCurrentGeneration = useCallback((generation: number) => {
    return generation === generationRef.current;
  }, []);

  const canSubmit = Boolean(
    open &&
      state.context &&
      !state.validationError &&
      state.scanner.generatedPdf &&
      state.operation.status !== "saving" &&
      state.operation.status !== "uploading",
  );

  return {
    state,
    clear,
    clearPages,
    setOperation,
    canSubmit,
    currentGeneration: state.generation,
    isCurrentGeneration,
  };
};
