import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  ELIMINAR_DOCUMENTO_STORAGE_ENGINE_ENDPOINT,
  eliminarDocumentoStorageEngine,
} from "./eliminarDocumentoStorageEngine.service";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    delete: vi.fn(),
  },
}));

vi.mock("../../almacenamientoDocumental/utils/storageFile.utils", async () => {
  const actual = await vi.importActual<typeof import("../../almacenamientoDocumental/utils/storageFile.utils")>(
    "../../almacenamientoDocumental/utils/storageFile.utils",
  );

  return {
    ...actual,
    createStorageRequestId: vi.fn(() => "req-generated"),
  };
});

const mockedDelete = vi.mocked(clienteApi.delete);

const axiosError = (status: number, data?: unknown) =>
  Object.assign(new Error("Request failed"), {
    isAxiosError: true,
    response: {
      status,
      data,
    },
  });

describe("[SPEC:SCRUMCORE-294] eliminarDocumentoStorageEngine service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it("invoca DELETE con requestId, gabinete y sourceModule normalizados", async () => {
    mockedDelete.mockResolvedValueOnce({
      status: 204,
      data: "",
    });

    const result = await eliminarDocumentoStorageEngine({
      idAlmacen: 91,
      nombreGabinete: "  WF_DOCS  ",
      requestId: "  req-91  ",
      sourceModule: "WORKFLOW",
    });

    expect(mockedDelete).toHaveBeenCalledWith(
      ELIMINAR_DOCUMENTO_STORAGE_ENGINE_ENDPOINT(91),
      {
        params: {
          nombreGabinete: "WF_DOCS",
          sourceModule: "WORKFLOW",
        },
        headers: {
          "X-Request-Id": "req-91",
        },
        signal: undefined,
      },
    );
    expect(result).toEqual({
      success: true,
      message: "Documento eliminado correctamente.",
      severity: "success",
      requestId: "req-91",
      httpStatus: 204,
      rawResponse: "",
    });
  });

  it("prioriza UserMessage sobre Message y conserva requestId desde meta", async () => {
    mockedDelete.mockRejectedValueOnce(
      axiosError(409, {
        success: false,
        message: "Mensaje visible",
        errors: [
          {
            UserMessage: "No se puede eliminar el documento.",
            Message: "Detalle tecnico",
          },
        ],
        meta: { requestId: "req-meta" },
      }),
    );

    const result = await eliminarDocumentoStorageEngine({
      idAlmacen: 92,
      nombreGabinete: "WF_DOCS",
    });

    expect(result).toEqual({
      success: false,
      message: "No se puede eliminar el documento.",
      severity: "warning",
      requestId: "req-meta",
      httpStatus: 409,
      rawResponse: {
        success: false,
        message: "Mensaje visible",
        errors: [
          {
            UserMessage: "No se puede eliminar el documento.",
            Message: "Detalle tecnico",
          },
        ],
        meta: { requestId: "req-meta" },
      },
    });
  });

  it("convierte HTTP 400 en warning con el mensaje funcional del backend", async () => {
    mockedDelete.mockRejectedValueOnce(
      axiosError(400, {
        message: "Delete feature is disabled: DELETE_STORAGE_ENGINE",
        errors: [
          {
            UserMessage: "No es posible eliminar este documento.",
          },
        ],
      }),
    );

    const result = await eliminarDocumentoStorageEngine({
      idAlmacen: 94,
      nombreGabinete: "WF_DOCS",
    });

    expect(result).toEqual({
      success: false,
      message: "No es posible eliminar este documento.",
      severity: "warning",
      requestId: "req-generated",
      httpStatus: 400,
      rawResponse: {
        message: "Delete feature is disabled: DELETE_STORAGE_ENGINE",
        errors: [
          {
            UserMessage: "No es posible eliminar este documento.",
          },
        ],
      },
    });
  });

  it("marca como fallo HTTP cuando el backend responde 400 sin success explicito", async () => {
    mockedDelete.mockRejectedValueOnce(
      axiosError(400, {
        message: "Delete feature is disabled: DELETE_STORAGE_ENGINE",
        errors: [
          {
            Message: "No es posible eliminar este anexo.",
          },
        ],
      }),
    );

    const result = await eliminarDocumentoStorageEngine({
      idAlmacen: 95,
      nombreGabinete: "WF_DOCS",
    });

    expect(result.success).toBe(false);
    expect(result.severity).toBe("warning");
    expect(result.message).toBe("No es posible eliminar este anexo.");
  });

  it("devuelve error tipado cuando el backend responde vacio con HTTP 403", async () => {
    mockedDelete.mockRejectedValueOnce(axiosError(403, undefined));

    const result = await eliminarDocumentoStorageEngine({
      idAlmacen: 93,
      nombreGabinete: "WF_DOCS",
    });

    expect(result).toEqual({
      success: false,
      message: "No fue posible eliminar el documento.",
      severity: "error",
      requestId: "req-generated",
      httpStatus: 403,
      rawResponse: undefined,
    });
  });

  it("valida idAlmacen antes de ejecutar la llamada", async () => {
    await expect(
      eliminarDocumentoStorageEngine({
        idAlmacen: 0,
        nombreGabinete: "WF_DOCS",
      }),
    ).rejects.toThrow(TypeError);

    expect(mockedDelete).not.toHaveBeenCalled();
  });
});
