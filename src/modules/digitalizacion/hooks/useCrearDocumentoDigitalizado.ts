import { useCallback } from "react";
import { crearDocumentoDigitalizado } from "../services/digitalizacionDocumentos.api";
import type {
  CrearDocumentoDigitalizadoRequest,
  CrearDocumentoDigitalizadoResponse,
} from "../types/digitalizacionApi.types";
import { useDigitalizacionApiOperation } from "./useDigitalizacionApiOperation";

export function useCrearDocumentoDigitalizado() {
  const operation = useCallback(
    (request: CrearDocumentoDigitalizadoRequest, options: { signal?: AbortSignal }) =>
      crearDocumentoDigitalizado(request, options),
    [],
  );
  const state = useDigitalizacionApiOperation<
    CrearDocumentoDigitalizadoRequest,
    CrearDocumentoDigitalizadoResponse
  >({
    operation,
    concurrentErrorCode: "CREATE_ALREADY_IN_PROGRESS",
  });

  return { ...state, create: state.run };
}
