import { Empty, Select } from "antd";
import type { ReactNode } from "react";
import { forwardRef, useEffect, useId, useMemo, useRef, useState } from "react";
import type { BaseSelectRef } from "rc-select";
import styles from "./AppInputSelect.module.css";

type Primitive = string | number;

export type AppInputSelectSize = "sm" | "md" | "lg";
export type AppInputSelectState = "default" | "error";

export type AppInputSelectOption<TValue extends Primitive = string> = {
  label: ReactNode;
  value: TValue;
  disabled?: boolean;
  meta?: Record<string, unknown>;
};

export type AppInputSelectFetchResult<TValue extends Primitive = string> = {
  options: AppInputSelectOption<TValue>[];
  total?: number;
};

export type AppInputSelectBackendItem = {
  id: Primitive;
  nombre: ReactNode;
  activo?: boolean;
};

export type AppInputSelectMode = "single" | "multiple" | "tags";

export type AppInputSelectProps<TValue extends Primitive = string> = {
  id?: string;
  value?: TValue | TValue[];
  defaultValue?: TValue | TValue[];
  options?: AppInputSelectOption<TValue>[];
  placeholder?: string;
  size?: AppInputSelectSize;
  mode?: AppInputSelectMode;
  disabled?: boolean;
  loading?: boolean;
  allowClear?: boolean;
  searchable?: boolean;
  noDataText?: ReactNode;
  onChange?: (
    value: TValue | TValue[],
    option?: AppInputSelectOption<TValue> | AppInputSelectOption<TValue>[],
  ) => void;
  onSearch?: (query: string) => void;
  fetchOptions?: (query?: string) => Promise<AppInputSelectFetchResult<TValue>>;
  className?: string;
  status?: "error" | "warning";
  label?: ReactNode;
  helperText?: ReactNode;
  error?: boolean;
  state?: AppInputSelectState;
  "aria-label"?: string;
  "aria-labelledby"?: string;
  "aria-describedby"?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const SIZE_MAP: Record<AppInputSelectSize, "small" | "middle" | "large"> = {
  sm: "small",
  md: "middle",
  lg: "large",
};

const MODE_MAP: Record<AppInputSelectMode, undefined | "multiple" | "tags"> = {
  single: undefined,
  multiple: "multiple",
  tags: "tags",
};

export const toAppInputSelectOption = <TValue extends Primitive = Primitive>(
  item: AppInputSelectBackendItem,
) =>
  ({
    label: item.nombre,
    value: item.id as TValue,
    disabled: item.activo === false,
  }) satisfies AppInputSelectOption<TValue>;

export const AppInputSelect = forwardRef<BaseSelectRef, AppInputSelectProps>(
  function AppInputSelect(
    {
      id,
      value,
      defaultValue,
      options = [],
      placeholder,
      size = "md",
      mode = "single",
      disabled = false,
      loading = false,
      allowClear = false,
      searchable = false,
      noDataText,
      onChange,
      onSearch,
      fetchOptions,
      className,
      status,
      label,
      helperText,
      error = false,
      state = "default",
      "aria-label": ariaLabel,
      "aria-labelledby": ariaLabelledBy,
      "aria-describedby": ariaDescribedBy,
    },
    ref,
  ) {
    const generatedId = useId();
    const selectId = id ?? `app-input-select-${generatedId}`;
    const helperId = helperText ? `${selectId}-helper` : undefined;
    const describedBy = [ariaDescribedBy, helperId].filter(Boolean).join(" ") || undefined;
    const hasError = error || state === "error";
    const [remoteOptions, setRemoteOptions] = useState<AppInputSelectOption[]>([]);
    const [remoteLoading, setRemoteLoading] = useState(false);
    const [hasLoadedRemotely, setHasLoadedRemotely] = useState(false);
    const [remoteError, setRemoteError] = useState(false);
    const requestIdRef = useRef(0);
    const usesRemoteOptions = Boolean(fetchOptions);

    const executeRemoteFetch = async (query?: string) => {
      if (!fetchOptions) {
        return;
      }

      const requestId = requestIdRef.current + 1;
      requestIdRef.current = requestId;
      setRemoteLoading(true);
      setRemoteError(false);

      try {
        const result = await fetchOptions(query);
        if (requestId !== requestIdRef.current) {
          return;
        }
        setRemoteOptions(result.options ?? []);
        setHasLoadedRemotely(true);
      } catch {
        if (requestId !== requestIdRef.current) {
          return;
        }
        setRemoteOptions([]);
        setHasLoadedRemotely(true);
        setRemoteError(true);
      } finally {
        if (requestId === requestIdRef.current) {
          setRemoteLoading(false);
        }
      }
    };

    useEffect(() => {
      if (!fetchOptions || searchable) {
        return;
      }

      void executeRemoteFetch();
      // Intentionally tied to fetch function/search mode changes only.
      // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [fetchOptions, searchable]);

    const currentOptions = usesRemoteOptions ? remoteOptions : options;
    const effectiveLoading = loading || remoteLoading;
    const effectiveStatus = hasError ? "error" : status;
    const canSearch = searchable || usesRemoteOptions;

    const emptyNode = useMemo(() => {
      if (effectiveLoading) {
        return null;
      }

      if (noDataText) {
        return <div className={styles.emptyContent}>{noDataText}</div>;
      }

      if (remoteError) {
        return <div className={styles.emptyContent}>No fue posible cargar las opciones</div>;
      }

      return (
        <div className={styles.emptyContent}>
          <Empty description="Sin datos" image={Empty.PRESENTED_IMAGE_SIMPLE} />
        </div>
      );
    }, [effectiveLoading, noDataText, remoteError]);

    const handleSearch = (query: string) => {
      onSearch?.(query);

      if (fetchOptions) {
        void executeRemoteFetch(query);
      }
    };

    return (
      <div className={joinClasses(styles.field, className)}>
        {label ? (
          <label className={styles.label} htmlFor={selectId}>
            {label}
          </label>
        ) : null}

        <Select<TValue | TValue[], AppInputSelectOption<TValue>>
          ref={ref}
          allowClear={allowClear}
          aria-describedby={describedBy}
          aria-label={ariaLabel}
          aria-labelledby={ariaLabelledBy}
          className={joinClasses(
            styles.select,
            styles[size],
            styles[mode],
            hasError && styles.selectError,
            remoteError && styles.selectWarning,
            disabled && styles.selectDisabled,
          )}
          classNames={{ popup: { root: styles.dropdown } }}
          disabled={disabled}
          id={selectId}
          loading={effectiveLoading}
          maxTagCount={mode === "single" ? undefined : "responsive"}
          mode={MODE_MAP[mode]}
          notFoundContent={emptyNode}
          onChange={(nextValue, nextOption) => {
            onChange?.(
              nextValue as TValue | TValue[],
              nextOption as AppInputSelectOption<TValue> | AppInputSelectOption<TValue>[],
            );
          }}
          onOpenChange={(open) => {
            if (
              open &&
              fetchOptions &&
              !searchable &&
              !hasLoadedRemotely &&
              !effectiveLoading
            ) {
              void executeRemoteFetch();
            }
          }}
          onSearch={canSearch ? handleSearch : undefined}
          options={currentOptions}
          placeholder={placeholder}
          showSearch={canSearch}
          size={SIZE_MAP[size]}
          status={effectiveStatus}
          value={value}
          defaultValue={defaultValue}
          filterOption={usesRemoteOptions ? false : undefined}
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
