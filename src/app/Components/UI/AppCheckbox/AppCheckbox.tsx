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
  className?: string;
  onChange: (value: TValue[]) => void;
};

export type AppCheckboxCheckAllProps<TValue extends Primitive = string> = {
  options: AppCheckboxOption<TValue>[];
  value: TValue[];
  disabled?: boolean;
  size?: AppCheckboxSize;
  checkAllLabel?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  className?: string;
  name?: string;
  rules?: Rule[];
  onChange: (value: TValue[]) => void;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const getSelectableValues = <TValue extends Primitive>(
  options: AppCheckboxOption<TValue>[],
) => options.filter((option) => !option.disabled).map((option) => option.value);

const getCheckAllState = <TValue extends Primitive>(
  value: TValue[],
  options: AppCheckboxOption<TValue>[],
) => {
  const selectableValues = getSelectableValues(options);
  const selectedSelectableCount = selectableValues.filter((optionValue) =>
    value.includes(optionValue)
  ).length;
  const checkedAll = selectableValues.length > 0 && selectedSelectableCount === selectableValues.length;
  const indeterminate =
    selectedSelectableCount > 0 && selectedSelectableCount < selectableValues.length;

  return { selectableValues, checkedAll, indeterminate };
};

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

export function AppCheckboxGroup<TValue extends Primitive = string>({
  value,
  options,
  disabled = false,
  size = "md",
  direction = "vertical",
  label,
  helperText,
  error = false,
  name,
  className,
  onChange,
}: AppCheckboxGroupProps<TValue>) {
  const generatedId = useId();
  const groupId = `app-checkbox-group-${generatedId}`;
  const helperId = helperText ? `${groupId}-helper` : undefined;

  const handleOptionToggle = (optionValue: TValue, checked: boolean) => {
    if (checked) {
      onChange(value.includes(optionValue) ? value : [...value, optionValue]);
      return;
    }

    onChange(value.filter((currentValue) => currentValue !== optionValue));
  };

  return (
    <div className={joinClasses(styles.field, className)}>
      {label ? (
        <div className={styles.groupLabel} id={groupId}>
          {label}
        </div>
      ) : null}

      <div
        aria-describedby={helperId}
        aria-labelledby={label ? groupId : undefined}
        className={joinClasses(
          styles.group,
          direction === "horizontal" ? styles.groupHorizontal : styles.groupVertical,
          error && styles.groupError,
        )}
        role="group"
      >
        {options.map((option) => (
          <AppCheckbox
            key={String(option.value)}
            checked={value.includes(option.value)}
            className={styles.groupItem}
            disabled={disabled || option.disabled}
            label={option.label}
            name={name}
            onChange={(checked) => handleOptionToggle(option.value, checked)}
            size={size}
          />
        ))}
      </div>

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

export function AppCheckboxCheckAll<TValue extends Primitive = string>({
  options,
  value,
  disabled = false,
  size = "md",
  checkAllLabel = "Seleccionar todo",
  helperText,
  error = false,
  className,
  name,
  onChange,
}: AppCheckboxCheckAllProps<TValue>) {
  const { selectableValues, checkedAll, indeterminate } = getCheckAllState(value, options);

  const handleCheckAll = (checked: boolean) => {
    onChange(checked ? selectableValues : []);
  };

  return (
    <div className={joinClasses(styles.checkAllWrapper, className)}>
      <AppCheckbox
        checked={checkedAll}
        className={styles.checkAllMaster}
        disabled={disabled || selectableValues.length === 0}
        error={error}
        helperText={helperText}
        indeterminate={indeterminate}
        label={checkAllLabel}
        name={name}
        onChange={handleCheckAll}
        size={size}
      />

      <AppCheckboxGroup
        className={styles.checkAllGroup}
        disabled={disabled}
        error={error}
        name={name}
        onChange={onChange}
        options={options}
        size={size}
        value={value}
      />
    </div>
  );
}
