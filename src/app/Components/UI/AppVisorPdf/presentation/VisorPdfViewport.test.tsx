import { act, render, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import type { PdfEngine } from "../engine/pdfEngine.types";
import { VisorPdfViewport } from "./VisorPdfViewport";

function createEngineSpy(): PdfEngine & { calls: { pages: number[] } } {
  const calls = { pages: [] as number[] };
  return {
    calls,
    load: vi.fn(async () => ({ pageCount: 100 })),
    renderPage: vi.fn(async ({ pageNumber }) => {
      calls.pages.push(pageNumber);
      return { width: 100, height: 200 };
    }),
    destroy: vi.fn(() => undefined),
  };
}

describe("VisorPdfViewport [SPEC:SCRUMCORE-191]", () => {
  it("virtualiza: renderiza pagina activa + buffer y no renderiza paginas lejanas", async () => {
    // JSDOM doesn't implement canvas; provide minimal stub.
    // @ts-expect-error test-only stub
    HTMLCanvasElement.prototype.getContext = vi.fn(() => ({}));

    const engine = createEngineSpy();

    render(
      <VisorPdfViewport
        input={{ kind: "bytes", bytes: new Uint8Array([1, 2, 3]) }}
        engine={engine}
        page={10}
        zoom={1}
        buffer={1}
      />,
    );

    await act(async () => {
      await new Promise((r) => setTimeout(r, 0));
    });

    await waitFor(
      () => {
        const sorted = engine.calls.pages.slice().sort((a, b) => a - b);
        expect(sorted.includes(10)).toBe(true);
        expect(sorted.every((value) => value >= 9 && value <= 11)).toBe(true);
      },
      { timeout: 1000 },
    );
  });

  it("no deja loading colgado cuando se aborta por cambio de props", async () => {
    // JSDOM doesn't implement canvas; provide minimal stub.
    // @ts-expect-error test-only stub
    HTMLCanvasElement.prototype.getContext = vi.fn(() => ({}));

    let resolveLoad: ((value: { pageCount: number }) => void) | null = null;
    const loadPromise = new Promise<{ pageCount: number }>((resolve) => {
      resolveLoad = resolve;
    });

    const engine: PdfEngine = {
      load: vi.fn(() => loadPromise),
      renderPage: vi.fn(async () => ({ width: 100, height: 200 })),
      destroy: vi.fn(() => undefined),
    };

    const onLoadStateChange = vi.fn();

    const { rerender } = render(
      <VisorPdfViewport
        input={{ kind: "bytes", bytes: new Uint8Array([1, 2, 3]) }}
        engine={engine}
        page={1}
        zoom={1}
        buffer={0}
        onLoadStateChange={onLoadStateChange}
      />,
    );

    rerender(
      <VisorPdfViewport
        input={{ kind: "bytes", bytes: new Uint8Array([1, 2, 3]) }}
        engine={engine}
        page={2}
        zoom={1}
        buffer={0}
        onLoadStateChange={onLoadStateChange}
      />,
    );

    await act(async () => {
      resolveLoad?.({ pageCount: 2 });
      await new Promise((r) => setTimeout(r, 0));
    });

    expect(onLoadStateChange).toHaveBeenCalledWith("loading");
    expect(onLoadStateChange).toHaveBeenCalledWith("ready");
  });
});
