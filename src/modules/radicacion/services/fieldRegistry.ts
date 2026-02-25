import {
  CheckboxField,
  SelectField,
  TextAreaField,
  TextField,
} from "../components/FieldComponents";

export const radicacionFieldRegistry = {
  text: TextField,
  number: TextField,
  date: TextField,
  textarea: TextAreaField,
  select: SelectField,
  checkbox: CheckboxField,
} as const;
