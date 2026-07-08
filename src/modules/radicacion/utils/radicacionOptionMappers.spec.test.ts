import { describe, expect, it } from "vitest";
import type { CampoPlantillaDTO } from "../models/CampoPlantillaDTO";
import {
  mapCampoDrowlistOptions,
  mapTipoRadicadoOptions,
  mapTramiteOptions,
  normalizeCampoName,
  resolveCampoIdScript,
} from "./radicacionOptionMappers";

describe("radicacionOptionMappers", () => {
  it("[SPEC:TD-FE-03] normaliza opciones de tramite con variantes del backend", () => {
    expect(
      mapTramiteOptions([
        { id_value: "1", value_campo: "PQRS" },
        { idValue: 23, Value: "CITACION" },
        { value: "33", label: "GENERAL" },
        { idValue: null, Value: "" },
      ]),
    ).toEqual([
      { value: "1", label: "PQRS" },
      { value: 23, label: "CITACION" },
      { value: "33", label: "GENERAL" },
    ]);
  });

  it("[SPEC:TD-FE-03] normaliza TipoRadicado y conserva opcion inicial", () => {
    expect(
      mapTipoRadicadoOptions([
        { idValue: "E", Value: "Entrada" },
        { id_value: "I", value_campo: "Interno" },
      ]),
    ).toEqual([
      { value: "", label: "Seleccionar" },
      { value: "E", label: "Entrada" },
      { value: "I", label: "Interno" },
    ]);
  });

  it("[SPEC:TD-FE-03] normaliza opciones de campos dinamicos con fallback estable", () => {
    expect(
      mapCampoDrowlistOptions([
        { idValue: "CC", Value: "Cedula" },
        { id_value: "A", value_campo: "Activo" },
        { Value: "Sin valor explicito" },
      ]),
    ).toEqual([
      { value: "CC", label: "Cedula" },
      { value: "A", label: "Activo" },
      { value: "2", label: "Sin valor explicito" },
    ]);
  });

  it("[SPEC:TD-FE-03] centraliza normalizacion de nombre de campo e idScript", () => {
    expect(normalizeCampoName(" destinatario_cor ")).toBe("DESTINATARIO_COR");
    expect(
      resolveCampoIdScript({
        TomPParameterTomSelelect: { id_escript: 987 },
      } as CampoPlantillaDTO),
    ).toBe(987);
    expect(resolveCampoIdScript({ id_escript: 654 } as unknown as CampoPlantillaDTO)).toBe(
      654,
    );
  });
});
