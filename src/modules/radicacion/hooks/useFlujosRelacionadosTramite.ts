import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";

const FLUJOS_TRAMITE_ENDPOINT =
  "/api/tramite/tramites/empsolicitaListaflujosRelacionadosTramite";

export interface FlujoRelacionadoOption {
  value: string;
  label: string;
}

export const normalizeTramiteId = (value: unknown): string | null => {
  if (value === null || value === undefined) {
    return null;
  }
  const normalized = String(value).trim();
  return normalized.length > 0 ? normalized : null;
};

export const normalizeFlujosRelacionados = (
  payload: unknown,
): FlujoRelacionadoOption[] => {
  const source = payload as
    | { data?: unknown; Data?: unknown }
    | Array<Record<string, unknown>>
    | null
    | undefined;
  const listCandidate = Array.isArray(source)
    ? source
    : Array.isArray(source?.data)
      ? source.data
      : Array.isArray(source?.Data)
        ? source.Data
        : [];

  return listCandidate
    .map((item) => {
      const row = item as Record<string, unknown>;
      const valueRaw = row.idValue ?? row.id_value ?? row.Id ?? row.id ?? null;
      const labelRaw =
        row.Value ?? row.value_campo ?? row.valueCampo ?? row.label ?? "";
      const value = valueRaw === null || valueRaw === undefined ? "" : String(valueRaw);
      const label = String(labelRaw ?? "").trim();
      return { value, label };
    })
    .filter((item) => item.value.length > 0 && item.label.length > 0);
};

export function useFlujosRelacionadosTramite(
  idTipoDocEntrante: unknown,
  enabled = true,
) {
  const tramiteId = normalizeTramiteId(idTipoDocEntrante);
  const shouldFetch = enabled && Boolean(tramiteId);
  const queryKey = useMemo(
    () => ["flujos-relacionados-tramite", tramiteId ?? ""],
    [tramiteId],
  );

  const query = useQuery<ApiResponse<unknown>, AxiosError>({
    queryKey,
    enabled: shouldFetch,
    retry: false,
    queryFn: async () => {
      const { data } = await clienteApi.post<ApiResponse<unknown>>(
        FLUJOS_TRAMITE_ENDPOINT,
        { idTipoDocEntrante: tramiteId },
      );
      return data;
    },
  });

  return {
    data: normalizeFlujosRelacionados(query.data),
    isLoading: query.isLoading,
    isFetching: query.isFetching,
    error: query.error,
    shouldFetch,
  };
}
