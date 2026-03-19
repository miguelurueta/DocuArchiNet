import { useEffect } from "react";
import { useQuery } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import clienteApi from "../../../api/Clienteaxios";
import type { ApiResponse } from "../../../api/ApiResponse";
import { useAuth } from "../../../app/auth/Hoks/useAuth";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";

const PLANTILLA_ENDPOINT = "/api/PlantillaRadicado/listaPlantilla";

export function useCamposPlantilla() {
  const { estaLogueado } = useAuth();
  const { block, unblock } = useOperationBlocker();

  const query = useQuery<ApiResponse<CampoPlantillaDTO[]>, AxiosError>({
    queryKey: ["dashboard-radicacion-campos"],
    enabled: estaLogueado,
    retry: false,
    queryFn: async () => {
      const { data } = await clienteApi.get<ApiResponse<CampoPlantillaDTO[]>>(
        PLANTILLA_ENDPOINT,
      );
      console.log(data);
      return data;
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
    data: query.data?.data ?? [],
    isLoading: query.isLoading,
    error: query.error,
    refetch: query.refetch,
  };
}
