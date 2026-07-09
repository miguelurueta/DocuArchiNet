import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { act, renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { registrarRadicacionEntrante } from "../services/radicacionRegistro.service";
import type { RadicacionDocumentalContextValue } from "../types/radicacionDocumental.types";
import type { RegistrarRadicacionEntranteRequestDto } from "../types/radicacionRegistro.types";
import { useRegistrarRadicacion } from "./useRegistrarRadicacion";

let mockContext: RadicacionDocumentalContextValue;

vi.mock("../services/radicacionRegistro.service", () => ({
  registrarRadicacionEntrante: vi.fn(),
}));

vi.mock("./useRadicacionDocumentalContext", () => ({
  useRadicacionDocumentalContext: () => mockContext,
}));

const mockedRegistrar = vi.mocked(registrarRadicacionEntrante);

const request = {
  tipoModuloRadicacion: 1,
  ASUNTO: "Solicitud",
  Remitente: { Nombre: "", id_Dest_Ext: 0 },
  Destinatario: { Destinatario: "", id_Remit_Dest_Int: 0 },
  Tipo_tramite: { Descripcion: "", tipo_doc_entrante: 0 },
  RE_flujo_trabajo: { NombreFlujo: "", id_tipo_flujo_workflow: 0 },
  TipoRadicado: { TipoRadicacion: "", IdTipoRadicado: 0 },
  TipoPlantillaRadicado: {
    TipoPlantillaRadicado: "",
    IdTipoPlantillaRdicado: 0,
  },
  expedienteRelacionado: { Expediente: "", idExpediente: 0 },
  radicadoRelacionados: [],
  ANEXOS_COR: "",
  FECHALIMITERESPUESTA: "",
  Campos: [],
} satisfies RegistrarRadicacionEntranteRequestDto;

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

describe("useRegistrarRadicacion", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    mockContext = {
      idEstadoRadicado: null,
      requiereGestionDocumental: false,
      tieneTramiteDocumentalActivoEstado0: false,
      setContextoDocumental: vi.fn(),
      clearContextoDocumental: vi.fn(),
    };
  });

  it("[SPEC:FE-01] conserva post-registro y contexto documental cuando backend lo informa", async () => {
    mockedRegistrar.mockResolvedValueOnce({
      success: true,
      message: "OK",
      data: {
        ConsecutivoRadicado: "RAD-303",
        ReturnRegistraRadicacion: {
          ConsecutivoRadicado: "RAD-303",
          IdRadicado: 77,
          IdEstadoRadicado: 88,
        },
        EstadoAsignacion: "Asignado",
        MetadataOperativa: {
          requiereGestionDocumental: true,
          tieneTramiteDocumentalActivoEstado0: true,
        },
      },
    });

    const { result } = renderHook(() => useRegistrarRadicacion(), {
      wrapper: createWrapper(),
    });

    await act(async () => {
      await result.current.registrar(request);
    });

    await waitFor(() => {
      expect(result.current.postRegistro?.consecutivoRadicado).toBe("RAD-303");
    });
    expect(mockContext.setContextoDocumental).toHaveBeenCalledWith(
      expect.objectContaining({
        idEstadoRadicado: 88,
        idRadicado: 77,
        requiereGestionDocumental: true,
        tieneTramiteDocumentalActivoEstado0: true,
        destinoPostRegistro: "documentos",
      }),
    );
  });

  it("[SPEC:FE-01] expone error funcional success=false sin limpiar contexto", async () => {
    const onError = vi.fn();
    mockedRegistrar.mockResolvedValueOnce({
      success: false,
      message: "Validacion backend",
      data: null,
    });

    const { result } = renderHook(() => useRegistrarRadicacion({ onError }), {
      wrapper: createWrapper(),
    });

    await expect(result.current.registrar(request)).rejects.toThrow(
      "Validacion backend",
    );
    expect(onError).toHaveBeenCalledWith("Validacion backend");
    expect(mockContext.setContextoDocumental).not.toHaveBeenCalled();
  });

  it("[SPEC:FE-01] expone detalles de validacion 400 por campo", async () => {
    const onError = vi.fn();
    mockedRegistrar.mockRejectedValueOnce({
      response: {
        data: {
          message: "Validacion fallida",
          errors: {
            "Destinatario.id_Remit_Dest_Int": ["requerido"],
            "Remitente.id_Dest_Ext": ["requerido"],
          },
        },
      },
    });

    const { result } = renderHook(() => useRegistrarRadicacion({ onError }), {
      wrapper: createWrapper(),
    });

    await expect(result.current.registrar(request)).rejects.toBeDefined();
    expect(onError).toHaveBeenCalledWith(
      "Validacion fallida: Destinatario.id_Remit_Dest_Int: requerido | Remitente.id_Dest_Ext: requerido",
    );
    expect(mockContext.setContextoDocumental).not.toHaveBeenCalled();
  });
});
