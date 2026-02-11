import type RespuestaAutenticacion from "../../../modules/login/models/RespuestaAutenticacionDTO";
import type Claim from "../Dto/Claim";

const llaveToken = "token";
const llaveExpiracion = "token-expiracion";

// 🔑 Key legacy (antes guardabas permisos como CSV). Mantener para compatibilidad.
const llavePermisos = "permisos";

// Nombre estándar del claim de permisos
const CLAIM_NAME = "perm";

// =======================================================
// Storage helpers (producción: sin logs sensibles)
// =======================================================

export function existeTokenRegistrado(): boolean {
  return !!localStorage.getItem(llaveToken);
}

export function guardarTokenLocalStorage(autenticacion: RespuestaAutenticacion) {
  localStorage.setItem(llaveToken, autenticacion.token);
  localStorage.setItem(llaveExpiracion, autenticacion.expiracion.toString());

  // ✅ Guardar permisos como JSON (mejor que CSV) pero soportar lectura legacy
  const permisos = autenticacion?.usuario?.permisos ?? [];
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
  if (!raw) {
    logout();
    return [];
  }

  const permisos = parsePermisos(raw);
  return permisos.map((permiso) => ({
    nombre: CLAIM_NAME,
    valor: permiso,
  }));
}

// Soporta JSON moderno o CSV legacy
function parsePermisos(raw: string): string[] {
  // 1) JSON array
  try {
    const parsed = JSON.parse(raw);
    if (Array.isArray(parsed)) {
      return parsed.map((x) => String(x)).filter(Boolean);
    }
  } catch {
    // ignore
  }

  // 2) Legacy CSV
  return raw
    .split(",")
    .map((p) => p.trim())
    .filter((p) => p.length > 0);
}

export function finalizarSesionYRedirigir() {
  logout();
  window.location.href = "/LoginPage";
}
