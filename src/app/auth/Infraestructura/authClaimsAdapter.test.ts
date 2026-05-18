import { describe, expect, test } from "vitest";
import {
  extractEffectivePermissions,
  hasPermissionClaim,
  parseStoredPermissions,
} from "./authClaimsAdapter";

const buildUnsignedJwt = (payload: Record<string, unknown>): string => {
  const header = { alg: "none", typ: "JWT" };

  const encode = (value: unknown): string =>
    Buffer.from(JSON.stringify(value))
      .toString("base64")
      .replace(/\+/g, "-")
      .replace(/\//g, "_")
      .replace(/=+$/g, "");

  return `${encode(header)}.${encode(payload)}.`;
};

describe("[SPEC:ACTUALIZACION-CLAIM-001] authClaimsAdapter", () => {
  test("prioriza claims modernos sobre permisos legacy", () => {
    const permissions = extractEffectivePermissions({
      token: "token",
      claims: [{ nombre: "perm", valor: "tramites.aprobar" }],
      usuario: {
        permisos: ["tramites.legacy"],
      },
    });

    expect(permissions).toEqual(["tramites.aprobar"]);
  });

  test("usa fallback legacy cuando no hay claims modernos", () => {
    const permissions = extractEffectivePermissions({
      token: "token",
      usuario: {
        permisos: ["tramites.archivar", " tramites.archivar "],
      },
    });

    expect(permissions).toEqual(["tramites.archivar"]);
  });

  test("extrae permisos desde token jwt cuando están en payload", () => {
    const token = buildUnsignedJwt({
      permissions: ["tramites.gestionar"],
    });

    const permissions = extractEffectivePermissions({
      token,
      usuario: {},
    });

    expect(permissions).toEqual(["tramites.gestionar"]);
  });

  test("normaliza permisos persistidos en json y csv legacy", () => {
    expect(parseStoredPermissions('["A.B","a.b","  c.d  "]')).toEqual(["a.b", "c.d"]);
    expect(parseStoredPermissions("x.y, x.y , z.k")).toEqual(["x.y", "z.k"]);
  });

  test("evalua permisos requeridos con comparacion normalizada", () => {
    const claims = [
      { nombre: "perm", valor: "tramites.aprobar" },
      { nombre: "role", valor: "admin" },
    ];

    expect(hasPermissionClaim(claims, " TRAMITES.APROBAR ")).toBe(true);
    expect(hasPermissionClaim(claims, "tramites.archivar")).toBe(false);
  });
});
