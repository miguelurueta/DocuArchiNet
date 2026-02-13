import { useMemo, useState } from "react";
import type { ChangeEvent } from "react";
import type { PlantillaRadicadoDTO } from "../models/PlantillaRadicadoDTO";
import {
  mapPlantillaToFieldConfig,
  type RadicacionFieldConfig,
} from "../services/radicacionMetadataMapper";
import {
  serializeRadicacionPayload,
  type RadicacionFormValues,
  type RadicacionPayloadDTO,
} from "../services/radicacionPayloadSerializer";

interface UseRadicacionDynamicFormResult {
  fields: ReadonlyArray<RadicacionFieldConfig>;
  values: RadicacionFormValues;
  setFieldValue: (name: string, value: string) => void;
  getInputValue: (name: string) => string;
  onInputChange: (event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => void;
  serialize: () => RadicacionPayloadDTO;
}

function createInitialValues(
  fields: ReadonlyArray<RadicacionFieldConfig>,
): RadicacionFormValues {
  return fields.reduce<RadicacionFormValues>((accumulator, field) => {
    accumulator[field.name] = field.defaultValue;
    return accumulator;
  }, {});
}

export function useRadicacionDynamicForm(
  plantilla: PlantillaRadicadoDTO,
): UseRadicacionDynamicFormResult {
  const fields = useMemo(() => mapPlantillaToFieldConfig(plantilla), [plantilla]);

  const [values, setValues] = useState<RadicacionFormValues>(() =>
    createInitialValues(fields),
  );

  const setFieldValue = (name: string, value: string): void => {
    setValues((current) => ({
      ...current,
      [name]: value,
    }));
  };

  const getInputValue = (name: string): string => values[name] ?? "";

  const onInputChange = (
    event: ChangeEvent<
      HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
    >,
  ): void => {
    const { name, value, type } = event.target;
    if (type === "checkbox") {
      const isChecked = (event.target as HTMLInputElement).checked;
      setFieldValue(name, isChecked ? "true" : "false");
      return;
    }
    setFieldValue(name, value);
  };

  const serialize = (): RadicacionPayloadDTO =>
    serializeRadicacionPayload(fields, values);

  return {
    fields,
    values,
    setFieldValue,
    getInputValue,
    onInputChange,
    serialize,
  };
}
