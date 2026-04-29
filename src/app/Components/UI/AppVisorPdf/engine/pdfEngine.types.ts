import type { AppVisorPdfInput } from "../domain/visorPdf.types";

export type PdfLoadResult = {
  pageCount: number;
  fingerprint?: string;
};

export type PdfRenderRequest = {
  pageNumber: number;
  zoom: number;
};

export type PdfRenderResult = {
  width: number;
  height: number;
};

export interface PdfEngine {
  load: (input: AppVisorPdfInput) => Promise<PdfLoadResult>;
  renderPage: (
    req: PdfRenderRequest,
    canvas: HTMLCanvasElement,
    signal?: AbortSignal,
  ) => Promise<PdfRenderResult>;
  destroy: () => void;
}

