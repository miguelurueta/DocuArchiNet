import { BookOutlined, LeftOutlined, RightOutlined } from "@ant-design/icons";
import { useCallback, useEffect, useId, useMemo, useRef, useState } from "react";
import { toast, type Id as ToastId } from "react-toastify";
import axios from "axios";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppCollapseRail } from "../../../../app/Components/UI/AppCollapseRail";
import { AppTreeTable } from "../../../../app/Components/UI/AppTreeTable";
import { AppVisorEmbedPdf } from "../../../../app/Components/UI/AppVisorEmbedPdf";
import type { AppVisorEmbedPdfRef } from "../../../../app/Components/UI/AppVisorEmbedPdf";
import { useDocumentViewerOrchestrator } from "../../../../app/Components/UI/AppDocumentViewerOrchestrator";
import {
  cancelUploadTemporal,
  completeUploadTemporal,
  initUploadTemporalPdfAnotado,
  reemplazarPaginasPdfAnotadas,
  statusUploadTemporal,
  uploadTemporalChunk,
} from "../../../../app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.service";
import { ReemplazoPaginasPdfAnotadasError } from "../../../../app/Components/UI/AppVisorEmbedPdf/services/reemplazoPaginasPdfAnotadas.types";
import { useGestionRespuestaDocumentosTable } from "../../hooks/useGestionRespuestaDocumentosTable";
import styles from "./DocumentosWorkbench.module.css";

const MOBILE_QUERY = "(max-width: 768px)";
const DEFAULT_REEMPLAZO_CHUNK_SIZE_BYTES = 1_048_576;

const useMediaQuery = (query: string) => {
  const getMatches = () =>
    typeof window !== "undefined" ? window.matchMedia(query).matches : false;
  const [matches, setMatches] = useState(getMatches);

  useEffect(() => {
    const mediaQueryList = window.matchMedia(query);
    const update = (event: MediaQueryListEvent) => {
      setMatches(event.matches);
    };

    setMatches(mediaQueryList.matches);
    mediaQueryList.addEventListener("change", update);
    return () => {
      mediaQueryList.removeEventListener("change", update);
    };
  }, [query]);

  return matches;
};

function resolveIsTablet() {
  if (typeof window === "undefined") return false;

  const width = window.innerWidth;
  const isTouchDevice =
    typeof navigator !== "undefined" && (navigator.maxTouchPoints ?? 0) > 0;

  return isTouchDevice && width > 768 && width <= 1366;
}

type DocumentosWorkbenchProps = {
  idTareaWf?: number;
};

type ApiErrorItem = { Message?: unknown };
type ApiEnvelope = { message?: unknown; errors?: unknown };

function dvDebugEnabled(): boolean {
  return typeof window !== "undefined" && Boolean((window as any).__DV_DEBUG__);
}

function dvLog(...args: unknown[]) {
  if (!dvDebugEnabled()) return;
  // eslint-disable-next-line no-console
  console.log(...args);
}

function isCancelledError(err: unknown): boolean {
  if (err instanceof DOMException && err.name === "AbortError") return true;
  if (axios.isAxiosError(err) && err.code === "ERR_CANCELED") return true;
  return false;
}

function extractBackendCause(err: unknown): { message: string; http?: number } | null {
  if (!axios.isAxiosError(err)) return null;
  const http = typeof err.response?.status === "number" ? err.response.status : undefined;
  const data = err.response?.data as ApiEnvelope | undefined;

  const errors = Array.isArray(data?.errors) ? (data?.errors as unknown[]) : null;
  const first = errors?.[0] as ApiErrorItem | undefined;
  if (typeof first?.Message === "string" && first.Message.trim()) {
    return { message: first.Message.trim(), http };
  }

  if (typeof data?.message === "string" && data.message.trim()) {
    return { message: data.message.trim(), http };
  }

  if (typeof err.message === "string" && err.message.trim()) {
    return { message: err.message.trim(), http };
  }

  return http ? { message: `Request failed (HTTP ${http}).`, http } : null;
}

function getReplacementErrorMessage(err: unknown): string {
  if (err instanceof ReemplazoPaginasPdfAnotadasError) {
    const suffix = err.requestId ? ` RequestId: ${err.requestId}` : "";
    if (err.field === "originalPdfPassword") {
      return `El PDF original requiere una contrasena valida para reemplazar paginas.${suffix}`;
    }
    return `${err.message || "No fue posible reemplazar las paginas anotadas."}${suffix}`;
  }

  const backendCause = extractBackendCause(err);
  if (backendCause) {
    return backendCause.http ? `${backendCause.message} (HTTP ${backendCause.http}).` : backendCause.message;
  }

  if (err instanceof Error && err.message.trim()) return err.message.trim();
  return "No fue posible reemplazar las paginas anotadas.";
}

function summarizeReplacementError(err: unknown) {
  if (err instanceof ReemplazoPaginasPdfAnotadasError) {
    return {
      name: err.name,
      message: err.message,
      field: err.field,
      type: err.type,
      requestId: err.requestId,
      details: err.details,
    };
  }

  if (axios.isAxiosError(err)) {
    return {
      name: err.name,
      message: err.message,
      code: err.code,
      status: err.response?.status,
      statusText: err.response?.statusText,
      data: err.response?.data,
      url: err.config?.url,
      method: err.config?.method,
    };
  }

  if (err instanceof Error) {
    return {
      name: err.name,
      message: err.message,
      stack: err.stack,
    };
  }

  return {
    type: typeof err,
    value: err,
  };
}

function withViewerReloadToken(url: string, attemptId?: number): string {
  if (!attemptId || url.startsWith("blob:") || url.startsWith("data:")) return url;
  const separator = url.includes("?") ? "&" : "?";
  return `${url}${separator}_dvAttempt=${encodeURIComponent(String(attemptId))}`;
}

export function DocumentosWorkbench({ idTareaWf }: DocumentosWorkbenchProps) {
  const panelId = useId();
  const rootRef = useRef<HTMLElement | null>(null);
  const viewSeqRef = useRef(0);
  const openTimeoutRef = useRef<number | null>(null);
  const replacementAbortRef = useRef<AbortController | null>(null);
  const replacementSeqRef = useRef(0);
  const viewerLoadingDelayRef = useRef<number | null>(null);
  const viewerLoadingKeyRef = useRef<string | null>(null);
  const viewerLoadingShownAtRef = useRef<number | null>(null);
  const viewerLoadingMinHideRef = useRef<number | null>(null);
  const documentHintTimeoutRef = useRef<number | null>(null);
  const attemptIdRef = useRef(0);
  const lastNotifiedErrorRef = useRef<string | null>(null);
  const toastIdRef = useRef<ToastId | null>(null);
  const isMobile = useMediaQuery(MOBILE_QUERY);
  const [isTablet] = useState(resolveIsTablet);
  const [collapsed, setCollapsed] = useState(isTablet);
  const documentosTable = useGestionRespuestaDocumentosTable(idTareaWf);
  const visorRef = useRef<AppVisorEmbedPdfRef | null>(null);
  const lastVisorLoadKeyRef = useRef<string | null>(null);
  const [activeFileUrl, setActiveFileUrl] = useState<string | undefined>(undefined);
  const [activeRowId, setActiveRowId] = useState<string | undefined>(undefined);
  const [viewerError, setViewerError] = useState<string | null>(null);
  const [showViewerLoading, setShowViewerLoading] = useState(false);
  const [documentHintActive, setDocumentHintActive] = useState(false);
  const [isReplacingAnnotatedPages, setIsReplacingAnnotatedPages] = useState(false);
  const [replacementProgress, setReplacementProgress] = useState<number | undefined>(undefined);
  const documentViewer = useDocumentViewerOrchestrator();

  const startViewerLoading = useCallback((key: string) => {
    viewerLoadingKeyRef.current = key;
    // Hint "Abriendo documento…" removido: no se registra timestamp de hint.
    viewerLoadingShownAtRef.current = null;
    // setShowViewerLoadingHint(true);
    setShowViewerLoading(false);
    dvLog("[DV][loading] startViewerLoading", { key, at: Date.now() });
    if (viewerLoadingDelayRef.current) window.clearTimeout(viewerLoadingDelayRef.current);
    if (viewerLoadingMinHideRef.current) window.clearTimeout(viewerLoadingMinHideRef.current);
    viewerLoadingMinHideRef.current = null;
    viewerLoadingDelayRef.current = window.setTimeout(() => {
      if (viewerLoadingKeyRef.current !== key) return;
      viewerLoadingShownAtRef.current = Date.now();
      dvLog("[DV][loading] skeleton ON (after delay)", { key, at: viewerLoadingShownAtRef.current });
      setShowViewerLoading(true);
      // setShowViewerLoadingHint(false);
      // (hint removido)
    }, 100);
  }, []);

  const stopViewerLoading = useCallback((key: string) => {
    if (viewerLoadingKeyRef.current !== key) return;
    if (viewerLoadingDelayRef.current) window.clearTimeout(viewerLoadingDelayRef.current);
    viewerLoadingDelayRef.current = null;
    dvLog("[DV][loading] stopViewerLoading", { key, at: Date.now(), skeletonShownAt: viewerLoadingShownAtRef.current });

    const hintShownAt = null;
    const skeletonShownAt = viewerLoadingShownAtRef.current;

    // Si nunca llegó a mostrarse el skeleton completo, aplicar mínimo de visibilidad al hint
    // para asegurar al menos 1 paint (evita "a veces no aparece" cuando el engine responde muy rápido).
    if (!skeletonShownAt && hintShownAt) {
      const minHintVisibleMs = 180;
      const elapsedHint = Date.now() - hintShownAt;
      const remainingHint = Math.max(0, minHintVisibleMs - elapsedHint);
      if (remainingHint > 0) {
        if (viewerLoadingMinHideRef.current) window.clearTimeout(viewerLoadingMinHideRef.current);
        viewerLoadingMinHideRef.current = window.setTimeout(() => {
          if (viewerLoadingKeyRef.current !== key) return;
          viewerLoadingKeyRef.current = null;
          // setShowViewerLoadingHint(false);
          setShowViewerLoading(false);
          // (hint removido)
          viewerLoadingShownAtRef.current = null;
          viewerLoadingMinHideRef.current = null;
        }, remainingHint);
        return;
      }
    }

    // setShowViewerLoadingHint(false);
    const shownAt = viewerLoadingShownAtRef.current;
    const minVisibleMs = 500;
    if (!shownAt) {
      viewerLoadingKeyRef.current = null;
      setShowViewerLoading(false);
      // (hint removido)
      return;
    }

    const elapsed = Date.now() - shownAt;
    const remaining = Math.max(0, minVisibleMs - elapsed);
    if (remaining <= 0) {
      viewerLoadingKeyRef.current = null;
      setShowViewerLoading(false);
      viewerLoadingShownAtRef.current = null;
      // (hint removido)
      return;
    }

    if (viewerLoadingMinHideRef.current) window.clearTimeout(viewerLoadingMinHideRef.current);
    viewerLoadingMinHideRef.current = window.setTimeout(() => {
      // Si entre tanto se inició otro attempt, no ocultar el overlay nuevo.
      if (viewerLoadingKeyRef.current !== key) return;
      viewerLoadingKeyRef.current = null;
      setShowViewerLoading(false);
      viewerLoadingShownAtRef.current = null;
      // (hint removido)
      viewerLoadingMinHideRef.current = null;
    }, remaining);
  }, []);

  // `isTablet` se resuelve al montar y se actualiza manualmente; el listener de resize
  // se dejó desactivado temporalmente.
  /*
    useEffect(() => {
      const handler = () => setIsTablet(resolveIsTablet());
      window.addEventListener("resize", handler);
      return () => window.removeEventListener("resize", handler);
    }, []);

    useEffect(() => {
      setCollapsed(isTablet);
    }, [isTablet]);
  */

  const variant = useMemo(
    () => (isMobile || isTablet ? "overlay" : "inline"),
    [isMobile, isTablet],
  );
  const layoutCollapsed = variant === "overlay" ? true : collapsed;
  const documentsCounter = useMemo(() => {
    const total = documentosTable.totalDocumentsCount ?? 0;
    const selected = documentosTable.selectedDocumentsCount ?? 0;
    return selected > 0
      ? `Documentos (${total}) · Seleccionados (${selected})`
      : `Documentos (${total})`;
  }, [documentosTable.selectedDocumentsCount, documentosTable.totalDocumentsCount]);

  const toggleIcon = layoutCollapsed ? <LeftOutlined /> : <RightOutlined />;

  const triggerDocumentListHint = useCallback(() => {
    setCollapsed(false);
    setDocumentHintActive(false);
    if (documentHintTimeoutRef.current) window.clearTimeout(documentHintTimeoutRef.current);

    const scheduleFrame =
      window.requestAnimationFrame ??
      ((callback: FrameRequestCallback) => window.setTimeout(() => callback(performance.now()), 0));
    scheduleFrame(() => {
      setDocumentHintActive(true);
      documentHintTimeoutRef.current = window.setTimeout(() => {
        setDocumentHintActive(false);
        documentHintTimeoutRef.current = null;
      }, 1600);
    });
  }, []);

  useEffect(() => {
    const fileUrl = documentViewer.documentoActivo?.fileUrl ?? null;
    if (!fileUrl) return;
    const attemptId = documentViewer.documentoActivo?.attemptId;
    setActiveFileUrl(withViewerReloadToken(fileUrl, attemptId));
    if (typeof attemptId === "number") stopViewerLoading(String(attemptId));
  }, [documentViewer.documentoActivo?.attemptId, documentViewer.documentoActivo?.fileUrl, stopViewerLoading]);

  const documentContext = useMemo(() => {
    const doc = documentViewer.documentoActivo;
    if (!doc) return null;

    return {
      documentId: doc.documentId,
      nombreGabinete: doc.nombreGabinete,
      fileUrl: doc.fileUrl ? withViewerReloadToken(doc.fileUrl, doc.attemptId) : doc.fileUrl,
      attemptId: doc.attemptId,
      documentKey: doc.documentKey,
      isPdf: doc.isPdf,
      viewerKind: doc.viewerKind,
      isElectronicallySigned: doc.isElectronicallySigned,
      firmaCheckStatus: doc.firmaCheckStatus,
    };
  }, [
    documentViewer.documentoActivo?.attemptId,
    documentViewer.documentoActivo?.documentId,
    documentViewer.documentoActivo?.documentKey,
    documentViewer.documentoActivo?.fileUrl,
    documentViewer.documentoActivo?.firmaCheckStatus,
    documentViewer.documentoActivo?.isElectronicallySigned,
    documentViewer.documentoActivo?.isPdf,
    documentViewer.documentoActivo?.nombreGabinete,
    documentViewer.documentoActivo?.viewerKind,
  ]);

  const cancelTemporalsBestEffort = useCallback(
    async (temporals: Array<{ rutaTemporalId: string; archivoTemporalId: string }>) => {
      await Promise.allSettled(
        temporals.map((temporal) =>
          cancelUploadTemporal({
            rutaTemporalId: temporal.rutaTemporalId,
            archivoTemporalId: temporal.archivoTemporalId,
          }),
        ),
      );
    },
    [],
  );

  const uploadAnnotatedPagePdf = useCallback(
    async (params: { pageNumber: number; fileName: string; blob: Blob; hashSha256?: string }, signal: AbortSignal) => {
      const numeroChunks = Math.max(1, Math.ceil(params.blob.size / DEFAULT_REEMPLAZO_CHUNK_SIZE_BYTES));
      const init = await initUploadTemporalPdfAnotado(
        {
          NombreOriginal: params.fileName,
          TamanoBytes: params.blob.size,
          Extension: ".PDF",
          HashSha256Esperado: params.hashSha256 ?? null,
          NumeroChunks: numeroChunks,
        },
        { signal },
      );

      const chunkSize = init.ChunkSizeBytes || DEFAULT_REEMPLAZO_CHUNK_SIZE_BYTES;
      const totalChunks = Math.max(1, Math.ceil(params.blob.size / chunkSize));
      for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex += 1) {
        const start = chunkIndex * chunkSize;
        const end = Math.min(params.blob.size, start + chunkSize);
        await uploadTemporalChunk(
          {
            rutaTemporalId: init.RutaTemporalId,
            archivoTemporalId: init.ArchivoTemporalId,
            chunkIndex,
            totalChunks,
            chunk: params.blob.slice(start, end),
          },
          { signal },
        );
      }

      await completeUploadTemporal(
        { rutaTemporalId: init.RutaTemporalId, archivoTemporalId: init.ArchivoTemporalId },
        { signal },
      );
      const status = await statusUploadTemporal(
        { rutaTemporalId: init.RutaTemporalId, archivoTemporalId: init.ArchivoTemporalId },
        { signal },
      );
      if (status.Estado !== "COMPLETED") {
        throw new Error(`El temporal de la pagina ${params.pageNumber} no quedo COMPLETED.`);
      }

      return {
        pageNumber: params.pageNumber,
        rutaTemporalId: init.RutaTemporalId,
        archivoTemporalId: init.ArchivoTemporalId,
        hashSha256: params.hashSha256,
      };
    },
    [],
  );

  useEffect(() => {
    // Modo managed del visor: cargar con contexto consolidado + permisos/policy.
    if (!documentContext) return;
    if (!documentContext.isPdf) return;
    if (!documentContext.fileUrl) return;

    // Guardrail: evitar múltiples `load()` por el mismo documento/fuente.
    // Bajo re-renders (firma/rowId) el effect puede dispararse varias veces y abrir
    // múltiples documentos en el DocumentManager, alcanzando el límite (10).
    const loadKey = [
      documentContext.documentId,
      documentContext.fileUrl,
      documentContext.documentKey ?? "",
      documentContext.isElectronicallySigned ?? "",
    ].join(":");
    if (lastVisorLoadKeyRef.current === loadKey) return;
    lastVisorLoadKeyRef.current = loadKey;

    const ctx = documentosTable.getWorkbenchContext?.();
    const radicado = ctx?.radicado ?? "";
    const idTareaWorkflow = typeof idTareaWf === "number" ? idTareaWf : 0;
    const attemptId = documentContext.attemptId;
    const attemptKey = typeof attemptId === "number" ? String(attemptId) : null;

    void visorRef.current
      ?.load({
      url: documentContext.fileUrl,
      attemptId: documentContext.attemptId,
      documentKey: documentContext.documentKey,
      isElectronicallySigned: Boolean(documentContext.isElectronicallySigned),
      idImagen: documentContext.documentId,
      nombreGabinete: documentContext.nombreGabinete,
      idTareaWorkflow,
      radicado,
      nombre_modulo: "gestioncorrespondencia",
    })
      .then((result) => {
        if (!attemptKey) return;
        if (result.loadStatus === "loaded" || result.loadStatus === "failed" || result.loadStatus === "cancelled") {
          stopViewerLoading(attemptKey);
        }
      })
      .catch(() => {
        if (!attemptKey) return;
        stopViewerLoading(attemptKey);
      });
  }, [documentContext, documentosTable.getWorkbenchContext, idTareaWf, stopViewerLoading]);

  useEffect(() => {
    const doc = documentViewer.documentoActivo;
    if (!doc) return;

    const message = doc.errors?.[0];
    if (!message) return;

    if (doc.resolveStatus === "failed") {
      setViewerError(message);
    } else if (doc.firmaCheckStatus === "failed") {
      setViewerError(message);
    }
  }, [documentViewer.documentoActivo]);

  useEffect(() => {
    if (!viewerError) return;
    if (viewerError === lastNotifiedErrorRef.current) return;

    lastNotifiedErrorRef.current = viewerError;
    toastIdRef.current = toast.error(viewerError, { autoClose: false, closeOnClick: false });
  }, [viewerError]);

  useEffect(
    () => () => {
      if (documentHintTimeoutRef.current) window.clearTimeout(documentHintTimeoutRef.current);
    },
    [],
  );

  useEffect(() => {
    if (!toastIdRef.current) return;

    let cancelled = false;

    const dismiss = () => {
      if (cancelled) return;
      const toastId = toastIdRef.current;
      if (toastId) toast.dismiss(toastId);
      toastIdRef.current = null;
      setViewerError(null);
    };

    // Evita cerrar el toast inmediatamente por el mismo click que lo disparó.
    const timeoutId = window.setTimeout(() => {
      if (cancelled) return;
      window.addEventListener("pointerdown", dismiss, { capture: true });
    }, 400);

    return () => {
      cancelled = true;
      window.clearTimeout(timeoutId);
      window.removeEventListener("pointerdown", dismiss, { capture: true });
    };
  }, [viewerError]);

  const openViewerFromRow = useCallback(
    (rowId: string) => {
      viewSeqRef.current += 1;
      const seq = viewSeqRef.current;
      attemptIdRef.current += 1;
      const attemptId = attemptIdRef.current;
      const attemptKey = `[DV][attempt:${attemptId}][seq:${seq}]`;
      setViewerError(null);
      startViewerLoading(String(attemptId));
      // Permite que el mismo mensaje vuelva a notificarse si el usuario reintenta
      // abrir el documento (click repetido en la misma fila).
      lastNotifiedErrorRef.current = null;
      // Guardrail: si la action `ver_documento` no responde, dejar huella visible.
      if (openTimeoutRef.current) window.clearTimeout(openTimeoutRef.current);
      openTimeoutRef.current = window.setTimeout(() => {
        if (seq !== viewSeqRef.current) return;
        dvLog(attemptKey, "ver_documento timeout (10s)");
        setViewerError("ver_documento: sin respuesta del backend (timeout).");
        stopViewerLoading(String(attemptId));
      }, 10000);

      void (async () => {
        try {
          // Cancelación encadenada (click cancelable): evitar backlog de requests/loads.
          dvLog(attemptKey, "click -> cancel chain (visor.cancelCurrentLoad + orchestrator.cancelCurrentRequest)");
          visorRef.current?.cancelCurrentLoad();
          documentViewer.cancelCurrentRequest();

          dvLog(attemptKey, "ver_documento start", { rowId });
          const result = await documentosTable.onSelectRow(rowId);
          if (seq !== viewSeqRef.current) return;
          if (!result?.documentResolveRequest) {
            dvLog(attemptKey, "ver_documento missing DocumentResolveRequest");
            stopViewerLoading(String(attemptId));
            setViewerError("ver_documento: No se recibió DocumentResolveRequest.");
            return;
          }
          dvLog(attemptKey, "ver_documento ok", {
            IdDocumento: result.documentResolveRequest.IdDocumento,
            NombreGabinete: result.documentResolveRequest.NombreGabinete,
          });
          setActiveRowId(rowId);
          dvLog(attemptKey, "orchestrator.visualizarDocumento start");
          void documentViewer.visualizarDocumento({
            documentId: result.documentResolveRequest.IdDocumento,
            nombreGabinete: result.documentResolveRequest.NombreGabinete,
            attemptId,
            // Clave estable por documento (sin attemptId) para features por-documento en runtime (p.ej. rotación).
            documentKey: `${result.documentResolveRequest.NombreGabinete}:${result.documentResolveRequest.IdDocumento}`,
            context: typeof idTareaWf === "number" ? { idTareaWorkflow: idTareaWf } : undefined,
          });
        } catch (err) {
          if (seq !== viewSeqRef.current) return;
          if (isCancelledError(err)) return;
          dvLog(attemptKey, "ver_documento failed", err);
          const cause = extractBackendCause(err);
          if (cause?.http) {
            setViewerError(`ver_documento: ${cause.message} (HTTP ${cause.http}).`);
            stopViewerLoading(String(attemptId));
            return;
          }
          setViewerError(`ver_documento: ${cause?.message ?? "No fue posible ejecutar la acción."}`);
          stopViewerLoading(String(attemptId));
        } finally {
          if (openTimeoutRef.current) window.clearTimeout(openTimeoutRef.current);
          openTimeoutRef.current = null;
        }
      })();
    },
    [documentViewer, documentosTable, idTareaWf, startViewerLoading, stopViewerLoading],
  );

  const onSaveAnnotatedPages = useCallback(() => {
    const ctx = documentosTable.getWorkbenchContext?.();
    const active = documentContext;
    if (!active) {
      toast.error("Selecciona un documento PDF antes de guardar paginas anotadas.");
      return;
    }
    if (!active.isPdf || active.viewerKind !== "pdf") {
      toast.error("El reemplazo de paginas anotadas solo aplica para documentos PDF.");
      return;
    }
    if (active.isElectronicallySigned) {
      toast.error("No se pueden reemplazar paginas de un documento firmado electronicamente.");
      return;
    }
    if (!active.nombreGabinete || !active.documentId) {
      toast.error("No fue posible identificar el documento de gabinete activo.");
      return;
    }
    if (!visorRef.current) {
      toast.error("El visor PDF no esta listo.");
      return;
    }

    replacementAbortRef.current?.abort();
    const abortController = new AbortController();
    replacementAbortRef.current = abortController;
    replacementSeqRef.current += 1;
    const seq = replacementSeqRef.current;

    void (async () => {
      const createdTemporals: Array<{ rutaTemporalId: string; archivoTemporalId: string }> = [];
      let replacementSucceeded = false;
      let replacementStage = "start";

      setIsReplacingAnnotatedPages(true);
      setReplacementProgress(0);
      try {
        replacementStage = "exportAnnotatedPdfPages";
        dvLog("[DV][reemplazo-paginas][start]", {
          documentId: active.documentId,
          nombreGabinete: active.nombreGabinete,
          documentKey: active.documentKey,
          isPdf: active.isPdf,
          viewerKind: active.viewerKind,
        });
        const exported = await visorRef.current!.exportAnnotatedPdfPages({
          calculateHashSha256: true,
          signal: abortController.signal,
        });
        dvLog("[DV][reemplazo-paginas][exported]", {
          hasAnnotations: exported.hasAnnotations,
          pageNumbers: exported.pageNumbers,
          pages: exported.pages.map((page) => ({
            pageNumber: page.pageNumber,
            fileName: page.fileName,
            sizeBytes: page.sizeBytes,
            hashSha256: page.hashSha256,
          })),
        });
        if (seq !== replacementSeqRef.current) return;
        if (exported.pages.length === 0) {
          toast.error("No hay paginas anotadas para reemplazar.");
          return;
        }
        setReplacementProgress(0.15);

        const uploads = [];
        for (const [pageIndex, page] of exported.pages.entries()) {
          replacementStage = `uploadAnnotatedPagePdf:${page.pageNumber}`;
          dvLog("[DV][reemplazo-paginas][upload:start]", {
            pageNumber: page.pageNumber,
            fileName: page.fileName,
            sizeBytes: page.sizeBytes,
            hashSha256: page.hashSha256,
          });
          const upload = await uploadAnnotatedPagePdf(
            {
              pageNumber: page.pageNumber,
              fileName: page.fileName,
              blob: page.blob,
              hashSha256: page.hashSha256,
            },
            abortController.signal,
          );
          createdTemporals.push({
            rutaTemporalId: upload.rutaTemporalId,
            archivoTemporalId: upload.archivoTemporalId,
          });
          uploads.push(upload);
          dvLog("[DV][reemplazo-paginas][upload:done]", upload);
          setReplacementProgress(0.15 + ((pageIndex + 1) / exported.pages.length) * 0.65);
        }

        if (seq !== replacementSeqRef.current) return;

        const originalPdfPassword = visorRef.current?.getOriginalPdfPassword();
        replacementStage = "reemplazarPaginasPdfAnotadas";
        dvLog("[DV][reemplazo-paginas][final-request]", {
          NombreGabinete: active.nombreGabinete,
          IdDocumento: active.documentId,
          RutaTemporalId: uploads[0]?.rutaTemporalId,
          OriginalPdfPassword: Boolean(originalPdfPassword),
          Paginas: uploads.map((upload) => ({
            PageNumber: upload.pageNumber,
            RutaTemporalId: upload.rutaTemporalId,
            ArchivoTemporalId: upload.archivoTemporalId,
            ContentType: "application/pdf",
            HashSha256Esperado: upload.hashSha256 ?? null,
          })),
          Radicado: ctx?.radicado,
          IdTareaWorkflow: typeof idTareaWf === "number" ? idTareaWf : 0,
        });
        const response = await reemplazarPaginasPdfAnotadas(
          {
            NombreGabinete: active.nombreGabinete,
            IdDocumento: active.documentId,
            RutaTemporalId: uploads[0]?.rutaTemporalId,
            ...(originalPdfPassword ? { OriginalPdfPassword: originalPdfPassword } : {}),
            Paginas: uploads.map((upload) => ({
              PageNumber: upload.pageNumber,
              RutaTemporalId: upload.rutaTemporalId,
              ArchivoTemporalId: upload.archivoTemporalId,
              ContentType: "application/pdf",
              HashSha256Esperado: upload.hashSha256 ?? null,
            })),
            Motivo: "Actualizacion de grafo PDF desde visor",
            DescOp: "AGREGA GRAFO PDF",
            ModuloRegistro: "DOCUARCHI",
            Radicado: ctx?.radicado,
            IdTareaWorkflow: typeof idTareaWf === "number" ? idTareaWf : 0,
          },
          { signal: abortController.signal },
        );
        replacementSucceeded = true;
        setReplacementProgress(1);
        dvLog("[DV][reemplazo-paginas][final-response]", response);

        if (seq !== replacementSeqRef.current) return;

        const pages = response.PaginasReemplazadas.join(", ");
        toast.success(`Paginas ${pages} actualizadas correctamente. RequestId: ${response.RequestId}`);

        attemptIdRef.current += 1;
        const refreshAttemptId = attemptIdRef.current;
        lastVisorLoadKeyRef.current = null;
        startViewerLoading(String(refreshAttemptId));
        void documentViewer.visualizarDocumento({
          documentId: active.documentId,
          nombreGabinete: active.nombreGabinete,
          attemptId: refreshAttemptId,
          documentKey: active.documentKey ?? `${active.nombreGabinete}:${active.documentId}`,
          context: typeof idTareaWf === "number" ? { idTareaWorkflow: idTareaWf, radicado: ctx?.radicado } : undefined,
        });
      } catch (err) {
        if (!replacementSucceeded) {
          await cancelTemporalsBestEffort(createdTemporals);
        }
        if (isCancelledError(err)) return;
        dvLog("[DV][reemplazo-paginas][failed]", {
          stage: replacementStage,
          createdTemporals,
          error: summarizeReplacementError(err),
        });
        toast.error(getReplacementErrorMessage(err));
      } finally {
        if (replacementAbortRef.current === abortController) {
          replacementAbortRef.current = null;
        }
        if (seq === replacementSeqRef.current) {
          setIsReplacingAnnotatedPages(false);
          setReplacementProgress(undefined);
        }
      }
    })();
  }, [
    cancelTemporalsBestEffort,
    documentContext,
    documentViewer,
    documentosTable,
    idTareaWf,
    startViewerLoading,
    uploadAnnotatedPagePdf,
  ]);

  const activeDocumentOperationKey = `${documentContext?.documentKey ?? ""}:${documentContext?.documentId ?? ""}`;

  useEffect(() => {
    replacementSeqRef.current += 1;
    replacementAbortRef.current?.abort();
    replacementAbortRef.current = null;
    setIsReplacingAnnotatedPages(false);
    setReplacementProgress(undefined);
  }, [activeDocumentOperationKey]);

  useEffect(
    () => () => {
      replacementSeqRef.current += 1;
      replacementAbortRef.current?.abort();
      replacementAbortRef.current = null;
    },
    [],
  );

  useEffect(() => {
    if (variant !== "overlay") return;
    const root = rootRef.current;
    if (!root || typeof MutationObserver === "undefined") return;

    const tabPane = root.closest(".ant-tabs-tabpane") as HTMLElement | null;
    if (!tabPane) return;

    const isHidden = () => tabPane.classList.contains("ant-tabs-tabpane-hidden");

    const observer = new MutationObserver(() => {
      if (isHidden()) {
        setCollapsed(true);
      }
    });

    observer.observe(tabPane, {
      attributes: true,
      attributeFilter: ["class", "style"],
    });

    return () => observer.disconnect();
  }, [variant]);

  const isSaveAnnotatedPagesDisabled =
    isReplacingAnnotatedPages ||
    !documentContext ||
    documentContext.viewerKind !== "pdf" ||
    !documentContext.isPdf ||
    Boolean(documentContext.isElectronicallySigned);

  return (
    <section
      ref={(node) => {
        rootRef.current = node;
      }}
      className={styles.workbenchBody}
      aria-label="Workbench de documentos"
      data-collapsed={layoutCollapsed}
      data-variant={variant}
      data-testid="documentos-workbench"
    >
      <div className={styles.viewer}>
        {documentViewer.documentoActivo?.viewerKind === "pdf" ? (
          <AppVisorEmbedPdf
            ref={visorRef}
            fileUrl={activeFileUrl}
            loading={showViewerLoading}
            onEmptyDocumentHintRequest={triggerDocumentListHint}
            onSaveAnnotatedPages={onSaveAnnotatedPages}
            isSaveAnnotatedPagesDisabled={isSaveAnnotatedPagesDisabled}
            isSavingAnnotatedPages={isReplacingAnnotatedPages}
            saveAnnotatedPagesProgress={replacementProgress}
          />
        ) : documentViewer.documentoActivo?.viewerKind === "image" && activeFileUrl ? (
          <img
            src={activeFileUrl}
            alt="Documento"
            style={{ width: "100%", height: "100%", objectFit: "contain", background: "#fff" }}
            onLoad={() => setShowViewerLoading(false)}
            onError={() => setShowViewerLoading(false)}
          />
        ) : (
          <AppVisorEmbedPdf
            ref={visorRef}
            fileUrl={activeFileUrl}
            loading={showViewerLoading}
            onEmptyDocumentHintRequest={triggerDocumentListHint}
            onSaveAnnotatedPages={onSaveAnnotatedPages}
            isSaveAnnotatedPagesDisabled={isSaveAnnotatedPagesDisabled}
            isSavingAnnotatedPages={isReplacingAnnotatedPages}
            saveAnnotatedPagesProgress={replacementProgress}
          />
        )}
      </div>

      <AppCollapseRail
        title="Documentos"
        collapsed={collapsed}
        onToggle={() => setCollapsed((prev) => !prev)}
        placement="right"
        variant={variant}
        panelId={panelId}
        railLabel="Documentos"
        railIcon={<BookOutlined />}
        className={styles.collapseRail}
      >
        <div className={styles.listPanel}>
          <header className={styles.listHeader}>
            <h3 className={styles.listTitle}>{documentsCounter}</h3>
            <AppButton
              variant="ghost"
              size="sm"
              onClick={() => setCollapsed((prev) => !prev)}
              aria-label={layoutCollapsed ? "Mostrar documentos" : "Ocultar documentos"}
              icon={toggleIcon}
              className={styles.collapseButton}
            />
          </header>
          <div
            className={styles.listSurface}
            aria-label="Listado de documentos"
            data-document-hint-active={documentHintActive}
          >
          <AppTreeTable
              load={documentosTable.load}
              loadChildren={documentosTable.loadChildren}
              tableColumns={documentosTable.getTableColumns()}
              columns={documentosTable.getColumns()}
              tableLayoutMode="fill"
              rowClickAffordance
              rowClickTooltip="Visualizar documento"
              rowSelection="multiple"
              rowSelectionCheckboxes
              rowSelectionHeaderCheckbox
              suppressRowClickSelection={false}
              onSelectionChanged={documentosTable.onSelectionChanged}
              activeRowId={activeRowId}
              onSelectRow={openViewerFromRow}
              onActionTriggered={(params) => {
                if (params.actionId === "ver_documento") {
                  openViewerFromRow(params.rowId);
                  return;
                }

                void documentosTable.onActionTriggered({ actionId: params.actionId, rowId: params.rowId });
              }}
              emptyMessage="Sin documentos adjuntos."
            />
          </div>
        </div>
      </AppCollapseRail>
    </section>
  );
}
