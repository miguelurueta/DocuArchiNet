import { describe, expect, test } from "vitest";
import {
  applySignedOverride,
  failClosedEffectivePermissions,
  mapPermisosVisorPdfToEffectivePermissions,
  resolveCodigoImplementacion,
} from "./AppVisorEmbedPdf.permissions";

describe("AppVisorEmbedPdf permissions", () => {
  test("resuelve gestioncorrespondencia al codigoImpl oficial", () => {
    expect(resolveCodigoImplementacion("gestioncorrespondencia")).toBe("gestion_correspondencia");
    expect(resolveCodigoImplementacion("GESTIONCORRESPONDENCIA")).toBe("gestion_correspondencia");
  });

  test("mapea print y download al contrato efectivo del visor", () => {
    const effective = mapPermisosVisorPdfToEffectivePermissions({
      "pdf.print": true,
      "pdf.download": true,
    });

    expect(effective.allowPrint).toBe(true);
    expect(effective.allowExport).toBe(true);
  });

  test("mapea permisos de firma documentados por backend", () => {
    const effective = mapPermisosVisorPdfToEffectivePermissions({
      "pdf.annotate.signature.place": true,
      "pdf.annotate.signature.delete": true,
      "pdf.annotate.signature.lock": true,
    });

    expect(effective.allowSignaturePlacement).toBe(true);
    expect(effective.allowSignatureDelete).toBe(true);
    expect(effective.allowSignatureLockToggle).toBe(true);
    expect(effective.allowAnnotationEdit).toBe(true);
  });

  test("unlock tambien habilita toggle de bloqueo de firma", () => {
    const effective = mapPermisosVisorPdfToEffectivePermissions({
      "pdf.annotate.signature.unlock": true,
    });

    expect(effective.allowSignatureLockToggle).toBe(true);
  });

  test("permisos vacios o desconocidos quedan fail-closed", () => {
    expect(mapPermisosVisorPdfToEffectivePermissions({})).toEqual(failClosedEffectivePermissions());
    expect(mapPermisosVisorPdfToEffectivePermissions({ "pdf.unknown": true })).toEqual(
      failClosedEffectivePermissions(),
    );
  });

  test("override por documento firmado bloquea edicion y firma", () => {
    const effective = mapPermisosVisorPdfToEffectivePermissions({
      "pdf.annotate.signature.place": true,
      "pdf.annotate.signature.delete": true,
      "pdf.annotate.signature.lock": true,
      "pdf.download": true,
      "pdf.print": true,
    });

    expect(applySignedOverride({ effective, isElectronicallySigned: true })).toEqual({
      allowSignaturePlacement: false,
      allowSignatureDelete: false,
      allowSignatureLockToggle: false,
      allowAnnotationEdit: false,
      allowExport: true,
      allowPrint: true,
    });
  });
});
