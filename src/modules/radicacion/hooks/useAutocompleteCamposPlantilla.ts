import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import type { CDeRelacionEstadoRetriccionDto } from "../models/CDeRelacionEstadoRetriccionDto";
import { C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT } from "../models/CDeRelacionEstadoRetriccionDto";

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
  CDeRelacionEstadoRetriccionDto?: CDeRelacionEstadoRetriccionDto;
}

interface AutoCompleteTerceroRequest {
  idScript: number;
  nombreCampo: string;
  valueCampo: string;
}

interface AutoCompleteDestinatarioRestriccionRequest {
  ValueAuto: string;
  CDeRelacionEstadoRetriccionDto: CDeRelacionEstadoRetriccionDto;
}

const AUTOCOMPLETE_ENDPOINT_DEFAULT =
  "/api/PlantillaRadicado/solicitaAutoCompleteCampos";
const AUTOCOMPLETE_ENDPOINT_REMITENTE =
  "/api/PlantillaRadicado/autoCompleteTercero";
const AUTOCOMPLETE_ENDPOINT_DESTINATARIO =
  "/api/PlantillaRadicado/solicitaAutoCompleteDestinatarioRestriccion";

const normalizeFieldName = (value: string | null | undefined) =>
  String(value ?? "").trim().toUpperCase();

export const resolveAutocompleteEndpoint = (nameCampo: string | null | undefined) => {
  const normalized = normalizeFieldName(nameCampo);
  if (normalized === "REMITENTE_COR") {
    return AUTOCOMPLETE_ENDPOINT_REMITENTE;
  }
  if (normalized === "DESTINATARIO_COR") {
    return AUTOCOMPLETE_ENDPOINT_DESTINATARIO;
  }
  return AUTOCOMPLETE_ENDPOINT_DEFAULT;
};

export const buildAutocompletePayload = (
  endpoint: string,
  params: AutoCompleteCampoRequest,
): AutoCompleteCampoRequest | AutoCompleteTerceroRequest | AutoCompleteDestinatarioRestriccionRequest => {
  if (endpoint === AUTOCOMPLETE_ENDPOINT_REMITENTE) {
    return {
      idScript: typeof params.idScript === "number" ? params.idScript : 0,
      nombreCampo: normalizeFieldName(params.name_campo),
      valueCampo: params.TextoBuscado,
    };
  }
  if (endpoint === AUTOCOMPLETE_ENDPOINT_DESTINATARIO) {
    const dto =
      params.CDeRelacionEstadoRetriccionDto ??
      C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT;
    return {
      ValueAuto: params.TextoBuscado,
      CDeRelacionEstadoRetriccionDto: dto,
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
