import { useCallback } from "react";
import { getDigitalizacionConfiguracion } from "../services/digitalizacionConfiguracion.api";
import type {
  DigitalizacionConfiguracionQuery,
  DigitalizacionConfiguracionResponse,
} from "../types/digitalizacionApi.types";
import { useDigitalizacionApiOperation } from "./useDigitalizacionApiOperation";

export function useDigitalizacionConfiguracion() {
  const operation = useCallback(
    (query: DigitalizacionConfiguracionQuery, options: { signal?: AbortSignal }) =>
      getDigitalizacionConfiguracion(query, options),
    [],
  );
  const state = useDigitalizacionApiOperation<
    DigitalizacionConfiguracionQuery,
    DigitalizacionConfiguracionResponse
  >({
    operation,
    concurrentErrorCode: "CONFIGURACION_ALREADY_IN_PROGRESS",
  });

  return { ...state, load: state.run };
}
