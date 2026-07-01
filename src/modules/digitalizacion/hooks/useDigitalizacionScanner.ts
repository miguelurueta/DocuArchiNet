import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import {
  DynamsoftScannerError,
  toDynamsoftScannerError,
} from "../infrastructure/dynamsoft";
import type {
  DigitalizacionScannerClient,
  PdfGenerationResult,
  PageCropSelection,
  ScanOptions,
  ScanPage,
  ScanProgressSnapshot,
  ScannerDevice,
} from "../infrastructure/dynamsoft";

export type DigitalizacionScannerStatus =
  | "idle"
  | "initializing"
  | "ready"
  | "scanning"
  | "processingPage"
  | "generatingPdf"
  | "error";

export type DigitalizacionScannerHookState = {
  status: DigitalizacionScannerStatus;
  devices: ScannerDevice[];
  selectedDeviceId: string | null;
  pages: ScanPage[];
  pdf: PdfGenerationResult | null;
  progress: ScanProgressSnapshot | null;
  error: DynamsoftScannerError | null;
};

const initialState: DigitalizacionScannerHookState = {
  status: "idle",
  devices: [],
  selectedDeviceId: null,
  pages: [],
  pdf: null,
  progress: null,
  error: null,
};

const getMetricStart = () => performance.now();

const logDevelopmentMetric = (
  label:
    | "SCAN_SELECTION_TIME"
    | "CROP_TIME"
    | "DESKEW_TIME"
    | "DUPLICATE_TIME"
    | "ROTATE_TIME"
    | "DELETE_TIME"
    | "REORDER_TIME"
    | "PDF_GENERATION_TIME",
  startedAt: number,
  metadata?: Record<string, unknown>,
) => {
  if (!import.meta.env.DEV) {
    return;
  }

  console.info(label, {
    durationMs: Math.round(performance.now() - startedAt),
    ...metadata,
  });
};

export const useDigitalizacionScanner = ({
  client,
}: {
  client: DigitalizacionScannerClient;
}) => {
  const mountedRef = useRef(true);
  const generationRef = useRef(0);
  const [state, setState] = useState<DigitalizacionScannerHookState>(initialState);

  const updateIfCurrent = useCallback(
    (
      generation: number,
      updater: (current: DigitalizacionScannerHookState) => DigitalizacionScannerHookState,
    ) => {
      if (!mountedRef.current || generation !== generationRef.current) {
        return;
      }

      setState(updater);
    },
    [],
  );

  const handleError = useCallback(
    (generation: number, error: unknown, fallbackMessage: string) => {
      const scannerError = toDynamsoftScannerError(error, "SCAN_FAILED", fallbackMessage);
      updateIfCurrent(generation, (current) => ({
        ...current,
        status: "error",
        progress: null,
        error: scannerError,
      }));
      return scannerError;
    },
    [updateIfCurrent],
  );

  const initialize = useCallback(async () => {
    const generation = generationRef.current;
    updateIfCurrent(generation, (current) => ({
      ...current,
      status: "initializing",
      progress: {
        stage: "preparingDocument",
        label: "Preparando scanner",
        detail: "Cargando recursos de captura.",
        cancellable: false,
      },
      error: null,
    }));

    try {
      await client.initialize();
      const devices = await client.listDevices();
      updateIfCurrent(generation, (current) => ({
        ...current,
        status: "ready",
        devices,
        progress: null,
        error: null,
      }));
    } catch (error) {
      handleError(generation, error, "No fue posible inicializar el scanner.");
    }
  }, [client, handleError, updateIfCurrent]);

  const selectDevice = useCallback(
    async (deviceId: string) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      try {
        await client.selectDevice(deviceId);
        updateIfCurrent(generation, (current) => ({
          ...current,
          selectedDeviceId: deviceId,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("SCAN_SELECTION_TIME", startedAt, { status: "success" });
      } catch (error) {
        logDevelopmentMetric("SCAN_SELECTION_TIME", startedAt, { status: "error" });
        handleError(generation, error, "No fue posible seleccionar el scanner.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const scan = useCallback(
    async (options: ScanOptions) => {
      const generation = generationRef.current;
      updateIfCurrent(generation, (current) => ({
        ...current,
        status: "scanning",
        progress: {
          stage: "acquiring",
          label: "Escaneando documentos",
          detail: "Esperando paginas desde el driver.",
          cancellable: true,
        },
        error: null,
      }));

      try {
        console.log("[1] before await client.scan");
        const pages = await client.scan({
          ...options,
          onProgress: (progress) => {
            updateIfCurrent(generation, (current) => ({
              ...current,
              progress,
            }));
            options.onProgress?.(progress);
          },
        });
        console.log("[2] after client.scan", pages);
        console.log("[3] before updateIfCurrent");
        updateIfCurrent(generation, (current) => ({
          ...current,
          status: "ready",
          pages,
          pdf: null,
          progress: null,
          error: null,
        }));
        console.log("[4] after updateIfCurrent");
        console.log("[5] before return");
      } catch (error) {
        console.error("[SCAN CATCH]", error);
        console.error(error instanceof Error ? error.stack : error);
        handleError(generation, error, "No fue posible completar el escaneo.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const removePage = useCallback(
    async (pageId: string) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      try {
        await client.removePage(pageId);
        updateIfCurrent(generation, (current) => ({
          ...current,
          pages: current.pages.filter((page) => page.id !== pageId),
          pdf: null,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("DELETE_TIME", startedAt, { status: "success" });
      } catch (error) {
        logDevelopmentMetric("DELETE_TIME", startedAt, { status: "error" });
        handleError(generation, error, "No fue posible remover la pagina.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const reorderPages = useCallback(
    async (pageIds: string[]) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      try {
        const pages = await client.reorderPages(pageIds);
        updateIfCurrent(generation, (current) => ({
          ...current,
          pages,
          pdf: null,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("REORDER_TIME", startedAt, {
          status: "success",
          pageCount: pageIds.length,
        });
      } catch (error) {
        logDevelopmentMetric("REORDER_TIME", startedAt, { status: "error" });
        handleError(generation, error, "No fue posible reordenar las paginas.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const duplicatePage = useCallback(
    async (pageId: string) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      try {
        const pages = await client.duplicatePage(pageId);
        updateIfCurrent(generation, (current) => ({
          ...current,
          pages,
          pdf: null,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("DUPLICATE_TIME", startedAt, { status: "success" });
        return pages;
      } catch (error) {
        logDevelopmentMetric("DUPLICATE_TIME", startedAt, { status: "error" });
        handleError(generation, error, "No fue posible duplicar la pagina.");
        return null;
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const rotatePage = useCallback(
    async (pageId: string, degrees: 90 | 180 | 270) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      try {
        const pages = await client.rotatePage(pageId, degrees);
        updateIfCurrent(generation, (current) => ({
          ...current,
          pages,
          pdf: null,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("ROTATE_TIME", startedAt, {
          status: "success",
          degrees,
        });
      } catch (error) {
        logDevelopmentMetric("ROTATE_TIME", startedAt, { status: "error", degrees });
        handleError(generation, error, "No fue posible rotar la pagina.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const deskewPage = useCallback(
    async (pageId: string) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      updateIfCurrent(generation, (current) => ({
        ...current,
        status: "processingPage",
        progress: {
          stage: "applyingDeskew",
          label: "Corrigiendo inclinacion",
          detail: "Procesando pagina capturada.",
          cancellable: false,
        },
        error: null,
      }));

      try {
        const pages = await client.deskewPage(pageId);
        updateIfCurrent(generation, (current) => ({
          ...current,
          status: "ready",
          pages,
          pdf: null,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("DESKEW_TIME", startedAt, { status: "success" });
      } catch (error) {
        logDevelopmentMetric("DESKEW_TIME", startedAt, { status: "error" });
        handleError(generation, error, "No fue posible corregir la inclinacion.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const cropPage = useCallback(
    async (pageId: string, selection: PageCropSelection) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      try {
        const pages = await client.cropPage(pageId, selection);
        updateIfCurrent(generation, (current) => ({
          ...current,
          pages,
          pdf: null,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("CROP_TIME", startedAt, { status: "success" });
      } catch (error) {
        logDevelopmentMetric("CROP_TIME", startedAt, { status: "error" });
        handleError(generation, error, "No fue posible recortar la pagina.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const clear = useCallback(async () => {
    const generation = generationRef.current;
    try {
      await client.clear();
      updateIfCurrent(generation, (current) => ({
        ...current,
        pages: [],
        pdf: null,
        progress: null,
        error: null,
      }));
    } catch (error) {
      handleError(generation, error, "No fue posible limpiar el scanner.");
    }
  }, [client, handleError, updateIfCurrent]);

  const generatePdf = useCallback(
    async (fileName: string) => {
      const generation = generationRef.current;
      const startedAt = getMetricStart();
      updateIfCurrent(generation, (current) => ({
        ...current,
        status: "generatingPdf",
        progress: {
          stage: "generatingPdf",
          label: "Generando PDF",
          detail: "Construyendo el documento final.",
          totalPages: current.pages.length,
          progress: 85,
          cancellable: false,
        },
        error: null,
      }));

      try {
        const pdf = await client.generatePdf(fileName);
        updateIfCurrent(generation, (current) => ({
          ...current,
          status: "ready",
          pdf,
          progress: null,
          error: null,
        }));
        logDevelopmentMetric("PDF_GENERATION_TIME", startedAt, {
          status: "success",
          pageCount: pdf.pageCount,
        });
        return pdf;
      } catch (error) {
        logDevelopmentMetric("PDF_GENERATION_TIME", startedAt, { status: "error" });
        handleError(generation, error, "No fue posible generar el PDF.");
        return null;
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const dispose = useCallback(async () => {
    generationRef.current += 1;
    await client.dispose();
    if (mountedRef.current) {
      setState(initialState);
    }
  }, [client]);

  useEffect(() => {
    mountedRef.current = true;

    return () => {
      mountedRef.current = false;
      generationRef.current += 1;
      void client.dispose();
    };
  }, [client]);

  const loading = useMemo(
    () =>
      state.status === "initializing" ||
      state.status === "scanning" ||
      state.status === "processingPage" ||
      state.status === "generatingPdf",
    [state.status],
  );

  return {
    ...state,
    loading,
    initialize,
    selectDevice,
    scan,
    removePage,
    reorderPages,
    duplicatePage,
    rotatePage,
    deskewPage,
    cropPage,
    clear,
    generatePdf,
    dispose,
  };
};
