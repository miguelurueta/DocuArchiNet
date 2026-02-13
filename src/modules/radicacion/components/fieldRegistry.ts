import {
  CheckboxField,
  SelectField,
  TextAreaField,
  TextField,
} from "./FieldComponents";

export const radicacionFieldRegistry = {
  text: TextField,
  number: TextField,
  date: TextField,
  textarea: TextAreaField,
  select: SelectField,
  checkbox: CheckboxField,
} as const;
