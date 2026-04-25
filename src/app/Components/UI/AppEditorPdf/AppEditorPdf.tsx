import { AppEditor } from "../AppEditor";
import type { AppEditorPdfProps } from "./domain/editor-pdf.types";
import styles from "./AppEditorPdf.module.css";

const DEFAULT_PAGE_MARGINS = {
  top: 96,
  right: 72,
  bottom: 96,
  left: 72,
} as const;

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
  const {
    className,
    label,
    paginationMode = "visual",
    pageFormat = "A4",
    pageOrientation = "portrait",
    pageMargins,
    "aria-label": ariaLabel,
    ...rest
  } = props;

  const resolvedPageMargins = {
    ...DEFAULT_PAGE_MARGINS,
    ...pageMargins,
  };
  const resolvedAriaLabel = resolveAriaLabel({ ariaLabel, label });

  return (
    <AppEditor
      {...rest}
      label={label}
      paginationMode={paginationMode}
      pageFormat={pageFormat}
      pageOrientation={pageOrientation}
      pageMargins={resolvedPageMargins}
      aria-label={resolvedAriaLabel}
      className={joinClassNames(styles.root, className)}
    />
  );
}
