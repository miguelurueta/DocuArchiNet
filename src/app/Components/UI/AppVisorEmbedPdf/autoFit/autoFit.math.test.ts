import { describe, expect, it } from "vitest";
import { computeFitScale } from "./autoFit.math";

describe("autoFit.math", () => {
  it("computeFitScale fitMode=width usa viewport.width/content.width", () => {
    const scale = computeFitScale({
      viewport: { width: 1000, height: 800 },
      content: { width: 2000, height: 3000 },
      fitMode: "width",
    });
    expect(scale).toBeCloseTo(0.5, 5);
  });

  it("computeFitScale fitMode=page usa min(width,height)", () => {
    const scale = computeFitScale({
      viewport: { width: 1000, height: 800 },
      content: { width: 1200, height: 2000 },
      fitMode: "page",
    });
    expect(scale).toBeCloseTo(0.4, 5);
  });

  it("computeFitScale fallback a 1 con tamaños inválidos", () => {
    const scale = computeFitScale({
      viewport: { width: 0, height: 800 },
      content: { width: 1200, height: 2000 },
      fitMode: "width",
    });
    expect(scale).toBe(1);
  });

  it("computeFitScale para contenido rotado (swap) debe permitir fit estable", () => {
    // Simula rotación 90°: content (w=1000,h=2000) se vuelve (w=2000,h=1000)
    // Para fit-to-width en viewport 1000, escala esperada ~0.5.
    const scale = computeFitScale({
      viewport: { width: 1000, height: 800 },
      content: { width: 2000, height: 1000 },
      fitMode: "width",
    });
    expect(scale).toBeCloseTo(0.5, 5);
  });
});
