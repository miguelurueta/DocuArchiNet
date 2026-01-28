import { useQuery } from "@tanstack/react-query";
import clienteApi from "../../../api/Clienteaxios";
import type { AxiosError } from "axios";
import type { ApiResponse } from "../../../api/ApiResponse";
import type ModuloDTO from "../models/ModuloDTO";

export function useModulosEmpresa(idEmpresa: number) {
  return useQuery<ApiResponse<ModuloDTO[]>, AxiosError>({
    queryKey: ["modulos-empresa", idEmpresa],
    enabled: idEmpresa > 0, // 🔥 se ejecuta apenas llegue la empresa
    queryFn: async () => {
      const { data } = await clienteApi.post<ApiResponse<ModuloDTO[]>>(
        "/api/accout/SolicitaModulosEmpresa",
        { IdEmpresa: idEmpresa }
      );
      return data;
    },
    retry: false,
  });
}
