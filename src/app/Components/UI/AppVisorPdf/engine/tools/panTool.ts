import type { Canvas, TPointerEvent } from "fabric";
import type { FabricTool } from "./tool.types";

type PanHandlers = {
  down: (opt: { e: TPointerEvent }) => void;
  move: (opt: { e: TPointerEvent }) => void;
  up: () => void;
};

const handlerMap = new WeakMap<Canvas, PanHandlers>();

export const panTool: FabricTool = {
  tool: "pan",
  attach: (canvas: Canvas) => {
    canvas.isDrawingMode = false;
    canvas.selection = false;
    for (const object of canvas.getObjects()) {
      object.selectable = false;
      object.evented = false;
    }

    let isPanning = false;
    let lastX = 0;
    let lastY = 0;

    const down: PanHandlers["down"] = ({ e }) => {
      const mouseEvent = e as unknown as MouseEvent;
      isPanning = true;
      lastX = mouseEvent.clientX;
      lastY = mouseEvent.clientY;
    };

    const move: PanHandlers["move"] = ({ e }) => {
      if (!isPanning) return;
      const mouseEvent = e as unknown as MouseEvent;
      const vpt = canvas.viewportTransform;
      if (!vpt) return;
      vpt[4] += mouseEvent.clientX - lastX;
      vpt[5] += mouseEvent.clientY - lastY;
      lastX = mouseEvent.clientX;
      lastY = mouseEvent.clientY;
      canvas.requestRenderAll();
    };

    const up: PanHandlers["up"] = () => {
      isPanning = false;
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

