import { describe, expect, it, vi } from "vitest";
import clienteApi from "../../../api/Clienteaxios";
import {
  RADICACION_REGISTRAR_ENTRANTE_ENDPOINT,
  registrarRadicacionEntrante,
} from "./radicacionRegistro.service";
import type { RegistrarRadicacionEntranteRequestDto } from "../types/radicacionRegistro.types";

vi.mock("../../../api/Clienteaxios", () => ({
  default: {
    post: vi.fn(),
  },
}));

const mockedPost = vi.mocked(clienteApi.post);

const request: RegistrarRadicacionEntranteRequestDto = {
  tipoModuloRadicacion: 1,
  ASUNTO: "Solicitud",
  Remitente: { Nombre: "Remitente", id_Dest_Ext: 1 },
  Destinatario: { Destinatario: "Destino", id_Remit_Dest_Int: 2 },
  Tipo_tramite: { Descripcion: "PQRS", tipo_doc_entrante: 3 },
  RE_flujo_trabajo: { NombreFlujo: "Flujo", id_tipo_flujo_workflow: 4 },
  TipoRadicado: { TipoRadicacion: "Entrada", IdTipoRadicado: 5 },
  TipoPlantillaRadicado: {
    TipoPlantillaRadicado: "Plantilla",
    IdTipoPlantillaRdicado: 6,
  },
  expedienteRelacionado: { Expediente: "", idExpediente: 0 },
  radicadoRelacionados: [],
  ANEXOS_COR: "Anexo",
  FECHALIMITERESPUESTA: "2026-07-08",
  numeroFolios: null,
  Campos: [],
};

describe("radicacionRegistro.service", () => {
  it("[SPEC:FE-01] ejecuta POST registrar-entrante con tipoModuloRadicacion", async () => {
    mockedPost.mockResolvedValueOnce({
      data: {
        success: true,
        message: "OK",
        data: { ConsecutivoRadicado: "RAD-1" },
      },
    });

    await expect(registrarRadicacionEntrante(request)).resolves.toMatchObject({
      success: true,
      data: { ConsecutivoRadicado: "RAD-1" },
    });
    expect(mockedPost).toHaveBeenCalledWith(
      RADICACION_REGISTRAR_ENTRANTE_ENDPOINT,
      request,
      { params: { tipoModuloRadicacion: 1 } },
    );
    expect(mockedPost.mock.calls[0]?.[1]).not.toHaveProperty("ModuloRegistro");
  });
});
