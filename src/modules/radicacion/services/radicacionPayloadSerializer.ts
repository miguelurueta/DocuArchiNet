import type { RadicacionFieldConfig } from "./radicacionMetadataMapper";

export interface CampoValorRadicadoDTO {
  IdDetallePlantillaRadicado: number;
  NombreCampo: string;
  Valor: string;
}

export interface RadicacionPayloadDTO {
  Campos: ReadonlyArray<CampoValorRadicadoDTO>;
}

export type RadicacionFormValues = Record<string, string>;

export function serializeRadicacionPayload(
  fields: ReadonlyArray<RadicacionFieldConfig>,
  values: RadicacionFormValues,
): RadicacionPayloadDTO {
  return {
    Campos: fields.map((field) => ({
      IdDetallePlantillaRadicado: field.id,
      NombreCampo: field.name,
      Valor: values[field.name] ?? "",
    })),
  };
}
