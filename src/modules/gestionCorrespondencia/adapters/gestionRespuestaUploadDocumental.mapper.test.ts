import { describe, expect, it } from "vitest";
import {
  buildGestionRespuestaAlmacenarDocumentoRequest,
  GestionRespuestaUploadDocumentalMapperError,
  isWorkflowAnexoCreated,
  normalizeWorkflowAnexoStorageResult,
} from "./gestionRespuestaUploadDocumental.mapper";

const baseInput = {
  context: {
    nombreGabinete: "CORRESPO",
    idRespuesta: 672,
    idTareaWorkflow: 933,
    idRutaWorkflow: 9,
    nameModulo: "2600466700021",
    idUsuarioGestion: 136,
    idEmpresa: 2,
    fechaElaboracion: "2026-07-02",
  },
  metadata: {
    idTipoDocumento: 43,
    nombreTipoDocumento: "Comprobante De Egreso",
    numeroPaginas: 1,
  },
  fileName: "soporte-respuesta.pdf",
  requestId: "workflow-anexo-2600466700021-001",
};

describe("[SCRUMCORE-277] gestionRespuestaUploadDocumental.mapper", () => {
  it("construye request final con AnexoRespuesta, CabinetIndexSeed y Trd", () => {
    const request = buildGestionRespuestaAlmacenarDocumentoRequest(baseInput);

    expect(request).toMatchObject({
      nombreGabinete: "CORRESPO",
      nombreDocumento: "Anexo workflow respuesta 2600466700021",
      requestId: "workflow-anexo-2600466700021-001",
      inventario: {
        IdUsuarioGestion: 136,
        IdEmpresa: 2,
        Radicado: "2600466700021",
        FechaElaboracion: "2026-07-02",
      },
      trd: {
        idTipoDocumento: 43,
        nombreTipoDocumento: "Comprobante De Egreso",
      },
      cabinetIndexSeed: {
        sourceModule: "RADICACION",
        providerKey: "RADICACION",
        version: "1.0.0",
        payload: {
          modoResolucion: "RespuestaRadicado",
        },
      },
      anexoRespuesta: {
        idRespuestaRadicado: 672,
        nombreArchivo: "soporte-respuesta.pdf",
        tipoAdjunto: "respuesta",
      },
      documento: {
        idDocumento: "wf-anexo-workflow-anexo-2600466700021-001",
        nombreOriginal: "soporte-respuesta.pdf",
        extension: ".pdf",
        numeroPaginas: 1,
      },
    });
  });

  it("usa solo el nombre del archivo y no conserva ruta local", () => {
    const request = buildGestionRespuestaAlmacenarDocumentoRequest({
      ...baseInput,
      fileName: "C:\\temp\\soporte-respuesta.pdf",
    });

    expect(request.anexoRespuesta?.nombreArchivo).toBe("soporte-respuesta.pdf");
    expect(request.documento?.nombreOriginal).toBe("soporte-respuesta.pdf");
  });

  it("rechaza contexto sin idRespuestaRadicado valido", () => {
    expect(() =>
      buildGestionRespuestaAlmacenarDocumentoRequest({
        ...baseInput,
        context: { ...baseInput.context, idRespuesta: undefined },
      }),
    ).toThrow(GestionRespuestaUploadDocumentalMapperError);
  });

  it("normaliza response anidado y confirma Created true", () => {
    const raw = {
      Documento: {
        IdAlmacen: 9967,
        IdRegistroProduccionDocumental: 23040,
        NombreArchivoFinal: "DIG00009967.pdf",
      },
      AnexoRespuesta: {
        IdAnexoRespuesta: 150,
        IdRespuestaRadicado: 672,
        IdAlmacen: 9967,
        NombreGabinete: "CORRESPO",
        NombreArchivo: "soporte-respuesta.pdf",
        Created: true,
      },
      Indice: {
        ProviderKey: "RADICACION",
        Resolved: true,
        SourceTrace: "ra_respuesta_radicado",
      },
      Workflow: {
        LogInserted: true,
        IdTareaWorkflow: 933,
        IdRutaWorkflow: 9,
      },
    };

    expect(normalizeWorkflowAnexoStorageResult(raw)).toMatchObject({
      documento: { idAlmacen: 9967, nombreArchivoFinal: "DIG00009967.pdf" },
      anexoRespuesta: { idAnexoRespuesta: 150, created: true },
      indice: { providerKey: "RADICACION", resolved: true },
      workflow: { logInserted: true, idTareaWorkflow: 933 },
    });
    expect(isWorkflowAnexoCreated(raw)).toBe(true);
  });

  it("no confirma anexo si Created no es true", () => {
    expect(
      isWorkflowAnexoCreated({
        Documento: {
          IdAlmacen: 9967,
          IdRegistroProduccionDocumental: 23040,
          NombreArchivoFinal: "DIG00009967.pdf",
        },
        AnexoRespuesta: {
          IdRespuestaRadicado: 672,
          IdAlmacen: 9967,
          NombreGabinete: "CORRESPO",
          NombreArchivo: "soporte-respuesta.pdf",
          Created: false,
        },
      }),
    ).toBe(false);
  });
});
