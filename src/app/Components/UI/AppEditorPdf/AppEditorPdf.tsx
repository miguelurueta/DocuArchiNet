import { AppEditor } from "../AppEditor";
import type { AppEditorPdfProps } from "./domain/editor-pdf.types";
import styles from "./AppEditorPdf.module.css";

const joinClassNames = (...values: Array<string | undefined>) =>
  values.filter(Boolean).join(" ");

export function AppEditorPdf(props: AppEditorPdfProps) {
  const { className, ...rest } = props;

  return (
    <AppEditor
      {...rest}
      className={joinClassNames(styles.root, className)}
    />
  );
}
