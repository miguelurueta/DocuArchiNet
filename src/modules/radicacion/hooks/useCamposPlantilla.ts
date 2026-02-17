import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import { useAuth } from "../../../app/auth/Hoks/useAuth";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import type {
  CampoPlantillaDTO,
  PlantillaCamposApiResponseDTO,
} from "../models/CampoPlantillaDTO";

const PLANTILLA_ENDPOINT = "/api/PlantillaRadicado/listaPlantilla";

function normalizeCamposPayload(
  payload: ApiResponse<CampoPlantillaDTO[]> | PlantillaCamposApiResponseDTO,
): ReadonlyArray<CampoPlantillaDTO> {
  if ("data" in payload) {
    return payload.data ?? [];
  }

  return payload.Data ?? [];
}

export function useCamposPlantilla() {
  const { estaLogueado } = useAuth();
  const { block, unblock } = useOperationBlocker();

  const query = useQuery<ReadonlyArray<CampoPlantillaDTO>, AxiosError>({
    queryKey: ["radicacion-campos-plantilla"],
    enabled: estaLogueado,
    retry: false,
    queryFn: async () => {
      const { data } = await clienteApi.get<
        ApiResponse<CampoPlantillaDTO[]> | PlantillaCamposApiResponseDTO
      >(PLANTILLA_ENDPOINT);

      return normalizeCamposPayload(data);
    },
    placeholderData: (previousData) => previousData,
  });

  useEffect(() => {
    if (!estaLogueado) {
      unblock();
      return;
    }

    if (query.isLoading || query.isFetching) {
      block("Cargando campos de plantilla...");
      return;
    }

    unblock();
  }, [block, estaLogueado, query.isFetching, query.isLoading, unblock]);

  return {
    data: query.data ?? [],
    isLoading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
}
