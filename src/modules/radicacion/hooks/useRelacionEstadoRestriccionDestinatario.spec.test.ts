import { describe, expect, it } from "vitest";
import {
  C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT,
} from "../models/CDeRelacionEstadoRetriccionDto";
import {
  buildRestriccionDestinatarioPayload,
  normalizeRestriccionDestinatario,
} from "./useRelacionEstadoRestriccionDestinatario";

describe("useRelacionEstadoRestriccionDestinatario helpers", () => {
  it("[SPEC:RDS-001] inicializa constante de restriccion de destinatario", () => {
    expect(C_DE_RELACION_ESTADO_RETRICCION_DESTINATARIO_DEFAULT).toEqual({
      IdRestriTipoDestInterno: 0,
      IdTipoRestriccion: 0,
      DescripcionTipo: "string",
      MoluloRadicacion: 0,
      ModuloRadicacionSimple: 0,
      ModuloRadicacionInterna: 0,
    });
  });

  it("[SPEC:RDS-002] normaliza actualizacion desde API con estructura parcial", () => {
    expect(
      normalizeRestriccionDestinatario({
        IdRestriTipoDestInterno: 4,
        IdTipoRestriccion: 2,
        DescripcionTipo: "RESTRICCION ACTIVA",
        ModuloRadicacionSimple: 1,
      }),
    ).toEqual({
      IdRestriTipoDestInterno: 4,
      IdTipoRestriccion: 2,
      DescripcionTipo: "RESTRICCION ACTIVA",
      MoluloRadicacion: 0,
      ModuloRadicacionSimple: 1,
      ModuloRadicacionInterna: 0,
    });
  });

  it("[SPEC:RDS-003] construye payload para consumo de servicio", () => {
    const payload = buildRestriccionDestinatarioPayload({
      IdRestriTipoDestInterno: 7,
      IdTipoRestriccion: 9,
      DescripcionTipo: "DEST",
      MoluloRadicacion: 1,
      ModuloRadicacionSimple: 1,
      ModuloRadicacionInterna: 0,
    });

    expect(payload).toEqual({
      IdRestriTipoDestInterno: 7,
      IdTipoRestriccion: 9,
      DescripcionTipo: "DEST",
      MoluloRadicacion: 1,
      ModuloRadicacionSimple: 1,
      ModuloRadicacionInterna: 0,
    });
  });
});
