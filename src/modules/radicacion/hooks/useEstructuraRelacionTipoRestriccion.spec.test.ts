import { describe, expect, it } from "vitest";
import {
  buildEstructuraRelacionTipoRestriccionParams,
  normalizeTramiteIdForRestriccion,
} from "./useEstructuraRelacionTipoRestriccion";

describe("useEstructuraRelacionTipoRestriccion helpers", () => {
  it("[SPEC:TRS-001] normaliza id de tramite para consulta de restriccion", () => {
    expect(normalizeTramiteIdForRestriccion(23)).toBe("23");
    expect(normalizeTramiteIdForRestriccion(" 45 ")).toBe("45");
    expect(normalizeTramiteIdForRestriccion("   ")).toBeNull();
    expect(normalizeTramiteIdForRestriccion(null)).toBeNull();
  });

  it("[SPEC:TRS-002] construye query params para estructura de restriccion", () => {
    expect(buildEstructuraRelacionTipoRestriccionParams("23")).toEqual({
      idValue: "23",
    });
  });
});
