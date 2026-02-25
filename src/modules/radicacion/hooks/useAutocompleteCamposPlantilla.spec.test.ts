import { describe, expect, it } from "vitest";
import {
  buildAutocompletePayload,
  resolveAutocompleteEndpoint,
} from "./useAutocompleteCamposPlantilla";

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

  it("[SPEC:RMT-004] construye payload de tercero para REMITENTE_COR", () => {
    const endpoint = resolveAutocompleteEndpoint("REMITENTE_COR");
    const payload = buildAutocompletePayload(endpoint, {
      TextoBuscado: "mi",
      defaultDbAlias: "",
      tbl_control: "RAD_GESTION",
      name_campo: "REMITENTE_COR",
    });

    expect(payload).toEqual({
      idScript: 0,
      nombreCampo: "REMITENTE_COR",
      valueCampo: "mi",
    });
  });

  it("[SPEC:RMT-005] mantiene payload legacy para otros campos", () => {
    const endpoint = resolveAutocompleteEndpoint("ANEXOS_COR");
    const payload = buildAutocompletePayload(endpoint, {
      TextoBuscado: "55",
      defaultDbAlias: "",
      tbl_control: "rad_gestion",
      name_campo: "ANEXOS_COR",
    });

    expect(payload).toEqual({
      TextoBuscado: "55",
      defaultDbAlias: "",
      tbl_control: "rad_gestion",
      name_campo: "ANEXOS_COR",
    });
  });
});
