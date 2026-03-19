import { Fragment } from "react";
import type { ChangeEventHandler } from "react";
import type { RadicacionFieldConfig } from "../services/radicacionMetadataMapper";
import type { RadicacionFormValues } from "../services/radicacionPayloadSerializer";
import { radicacionFieldRegistry } from "../services/fieldRegistry";
import type { RadicacionFieldComponentProps } from "./FieldComponents";

interface RadicacionDynamicRendererProps {
  fields: ReadonlyArray<RadicacionFieldConfig>;
  values: RadicacionFormValues;
  onChange: ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
  >;
}

export function RadicacionDynamicRenderer({
  fields,
  values,
  onChange,
}: RadicacionDynamicRendererProps) {
  return (
    <Fragment>
      {fields.map((field) => {
        const FieldComponent = radicacionFieldRegistry[field.type];
        const componentProps: RadicacionFieldComponentProps = {
          field,
          value: values[field.name] ?? "",
          onChange,
        };
        return (
          <div key={field.id}>
            <label htmlFor={field.name}>{field.label}</label>
            <FieldComponent {...componentProps} />
          </div>
        );
      })}
    </Fragment>
  );
}
