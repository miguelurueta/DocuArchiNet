import type { Canvas } from "fabric";
import type { AppVisorPdfTool } from "../../domain/visorPdf.types";

export type FabricTool = {
  tool: AppVisorPdfTool;
  attach: (canvas: Canvas) => void;
  detach: (canvas: Canvas) => void;
};

