import { AppEditor } from "../AppEditor";
import type { AppEditorPdfProps } from "./domain/editor-pdf.types";

export function AppEditorPdf(props: AppEditorPdfProps) {
  return <AppEditor {...props} />;
}

