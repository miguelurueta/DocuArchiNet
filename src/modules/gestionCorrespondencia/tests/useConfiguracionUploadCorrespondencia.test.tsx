import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useConfiguracionUploadCorrespondencia } from "../hooks/useConfiguracionUploadCorrespondencia";
import {
  ConfiguracionUploadCorrespondenciaError,
  getConfiguracionUploadCorrespondencia,
} from "../services/configuracionUploadCorrespondencia.service";
import type { ConfiguracionUploadCorrespondencia } from "../types/configuracionUploadCorrespondencia.types";

vi.mock("../services/configuracionUploadCorrespondencia.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/configuracionUploadCorrespondencia.service")
  >("../services/configuracionUploadCorrespondencia.service");

  return {
    ...actual,
    getConfiguracionUploadCorrespondencia: vi.fn(),
  };
});

const buildConfig = (
  accept = ".pdf,.docx",
  maxSizeBytes = 600000000,
): ConfiguracionUploadCorrespondencia => ({
  nameProceso: "CORRESPO",
  accept,
  allowedExtensions: accept.split(","),
  maxSizeBytes,
});

const deferred = <T,>() => {
  let resolve!: (value: T) => void;
  let reject!: (reason?: unknown) => void;
  const promise = new Promise<T>((promiseResolve, promiseReject) => {
    resolve = promiseResolve;
    reject = promiseReject;
  });
  return { promise, resolve, reject };
};

describe("[SCRUMCORE-287] useConfiguracionUploadCorrespondencia", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("no carga si enabled=false", () => {
    const { result } = renderHook(() =>
      useConfiguracionUploadCorrespondencia({ enabled: false }),
    );

    expect(getConfiguracionUploadCorrespondencia).not.toHaveBeenCalled();
    expect(result.current.config).toBeUndefined();
    expect(result.current.loading).toBe(false);
    expect(result.current.error).toBeUndefined();
    expect(result.current.empty).toBe(false);
  });

  it("carga configuracion al montar y expone loading", async () => {
    const pending = deferred<ConfiguracionUploadCorrespondencia>();
    vi.mocked(getConfiguracionUploadCorrespondencia).mockReturnValueOnce(pending.promise);

    const { result } = renderHook(() => useConfiguracionUploadCorrespondencia());

    await waitFor(() => {
      expect(result.current.loading).toBe(true);
    });

    act(() => {
      pending.resolve(buildConfig());
    });

    await waitFor(() => {
      expect(result.current.config).toEqual(buildConfig());
    });
    expect(result.current.loading).toBe(false);
    expect(result.current.error).toBeUndefined();
    expect(result.current.empty).toBe(false);
  });

  it("expone empty=true para respuesta funcional sin configuracion", async () => {
    vi.mocked(getConfiguracionUploadCorrespondencia).mockRejectedValueOnce(
      new ConfiguracionUploadCorrespondenciaError("No hay configuracion de adjuntos para CORRESPO."),
    );

    const { result } = renderHook(() => useConfiguracionUploadCorrespondencia());

    await waitFor(() => {
      expect(result.current.empty).toBe(true);
    });
    expect(result.current.config).toBeUndefined();
    expect(result.current.error).toBe("No hay configuracion de adjuntos para CORRESPO.");
  });

  it("expone error y permite reload", async () => {
    vi.mocked(getConfiguracionUploadCorrespondencia)
      .mockRejectedValueOnce(new Error("fallo configuracion"))
      .mockResolvedValueOnce(buildConfig(".pdf", 1024));

    const { result } = renderHook(() => useConfiguracionUploadCorrespondencia());

    await waitFor(() => {
      expect(result.current.error).toBe("fallo configuracion");
    });

    await act(async () => {
      await result.current.reload();
    });

    await waitFor(() => {
      expect(result.current.config).toEqual(buildConfig(".pdf", 1024));
    });
    expect(result.current.error).toBeUndefined();
    expect(getConfiguracionUploadCorrespondencia).toHaveBeenCalledTimes(2);
  });

  it("aborta request en curso al desmontar", async () => {
    const pending = deferred<ConfiguracionUploadCorrespondencia>();
    let capturedSignal: AbortSignal | undefined;
    vi.mocked(getConfiguracionUploadCorrespondencia).mockImplementationOnce(async (options) => {
      capturedSignal = options?.signal;
      return pending.promise;
    });

    const { unmount } = renderHook(() => useConfiguracionUploadCorrespondencia());

    await waitFor(() => {
      expect(capturedSignal).toBeDefined();
    });

    unmount();

    expect(capturedSignal?.aborted).toBe(true);
  });

  it("ignora respuestas stale cuando se recarga", async () => {
    const slow = deferred<ConfiguracionUploadCorrespondencia>();
    vi.mocked(getConfiguracionUploadCorrespondencia)
      .mockReturnValueOnce(slow.promise)
      .mockResolvedValueOnce(buildConfig(".xlsx", 4096));

    const { result } = renderHook(() => useConfiguracionUploadCorrespondencia());

    await act(async () => {
      await result.current.reload();
    });

    await waitFor(() => {
      expect(result.current.config).toEqual(buildConfig(".xlsx", 4096));
    });

    act(() => {
      slow.resolve(buildConfig(".pdf", 1024));
    });

    await waitFor(() => {
      expect(result.current.config).toEqual(buildConfig(".xlsx", 4096));
    });
  });
});

