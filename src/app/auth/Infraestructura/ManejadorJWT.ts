import type RespuestaAutenticacion from "../../../modules/login/models/RespuestaAutenticacionDTO";
import type { NavigateFunction } from "react-router-dom";
import type Claim from "../Dto/Claim";
import {
  buildClaimsFromPermissions,
  extractEffectivePermissions,
  extractPermissionsFromToken,
  parseStoredPermissions,
} from "./authClaimsAdapter";

const llaveToken = "token";
const llaveExpiracion = "token-expiracion";

// 🔑 Key legacy (antes guardabas permisos como CSV). Mantener para compatibilidad.
const llavePermisos = "permisos";

// =======================================================
// Storage helpers (producción: sin logs sensibles)
// =======================================================

export function existeTokenRegistrado(): boolean {
  return !!localStorage.getItem(llaveToken);
}

export function guardarTokenLocalStorage(autenticacion: RespuestaAutenticacion) {
  localStorage.setItem(llaveToken, autenticacion.token);
  const expiracionNormalizada =
    autenticacion.expiracion instanceof Date
      ? autenticacion.expiracion.toISOString()
      : String(autenticacion.expiracion);
  localStorage.setItem(llaveExpiracion, expiracionNormalizada);

  // ✅ Prioriza claims del contrato nuevo y usa permisos legacy solo como fallback.
  const permisos = extractEffectivePermissions(autenticacion);
  localStorage.setItem(llavePermisos, JSON.stringify(permisos));
}

export function logout() {
  localStorage.removeItem(llaveToken);
  localStorage.removeItem(llaveExpiracion);
  localStorage.removeItem(llavePermisos);
}

export function obtenerToken() {
  return localStorage.getItem(llaveToken);
}

// =======================================================
// Validación de sesión / expiración (alineada con TokenWatcher)
// =======================================================

export function tokenExpirado(): boolean {
  const expiracion = obtenerExpiracionDate();
  if (!expiracion) return true;
  return expiracion.getTime() <= Date.now();
}

export function sesionValida(): boolean {
  return existeTokenRegistrado() && !tokenExpirado();
}

function obtenerExpiracionDate(): Date | null {
  const token = localStorage.getItem(llaveToken);
  const expiracion = localStorage.getItem(llaveExpiracion);

  if (!token || !expiracion) return null;

  const d = new Date(expiracion);
  if (Number.isNaN(d.getTime())) return null;

  return d;
}

// =======================================================
// Claims (permiso) - fuente: permisos planos guardados
// =======================================================

export function obtenerClaims(): Claim[] {
  // Si la sesión no es válida, limpiar y devolver vacío
  if (!sesionValida()) {
    logout();
    return [];
  }

  const raw = localStorage.getItem(llavePermisos);
  if (raw) {
    return buildClaimsFromPermissions(parseStoredPermissions(raw));
  }

  // Fallback deterministico por token cuando no existe snapshot en storage.
  const permisosToken = extractPermissionsFromToken(obtenerToken());
  if (permisosToken.length === 0) return [];
  return buildClaimsFromPermissions(permisosToken);
}

export function finalizarSesionYRedirigir(navigate?: NavigateFunction) {
  logout();

  if (navigate) {
    navigate("/LoginPage", { replace: true });
    return;
  }

  if (typeof window !== "undefined") {
    window.location.replace("/LoginPage");
  }
}
