import { adjuntarDigitalizacion, validarAdjuntarDigitalizacion } from "./adjuntarDigitalizacion.api";
import { getDigitalizacionConfiguracion } from "./digitalizacionConfiguracion.api";
import { crearDocumentoDigitalizado } from "./digitalizacionDocumentos.api";
import { getDigitalizacionListaChequeo } from "./digitalizacionListaChequeo.api";
import { resolveDigitalizacionMetadata } from "./digitalizacionMetadata.api";
import { uploadPdfTemporal } from "./digitalizacionUploadTemporal.api";
import type { DigitalizacionApiClient } from "../types/digitalizacionApi.types";

export const digitalizacionApiClient: DigitalizacionApiClient = {
  getConfiguracion: getDigitalizacionConfiguracion,
  getListaChequeo: getDigitalizacionListaChequeo,
  resolveMetadata: resolveDigitalizacionMetadata,
  uploadPdfTemporal,
  crearDocumentoDigitalizado,
  validarAdjuntarDigitalizacion,
  adjuntarDigitalizacion,
};
