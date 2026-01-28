import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router";
import { useOperationBlocker } from "../../../app/Components/UI/OperationBlockerContext";
import { useAxiosErrorNotifier } from "../../../shared/hooks/useAxiosErrorNotifier";

import type LoginRequestDTO from "../models/LoginRequestDTO.model";
import { validarRespuestaAutenticacion } from "../models/validarRespuestaAutenticacion";
import crearAxiosErrorContrato from "../../../shared/errors/crearAxiosErrorContrato";
import { guardarTokenLocalStorage } from "../../../app/auth/ManejadorJWT";

import { seLoginUsuario } from "../services/seLoginUsuario";

export default function useLogin() {
  const navigate = useNavigate();
  const notifyError = useAxiosErrorNotifier();
  const { block, unblock } = useOperationBlocker();

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
          token: dataResp.Token,
          expiracion: new Date(dataResp.Expiracion),
          usuario: {
            usuarioId: dataResp.Usuario.UsuarioId,
            login: dataResp.Usuario.Login,
            email: dataResp.Usuario.Email ?? undefined,
            nombre: dataResp.Usuario.Nombre,
            activo: dataResp.Usuario.Activo,
            fechaLimiteAcceso: dataResp.Usuario.FechaLimiteAcceso
              ? new Date(dataResp.Usuario.FechaLimiteAcceso)
              : undefined,
            permisos: dataResp.Usuario.Permisos ?? [],
          }
        }
      };
    },

    onSuccess: (result) => {
      if (result.tipo === "SECOND_FACTOR") {
        navigate("/verificar-otp", { state: result });
        return;
      }

      if (result.tipo === "AUTH_SUCCESS") {
        guardarTokenLocalStorage(result.payload);
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
