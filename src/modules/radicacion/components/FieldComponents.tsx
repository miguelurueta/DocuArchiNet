import type { ChangeEventHandler } from "react";
import type { RadicacionFieldConfig } from "../services/radicacionMetadataMapper";

export interface RadicacionFieldComponentProps {
  field: RadicacionFieldConfig;
  value: string;
  onChange: ChangeEventHandler<
    HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
  >;
}

export function TextField({
  field,
  value,
  onChange,
}: RadicacionFieldComponentProps) {
  return (
    <input
      id={field.name}
      name={field.name}
      type={field.type}
      value={value}
      onChange={onChange}
      placeholder={field.placeholder}
      required={field.required}
    />
  );
}

export function TextAreaField({
  field,
  value,
  onChange,
}: RadicacionFieldComponentProps) {
  return (
    <textarea
      id={field.name}
      name={field.name}
      value={value}
      onChange={onChange}
      placeholder={field.placeholder}
      required={field.required}
    />
  );
}

export function SelectField({
  field,
  value,
  onChange,
}: RadicacionFieldComponentProps) {
  return (
    <select id={field.name} name={field.name} value={value} onChange={onChange}>
      <option value="">Seleccione...</option>
      {field.options.map((option) => (
        <option key={`${field.name}-${option}`} value={option}>
          {option}
        </option>
      ))}
    </select>
  );
}

export function CheckboxField({
  field,
  value,
  onChange,
}: RadicacionFieldComponentProps) {
  return (
    <input
      id={field.name}
      name={field.name}
      type="checkbox"
      checked={value === "true"}
      onChange={onChange}
      required={field.required}
    />
  );
}
