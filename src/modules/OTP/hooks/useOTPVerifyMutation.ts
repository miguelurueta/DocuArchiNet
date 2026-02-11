import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";
import type RespuestaAutenticacion from "../../login/models/RespuestaAutenticacionDTO";
import type { VerificarOtpRequest } from "../models/VerificarOtpRequest";
import { AuthSessionService } from "../service/AuthSessionService";
import { seVerificarSegundoFactor } from "../service/seVerificarSegundoFactor";
import { useAuth } from "../../../app/auth/Hoks/useAuth";

export function useOTPVerifyMutation() {
  const navigate = useNavigate();
  const notifyError = useAxiosErrorNotifier();
  const { block, unblock } = useOperationBlocker();
  const { refrescarClaims } = useAuth();
  return useMutation<RespuestaAutenticacion, any, VerificarOtpRequest>({
    mutationFn: async (data) => {
      block("Verificando segundo factor...");
      return await seVerificarSegundoFactor(data); // ✅ ya retorna RespuestaAutenticacion
    },
    onSuccess: (auth) => {
      // 1️⃣ Persistir sesión (token + expiración + usuario)
      AuthSessionService.iniciarSesion(auth);

      // 2️⃣ Sincronizar estado React (claims)
      refrescarClaims();

      // 3️⃣ Navegar DIRECTAMENTE (fuente de verdad = backend)
      navigate("/dashboard");
    },
    onError: (error) => notifyError(error),
    onSettled: () => unblock(),
  });
}
