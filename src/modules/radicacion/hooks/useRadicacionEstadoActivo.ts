import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import {
  fetchRadicacionEstadoActivo,
  mapEstadoActivoToDocumentalState,
} from "../services/radicacionPendientes.service";
import type { RadicacionPendienteEstadoActivoDto } from "../types/radicacionDocumental.types";

export const RADICACION_ESTADO_ACTIVO_QUERY_KEY = [
  "radicacion",
  "pendientes",
  "estado-activo",
] as const;

export function useRadicacionEstadoActivo() {
  const query = useQuery<RadicacionPendienteEstadoActivoDto | null, AxiosError>({
    queryKey: RADICACION_ESTADO_ACTIVO_QUERY_KEY,
    retry: false,
    queryFn: fetchRadicacionEstadoActivo,
  });

  return {
    data: query.data ?? null,
    contextoDocumental: mapEstadoActivoToDocumentalState(query.data ?? null),
    isLoading: query.isLoading,
    isFetching: query.isFetching,
    isError: query.isError,
    error: query.error,
    refetch: query.refetch,
  };
}
