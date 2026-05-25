import { useCallback, useMemo, useRef, useState } from "react";
import type {
  AppDocumentViewerOrchestratorInput,
  AppDocumentViewerRuntimeState,
  FirmaCheckStatus,
} from "./AppDocumentViewerOrchestrator.types";
import {
  buildInitialRuntimeState,
  buildResolvedRuntimeState,
  isPdfFromContentType,
  pickResolvedFileUrl,
} from "./AppDocumentViewerOrchestrator.adapter";
import { fetchFirmaElectronica, resolveVisualizacionDocumento } from "./AppDocumentViewerOrchestrator.service";

type OrchestratorState = {
  documentoActivo: AppDocumentViewerRuntimeState | null;
  loading: boolean;
  error: string | null;
};

const buildErrorMessage = (err: unknown): string => {
  if (err instanceof DOMException && err.name === "AbortError") return "cancelled";
  if (err instanceof Error) return err.message || "error";
  return "error";
};

export function useDocumentViewerOrchestrator() {
  const [state, setState] = useState<OrchestratorState>({
    documentoActivo: null,
    loading: false,
    error: null,
  });

  const requestIdRef = useRef(0);
  const abortRef = useRef<AbortController | null>(null);

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
    setState({ documentoActivo: null, loading: false, error: null });
  }, [cancelCurrentRequest]);

  const visualizarDocumento = useCallback(async (input: AppDocumentViewerOrchestratorInput) => {
    requestIdRef.current += 1;
    const requestId = requestIdRef.current;

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
      const resolveDto = await resolveVisualizacionDocumento({
        request: { IdDocumento: input.documentId, NombreGabinete: input.nombreGabinete },
        signal: abortController.signal,
      });

      if (requestId !== requestIdRef.current) return;

      const fileUrl = pickResolvedFileUrl(resolveDto);
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
          resolveStatus: "resolved",
          firmaCheckStatus: isPdf ? "resolved" : "not_required",
          isElectronicallySigned: isPdf ? null : null,
        }),
      }));

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
        setState((prev) => ({ ...prev, loading: false, error: null }));
        return;
      }

      // Firma electrónica: solo PDF y no bloquea visualización.
      let firmaCheckStatus: FirmaCheckStatus = "resolved";
      let signed: boolean | null = null;
      try {
        const firmaDto = await fetchFirmaElectronica({
          idArchivo: resolveDto.IdDocumento,
          nombreGabinete: resolveDto.NombreGabinete,
          signal: abortController.signal,
        });
        signed = Boolean(firmaDto.FirmadoElectronico);
      } catch (err) {
        firmaCheckStatus = err instanceof DOMException && err.name === "AbortError" ? "failed" : "failed";
        signed = null;
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
      const message = buildErrorMessage(err);
      setState((prev) => ({
        ...prev,
        loading: false,
        error: message === "cancelled" ? null : "No fue posible resolver el documento.",
        documentoActivo: prev.documentoActivo
          ? {
              ...prev.documentoActivo,
              resolveStatus: message === "cancelled" ? "cancelled" : "failed",
              errors:
                message === "cancelled"
                  ? prev.documentoActivo.errors
                  : Array.from(new Set([...(prev.documentoActivo.errors || []), "RESOLVE_FAILED"])),
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
