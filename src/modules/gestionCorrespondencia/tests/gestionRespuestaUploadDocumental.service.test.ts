import { beforeEach, describe, expect, it, vi } from "vitest";
import { loadGestionRespuestaUploadConfig } from "../services/gestionRespuestaUploadDocumental.service";
import { getConfiguracionUploadCorrespondencia } from "../services/configuracionUploadCorrespondencia.service";

vi.mock("../services/configuracionUploadCorrespondencia.service", () => ({
  getConfiguracionUploadCorrespondencia: vi.fn(),
}));

describe("[SCRUMCORE-287] gestionRespuestaUploadDocumental.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("construye UploadDocumentalConfig desde configuracion backend preservando flags documentales", async () => {
    vi.mocked(getConfiguracionUploadCorrespondencia).mockResolvedValueOnce({
      nameProceso: "CORRESPO",
      accept: ".pdf,.doc,.docx,.zip,.xls,.xlsx",
      allowedExtensions: [".pdf", ".doc", ".docx", ".zip", ".xls", ".xlsx"],
      maxSizeBytes: 600000000,
    });

    await expect(loadGestionRespuestaUploadConfig()).resolves.toEqual({
      accept: ".pdf,.doc,.docx,.zip,.xls,.xlsx",
      allowedExtensions: [".pdf", ".doc", ".docx", ".zip", ".xls", ".xlsx"],
      maxSizeBytes: 600000000,
      multiple: true,
      requiereTipologia: true,
      requiereFechaCarga: false,
      fechaCargaObligatoria: false,
      validationMode: "queue-with-error",
    });
    expect(getConfiguracionUploadCorrespondencia).toHaveBeenCalledTimes(1);
  });

  it("falla cerrado si el servicio backend no entrega configuracion usable", async () => {
    vi.mocked(getConfiguracionUploadCorrespondencia).mockRejectedValueOnce(
      new Error("No hay configuracion de adjuntos para CORRESPO."),
    );

    await expect(loadGestionRespuestaUploadConfig()).rejects.toThrow(
      "No hay configuracion de adjuntos para CORRESPO.",
    );
  });
});

