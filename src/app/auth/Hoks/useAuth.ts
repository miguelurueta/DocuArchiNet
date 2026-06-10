import { useContext, useMemo } from "react";
import AutenticacionContext from "../Estado/AutenticacionContext";
import type Claim from "../Dto/Claim";
import { hasPermissionClaim } from "../Infraestructura/authClaimsAdapter";
import { sesionValida } from "../Infraestructura/ManejadorJWT";

export interface UseAuthResult {
  claims: Claim[];
  estaLogueado: boolean;
  tienePermiso: (permiso: string) => boolean;
  refrescarClaims: () => void;
}

export function useAuth(): UseAuthResult {
  const { claims, refrescarClaims } = useContext(AutenticacionContext);

  const estaLogueado = useMemo(() => sesionValida(), [claims]);

  const tienePermiso = (permiso: string): boolean => {
    return hasPermissionClaim(claims, permiso);
  };

  return {
    claims,
    estaLogueado,
    tienePermiso,
    refrescarClaims,
  };
}
