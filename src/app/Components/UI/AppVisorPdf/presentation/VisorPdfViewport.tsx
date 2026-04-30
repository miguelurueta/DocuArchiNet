import { useEffect, useMemo, useRef, useState } from "react";
import type { PdfEngine } from "../engine/pdfEngine.types";
import type { AppVisorPdfInput } from "../domain/visorPdf.types";
import type { AnnotateEngine } from "../domain/annotations.types";
import styles from "./VisorPdfViewport.module.css";

export type VisorPdfViewportProps = {
  input: AppVisorPdfInput;
  engine: PdfEngine;
  annotateEngine?: AnnotateEngine;
  page: number;
  zoom: number;
  buffer?: number;
  continuous?: boolean;
  onDocumentInfo?: (info: { pageCount: number; fingerprint?: string }) => void;
  onLoadStateChange?: (state: "loading" | "ready" | "error") => void;
  onError?: (message: string) => void;
};

export function VisorPdfViewport({
  input,
  engine,
  annotateEngine,
  page,
  zoom,
  buffer = 1,
  continuous = false,
  onDocumentInfo,
  onLoadStateChange,
  onError,
}: VisorPdfViewportProps) {
  const loadAbortRef = useRef<AbortController | null>(null);
  const renderAbortRef = useRef<AbortController | null>(null);
  const canvasRefs = useRef<Map<number, HTMLCanvasElement>>(new Map());
  const overlayRefs = useRef<Map<number, HTMLCanvasElement>>(new Map());
  const loadedInputKeyRef = useRef<string | null>(null);
  const latestLoadIdRef = useRef(0);
  const [loadVersion, setLoadVersion] = useState(0);
  const [pageCount, setPageCount] = useState<number>(0);
  const [visiblePages, setVisiblePages] = useState<Set<number>>(() => new Set([1]));
  const pageElementRefs = useRef<Map<number, HTMLElement>>(new Map());
  const [placeholderSize, setPlaceholderSize] = useState<{
    width: number;
    height: number;
  } | null>(null);
  const onDocumentInfoRef = useRef<typeof onDocumentInfo>(onDocumentInfo);
  const onLoadStateChangeRef = useRef<typeof onLoadStateChange>(onLoadStateChange);
  const onErrorRef = useRef<typeof onError>(onError);

  const isCancelled = (error: unknown, signal?: AbortSignal) => {
    if (signal?.aborted) return true;
    if (error instanceof DOMException && error.name === "AbortError") return true;
    const message = error instanceof Error ? error.message : String(error);
    return (
      /render(ing)?\s+cancel(ed|led)/i.test(message) ||
      /cancelado/i.test(message) ||
      (message.includes("sendWithPromise") && signal?.aborted === true)
    );
  };

  useEffect(() => {
    onDocumentInfoRef.current = onDocumentInfo;
    onLoadStateChangeRef.current = onLoadStateChange;
    onErrorRef.current = onError;
  }, [onDocumentInfo, onError, onLoadStateChange]);

  const pagesToRender = useMemo(() => {
    const normalizedBuffer = Math.max(0, Math.floor(buffer));

    if (continuous) {
      const visibleList = Array.from(visiblePages.values()).sort((a, b) => a - b);
      if (visibleList.length === 0) return [];
      const minVisible = visibleList[0];
      const maxVisible = visibleList[visibleList.length - 1];

      const list: number[] = [];
      const start = Math.max(1, minVisible - normalizedBuffer);
      const end = pageCount > 0 ? Math.min(pageCount, maxVisible + normalizedBuffer) : maxVisible;
      for (let i = start; i <= end; i += 1) list.push(i);
      return list;
    }

    const list: number[] = [];
    for (let i = page - normalizedBuffer; i <= page + normalizedBuffer; i += 1) {
      if (i < 1) continue;
      if (pageCount > 0 && i > pageCount) continue;
      list.push(i);
    }
    return list;
  }, [buffer, continuous, page, pageCount, visiblePages]);

  useEffect(() => {
    loadAbortRef.current?.abort();
    const abortController = new AbortController();
    loadAbortRef.current = abortController;
    const loadId = (latestLoadIdRef.current += 1);

    let didSignalLoading = false;
    const settle = (state: "ready" | "error") => {
      if (!didSignalLoading) return;
      onLoadStateChangeRef.current?.(state);
    };

    const run = async () => {
      didSignalLoading = true;
      loadedInputKeyRef.current = null;
      setPageCount(0);
      setPlaceholderSize(null);
      setVisiblePages(new Set([1]));
      onLoadStateChangeRef.current?.("loading");
      try {
        const loadResult = await engine.load(input);
        if (abortController.signal.aborted || latestLoadIdRef.current !== loadId) {
          settle("ready");
          return;
        }

        loadedInputKeyRef.current =
          input.kind === "url" ? input.url : `bytes:${loadResult.fingerprint ?? "unknown"}`;
        setLoadVersion((v) => v + 1);
        setPageCount(loadResult.pageCount);

        onDocumentInfoRef.current?.({
          pageCount: loadResult.pageCount,
          fingerprint: loadResult.fingerprint,
        });

        onLoadStateChangeRef.current?.("ready");
      } catch (error) {
        if (abortController.signal.aborted || latestLoadIdRef.current !== loadId) {
          settle("ready");
          return;
        }
        const message = error instanceof Error ? error.message : String(error);
        onErrorRef.current?.(message);
        onLoadStateChangeRef.current?.("error");
      }
    };

    void run();

    return () => {
      abortController.abort();
      settle("ready");
    };
  }, [engine, input]);

  useEffect(() => {
    if (!continuous) return;
    if (pageCount <= 0) return;

    const root = pageElementRefs.current.get(1)?.closest(`.${styles.root}`) as HTMLElement | null;
    if (!root || typeof IntersectionObserver === "undefined") return;

    const observer = new IntersectionObserver(
      (entries) => {
        setVisiblePages((prev) => {
          const next = new Set(prev);
          for (const entry of entries) {
            const pageNumber = Number((entry.target as HTMLElement).dataset.page ?? "");
            if (!Number.isFinite(pageNumber) || pageNumber < 1) continue;
            if (entry.isIntersecting) next.add(pageNumber);
            else next.delete(pageNumber);
          }
          if (next.size === 0) next.add(1);
          return next;
        });
      },
      { root, rootMargin: "200px 0px", threshold: 0.01 },
    );

    for (const el of pageElementRefs.current.values()) {
      observer.observe(el);
    }

    return () => observer.disconnect();
  }, [continuous, pageCount]);

  useEffect(() => {
    renderAbortRef.current?.abort();
    const abortController = new AbortController();
    renderAbortRef.current = abortController;

    const run = async () => {
      if (!loadedInputKeyRef.current) return;

      // Give React a couple ticks to commit canvas refs before rendering pages.
      await new Promise<void>((resolve) => setTimeout(resolve, 0));
      await new Promise<void>((resolve) => setTimeout(resolve, 0));

      try {
        for (const pageNumber of pagesToRender) {
          const canvas = canvasRefs.current.get(pageNumber);
          const overlay = overlayRefs.current.get(pageNumber);
          if (!canvas) continue;

          await engine.renderPage({ pageNumber, zoom }, canvas, abortController.signal);
          if (abortController.signal.aborted) return;

          if (!placeholderSize && canvas.width > 0 && canvas.height > 0) {
            setPlaceholderSize({ width: canvas.width, height: canvas.height });
          }

          if (overlay) {
            overlay.width = canvas.width;
            overlay.height = canvas.height;
            annotateEngine?.attach(pageNumber, overlay);
          }
          await new Promise<void>((resolve) => setTimeout(resolve, 0));
        }
      } catch (error) {
        if (isCancelled(error, abortController.signal)) return;
        const message = error instanceof Error ? error.message : String(error);
        onErrorRef.current?.(message);
      }
    };

    void run();

    return () => {
      abortController.abort();
      for (const pageNumber of overlayRefs.current.keys()) {
        annotateEngine?.detach(pageNumber);
      }
    };
  }, [annotateEngine, engine, loadVersion, pagesToRender, zoom]);

  return (
    <div className={styles.root}>
      {(continuous && pageCount > 0
        ? Array.from({ length: pageCount }, (_, idx) => idx + 1)
        : pagesToRender
      ).map((pageNumber) => {
        const shouldMountCanvases = !continuous || pagesToRender.includes(pageNumber);
        return (
          <div
            key={pageNumber}
            className={styles.page}
            data-page={pageNumber}
            ref={(node) => {
              if (!node) {
                pageElementRefs.current.delete(pageNumber);
                return;
              }
              pageElementRefs.current.set(pageNumber, node);
            }}
          >
            <div className={styles.layer}>
              {shouldMountCanvases ? (
                <>
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
                  <canvas
                    className={styles.overlay}
                    ref={(node) => {
                      if (!node) {
                        overlayRefs.current.delete(pageNumber);
                        return;
                      }
                      overlayRefs.current.set(pageNumber, node);
                    }}
                  />
                </>
              ) : (
                <div
                  className={styles.placeholder}
                  style={
                    placeholderSize
                      ? {
                          width: `${placeholderSize.width}px`,
                          height: `${placeholderSize.height}px`,
                        }
                      : undefined
                  }
                />
              )}
            </div>
          </div>
        );
      })}
    </div>
  );
}
