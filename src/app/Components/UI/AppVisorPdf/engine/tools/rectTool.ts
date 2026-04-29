import { Rect } from "fabric";
import type { Canvas, TPointerEvent } from "fabric";
import type { FabricTool } from "./tool.types";

type RectHandlers = {
  down: (opt: { e: TPointerEvent }) => void;
  move: (opt: { e: TPointerEvent }) => void;
  up: () => void;
};

const handlerMap = new WeakMap<Canvas, RectHandlers>();

export const rectTool: FabricTool = {
  tool: "rect",
  attach: (canvas: Canvas) => {
    canvas.isDrawingMode = false;
    canvas.selection = false;

    let active: Rect | null = null;
    let startX = 0;
    let startY = 0;

    const down: RectHandlers["down"] = ({ e }) => {
      const pointer = canvas.getPointer(e as unknown as MouseEvent);
      startX = pointer.x;
      startY = pointer.y;
      active = new Rect({
        left: startX,
        top: startY,
        width: 1,
        height: 1,
        fill: "rgba(59, 130, 246, 0.1)",
        stroke: "#3b82f6",
        strokeWidth: 2,
        selectable: false,
        evented: false,
      });
      canvas.add(active);
    };

    const move: RectHandlers["move"] = ({ e }) => {
      if (!active) return;
      const pointer = canvas.getPointer(e as unknown as MouseEvent);
      active.set({
        left: Math.min(pointer.x, startX),
        top: Math.min(pointer.y, startY),
        width: Math.abs(pointer.x - startX),
        height: Math.abs(pointer.y - startY),
      });
      canvas.requestRenderAll();
    };

    const up: RectHandlers["up"] = () => {
      if (!active) return;
      active.set({ selectable: true, evented: true });
      active = null;
      canvas.requestRenderAll();
    };

    handlerMap.set(canvas, { down, move, up });
    canvas.on("mouse:down", down);
    canvas.on("mouse:move", move);
    canvas.on("mouse:up", up);
  },
  detach: (canvas: Canvas) => {
    const handlers = handlerMap.get(canvas);
    if (!handlers) return;
    canvas.off("mouse:down", handlers.down);
    canvas.off("mouse:move", handlers.move);
    canvas.off("mouse:up", handlers.up);
    handlerMap.delete(canvas);
  },
};

