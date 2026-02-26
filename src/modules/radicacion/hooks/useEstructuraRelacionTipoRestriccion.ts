import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import type { CDeRelacionEstadoRetriccionDto } from "../models/CDeRelacionEstadoRetriccionDto";
import { C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT } from "../models/CDeRelacionEstadoRetriccionDto";

const ESTRUCTURA_RESTRICCION_ENDPOINT =
  "/api/tramite/tramites/solicitaEstructuraRelacionTipoRestriccion";

export const normalizeTramiteIdForRestriccion = (value: unknown): string | null => {
  if (value === null || value === undefined) {
    return null;
  }
  const normalized = String(value).trim();
  return normalized.length > 0 ? normalized : null;
};

export const buildEstructuraRelacionTipoRestriccionParams = (idValue: string) => ({
  idValue,
});

const mapToRestriccionDto = (payload: unknown): CDeRelacionEstadoRetriccionDto => {
  const source = payload as
    | Record<string, unknown>
    | { data?: unknown; Data?: unknown }
    | Array<Record<string, unknown>>
    | null
    | undefined;
  const listCandidate = Array.isArray(source)
    ? source
    : Array.isArray((source as { data?: unknown })?.data)
      ? ((source as { data?: unknown }).data as Array<Record<string, unknown>>)
      : Array.isArray((source as { Data?: unknown })?.Data)
        ? ((source as { Data?: unknown }).Data as Array<Record<string, unknown>>)
        : [];

  const row =
    (listCandidate[0] as Partial<CDeRelacionEstadoRetriccionDto> | undefined) ??
    (source as Partial<CDeRelacionEstadoRetriccionDto> | undefined) ??
    C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT;

  return {
    IdRestriTipoDestInterno: Number(row.IdRestriTipoDestInterno ?? 0) || 0,
    IdTipoRestriccion: Number(row.IdTipoRestriccion ?? 0) || 0,
    DescripcionTipo: String(row.DescripcionTipo ?? ""),
    MoluloRadicacion: Number(row.MoluloRadicacion ?? 0) || 0,
    ModuloRadicacionSimple: Number(row.ModuloRadicacionSimple ?? 0) || 0,
    ModuloRadicacionInterna: Number(row.ModuloRadicacionInterna ?? 0) || 0,
  };
};

export function useEstructuraRelacionTipoRestriccion(
  selectedTramiteId: unknown,
  enabled = true,
) {
  const tramiteId = normalizeTramiteIdForRestriccion(selectedTramiteId);
  const shouldFetch = enabled && Boolean(tramiteId);
  const queryKey = useMemo(
    () => ["estructura-relacion-tipo-restriccion", tramiteId ?? ""],
    [tramiteId],
  );

  const query = useQuery<ApiResponse<unknown>, AxiosError>({
    queryKey,
    enabled: shouldFetch,
    retry: false,
    queryFn: async () => {
      const { data } = await clienteApi.get<ApiResponse<unknown>>(
        ESTRUCTURA_RESTRICCION_ENDPOINT,
        {
          params: buildEstructuraRelacionTipoRestriccionParams(String(tramiteId)),
        },
      );
      return data;
    },
  });

  return {
    data: mapToRestriccionDto(query.data),
    isLoading: query.isLoading,
    isFetching: query.isFetching,
    error: query.error,
    shouldFetch,
  };
}
