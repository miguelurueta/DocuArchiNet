
import type { ApiResponse } from "../ApiResponse";
import clienteApi from "../Clienteaxios";
import { useEntidadMutation } from "./useEntidadMutation";


export function useCrearEntidad<TDto, TCreate>(
  endpoint: string,
  redirectTo: string
) {
  return useEntidadMutation<TDto, TCreate>(
    endpoint,
    async (data) => {
      const response = await clienteApi.post<ApiResponse<TDto>>(endpoint, data);
      return response.data;
    },
    {
      successMessage: "",
      redirectTo
    }
  );
}


