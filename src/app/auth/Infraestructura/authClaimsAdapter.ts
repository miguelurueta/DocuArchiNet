import { jwtDecode } from "jwt-decode";
import type Claim from "../Dto/Claim";

const PERMISSION_CLAIM_NAME = "perm";

const PERMISSION_LIKE_NAMES = new Set([
  "perm",
  "permission",
  "permissions",
  "scope",
  "scp",
  "role",
  "roles",
  "permisos",
  "permiso",
]);

type UnknownRecord = Record<string, unknown>;

const isRecord = (value: unknown): value is UnknownRecord =>
  typeof value === "object" && value !== null;

const getRecordValue = (record: UnknownRecord, keys: string[]): unknown => {
  for (const key of keys) {
    if (key in record) return record[key];
  }
  return undefined;
};

const normalizePermission = (value: unknown): string | null => {
  if (typeof value !== "string") return null;
  const normalized = value.trim().toLowerCase();
  return normalized.length > 0 ? normalized : null;
};

const normalizePermissionCollection = (values: unknown[]): string[] => {
  const result: string[] = [];
  const seen = new Set<string>();

  for (const value of values) {
    const normalized = normalizePermission(value);
    if (!normalized || seen.has(normalized)) continue;
    seen.add(normalized);
    result.push(normalized);
  }

  return result;
};

const toStringArray = (value: unknown): string[] => {
  if (Array.isArray(value)) {
    return normalizePermissionCollection(value);
  }

  if (typeof value === "string") {
    const raw = value.includes(" ") ? value.split(" ") : value.split(",");
    return normalizePermissionCollection(raw);
  }

  return [];
};

const extractPermissionsFromClaimsArray = (value: unknown): string[] => {
  if (!Array.isArray(value)) return [];

  const rawPermissions: unknown[] = [];

  for (const item of value) {
    if (typeof item === "string") {
      rawPermissions.push(item);
      continue;
    }

    if (!isRecord(item)) continue;

    const rawName = getRecordValue(item, ["nombre", "name", "type", "claimType"]);
    const rawValue = getRecordValue(item, ["valor", "value", "claimValue"]);

    if (typeof rawName === "string") {
      const normalizedName = rawName.trim().toLowerCase();
      if (PERMISSION_LIKE_NAMES.has(normalizedName)) {
        rawPermissions.push(...toStringArray(rawValue));
      }
      continue;
    }

    rawPermissions.push(...toStringArray(rawValue));
  }

  return normalizePermissionCollection(rawPermissions);
};

const extractPermissionsFromJwtPayload = (payload: UnknownRecord): string[] => {
  const keys = ["permissions", "permisos", "scope", "scp", "roles", "role", "perm"];
  const values: unknown[] = [];

  for (const key of keys) {
    values.push(...toStringArray(payload[key]));
  }

  return normalizePermissionCollection(values);
};

export const extractPermissionsFromToken = (token: unknown): string[] => {
  if (typeof token !== "string" || token.trim().length === 0) return [];

  try {
    const payload = jwtDecode<UnknownRecord>(token);
    return extractPermissionsFromJwtPayload(payload);
  } catch {
    return [];
  }
};

export const extractEffectivePermissions = (authPayload: unknown): string[] => {
  if (!isRecord(authPayload)) return [];

  const user = isRecord(authPayload.usuario) ? authPayload.usuario : undefined;

  const fromUserClaims = extractPermissionsFromClaimsArray(user?.claims);
  const fromTopClaims = extractPermissionsFromClaimsArray(authPayload.claims);
  const fromToken = extractPermissionsFromToken(authPayload.token);
  const fromLegacyPermisos = toStringArray(user?.permisos);

  const modernPermissions = normalizePermissionCollection([
    ...fromUserClaims,
    ...fromTopClaims,
    ...fromToken,
  ]);

  if (modernPermissions.length > 0) {
    return modernPermissions;
  }

  return fromLegacyPermisos;
};

export const buildClaimsFromPermissions = (permissions: string[]): Claim[] =>
  permissions.map((permission) => ({
    nombre: PERMISSION_CLAIM_NAME,
    valor: permission,
  }));

export const parseStoredPermissions = (rawValue: string): string[] => {
  try {
    const parsed = JSON.parse(rawValue);
    if (Array.isArray(parsed)) {
      return normalizePermissionCollection(parsed);
    }
  } catch {
    // fallback CSV legacy
  }

  return normalizePermissionCollection(rawValue.split(","));
};

export const hasPermissionClaim = (claims: Claim[], requiredPermission: string): boolean => {
  const normalizedRequired = normalizePermission(requiredPermission);
  if (!normalizedRequired) return false;

  return claims.some((claim) => {
    if (typeof claim?.nombre !== "string") return false;
    if (claim.nombre.trim().toLowerCase() !== PERMISSION_CLAIM_NAME) return false;
    const value = normalizePermission(claim.valor);
    return value === normalizedRequired;
  });
};
