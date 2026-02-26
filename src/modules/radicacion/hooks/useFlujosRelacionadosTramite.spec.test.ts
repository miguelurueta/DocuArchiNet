import { describe, expect, it } from "vitest";
import {
  normalizeFlujosRelacionados,
  normalizeTramiteId,
} from "./useFlujosRelacionadosTramite";

describe("useFlujosRelacionadosTramite helpers", () => {
  it("[SPEC:FLJ-003] normaliza idTipoDocEntrante desde value seleccionado", () => {
    expect(normalizeTramiteId(23)).toBe("23");
    expect(normalizeTramiteId("  45 ")).toBe("45");
    expect(normalizeTramiteId(null)).toBeNull();
    expect(normalizeTramiteId("   ")).toBeNull();
  });

  it("[SPEC:FLJ-004] normaliza respuesta de flujos relacionados", () => {
    expect(
      normalizeFlujosRelacionados({
        data: [
          { idValue: 1, Value: "FLUJO 1" },
          { id_value: "2", value_campo: "FLUJO 2" },
        ],
      }),
    ).toEqual([
      { value: "1", label: "FLUJO 1" },
      { value: "2", label: "FLUJO 2" },
    ]);
  });
});
