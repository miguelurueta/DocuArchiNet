import clienteApi from "../../../api/Clienteaxios";
import type {
  DigitalizacionApiRequestOptions,
  DigitalizacionConfiguracionQuery,
  DigitalizacionConfiguracionResponse,
} from "../types/digitalizacionApi.types";
import {
  assertRecord,
  getBoolean,
  getNumber,
  getString,
  getStringArray,
  createDigitalizacionApiError,
  unwrapAppResponse,
  withSignal,
} from "./digitalizacionApiClient";

export const DIGITALIZACION_CONFIGURACION_ENDPOINT =
  "/api/gestor-documental/digitalizacion/configuracion";

const validateConfiguracion = (value: unknown): DigitalizacionConfiguracionResponse => {
  const record = assertRecord(
    value,
    "CONFIGURACION_INVALID",
    "Configuracion de digitalizacion invalida.",
  );

  const idConfiguracionDigitalizacion = getNumber(
    record,
    "idConfiguracionDigitalizacion",
    "IdConfiguracionDigitalizacion",
  );
  const tipoDigitalizacion = getString(record, "tipoDigitalizacion", "TipoDigitalizacion");
  const nombreGabinete = getString(record, "nombreGabinete", "NombreGabinete");

  if (!idConfiguracionDigitalizacion || idConfiguracionDigitalizacion <= 0) {
    throw createDigitalizacionApiError(
      "CONFIGURACION_ID_INVALID",
      "IdConfiguracionDigitalizacion debe ser mayor a cero.",
      "error",
      "idConfiguracionDigitalizacion",
    );
  }
  if (!tipoDigitalizacion || !nombreGabinete) {
    throw createDigitalizacionApiError(
      "CONFIGURACION_REQUIRED_FIELDS",
      "Configuracion sin campos obligatorios.",
      "error",
    );
  }

  return {
    idConfiguracionDigitalizacion,
    tipoDigitalizacion,
    nombreGabinete,
    activaListaChequeo: getBoolean(record, "activaListaChequeo", "ActivaListaChequeo") ?? false,
    obligaListaChequeo: getBoolean(record, "obligaListaChequeo", "ObligaListaChequeo") ?? false,
    permiteCrearDocumento:
      getBoolean(record, "permiteCrearDocumento", "PermiteCrearDocumento") ?? false,
    permiteAdjuntarDocumento:
      getBoolean(record, "permiteAdjuntarDocumento", "PermiteAdjuntarDocumento") ?? false,
    requiereMetadata: getBoolean(record, "requiereMetadata", "RequiereMetadata") ?? false,
    formatosPermitidos: getStringArray(record, "formatosPermitidos", "FormatosPermitidos"),
  };
};

export async function getDigitalizacionConfiguracion(
  query: DigitalizacionConfiguracionQuery,
  options: DigitalizacionApiRequestOptions = {},
) {
  const response = await clienteApi.get(DIGITALIZACION_CONFIGURACION_ENDPOINT, {
    params: query,
    ...withSignal(options.signal),
  });

  return unwrapAppResponse<DigitalizacionConfiguracionResponse>(
    response.data,
    validateConfiguracion,
    "configuracion",
  );
}
