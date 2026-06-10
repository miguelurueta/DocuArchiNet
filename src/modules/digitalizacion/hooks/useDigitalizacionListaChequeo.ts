import { useCallback } from "react";
import { getDigitalizacionListaChequeo } from "../services/digitalizacionListaChequeo.api";
import type {
  DigitalizacionListaChequeoQuery,
  DigitalizacionListaChequeoResponse,
} from "../types/digitalizacionApi.types";
import { useDigitalizacionApiOperation } from "./useDigitalizacionApiOperation";

export function useDigitalizacionListaChequeo() {
  const operation = useCallback(
    (query: DigitalizacionListaChequeoQuery, options: { signal?: AbortSignal }) =>
      getDigitalizacionListaChequeo(query, options),
    [],
  );
  const state = useDigitalizacionApiOperation<
    DigitalizacionListaChequeoQuery,
    DigitalizacionListaChequeoResponse
  >({
    operation,
    concurrentErrorCode: "LISTA_CHEQUEO_ALREADY_IN_PROGRESS",
  });

  return { ...state, load: state.run };
}
