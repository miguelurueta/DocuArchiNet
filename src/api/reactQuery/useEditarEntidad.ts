
import type { ApiResponse } from "../ApiResponse";
import clienteApi from "../Clienteaxios";
import { useEntidadMutation } from "./useEntidadMutation";

export function useEditarEntidad<TDto, TUpdate>(
  endpoint: string,
  id: number,
  redirectTo: string
) {
  return useEntidadMutation<TDto, TUpdate>(
    endpoint,
    async (data) => {
      const response = await clienteApi.put<ApiResponse<TDto>>(
        `${endpoint}/${id}`,
        data
      );
      return response.data;
    },
    {
      successMessage: "",
      redirectTo
    }
  );
}

