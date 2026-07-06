import { describe, expect, it } from "vitest";
import {
  RADICACION_ROUTES,
  RADICACION_ROUTE_SEGMENTS,
  RADICACION_TAB_KEYS,
  resolveRadicacionTabFromDestino,
} from "./radicacionRoutes";

describe("radicacionRoutes", () => {
  it("[SPEC:NAV-001] centraliza rutas semanticas del modulo", () => {
    expect(RADICACION_ROUTE_SEGMENTS.root).toBe("radicacion");
    expect(RADICACION_ROUTES.root).toBe("/dashboard/radicacion");
    expect(RADICACION_ROUTES.registro(12)).toBe(
      "/dashboard/radicacion/registro/12",
    );
    expect(RADICACION_ROUTES.documentos(12)).toBe(
      "/dashboard/radicacion/registro/12/documentos",
    );
  });

  it("[SPEC:NAV-002] usa keys de dominio para tabs", () => {
    expect(Object.values(RADICACION_TAB_KEYS)).toEqual([
      "ia",
      "radicacion",
      "documentos",
      "gestion-radicados",
    ]);
  });

  it("[SPEC:NAV-003] resuelve tab inicial sin indices numericos", () => {
    expect(
      resolveRadicacionTabFromDestino({
        destinoPostRegistro: "documentos",
        documentosDisponibles: true,
      }),
    ).toBe(RADICACION_TAB_KEYS.documentos);

    expect(
      resolveRadicacionTabFromDestino({
        destinoPostRegistro: "documentos",
        documentosDisponibles: false,
      }),
    ).toBe(RADICACION_TAB_KEYS.radicacion);
  });
});
