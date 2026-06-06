import { beforeEach, describe, expect, test, vi } from "vitest";
import clienteApi from "../../../../api/Clienteaxios";
import { fetchMisPermisosVisorPdf } from "./AppVisorEmbedPdf.service";

vi.mock("../../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
  },
}));

const mockedGet = vi.mocked(clienteApi.get);

describe("fetchMisPermisosVisorPdf", () => {
  beforeEach(() => {
    mockedGet.mockReset();
  });

  test("consulta mis-permisos con codigoImpl sin enviar idUsuario", async () => {
    const signal = new AbortController().signal;
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          CodigoImplementacion: "gestion_correspondencia",
          IdUsuario: 205,
          Permissions: {
            "pdf.view": true,
            "pdf.download": true,
          },
          Sources: {
            "pdf.download": "perfil_activo",
          },
          GeneratedAt: "2026-05-20T14:40:00Z",
        },
        meta: { Status: "success", Total: 13 },
        errors: [],
      },
    });

    const result = await fetchMisPermisosVisorPdf({ codigoImpl: "gestion_correspondencia", signal });

    expect(mockedGet).toHaveBeenCalledWith(
      "/api/gestor-documental/permisos-visorpdf/implementaciones/gestion_correspondencia/mis-permisos",
      { signal },
    );
    expect(mockedGet.mock.calls[0]?.[0]).not.toContain("/usuarios/");
    expect(mockedGet.mock.calls[0]?.[0]).not.toContain("idUsuario");
    expect(result.Permissions["pdf.download"]).toBe(true);
    expect(result.IdUsuario).toBe(205);
    expect(result.Sources?.["pdf.download"]).toBe("perfil_activo");
  });

  test("codifica codigoImpl en el path", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          CodigoImplementacion: "workflow test",
          IdUsuario: 1,
          Permissions: {},
          GeneratedAt: "2026-05-20T14:40:00Z",
        },
        errors: [],
      },
    });

    await fetchMisPermisosVisorPdf({ codigoImpl: "workflow test" });

    expect(mockedGet.mock.calls[0]?.[0]).toContain("workflow%20test");
  });

  test("rechaza success false con mensaje del envelope", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: false,
        message: "No cuenta con permisos administrativos",
        data: {},
        errors: [],
      },
    });

    await expect(fetchMisPermisosVisorPdf({ codigoImpl: "gestion_correspondencia" })).rejects.toThrow(
      "No cuenta con permisos administrativos",
    );
  });

  test("rechaza contrato con Permissions en raiz", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        CodigoImplementacion: "gestion_correspondencia",
        IdUsuario: 205,
        Permissions: {
          "pdf.print": true,
        },
        GeneratedAt: "2026-05-20T14:40:00Z",
      },
    });

    await expect(fetchMisPermisosVisorPdf({ codigoImpl: "gestion_correspondencia" })).rejects.toThrow(
      "Permisos visor PDF: contrato invalido.",
    );
  });

  test("rechaza contrato sin data.Permissions", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {
          CodigoImplementacion: "gestion_correspondencia",
          IdUsuario: 205,
          GeneratedAt: "2026-05-20T14:40:00Z",
        },
        errors: [],
      },
    });

    await expect(fetchMisPermisosVisorPdf({ codigoImpl: "gestion_correspondencia" })).rejects.toThrow(
      "Permisos visor PDF: contrato invalido.",
    );
  });

  test("usa el primer mensaje de errors cuando existe", async () => {
    mockedGet.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: {},
        errors: [{ Type: "Validation", Field: "data", Message: "Permissions requerido" }],
      },
    });

    await expect(fetchMisPermisosVisorPdf({ codigoImpl: "gestion_correspondencia" })).rejects.toThrow(
      "Permissions requerido",
    );
  });
});
