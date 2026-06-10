import clienteApi from "../../../api/Clienteaxios";
import type {
  DigitalizacionApiRequestOptions,
  DigitalizacionListaChequeoItem,
  DigitalizacionListaChequeoQuery,
  DigitalizacionListaChequeoResponse,
} from "../types/digitalizacionApi.types";
import {
  assertRecord,
  getBoolean,
  getNumber,
  getString,
  createDigitalizacionApiError,
  unwrapAppResponse,
  withSignal,
} from "./digitalizacionApiClient";

export const DIGITALIZACION_LISTA_CHEQUEO_ENDPOINT =
  "/api/gestor-documental/digitalizacion/lista-chequeo";

const validateItem = (value: unknown): DigitalizacionListaChequeoItem => {
  const record = assertRecord(value, "LISTA_CHEQUEO_ITEM_INVALID", "Item de lista invalido.");
  const idTipoListaChequeo = getNumber(record, "idTipoListaChequeo", "IdTipoListaChequeo");
  const nombreTipoDocumento = getString(record, "nombreTipoDocumento", "NombreTipoDocumento");

  if (!idTipoListaChequeo || idTipoListaChequeo <= 0 || !nombreTipoDocumento) {
    throw createDigitalizacionApiError(
      "LISTA_CHEQUEO_ITEM_REQUIRED_FIELDS",
      "Item de lista sin campos obligatorios.",
      "error",
    );
  }

  return {
    idTipoListaChequeo,
    nombreTipoDocumento,
    idArea: getNumber(record, "idArea", "IdArea"),
    idSerie: getNumber(record, "idSerie", "IdSerie"),
    idSubSerie: getNumber(record, "idSubSerie", "IdSubSerie"),
    idTipoDocumento: getNumber(record, "idTipoDocumento", "IdTipoDocumento"),
    nombreArea: getString(record, "nombreArea", "NombreArea"),
    nombreSerie: getString(record, "nombreSerie", "NombreSerie"),
    nombreSubSerie: getString(record, "nombreSubSerie", "NombreSubSerie"),
    esUnico: getBoolean(record, "esUnico", "EsUnico") ?? false,
    obligatorio: getBoolean(record, "obligatorio", "Obligatorio") ?? false,
    disponible: getBoolean(record, "disponible", "Disponible") ?? true,
    mensajeNoDisponible: getString(record, "mensajeNoDisponible", "MensajeNoDisponible"),
  };
};

const validateListaChequeo = (value: unknown): DigitalizacionListaChequeoResponse => {
  const record = assertRecord(value, "LISTA_CHEQUEO_INVALID", "Lista de chequeo invalida.");
  const idConfiguracionDigitalizacion = getNumber(
    record,
    "idConfiguracionDigitalizacion",
    "IdConfiguracionDigitalizacion",
  );
  const rawItems = record.items ?? record.Items;

  if (!idConfiguracionDigitalizacion || idConfiguracionDigitalizacion <= 0 || !Array.isArray(rawItems)) {
    throw createDigitalizacionApiError(
      "LISTA_CHEQUEO_REQUIRED_FIELDS",
      "Lista de chequeo sin campos obligatorios.",
      "error",
    );
  }

  return {
    idConfiguracionDigitalizacion,
    obligaListaChequeo: getBoolean(record, "obligaListaChequeo", "ObligaListaChequeo") ?? false,
    items: rawItems.map(validateItem),
  };
};

export async function getDigitalizacionListaChequeo(
  query: DigitalizacionListaChequeoQuery,
  options: DigitalizacionApiRequestOptions = {},
) {
  const response = await clienteApi.get(DIGITALIZACION_LISTA_CHEQUEO_ENDPOINT, {
    params: query,
    ...withSignal(options.signal),
  });

  return unwrapAppResponse<DigitalizacionListaChequeoResponse>(
    response.data,
    validateListaChequeo,
    "lista-chequeo",
  );
}
