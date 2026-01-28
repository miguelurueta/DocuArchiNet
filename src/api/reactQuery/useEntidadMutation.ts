import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { AxiosError } from "axios";
import { toast } from "react-toastify";
import { useNavigate } from "react-router";
import type { ApiResponse } from "../ApiResponse";
import { useAxiosErrorNotifier } from "../../shared/hooks/useAxiosErrorNotifier";


export function useEntidadMutation<TResult, TVariables>(
  queryKey: string,
  mutationFn: (variables: TVariables) => Promise<ApiResponse<TResult>>,
  options?: {
    successMessage?: string;
    redirectTo?: string;
  }
) {
  const queryClient = useQueryClient();
  const notifyError = useAxiosErrorNotifier();
  const navigate = useNavigate();

  const mutation = useMutation<ApiResponse<TResult>, AxiosError, TVariables>({
    mutationFn,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: [queryKey] });
    },
  });

  // 🔥 API empresarial DocuArchiCore
  const ejecutar = async (data: TVariables) => {
    try {
      await mutation.mutateAsync(data);

      if (options?.successMessage) {
        toast.success(options.successMessage);
      }

      if (options?.redirectTo) {
        navigate(options.redirectTo);
      }
    } catch (error) {
      notifyError(error);
      throw error;
    }
  };

  return {
    ejecutar,
    ...mutation
  };
}

