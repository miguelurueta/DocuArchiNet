import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";
import type RespuestaAutenticacion from "../../login/models/RespuestaAutenticacionDTO";
import type { VerificarOtpRequest } from "../models/VerificarOtpRequest";
import { AuthSessionService } from "../service/AuthSessionService";
import { seVerificarSegundoFactor } from "../service/seVerificarSegundoFactor";


export function useOTPVerifyMutation() {
  const navigate = useNavigate();
  const notifyError = useAxiosErrorNotifier();
  const { block, unblock } = useOperationBlocker();

  return useMutation<RespuestaAutenticacion, any, VerificarOtpRequest>({
    mutationFn: async (data) => {
      
      block("Verificando segundo factor...");
      return await seVerificarSegundoFactor(data); // ✅ ya retorna RespuestaAutenticacion
    },
    onSuccess: (auth) => {
      AuthSessionService.iniciarSesion(auth);
      navigate("/");
    },
    onError: (error) => notifyError(error),
    onSettled: () => unblock(),
  });
}
