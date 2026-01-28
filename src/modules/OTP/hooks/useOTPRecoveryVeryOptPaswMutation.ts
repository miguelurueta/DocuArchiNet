import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";
import { seVerificarOtpPaswRecovery } from "../service/seVerificarOtpPaswRecovery";
import type { OptRecoveryPaswResponse } from "../models/OptRecoveryPaswResponse";
import type { OptRecoveryPaswReguest } from "../models/OptRecoveryPaswReguest";

export function useOTPRecoveryVeryOptPaswMutation() {
  const navigate = useNavigate();
  const notifyError = useAxiosErrorNotifier();
  const { block, unblock } = useOperationBlocker();

  return useMutation<OptRecoveryPaswResponse, any, OptRecoveryPaswReguest>({
    mutationFn: async (data) => {
      block("Verificando código...");
      return await seVerificarOtpPaswRecovery(data);
    },
    onSuccess: (result) => {
      console.log(result);
      navigate("/RecoveryPassword/cambiar-password", {
        state: {
          token: result.token,
          userId: result.usuario.usuarioId ,
          idModule:result.idModule
        },
      });
    },
    onError: (error) => notifyError(error),
    onSettled: () => unblock(),
  });
}
