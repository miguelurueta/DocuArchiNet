import { describe, expect, it } from "vitest";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import {
  buildCampoPlantillaRules,
  getCampoMaxLength,
  shouldValidateCampoMaxLength,
} from "./radicacionCampoValidation";

const campo = {
  obligatorio_campo: 1,
  max_leng_campo: 5,
} as CampoPlantillaDTO;

describe("radicacionCampoValidation", () => {
  it("[SPEC:FE-01] construye required y max desde metadata backend para texto", () => {
    expect(buildCampoPlantillaRules(campo, { label: "Solicitante" })).toEqual([
      { required: true, message: "Ingrese Solicitante" },
      {
        max: 5,
        message: "Solicitante supera la longitud maxima permitida.",
      },
    ]);
  });

  it("[SPEC:FE-01] no aplica max_leng_campo a selects porque envian idValue", () => {
    expect(
      buildCampoPlantillaRules(campo, {
        label: "Tipo De Recepcion",
        mode: "selection",
      }),
    ).toEqual([
      { required: true, message: "Seleccione Tipo De Recepcion" },
    ]);
  });

  it("[SPEC:FE-01] no aplica max_leng_campo a campos numericos", () => {
    expect(
      buildCampoPlantillaRules(campo, {
        label: "Número Folios",
        mode: "number",
      }),
    ).toEqual([{ required: true, message: "Ingrese Número Folios" }]);
  });

  it("[SPEC:FE-01] expone helpers para sincronizar dialecto con backend", () => {
    expect(getCampoMaxLength(campo)).toBe(5);
    expect(shouldValidateCampoMaxLength("text")).toBe(true);
    expect(shouldValidateCampoMaxLength("selection")).toBe(false);
    expect(shouldValidateCampoMaxLength("number")).toBe(false);
  });
});
