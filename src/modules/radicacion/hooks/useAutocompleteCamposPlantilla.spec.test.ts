import { describe, expect, it } from "vitest";
import { resolveAutocompleteEndpoint } from "./useAutocompleteCamposPlantilla";

describe("useAutocompleteCamposPlantilla endpoint resolver", () => {
  it("[SPEC:RMT-001] usa endpoint de tercero para REMITENTE_COR", () => {
    expect(resolveAutocompleteEndpoint("REMITENTE_COR")).toBe(
      "/api/PlantillaRadicado/autoCompleteTercero",
    );
    expect(resolveAutocompleteEndpoint(" remitente_cor ")).toBe(
      "/api/PlantillaRadicado/autoCompleteTercero",
    );
  });

  it("[SPEC:RMT-002] usa endpoint default para otros campos", () => {
    expect(resolveAutocompleteEndpoint("ANEXOS_COR")).toBe(
      "/api/PlantillaRadicado/solicitaAutoCompleteCampos",
    );
  });
});
