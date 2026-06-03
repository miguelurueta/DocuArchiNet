import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useEstructuraRespuestaIdTarea } from "../hooks/useEstructuraRespuestaIdTarea";
import * as estructuraService from "../services/solicitaEstructuraRespuestaIdTarea.service";

vi.mock("../services/solicitaEstructuraRespuestaIdTarea.service", () => ({
  getSolicitaEstructuraRespuestaIdTarea: vi.fn(),
}));

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });

  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
};

describe("[SPEC:gestion-correspondencia][SPEC:SCRUMCORE-219] useEstructuraRespuestaIdTarea", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("no consulta backend cuando no existe idTareaWf válido", () => {
    const { result } = renderHook(() => useEstructuraRespuestaIdTarea(undefined), {
      wrapper: createWrapper(),
    });

    expect(estructuraService.getSolicitaEstructuraRespuestaIdTarea).not.toHaveBeenCalled();
    expect(result.current.estrucTuraRespuesta).toBeNull();
    expect(result.current.isEmpty).toBe(false);
    expect(result.current.isEmptyLatched).toBe(false);
    expect(result.current.fetching).toBe(false);
    expect(result.current.resolved).toBe(false);
  });

  it("normaliza el primer item del backend como estrucTuraRespuesta", async () => {
    vi.mocked(estructuraService.getSolicitaEstructuraRespuestaIdTarea).mockResolvedValue({
      success: true,
      message: "YES",
      data: [
        {
          radicado: "2025-0001",
          destinatario: "Contasoft Company",
          tramiteDocumento: "Respuesta a derecho de petición",
        },
      ],
      errors: [],
    });

    const { result } = renderHook(() => useEstructuraRespuestaIdTarea(924), {
      wrapper: createWrapper(),
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(estructuraService.getSolicitaEstructuraRespuestaIdTarea).toHaveBeenCalledWith(924);
    expect(result.current.estrucTuraRespuesta).toEqual({
      Radicado: "2025-0001",
      Destinatario: "Contasoft Company",
      TramiteDocumento: "Respuesta a derecho de petición",
    });
    expect(result.current.isEmpty).toBe(false);
    expect(result.current.isEmptyLatched).toBe(false);
    expect(result.current.fetching).toBe(false);
    expect(result.current.resolved).toBe(true);
  });

  it("retorna idRespuestaRadicado normalizado desde el hook", async () => {
    vi.mocked(estructuraService.getSolicitaEstructuraRespuestaIdTarea).mockResolvedValue({
      success: true,
      message: "YES",
      data: [
        {
          Radicado: "2025-0002",
          Destinatario: "Contasoft Company",
          TramiteDocumento: "Respuesta a oficio",
          ID_RESPUESTA_RADICADO: 7788,
        },
      ],
      errors: [],
    });

    const { result } = renderHook(() => useEstructuraRespuestaIdTarea(924), {
      wrapper: createWrapper(),
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.estrucTuraRespuesta).toEqual({
      Radicado: "2025-0002",
      Destinatario: "Contasoft Company",
      TramiteDocumento: "Respuesta a oficio",
      idRespuestaRadicado: 7788,
    });
    expect(result.current.isEmpty).toBe(false);
    expect(result.current.isEmptyLatched).toBe(false);
    expect(result.current.fetching).toBe(false);
    expect(result.current.resolved).toBe(true);
  });

  it("mantiene payloads legacy sin idRespuestaRadicado sin errores runtime", async () => {
    vi.mocked(estructuraService.getSolicitaEstructuraRespuestaIdTarea).mockResolvedValue({
      success: true,
      message: "YES",
      data: [
        {
          Radicado: "2025-0003",
          Destinatario: "Contasoft Company",
          TramiteDocumento: "Respuesta legacy",
        },
      ],
      errors: [],
    });

    const { result } = renderHook(() => useEstructuraRespuestaIdTarea(924), {
      wrapper: createWrapper(),
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.error).toBeNull();
    expect(result.current.estrucTuraRespuesta?.idRespuestaRadicado).toBeUndefined();
    expect(result.current.estrucTuraRespuesta).toEqual({
      Radicado: "2025-0003",
      Destinatario: "Contasoft Company",
      TramiteDocumento: "Respuesta legacy",
    });
  });

  it("expone isEmpty cuando success es true y data es vacía", async () => {
    vi.mocked(estructuraService.getSolicitaEstructuraRespuestaIdTarea).mockResolvedValue({
      success: true,
      message: "Sin resultados",
      data: [],
      errors: [],
    });

    const { result } = renderHook(() => useEstructuraRespuestaIdTarea(924), {
      wrapper: createWrapper(),
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.estrucTuraRespuesta).toBeNull();
    expect(result.current.isEmpty).toBe(true);
    expect(result.current.isEmptyLatched).toBe(true);
    expect(result.current.fetching).toBe(false);
    expect(result.current.resolved).toBe(true);
  });

  it("maneja error sin producir estructura utilizable", async () => {
    vi.mocked(estructuraService.getSolicitaEstructuraRespuestaIdTarea).mockRejectedValue(
      new Error("boom"),
    );

    const { result } = renderHook(() => useEstructuraRespuestaIdTarea(924), {
      wrapper: createWrapper(),
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(false);
    });

    expect(result.current.estrucTuraRespuesta).toBeNull();
    expect(result.current.error).toEqual(expect.any(Error));
    expect(result.current.isEmpty).toBe(false);
    expect(result.current.isEmptyLatched).toBe(false);
    expect(result.current.fetching).toBe(false);
    expect(result.current.resolved).toBe(true);
  });
});
