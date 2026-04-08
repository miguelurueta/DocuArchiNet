import {
  CloseCircleFilled,
  LoadingOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import { AutoComplete, Input as AntInput } from "antd";
import type { InputRef } from "antd";
import type { ComponentProps, ReactNode } from "react";
import {
  forwardRef,
  useEffect,
  useId,
  useMemo,
  useRef,
  useState,
} from "react";
import styles from "./AppInputSearch.module.css";

type AntInputProps = ComponentProps<typeof AntInput>;

export type AppInputSearchSize = "sm" | "md" | "lg";
export type AppInputSearchState = "default" | "error";

export type AppInputSearchOption = {
  value: string;
  label?: string;
};

export type AppInputSearchProps = Omit<
  AntInputProps,
  | "className"
  | "defaultValue"
  | "onBlur"
  | "onChange"
  | "onFocus"
  | "prefix"
  | "size"
  | "status"
  | "suffix"
  | "value"
> & {
  value?: string;
  defaultValue?: string;
  placeholder?: string;
  disabled?: boolean;
  autoFocus?: boolean;
  debounceMs?: number;
  minLength?: number;
  loading?: boolean;
  clearOnEscape?: boolean;
  options?: AppInputSearchOption[];
  onChange?: (value: string) => void;
  onSearch?: (value: string) => void;
  onClear?: () => void;
  onFocus?: () => void;
  onBlur?: () => void;
  size?: AppInputSearchSize;
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  state?: AppInputSearchState;
  className?: string;
  "aria-label"?: string;
  "aria-labelledby"?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const canRunSearch = (value: string, minLength?: number) =>
  minLength === undefined || value.length >= minLength;

export const AppInputSearch = forwardRef<InputRef, AppInputSearchProps>(
  function AppInputSearch(
    {
      id,
      value,
      defaultValue = "",
      placeholder,
      disabled = false,
      autoFocus,
      debounceMs = 0,
      minLength,
      loading = false,
      clearOnEscape = false,
      options,
      onChange,
      onSearch,
      onClear,
      onFocus,
      onBlur,
      size = "md",
      label,
      helperText,
      error = false,
      state = "default",
      className,
      "aria-describedby": ariaDescribedBy,
      "aria-label": ariaLabel,
      "aria-labelledby": ariaLabelledBy,
      onKeyDown,
      ...inputProps
    },
    ref,
  ) {
    const generatedId = useId();
    const inputId = id ?? `app-input-search-${generatedId}`;
    const helperId = helperText ? `${inputId}-helper` : undefined;
    const describedBy =
      [ariaDescribedBy, helperId].filter(Boolean).join(" ") || undefined;
    const isControlled = value !== undefined;
    const [internalValue, setInternalValue] = useState(defaultValue);
    const currentValue = isControlled ? value : internalValue;
    const debounceRef = useRef<ReturnType<typeof setTimeout> | null>(null);
    const latestValueRef = useRef(currentValue);
    const hasError = error || state === "error";
    const showClear = currentValue.length > 0 && !disabled;

    const autocompleteOptions = useMemo(
      () =>
        options?.map((option) => ({
          value: option.value,
          label: option.label ?? option.value,
        })),
      [options],
    );

    const clearDebounce = () => {
      if (debounceRef.current) {
        clearTimeout(debounceRef.current);
        debounceRef.current = null;
      }
    };

    useEffect(
      () => () => {
        if (debounceRef.current) {
          clearTimeout(debounceRef.current);
        }
      },
      [],
    );

    useEffect(() => {
      latestValueRef.current = currentValue;
    }, [currentValue]);

    const updateValue = (nextValue: string) => {
      latestValueRef.current = nextValue;

      if (!isControlled) {
        setInternalValue(nextValue);
      }

      onChange?.(nextValue);
    };

    const runImmediateSearch = (searchValue: string) => {
      clearDebounce();

      if (!disabled && canRunSearch(searchValue, minLength)) {
        onSearch?.(searchValue);
      }
    };

    const scheduleSearch = (searchValue: string) => {
      clearDebounce();

      if (!onSearch || disabled || !debounceMs || debounceMs <= 0) {
        return;
      }

      if (!canRunSearch(searchValue, minLength)) {
        return;
      }

      debounceRef.current = setTimeout(() => {
        debounceRef.current = null;
        onSearch(searchValue);
      }, debounceMs);
    };

    const handleChange = (nextValue: string) => {
      updateValue(nextValue);
      scheduleSearch(nextValue);
    };

    const handleSelect = (selectedValue: string) => {
      if (latestValueRef.current !== selectedValue) {
        updateValue(selectedValue);
      }
      runImmediateSearch(selectedValue);
    };

    const handleClear = () => {
      if (disabled) {
        return;
      }

      clearDebounce();
      updateValue("");
      onClear?.();
    };

    const handleKeyDown: AntInputProps["onKeyDown"] = (event) => {
      onKeyDown?.(event);

      if (event.defaultPrevented) {
        return;
      }

      if (event.key === "Enter") {
        runImmediateSearch(latestValueRef.current);
        return;
      }

      if (event.key === "Escape" && clearOnEscape && currentValue.length > 0) {
        handleClear();
      }
    };

    const handleFocus = () => {
      onFocus?.();
    };

    const handleBlur = () => {
      onBlur?.();
    };

    const suffix = (
      <span className={styles.suffix}>
        {loading && !disabled ? (
          <span className={styles.loadingIcon} aria-hidden="true">
            <LoadingOutlined />
          </span>
        ) : null}
        {showClear ? (
          <button
            aria-label="Limpiar"
            className={styles.iconButton}
            onClick={handleClear}
            tabIndex={0}
            type="button"
          >
            <CloseCircleFilled />
          </button>
        ) : null}
        <button
          aria-label="Buscar"
          className={styles.iconButton}
          disabled={disabled}
          onClick={() => runImmediateSearch(latestValueRef.current)}
          type="button"
        >
          <SearchOutlined />
        </button>
      </span>
    );

    return (
      <div className={joinClasses(styles.field, className)}>
        {label ? (
          <label className={styles.label} htmlFor={inputId}>
            {label}
          </label>
        ) : null}

        <AutoComplete
          className={styles.autoComplete}
          disabled={disabled}
          onChange={handleChange}
          onSelect={handleSelect}
          options={autocompleteOptions}
          value={currentValue}
        >
          <AntInput
            {...inputProps}
            aria-describedby={describedBy}
            aria-invalid={hasError}
            aria-label={ariaLabel}
            aria-labelledby={ariaLabelledBy}
            autoFocus={autoFocus}
            className={joinClasses(
              styles.input,
              styles[size],
              hasError && styles.inputError,
              disabled && styles.inputDisabled,
            )}
            disabled={disabled}
            id={inputId}
            onBlur={handleBlur}
            onFocus={handleFocus}
            onKeyDown={handleKeyDown}
            placeholder={placeholder}
            ref={ref}
            suffix={suffix}
          />
        </AutoComplete>

        {helperText ? (
          <div
            className={joinClasses(
              styles.helperText,
              hasError && styles.helperTextError,
            )}
            id={helperId}
          >
            {helperText}
          </div>
        ) : null}
      </div>
    );
  },
);
