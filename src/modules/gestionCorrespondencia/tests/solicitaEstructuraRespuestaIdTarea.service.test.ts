import { beforeEach, describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  getSolicitaEstructuraRespuestaIdTarea,
  SOLICITA_ESTRUCTURA_RESPUESTA_ID_TAREA_ENDPOINT,
} from "../services/solicitaEstructuraRespuestaIdTarea.service";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    get: vi.fn(),
  },
}));

describe("[SPEC:gestion-correspondencia] solicitaEstructuraRespuestaIdTarea.service", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("llama el endpoint esperado con idTareaWf por querystring", async () => {
    vi.mocked(clienteApi.get).mockResolvedValue({
      data: {
        success: true,
        message: "YES",
        data: [],
        errors: [],
      },
    });

    await getSolicitaEstructuraRespuestaIdTarea(12345);

    expect(clienteApi.get).toHaveBeenCalledWith(
      SOLICITA_ESTRUCTURA_RESPUESTA_ID_TAREA_ENDPOINT,
      {
        params: { idTareaWf: 12345 },
      },
    );
  });

  it("devuelve el contrato tipado sin transformar la respuesta del backend", async () => {
    vi.mocked(clienteApi.get).mockResolvedValue({
      data: {
        success: true,
        message: "YES",
        data: [
          {
            Radicado: "2025-0001",
            Destinatario: "Contasoft Company",
            TramiteDocumento: "Respuesta a derecho de petición",
          },
        ],
        errors: [],
      },
    });

    await expect(getSolicitaEstructuraRespuestaIdTarea(12345)).resolves.toEqual({
      success: true,
      message: "YES",
      data: [
        {
          Radicado: "2025-0001",
          Destinatario: "Contasoft Company",
          TramiteDocumento: "Respuesta a derecho de petición",
        },
      ],
      errors: [],
    });
  });
});
