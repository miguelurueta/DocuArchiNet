import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useDigitalizacionOperationOrchestrator } from "../hooks/useDigitalizacionOperationOrchestrator";
import type {
  AdjuntarDigitalizacionPdfResponse,
  AdjuntarDigitalizacionValidacionResponse,
  CrearDocumentoDigitalizadoResponse,
  DigitalizacionApiClient,
  DigitalizacionConfiguracionResponse,
  DigitalizacionListaChequeoResponse,
  DigitalizacionMetadataResolveResponse,
  UploadTemporalReferencia,
} from "../types/digitalizacionApi.types";
import type { DigitalizacionContext } from "../types/digitalizacion.types";

const createDeferred = <T,>() => {
  let resolveValue: (value: T) => void = () => undefined;
  const promise = new Promise<T>((resolve) => {
    resolveValue = resolve;
  });

  return { promise, resolve: resolveValue };
};

const pdfFile = () => new File(["pdf-content"], "digitalizacion.pdf", { type: "application/pdf" });

const crearContext: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "GAB",
  radicado: "RAD-1",
};

const adjuntarContext: DigitalizacionContext = {
  modo: "adjuntar",
  nombreGabinete: "GAB",
  radicado: "RAD-2",
  idDocumentoDestino: 42,
};

const createApiClient = (
  overrides: Partial<DigitalizacionApiClient> = {},
): DigitalizacionApiClient => ({
  getConfiguracion: vi.fn<DigitalizacionApiClient["getConfiguracion"]>(
    async () =>
      ({
        idConfiguracionDigitalizacion: 1,
        tipoDigitalizacion: "crear",
        nombreGabinete: "GAB",
        activaListaChequeo: false,
        obligaListaChequeo: false,
        permiteCrearDocumento: true,
        permiteAdjuntarDocumento: true,
        requiereMetadata: false,
        formatosPermitidos: ["pdf"],
      }) satisfies DigitalizacionConfiguracionResponse,
  ),
  getListaChequeo: vi.fn<DigitalizacionApiClient["getListaChequeo"]>(
    async () =>
      ({
        idConfiguracionDigitalizacion: 1,
        obligaListaChequeo: false,
        items: [],
      }) satisfies DigitalizacionListaChequeoResponse,
  ),
  resolveMetadata: vi.fn<DigitalizacionApiClient["resolveMetadata"]>(
    async (request) =>
      ({
        idTipoListaChequeo: request.IdTipoListaChequeo,
        idConfiguracionDigitalizacion: request.IdConfiguracionDigitalizacion,
        obligaListaChequeo: false,
        esUnico: false,
        unicidadValidada: true,
        trd: null,
      }) satisfies DigitalizacionMetadataResolveResponse,
  ),
  uploadPdfTemporal: vi.fn<DigitalizacionApiClient["uploadPdfTemporal"]>(
    async () =>
      ({
        rutaTemporalId: "ruta-1",
        archivoTemporalId: "archivo-1",
      }) satisfies UploadTemporalReferencia,
  ),
  crearDocumentoDigitalizado: vi.fn<DigitalizacionApiClient["crearDocumentoDigitalizado"]>(
    async () =>
      ({
        idDocumento: 100,
        nombreGabinete: "GAB",
        nombreDocumento: "digitalizacion.pdf",
        extension: "pdf",
        numeroPaginas: 2,
      }) satisfies CrearDocumentoDigitalizadoResponse,
  ),
  validarAdjuntarDigitalizacion: vi.fn<DigitalizacionApiClient["validarAdjuntarDigitalizacion"]>(
    async (idDocumento) =>
      ({
        idDocumento,
        nombreGabinete: "GAB",
        permitido: true,
        esPdf: true,
        estaFirmado: false,
        estaBloqueado: false,
        radicadoNoModificable: false,
        numeroPaginasActual: 3,
      }) satisfies AdjuntarDigitalizacionValidacionResponse,
  ),
  adjuntarDigitalizacion: vi.fn<DigitalizacionApiClient["adjuntarDigitalizacion"]>(
    async (idDocumento) =>
      ({
        idDocumento,
        nombreGabinete: "GAB",
        extension: "pdf",
        numeroPaginasAnterior: 3,
        numeroPaginasAgregadas: 2,
        numeroPaginasFinal: 5,
        documentoActualizado: true,
      }) satisfies AdjuntarDigitalizacionPdfResponse,
  ),
  ...overrides,
});

describe("[SPEC:SCRUMCORE-243] useDigitalizacionOperationOrchestrator", () => {
  it("creates a document after upload and calls onCompleted once", async () => {
    const apiClient = createApiClient();
    const onCompleted = vi.fn();
    const { result } = renderHook(() =>
      useDigitalizacionOperationOrchestrator({ apiClient, onCompleted }),
    );

    await act(async () => {
      await result.current.submit({
        context: crearContext,
        pdf: pdfFile(),
        pageCount: 2,
        requestId: "req-1",
      });
    });

    expect(apiClient.uploadPdfTemporal).toHaveBeenCalledOnce();
    expect(apiClient.crearDocumentoDigitalizado).toHaveBeenCalledWith(
      expect.objectContaining({
        NombreGabinete: "GAB",
        RutaTemporalId: "ruta-1",
        ArchivoTemporalId: "archivo-1",
        NombreDocumento: "digitalizacion.pdf",
        NumeroPaginasDeclaradas: 2,
      }),
      expect.objectContaining({ signal: expect.any(AbortSignal) }),
    );
    expect(onCompleted).toHaveBeenCalledTimes(1);
    expect(onCompleted).toHaveBeenCalledWith(
      expect.objectContaining({ accion: "documento-creado", idDocumento: 100 }),
    );
    expect(result.current.status).toBe("completed");
  });

  it("validates attach target before upload and attaches the temporal PDF", async () => {
    const apiClient = createApiClient();
    const onCompleted = vi.fn();
    const { result } = renderHook(() =>
      useDigitalizacionOperationOrchestrator({ apiClient, onCompleted }),
    );

    await act(async () => {
      await result.current.submit({
        context: adjuntarContext,
        pdf: pdfFile(),
        pageCount: 2,
      });
    });

    expect(apiClient.validarAdjuntarDigitalizacion).toHaveBeenCalledWith(
      42,
      { NombreGabinete: "GAB", Radicado: "RAD-2" },
      expect.objectContaining({ signal: expect.any(AbortSignal) }),
    );
    expect(apiClient.uploadPdfTemporal).toHaveBeenCalledOnce();
    expect(apiClient.adjuntarDigitalizacion).toHaveBeenCalledWith(
      42,
      expect.objectContaining({
        NombreGabinete: "GAB",
        RutaTemporalId: "ruta-1",
        ArchivoTemporalId: "archivo-1",
      }),
      expect.objectContaining({ signal: expect.any(AbortSignal) }),
    );
    expect(onCompleted).toHaveBeenCalledWith(
      expect.objectContaining({ accion: "documento-adjuntado", numeroPaginas: 5 }),
    );
  });

  it("does not upload when attach validation blocks the target", async () => {
    const onError = vi.fn();
    const apiClient = createApiClient({
      validarAdjuntarDigitalizacion: vi.fn<DigitalizacionApiClient["validarAdjuntarDigitalizacion"]>(
        async (idDocumento) =>
          ({
            idDocumento,
            nombreGabinete: "GAB",
            permitido: false,
            codigoBloqueo: "DOCUMENTO_BLOQUEADO",
            mensajeBloqueo: "Documento bloqueado.",
            esPdf: true,
            estaFirmado: false,
            estaBloqueado: true,
            radicadoNoModificable: false,
          }) satisfies AdjuntarDigitalizacionValidacionResponse,
      ),
    });
    const { result } = renderHook(() =>
      useDigitalizacionOperationOrchestrator({ apiClient, onError }),
    );

    await act(async () => {
      await expect(
        result.current.submit({
          context: adjuntarContext,
          pdf: pdfFile(),
          pageCount: 1,
        }),
      ).rejects.toMatchObject({
        detail: expect.objectContaining({ code: "DOCUMENTO_BLOQUEADO" }),
      });
    });

    expect(apiClient.uploadPdfTemporal).not.toHaveBeenCalled();
    expect(onError).toHaveBeenCalledWith(
      expect.objectContaining({ code: "DOCUMENTO_BLOQUEADO", status: "conflict" }),
    );
  });

  it("blocks required metadata before upload", async () => {
    const apiClient = createApiClient();
    const { result } = renderHook(() => useDigitalizacionOperationOrchestrator({ apiClient }));

    await act(async () => {
      await expect(
        result.current.submit({
          context: { ...crearContext, requiereMetadata: true },
          pdf: pdfFile(),
          pageCount: 1,
        }),
      ).rejects.toMatchObject({
        detail: expect.objectContaining({ code: "METADATA_REQUIRED" }),
      });
    });

    expect(apiClient.uploadPdfTemporal).not.toHaveBeenCalled();
  });

  it("blocks double submit while the operation is active", async () => {
    const deferred = createDeferred<UploadTemporalReferencia>();
    const apiClient = createApiClient({
      uploadPdfTemporal: vi.fn<DigitalizacionApiClient["uploadPdfTemporal"]>(() => deferred.promise),
    });
    const { result } = renderHook(() => useDigitalizacionOperationOrchestrator({ apiClient }));

    void act(() => {
      void result.current.submit({ context: crearContext, pdf: pdfFile(), pageCount: 1 });
    });

    await waitFor(() => {
      expect(result.current.loading).toBe(true);
    });

    await act(async () => {
      await expect(
        result.current.submit({ context: crearContext, pdf: pdfFile(), pageCount: 1 }),
      ).rejects.toMatchObject({
        detail: expect.objectContaining({ code: "SUBMIT_ALREADY_IN_PROGRESS" }),
      });
    });

    await act(async () => {
      deferred.resolve({ rutaTemporalId: "ruta-1", archivoTemporalId: "archivo-1" });
    });
  });

  it("ignores stale upload response after cancel", async () => {
    const deferred = createDeferred<UploadTemporalReferencia>();
    const onCompleted = vi.fn();
    const apiClient = createApiClient({
      uploadPdfTemporal: vi.fn<DigitalizacionApiClient["uploadPdfTemporal"]>(() => deferred.promise),
    });
    const { result } = renderHook(() =>
      useDigitalizacionOperationOrchestrator({ apiClient, onCompleted }),
    );

    void act(() => {
      void result.current
        .submit({ context: crearContext, pdf: pdfFile(), pageCount: 1 })
        .catch(() => undefined);
    });
    await waitFor(() => {
      expect(result.current.loading).toBe(true);
    });

    act(() => {
      result.current.cancel();
    });
    await act(async () => {
      deferred.resolve({ rutaTemporalId: "ruta-1", archivoTemporalId: "archivo-1" });
    });

    await waitFor(() => {
      expect(result.current.status).toBe("cancelled");
    });
    expect(apiClient.crearDocumentoDigitalizado).not.toHaveBeenCalled();
    expect(onCompleted).not.toHaveBeenCalled();
  });
});
