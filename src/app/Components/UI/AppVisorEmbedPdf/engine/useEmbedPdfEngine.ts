import { usePdfiumEngine } from "@embedpdf/engines/react";
import type { PdfEngine } from "@embedpdf/engines/pdfium";

export type EmbedPdfEngineState =
  | { status: "loading" }
  | { status: "error"; error: unknown }
  | { status: "ready"; engine: PdfEngine<Blob> };

export function useEmbedPdfEngine(): EmbedPdfEngineState {
  const { engine, isLoading, error } = usePdfiumEngine();

  if (isLoading) {
    return { status: "loading" };
  }
  if (error) {
    return { status: "error", error };
  }
  if (engine) {
    return { status: "ready", engine: engine as PdfEngine<Blob> };
  }
  return { status: "error", error: new Error("Pdfium engine no disponible") };
}
