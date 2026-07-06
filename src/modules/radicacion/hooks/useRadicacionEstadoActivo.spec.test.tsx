import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useRadicacionEstadoActivo } from "./useRadicacionEstadoActivo";
import { fetchRadicacionEstadoActivo } from "../services/radicacionPendientes.service";

vi.mock("../services/radicacionPendientes.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/radicacionPendientes.service")
  >("../services/radicacionPendientes.service");

  return {
    ...actual,
    fetchRadicacionEstadoActivo: vi.fn(),
  };
});

const mockedFetch = vi.mocked(fetchRadicacionEstadoActivo);

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });

  return ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
};

describe("useRadicacionEstadoActivo", () => {
  beforeEach(() => {
    mockedFetch.mockReset();
  });

  it("[SPEC:BOOT-004] encapsula la consulta REST y expone contexto documental activo", async () => {
    mockedFetch.mockResolvedValueOnce({
      tieneActivoEstado0: true,
      idEstadoRadicado: 77,
      estadoActual: 0,
      requiereGestionDocumental: true,
      tieneTramiteDocumentalActivoEstado0: true,
      destinoPostRegistro: "documentos",
    });

    const { result } = renderHook(() => useRadicacionEstadoActivo(), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isLoading).toBe(false));

    expect(mockedFetch).toHaveBeenCalledTimes(1);
    expect(result.current.contextoDocumental).toMatchObject({
      idEstadoRadicado: 77,
      destinoPostRegistro: "documentos",
    });
  });
});
