import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { mapEstructuraRespuesta } from "../adapters/mapEstructuraRespuesta";
import { getSolicitaEstructuraRespuestaIdTarea } from "../services/solicitaEstructuraRespuestaIdTarea.service";
import type { GestionRespuestaEstructuraRespuesta } from "../types/gestionRespuestaEstructura.types";

export type UseEstructuraRespuestaIdTareaResult = {
  estrucTuraRespuesta: GestionRespuestaEstructuraRespuesta | null;
  loading: boolean;
  error: Error | null;
  isEmpty: boolean;
};

type ApiResponseLike = {
  success?: unknown;
  Success?: unknown;
  data?: unknown;
  Data?: unknown;
};

const isTruthySuccess = (value: unknown): boolean =>
  value === true || value === 1 || value === "true" || value === "True";

const normalizeError = (error: unknown): Error | null => {
  if (!error) return null;
  if (axios.isAxiosError(error)) {
    const status = error.response?.status;
    const baseURL = error.config?.baseURL ?? "";
    const url = error.config?.url ?? "";
    const detail = status ? `HTTP ${status}` : error.message;
    const target = baseURL || url ? ` (${baseURL}${url})` : "";
    return new Error(`${detail}${target}`);
  }
  return error instanceof Error ? error : new Error(String(error));
};

export const useEstructuraRespuestaIdTarea = (
  idTareaWf?: number,
): UseEstructuraRespuestaIdTareaResult => {
  const query = useQuery({
    queryKey: ["gestion-correspondencia", "estructura-respuesta", idTareaWf],
    enabled: typeof idTareaWf === "number" && Number.isFinite(idTareaWf) && idTareaWf > 0,
    retry: false,
    queryFn: async () => getSolicitaEstructuraRespuestaIdTarea(idTareaWf as number),
  });

  const apiResponse = query.data as unknown as ApiResponseLike | undefined;
  const rawSuccess = apiResponse?.success ?? apiResponse?.Success;
  const hasSuccess = isTruthySuccess(rawSuccess);
  const rawData = apiResponse?.data ?? apiResponse?.Data;
  const payload = Array.isArray(rawData)
    ? rawData
    : rawData && typeof rawData === "object"
      ? [rawData]
      : [];
  const isEmpty = hasSuccess && payload.length === 0;
  const estrucTuraRespuesta =
    hasSuccess && payload.length > 0 ? mapEstructuraRespuesta(payload[0] as any) : null;

  return {
    estrucTuraRespuesta,
    loading: query.isLoading || query.isFetching,
    error: normalizeError(query.error),
    isEmpty,
  };
};
