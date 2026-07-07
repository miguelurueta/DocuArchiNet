import { useQuery } from "@tanstack/react-query";
import { fetchRadicacionPendientesContador } from "../services/radicacionPendientes.service";

export const RADICACION_PENDIENTES_CONTADOR_QUERY_KEY = [
  "radicacion",
  "pendientes",
  "contador",
] as const;

const resolveContador = (
  value: Awaited<ReturnType<typeof fetchRadicacionPendientesContador>>,
) =>
  value?.totalPendientes ??
  value?.TotalPendientes ??
  value?.cantidad ??
  value?.Cantidad ??
  value?.total ??
  value?.Total ??
  null;

export function useRadicacionPendientesContador(enabled: boolean) {
  const query = useQuery({
    queryKey: RADICACION_PENDIENTES_CONTADOR_QUERY_KEY,
    queryFn: fetchRadicacionPendientesContador,
    enabled,
    retry: false,
  });

  return {
    contador: resolveContador(query.data ?? null),
    loading: query.isFetching,
    error: query.error,
    refetch: query.refetch,
  };
}
