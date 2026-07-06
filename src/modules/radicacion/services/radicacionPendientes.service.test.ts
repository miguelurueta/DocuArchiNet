import { describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  RADICACION_ESTADO_ACTIVO_ENDPOINT,
  fetchRadicacionEstadoActivo,
  mapEstadoActivoToDocumentalState,
} from "./radicacionPendientes.service";
import type { RadicacionPendienteEstadoActivoDto } from "../types/radicacionDocumental.types";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
  },
}));

const mockedGet = vi.mocked(clienteApi.get);

describe("radicacionPendientes.service", () => {
  it("[SPEC:BOOT-001] consulta el endpoint estado-activo sin alterar el contrato backend", async () => {
    const dto: RadicacionPendienteEstadoActivoDto = {
      tieneActivoEstado0: false,
      requiereGestionDocumental: false,
      tieneTramiteDocumentalActivoEstado0: false,
      destinoPostRegistro: "resumen",
    };
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: dto,
      },
    });

    await expect(fetchRadicacionEstadoActivo()).resolves.toEqual(dto);
    expect(mockedGet).toHaveBeenCalledWith(RADICACION_ESTADO_ACTIVO_ENDPOINT);
  });

  it("[SPEC:BOOT-002] mapea estado activo al contrato del contexto documental", () => {
    const state = mapEstadoActivoToDocumentalState({
      tieneActivoEstado0: true,
      idEstadoRadicado: 77,
      idRadicado: 88,
      consecutivoRadicado: "RAD-77",
      idTareaWorkflow: 99,
      estadoActual: 0,
      tramite: "PQRS",
      remitente: "Contosoft",
      plantillaId: 1,
      tipoPlantillaId: 2,
      requiereGestionDocumental: true,
      tieneTramiteDocumentalActivoEstado0: true,
      destinoPostRegistro: "documentos",
      contextoDocumental: {
        idGabinete: 10,
        nombreGabinete: "RAD",
      },
    });

    expect(state).toMatchObject({
      idEstadoRadicado: 77,
      estadoActual: 0,
      requiereGestionDocumental: true,
      tieneTramiteDocumentalActivoEstado0: true,
      destinoPostRegistro: "documentos",
      contextoDocumental: {
        idGabinete: 10,
      },
    });
  });

  it("[SPEC:BOOT-003] retorna null si no existe tramite activo estado 0", () => {
    expect(
      mapEstadoActivoToDocumentalState({
        tieneActivoEstado0: false,
        requiereGestionDocumental: false,
        tieneTramiteDocumentalActivoEstado0: false,
        destinoPostRegistro: "resumen",
      }),
    ).toBeNull();
  });
});
