import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";
import type LoginRequestDTO from "../models/LoginRequestDTO.model";
import { validarRespuestaAutenticacion } from "../models/validarRespuestaAutenticacion";
import crearAxiosErrorContrato from "../../../shared/errors/crearAxiosErrorContrato";
import { seLoginUsuario } from "../services/seLoginUsuario";
import { useAuth } from "../../../app/auth/Hoks/useAuth";
import { AuthSessionService } from "../../OTP/service/AuthSessionService";

export default function useLogin() {
  const navigate = useNavigate();
  const notifyError = useAxiosErrorNotifier();
  const { block, unblock } = useOperationBlocker();
  const {  refrescarClaims } = useAuth();
  const mutation = useMutation({
    mutationFn: async (data: LoginRequestDTO) => {
      block("Validando credenciales...");
      const resp = await seLoginUsuario(data);
      // 🔐 Segundo factor
      if (resp.message === "SECOND_FACTOR_REQUIRED") {
        return {
          tipo: "SECOND_FACTOR",
          payload: resp.data
        };
      }
      // ✅ Autenticación normal
      const dataResp = resp.data;
      const errores = validarRespuestaAutenticacion(dataResp);
      if (errores.length > 0) {
        throw crearAxiosErrorContrato(errores);
      }
      return {
        tipo: "AUTH_SUCCESS",
        payload: {
          token: dataResp.token,
          expiracion: new Date(dataResp.expiracion),
          usuario: {
            usuarioId: dataResp.usuario.usuarioId,
            login: dataResp.usuario.login,
            email: dataResp.usuario.email ?? undefined,
            nombre: dataResp.usuario.nombre,
            activo: dataResp.usuario.activo,
            fechaLimiteAcceso: dataResp.usuario.fechaLimiteAcceso
              ? new Date(dataResp.usuario.fechaLimiteAcceso)
              : undefined,
            permisos: dataResp.usuario.permisos ?? [],
            claims: dataResp.usuario.claims ?? dataResp.claims ?? [],
          }
        },
      };
    },
    onSuccess: (result) => {
      if (result.tipo === "SECOND_FACTOR") {
        navigate("/verificar-otp", { state: result });
        return;
      }
      if (result.tipo === "AUTH_SUCCESS") {
        // 1️⃣ Persistir sesión (token + expiración + usuario)
        AuthSessionService.iniciarSesion(result.payload);
       
        // 2️⃣ Sincronizar estado React (claims)
        refrescarClaims();

        // 3️⃣ Navegar DIRECTAMENTE (fuente de verdad = backend)
        navigate("/dashboard");
       
      }
    },

    onError: (error: any) => {
      if (error?.isAxiosError) {
        notifyError(error);
      } else {
        console.error("Error no Axios:", error);
        notifyError(new Error("Error inesperado al iniciar sesión"));
      }
    },

    onSettled: () => {
      unblock();
    }
  });

  return {
    login: mutation.mutateAsync,
    isLoading: mutation.isPending
  };
}


