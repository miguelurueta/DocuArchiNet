import { IText } from "fabric";
import type { Canvas, TPointerEvent } from "fabric";
import type { FabricTool } from "./tool.types";

type TextHandlers = {
  down: (opt: { e: TPointerEvent }) => void;
};

const handlerMap = new WeakMap<Canvas, TextHandlers>();

export const textTool: FabricTool = {
  tool: "text",
  attach: (canvas: Canvas) => {
    canvas.isDrawingMode = false;
    canvas.selection = false;
    for (const object of canvas.getObjects()) {
      object.selectable = false;
      object.evented = false;
    }

    const down: TextHandlers["down"] = ({ e }) => {
      const pointer = canvas.getPointer(e as unknown as MouseEvent);
      const text = new IText("Texto", {
        left: pointer.x,
        top: pointer.y,
        fontSize: 16,
        fill: "#111827",
      });
      canvas.add(text);
      canvas.setActiveObject(text);
      canvas.requestRenderAll();
      text.enterEditing();
      text.selectAll();
    };

    handlerMap.set(canvas, { down });
    canvas.on("mouse:down", down);
  },
  detach: (canvas: Canvas) => {
    const handlers = handlerMap.get(canvas);
    if (!handlers) return;
    canvas.off("mouse:down", handlers.down);
    handlerMap.delete(canvas);
  },
};
