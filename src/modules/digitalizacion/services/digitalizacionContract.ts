import type {
  DigitalizacionContext,
  DigitalizacionFunctionalError,
} from "../types/digitalizacion.types";

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

const isPositiveNumber = (value: unknown): value is number =>
  typeof value === "number" && Number.isFinite(value) && value > 0;

export const buildDigitalizacionContextSignature = (
  context: DigitalizacionContext | null,
) => {
  if (!context) {
    return "null";
  }

  return JSON.stringify({
    modo: context.modo,
    nombreGabinete: context.nombreGabinete,
    radicado: context.radicado ?? null,
    idTramite: context.idTramite ?? null,
    tipoTramite: context.tipoTramite ?? null,
    idTareaWorkflow: context.idTareaWorkflow ?? null,
    idRutaWorkflow: context.idRutaWorkflow ?? null,
    idDocumentoDestino: context.idDocumentoDestino ?? null,
    requiereMetadata: context.requiereMetadata ?? false,
    titulo: context.titulo ?? null,
    sourceModule: context.sourceModule ?? null,
  });
};

export const validateDigitalizacionContext = (
  context: DigitalizacionContext | null,
): DigitalizacionFunctionalError | null => {
  if (!isRecord(context)) {
    return {
      code: "CONTEXT_REQUIRED",
      message: "El contexto documental es obligatorio.",
    };
  }

  if (context.modo !== "crear" && context.modo !== "adjuntar") {
    return {
      code: "INVALID_MODE",
      message: "modo debe ser crear o adjuntar.",
      field: "modo",
    };
  }

  if (
    typeof context.nombreGabinete !== "string" ||
    context.nombreGabinete.trim().length === 0
  ) {
    return {
      code: "NOMBRE_GABINETE_REQUIRED",
      message: "nombreGabinete es obligatorio.",
      field: "nombreGabinete",
    };
  }

  if (context.modo === "adjuntar" && !isPositiveNumber(context.idDocumentoDestino)) {
    return {
      code: "ID_DOCUMENTO_DESTINO_REQUIRED",
      message: "idDocumentoDestino es obligatorio para modo adjuntar.",
      field: "idDocumentoDestino",
    };
  }

  return null;
};
