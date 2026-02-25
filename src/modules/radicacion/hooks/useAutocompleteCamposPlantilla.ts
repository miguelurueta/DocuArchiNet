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

const AUTOCOMPLETE_ENDPOINT =
  "/api/PlantillaRadicado/solicitaAutoCompleteCampos";

export function useAutocompleteCamposPlantilla(
  params: AutoCompleteCampoRequest | null,
  enabled: boolean,
) {
  const queryKey = useMemo(
    () => [
      "autocomplete-campos-plantilla",
      params?.name_campo ?? "",
      params?.tbl_control ?? "",
      params?.TextoBuscado ?? "",
    ],
    [params?.TextoBuscado, params?.name_campo, params?.tbl_control],
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
      const { data } = await clienteApi.post<
        ApiResponse<AutoCompleteCampoItemDTO[]>
      >(AUTOCOMPLETE_ENDPOINT, params);
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
