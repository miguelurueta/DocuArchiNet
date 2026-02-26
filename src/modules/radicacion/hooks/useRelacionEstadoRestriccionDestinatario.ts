import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import type { CDeRelacionEstadoRetriccionDto } from "../models/CDeRelacionEstadoRetriccionDto";
import { C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT } from "../models/CDeRelacionEstadoRetriccionDto";

const RESTRICCION_DESTINATARIO_ENDPOINT =
  "/api/radicacion/restriccion-destinatario";

export const normalizeRestriccionDestinatario = (
  payload: unknown,
): CDeRelacionEstadoRetriccionDto => {
  const row = (payload ?? {}) as Partial<CDeRelacionEstadoRetriccionDto>;
  return {
    IdRestriTipoDestInterno:
      Number(row.IdRestriTipoDestInterno ?? 0) || 0,
    IdTipoRestriccion: Number(row.IdTipoRestriccion ?? 0) || 0,
    DescripcionTipo: String(row.DescripcionTipo ?? "string"),
    MoluloRadicacion: Number(row.MoluloRadicacion ?? 0) || 0,
    ModuloRadicacionSimple: Number(row.ModuloRadicacionSimple ?? 0) || 0,
    ModuloRadicacionInterna: Number(row.ModuloRadicacionInterna ?? 0) || 0,
  };
};

export const buildRestriccionDestinatarioPayload = (
  dto: CDeRelacionEstadoRetriccionDto,
) => ({
  ...dto,
});

export function useRelacionEstadoRestriccionDestinatario(
  initialValue: CDeRelacionEstadoRetriccionDto = C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT,
  enabled = true,
) {
  const query = useQuery<ApiResponse<unknown>, AxiosError>({
    queryKey: ["radicacion-restriccion-destinatario"],
    enabled,
    retry: false,
    queryFn: async () => {
      const payload = buildRestriccionDestinatarioPayload(initialValue);
      const { data } = await clienteApi.post<ApiResponse<unknown>>(
        RESTRICCION_DESTINATARIO_ENDPOINT,
        payload,
      );
      return data;
    },
  });

  const resolvedValue = useMemo(() => {
    const candidate = query.data?.data;
    if (!candidate) {
      return initialValue;
    }
    return normalizeRestriccionDestinatario(candidate);
  }, [initialValue, query.data?.data]);

  return {
    data: resolvedValue,
    isLoading: query.isLoading,
    isFetching: query.isFetching,
    error: query.error,
  };
}
