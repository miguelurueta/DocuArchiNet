import { useQuery } from "@tanstack/react-query";

import type { AxiosError } from "axios";
import type { ApiResponse } from "../ApiResponse";
import clienteApi from "../Clienteaxios";



type ListadoParams = {
  page?: number;
  pageSize?: number;
  search?: string;
  sort?: string;
  order?: "asc" | "desc";
};

export function useListadoEntidad<T>(
  endpoint: string,
  queryKey: string,
  params?: ListadoParams
) {
  return useQuery<ApiResponse<T[]>, AxiosError>({
    queryKey: [queryKey, params],
    queryFn: async () => {
      const { data } = await clienteApi.get<ApiResponse<T[]>>(endpoint, {
        params,
      });
      return data;
    },
    retry: false,
    placeholderData: (previousData) => previousData, // ✅ equivalente moderno
  });
}

