import { beforeEach, describe, expect, test, vi } from "vitest";
import clienteApi from "../../../../../api/Clienteaxios";
import {
  cancelUploadTemporal,
  completeUploadTemporal,
  initUploadTemporalPdfAnotado,
  REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS,
  reemplazarPaginasPdfAnotadas,
  statusUploadTemporal,
  unwrapAppResponse,
  uploadTemporalChunk,
} from "./reemplazoPaginasPdfAnotadas.service";
import { ReemplazoPaginasPdfAnotadasError } from "./reemplazoPaginasPdfAnotadas.types";

vi.mock("../../../../../api/Clienteaxios", () => ({
  default: {
    post: vi.fn(),
    put: vi.fn(),
    get: vi.fn(),
    delete: vi.fn(),
  },
}));

const mockedPost = vi.mocked(clienteApi.post);
const mockedPut = vi.mocked(clienteApi.put);
const mockedGet = vi.mocked(clienteApi.get);
const mockedDelete = vi.mocked(clienteApi.delete);

describe("reemplazoPaginasPdfAnotadas.service", () => {
  beforeEach(() => {
    mockedPost.mockReset();
    mockedPut.mockReset();
    mockedGet.mockReset();
    mockedDelete.mockReset();
  });

  test("initUploadTemporalPdfAnotado usa endpoint y contrato oficial", async () => {
    const signal = new AbortController().signal;
    const request = {
      NombreOriginal: "DIG00015416-PAGINA-2-ANOTADA.PDF",
      TamanoBytes: 251004,
      Extension: ".PDF" as const,
      HashSha256Esperado: "sha",
      NumeroChunks: 1,
    };
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          RutaTemporalId: "usr_141",
          ArchivoTemporalId: "af_1.pdf",
          ChunkSizeBytes: 1048576,
          Estado: "IN_PROGRESS",
        },
        errors: [],
      },
    });

    const result = await initUploadTemporalPdfAnotado(request, { signal });

    expect(mockedPost).toHaveBeenCalledWith(REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.init, request, { signal });
    expect(result.RutaTemporalId).toBe("usr_141");
    expect(result.ChunkSizeBytes).toBe(1048576);
  });

  test("uploadTemporalChunk envia body binario puro sin Content-Length manual", async () => {
    const signal = new AbortController().signal;
    const chunk = new Blob(["pdf-page"], { type: "application/pdf" });
    mockedPut.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: { chunkIndex: 0 },
        errors: [],
      },
    });

    await uploadTemporalChunk(
      {
        rutaTemporalId: "usr_141",
        archivoTemporalId: "af_1.pdf",
        chunkIndex: 0,
        totalChunks: 3,
        chunk,
      },
      { signal },
    );

    expect(mockedPut).toHaveBeenCalledWith(
      "/api/gestor-documental/documentos/reemplazopdf/upload-temporal/usr_141/af_1.pdf/chunk/0",
      chunk,
      {
        signal,
        headers: {
          "Content-Type": "application/octet-stream",
          "X-Total-Chunks": "3",
        },
      },
    );
    expect(mockedPut.mock.calls[0]?.[2]?.headers).not.toHaveProperty("Content-Length");
  });

  test("statusUploadTemporal consulta estado del temporal", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          Estado: "COMPLETED",
          ChunksRecibidos: 1,
          ChunksPendientes: 0,
          TamanoRecibidoBytes: 251004,
        },
        errors: [],
      },
    });

    const result = await statusUploadTemporal({ rutaTemporalId: "usr_141", archivoTemporalId: "af_1.pdf" });

    expect(mockedGet).toHaveBeenCalledWith(
      "/api/gestor-documental/documentos/reemplazopdf/upload-temporal/usr_141/af_1.pdf/status",
      undefined,
    );
    expect(result.Estado).toBe("COMPLETED");
  });

  test("completeUploadTemporal valida estado COMPLETED retornado por backend", async () => {
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: { Estado: "COMPLETED" },
        errors: [],
      },
    });

    const result = await completeUploadTemporal({ rutaTemporalId: "usr_141", archivoTemporalId: "af_1.pdf" });

    expect(mockedPost).toHaveBeenCalledWith(
      "/api/gestor-documental/documentos/reemplazopdf/upload-temporal/usr_141/af_1.pdf/complete",
      {},
      undefined,
    );
    expect(result.Estado).toBe("COMPLETED");
  });

  test("cancelUploadTemporal usa DELETE del temporal", async () => {
    mockedDelete.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: { Estado: "CANCELLED" },
        errors: [],
      },
    });

    const result = await cancelUploadTemporal({ rutaTemporalId: "usr_141", archivoTemporalId: "af_1.pdf" });

    expect(mockedDelete).toHaveBeenCalledWith(
      "/api/gestor-documental/documentos/reemplazopdf/upload-temporal/usr_141/af_1.pdf",
      undefined,
    );
    expect(result.Estado).toBe("CANCELLED");
  });

  test("reemplazarPaginasPdfAnotadas envia RutaTemporalId por cada pagina", async () => {
    const request = {
      NombreGabinete: "contabil",
      IdDocumento: 15416,
      RutaTemporalId: "usr_page2",
      Paginas: [
        {
          PageNumber: 2,
          RutaTemporalId: "usr_page2",
          ArchivoTemporalId: "af_2.pdf",
          ContentType: "application/pdf" as const,
          HashSha256Esperado: "sha2",
        },
        {
          PageNumber: 5,
          RutaTemporalId: "usr_page5",
          ArchivoTemporalId: "af_5.pdf",
          ContentType: "application/pdf" as const,
          HashSha256Esperado: "sha5",
        },
      ],
      DescOp: "AGREGA GRAFO PDF",
      ModuloRegistro: "DOCUARCHI" as const,
    };
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          IdDocumento: 15416,
          NombreGabinete: "contabil",
          PaginasReemplazadas: [2, 5],
          RutaArchivoFinal: "D:/final.pdf",
          RutaRespaldo: "D:/backup.pdf",
          TamanoAnteriorBytes: 1,
          TamanoNuevoBytes: 2,
          HashAnteriorSha256: "old",
          HashNuevoSha256: "new",
          RequestId: "req-1",
        },
        errors: [],
      },
    });

    const result = await reemplazarPaginasPdfAnotadas(request);

    expect(mockedPost).toHaveBeenCalledWith(REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.reemplazar, request, undefined);
    expect(mockedPost.mock.calls[0]?.[1]).toMatchObject({
      Paginas: [
        { PageNumber: 2, RutaTemporalId: "usr_page2", ContentType: "application/pdf" },
        { PageNumber: 5, RutaTemporalId: "usr_page5", ContentType: "application/pdf" },
      ],
    });
    expect(result.RequestId).toBe("req-1");
  });

  test("unwrapAppResponse preserva Field y Message de errores backend", () => {
    expect(() =>
      unwrapAppResponse(
        {
          success: false,
          message: "Validation",
          data: null,
          meta: { RequestId: "req-error-1" },
          errors: [{ Type: "Validation", Field: "originalPdfPassword", Message: "Password invalida" }],
        },
        { operation: "reemplazar paginas PDF anotadas" },
      ),
    ).toThrow("Password invalida");

    try {
      unwrapAppResponse(
        {
          success: false,
          message: "Validation",
          data: null,
          errors: [
            {
              Type: "Validation",
              Field: "originalPdfPassword",
              Message: "Password invalida",
              RequestId: "req-error-1",
            },
          ],
        },
        { operation: "reemplazar paginas PDF anotadas" },
      );
    } catch (error) {
      expect(error).toBeInstanceOf(ReemplazoPaginasPdfAnotadasError);
      expect((error as ReemplazoPaginasPdfAnotadasError).field).toBe("originalPdfPassword");
      expect((error as ReemplazoPaginasPdfAnotadasError).type).toBe("Validation");
      expect((error as ReemplazoPaginasPdfAnotadasError).requestId).toBe("req-error-1");
    }
  });

  test("unwrapAppResponse rechaza data null cuando el endpoint requiere datos", () => {
    expect(() =>
      unwrapAppResponse(
        {
          success: true,
          message: "OK",
          data: null,
          errors: [],
        },
        { operation: "init upload temporal PDF anotado" },
      ),
    ).toThrow("init upload temporal PDF anotado: contrato invalido, data requerido.");
  });

  test("codifica ids de temporales en rutas", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          Estado: "IN_PROGRESS",
          ChunksRecibidos: 0,
          ChunksPendientes: 1,
          TamanoRecibidoBytes: 0,
        },
        errors: [],
      },
    });

    await statusUploadTemporal({ rutaTemporalId: "usr 141", archivoTemporalId: "af/1.pdf" });

    expect(mockedGet.mock.calls[0]?.[0]).toContain("usr%20141");
    expect(mockedGet.mock.calls[0]?.[0]).toContain("af%2F1.pdf");
  });
});
