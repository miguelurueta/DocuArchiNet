import { Group, Line, Triangle } from "fabric";
import type { Canvas, TPointerEvent } from "fabric";
import type { FabricTool } from "./tool.types";

type ArrowHandlers = {
  down: (opt: { e: TPointerEvent }) => void;
  move: (opt: { e: TPointerEvent }) => void;
  up: () => void;
};

const handlerMap = new WeakMap<Canvas, ArrowHandlers>();

function degrees(radians: number) {
  return (radians * 180) / Math.PI;
}

export const arrowTool: FabricTool = {
  tool: "arrow",
  attach: (canvas: Canvas) => {
    canvas.isDrawingMode = false;
    canvas.selection = false;

    let active: Group | null = null;
    let startX = 0;
    let startY = 0;

    const down: ArrowHandlers["down"] = ({ e }) => {
      const pointer = canvas.getPointer(e as unknown as MouseEvent);
      startX = pointer.x;
      startY = pointer.y;

      const line = new Line([startX, startY, startX + 1, startY + 1], {
        stroke: "#ef4444",
        strokeWidth: 2,
        selectable: false,
        evented: false,
      });

      const head = new Triangle({
        left: startX,
        top: startY,
        originX: "center",
        originY: "center",
        width: 10,
        height: 10,
        fill: "#ef4444",
        selectable: false,
        evented: false,
      });

      active = new Group([line, head], {
        left: 0,
        top: 0,
        selectable: false,
        evented: false,
      });
      canvas.add(active);
    };

    const move: ArrowHandlers["move"] = ({ e }) => {
      if (!active) return;
      const pointer = canvas.getPointer(e as unknown as MouseEvent);

      const [line, head] = active.getObjects() as [Line, Triangle];
      line.set({ x1: startX, y1: startY, x2: pointer.x, y2: pointer.y });

      const angle = Math.atan2(pointer.y - startY, pointer.x - startX);
      head.set({
        left: pointer.x,
        top: pointer.y,
        angle: degrees(angle) + 90,
      });

      active.addWithUpdate();
      canvas.requestRenderAll();
    };

    const up: ArrowHandlers["up"] = () => {
      if (!active) return;
      active.set({ selectable: true, evented: true });
      for (const obj of active.getObjects()) {
        obj.set({ selectable: false, evented: false });
      }
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

