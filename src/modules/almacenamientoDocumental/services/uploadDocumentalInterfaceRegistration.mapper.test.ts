import { describe, expect, it } from "vitest";
import type { AlmacenarDocumentoStoredResult } from "../components/AppUploadDocumental";
import { buildUploadDocumentalInterfaceRegistration } from "./uploadDocumentalInterfaceRegistration.mapper";

const stored: AlmacenarDocumentoStoredResult = {
  fileUid: "file-1",
  fileName: "scan.pdf",
  idAlmacen: 1,
  idRegistroProduccionDocumental: 2,
  nombreArchivoFinal: "scan-final.pdf",
  requestId: "req-1",
  metadata: { idTipoDocumento: 3, nombreTipoDocumento: "Contrato", fechaCarga: "2026-01-10" },
};

describe("[SPEC:SCRUMCORE-271] uploadDocumentalInterfaceRegistration mapper", () => {
  it("construye evento production por contexto cuando no hay raw especifico", () => {
    const events = buildUploadDocumentalInterfaceRegistration({
      stored,
      context: { nombreGabinete: "Gestion", idImagen: 99 },
      metadata: stored.metadata,
      proceso: "radicacion",
    });

    expect(events).toEqual([
      {
        kind: "production-document-row",
        idRegistro: 2,
        idImagen: 99,
        nombreArchivo: "scan-final.pdf",
        fecha: "2026-01-10",
        tipoDocumental: "Contrato",
        nombreGabinete: "Gestion",
      },
    ]);
  });

  it("mapea variantes conocidas desde raw backend", () => {
    const events = buildUploadDocumentalInterfaceRegistration({
      stored,
      rawBackendResult: {
        urlPreview: "/tmp/a.pdf",
        contadorPaginas: 4,
        urlImagenSemaforo: "/semaforo.png",
        dropdownText: "Respuesta",
        dropdownValue: 7,
        target: "respuesta",
      },
      context: { nombreGabinete: "Gestion" },
      metadata: stored.metadata,
      proceso: "radicacion",
    });

    expect(events).toEqual(
      expect.arrayContaining([
        { kind: "migration-preview", url: "/tmp/a.pdf", idRegistro: undefined },
        { kind: "page-counter", contadorPaginas: 4 },
        { kind: "traffic-light", urlImagenSemaforo: "/semaforo.png" },
        { kind: "dropdown-option", text: "Respuesta", value: 7, target: "respuesta" },
      ]),
    );
  });

  it("usa fallback raw cuando hay dato util no normalizable", () => {
    const raw = { codigoEvento: "EV-1" };
    const events = buildUploadDocumentalInterfaceRegistration({
      stored,
      rawBackendResult: raw,
      context: { nombreGabinete: "Gestion" },
      metadata: stored.metadata,
      proceso: "custom",
    });

    expect(events[0]).toMatchObject({ kind: "production-document-row" });
  });
});
