import type { ApiResponse } from "../ApiResponse";
import clienteApi from "../Clienteaxios";
import { useEntidadMutation } from "./useEntidadMutation";

export function useEliminarEntidad(endpoint: string) {
  return useEntidadMutation<void, number>(
    endpoint,
    async (id) => {
      const response = await clienteApi.delete<ApiResponse<void>>(
        `${endpoint}/${id}`
      );
      return response.data;
    },
    {
      successMessage: ""
    }
  );
}

