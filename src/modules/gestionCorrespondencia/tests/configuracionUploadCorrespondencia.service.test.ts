import { beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  CONFIGURACION_UPLOAD_CORRESPONDENCIA_ENDPOINT,
  ConfiguracionUploadCorrespondenciaError,
  getConfiguracionUploadCorrespondencia,
  normalizeConfiguracionUploadCorrespondenciaResponse,
  normalizeUploadExtensions,
} from "../services/configuracionUploadCorrespondencia.service";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
  },
}));

describe("[SCRUMCORE-287] configuracionUploadCorrespondencia.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("consulta configuracion upload con nameProceso=CORRESPO y respeta AbortSignal", async () => {
    const signal = new AbortController().signal;
    vi.mocked(clienteApi.get).mockResolvedValue({
      data: {
        success: true,
        message: "OK",
        data: [
          {
            IdConfigUploadGestion: 3,
            ExtensionUpload: ".PDF,.DOC,.DOCX,.ZIP,.XLS,.XLSX",
            LengUpload: 600000000,
            NameProceso: "CORRESPO",
            EstadoProceso: 1,
          },
        ],
        errors: [],
      },
    });

    const result = await getConfiguracionUploadCorrespondencia({ signal });

    expect(clienteApi.get).toHaveBeenCalledWith(CONFIGURACION_UPLOAD_CORRESPONDENCIA_ENDPOINT, {
      params: {
        nameProceso: "CORRESPO",
      },
      signal,
    });
    expect(result).toEqual({
      nameProceso: "CORRESPO",
      accept: ".pdf,.doc,.docx,.zip,.xls,.xlsx",
      allowedExtensions: [".pdf", ".doc", ".docx", ".zip", ".xls", ".xlsx"],
      maxSizeBytes: 600000000,
    });
  });

  it("normaliza respuesta PascalCase", () => {
    expect(
      normalizeConfiguracionUploadCorrespondenciaResponse({
        success: true,
        message: "OK",
        data: [
          {
            ExtensionUpload: ".PDF,.DOCX",
            LengUpload: 1024,
            NameProceso: "CORRESPO",
            EstadoProceso: 1,
          },
        ],
      }),
    ).toEqual({
      nameProceso: "CORRESPO",
      accept: ".pdf,.docx",
      allowedExtensions: [".pdf", ".docx"],
      maxSizeBytes: 1024,
    });
  });

  it("normaliza respuesta camelCase", () => {
    expect(
      normalizeConfiguracionUploadCorrespondenciaResponse({
        success: true,
        message: "OK",
        data: [
          {
            extensionUpload: "pdf, xlsx",
            lengUpload: 2048,
            nameProceso: "CORRESPO",
            estadoProceso: 1,
          },
        ],
      }),
    ).toEqual({
      nameProceso: "CORRESPO",
      accept: ".pdf,.xlsx",
      allowedExtensions: [".pdf", ".xlsx"],
      maxSizeBytes: 2048,
    });
  });

  it("selecciona la primera fila activa y usa fallback a primera fila si ninguna esta activa", () => {
    const activeResult = normalizeConfiguracionUploadCorrespondenciaResponse({
      success: true,
      message: "OK",
      data: [
        { ExtensionUpload: ".png", LengUpload: 100, EstadoProceso: 0 },
        { ExtensionUpload: ".pdf", LengUpload: 200, EstadoProceso: 1 },
      ],
    });
    const fallbackResult = normalizeConfiguracionUploadCorrespondenciaResponse({
      success: true,
      message: "OK",
      data: [
        { ExtensionUpload: ".zip", LengUpload: 300, EstadoProceso: 0 },
        { ExtensionUpload: ".pdf", LengUpload: 400, EstadoProceso: 0 },
      ],
    });

    expect(activeResult.accept).toBe(".pdf");
    expect(activeResult.maxSizeBytes).toBe(200);
    expect(fallbackResult.accept).toBe(".zip");
    expect(fallbackResult.maxSizeBytes).toBe(300);
  });

  it("normaliza extensiones con espacios, punto faltante, vacios y duplicados", () => {
    expect(normalizeUploadExtensions(".PDF, DOC, .docx, ,PDF, .xlsx ")).toEqual([
      ".pdf",
      ".doc",
      ".docx",
      ".xlsx",
    ]);
  });

  it("lanza error funcional cuando success=false", () => {
    expect(() =>
      normalizeConfiguracionUploadCorrespondenciaResponse({
        success: false,
        message: "Error general",
        data: [],
        errors: [{ UserMessage: "Configuracion no disponible." }],
      }),
    ).toThrow("Configuracion no disponible.");
  });

  it("lanza error si data=[], extensiones vacias o LengUpload invalido", () => {
    expect(() =>
      normalizeConfiguracionUploadCorrespondenciaResponse({
        success: true,
        message: "OK",
        data: [],
      }),
    ).toThrow(ConfiguracionUploadCorrespondenciaError);

    expect(() =>
      normalizeConfiguracionUploadCorrespondenciaResponse({
        success: true,
        message: "OK",
        data: [{ ExtensionUpload: " , ", LengUpload: 100, EstadoProceso: 1 }],
      }),
    ).toThrow("La configuracion de adjuntos no contiene extensiones permitidas.");

    expect(() =>
      normalizeConfiguracionUploadCorrespondenciaResponse({
        success: true,
        message: "OK",
        data: [{ ExtensionUpload: ".pdf", LengUpload: 0, EstadoProceso: 1 }],
      }),
    ).toThrow("La configuracion de adjuntos no contiene un tamano maximo valido.");
  });
});

