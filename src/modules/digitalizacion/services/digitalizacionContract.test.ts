import { describe, expect, it } from "vitest";
import { validateDigitalizacionContext } from "./digitalizacionContract";
import type { DigitalizacionContext } from "../types/digitalizacion.types";

describe("[SPEC:SCRUMCORE-239] digitalizacion contract", () => {
  it("requires context", () => {
    expect(validateDigitalizacionContext(null)).toMatchObject({
      code: "CONTEXT_REQUIRED",
      message: "El contexto documental es obligatorio.",
    });
  });

  it("requires a valid mode", () => {
    const context = {
      modo: "editar",
      nombreGabinete: "Gestion",
    } as unknown as DigitalizacionContext;

    expect(validateDigitalizacionContext(context)).toMatchObject({
      code: "INVALID_MODE",
      field: "modo",
    });
  });

  it("requires nombreGabinete", () => {
    expect(
      validateDigitalizacionContext({
        modo: "crear",
        nombreGabinete: "   ",
      }),
    ).toMatchObject({
      code: "NOMBRE_GABINETE_REQUIRED",
      message: "nombreGabinete es obligatorio.",
    });
  });

  it("requires idDocumentoDestino for adjuntar", () => {
    expect(
      validateDigitalizacionContext({
        modo: "adjuntar",
        nombreGabinete: "Gestion",
        idDocumentoDestino: 0,
      }),
    ).toMatchObject({
      code: "ID_DOCUMENTO_DESTINO_REQUIRED",
      message: "idDocumentoDestino es obligatorio para modo adjuntar.",
    });
  });

  it("accepts crear context", () => {
    expect(
      validateDigitalizacionContext({
        modo: "crear",
        nombreGabinete: "Gestion",
      }),
    ).toBeNull();
  });
});
