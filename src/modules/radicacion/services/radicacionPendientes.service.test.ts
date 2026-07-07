import { describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  buildRadicacionEnviarPendienteEndpoint,
  buildRadicacionTomarPendienteEndpoint,
  fetchRadicacionPendientesContador,
  fetchRadicacionPendientesTable,
  RADICACION_ESTADO_ACTIVO_ENDPOINT,
  RADICACION_PENDIENTES_CONTADOR_ENDPOINT,
  RADICACION_PENDIENTES_LISTADO_ENDPOINT,
  fetchRadicacionEstadoActivo,
  mapEstadoActivoToDocumentalState,
  enviarRadicacionPendiente,
  tomarRadicacionPendiente,
} from "./radicacionPendientes.service";
import type { RadicacionPendienteEstadoActivoDto } from "../types/radicacionDocumental.types";
import { extractRadicacionPendienteActionPayload } from "../types/radicacionPendientes.types";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
  },
}));

const mockedGet = vi.mocked(clienteApi.get);
const mockedPost = vi.mocked(clienteApi.post);

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

  it("[SPEC:BOOT-004] restaura contexto si backend informa activo con flag documental", () => {
    const state = mapEstadoActivoToDocumentalState({
      idEstadoRadicado: 91,
      estadoActual: 0,
      tieneTramiteDocumentalActivoEstado0: true,
    });

    expect(state).toMatchObject({
      idEstadoRadicado: 91,
      estadoActual: 0,
      requiereGestionDocumental: true,
      tieneTramiteDocumentalActivoEstado0: true,
      destinoPostRegistro: "documentos",
    });
  });

  it("[SPEC:BOOT-005] restaura contexto con contrato PascalCase o snake_case", () => {
    const state = mapEstadoActivoToDocumentalState({
      IdEstadoRadicado: "92",
      EstadoActual: "0",
      TieneTramiteDocumentalActivoEstado0: "true",
      RequiereGestionDocumental: 1,
      ConsecutivoRadicado: "RAD-92",
    } as unknown as RadicacionPendienteEstadoActivoDto);

    expect(state).toMatchObject({
      idEstadoRadicado: 92,
      estadoActual: 0,
      consecutivoRadicado: "RAD-92",
      requiereGestionDocumental: true,
      tieneTramiteDocumentalActivoEstado0: true,
      destinoPostRegistro: "documentos",
    });
  });

  it("[SPEC:PEND-001] consulta listado de pendientes en el endpoint existente", async () => {
    const request = {
      SearchType: 1,
      Search: "",
      SortField: "id_estado_radicado",
      SortDir: "DESC" as const,
      Page: 1,
      PageSize: 10,
      IncludeConfig: true,
    };
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          TableId: "lista-radicados-pendientes",
          Rows: [],
          Columns: [],
        },
      },
    });

    await expect(fetchRadicacionPendientesTable(request)).resolves.toMatchObject({
      success: true,
    });
    expect(mockedPost).toHaveBeenCalledWith(
      RADICACION_PENDIENTES_LISTADO_ENDPOINT,
      request,
    );
  });

  it("[SPEC:PEND-002] consulta contador de pendientes", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: { totalPendientes: 3 },
      },
    });

    await expect(fetchRadicacionPendientesContador()).resolves.toEqual({
      totalPendientes: 3,
    });
    expect(mockedGet).toHaveBeenCalledWith(RADICACION_PENDIENTES_CONTADOR_ENDPOINT);
  });

  it("[SPEC:PEND-003] toma pendiente por id_estado_radicado", async () => {
    const dto: RadicacionPendienteEstadoActivoDto = {
      tieneActivoEstado0: true,
      idEstadoRadicado: 10,
      estadoActual: 0,
      requiereGestionDocumental: true,
      tieneTramiteDocumentalActivoEstado0: true,
      destinoPostRegistro: "documentos",
    };
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: dto,
      },
    });

    await expect(tomarRadicacionPendiente(10)).resolves.toEqual(dto);
    expect(mockedPost).toHaveBeenCalledWith(
      buildRadicacionTomarPendienteEndpoint(10),
      {},
    );
  });

  it("[SPEC:PEND-004] construye endpoint de enviar a pendiente", () => {
    expect(buildRadicacionEnviarPendienteEndpoint(10)).toBe(
      "/api/radicacion/pendientes/10/enviar-pendiente",
    );
  });

  it("[SPEC:PEND-007] envia tramite activo a pendiente", async () => {
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          idEstadoRadicado: 10,
          estadoAnterior: 0,
          estadoActual: 1,
          tieneTramiteDocumentalActivoEstado0: false,
          destinoPostRegistro: "resumen",
          mensaje: "Tramite enviado a pendiente.",
        },
      },
    });

    await expect(enviarRadicacionPendiente(10)).resolves.toMatchObject({
      idEstadoRadicado: 10,
      estadoActual: 1,
      tieneTramiteDocumentalActivoEstado0: false,
    });
    expect(mockedPost).toHaveBeenCalledWith(
      buildRadicacionEnviarPendienteEndpoint(10),
      {},
    );
  });

  it("[SPEC:PEND-005] extrae campos de accion con nombres tolerantes", () => {
    expect(
      extractRadicacionPendienteActionPayload({
        IdEstadoRadicado: "15",
        IdTareaWorkflow: 25,
        RADICADO: "RAD-15",
      }),
    ).toEqual({
      idEstadoRadicado: 15,
      idTareaWorkflow: 25,
      consecutivoRadicado: "RAD-15",
    });
  });

  it("[SPEC:PEND-006] bloquea accion sin id_estado_radicado", () => {
    expect(
      extractRadicacionPendienteActionPayload({
        id_tarea_workflow: 25,
      }),
    ).toBeNull();
  });
});
