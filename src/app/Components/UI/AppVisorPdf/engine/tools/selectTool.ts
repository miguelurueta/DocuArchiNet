import type { Canvas } from "fabric";
import type { FabricTool } from "./tool.types";

export const selectTool: FabricTool = {
  tool: "select",
  attach: (canvas: Canvas) => {
    canvas.isDrawingMode = false;
    canvas.selection = true;
    for (const object of canvas.getObjects()) {
      object.selectable = true;
      object.evented = true;
    }
    canvas.requestRenderAll();
  },
  detach: (_canvas: Canvas) => {},
};

