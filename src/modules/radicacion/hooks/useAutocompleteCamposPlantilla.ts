import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";

export interface AutoCompleteCampoItemDTO {
  idValue: string | null;
  texValue: string;
}

export interface AutoCompleteCampoRequest {
  TextoBuscado: string;
  defaultDbAlias: string;
  tbl_control: string;
  name_campo: string;
}

interface AutoCompleteTerceroRequest {
  idScript: number;
  nombreCampo: string;
  valueCampo: string;
}

const AUTOCOMPLETE_ENDPOINT_DEFAULT =
  "/api/PlantillaRadicado/solicitaAutoCompleteCampos";
const AUTOCOMPLETE_ENDPOINT_REMITENTE =
  "/api/PlantillaRadicado/autoCompleteTercero";

const normalizeFieldName = (value: string | null | undefined) =>
  String(value ?? "").trim().toUpperCase();

export const resolveAutocompleteEndpoint = (nameCampo: string | null | undefined) => {
  if (normalizeFieldName(nameCampo) === "REMITENTE_COR") {
    return AUTOCOMPLETE_ENDPOINT_REMITENTE;
  }
  return AUTOCOMPLETE_ENDPOINT_DEFAULT;
};

export const buildAutocompletePayload = (
  endpoint: string,
  params: AutoCompleteCampoRequest,
): AutoCompleteCampoRequest | AutoCompleteTerceroRequest => {
  if (endpoint === AUTOCOMPLETE_ENDPOINT_REMITENTE) {
    return {
      idScript: 0,
      nombreCampo: normalizeFieldName(params.name_campo),
      valueCampo: params.TextoBuscado,
    };
  }
  return params;
};

export function useAutocompleteCamposPlantilla(
  params: AutoCompleteCampoRequest | null,
  enabled: boolean,
) {
  const endpoint = resolveAutocompleteEndpoint(params?.name_campo);
  const queryKey = useMemo(
    () => [
      "autocomplete-campos-plantilla",
      endpoint,
      params?.name_campo ?? "",
      params?.tbl_control ?? "",
      params?.TextoBuscado ?? "",
    ],
    [endpoint, params?.TextoBuscado, params?.name_campo, params?.tbl_control],
  );

  const query = useQuery<
    ApiResponse<AutoCompleteCampoItemDTO[]>,
    AxiosError
  >({
    queryKey,
    enabled,
    retry: false,
    queryFn: async () => {
      if (!params) {
        return {
          success: true,
          message: "OK",
          data: [],
        };
      }
      const payload = buildAutocompletePayload(endpoint, params);
      const { data } = await clienteApi.post<
        ApiResponse<AutoCompleteCampoItemDTO[]>
      >(endpoint, payload);
      return data;
    },
  });

  return {
    data: query.data?.data ?? [],
    isLoading: query.isLoading,
    isFetching: query.isFetching,
    error: query.error,
  };
}
