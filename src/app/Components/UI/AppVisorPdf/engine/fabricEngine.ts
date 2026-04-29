import { Canvas, util } from "fabric";
import type { AppVisorPdfTool } from "../domain/visorPdf.types";
import type {
  AnnotateEngine,
  VisorPdfAnnotationsPayloadV1,
} from "../domain/annotations.types";
import { resolveTool } from "./tools";

type PageState = {
  canvasEl: HTMLCanvasElement;
  fabricCanvas: Canvas;
  undoStack: string[];
  redoStack: string[];
};

type FabricEngineOptions = {
  fingerprint?: string;
};

function safeStringify(value: unknown): string {
  return JSON.stringify(value);
}

function stablePageSort(a: number, b: number) {
  return a - b;
}

export function createFabricEngine(
  options: FabricEngineOptions = {},
): AnnotateEngine {
  let destroyed = false;
  let currentTool: AppVisorPdfTool = "select";

  const pageMap = new Map<number, PageState>();
  const pendingRestore = new Map<number, unknown[]>();

  const toolBindings = new Map<number, AppVisorPdfTool>();

  const snapshot = (pageNumber: number) => {
    const page = pageMap.get(pageNumber);
    if (!page) return;
    const json = safeStringify(page.fabricCanvas.toObject());
    const last = page.undoStack[page.undoStack.length - 1];
    if (last === json) return;
    page.undoStack.push(json);
    page.redoStack.length = 0;
  };

  const applyToolToPage = (pageNumber: number, tool: AppVisorPdfTool) => {
    const page = pageMap.get(pageNumber);
    if (!page) return;
    const previous = toolBindings.get(pageNumber);
    if (previous === tool) return;
    if (previous) {
      resolveTool(previous).detach(page.fabricCanvas);
    }
    resolveTool(tool).attach(page.fabricCanvas);
    toolBindings.set(pageNumber, tool);
  };

  const attach: AnnotateEngine["attach"] = (pageNumber, overlayCanvas) => {
    if (destroyed) return;

    // Replace if already attached (virtualization recycle).
    if (pageMap.has(pageNumber)) {
      pageMap.get(pageNumber)?.fabricCanvas.dispose();
      pageMap.delete(pageNumber);
      toolBindings.delete(pageNumber);
    }

    const fabricCanvas = new Canvas(overlayCanvas, {
      selection: true,
      preserveObjectStacking: true,
    });

    const page: PageState = {
      canvasEl: overlayCanvas,
      fabricCanvas,
      undoStack: [],
      redoStack: [],
    };

    pageMap.set(pageNumber, page);

    const onChange = () => snapshot(pageNumber);
    fabricCanvas.on("object:added", onChange);
    fabricCanvas.on("object:modified", onChange);
    fabricCanvas.on("object:removed", onChange);
    fabricCanvas.on("path:created", onChange);

    // Initial snapshot.
    snapshot(pageNumber);

    // Apply current tool.
    applyToolToPage(pageNumber, currentTool);

    // Apply pending restore, if any.
    const pending = pendingRestore.get(pageNumber);
    if (pending && pending.length) {
      const objects = pending.filter((obj) => obj && typeof obj === "object");
      util.enlivenObjects(objects, (enlivened) => {
        if (destroyed) return;
        for (const obj of enlivened) {
          if (!obj) continue;
          fabricCanvas.add(obj);
        }
        fabricCanvas.requestRenderAll();
        snapshot(pageNumber);
      });
      pendingRestore.delete(pageNumber);
    }
  };

  const detach: AnnotateEngine["detach"] = (pageNumber) => {
    const page = pageMap.get(pageNumber);
    if (!page) return;
    resolveTool(toolBindings.get(pageNumber) ?? "select").detach(page.fabricCanvas);
    page.fabricCanvas.dispose();
    pageMap.delete(pageNumber);
    toolBindings.delete(pageNumber);
  };

  const setTool: AnnotateEngine["setTool"] = (tool) => {
    if (destroyed) return;
    // `stamp_grafo` is not implemented in this ticket; default to select to be safe.
    currentTool = tool === "stamp_grafo" ? "select" : tool;
    for (const pageNumber of pageMap.keys()) {
      applyToolToPage(pageNumber, currentTool);
    }
  };

  const undo: AnnotateEngine["undo"] = () => {
    if (destroyed) return;
    // UX definition: undo affects the currently attached page with the highest page number (most recently visible).
    const pageNumbers = Array.from(pageMap.keys()).sort(stablePageSort);
    const pageNumber = pageNumbers[pageNumbers.length - 1];
    if (!pageNumber) return;
    const page = pageMap.get(pageNumber);
    if (!page) return;
    if (page.undoStack.length <= 1) return;
    const current = page.undoStack.pop();
    if (!current) return;
    page.redoStack.push(current);
    const previous = page.undoStack[page.undoStack.length - 1];
    if (!previous) return;
    page.fabricCanvas.loadFromJSON(previous, () => {
      page.fabricCanvas.requestRenderAll();
    });
  };

  const redo: AnnotateEngine["redo"] = () => {
    if (destroyed) return;
    const pageNumbers = Array.from(pageMap.keys()).sort(stablePageSort);
    const pageNumber = pageNumbers[pageNumbers.length - 1];
    if (!pageNumber) return;
    const page = pageMap.get(pageNumber);
    if (!page) return;
    const next = page.redoStack.pop();
    if (!next) return;
    page.undoStack.push(next);
    page.fabricCanvas.loadFromJSON(next, () => {
      page.fabricCanvas.requestRenderAll();
    });
  };

  const serialize: AnnotateEngine["serialize"] = () => {
    const pages = Array.from(pageMap.entries())
      .sort(([a], [b]) => stablePageSort(a, b))
      .map(([pageNumber, page]) => {
        const asObject = page.fabricCanvas.toObject() as { objects?: unknown[] };
        return {
          pageNumber,
          objects: Array.isArray(asObject.objects) ? asObject.objects : [],
        };
      });

    return {
      version: 1,
      fingerprint: options.fingerprint,
      pages,
    };
  };

  const restore: AnnotateEngine["restore"] = (payload) => {
    if (destroyed) return;
    // Keep only v1 payloads.
    if (!payload || payload.version !== 1) return;
    for (const page of payload.pages ?? []) {
      const pageNumber = page.pageNumber;
      if (!Number.isFinite(pageNumber) || pageNumber < 1) continue;
      const objects = Array.isArray(page.objects) ? page.objects : [];
      const attached = pageMap.get(pageNumber);
      if (!attached) {
        pendingRestore.set(pageNumber, objects);
        continue;
      }

      // Clear existing objects, then rehydrate supported ones.
      attached.fabricCanvas.getObjects().forEach((obj) => attached.fabricCanvas.remove(obj));
      util.enlivenObjects(
        objects.filter((obj) => obj && typeof obj === "object"),
        (enlivened) => {
          if (destroyed) return;
          for (const obj of enlivened) {
            if (!obj) continue;
            attached.fabricCanvas.add(obj);
          }
          attached.fabricCanvas.requestRenderAll();
          snapshot(pageNumber);
        },
      );
    }
  };

  const destroy: AnnotateEngine["destroy"] = () => {
    if (destroyed) return;
    destroyed = true;
    for (const [pageNumber] of pageMap) {
      detach(pageNumber);
    }
    pageMap.clear();
    pendingRestore.clear();
    toolBindings.clear();
  };

  return {
    attach,
    detach,
    setTool,
    undo,
    redo,
    serialize,
    restore,
    destroy,
  };
}

