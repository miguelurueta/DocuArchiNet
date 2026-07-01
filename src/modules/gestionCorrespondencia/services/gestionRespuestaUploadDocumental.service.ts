import type {
  TipoDocumentalOption,
  UploadDocumentalConfig,
  UploadDocumentalContext,
  UploadDocumentalProcessKey,
} from "../../almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.types";
import {
  getTipologiasDocumentalesWorkflow,
  TipologiasDocumentalesWorkflowError,
} from "./tipologiasDocumentalesWorkflow.service";

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

export type GestionRespuestaUploadConfigInput = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
};

export type GestionRespuestaTiposDocumentalesInput = {
  proceso: UploadDocumentalProcessKey;
  context: UploadDocumentalContext;
};

export async function loadGestionRespuestaTiposDocumentales({
  context,
}: GestionRespuestaTiposDocumentalesInput): Promise<TipoDocumentalOption[]> {
  const idTareaWf = requirePositiveNumber(context.idTareaWorkflow, "idTareaWf");
  const idRutaWf = requirePositiveNumber(context.idRutaWorkflow, "idRutaWf");
  const options = await getTipologiasDocumentalesWorkflow({ idTareaWf, idRutaWf });

  return options.map((option) => ({
    idTipoDocumento: option.idTipoDocumento,
    nombreTipoDocumento: option.nombreTipoDocumento,
  }));
}

function requirePositiveNumber(value: number | undefined, fieldName: string): number {
  if (typeof value !== "number" || !Number.isFinite(value) || value <= 0) {
    throw new TipologiasDocumentalesWorkflowError(
      `${fieldName} es requerido para cargar las tipologias documentales del workflow.`,
    );
  }

  return value;
}
