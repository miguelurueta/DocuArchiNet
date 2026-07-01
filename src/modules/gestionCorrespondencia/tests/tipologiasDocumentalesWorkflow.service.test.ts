import { beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  getTipologiasDocumentalesWorkflow,
  normalizeWorkflowTypologyResponse,
  TIPOLOGIAS_DOCUMENTALES_WORKFLOW_ENDPOINT,
  TipologiasDocumentalesWorkflowError,
} from "../services/tipologiasDocumentalesWorkflow.service";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
  },
}));

describe("[SCRUMCORE-284] tipologiasDocumentalesWorkflow.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("consulta el endpoint WORKFLOW con IdTareaWf e IdRutaWf sin IdTipoTramite", async () => {
    const signal = new AbortController().signal;
    vi.mocked(clienteApi.get).mockResolvedValue({
      data: {
        success: true,
        message: "OK",
        data: [{ Id: 43, Descripcion: "Comprobante De Egreso" }],
        errors: [],
      },
    });

    const result = await getTipologiasDocumentalesWorkflow(
      { idTareaWf: 933, idRutaWf: 9 },
      { signal },
    );

    expect(clienteApi.get).toHaveBeenCalledWith(TIPOLOGIAS_DOCUMENTALES_WORKFLOW_ENDPOINT, {
      params: {
        Contexto: "WORKFLOW",
        IdTareaWf: 933,
        IdRutaWf: 9,
      },
      signal,
    });
    expect(clienteApi.get).not.toHaveBeenCalledWith(
      expect.any(String),
      expect.objectContaining({
        params: expect.objectContaining({ IdTipoTramite: expect.anything() }),
      }),
    );
    expect(result).toEqual([
      {
        value: 43,
        label: "Comprobante De Egreso",
        idTipoDocumento: 43,
        nombreTipoDocumento: "Comprobante De Egreso",
      },
    ]);
  });

  it("acepta catalogo vacio cuando success=true", () => {
    expect(
      normalizeWorkflowTypologyResponse({
        success: true,
        message: "OK",
        data: [],
      }),
    ).toEqual([]);
  });

  it("rechaza ids invalidos antes de llamar backend", async () => {
    await expect(
      getTipologiasDocumentalesWorkflow({ idTareaWf: 0, idRutaWf: 9 }),
    ).rejects.toThrow(TipologiasDocumentalesWorkflowError);
    await expect(
      getTipologiasDocumentalesWorkflow({ idTareaWf: 933, idRutaWf: -1 }),
    ).rejects.toThrow(TipologiasDocumentalesWorkflowError);

    expect(clienteApi.get).not.toHaveBeenCalled();
  });

  it("lanza error funcional usando UserMessage cuando success=false", () => {
    expect(() =>
      normalizeWorkflowTypologyResponse({
        success: false,
        message: "Error general",
        data: [],
        errors: [{ UserMessage: "No hay tipologias para esta ruta." }],
      }),
    ).toThrow("No hay tipologias para esta ruta.");
  });

  it("lanza error para shape invalido de item", () => {
    expect(() =>
      normalizeWorkflowTypologyResponse({
        success: true,
        message: "OK",
        data: [{ Id: 0, Descripcion: " " }],
      }),
    ).toThrow(TipologiasDocumentalesWorkflowError);
  });
});
