import clienteApi from "../../../api/Clienteaxios";
import type {
  CrearDocumentoDigitalizadoRequest,
  CrearDocumentoDigitalizadoResponse,
  DigitalizacionApiRequestOptions,
} from "../types/digitalizacionApi.types";
import {
  assertNonEmptyString,
  assertPositiveNumber,
  assertRecord,
  getNumber,
  getString,
  unwrapAppResponse,
  withSignal,
} from "./digitalizacionApiClient";

export const DIGITALIZACION_DOCUMENTOS_ENDPOINT =
  "/api/gestor-documental/digitalizacion/documentos";

export const validateCrearDocumentoRequest = (request: CrearDocumentoDigitalizadoRequest) => {
  assertNonEmptyString(request.NombreGabinete, "NOMBRE_GABINETE_REQUIRED", "NombreGabinete es obligatorio.");
  assertNonEmptyString(request.RutaTemporalId, "RUTA_TEMPORAL_REQUIRED", "RutaTemporalId es obligatorio.");
  assertNonEmptyString(
    request.ArchivoTemporalId,
    "ARCHIVO_TEMPORAL_REQUIRED",
    "ArchivoTemporalId es obligatorio.",
  );
  assertNonEmptyString(request.NombreDocumento, "NOMBRE_DOCUMENTO_REQUIRED", "NombreDocumento es obligatorio.");
};

const validateCrearDocumentoResponse = (value: unknown): CrearDocumentoDigitalizadoResponse => {
  const record = assertRecord(value, "CREAR_DOCUMENTO_INVALID", "Respuesta crear documento invalida.");
  const idDocumento = assertPositiveNumber(
    getNumber(record, "idDocumento", "IdDocumento"),
    "ID_DOCUMENTO_INVALID",
    "IdDocumento debe ser mayor a cero.",
    "idDocumento",
  );
  const nombreGabinete = assertNonEmptyString(
    getString(record, "nombreGabinete", "NombreGabinete"),
    "NOMBRE_GABINETE_REQUIRED",
    "NombreGabinete es obligatorio.",
  );
  const nombreDocumento = assertNonEmptyString(
    getString(record, "nombreDocumento", "NombreDocumento"),
    "NOMBRE_DOCUMENTO_REQUIRED",
    "NombreDocumento es obligatorio.",
  );

  return {
    idDocumento,
    nombreGabinete,
    nombreDocumento,
    extension: getString(record, "extension", "Extension") ?? "pdf",
    numeroPaginas: getNumber(record, "numeroPaginas", "NumeroPaginas") ?? 0,
    radicado: getString(record, "radicado", "Radicado"),
    requestId: getString(record, "requestId", "RequestId"),
  };
};

export async function crearDocumentoDigitalizado(
  request: CrearDocumentoDigitalizadoRequest,
  options: DigitalizacionApiRequestOptions = {},
) {
  validateCrearDocumentoRequest(request);
  const response = await clienteApi.post(DIGITALIZACION_DOCUMENTOS_ENDPOINT, request, {
    ...withSignal(options.signal),
  });

  return unwrapAppResponse<CrearDocumentoDigitalizadoResponse>(
    response.data,
    validateCrearDocumentoResponse,
    "crear-documento",
  );
}
