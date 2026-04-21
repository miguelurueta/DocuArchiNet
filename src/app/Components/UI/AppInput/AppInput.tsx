import { forwardRef, useId } from "react";
import { Input as AntInput, Select } from "antd";
import type { ChangeEventHandler, ComponentProps, ReactNode } from "react";
import type { InputRef, SelectProps } from "antd";
import styles from "./AppInput.module.css";

type AntInputProps = ComponentProps<typeof AntInput>;
type AntSelectProps = SelectProps;

export type AppInputState = "default" | "error";

type AppInputBaseProps = {
  id?: string;
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  state?: AppInputState;
  className?: string;
  disabled?: boolean;
  "aria-describedby"?: string;
};

export type AppInputOption = {
  label: string;
  value: string | number;
};

export type AppInputSelectProps = Omit<AntSelectProps, "options" | "onChange" | "className"> &
  AppInputBaseProps & {
    type: "select";
    options: AppInputOption[];
    onChange?: (value: string | number | undefined) => void;
  };

export type AppInputCheckboxProps = AppInputBaseProps & {
  type: "checkbox";
  checked?: boolean;
  defaultChecked?: boolean;
  onChange?: ChangeEventHandler<HTMLInputElement>;
  name?: string;
  value?: string;
};

export type AppInputTextProps = Omit<
  AntInputProps,
  "suffix" | "status" | "size"
> &
  AppInputBaseProps & {
    type?: Exclude<AntInputProps["type"], "select">;
    options?: never;
  };

export type AppInputProps = AppInputSelectProps | AppInputCheckboxProps | AppInputTextProps;

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export const AppInput = forwardRef<InputRef, AppInputProps>(
  function AppInput(
    {
      id,
      label,
      helperText,
      error = false,
      state = "default",
      className,
      disabled = false,
      "aria-describedby": ariaDescribedBy,
      ...restProps
    },
    ref,
  ) {
    const generatedId = useId();
    const inputId = id ?? `app-input-${generatedId}`;
    const helperId = helperText ? `${inputId}-helper` : undefined;
    const describedBy = [ariaDescribedBy, helperId].filter(Boolean).join(" ") || undefined;
    const hasError = error || state === "error";
    const isSelect = (restProps as AppInputSelectProps).type === "select";
    const isCheckbox = (restProps as AppInputCheckboxProps).type === "checkbox";

    return (
      <div className={styles.field}>
        {label && !isCheckbox ? (
          <label className={styles.label} htmlFor={inputId}>
            {label}
          </label>
        ) : null}

        {isSelect ? (
          <Select
            {...(restProps as AppInputSelectProps)}
            id={inputId}
            disabled={disabled}
            aria-invalid={hasError}
            aria-describedby={describedBy}
            className={joinClasses(
              styles.input,
              hasError && styles.inputError,
              disabled && styles.inputDisabled,
              className,
            )}
            options={(restProps as AppInputSelectProps).options}
            onChange={(value) =>
              (restProps as AppInputSelectProps).onChange?.(value ?? undefined)
            }
          />
        ) : isCheckbox ? (
          <label className={joinClasses(styles.checkboxLabel, className)} htmlFor={inputId}>
            <input
              id={inputId}
              type="checkbox"
              name={(restProps as AppInputCheckboxProps).name}
              value={(restProps as AppInputCheckboxProps).value}
              checked={(restProps as AppInputCheckboxProps).checked}
              defaultChecked={(restProps as AppInputCheckboxProps).defaultChecked}
              onChange={(restProps as AppInputCheckboxProps).onChange}
              disabled={disabled}
              aria-invalid={hasError}
              aria-describedby={describedBy}
              className={joinClasses(
                styles.checkbox,
                hasError && styles.checkboxError,
                disabled && styles.checkboxDisabled,
              )}
            />
            {label ? <span className={styles.checkboxText}>{label}</span> : null}
          </label>
        ) : (
          <AntInput
            {...(restProps as AppInputTextProps)}
            id={inputId}
            ref={ref}
            disabled={disabled}
            aria-invalid={hasError}
            aria-describedby={describedBy}
            className={joinClasses(
              styles.input,
              hasError && styles.inputError,
              disabled && styles.inputDisabled,
              className,
            )}
          />
        )}

        {helperText ? (
          <div
            className={joinClasses(styles.helperText, hasError && styles.helperTextError)}
            id={helperId}
          >
            {helperText}
          </div>
        ) : null}
      </div>
    );
  },
);
