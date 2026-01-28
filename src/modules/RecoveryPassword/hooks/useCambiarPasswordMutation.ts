import { useNavigate } from "react-router";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import { useMutation } from "@tanstack/react-query";

import { seResetPassword } from "../services/seCambiarPassword";
import type { CambiarPasswordRequest } from "../Models/CambiarPasswordRequest";

export function useCambiarPasswordMutation() {
  const navigate = useNavigate();
  const notifyError = useAxiosErrorNotifier();
  const { block, unblock } = useOperationBlocker();

  return useMutation({
    mutationFn: async (data: CambiarPasswordRequest) => {
      block("Actualizando contraseña...");
      return await seResetPassword(data);
    },
    onSuccess: () => {
      navigate("/", { state: { reason: "PASSWORD_RESET_OK" } });
    },
    onError: (error) => notifyError(error),
    onSettled: () => unblock(),
  });
}
