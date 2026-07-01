import type {
  TipoDocumentalOption,
  UploadDocumentalConfig,
  UploadDocumentalContext,
  UploadDocumentalProcessKey,
} from "../../almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.types";

const DEFAULT_MAX_SIZE_BYTES = 25 * 1024 * 1024;
const DEFAULT_ALLOWED_EXTENSIONS = [".pdf", ".png", ".jpg", ".jpeg", ".tif", ".tiff"];

export async function loadGestionRespuestaUploadConfig(): Promise<UploadDocumentalConfig> {
  return {
    accept: DEFAULT_ALLOWED_EXTENSIONS.join(","),
    allowedExtensions: DEFAULT_ALLOWED_EXTENSIONS,
    maxSizeBytes: DEFAULT_MAX_SIZE_BYTES,
    multiple: true,
    requiereTipologia: true,
    requiereFechaCarga: false,
    fechaCargaObligatoria: false,
    validationMode: "queue-with-error",
  };
}

export async function loadGestionRespuestaTiposDocumentales(): Promise<TipoDocumentalOption[]> {
  return [
    {
      idTipoDocumento: 43,
      nombreTipoDocumento: "Comprobante De Egreso",
    },
  ];
}

export type GestionRespuestaUploadConfigInput = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
};

export type GestionRespuestaTiposDocumentalesInput = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
};
