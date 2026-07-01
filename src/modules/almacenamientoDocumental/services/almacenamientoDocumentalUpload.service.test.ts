import { beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import { AlmacenamientoDocumentalUploadError } from "../types/almacenamientoDocumental.types";
import {
  ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS,
  almacenarDocumento,
  cancelTemporaryUpload,
  completeTemporaryUpload,
  getTemporaryUploadStatus,
  initTemporaryUpload,
  uploadAndStoreOneDocument,
  uploadTemporaryChunk,
  unwrapStorageResponse,
} from "./almacenamientoDocumentalUpload.service";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}));

const mockedGet = vi.mocked(clienteApi.get);
const mockedPost = vi.mocked(clienteApi.post);
const mockedPut = vi.mocked(clienteApi.put);
const mockedDelete = vi.mocked(clienteApi.delete);

function initEnvelope(chunkSizeBytes = 3) {
  return {
    data: {
      success: true,
      data: {
        rutaTemporalId: "ruta-1",
        archivoTemporalId: "archivo-1",
        chunkSizeBytes,
        estado: "Inicializado",
      },
    },
  };
}

function storeEnvelope(extra: Record<string, unknown> = {}) {
  return {
    data: {
      success: true,
      data: {
        idAlmacen: 10,
        idRegistroProduccionDocumental: 20,
        nombreArchivoFinal: "scan.pdf",
        requestId: "req-1",
        ...extra,
      },
    },
  };
}

function uploadInput(file = new File(["abcdef"], "scan.pdf", { type: "application/pdf" })) {
  return {
    fileUid: "file-1",
    file,
    initialChunkSizeBytes: 2,
    request: {
      nombreGabinete: "Gestion",
      nombreDocumento: "scan.pdf",
      requestId: "req-1",
    },
  };
}

describe("[SPEC:SCRUMCORE-272] almacenamientoDocumentalUpload service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("calls init with the expected payload and validates the response", async () => {
    mockedPost.mockResolvedValueOnce(initEnvelope(4));

    const result = await initTemporaryUpload({
      nombreOriginal: "scan.pdf",
      tamanoBytes: 6,
      extension: ".pdf",
      hashSha256Esperado: null,
      numeroChunks: 2,
    });

    expect(mockedPost).toHaveBeenCalledWith(
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.init,
      {
        nombreOriginal: "scan.pdf",
        tamanoBytes: 6,
        extension: ".pdf",
        hashSha256Esperado: null,
        numeroChunks: 2,
      },
      undefined,
    );
    expect(result).toMatchObject({ rutaTemporalId: "ruta-1", chunkSizeBytes: 4 });
  });

  it("rejects invalid init shape with a typed contract error", async () => {
    mockedPost.mockResolvedValueOnce({
      data: { success: true, data: { rutaTemporalId: "", archivoTemporalId: "archivo-1" } },
    });

    await expect(
      initTemporaryUpload({
        nombreOriginal: "scan.pdf",
        tamanoBytes: 6,
        extension: ".pdf",
        numeroChunks: 2,
      }),
    ).rejects.toMatchObject({ code: "storage_contract_error" });
    expect(mockedPut).not.toHaveBeenCalled();
  });

  it("uploads chunks as raw bytes with required headers", async () => {
    const chunk = new Blob(["abc"], { type: "application/pdf" });
    mockedPut.mockResolvedValueOnce({ data: { success: true, data: {} } });

    await uploadTemporaryChunk({
      rutaTemporalId: "ruta/1",
      archivoTemporalId: "archivo 1",
      chunkIndex: 0,
      totalChunks: 2,
      chunk,
    });

    expect(mockedPut).toHaveBeenCalledWith(
      "/api/gestor-documental/almacenamiento/upload-temporal/ruta%2F1/archivo%201/chunk/0",
      chunk,
      {
        signal: undefined,
        headers: {
          "Content-Type": "application/octet-stream",
          "X-Total-Chunks": 2,
        },
      },
    );
    expect(mockedPut.mock.calls[0][1]).toBeInstanceOf(Blob);
    expect(mockedPut.mock.calls[0][1]).not.toBeInstanceOf(FormData);
  });

  it("calls status, complete, cancel and store endpoints", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          rutaTemporalId: "ruta-1",
          archivoTemporalId: "archivo-1",
          estado: "Parcial",
          chunksRecibidos: 1,
          totalChunks: 2,
        },
      },
    });
    mockedPost.mockResolvedValueOnce({ data: { success: true, data: {} } });
    mockedDelete.mockResolvedValueOnce({ data: { success: true, data: {} } });
    mockedPost.mockResolvedValueOnce(storeEnvelope().data);

    await expect(
      getTemporaryUploadStatus({ rutaTemporalId: "ruta-1", archivoTemporalId: "archivo-1" }),
    ).resolves.toMatchObject({ chunksRecibidos: 1 });
    await expect(
      completeTemporaryUpload({ rutaTemporalId: "ruta-1", archivoTemporalId: "archivo-1" }),
    ).resolves.toBeUndefined();
    await expect(
      cancelTemporaryUpload({ rutaTemporalId: "ruta-1", archivoTemporalId: "archivo-1" }),
    ).resolves.toBeUndefined();
    await expect(
      almacenarDocumento({
        nombreGabinete: "Gestion",
        rutaTemporalId: "ruta-1",
        nombreDocumento: "scan.pdf",
        requestId: "req-1",
        documentos: [
          {
            archivoTemporalId: "archivo-1",
            nombreOriginal: "scan.pdf",
            extension: ".pdf",
          },
        ],
      }),
    ).resolves.toMatchObject({ idAlmacen: 10 });
  });

  it("runs init -> chunks -> complete -> store and preserves raw backend result", async () => {
    mockedPost.mockResolvedValueOnce(initEnvelope(3));
    mockedPut.mockResolvedValue({ data: { success: true, data: {} } });
    mockedPost.mockResolvedValueOnce({ data: { success: true, data: {} } });
    mockedPost.mockResolvedValueOnce(storeEnvelope({ codigoEvento: "EV-10" }));

    const progress = vi.fn();
    const result = await uploadAndStoreOneDocument({ ...uploadInput(), onProgress: progress });

    expect(mockedPost).toHaveBeenNthCalledWith(
      1,
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.init,
      expect.objectContaining({
        nombreOriginal: "scan.pdf",
        tamanoBytes: 6,
        extension: ".pdf",
        numeroChunks: 3,
      }),
      undefined,
    );
    expect(mockedPut).toHaveBeenCalledTimes(2);
    expect(mockedPost).toHaveBeenNthCalledWith(
      2,
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.complete("ruta-1", "archivo-1"),
      undefined,
      undefined,
    );
    expect(mockedPost).toHaveBeenNthCalledWith(
      3,
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.almacenar,
      expect.objectContaining({
        rutaTemporalId: "ruta-1",
        requestId: "req-1",
        documentos: [
          {
            archivoTemporalId: "archivo-1",
            nombreOriginal: "scan.pdf",
            extension: ".pdf",
          },
        ],
      }),
      undefined,
    );
    expect(result.temporal).toMatchObject({ rutaTemporalId: "ruta-1" });
    expect(result.response).toMatchObject({ idRegistroProduccionDocumental: 20 });
    expect(result.rawBackendResult).toMatchObject({ codigoEvento: "EV-10" });
    expect(progress.mock.calls.map(([event]) => event.phase)).toEqual([
      "initializing",
      "uploading",
      "uploading",
      "completing",
      "completing",
      "storing",
      "storing",
    ]);
  });

  it("can send PascalCase payloads and validates status before complete", async () => {
    mockedPost.mockResolvedValueOnce(initEnvelope(3));
    mockedPut.mockResolvedValue({ data: { success: true, data: {} } });
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          Estado: "Uploading",
          ChunksRecibidos: [0, 1],
          ChunksPendientes: [],
          TamanoRecibidoBytes: 6,
        },
      },
    });
    mockedPost.mockResolvedValueOnce({ data: { success: true, data: { Estado: "Completed" } } });
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          Documento: {
            IdAlmacen: 10,
            IdRegistroProduccionDocumental: 20,
            NombreArchivoFinal: "DIG000010.pdf",
          },
          AnexoRespuesta: {
            IdRespuestaRadicado: 672,
            IdAlmacen: 10,
            NombreGabinete: "CORRESPO",
            NombreArchivo: "scan.pdf",
            Created: true,
          },
        },
        meta: { RequestId: "req-1" },
      },
    });

    await expect(
      uploadAndStoreOneDocument({
        ...uploadInput(),
        backendPayloadCase: "pascal",
        validateStatusBeforeComplete: true,
      }),
    ).resolves.toMatchObject({
      response: {
        idAlmacen: 10,
        idRegistroProduccionDocumental: 20,
        nombreArchivoFinal: "DIG000010.pdf",
        requestId: "req-1",
      },
    });

    expect(mockedPost).toHaveBeenNthCalledWith(
      1,
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.init,
      expect.objectContaining({
        NombreOriginal: "scan.pdf",
        TamanoBytes: 6,
        Extension: ".pdf",
        NumeroChunks: 3,
      }),
      undefined,
    );
    expect(mockedGet).toHaveBeenCalledWith(
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.status("ruta-1", "archivo-1"),
      undefined,
    );
    expect(mockedPost).toHaveBeenNthCalledWith(
      3,
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.almacenar,
      expect.objectContaining({
        NombreGabinete: "Gestion",
        RutaTemporalId: "ruta-1",
        RequestId: "req-1",
        Documentos: [
          expect.objectContaining({
            ArchivoTemporalId: "archivo-1",
            NombreOriginal: "scan.pdf",
            Extension: ".pdf",
          }),
        ],
      }),
      undefined,
    );
  });

  it("does not complete or store when status has pending chunks", async () => {
    mockedPost.mockResolvedValueOnce(initEnvelope(3));
    mockedPut.mockResolvedValue({ data: { success: true, data: {} } });
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          Estado: "Uploading",
          ChunksRecibidos: [0],
          ChunksPendientes: [1],
          TamanoRecibidoBytes: 3,
        },
      },
    });

    await expect(
      uploadAndStoreOneDocument({
        ...uploadInput(),
        validateStatusBeforeComplete: true,
      }),
    ).rejects.toMatchObject({ code: "storage_status_error" });

    expect(mockedPost).toHaveBeenCalledTimes(1);
  });

  it("recalculates total chunks when backend returns a different chunkSizeBytes", async () => {
    mockedPost.mockResolvedValueOnce(initEnvelope(4));
    mockedPut.mockResolvedValue({ data: { success: true, data: {} } });
    mockedPost.mockResolvedValueOnce({ data: { success: true, data: {} } });
    mockedPost.mockResolvedValueOnce(storeEnvelope());

    await uploadAndStoreOneDocument(uploadInput(new File(["abcdefg"], "scan.pdf")));

    expect(mockedPost).toHaveBeenNthCalledWith(
      1,
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.init,
      expect.objectContaining({ numeroChunks: 4 }),
      undefined,
    );
    expect(mockedPut).toHaveBeenCalledTimes(2);
    expect(mockedPut).toHaveBeenNthCalledWith(
      1,
      expect.stringContaining("/chunk/0"),
      expect.any(Blob),
      expect.objectContaining({
        headers: expect.objectContaining({ "X-Total-Chunks": 2 }),
      }),
    );
  });

  it("does not call store if a chunk fails", async () => {
    mockedPost.mockResolvedValueOnce(initEnvelope(3));
    mockedPut.mockRejectedValueOnce(new Error("chunk failed"));

    await expect(uploadAndStoreOneDocument(uploadInput())).rejects.toMatchObject({ code: "storage_chunk_error" });
    expect(mockedPost).toHaveBeenCalledTimes(1);
  });

  it("does not call store if complete fails", async () => {
    mockedPost.mockResolvedValueOnce(initEnvelope(3));
    mockedPut.mockResolvedValue({ data: { success: true, data: {} } });
    mockedPost.mockRejectedValueOnce(new Error("complete failed"));

    await expect(uploadAndStoreOneDocument(uploadInput())).rejects.toMatchObject({ code: "storage_complete_error" });
    expect(mockedPost).toHaveBeenCalledTimes(2);
  });

  it("aborts before init without backend calls", async () => {
    const controller = new AbortController();
    controller.abort();

    await expect(uploadAndStoreOneDocument({ ...uploadInput(), signal: controller.signal })).rejects.toMatchObject({
      code: "storage_aborted",
    });
    expect(mockedPost).not.toHaveBeenCalled();
    expect(mockedDelete).not.toHaveBeenCalled();
  });

  it("cancels temporary upload when abort happens after init", async () => {
    const controller = new AbortController();
    mockedPost.mockResolvedValueOnce(initEnvelope(3));
    mockedPut.mockImplementationOnce(() => {
      controller.abort();
      return Promise.reject(new DOMException("aborted", "AbortError"));
    });
    mockedDelete.mockResolvedValueOnce({ data: { success: true, data: {} } });

    await expect(uploadAndStoreOneDocument({ ...uploadInput(), signal: controller.signal })).rejects.toMatchObject({
      code: "storage_aborted",
    });
    expect(mockedDelete).toHaveBeenCalledWith(
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.cancel("ruta-1", "archivo-1"),
      undefined,
    );
  });

  it("keeps aborted as the primary error when cancellation cleanup fails", async () => {
    const controller = new AbortController();
    mockedPost.mockResolvedValueOnce(initEnvelope(3));
    mockedPut.mockImplementationOnce(() => {
      controller.abort();
      return Promise.reject(new DOMException("aborted", "AbortError"));
    });
    mockedDelete.mockRejectedValueOnce(new Error("delete failed"));

    await expect(uploadAndStoreOneDocument({ ...uploadInput(), signal: controller.signal })).rejects.toMatchObject({
      code: "storage_aborted",
      details: expect.objectContaining({
        rutaTemporalId: "ruta-1",
        archivoTemporalId: "archivo-1",
        cancelWarning: expect.any(Error),
      }),
    });
  });

  it("rejects invalid final storage shape with a typed contract error", async () => {
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          idAlmacen: 0,
          idRegistroProduccionDocumental: 20,
          nombreArchivoFinal: "",
          requestId: "req-1",
        },
      },
    });

    await expect(
      almacenarDocumento({
        nombreGabinete: "Gestion",
        rutaTemporalId: "ruta-1",
        nombreDocumento: "scan.pdf",
        requestId: "req-1",
        documentos: [
          {
            archivoTemporalId: "archivo-1",
            nombreOriginal: "scan.pdf",
            extension: ".pdf",
          },
        ],
      }),
    ).rejects.toMatchObject({ code: "storage_contract_error" });
  });

  it("preserves requestId from meta and errors envelope paths", () => {
    const successResult = unwrapStorageResponse(
      {
        success: true,
        data: {
          idAlmacen: 1,
          idRegistroProduccionDocumental: 2,
          nombreArchivoFinal: "scan.pdf",
          requestId: "req-data",
        },
        meta: { requestId: "req-meta" },
      },
      (value) => value,
      { code: "storage_store_error", phase: "storing", operation: "store" },
    );
    expect(successResult.requestId).toBe("req-data");

    expect(() =>
      unwrapStorageResponse(
        {
          success: false,
          message: "Contrato invalido",
          errors: { requestId: "req-error" },
        },
        (value) => value,
        { code: "storage_store_error", phase: "storing", operation: "store" },
      ),
    ).toThrow(AlmacenamientoDocumentalUploadError);
  });

  it("keeps endpoints free of legacy transports", () => {
    expect(JSON.stringify(ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS)).not.toContain(".ashx");
    expect(JSON.stringify(ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS)).not.toContain("XMLHttpRequest");
    expect(JSON.stringify(ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS)).not.toContain("FormData");
  });
});
