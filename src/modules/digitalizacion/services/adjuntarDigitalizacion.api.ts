import clienteApi from "../../../api/Clienteaxios";
import type {
  AdjuntarDigitalizacionPdfRequest,
  AdjuntarDigitalizacionPdfResponse,
  AdjuntarDigitalizacionValidacionQuery,
  AdjuntarDigitalizacionValidacionResponse,
  DigitalizacionApiRequestOptions,
} from "../types/digitalizacionApi.types";
import {
  assertNonEmptyString,
  assertPositiveNumber,
  assertRecord,
  getBoolean,
  getNumber,
  getString,
  unwrapAppResponse,
  withSignal,
} from "./digitalizacionApiClient";

export const getAdjuntarDigitalizacionValidacionEndpoint = (idDocumento: number) =>
  `/api/gestor-documental/documentos/${idDocumento}/adjuntar-digitalizacion/validacion`;

export const getAdjuntarDigitalizacionEndpoint = (idDocumento: number) =>
  `/api/gestor-documental/documentos/${idDocumento}/adjuntar-digitalizacion`;

const validateIdDocumentoDestino = (idDocumento: number) =>
  assertPositiveNumber(
    idDocumento,
    "ID_DOCUMENTO_DESTINO_REQUIRED",
    "idDocumento debe ser mayor a cero.",
    "idDocumento",
  );

export const validateAdjuntarRequest = (request: AdjuntarDigitalizacionPdfRequest) => {
  assertNonEmptyString(request.NombreGabinete, "NOMBRE_GABINETE_REQUIRED", "NombreGabinete es obligatorio.");
  assertNonEmptyString(request.RutaTemporalId, "RUTA_TEMPORAL_REQUIRED", "RutaTemporalId es obligatorio.");
  assertNonEmptyString(
    request.ArchivoTemporalId,
    "ARCHIVO_TEMPORAL_REQUIRED",
    "ArchivoTemporalId es obligatorio.",
  );
};

const validateValidacionResponse = (value: unknown): AdjuntarDigitalizacionValidacionResponse => {
  const record = assertRecord(value, "ADJUNTAR_VALIDACION_INVALID", "Validacion adjuntar invalida.");
  const idDocumento = validateIdDocumentoDestino(getNumber(record, "idDocumento", "IdDocumento") ?? 0);
  const nombreGabinete = assertNonEmptyString(
    getString(record, "nombreGabinete", "NombreGabinete"),
    "NOMBRE_GABINETE_REQUIRED",
    "NombreGabinete es obligatorio.",
  );

  return {
    idDocumento,
    nombreGabinete,
    permitido: getBoolean(record, "permitido", "Permitido") ?? false,
    codigoBloqueo: getString(record, "codigoBloqueo", "CodigoBloqueo"),
    mensajeBloqueo: getString(record, "mensajeBloqueo", "MensajeBloqueo"),
    esPdf: getBoolean(record, "esPdf", "EsPdf") ?? false,
    estaFirmado: getBoolean(record, "estaFirmado", "EstaFirmado") ?? false,
    estaBloqueado: getBoolean(record, "estaBloqueado", "EstaBloqueado") ?? false,
    radicadoNoModificable:
      getBoolean(record, "radicadoNoModificable", "RadicadoNoModificable") ?? false,
    numeroPaginasActual: getNumber(record, "numeroPaginasActual", "NumeroPaginasActual"),
  };
};

const validateAdjuntarResponse = (value: unknown): AdjuntarDigitalizacionPdfResponse => {
  const record = assertRecord(value, "ADJUNTAR_DIGITALIZACION_INVALID", "Respuesta adjuntar invalida.");
  const idDocumento = validateIdDocumentoDestino(getNumber(record, "idDocumento", "IdDocumento") ?? 0);
  const nombreGabinete = assertNonEmptyString(
    getString(record, "nombreGabinete", "NombreGabinete"),
    "NOMBRE_GABINETE_REQUIRED",
    "NombreGabinete es obligatorio.",
  );

  return {
    idDocumento,
    nombreGabinete,
    extension: getString(record, "extension", "Extension") ?? "pdf",
    numeroPaginasAnterior: getNumber(record, "numeroPaginasAnterior", "NumeroPaginasAnterior") ?? 0,
    numeroPaginasAgregadas:
      getNumber(record, "numeroPaginasAgregadas", "NumeroPaginasAgregadas") ?? 0,
    numeroPaginasFinal: getNumber(record, "numeroPaginasFinal", "NumeroPaginasFinal") ?? 0,
    documentoActualizado: getBoolean(record, "documentoActualizado", "DocumentoActualizado") ?? false,
    requestId: getString(record, "requestId", "RequestId"),
  };
};

export async function validarAdjuntarDigitalizacion(
  idDocumento: number,
  query: AdjuntarDigitalizacionValidacionQuery,
  options: DigitalizacionApiRequestOptions = {},
) {
  validateIdDocumentoDestino(idDocumento);
  assertNonEmptyString(query.NombreGabinete, "NOMBRE_GABINETE_REQUIRED", "NombreGabinete es obligatorio.");

  const response = await clienteApi.get(getAdjuntarDigitalizacionValidacionEndpoint(idDocumento), {
    params: query,
    ...withSignal(options.signal),
  });

  return unwrapAppResponse<AdjuntarDigitalizacionValidacionResponse>(
    response.data,
    validateValidacionResponse,
    "adjuntar-validacion",
  );
}

export async function adjuntarDigitalizacion(
  idDocumento: number,
  request: AdjuntarDigitalizacionPdfRequest,
  options: DigitalizacionApiRequestOptions = {},
) {
  validateIdDocumentoDestino(idDocumento);
  validateAdjuntarRequest(request);

  const response = await clienteApi.post(getAdjuntarDigitalizacionEndpoint(idDocumento), request, {
    ...withSignal(options.signal),
  });

  return unwrapAppResponse<AdjuntarDigitalizacionPdfResponse>(
    response.data,
    validateAdjuntarResponse,
    "adjuntar-digitalizacion",
  );
}
