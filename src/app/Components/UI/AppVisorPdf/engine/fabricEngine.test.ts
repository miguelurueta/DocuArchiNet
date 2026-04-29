import { describe, expect, it, vi } from "vitest";

vi.mock("./tools", () => ({
  resolveTool: () => ({
    attach: () => {},
    detach: () => {},
  }),
}));

vi.mock("fabric", () => {
  type Handler = () => void;
  class Canvas {
    private objects: unknown[] = [];
    private handlers = new Map<string, Handler[]>();

    isDrawingMode = false;
    selection = true;
    viewportTransform: number[] | null = [1, 0, 0, 1, 0, 0];

    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    constructor(_el: HTMLCanvasElement, _options?: unknown) {}

    on(event: string, handler: Handler) {
      const list = this.handlers.get(event) ?? [];
      list.push(handler);
      this.handlers.set(event, list);
    }

    off(event: string, handler: Handler) {
      const list = this.handlers.get(event) ?? [];
      this.handlers.set(
        event,
        list.filter((item) => item !== handler),
      );
    }

    add(object: unknown) {
      this.objects.push(object);
      for (const handler of this.handlers.get("object:added") ?? []) handler();
    }

    remove(object: unknown) {
      this.objects = this.objects.filter((item) => item !== object);
      for (const handler of this.handlers.get("object:removed") ?? []) handler();
    }

    getObjects() {
      return [...this.objects];
    }

    toObject() {
      return { objects: [...this.objects] };
    }

    // eslint-disable-next-line @typescript-eslint/no-unused-vars
    loadFromJSON(json: string, callback: () => void) {
      const parsed = JSON.parse(json) as { objects?: unknown[] };
      this.objects = Array.isArray(parsed.objects) ? [...parsed.objects] : [];
      callback();
    }

    requestRenderAll() {}
    dispose() {}
  }

  const util = {
    enlivenObjects: (objects: unknown[], callback: (list: unknown[]) => void) =>
      callback(objects),
  };

  return { Canvas, util };
});

describe("createFabricEngine", () => {
  it("serializes per-page objects deterministically", async () => {
    const { createFabricEngine } = await import("./fabricEngine");

    const engine = createFabricEngine({ fingerprint: "fp" });
    const overlay = document.createElement("canvas");
    engine.attach(1, overlay);

    const payload1 = engine.serialize();
    const payload2 = engine.serialize();
    expect(payload1).toEqual(payload2);
    expect(payload1.version).toBe(1);
    expect(payload1.fingerprint).toBe("fp");
  });

  it("restore ignores unknown objects safely", async () => {
    const { createFabricEngine } = await import("./fabricEngine");

    const engine = createFabricEngine();
    const overlay = document.createElement("canvas");
    engine.attach(1, overlay);

    engine.restore({
      version: 1,
      pages: [{ pageNumber: 1, objects: ["unknown", { kind: "rect" }] }],
    });

    const payload = engine.serialize();
    expect(payload.pages[0]?.objects).toEqual([{ kind: "rect" }]);
  });

  it("undo/redo updates page state", async () => {
    const { createFabricEngine } = await import("./fabricEngine");

    const engine = createFabricEngine();
    const overlay = document.createElement("canvas");
    engine.attach(1, overlay);

    engine.restore({
      version: 1,
      pages: [{ pageNumber: 1, objects: [{ a: 1 }] }],
    });

    expect(engine.serialize().pages[0]?.objects.length).toBe(1);
    engine.undo();
    expect(engine.serialize().pages[0]?.objects.length).toBe(0);
    engine.redo();
    expect(engine.serialize().pages[0]?.objects.length).toBe(1);
  });
});

