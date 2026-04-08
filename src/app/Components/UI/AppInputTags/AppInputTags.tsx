import {
  CloseOutlined,
  DeleteOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import { AutoComplete, Button, Input, Spin, Tag } from "antd";
import type { KeyboardEvent, ReactNode } from "react";
import { useEffect, useId, useMemo, useRef, useState } from "react";
import styles from "./AppInputTags.module.css";

export type AppInputTagsMode = "single" | "multiple";
export type AppInputTagsSize = "sm" | "md" | "lg";
export type AppInputTagsState = "default" | "error";

export type AppInputTagsOption = {
  label: string;
  value: string;
  id?: number;
};

export type AppInputTagsProps = {
  name?: string;
  label?: ReactNode;
  value?: string[];
  defaultValue?: string[];
  mode?: AppInputTagsMode;
  options?: AppInputTagsOption[];
  placeholder?: string;
  minLength?: number;
  debounceMs?: number;
  loading?: boolean;
  clearOnEscape?: boolean;
  disabled?: boolean;
  selectDisabled?: boolean;
  size?: AppInputTagsSize;
  error?: boolean;
  state?: AppInputTagsState;
  helperText?: ReactNode;
  className?: string;
  toolbar?: {
    render: () => ReactNode;
  };
  onAddTag?: (tag: string) => void;
  onRemoveTag?: (tag: string) => void;
  onRemoveAll?: () => void;
  onSearch?: (query: string) => void;
  abrirInformacion?: (id: number) => void;
  formItemDataIdent?: string;
  selectDataIdent?: string;
  "aria-label"?: string;
  "aria-labelledby"?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const sizeClassBySize: Record<AppInputTagsSize, string> = {
  sm: styles.sizeSM,
  md: styles.sizeMD,
  lg: styles.sizeLG,
};

const normalizeTag = (value: string) => value.trim();

const getNextTags = (
  currentTags: string[],
  nextTag: string,
  mode: AppInputTagsMode,
) => {
  if (mode === "single") {
    return [nextTag];
  }

  return currentTags.includes(nextTag) ? currentTags : [...currentTags, nextTag];
};

export function AppInputTags({
  label,
  value,
  defaultValue = [],
  mode = "multiple",
  options = [],
  placeholder,
  minLength,
  debounceMs = 0,
  loading = false,
  clearOnEscape = false,
  disabled = false,
  selectDisabled = false,
  size = "md",
  error = false,
  state = "default",
  helperText,
  className,
  toolbar,
  onAddTag,
  onRemoveTag,
  onRemoveAll,
  onSearch,
  formItemDataIdent,
  selectDataIdent,
  "aria-label": ariaLabel,
  "aria-labelledby": ariaLabelledBy,
}: AppInputTagsProps) {
  const generatedId = useId();
  const inputId = `app-input-tags-${generatedId}`;
  const helperId = helperText ? `${inputId}-helper` : undefined;
  const [internalTags, setInternalTags] = useState<string[]>(defaultValue);
  const [inputValue, setInputValue] = useState("");
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const isControlled = Array.isArray(value);
  const visibleTags = isControlled ? value : internalTags;
  const isDisabled = disabled || selectDisabled;
  const hasError = error || state === "error";
  const canClear = inputValue.length > 0 && !isDisabled;
  const canRemoveAll = visibleTags.length > 0 && !isDisabled;

  const autocompleteOptions = useMemo(
    () =>
      options.map((option) => ({
        label: option.label,
        value: option.value,
      })),
    [options],
  );

  useEffect(
    () => () => {
      if (timerRef.current) {
        clearTimeout(timerRef.current);
      }
    },
    [],
  );

  const canSearch = (query: string) =>
    minLength === undefined || query.length >= minLength;

  const cancelPendingSearch = () => {
    if (timerRef.current) {
      clearTimeout(timerRef.current);
      timerRef.current = null;
    }
  };

  const dispatchSearch = (query: string) => {
    if (canSearch(query)) {
      onSearch?.(query);
    }
  };

  const scheduleSearch = (query: string) => {
    cancelPendingSearch();

    if (!canSearch(query)) {
      return;
    }

    if (!debounceMs) {
      onSearch?.(query);
      return;
    }

    timerRef.current = setTimeout(() => {
      timerRef.current = null;
      onSearch?.(query);
    }, debounceMs);
  };

  const updateUncontrolledTags = (nextTags: string[]) => {
    if (!isControlled) {
      setInternalTags(nextTags);
    }
  };

  const addTag = (rawTag: string) => {
    if (isDisabled) {
      return;
    }

    const nextTag = normalizeTag(rawTag);

    if (!nextTag) {
      return;
    }

    cancelPendingSearch();
    updateUncontrolledTags(getNextTags(visibleTags, nextTag, mode));
    onAddTag?.(nextTag);
    setInputValue("");
  };

  const removeTag = (tag: string) => {
    if (isDisabled) {
      return;
    }

    updateUncontrolledTags(visibleTags.filter((currentTag) => currentTag !== tag));
    onRemoveTag?.(tag);
  };

  const removeAllTags = () => {
    if (isDisabled) {
      return;
    }

    updateUncontrolledTags([]);
    onRemoveAll?.();
  };

  const clearInput = () => {
    if (isDisabled) {
      return;
    }

    cancelPendingSearch();
    setInputValue("");
  };

  const handleInputChange = (nextValue: string) => {
    setInputValue(nextValue);
    scheduleSearch(nextValue.trim());
  };

  const handleImmediateSearch = () => {
    cancelPendingSearch();
    dispatchSearch(inputValue.trim());
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      event.preventDefault();
      cancelPendingSearch();
      dispatchSearch(inputValue.trim());
      addTag(inputValue);
      return;
    }

    if (event.key === "Escape" && clearOnEscape) {
      event.preventDefault();
      clearInput();
    }
  };

  const suffix = (
    <span className={styles.suffixActions}>
      {loading ? <Spin aria-label="Cargando" size="small" /> : null}
      {canClear ? (
        <button
          aria-label="Limpiar"
          className={styles.iconButton}
          onClick={clearInput}
          type="button"
        >
          <CloseOutlined />
        </button>
      ) : null}
      <button
        aria-label="Buscar"
        className={styles.iconButton}
        disabled={isDisabled}
        onClick={handleImmediateSearch}
        type="button"
      >
        <SearchOutlined />
      </button>
    </span>
  );

  return (
    <div
      className={joinClasses(styles.field, className)}
      data-ident={formItemDataIdent}
    >
      {label ? (
        <label className={styles.label} htmlFor={inputId}>
          {label}
        </label>
      ) : null}

      <div className={styles.controlRow}>
        <AutoComplete
          className={joinClasses(
            styles.autoComplete,
            sizeClassBySize[size],
            hasError && styles.inputError,
            isDisabled && styles.inputDisabled,
          )}
          disabled={isDisabled}
          onChange={handleInputChange}
          onSelect={(selectedValue) => addTag(selectedValue)}
          options={autocompleteOptions}
          value={inputValue}
        >
          <Input
            aria-describedby={helperId}
            aria-invalid={hasError}
            aria-label={ariaLabel}
            aria-labelledby={ariaLabelledBy}
            data-ident={selectDataIdent}
            disabled={isDisabled}
            id={inputId}
            onKeyDown={handleKeyDown}
            placeholder={placeholder}
            suffix={suffix}
          />
        </AutoComplete>

        {toolbar ? <div className={styles.toolbar}>{toolbar.render()}</div> : null}

        {canRemoveAll ? (
          <Button
            aria-label="Eliminar todos"
            className={styles.removeAllButton}
            icon={<DeleteOutlined />}
            onClick={removeAllTags}
            type="text"
          />
        ) : null}
      </div>

      {visibleTags.length > 0 ? (
        <div aria-label="Etiquetas seleccionadas" className={styles.tags} role="list">
          {visibleTags.map((tag) => (
            <Tag className={styles.tag} key={tag} role="listitem">
              <span>{tag}</span>
              {!isDisabled ? (
                <button
                  aria-label={`Eliminar ${tag}`}
                  className={styles.tagRemoveButton}
                  onClick={() => removeTag(tag)}
                  type="button"
                >
                  <CloseOutlined />
                </button>
              ) : null}
            </Tag>
          ))}
        </div>
      ) : null}

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
}
