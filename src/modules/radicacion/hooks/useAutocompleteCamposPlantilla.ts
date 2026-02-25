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
  idScript?: number;
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
      idScript: typeof params.idScript === "number" ? params.idScript : 0,
      nombreCampo: normalizeFieldName(params.name_campo),
      valueCampo: params.TextoBuscado,
    };
  }
  return params;
};

export const normalizeAutoCompleteItems = (
  payload: unknown,
): AutoCompleteCampoItemDTO[] => {
  const source = payload as
    | { data?: unknown; Data?: unknown }
    | AutoCompleteCampoItemDTO[]
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
      const anyItem = item as Record<string, unknown>;
      const idValue =
        anyItem.idValue ??
        anyItem.id_value ??
        anyItem.id ??
        anyItem.Id ??
        anyItem.idTercero ??
        null;
      const textRaw =
        anyItem.texValue ??
        anyItem.valueCampo ??
        anyItem.value_campo ??
        anyItem.Value ??
        anyItem.nombre ??
        anyItem.descripcion ??
        anyItem.label ??
        anyItem.text ??
        "";
      return {
        idValue: idValue === null || idValue === undefined ? null : String(idValue),
        texValue: String(textRaw ?? "").trim(),
      };
    })
    .filter((item) => item.texValue.length > 0);
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
        ApiResponse<unknown>
      >(endpoint, payload);
      return data;
    },
  });

  const normalizedItems = normalizeAutoCompleteItems(query.data);

  return {
    data: normalizedItems,
    isLoading: query.isLoading,
    isFetching: query.isFetching,
    error: query.error,
  };
}
