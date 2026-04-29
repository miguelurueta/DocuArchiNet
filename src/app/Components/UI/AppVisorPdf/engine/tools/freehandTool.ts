import { PencilBrush } from "fabric";
import type { Canvas } from "fabric";
import type { FabricTool } from "./tool.types";

export const freehandTool: FabricTool = {
  tool: "freehand",
  attach: (canvas: Canvas) => {
    canvas.isDrawingMode = true;
    canvas.selection = false;
    const brush = new PencilBrush(canvas);
    brush.width = 2;
    brush.color = "#111827";
    canvas.freeDrawingBrush = brush;
  },
  detach: (canvas: Canvas) => {
    canvas.isDrawingMode = false;
  },
};

