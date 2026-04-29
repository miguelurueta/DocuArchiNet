import type { AppVisorPdfTool } from "./visorPdf.types";

export type VisorPdfAnnotationsPayloadV1 = {
  version: 1;
  fingerprint?: string;
  pages: Array<{
    pageNumber: number;
    objects: unknown[];
  }>;
};

export type AnnotateEngine = {
  attach: (pageNumber: number, overlayCanvas: HTMLCanvasElement) => void;
  detach: (pageNumber: number) => void;
  setTool: (tool: AppVisorPdfTool) => void;
  undo: () => void;
  redo: () => void;
  serialize: () => VisorPdfAnnotationsPayloadV1;
  restore: (payload: VisorPdfAnnotationsPayloadV1) => void;
  destroy: () => void;
};

