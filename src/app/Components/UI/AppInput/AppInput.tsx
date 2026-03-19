import { forwardRef, useId } from "react";
import { Input as AntInput } from "antd";
import type { ComponentProps, ReactNode } from "react";
import type { InputRef } from "antd";
import styles from "./AppInput.module.css";

type AntInputProps = ComponentProps<typeof AntInput>;

export type AppInputState = "default" | "error";

export type AppInputProps = Omit<
  AntInputProps,
  "prefix" | "suffix" | "status" | "size"
> & {
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  state?: AppInputState;
};

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

    return (
      <div className={styles.field}>
        {label ? (
          <label className={styles.label} htmlFor={inputId}>
            {label}
          </label>
        ) : null}

        <AntInput
          {...restProps}
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
