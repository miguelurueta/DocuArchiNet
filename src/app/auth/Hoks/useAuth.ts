import { useContext, useMemo } from "react";
import AutenticacionContext from "../Estado/AutenticacionContext";
import type Claim from "../Dto/Claim";

export interface UseAuthResult {
  claims: Claim[];
  estaLogueado: boolean;
  tienePermiso: (permiso: string) => boolean;
  refrescarClaims: () => void;
}

export function useAuth(): UseAuthResult {
  const { claims, refrescarClaims } = useContext(AutenticacionContext);

  const estaLogueado = useMemo(() => {
    return claims.length > 0;
  }, [claims]);

  const tienePermiso = (permiso: string): boolean => {
    return claims.some(
      (c) => c.nombre === "perm" && c.valor === permiso
    );
  };

  return {
    claims,
    estaLogueado,
    tienePermiso,
    refrescarClaims,
  };
}
