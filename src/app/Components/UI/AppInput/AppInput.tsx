import { forwardRef, useId } from "react";
import { Input as AntInput, Select } from "antd";
import type { ComponentProps, ReactNode } from "react";
import type { InputRef, SelectProps } from "antd";
import styles from "./AppInput.module.css";

type AntInputProps = ComponentProps<typeof AntInput>;
type AntSelectProps = SelectProps;

export type AppInputState = "default" | "error";

type AppInputBaseProps = {
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  state?: AppInputState;
  className?: string;
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

export type AppInputTextProps = Omit<
  AntInputProps,
  "prefix" | "suffix" | "status" | "size"
> &
  AppInputBaseProps & {
    type?: Exclude<AntInputProps["type"], "select">;
    options?: never;
  };

export type AppInputProps = AppInputSelectProps | AppInputTextProps;

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

    return (
      <div className={styles.field}>
        {label ? (
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
