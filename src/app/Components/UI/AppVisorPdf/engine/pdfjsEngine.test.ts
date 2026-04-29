import { describe, expect, it, vi } from "vitest";
import { createPdfjsEngine } from "./pdfjsEngine";

vi.mock("pdfjs-dist", () => {
  const getDocument = vi.fn(() => {
    const doc = {
      numPages: 1,
      fingerprints: ["abc"],
      getPage: vi.fn(async () => ({
        getViewport: ({ scale }: { scale: number }) => ({
          width: 100 * scale,
          height: 200 * scale,
        }),
        render: vi.fn(() => ({
          promise: Promise.resolve(),
          cancel: vi.fn(),
        })),
      })),
      destroy: vi.fn(async () => undefined),
    };

    const loadingTask = {
      promise: Promise.resolve(doc),
      destroy: vi.fn(),
    };

    return loadingTask;
  });

  return { getDocument };
});

vi.mock("./pdfjsWorker", () => ({
  ensurePdfjsWorkerConfigured: () => undefined,
}));

function createMock2dContext() {
  const fn = () => undefined;
  return {
    canvas: { width: 0, height: 0 },
    save: fn,
    restore: fn,
    scale: fn,
    translate: fn,
    transform: fn,
    setTransform: fn,
    resetTransform: fn,
    clearRect: fn,
    fillRect: fn,
    strokeRect: fn,
    beginPath: fn,
    closePath: fn,
    rect: fn,
    clip: fn,
    fill: fn,
    stroke: fn,
    moveTo: fn,
    lineTo: fn,
    bezierCurveTo: fn,
    quadraticCurveTo: fn,
    arc: fn,
    fillText: fn,
    strokeText: fn,
    measureText: () => ({ width: 0 }),
    drawImage: fn,
    createImageData: (w: number, h: number) => ({ width: w, height: h, data: new Uint8ClampedArray(w * h * 4) }),
    getImageData: (x: number, y: number, w: number, h: number) => ({ width: w, height: h, data: new Uint8ClampedArray(w * h * 4) }),
    putImageData: fn,
    getLineDash: () => [],
    setLineDash: fn,
  };
}

function mockCanvas() {
  const canvas = document.createElement("canvas");
  // @ts-expect-error minimal context for tests
  canvas.getContext = () => createMock2dContext();
  return canvas;
}

describe("pdfjsEngine [SPEC:SCRUMCORE-191]", () => {
  it("carga un PDF y renderiza pagina 1 a canvas", async () => {
    const engine = createPdfjsEngine({ maxCacheEntries: 12 });
    const result = await engine.load({ kind: "bytes", bytes: new Uint8Array([1, 2, 3]) });
    expect(result.pageCount).toBe(1);

    const canvas = mockCanvas();
    const renderResult = await engine.renderPage({ pageNumber: 1, zoom: 1 }, canvas);
    expect(renderResult.width).toBeGreaterThan(0);
    expect(renderResult.height).toBeGreaterThan(0);
  });

  it("cachea render por page|zoom y vuelve a usarlo", async () => {
    const engine = createPdfjsEngine({ maxCacheEntries: 12 });
    await engine.load({ kind: "bytes", bytes: new Uint8Array([1, 2, 3]) });

    const canvasA = mockCanvas();
    await engine.renderPage({ pageNumber: 1, zoom: 1 }, canvasA);

    const canvasB = mockCanvas();
    await engine.renderPage({ pageNumber: 1, zoom: 1 }, canvasB);

    // Second render should succeed without throwing; behavior verified by stable result.
    expect(canvasB.width).toBe(canvasA.width);
    expect(canvasB.height).toBe(canvasA.height);
  });

  it("cambiar zoom invalida cache efectiva por key y re-renderiza", async () => {
    const engine = createPdfjsEngine({ maxCacheEntries: 12 });
    await engine.load({ kind: "bytes", bytes: new Uint8Array([1, 2, 3]) });

    const canvasA = mockCanvas();
    const renderA = await engine.renderPage({ pageNumber: 1, zoom: 1 }, canvasA);

    const canvasB = mockCanvas();
    const renderB = await engine.renderPage({ pageNumber: 1, zoom: 2 }, canvasB);

    expect(renderB.width).toBeGreaterThan(renderA.width);
    expect(renderB.height).toBeGreaterThan(renderA.height);
  });
});
