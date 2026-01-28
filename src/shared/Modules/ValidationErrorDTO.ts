export interface ValidationErrorDTO {
  Field: string;
  Message: string;
  Type: string;
  AttemptedValue?: unknown;
}