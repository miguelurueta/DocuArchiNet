import { describe, expect, it } from "vitest";
import {
  buildEstructuraRelacionTipoRestriccionParams,
  normalizeTramiteIdForRestriccion,
  normalizeEstructuraRelacionTipoRestriccionPayload,
} from "./useEstructuraRelacionTipoRestriccion";

describe("useEstructuraRelacionTipoRestriccion helpers", () => {
  it("[SPEC:TRS-001] normaliza id de tramite para consulta de restriccion", () => {
    expect(normalizeTramiteIdForRestriccion(23)).toBe("23");
    expect(normalizeTramiteIdForRestriccion(" 45 ")).toBe("45");
    expect(normalizeTramiteIdForRestriccion("   ")).toBe("0");
    expect(normalizeTramiteIdForRestriccion(null)).toBe("0");
  });

  it("[SPEC:TRS-002] construye query params para estructura de restriccion", () => {
    expect(buildEstructuraRelacionTipoRestriccionParams("23")).toEqual({
      idTipoTramite: "23",
    });
  });

  it("[SPEC:TRS-003] mapea respuesta ApiResponse con data objeto", () => {
    const mapped = normalizeEstructuraRelacionTipoRestriccionPayload({
      success: true,
      message: "OK",
      data: {
        IdRestriTipoDestInterno: 9,
        IdTipoRestriccion: 2,
        DescripcionTipo: null,
        MoluloRadicacion: 1,
        ModuloRadicacionSimple: 1,
        ModuloRadicacionInterna: 0,
      },
    });

    expect(mapped).toEqual({
      IdRestriTipoDestInterno: 9,
      IdTipoRestriccion: 2,
      DescripcionTipo: "",
      MoluloRadicacion: 1,
      ModuloRadicacionSimple: 1,
      ModuloRadicacionInterna: 0,
    });
  });
});
