import { describe, expect, it } from "vitest";
import { mapEstructuraRespuesta } from "./mapEstructuraRespuesta";

describe("[SPEC:SCRUMCORE-219] mapEstructuraRespuesta", () => {
  it("normaliza idRespuestaRadicado desde camelCase", () => {
    expect(
      mapEstructuraRespuesta({
        idRespuestaRadicado: "resp-001",
      }).idRespuestaRadicado,
    ).toBe("resp-001");
  });

  it("normaliza idRespuestaRadicado desde PascalCase", () => {
    expect(
      mapEstructuraRespuesta({
        IdRespuestaRadicado: 1024,
      }).idRespuestaRadicado,
    ).toBe(1024);
  });

  it("normaliza idRespuestaRadicado desde uppercase snake case", () => {
    expect(
      mapEstructuraRespuesta({
        ID_RESPUESTA_RADICADO: "resp-003",
      }).idRespuestaRadicado,
    ).toBe("resp-003");
  });

  it("normaliza idRespuestaRadicado desde snake_case", () => {
    expect(
      mapEstructuraRespuesta({
        id_respuesta_radicado: 2048,
      }).idRespuestaRadicado,
    ).toBe(2048);
  });

  it("mantiene idRespuestaRadicado como undefined cuando no existe", () => {
    const result = mapEstructuraRespuesta({
      Radicado: "2025-0001",
    });

    expect(result.idRespuestaRadicado).toBeUndefined();
    expect(result).not.toHaveProperty("idRespuestaRadicado");
  });

  it("aplica precedencia deterministica cuando existen multiples variantes", () => {
    expect(
      mapEstructuraRespuesta({
        idRespuestaRadicado: "camel",
        IdRespuestaRadicado: "pascal",
        ID_RESPUESTA_RADICADO: "upper",
        id_respuesta_radicado: "snake",
      }).idRespuestaRadicado,
    ).toBe("camel");
  });

  it("preserva el mapping existente de Radicado, Destinatario y TramiteDocumento", () => {
    expect(
      mapEstructuraRespuesta({
        radicado: "2025-0001",
        destinatario: "Contasoft Company",
        tramiteDocumento: "Respuesta a derecho de peticion",
      }),
    ).toEqual({
      Radicado: "2025-0001",
      Destinatario: "Contasoft Company",
      TramiteDocumento: "Respuesta a derecho de peticion",
    });
  });
});
