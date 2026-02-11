import { useCallback } from "react";
import { guardarTokenLocalStorage } from "../Infraestructura/ManejadorJWT";
import type RespuestaAutenticacion from "../../../modules/login/models/RespuestaAutenticacionDTO";

/**
 * Hook reutilizable para renovar el token
 * Integra queries ya existentes
 */
export default function useRenovarToken() {
  const renovarToken = useCallback(async (): Promise<void> => {
    // ⚠️ Reutiliza tu query real existente aquí
    const response = await fetch("/api/auth/renew", {
      method: "POST",
      credentials: "include",
    });

    if (!response.ok) {
      throw new Error("No fue posible renovar el token");
    }

    const data: RespuestaAutenticacion = await response.json();

    // Reutiliza infraestructura existente
    guardarTokenLocalStorage(data);
  }, []);

  return { renovarToken };
}
