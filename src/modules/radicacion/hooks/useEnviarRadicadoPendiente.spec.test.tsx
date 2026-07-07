import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { act, renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { RADICACION_ROUTES } from "../routes/radicacionRoutes";
import { enviarRadicacionPendiente } from "../services/radicacionPendientes.service";
import type { RadicacionDocumentalContextValue } from "../types/radicacionDocumental.types";
import { useEnviarRadicadoPendiente } from "./useEnviarRadicadoPendiente";

const mockNavigate = vi.fn();
let mockContext: RadicacionDocumentalContextValue;

vi.mock("react-router", async () => {
  const actual = await vi.importActual<typeof import("react-router")>(
    "react-router",
  );
  return {
    ...actual,
    useNavigate: () => mockNavigate,
  };
});

vi.mock("../services/radicacionPendientes.service", async () => {
  const actual =
    await vi.importActual<typeof import("../services/radicacionPendientes.service")>(
      "../services/radicacionPendientes.service",
    );
  return {
    ...actual,
    enviarRadicacionPendiente: vi.fn(),
  };
});

vi.mock("./useRadicacionDocumentalContext", () => ({
  useRadicacionDocumentalContext: () => mockContext,
}));

const mockedEnviarRadicacionPendiente = vi.mocked(enviarRadicacionPendiente);

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });

  return function Wrapper({ children }: { children: ReactNode }) {
    return (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    );
  };
};

const createActiveContext = (): RadicacionDocumentalContextValue => ({
  idEstadoRadicado: 10,
  estadoActual: 0,
  requiereGestionDocumental: true,
  tieneTramiteDocumentalActivoEstado0: true,
  setContextoDocumental: vi.fn(),
  clearContextoDocumental: vi.fn(),
});

describe("useEnviarRadicadoPendiente", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockContext = createActiveContext();
  });

  it("[SPEC:PEND-008] limpia contexto y navega a resumen solo cuando backend confirma estadoActual 1", async () => {
    const onSuccess = vi.fn();
    const onError = vi.fn();
    mockedEnviarRadicacionPendiente.mockResolvedValueOnce({
      idEstadoRadicado: 10,
      estadoAnterior: 0,
      estadoActual: 1,
      tieneTramiteDocumentalActivoEstado0: false,
      destinoPostRegistro: "resumen",
      mensaje: "Listo.",
    });

    const { result } = renderHook(
      () => useEnviarRadicadoPendiente({ onSuccess, onError }),
      { wrapper: createWrapper() },
    );

    act(() => {
      result.current.enviarActivoAPendiente();
    });

    await waitFor(() => {
      expect(mockContext.clearContextoDocumental).toHaveBeenCalledTimes(1);
    });
    expect(mockedEnviarRadicacionPendiente).toHaveBeenCalledWith(10);
    expect(mockNavigate).toHaveBeenCalledWith(RADICACION_ROUTES.root);
    expect(onSuccess).toHaveBeenCalledWith("Listo.");
    expect(onError).not.toHaveBeenCalled();
  });

  it("[SPEC:PEND-009] conserva contexto si backend no confirma estadoActual 1", async () => {
    const onError = vi.fn();
    mockedEnviarRadicacionPendiente.mockResolvedValueOnce({
      idEstadoRadicado: 10,
      estadoActual: 0,
      tieneTramiteDocumentalActivoEstado0: true,
      destinoPostRegistro: "documentos",
    });

    const { result } = renderHook(
      () => useEnviarRadicadoPendiente({ onError }),
      { wrapper: createWrapper() },
    );

    act(() => {
      result.current.enviarActivoAPendiente();
    });

    await waitFor(() => {
      expect(onError).toHaveBeenCalledWith(
        "El backend no confirmo estadoActual 1 para enviar a pendiente.",
      );
    });
    expect(mockContext.clearContextoDocumental).not.toHaveBeenCalled();
    expect(mockNavigate).not.toHaveBeenCalled();
  });

  it("[SPEC:PEND-010] no ejecuta mutacion cuando no hay tramite activo", () => {
    const onError = vi.fn();
    mockContext = {
      ...createActiveContext(),
      idEstadoRadicado: null,
      estadoActual: null,
      requiereGestionDocumental: false,
      tieneTramiteDocumentalActivoEstado0: false,
    };

    const { result } = renderHook(
      () => useEnviarRadicadoPendiente({ onError }),
      { wrapper: createWrapper() },
    );

    act(() => {
      result.current.enviarActivoAPendiente();
    });

    expect(mockedEnviarRadicacionPendiente).not.toHaveBeenCalled();
    expect(onError).toHaveBeenCalledWith(
      "No existe un tramite documental activo para enviar a pendiente.",
    );
  });
});
