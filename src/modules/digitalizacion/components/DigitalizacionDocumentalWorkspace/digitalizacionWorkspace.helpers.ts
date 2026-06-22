import { DynamsoftScannerError } from "../../infrastructure/dynamsoft";
import type { DigitalizacionScannerClient } from "../../infrastructure/dynamsoft";

export const buildDigitalizacionTitle = (modo?: string) =>
  modo === "adjuntar" ? "Adjuntar digitalizacion" : "Digitalizar documento";

export const unavailableScannerClient: DigitalizacionScannerClient = {
  initialize: async () => undefined,
  listDevices: async () => [],
  selectDevice: async () => undefined,
  scan: async () => {
    throw new DynamsoftScannerError({
      code: "SCANNER_NOT_SELECTED",
      message: "Seleccione un scanner antes de escanear.",
    });
  },
  duplicatePage: async () => [],
  rotatePage: async () => [],
  cropPage: async () => [],
  removePage: async () => undefined,
  reorderPages: async () => [],
  clear: async () => undefined,
  generatePdf: async () => {
    throw new DynamsoftScannerError({
      code: "PDF_EMPTY",
      message: "No hay paginas para generar PDF.",
    });
  },
  dispose: async () => undefined,
};
