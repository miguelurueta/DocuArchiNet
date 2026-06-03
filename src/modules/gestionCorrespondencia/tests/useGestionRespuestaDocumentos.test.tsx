import { act, renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { GestionRespuestaDocumentosProvider } from "../context/GestionRespuestaDocumentosContext";
import { useGestionRespuestaDocumentos } from "../hooks/useGestionRespuestaDocumentos";
import * as gabineteService from "../services/solicitaGabineteRadicadoWorkflow.service";
import type { SolicitaGabineteRadicadoWorkflowResponse } from "../types/solicitaGabineteRadicadoWorkflow.types";

vi.mock("../services/solicitaGabineteRadicadoWorkflow.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/solicitaGabineteRadicadoWorkflow.service")
  >("../services/solicitaGabineteRadicadoWorkflow.service");

  return {
    ...actual,
    getSolicitaGabinetePorTareaWorkflow: vi.fn(),
  };
});

const buildGabineteResponse = (
  overrides: Partial<SolicitaGabineteRadicadoWorkflowResponse> = {},
): SolicitaGabineteRadicadoWorkflowResponse => ({
  success: true,
  message: "OK",
  data: {
    NombreGabinete: "WF_DOCS",
    Radicado: "2025-0001",
    EstadoExistenciaRadicado: "YES",
  },
  ...overrides,
});

const buildWrapper =
  (props: { idTareaWf?: number; radicado?: string; idRespuestaRadicado?: string | number }) =>
  ({ children }: { children: ReactNode }) => (
    <GestionRespuestaDocumentosProvider {...props}>{children}</GestionRespuestaDocumentosProvider>
  );

describe("[SPEC:SCRUMCORE-220] useGestionRespuestaDocumentos", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValue(
      buildGabineteResponse(),
    );
  });

  it("retorna fallback seguro fuera del provider", async () => {
    const { result } = renderHook(() => useGestionRespuestaDocumentos());

    expect(result.current.available).toBe(false);
    expect(result.current.files).toEqual([]);
    expect(result.current.gabineteLoading).toBe(false);
    expect(result.current.nombreGabinete).toBeUndefined();

    await expect(result.current.reloadGabinete()).resolves.toBeUndefined();
  });

  it("expone datos transversales y normaliza nombreGabinete desde el provider", async () => {
    const { result } = renderHook(() => useGestionRespuestaDocumentos(), {
      wrapper: buildWrapper({
        idTareaWf: 924,
        radicado: " 2025-0001 ",
        idRespuestaRadicado: "RESP-1",
      }),
    });

    expect(result.current.available).toBe(true);
    expect(result.current.idTareaWf).toBe(924);
    expect(result.current.radicado).toBe("2025-0001");
    expect(result.current.idRespuestaRadicado).toBe("RESP-1");

    await waitFor(() => {
      expect(result.current.nombreGabinete).toBe("WF_DOCS");
      expect(result.current.gabineteLoading).toBe(false);
    });
  });

  it("mantiene files/setFiles sin cambiar su semantica", () => {
    const { result } = renderHook(() => useGestionRespuestaDocumentos(), {
      wrapper: buildWrapper({ idTareaWf: undefined }),
    });

    act(() => {
      result.current.setFiles([{ uid: "1", name: "doc.pdf" }]);
    });

    expect(result.current.files).toEqual([{ uid: "1", name: "doc.pdf" }]);
  });

  it("no duplica fetch automatico cuando idTareaWf no cambia", async () => {
    const { result, rerender } = renderHook(() => useGestionRespuestaDocumentos(), {
      wrapper: buildWrapper({ idTareaWf: 924, radicado: "2025-0001" }),
    });

    await waitFor(() => {
      expect(result.current.nombreGabinete).toBe("WF_DOCS");
    });

    rerender();

    expect(gabineteService.getSolicitaGabinetePorTareaWorkflow).toHaveBeenCalledTimes(1);
  });

  it("reloadGabinete fuerza nueva carga explicita", async () => {
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow)
      .mockResolvedValueOnce(buildGabineteResponse())
      .mockResolvedValueOnce(buildGabineteResponse({ data: { NombreGabinete: "WF_DOCS_2" } }));

    const { result } = renderHook(() => useGestionRespuestaDocumentos(), {
      wrapper: buildWrapper({ idTareaWf: 924, radicado: "2025-0001" }),
    });

    await waitFor(() => {
      expect(result.current.nombreGabinete).toBe("WF_DOCS");
    });

    await act(async () => {
      await result.current.reloadGabinete();
    });

    await waitFor(() => {
      expect(result.current.nombreGabinete).toBe("WF_DOCS_2");
    });
    expect(gabineteService.getSolicitaGabinetePorTareaWorkflow).toHaveBeenCalledTimes(2);
  });

  it("expone gabineteError sin romper render", async () => {
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockRejectedValueOnce(
      new Error("fallo backend"),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentos(), {
      wrapper: buildWrapper({ idTareaWf: 924, radicado: "2025-0001" }),
    });

    await waitFor(() => {
      expect(result.current.gabineteError).toBe("fallo backend");
      expect(result.current.gabineteLoading).toBe(false);
    });
  });

  it("no permite que una respuesta stale sobrescriba el gabinete actual", async () => {
    const deferred = <T,>() => {
      let resolve!: (value: T) => void;
      const promise = new Promise<T>((res) => {
        resolve = res;
      });
      return { promise, resolve };
    };
    const slow = deferred<SolicitaGabineteRadicadoWorkflowResponse>();

    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow)
      .mockImplementationOnce(async () => slow.promise)
      .mockResolvedValueOnce(buildGabineteResponse({ data: { NombreGabinete: "FAST" } }));

    let providerProps: {
      idTareaWf?: number;
      radicado?: string;
      idRespuestaRadicado?: string | number;
    } = { idTareaWf: 1, radicado: "2025-0001" };
    const wrapper = ({ children }: { children: ReactNode }) => (
      <GestionRespuestaDocumentosProvider {...providerProps}>
        {children}
      </GestionRespuestaDocumentosProvider>
    );

    const { result, rerender } = renderHook(() => useGestionRespuestaDocumentos(), {
      wrapper,
    });

    providerProps = { idTareaWf: 2, radicado: "2025-0002" };
    rerender();

    await waitFor(() => {
      expect(result.current.nombreGabinete).toBe("FAST");
    });

    slow.resolve(buildGabineteResponse({ data: { NombreGabinete: "SLOW" } }));

    await waitFor(() => {
      expect(result.current.nombreGabinete).toBe("FAST");
    });
  });
});
