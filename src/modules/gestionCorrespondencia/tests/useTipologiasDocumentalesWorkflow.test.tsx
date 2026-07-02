import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useTipologiasDocumentalesWorkflow } from "../hooks/useTipologiasDocumentalesWorkflow";
import * as tipologiasService from "../services/tipologiasDocumentalesWorkflow.service";
import type { TipologiaDocumentalWorkflowOption } from "../types/tipologiasDocumentalesWorkflow.types";

vi.mock("../services/tipologiasDocumentalesWorkflow.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/tipologiasDocumentalesWorkflow.service")
  >("../services/tipologiasDocumentalesWorkflow.service");

  return {
    ...actual,
    getTipologiasDocumentalesWorkflow: vi.fn(),
  };
});

const buildOption = (
  idTipoDocumento = 43,
  nombreTipoDocumento = "Comprobante De Egreso",
): TipologiaDocumentalWorkflowOption => ({
  value: idTipoDocumento,
  label: nombreTipoDocumento,
  idTipoDocumento,
  nombreTipoDocumento,
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

describe("[SCRUMCORE-284] useTipologiasDocumentalesWorkflow", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("no llama API si faltan ids workflow", () => {
    const { result } = renderHook(() =>
      useTipologiasDocumentalesWorkflow({ idTareaWf: 933, idRutaWf: undefined }),
    );

    expect(tipologiasService.getTipologiasDocumentalesWorkflow).not.toHaveBeenCalled();
    expect(result.current.options).toEqual([]);
    expect(result.current.loading).toBe(false);
    expect(result.current.empty).toBe(false);
    expect(result.current.error).toBeUndefined();
  });

  it("carga opciones con ids validos y expone loading", async () => {
    const pending = deferred<TipologiaDocumentalWorkflowOption[]>();
    vi.mocked(tipologiasService.getTipologiasDocumentalesWorkflow).mockReturnValueOnce(
      pending.promise,
    );

    const { result } = renderHook(() =>
      useTipologiasDocumentalesWorkflow({ idTareaWf: 933, idRutaWf: 9 }),
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(true);
    });

    act(() => {
      pending.resolve([buildOption()]);
    });

    await waitFor(() => {
      expect(result.current.options).toEqual([buildOption()]);
    });
    expect(result.current.loading).toBe(false);
    expect(result.current.empty).toBe(false);
  });

  it("expone empty=true cuando el catalogo exitoso viene vacio", async () => {
    vi.mocked(tipologiasService.getTipologiasDocumentalesWorkflow).mockResolvedValueOnce([]);

    const { result } = renderHook(() =>
      useTipologiasDocumentalesWorkflow({ idTareaWf: 933, idRutaWf: 9 }),
    );

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.options).toEqual([]);
    expect(result.current.empty).toBe(true);
  });

  it("expone error y permite reload", async () => {
    vi.mocked(tipologiasService.getTipologiasDocumentalesWorkflow)
      .mockRejectedValueOnce(new Error("fallo catalogo"))
      .mockResolvedValueOnce([buildOption(44, "Derecho de Peticion")]);

    const { result } = renderHook(() =>
      useTipologiasDocumentalesWorkflow({ idTareaWf: 933, idRutaWf: 9 }),
    );

    await waitFor(() => {
      expect(result.current.error).toBe("fallo catalogo");
    });

    await act(async () => {
      await result.current.reload();
    });

    await waitFor(() => {
      expect(result.current.options).toEqual([buildOption(44, "Derecho de Peticion")]);
    });
    expect(result.current.error).toBeUndefined();
    expect(tipologiasService.getTipologiasDocumentalesWorkflow).toHaveBeenCalledTimes(2);
  });

  it("aborta request en curso al desmontar", async () => {
    const pending = deferred<TipologiaDocumentalWorkflowOption[]>();
    let capturedSignal: AbortSignal | undefined;
    vi.mocked(tipologiasService.getTipologiasDocumentalesWorkflow).mockImplementationOnce(
      async (_query, options) => {
        capturedSignal = options?.signal;
        return pending.promise;
      },
    );

    const { unmount } = renderHook(() =>
      useTipologiasDocumentalesWorkflow({ idTareaWf: 933, idRutaWf: 9 }),
    );

    await waitFor(() => {
      expect(capturedSignal).toBeDefined();
    });

    unmount();

    expect(capturedSignal?.aborted).toBe(true);
  });

  it("ignora respuesta stale cuando cambia tarea/ruta", async () => {
    const slow = deferred<TipologiaDocumentalWorkflowOption[]>();
    vi.mocked(tipologiasService.getTipologiasDocumentalesWorkflow)
      .mockReturnValueOnce(slow.promise)
      .mockResolvedValueOnce([buildOption(45, "Respuesta Workflow")]);

    const { result, rerender } = renderHook(
      ({ idTareaWf, idRutaWf }) =>
        useTipologiasDocumentalesWorkflow({ idTareaWf, idRutaWf }),
      {
        initialProps: { idTareaWf: 933, idRutaWf: 9 },
      },
    );

    rerender({ idTareaWf: 934, idRutaWf: 10 });

    await waitFor(() => {
      expect(result.current.options).toEqual([buildOption(45, "Respuesta Workflow")]);
    });

    act(() => {
      slow.resolve([buildOption(43, "Resultado Stale")]);
    });

    await waitFor(() => {
      expect(result.current.options).toEqual([buildOption(45, "Respuesta Workflow")]);
    });
  });
});
