import { describe, expect, it } from "vitest";
import { mapAppGridRowsToAppTableRows } from "../adapters/appGridToAppTableRows";

describe("[SPEC:IMPLEMENTACION-LISTA-GESTION-CORRESPONDENCIA] appGridToAppTableRows", () => {
  it("flattens AppGrid rows into AppTable rowData preserving id", () => {
    const result = mapAppGridRowsToAppTableRows([
      {
        id: "924",
        data: {
          RADICADO: "2500456700023",
          BENEFICIARIO: "Yeraldi Alvarado",
        },
      },
    ]);

    expect(result).toEqual([
      {
        id: "924",
        RADICADO: "2500456700023",
        BENEFICIARIO: "Yeraldi Alvarado",
      },
    ]);
  });
});
