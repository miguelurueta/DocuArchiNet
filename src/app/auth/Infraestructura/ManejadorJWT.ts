import type RespuestaAutenticacion from "../../../modules/login/models/RespuestaAutenticacionDTO";
import type { NavigateFunction } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import type Claim from "../Dto/Claim";
import {
  buildClaimsFromPermissions,
  extractEffectivePermissions,
  extractPermissionsFromToken,
  parseStoredPermissions,
} from "./authClaimsAdapter";

const llaveToken = "token";
const llaveExpiracion = "token-expiracion";
const llaveUsuario = "usuario-autenticado";

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
  localStorage.setItem(llaveUsuario, JSON.stringify(autenticacion.usuario));
}

export function logout() {
  localStorage.removeItem(llaveToken);
  localStorage.removeItem(llaveExpiracion);
  localStorage.removeItem(llavePermisos);
  localStorage.removeItem(llaveUsuario);
}

export function obtenerToken() {
  return localStorage.getItem(llaveToken);
}

export function obtenerUsuarioIdAutenticado(): number | undefined {
  const fromStoredUser = readPositiveNumberFromStoredUser();
  if (fromStoredUser) {
    return fromStoredUser;
  }

  const token = obtenerToken();
  if (!token) {
    return undefined;
  }

  try {
    const payload = jwtDecode<Record<string, unknown>>(token);
    return readPositiveNumber(payload, [
      "usuarioId",
      "UsuarioId",
      "idUsuario",
      "IdUsuario",
      "IdUsuarioGestion",
      "idUsuarioGestion",
      "nameid",
      "sub",
      "uid",
    ]);
  } catch {
    return undefined;
  }
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

function readPositiveNumberFromStoredUser(): number | undefined {
  const raw = localStorage.getItem(llaveUsuario);
  if (!raw) {
    return undefined;
  }

  try {
    const parsed = JSON.parse(raw) as Record<string, unknown>;
    return readPositiveNumber(parsed, ["usuarioId", "UsuarioId", "idUsuario", "IdUsuario"]);
  } catch {
    return undefined;
  }
}

function readPositiveNumber(record: Record<string, unknown>, keys: string[]): number | undefined {
  for (const key of keys) {
    const raw = record[key];
    const numeric = typeof raw === "string" ? Number(raw) : raw;
    if (typeof numeric === "number" && Number.isFinite(numeric) && numeric > 0) {
      return numeric;
    }
  }

  return undefined;
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
