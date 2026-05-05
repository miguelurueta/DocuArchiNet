import { useCallback } from "react";
import type RespuestaAutenticacion from "../../../modules/login/models/RespuestaAutenticacionDTO";
import clienteApi from "../../../api/Clienteaxios";
import { guardarTokenLocalStorage } from "../Infraestructura/ManejadorJWT";

/**
 * Hook reutilizable para renovar el token
 * Integra queries ya existentes
 */
export default function useRenovarToken() {
  const renovarToken = useCallback(async (): Promise<void> => {
    // ⚠️ Reutiliza tu query real existente aquí
    const response = await clienteApi.post<RespuestaAutenticacion>(
      "/api/auth/renew"
    );

    guardarTokenLocalStorage(response.data);
  }, []);

  return { renovarToken };
}
