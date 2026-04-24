import { AppEditor } from "../AppEditor";
import type { AppEditorPdfProps } from "./domain/editor-pdf.types";
import styles from "./AppEditorPdf.module.css";

const joinClassNames = (...values: Array<string | undefined>) =>
  values.filter(Boolean).join(" ");

function resolveAriaLabel({
  ariaLabel,
  label,
}: {
  ariaLabel?: string;
  label?: AppEditorPdfProps["label"];
}) {
  if (ariaLabel?.trim()) {
    return ariaLabel;
  }

  if (typeof label === "string" && label.trim()) {
    return label;
  }

  return "Editor PDF";
}

export function AppEditorPdf(props: AppEditorPdfProps) {
  const { className, label, "aria-label": ariaLabel, ...rest } = props;
  const resolvedAriaLabel = resolveAriaLabel({ ariaLabel, label });

  return (
    <AppEditor
      {...rest}
      label={label}
      aria-label={resolvedAriaLabel}
      className={joinClassNames(styles.root, className)}
    />
  );
}
