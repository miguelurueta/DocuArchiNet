import { describe, expect, it } from "vitest";
import {
  applyTipoDocumentalSuggestion,
  isValidDocumentalDate,
  normalizeTipoDocumentalText,
  suggestTipoDocumental,
  tokenizeTipoDocumentalText,
} from "./tipoDocumentalSuggestion.utils";

describe("[SPEC:SCRUMCORE-271] tipoDocumentalSuggestion utils", () => {
  it("normaliza texto, tokeniza e ignora tokens cortos", () => {
    expect(normalizeTipoDocumentalText("Cámara - FACTURA_2026.pdf")).toBe("CAMARA FACTURA 2026 PDF");
    expect(tokenizeTipoDocumentalText("ACTA de reunion")).toEqual(["ACTA", "REUNION"]);
  });

  it("sugiere la mejor tipologia cuando supera el umbral", () => {
    const suggestion = suggestTipoDocumental({
      fileName: "contrato_arrendamiento_final.pdf",
      options: [
        { idTipoDocumento: 1, nombreTipoDocumento: "Factura venta" },
        { idTipoDocumento: 2, nombreTipoDocumento: "Contrato arrendamiento" },
      ],
      threshold: 0.4,
    });

    expect(suggestion?.option.idTipoDocumento).toBe(2);
    expect(suggestion?.score).toBeGreaterThanOrEqual(0.4);
  });

  it("no sugiere cuando no alcanza el umbral y no sobreescribe seleccion manual", () => {
    const suggestion = suggestTipoDocumental({
      fileName: "imagen.png",
      options: [{ idTipoDocumento: 1, nombreTipoDocumento: "Factura venta" }],
      threshold: 0.9,
    });

    expect(suggestion).toBeNull();
    expect(
      applyTipoDocumentalSuggestion(
        { idTipoDocumento: 10, nombreTipoDocumento: "Manual", tipologiaManual: true },
        { option: { idTipoDocumento: 1, nombreTipoDocumento: "Factura venta" }, score: 1 },
      ),
    ).toMatchObject({ idTipoDocumento: 10, nombreTipoDocumento: "Manual" });
  });

  it("valida fecha documental real y no futura", () => {
    const nextYear = new Date().getFullYear() + 1;

    expect(isValidDocumentalDate("2025-02-28")).toBe(true);
    expect(isValidDocumentalDate("2025-02-30")).toBe(false);
    expect(isValidDocumentalDate(`${nextYear}-01-01`)).toBe(false);
  });
});
