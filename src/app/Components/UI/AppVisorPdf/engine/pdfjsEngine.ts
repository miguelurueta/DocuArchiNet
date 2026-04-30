import { getDocument, type PDFDocumentProxy } from "pdfjs-dist";
import type { PdfEngine, PdfLoadResult, PdfRenderRequest, PdfRenderResult } from "./pdfEngine.types";
import type { AppVisorPdfInput } from "../domain/visorPdf.types";
import { ensurePdfjsWorkerConfigured } from "./pdfjsWorker";
import { LruCache } from "./lruCache";

type CachedPage = {
  canvas: HTMLCanvasElement;
  width: number;
  height: number;
  zoom: number;
};

const buildCacheKey = (req: PdfRenderRequest) => `${req.pageNumber}|${req.zoom}`;

const toFriendlyError = (error: unknown) => {
  const message = error instanceof Error ? error.message : String(error);
  return message.trim() ? message : "No se pudo cargar el PDF.";
};

const isRenderCancelledError = (error: unknown) => {
  if (error instanceof DOMException && error.name === "AbortError") return true;
  const message = error instanceof Error ? error.message : String(error);
  return /render(ing)?\s+cancel(ed|led)/i.test(message) || /cancelado/i.test(message);
};

export type PdfjsEngineOptions = {
  maxCacheEntries?: number;
  disableWorker?: boolean;
  loadTimeoutMs?: number;
};

export function createPdfjsEngine(options: PdfjsEngineOptions = {}): PdfEngine {
  if (!options.disableWorker) {
    ensurePdfjsWorkerConfigured();
  }

  const maxCacheEntries = options.maxCacheEntries ?? 12;
  const cache = new LruCache<string, CachedPage>({ maxEntries: maxCacheEntries });

  let pdfDocument: PDFDocumentProxy | null = null;
  let currentFingerprint: string | undefined;
  let activeAbortController: AbortController | null = null;

  const abortActive = () => {
    activeAbortController?.abort();
    activeAbortController = null;
  };

  const destroy = () => {
    abortActive();
    cache.clear();
    currentFingerprint = undefined;
    const doc = pdfDocument;
    pdfDocument = null;
    if (doc) {
      void doc.destroy();
    }
  };

  const load = async (input: AppVisorPdfInput): Promise<PdfLoadResult> => {
    destroy();

    activeAbortController = new AbortController();
    const signal = activeAbortController.signal;

    const src =
      input.kind === "url"
        ? { url: input.url }
        : { data: input.bytes };

    try {
      const loadingTask = getDocument({
        ...src,
        // Keep worker enabled by default; configurable for tests.
        disableWorker: Boolean(options.disableWorker),
      });
      signal.addEventListener("abort", () => loadingTask.destroy(), { once: true });

      const timeoutMs = options.loadTimeoutMs ?? 20_000;
      const doc = await Promise.race([
        loadingTask.promise,
        new Promise<never>((_, reject) => {
          const timer = setTimeout(() => {
            clearTimeout(timer);
            loadingTask.destroy();
            reject(new Error("Tiempo de espera agotado cargando el PDF."));
          }, timeoutMs);
        }),
      ]);
      if (signal.aborted) {
        await doc.destroy();
        throw new Error("Carga cancelada.");
      }

      pdfDocument = doc;
      currentFingerprint = doc.fingerprints?.[0];

      return {
        pageCount: doc.numPages,
        fingerprint: currentFingerprint,
      };
    } catch (error) {
      throw new Error(toFriendlyError(error));
    }
  };

  const renderPage = async (
    req: PdfRenderRequest,
    canvas: HTMLCanvasElement,
    signal?: AbortSignal,
  ): Promise<PdfRenderResult> => {
    if (!pdfDocument) {
      throw new Error("PDF no cargado.");
    }

    const cacheKey = buildCacheKey(req);
    const cached = cache.get(cacheKey);
    if (cached && cached.zoom === req.zoom) {
      const ctx = canvas.getContext("2d");
      if (ctx) {
        canvas.width = cached.width;
        canvas.height = cached.height;
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(cached.canvas, 0, 0);
      }
      return { width: cached.width, height: cached.height };
    }

    try {
      const page = await pdfDocument.getPage(req.pageNumber);
      if (signal?.aborted) {
        throw new DOMException("Render cancelado.", "AbortError");
      }

      const viewport = page.getViewport({ scale: req.zoom });
      canvas.width = Math.ceil(viewport.width);
      canvas.height = Math.ceil(viewport.height);

      const ctx = canvas.getContext("2d");
      if (!ctx) {
        throw new Error("No se pudo inicializar canvas.");
      }

      const renderTask = page.render({
        canvasContext: ctx,
        viewport,
      });

      if (signal) {
        signal.addEventListener("abort", () => renderTask.cancel(), { once: true });
      }

      await renderTask.promise;
      if (signal?.aborted) {
        throw new DOMException("Render cancelado.", "AbortError");
      }

      const cachedCanvas = document.createElement("canvas");
      cachedCanvas.width = canvas.width;
      cachedCanvas.height = canvas.height;
      const cachedCtx = cachedCanvas.getContext("2d");
      cachedCtx?.drawImage(canvas, 0, 0);

      cache.set(cacheKey, {
        canvas: cachedCanvas,
        width: canvas.width,
        height: canvas.height,
        zoom: req.zoom,
      });

      return { width: canvas.width, height: canvas.height };
    } catch (error) {
      if (signal?.aborted || isRenderCancelledError(error)) {
        throw new DOMException("Render cancelado.", "AbortError");
      }
      throw new Error(toFriendlyError(error));
    }
  };

  return {
    load,
    renderPage,
    destroy,
  };
}
