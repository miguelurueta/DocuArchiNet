import { useNavigate } from "react-router";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import { useMutation } from "@tanstack/react-query";
import type { RecuperarPasswordRequest } from "../Models/RecuperarPasswordRequest";
import { setRecuperarPassword } from "../services/seRecuperarPassword";

export function useRecuperarPasswordMutation() {
  const navigate = useNavigate();
  const notifyError = useAxiosErrorNotifier();
  const { block, unblock } = useOperationBlocker();
  return useMutation({
    mutationFn: async (data: RecuperarPasswordRequest) => {
      block("Enviando código de recuperación...");
      return await setRecuperarPassword(data);
    },
    onSuccess: (payload) => {
      
      navigate("/RecoveryPassword/forgot-password/verify", {
        state: { payload },
      });
    },
    onError: (error) => notifyError(error),
    onSettled: () => unblock(),
  });
}
