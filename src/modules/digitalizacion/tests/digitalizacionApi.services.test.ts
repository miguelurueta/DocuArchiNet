import { beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  DIGITALIZACION_CONFIGURACION_ENDPOINT,
  getDigitalizacionConfiguracion,
} from "../services/digitalizacionConfiguracion.api";
import {
  crearDocumentoDigitalizado,
  DIGITALIZACION_DOCUMENTOS_ENDPOINT,
} from "../services/digitalizacionDocumentos.api";
import {
  adjuntarDigitalizacion,
  getAdjuntarDigitalizacionEndpoint,
} from "../services/adjuntarDigitalizacion.api";
import {
  getUploadTemporalChunkEndpoint,
  getUploadTemporalCompleteEndpoint,
  uploadPdfTemporal,
  UPLOAD_TEMPORAL_INIT_ENDPOINT,
} from "../services/digitalizacionUploadTemporal.api";
import { DigitalizacionApiContractError } from "../services/digitalizacionApiClient";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
  },
}));

const expectContractCode = async (promise: Promise<unknown>, code: string) => {
  await expect(promise).rejects.toMatchObject({
    detail: expect.objectContaining({ code }),
  } satisfies Partial<DigitalizacionApiContractError>);
};

describe("[SPEC:SCRUMCORE-242] digitalizacion API services", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("loads configuracion and validates AppResponses envelope", async () => {
    vi.mocked(clienteApi.get).mockResolvedValue({
      data: {
        success: true,
        data: {
          IdConfiguracionDigitalizacion: 7,
          TipoDigitalizacion: "WORKFLOW",
          NombreGabinete: "Gestion",
          ActivaListaChequeo: true,
          ObligaListaChequeo: false,
          PermiteCrearDocumento: true,
          PermiteAdjuntarDocumento: true,
          RequiereMetadata: true,
          FormatosPermitidos: ["pdf"],
        },
      },
    });

    const result = await getDigitalizacionConfiguracion({
      TipoDigitalizacion: "WORKFLOW",
      NombreGabinete: "Gestion",
    });

    expect(clienteApi.get).toHaveBeenCalledWith(
      DIGITALIZACION_CONFIGURACION_ENDPOINT,
      expect.objectContaining({
        params: {
          TipoDigitalizacion: "WORKFLOW",
          NombreGabinete: "Gestion",
        },
      }),
    );
    expect(result.idConfiguracionDigitalizacion).toBe(7);
    expect(result.formatosPermitidos).toEqual(["pdf"]);
  });

  it("rejects AppResponses success=false", async () => {
    vi.mocked(clienteApi.get).mockResolvedValue({
      data: {
        success: false,
        message: "Regla funcional",
        data: null,
        meta: { status: "validation" },
      },
    });

    await expectContractCode(
      getDigitalizacionConfiguracion({
        TipoDigitalizacion: "WORKFLOW",
        NombreGabinete: "Gestion",
      }),
      "APP_RESPONSE_UNSUCCESSFUL",
    );
  });

  it("rejects partial configuracion response", async () => {
    vi.mocked(clienteApi.get).mockResolvedValue({
      data: {
        success: true,
        data: {
          IdConfiguracionDigitalizacion: 0,
        },
      },
    });

    await expectContractCode(
      getDigitalizacionConfiguracion({
        TipoDigitalizacion: "WORKFLOW",
        NombreGabinete: "Gestion",
      }),
      "CONFIGURACION_ID_INVALID",
    );
  });

  it("uploads PDF by chunks and completes temporal upload", async () => {
    const file = new File(["abcdef"], "scan.pdf", { type: "application/pdf" });
    const progress = vi.fn();
    vi.mocked(clienteApi.post)
      .mockResolvedValueOnce({
        data: {
          success: true,
          data: {
            RutaTemporalId: "ruta-1",
            ArchivoTemporalId: "archivo-1",
            ChunkSizeBytes: 3,
            TotalChunks: 2,
          },
        },
      })
      .mockResolvedValueOnce({
        data: {
          success: true,
          data: {
            RutaTemporalId: "ruta-1",
            ArchivoTemporalId: "archivo-1",
            Completado: true,
          },
        },
      });
    vi.mocked(clienteApi.put).mockResolvedValue({ data: { success: true, data: {} } });

    await expect(
      uploadPdfTemporal(file, { chunkSizeBytes: 3, onProgress: progress }),
    ).resolves.toEqual({
      rutaTemporalId: "ruta-1",
      archivoTemporalId: "archivo-1",
    });

    expect(clienteApi.post).toHaveBeenNthCalledWith(
      1,
      UPLOAD_TEMPORAL_INIT_ENDPOINT,
      expect.objectContaining({ TotalChunks: 2 }),
      expect.any(Object),
    );
    expect(clienteApi.put).toHaveBeenNthCalledWith(
      1,
      getUploadTemporalChunkEndpoint("ruta-1", "archivo-1", 0),
      expect.any(Blob),
      expect.any(Object),
    );
    expect(clienteApi.put).toHaveBeenNthCalledWith(
      2,
      getUploadTemporalChunkEndpoint("ruta-1", "archivo-1", 1),
      expect.any(Blob),
      expect.any(Object),
    );
    expect(clienteApi.post).toHaveBeenNthCalledWith(
      2,
      getUploadTemporalCompleteEndpoint("ruta-1", "archivo-1"),
      expect.objectContaining({ TotalChunks: 2 }),
      expect.any(Object),
    );
    expect(progress).toHaveBeenLastCalledWith({
      uploadedChunks: 2,
      totalChunks: 2,
      progress: 100,
    });
  });

  it("rejects invalid upload init response", async () => {
    const file = new File(["pdf"], "scan.pdf", { type: "application/pdf" });
    vi.mocked(clienteApi.post).mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          RutaTemporalId: "",
          ArchivoTemporalId: "",
        },
      },
    });

    await expectContractCode(uploadPdfTemporal(file, { chunkSizeBytes: 2 }), "RUTA_TEMPORAL_REQUIRED");
  });

  it("stops upload when a chunk fails", async () => {
    const file = new File(["abcdef"], "scan.pdf", { type: "application/pdf" });
    vi.mocked(clienteApi.post).mockResolvedValueOnce({
      data: {
        success: true,
        data: {
          RutaTemporalId: "ruta-1",
          ArchivoTemporalId: "archivo-1",
        },
      },
    });
    vi.mocked(clienteApi.put).mockRejectedValueOnce(new Error("chunk failed"));

    await expect(uploadPdfTemporal(file, { chunkSizeBytes: 3 })).rejects.toThrow("chunk failed");
    expect(clienteApi.post).toHaveBeenCalledTimes(1);
  });

  it("rejects upload complete when backend does not confirm completion", async () => {
    const file = new File(["abc"], "scan.pdf", { type: "application/pdf" });
    vi.mocked(clienteApi.post)
      .mockResolvedValueOnce({
        data: {
          success: true,
          data: {
            RutaTemporalId: "ruta-1",
            ArchivoTemporalId: "archivo-1",
          },
        },
      })
      .mockResolvedValueOnce({
        data: {
          success: true,
          data: {
            RutaTemporalId: "ruta-1",
            ArchivoTemporalId: "archivo-1",
            Completado: false,
          },
        },
      });
    vi.mocked(clienteApi.put).mockResolvedValue({ data: { success: true, data: {} } });

    await expectContractCode(
      uploadPdfTemporal(file, { chunkSizeBytes: 3 }),
      "UPLOAD_COMPLETE_NOT_CONFIRMED",
    );
  });

  it("creates a digitalized document with validated response", async () => {
    vi.mocked(clienteApi.post).mockResolvedValue({
      data: {
        success: true,
        data: {
          IdDocumento: 99,
          NombreGabinete: "Gestion",
          NombreDocumento: "scan.pdf",
          Extension: "pdf",
          NumeroPaginas: 2,
        },
      },
    });

    await expect(
      crearDocumentoDigitalizado({
        NombreGabinete: "Gestion",
        RutaTemporalId: "ruta",
        ArchivoTemporalId: "archivo",
        NombreDocumento: "scan.pdf",
      }),
    ).resolves.toMatchObject({ idDocumento: 99, numeroPaginas: 2 });
    expect(clienteApi.post).toHaveBeenCalledWith(
      DIGITALIZACION_DOCUMENTOS_ENDPOINT,
      expect.objectContaining({ NombreGabinete: "Gestion" }),
      expect.any(Object),
    );
  });

  it("attaches digitalizacion PDF with validated response", async () => {
    vi.mocked(clienteApi.post).mockResolvedValue({
      data: {
        success: true,
        data: {
          IdDocumento: 77,
          NombreGabinete: "Gestion",
          Extension: "pdf",
          NumeroPaginasAnterior: 1,
          NumeroPaginasAgregadas: 2,
          NumeroPaginasFinal: 3,
          DocumentoActualizado: true,
        },
      },
    });

    await expect(
      adjuntarDigitalizacion(77, {
        NombreGabinete: "Gestion",
        RutaTemporalId: "ruta",
        ArchivoTemporalId: "archivo",
      }),
    ).resolves.toMatchObject({ idDocumento: 77, documentoActualizado: true });
    expect(clienteApi.post).toHaveBeenCalledWith(
      getAdjuntarDigitalizacionEndpoint(77),
      expect.objectContaining({ NombreGabinete: "Gestion" }),
      expect.any(Object),
    );
  });
});
