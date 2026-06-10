import { useCallback, useMemo, useRef, useState } from "react";
import type {
  AppDocumentViewerOrchestratorInput,
  AppDocumentViewerRuntimeState,
  FirmaCheckStatus,
} from "./AppDocumentViewerOrchestrator.types";
import axios from "axios";
import {
  buildInitialRuntimeState,
  buildResolvedRuntimeState,
  isPdfFromContentType,
  pickResolvedFileUrl,
} from "./AppDocumentViewerOrchestrator.adapter";
import {
  downloadVisualizacionBlob,
  fetchFirmaElectronica,
  resolveVisualizacionDocumento,
} from "./AppDocumentViewerOrchestrator.service";

type OrchestratorState = {
  documentoActivo: AppDocumentViewerRuntimeState | null;
  loading: boolean;
  error: string | null;
};

type ApiErrorItem = { Message?: unknown };
type ApiEnvelope = { message?: unknown; errors?: unknown };

function extractApiErrorMessage(err: unknown): string | null {
  if (!axios.isAxiosError(err)) return null;
  const data = err.response?.data as ApiEnvelope | undefined;

  const errors = Array.isArray(data?.errors) ? (data?.errors as unknown[]) : null;
  const first = errors?.[0] as ApiErrorItem | undefined;
  if (typeof first?.Message === "string" && first.Message.trim()) return first.Message.trim();

  if (typeof data?.message === "string" && data.message.trim()) return data.message.trim();

  return null;
}

const buildErrorSignal = (err: unknown): string => {
  if (err instanceof DOMException && err.name === "AbortError") return "cancelled";
  if (err instanceof Error) return err.message || "error";
  return "error";
};

const dvDebugEnabled = (): boolean =>
  typeof window !== "undefined" && Boolean((window as any).__DV_DEBUG__);

const dvLog = (...args: unknown[]) => {
  if (!dvDebugEnabled()) return;
  // eslint-disable-next-line no-console
  console.log(...args);
};

export function useDocumentViewerOrchestrator() {
  const [state, setState] = useState<OrchestratorState>({
    documentoActivo: null,
    loading: false,
    error: null,
  });

  const requestIdRef = useRef(0);
  const abortRef = useRef<AbortController | null>(null);
  const blobUrlRef = useRef<string | null>(null);
  const revokeTimersRef = useRef<number[]>([]);

  const scheduleRevoke = useCallback((url: string) => {
    // Guardrail: revocar diferido para evitar invalidar el blob mientras el visor aún lo consume.
    // En escenarios de clicks rápidos o PDFs grandes, el engine puede seguir leyendo bytes
    // del blob anterior por unos instantes.
    const id = window.setTimeout(() => {
      try {
        URL.revokeObjectURL(url);
      } catch {
        // noop
      }
    }, 5000);
    revokeTimersRef.current.push(id);
  }, []);

  const cancelCurrentRequest = useCallback(() => {
    abortRef.current?.abort();
    abortRef.current = null;
    setState((prev) => ({
      ...prev,
      loading: false,
      error: null,
      documentoActivo: prev.documentoActivo
        ? { ...prev.documentoActivo, resolveStatus: "cancelled", errors: [] }
        : prev.documentoActivo,
    }));
  }, []);

  const reset = useCallback(() => {
    cancelCurrentRequest();
    revokeTimersRef.current.forEach((id) => window.clearTimeout(id));
    revokeTimersRef.current = [];
    if (blobUrlRef.current) {
      URL.revokeObjectURL(blobUrlRef.current);
      blobUrlRef.current = null;
    }
    setState({ documentoActivo: null, loading: false, error: null });
  }, [cancelCurrentRequest]);

  const visualizarDocumento = useCallback(async (input: AppDocumentViewerOrchestratorInput) => {
    requestIdRef.current += 1;
    const requestId = requestIdRef.current;
    const attemptKey = `[DV][attempt:${input.attemptId ?? "na"}][req:${requestId}]`;

    abortRef.current?.abort();
    const abortController = new AbortController();
    abortRef.current = abortController;

    setState((prev) => {
      const base = buildInitialRuntimeState(input);
      const nextActive: AppDocumentViewerRuntimeState =
        prev.documentoActivo && prev.documentoActivo.fileUrl
          ? { ...prev.documentoActivo, resolveStatus: "loading", errors: [] }
          : { ...base, resolveStatus: "loading" };
      return { documentoActivo: nextActive, loading: true, error: null };
    });

    try {
      dvLog(attemptKey, "resolve start", { documentId: input.documentId, nombreGabinete: input.nombreGabinete });
      const resolveDto = await resolveVisualizacionDocumento({
        request: { IdDocumento: input.documentId, NombreGabinete: input.nombreGabinete },
        signal: abortController.signal,
      });

      if (requestId !== requestIdRef.current) return;
      dvLog(attemptKey, "resolve ok", {
        documentId: resolveDto.IdDocumento,
        nombreGabinete: resolveDto.NombreGabinete,
        contentType: resolveDto.ContentType,
        fileName: resolveDto.FileName,
      });

      const tokenUrl = pickResolvedFileUrl(resolveDto);
      if (!tokenUrl) {
        // No hay URL; tratar como fallo lógico sin romper estabilidad del documento previo.
        setState((prev) => ({
          ...prev,
          loading: false,
          error: "No fue posible resolver la URL del documento.",
          documentoActivo: prev.documentoActivo
            ? { ...prev.documentoActivo, resolveStatus: "failed", errors: ["RESOLVE_FAILED"] }
            : prev.documentoActivo,
        }));
        return;
      }

      // Descargar autenticado como Blob para evitar 401/403 en download/{token} por falta de credenciales.
      dvLog(attemptKey, "download blob start");
      const blob = await downloadVisualizacionBlob({ fileUrl: tokenUrl, signal: abortController.signal });
      if (requestId !== requestIdRef.current) return;
      dvLog(attemptKey, "download blob ok", { blobSize: blob.size, blobType: blob.type });

      const previousBlobUrl = blobUrlRef.current;
      const fileUrl = URL.createObjectURL(blob);
      blobUrlRef.current = fileUrl;
      dvLog(attemptKey, "blobUrl created", { fileUrl, previousBlobUrl });
      const contentType = resolveDto.ContentType ?? null;
      const isPdf = isPdfFromContentType(contentType, resolveDto.FileName);
      // Estado consolidado tras resolve (firma por defecto se define después).
      setState((prev) => ({
        ...prev,
        loading: true,
        error: null,
        documentoActivo: buildResolvedRuntimeState({
          input,
          resolve: resolveDto,
          fileUrlOverride: fileUrl,
          resolveStatus: "resolved",
          firmaCheckStatus: isPdf ? "resolved" : "not_required",
          isElectronicallySigned: isPdf ? null : null,
        }),
      }));

      // Cleanup diferido: evitar revocar el blobUrl que aún podría estar siendo consumido
      // por el visor hasta que React pinte el nuevo `fileUrl`.
      if (previousBlobUrl && previousBlobUrl !== fileUrl) scheduleRevoke(previousBlobUrl);
      if (previousBlobUrl && previousBlobUrl !== fileUrl) dvLog(attemptKey, "schedule revoke previousBlobUrl", previousBlobUrl);

      if (!fileUrl) {
        // No hay URL; tratar como fallo lógico sin romper estabilidad del documento previo.
        setState((prev) => ({
          ...prev,
          loading: false,
          error: "No fue posible resolver la URL del documento.",
          documentoActivo: prev.documentoActivo
            ? { ...prev.documentoActivo, resolveStatus: "failed", errors: ["RESOLVE_FAILED"] }
            : prev.documentoActivo,
        }));
        return;
      }

      if (!isPdf) {
        dvLog(attemptKey, "not pdf -> skip firma");
        setState((prev) => ({ ...prev, loading: false, error: null }));
        return;
      }

      // Firma electrónica: solo PDF y no bloquea visualización.
      dvLog(attemptKey, "firma start");
      let firmaCheckStatus: FirmaCheckStatus = "resolved";
      let signed: boolean | null = null;
      try {
        const firmaDto = await fetchFirmaElectronica({
          idArchivo: resolveDto.IdDocumento,
          nombreGabinete: resolveDto.NombreGabinete,
          signal: abortController.signal,
        });
        signed = Boolean(firmaDto.FirmadoElectronico);
        dvLog(attemptKey, "firma ok", { signed });
      } catch (err) {
        firmaCheckStatus = err instanceof DOMException && err.name === "AbortError" ? "failed" : "failed";
        signed = null;
        dvLog(attemptKey, "firma failed", err);
      }

      if (requestId !== requestIdRef.current) return;

      setState((prev) => ({
        ...prev,
        loading: false,
        error: null,
        documentoActivo: prev.documentoActivo
          ? {
              ...prev.documentoActivo,
              firmaCheckStatus,
              isElectronicallySigned: signed,
              // Si la firma falla, se conserva el documento visible (fileUrl ya está).
              errors:
                firmaCheckStatus === "failed"
                  ? Array.from(new Set([...(prev.documentoActivo.errors || []), "FIRMA_FAILED"]))
                  : prev.documentoActivo.errors,
            }
          : prev.documentoActivo,
      }));
    } catch (err) {
      if (requestId !== requestIdRef.current) return;
      const signal = buildErrorSignal(err);
      const apiMessage = extractApiErrorMessage(err);
      dvLog(attemptKey, "pipeline failed", { signal, apiMessage, err });
      setState((prev) => ({
        ...prev,
        loading: false,
        error: signal === "cancelled" ? null : apiMessage ?? "No fue posible resolver el documento.",
        documentoActivo: prev.documentoActivo
          ? {
              ...prev.documentoActivo,
              resolveStatus: signal === "cancelled" ? "cancelled" : "failed",
              errors:
                signal === "cancelled"
                  ? prev.documentoActivo.errors
                  : Array.from(
                      new Set([...(prev.documentoActivo.errors || []), ...(apiMessage ? [apiMessage] : []), "RESOLVE_FAILED"]),
                    ),
            }
          : prev.documentoActivo,
      }));
    } finally {
      if (requestId === requestIdRef.current) {
        abortRef.current = null;
      }
    }
  }, []);

  return useMemo(
    () => ({
      visualizarDocumento,
      documentoActivo: state.documentoActivo,
      loading: state.loading,
      error: state.error,
      reset,
      cancelCurrentRequest,
    }),
    [cancelCurrentRequest, reset, state.documentoActivo, state.error, state.loading, visualizarDocumento],
  );
}
