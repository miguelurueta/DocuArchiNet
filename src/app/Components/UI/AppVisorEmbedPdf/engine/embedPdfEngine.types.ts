import type { AppPdfCapabilities, AppPdfSource } from "../domain/pdf.types";

export type EmbedPdfLoadResult = {
  pageCount: number;
  fingerprint?: string;
  capabilities?: AppPdfCapabilities;
};

export type EmbedPdfRenderRequest = {
  pageNumber: number;
  zoom: number;
  rotation?: number;
};

export type EmbedPdfRenderResult = {
  width: number;
  height: number;
};

export type EmbedPdfEngine = {
  load: (source: AppPdfSource) => Promise<EmbedPdfLoadResult>;
  renderPage: (
    request: EmbedPdfRenderRequest,
    canvas: HTMLCanvasElement,
    signal?: AbortSignal,
  ) => Promise<EmbedPdfRenderResult>;
  destroy: () => void;
};

