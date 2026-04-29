import { useEffect, useMemo, useRef } from "react";
import type { PdfEngine } from "../engine/pdfEngine.types";
import type { AppVisorPdfInput } from "../domain/visorPdf.types";
import styles from "./VisorPdfViewport.module.css";

export type VisorPdfViewportProps = {
  input: AppVisorPdfInput;
  engine: PdfEngine;
  page: number;
  zoom: number;
  buffer?: number;
  onLoadStateChange?: (state: "loading" | "ready" | "error") => void;
  onError?: (message: string) => void;
};

export function VisorPdfViewport({
  input,
  engine,
  page,
  zoom,
  buffer = 1,
  onLoadStateChange,
  onError,
}: VisorPdfViewportProps) {
  const abortRef = useRef<AbortController | null>(null);
  const canvasRefs = useRef<Map<number, HTMLCanvasElement>>(new Map());

  const pagesToRender = useMemo(() => {
    const normalizedBuffer = Math.max(0, Math.floor(buffer));
    const list: number[] = [];
    for (let i = page - normalizedBuffer; i <= page + normalizedBuffer; i += 1) {
      if (i >= 1) list.push(i);
    }
    return list;
  }, [buffer, page]);

  useEffect(() => {
    abortRef.current?.abort();
    const abortController = new AbortController();
    abortRef.current = abortController;

    let cancelled = false;

    const run = async () => {
      onLoadStateChange?.("loading");
      try {
        await engine.load(input);
        if (abortController.signal.aborted || cancelled) return;

        // Give React a couple ticks to commit canvas refs before rendering pages.
        await new Promise<void>((resolve) => setTimeout(resolve, 0));
        await new Promise<void>((resolve) => setTimeout(resolve, 0));
        for (const pageNumber of pagesToRender) {
          const canvas = canvasRefs.current.get(pageNumber);
          if (!canvas) continue;
          await engine.renderPage(
            { pageNumber, zoom },
            canvas,
            abortController.signal,
          );
          if (abortController.signal.aborted || cancelled) return;
          await new Promise<void>((resolve) => setTimeout(resolve, 0));
        }

        onLoadStateChange?.("ready");
      } catch (error) {
        if (abortController.signal.aborted || cancelled) return;
        const message = error instanceof Error ? error.message : String(error);
        onError?.(message);
        onLoadStateChange?.("error");
      }
    };

    void run();

    return () => {
      cancelled = true;
      abortController.abort();
      engine.destroy();
    };
  }, [engine, input, onError, onLoadStateChange, pagesToRender, zoom]);

  return (
    <div className={styles.root}>
      {pagesToRender.map((pageNumber) => (
        <div key={pageNumber} className={styles.page} data-page={pageNumber}>
          <canvas
            className={styles.canvas}
            ref={(node) => {
              if (!node) {
                canvasRefs.current.delete(pageNumber);
                return;
              }
              canvasRefs.current.set(pageNumber, node);
            }}
          />
        </div>
      ))}
    </div>
  );
}
