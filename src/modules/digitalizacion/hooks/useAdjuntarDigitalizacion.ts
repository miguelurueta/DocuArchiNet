import { useCallback } from "react";
import {
  adjuntarDigitalizacion,
  validarAdjuntarDigitalizacion,
} from "../services/adjuntarDigitalizacion.api";
import type {
  AdjuntarDigitalizacionPdfRequest,
  AdjuntarDigitalizacionPdfResponse,
  AdjuntarDigitalizacionValidacionQuery,
  AdjuntarDigitalizacionValidacionResponse,
} from "../types/digitalizacionApi.types";
import { useDigitalizacionApiOperation } from "./useDigitalizacionApiOperation";

type ValidarInput = {
  idDocumento: number;
  query: AdjuntarDigitalizacionValidacionQuery;
};

type AdjuntarInput = {
  idDocumento: number;
  request: AdjuntarDigitalizacionPdfRequest;
};

export function useAdjuntarDigitalizacion() {
  const validateOperation = useCallback(
    (input: ValidarInput, options: { signal?: AbortSignal }) =>
      validarAdjuntarDigitalizacion(input.idDocumento, input.query, options),
    [],
  );
  const attachOperation = useCallback(
    (input: AdjuntarInput, options: { signal?: AbortSignal }) =>
      adjuntarDigitalizacion(input.idDocumento, input.request, options),
    [],
  );
  const validation = useDigitalizacionApiOperation<
    ValidarInput,
    AdjuntarDigitalizacionValidacionResponse
  >({
    operation: validateOperation,
    concurrentErrorCode: "ATTACH_VALIDATION_ALREADY_IN_PROGRESS",
  });
  const attach = useDigitalizacionApiOperation<AdjuntarInput, AdjuntarDigitalizacionPdfResponse>({
    operation: attachOperation,
    concurrentErrorCode: "ATTACH_ALREADY_IN_PROGRESS",
  });

  return {
    validation,
    attach,
    validar: validation.run,
    adjuntar: attach.run,
    cancel: () => {
      validation.cancel();
      attach.cancel();
    },
  };
}
