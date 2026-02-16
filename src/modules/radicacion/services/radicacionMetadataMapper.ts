import type {
  CamposPlantillaValidacionDTO,
  DetallePlantillaRadicadoDTO,
  PlantillaRadicadoDTO,
  RelCamposValRadicDTO,
} from "../models/PlantillaRadicadoDTO";

export type RadicacionInputType =
  | "text"
  | "number"
  | "date"
  | "textarea"
  | "select"
  | "checkbox";

export interface RadicacionFieldValidation {
  id: number;
  kind: string;
  message: string;
  parameter?: string;
}

export interface RadicacionFieldConfig {
  id: number;
  name: string;
  label: string;
  type: RadicacionInputType;
  required: boolean;
  order: number;
  placeholder?: string;
  defaultValue: string;
  options: ReadonlyArray<string>;
  validations: ReadonlyArray<RadicacionFieldValidation>;
}

const fieldTypeMap: Record<string, RadicacionInputType> = {
  texto: "text",
  text: "text",
  string: "text",
  numero: "number",
  number: "number",
  decimal: "number",
  fecha: "date",
  date: "date",
  textarea: "textarea",
  area: "textarea",
  lista: "select",
  select: "select",
  combo: "select",
  booleano: "checkbox",
  boolean: "checkbox",
  checkbox: "checkbox",
};

function mapFieldType(sourceType: string): RadicacionInputType {
  const normalized = sourceType.trim().toLowerCase();
  return fieldTypeMap[normalized] ?? "text";
}

function mapValidation(
  validation: CamposPlantillaValidacionDTO,
): RadicacionFieldValidation {
  return {
    id: validation.IdCampoPlantillaValidacion,
    kind: validation.TipoValidacion,
    message: validation.MensajeValidacion,
    parameter: validation.Parametro,
  };
}

function findValidationsByField(
  field: DetallePlantillaRadicadoDTO,
  links: ReadonlyArray<RelCamposValRadicDTO>,
  validations: ReadonlyArray<CamposPlantillaValidacionDTO>,
): ReadonlyArray<RadicacionFieldValidation> {
  const validationIds = new Set(
    links
      .filter(
        (item) =>
          item.IdDetallePlantillaRadicado === field.IdDetallePlantillaRadicado,
      )
      .map((item) => item.IdCampoPlantillaValidacion),
  );

  return validations
    .filter((validation) => validationIds.has(validation.IdCampoPlantillaValidacion))
    .map(mapValidation);
}

export function mapPlantillaToFieldConfig(
  plantilla: PlantillaRadicadoDTO,
): ReadonlyArray<RadicacionFieldConfig> {
  return [...plantilla.DetallePlantillaRadicadoDTO]
    .sort((left, right) => left.Orden - right.Orden)
    .map((field) => ({
      id: field.IdDetallePlantillaRadicado,
      name: field.NombreCampo,
      label: field.Etiqueta,
      type: mapFieldType(field.TipoCampo),
      required: field.Requerido,
      order: field.Orden,
      placeholder: field.Placeholder,
      defaultValue: field.ValorDefecto ?? "",
      options: field.Opciones ?? [],
      validations: findValidationsByField(
        field,
        plantilla.RelCamposValRadicDTO,
        plantilla.CamposPlantillaValidacionDTO,
      ),
    }));
}
