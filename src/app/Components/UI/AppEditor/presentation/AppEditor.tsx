import { useId } from "react";
import type { CSSProperties } from "react";
import type { AppEditorProps } from "../domain/editor.types";
import { useAppEditor } from "../application/useAppEditor";
import { TiptapEditorContent } from "../infrastructure/TiptapEditorContent";
import { AppEditorToolbar } from "./AppEditorToolbar";
import styles from "../AppEditor.module.css";

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

function buildAriaLabel({
  "aria-label": ariaLabel,
  label,
  title,
}: Pick<AppEditorProps, "aria-label" | "label" | "title">) {
  if (ariaLabel) {
    return ariaLabel;
  }

  if (typeof label === "string" && label.trim()) {
    return label;
  }

  if (typeof title === "string" && title.trim()) {
    return title;
  }

  return "Editor enriquecido";
}

export function AppEditor({
  value,
  defaultValue,
  onChange,
  placeholder,
  disabled = false,
  readOnly = false,
  label,
  error,
  helperText,
  className,
  title,
  description,
  headerActions,
  surfaceClassName,
  minHeight = 280,
  "aria-label": ariaLabel,
}: AppEditorProps) {
  const fieldId = useId();
  const labelId = label ? `${fieldId}-label` : undefined;
  const helperId = helperText ? `${fieldId}-helper` : undefined;
  const errorId = error ? `${fieldId}-error` : undefined;
  const describedBy = [errorId, helperId].filter(Boolean).join(" ") || undefined;
  const { editor, isEditable } = useAppEditor({
    value,
    defaultValue,
    onChange,
    placeholder,
    disabled,
    readOnly,
  });

  return (
    <section
      className={joinClasses(styles.editor, className)}
      data-disabled={disabled}
      data-readonly={readOnly}
      data-error={Boolean(error)}
    >
      {title || description || headerActions ? (
        <header className={styles.header}>
          <div className={styles.headerContent}>
            {title ? <h2 className={styles.title}>{title}</h2> : null}
            {description ? <p className={styles.description}>{description}</p> : null}
          </div>
          {headerActions ? <div className={styles.headerActions}>{headerActions}</div> : null}
        </header>
      ) : null}

      {label ? (
        <label id={labelId} className={styles.label}>
          {label}
        </label>
      ) : null}

      <div className={styles.frame}>
        <AppEditorToolbar editor={editor} disabled={!isEditable} />
        <div
          className={joinClasses(
            styles.surface,
            surfaceClassName,
            Boolean(error) && styles.surfaceError,
          )}
          style={{ "--app-editor-min-height": typeof minHeight === "number" ? `${minHeight}px` : minHeight } as CSSProperties}
        >
          <TiptapEditorContent
            editor={editor}
            className={styles.editorContent}
            aria-labelledby={labelId}
            aria-label={buildAriaLabel({ "aria-label": ariaLabel, label, title })}
            aria-describedby={describedBy}
            aria-invalid={Boolean(error)}
          />
        </div>
      </div>

      {error ? (
        <p id={errorId} className={styles.errorText}>
          {error}
        </p>
      ) : null}

      {helperText ? (
        <p id={helperId} className={styles.helperText}>
          {helperText}
        </p>
      ) : null}
    </section>
  );
}
