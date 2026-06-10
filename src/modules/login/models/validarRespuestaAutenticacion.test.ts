import { describe, expect, test } from "vitest";
import { validarRespuestaAutenticacion } from "./validarRespuestaAutenticacion";

describe("[SPEC:ACTUALIZACION-CLAIM-003] validarRespuestaAutenticacion", () => {
  test("acepta contrato nuevo con claims sin permisos legacy", () => {
    const errores = validarRespuestaAutenticacion({
      token: "jwt-token",
      expiracion: "2026-01-01T00:00:00.000Z",
      claims: [{ nombre: "perm", valor: "tramites.gestionar" }],
      usuario: {
        usuarioId: 1,
        login: "jdoe",
        nombre: "John Doe",
        activo: true,
      },
    });

    expect(errores).toEqual([]);
  });

  test("falla cuando no hay claims ni permisos", () => {
    const errores = validarRespuestaAutenticacion({
      token: "jwt-token",
      expiracion: "2026-01-01T00:00:00.000Z",
      usuario: {
        usuarioId: 1,
        login: "jdoe",
        nombre: "John Doe",
        activo: true,
      },
    });

    expect(errores).toContain("No se recibieron claims ni permisos para autorización");
  });
});
