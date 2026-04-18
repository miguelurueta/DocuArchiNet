import { CloseOutlined, DeleteOutlined } from "@ant-design/icons";
import { AutoComplete, Input, Spin, Tag } from "antd";
import type { KeyboardEvent, ReactNode } from "react";
import { useEffect, useId, useMemo, useRef, useState } from "react";
import styles from "./AppInputTags.module.css";

export type AppInputTagsMode = "single" | "multiple";
export type AppInputTagsSize = "sm" | "md" | "lg";
export type AppInputTagsState = "default" | "error";
export type AppInputTagsVariant = "default" | "email";

export type AppInputTagsOption = {
  label: string;
  value: string;
  id?: number;
  meta?: Record<string, unknown>;
};

export type AppInputTagsProps = {
  name?: string;
  label?: ReactNode;
  value?: string[];
  defaultValue?: string[];
  variant?: AppInputTagsVariant;
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

const normalizeEmailTag = (value: string) => normalizeTag(value).toLocaleLowerCase();

const splitEmailTags = (value: string) =>
  value
    .split(/[,\s;]+/g)
    .map((candidate) => candidate.trim())
    .filter(Boolean);

// Pragmatic validator (not full RFC 5322). Keeps UX predictable.
const isValidEmailTag = (value: string) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value);

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
  variant = "default",
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
  const [internalTags, setInternalTags] = useState<string[]>(defaultValue);
  const [inputValue, setInputValue] = useState("");
  const [isFocused, setIsFocused] = useState(false);
  const [emailValidationMessage, setEmailValidationMessage] = useState<string | null>(
    null,
  );
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);
  const didSelectAutocompleteOptionRef = useRef(false);

  const resolvedHelperText = emailValidationMessage ?? helperText;
  const helperId = resolvedHelperText ? `${inputId}-helper` : undefined;

  const isControlled = Array.isArray(value);
  const visibleTags = isControlled ? value : internalTags;
  const isDisabled = disabled || selectDisabled;
  const hasError = error || state === "error" || Boolean(emailValidationMessage);
  const canRemoveAll = visibleTags.length > 0 && !isDisabled;

  const autocompleteOptions = useMemo(
    () =>
      options.map((option) => ({
        label: option.label,
        value: option.value,
      })),
    [options],
  );

  const filteredAutocompleteOptions = useMemo(() => {
    const normalizedQuery = inputValue.trim().toLocaleLowerCase();

    if (!normalizedQuery) {
      return autocompleteOptions;
    }

    return autocompleteOptions.filter((option) => {
      const normalizedLabel = String(option.label).toLocaleLowerCase();
      const normalizedValue = option.value.toLocaleLowerCase();

      return (
        normalizedLabel.includes(normalizedQuery) ||
        normalizedValue.includes(normalizedQuery)
      );
    });
  }, [autocompleteOptions, inputValue]);

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

  const addEmailTags = (rawValue: string) => {
    if (isDisabled) {
      return;
    }

    const candidates = splitEmailTags(rawValue);

    if (candidates.length === 0) {
      return;
    }

    cancelPendingSearch();

    let hasInvalid = false;
    let didAdd = false;
    let nextTags = visibleTags;

    for (const candidate of candidates) {
      const nextEmail = normalizeEmailTag(candidate);

      if (!nextEmail) {
        continue;
      }

      if (!isValidEmailTag(nextEmail)) {
        hasInvalid = true;
        continue;
      }

      nextTags = getNextTags(nextTags, nextEmail, mode);
      onAddTag?.(nextEmail);
      didAdd = true;
    }

    updateUncontrolledTags(nextTags);

    if (didAdd) {
      setInputValue("");
    }

    if (hasInvalid) {
      setEmailValidationMessage("Correo inválido");
      return;
    }

    setEmailValidationMessage(null);
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
    didSelectAutocompleteOptionRef.current = false;
    if (emailValidationMessage) {
      setEmailValidationMessage(null);
    }
    setInputValue(nextValue);
    scheduleSearch(nextValue.trim());
  };

  const handleKeyDown = (event: KeyboardEvent<HTMLInputElement>) => {
    if (event.key === "Enter") {
      event.preventDefault();
      cancelPendingSearch();
      dispatchSearch(inputValue.trim());
      queueMicrotask(() => {
        if (didSelectAutocompleteOptionRef.current) {
          didSelectAutocompleteOptionRef.current = false;
          return;
        }

        const normalizedInputValue = inputValue.trim().toLocaleLowerCase();
        const matchingOption = filteredAutocompleteOptions.find((option) => {
          const normalizedLabel = String(option.label).trim().toLocaleLowerCase();
          const normalizedValue = option.value.trim().toLocaleLowerCase();

          return (
            normalizedLabel === normalizedInputValue ||
            normalizedValue === normalizedInputValue
          );
        });

        if (matchingOption) {
          if (variant === "email") {
            addEmailTags(matchingOption.value);
          } else {
            addTag(matchingOption.value);
          }
          return;
        }

        if (variant === "email") {
          addEmailTags(inputValue);
        } else {
          addTag(inputValue);
        }
      });
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

      <div className={joinClasses(styles.controlRow, canRemoveAll && styles.controlRowWithRemoveAll)}>
        <AutoComplete
          className={joinClasses(
            styles.autoComplete,
            sizeClassBySize[size],
            hasError && styles.inputError,
            isDisabled && styles.inputDisabled,
          )}
          popupClassName={styles.popup}
          getPopupContainer={(triggerNode) =>
            triggerNode.closest(".ant-modal") ?? triggerNode.parentElement ?? document.body
          }
          disabled={isDisabled}
          open={inputValue.trim().length > 0 && canSearch(inputValue.trim())}
          onChange={handleInputChange}
          onSelect={(selectedValue) => {
            didSelectAutocompleteOptionRef.current = true;
            if (variant === "email") {
              addEmailTags(selectedValue);
            } else {
              addTag(selectedValue);
            }
          }}
          options={filteredAutocompleteOptions}
          value={inputValue}
        >
          <Input
            aria-describedby={helperId}
            aria-invalid={hasError}
            aria-label={ariaLabel}
            aria-labelledby={ariaLabelledBy}
            data-ident={selectDataIdent}
            disabled={isDisabled}
            type={variant === "email" ? "email" : "text"}
            inputMode={variant === "email" ? "email" : undefined}
            autoComplete={variant === "email" ? "email" : undefined}
            autoCapitalize={variant === "email" ? "none" : undefined}
            autoCorrect={variant === "email" ? "off" : undefined}
            spellCheck={variant === "email" ? false : undefined}
            id={inputId}
            onBlur={() => setIsFocused(false)}
            onFocus={() => setIsFocused(true)}
            onKeyDown={handleKeyDown}
            placeholder={isFocused || visibleTags.length > 0 ? undefined : placeholder}
            prefix={
              visibleTags.length > 0 ? (
                <div
                  aria-label="Etiquetas seleccionadas"
                  className={styles.tagsInline}
                  role="list"
                >
                  {visibleTags.map((tag) => (
                    <Tag className={styles.tag} key={tag} role="listitem">
                      <span className={styles.tagText}>{tag}</span>
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
              ) : null
            }
            suffix={suffix}
          />
        </AutoComplete>

        {canRemoveAll ? (
          <button
            aria-label="Eliminar todos"
            className={joinClasses(styles.iconButton, styles.removeAllFloatingButton)}
            onClick={removeAllTags}
            type="button"
          >
            <DeleteOutlined />
          </button>
        ) : null}

        {toolbar ? <div className={styles.toolbar}>{toolbar.render()}</div> : null}
      </div>

      {resolvedHelperText ? (
        <div
          className={joinClasses(styles.helperText, hasError && styles.helperTextError)}
          id={helperId}
        >
          {resolvedHelperText}
        </div>
      ) : null}
    </div>
  );
}
