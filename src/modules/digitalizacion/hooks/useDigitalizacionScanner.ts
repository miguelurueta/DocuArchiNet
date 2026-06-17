import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import {
  DynamsoftScannerError,
  toDynamsoftScannerError,
} from "../infrastructure/dynamsoft";
import type {
  DigitalizacionScannerClient,
  PdfGenerationResult,
  ScanOptions,
  ScanPage,
  ScannerDevice,
} from "../infrastructure/dynamsoft";

export type DigitalizacionScannerStatus =
  | "idle"
  | "initializing"
  | "ready"
  | "scanning"
  | "generatingPdf"
  | "error";

export type DigitalizacionScannerHookState = {
  status: DigitalizacionScannerStatus;
  devices: ScannerDevice[];
  selectedDeviceId: string | null;
  pages: ScanPage[];
  pdf: PdfGenerationResult | null;
  error: DynamsoftScannerError | null;
};

const initialState: DigitalizacionScannerHookState = {
  status: "idle",
  devices: [],
  selectedDeviceId: null,
  pages: [],
  pdf: null,
  error: null,
};

export const useDigitalizacionScanner = ({
  client,
}: {
  client: DigitalizacionScannerClient;
}) => {
  const mountedRef = useRef(true);
  const generationRef = useRef(0);
  const [state, setState] = useState<DigitalizacionScannerHookState>(initialState);
  const stateRef = useRef<DigitalizacionScannerHookState>(initialState);

  useEffect(() => {
    stateRef.current = state;
  }, [state]);

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
      error: null,
    }));

    try {
      console.log("STATE_BEFORE", stateRef.current);
      await client.initialize();
      const devices = await client.listDevices();
      devices.forEach((device) => {
        console.debug("[DigitalizacionScanner]", "initialize.devices", {
          scannerName: device.name,
          scannerIndex: device.index,
        });
      });
      updateIfCurrent(generation, (current) => ({
        ...current,
        status: "ready",
        devices,
        error: null,
      }));
      console.log("STATE_AFTER", {
        ...stateRef.current,
        status: "ready",
        devices,
        error: null,
      });
    } catch (error) {
      handleError(generation, error, "No fue posible inicializar el scanner.");
    }
  }, [client, handleError, updateIfCurrent]);

  const selectDevice = useCallback(
    async (deviceId: string) => {
      const generation = generationRef.current;
      console.log("HOOK_SELECT_DEVICE", deviceId);
      const selectedDevice = stateRef.current.devices.find((device) => device.id === deviceId);
      console.debug("[DigitalizacionScanner]", "selectDevice.request", {
        scannerName: selectedDevice?.name ?? "",
        scannerIndex: selectedDevice?.index ?? Number(deviceId),
      });
      try {
        console.log("STATE_BEFORE", stateRef.current);
        await client.selectDevice(deviceId);
        updateIfCurrent(generation, (current) => ({
          ...current,
          selectedDeviceId: deviceId,
          error: null,
        }));
        console.log("STATE_AFTER", {
          ...stateRef.current,
          selectedDeviceId: deviceId,
          error: null,
        });
      } catch (error) {
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
        error: null,
      }));

      try {
        const pages = await client.scan(options);
        updateIfCurrent(generation, (current) => ({
          ...current,
          status: "ready",
          pages,
          pdf: null,
          error: null,
        }));
      } catch (error) {
        handleError(generation, error, "No fue posible completar el escaneo.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const removePage = useCallback(
    async (pageId: string) => {
      const generation = generationRef.current;
      try {
        await client.removePage(pageId);
        updateIfCurrent(generation, (current) => ({
          ...current,
          pages: current.pages.filter((page) => page.id !== pageId),
          pdf: null,
          error: null,
        }));
      } catch (error) {
        handleError(generation, error, "No fue posible remover la pagina.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const reorderPages = useCallback(
    async (pageIds: string[]) => {
      const generation = generationRef.current;
      try {
        const pages = await client.reorderPages(pageIds);
        updateIfCurrent(generation, (current) => ({
          ...current,
          pages,
          pdf: null,
          error: null,
        }));
      } catch (error) {
        handleError(generation, error, "No fue posible reordenar las paginas.");
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const rotatePage = useCallback(
    async (pageId: string, degrees: 90 | 180 | 270) => {
      const generation = generationRef.current;
      try {
        await client.rotatePage(pageId, degrees);
        updateIfCurrent(generation, (current) => ({ ...current, error: null }));
      } catch (error) {
        handleError(generation, error, "No fue posible rotar la pagina.");
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
        error: null,
      }));
    } catch (error) {
      handleError(generation, error, "No fue posible limpiar el scanner.");
    }
  }, [client, handleError, updateIfCurrent]);

  const generatePdf = useCallback(
    async (fileName: string) => {
      const generation = generationRef.current;
      updateIfCurrent(generation, (current) => ({
        ...current,
        status: "generatingPdf",
        error: null,
      }));

      try {
        const pdf = await client.generatePdf(fileName);
        updateIfCurrent(generation, (current) => ({
          ...current,
          status: "ready",
          pdf,
          error: null,
        }));
        return pdf;
      } catch (error) {
        handleError(generation, error, "No fue posible generar el PDF.");
        return null;
      }
    },
    [client, handleError, updateIfCurrent],
  );

  const dispose = useCallback(async () => {
    console.log("USE_DIGITALIZACION_SCANNER_DISPOSE_CALL");
    generationRef.current += 1;
    await client.dispose();
    if (mountedRef.current) {
      setState(initialState);
    }
  }, [client]);

  useEffect(() => {
    console.log("USE_DIGITALIZACION_SCANNER_EFFECT_MOUNT", {
      client,
    });
    mountedRef.current = true;

    return () => {
      console.log("USE_DIGITALIZACION_SCANNER_EFFECT_CLEANUP", {
        client,
        stack: new Error("USE_DIGITALIZACION_SCANNER_EFFECT_CLEANUP_STACK").stack,
      });
      mountedRef.current = false;
      generationRef.current += 1;
      void client.dispose();
    };
  }, [client]);

  const loading = useMemo(
    () =>
      state.status === "initializing" ||
      state.status === "scanning" ||
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
    rotatePage,
    clear,
    generatePdf,
    dispose,
  };
};
