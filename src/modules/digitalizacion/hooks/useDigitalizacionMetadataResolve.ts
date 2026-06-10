import { useCallback } from "react";
import { resolveDigitalizacionMetadata } from "../services/digitalizacionMetadata.api";
import type {
  DigitalizacionMetadataResolveRequest,
  DigitalizacionMetadataResolveResponse,
} from "../types/digitalizacionApi.types";
import { useDigitalizacionApiOperation } from "./useDigitalizacionApiOperation";

export function useDigitalizacionMetadataResolve() {
  const operation = useCallback(
    (request: DigitalizacionMetadataResolveRequest, options: { signal?: AbortSignal }) =>
      resolveDigitalizacionMetadata(request, options),
    [],
  );
  const state = useDigitalizacionApiOperation<
    DigitalizacionMetadataResolveRequest,
    DigitalizacionMetadataResolveResponse
  >({
    operation,
    concurrentErrorCode: "METADATA_RESOLVE_ALREADY_IN_PROGRESS",
  });

  return { ...state, resolve: state.run };
}
