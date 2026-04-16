import { Checkbox } from "antd";
import type { ComponentProps, ReactNode } from "react";
import { useId } from "react";
import type { CheckboxChangeEvent } from "antd/es/checkbox";
import type { Rule } from "antd/es/form";
import styles from "./AppCheckbox.module.css";

type Primitive = string | number;
type AntCheckboxProps = ComponentProps<typeof Checkbox>;

export type AppCheckboxSize = "sm" | "md" | "lg";

export type AppCheckboxOption<TValue extends Primitive = string> = {
  label: ReactNode;
  value: TValue;
  disabled?: boolean;
  meta?: Record<string, unknown>;
};

export type AppCheckboxProps = Omit<
  AntCheckboxProps,
  "children" | "onChange" | "className"
> & {
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  size?: AppCheckboxSize;
  className?: string;
  onChange?: (checked: boolean, event: CheckboxChangeEvent) => void;
  "aria-label"?: string;
  "aria-labelledby"?: string;
  "aria-describedby"?: string;
};

export type AppCheckboxGroupProps<TValue extends Primitive = string> = {
  value: TValue[];
  options: AppCheckboxOption<TValue>[];
  disabled?: boolean;
  size?: AppCheckboxSize;
  direction?: "vertical" | "horizontal";
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  name?: string;
  rules?: Rule[];
  onChange: (value: TValue[]) => void;
};

export type AppCheckboxCheckAllProps<TValue extends Primitive = string> = {
  options: AppCheckboxOption<TValue>[];
  value: TValue[];
  disabled?: boolean;
  size?: AppCheckboxSize;
  checkAllLabel?: ReactNode;
  name?: string;
  rules?: Rule[];
  onChange: (value: TValue[]) => void;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export function AppCheckbox({
  label,
  helperText,
  error = false,
  size = "md",
  className,
  disabled = false,
  onChange,
  "aria-describedby": ariaDescribedBy,
  ...restProps
}: AppCheckboxProps) {
  const generatedId = useId();
  const checkboxId = restProps.id ?? `app-checkbox-${generatedId}`;
  const helperId = helperText ? `${checkboxId}-helper` : undefined;
  const describedBy = [ariaDescribedBy, helperId].filter(Boolean).join(" ") || undefined;

  return (
    <div className={joinClasses(styles.field, className)}>
      <Checkbox
        {...restProps}
        id={checkboxId}
        aria-describedby={describedBy}
        className={joinClasses(
          styles.checkbox,
          styles[`size${size.toUpperCase()}` as keyof typeof styles],
          error && styles.error,
          disabled && styles.disabled,
        )}
        disabled={disabled}
        onChange={(event) => onChange?.(event.target.checked, event)}
      >
        {label ? <span className={styles.label}>{label}</span> : null}
      </Checkbox>

      {helperText ? (
        <div
          className={joinClasses(styles.helperText, error && styles.helperTextError)}
          id={helperId}
        >
          {helperText}
        </div>
      ) : null}
    </div>
  );
}
