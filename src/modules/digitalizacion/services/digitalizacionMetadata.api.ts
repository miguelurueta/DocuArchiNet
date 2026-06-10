import clienteApi from "../../../api/Clienteaxios";
import type {
  DigitalizacionApiRequestOptions,
  DigitalizacionMetadataResolveRequest,
  DigitalizacionMetadataResolveResponse,
} from "../types/digitalizacionApi.types";
import {
  assertRecord,
  createDigitalizacionApiError,
  getBoolean,
  getNumber,
  normalizeTrd,
  unwrapAppResponse,
  withSignal,
} from "./digitalizacionApiClient";

export const DIGITALIZACION_METADATA_RESOLVE_ENDPOINT =
  "/api/gestor-documental/digitalizacion/metadata/resolve";

const validateMetadataResolve = (value: unknown): DigitalizacionMetadataResolveResponse => {
  const record = assertRecord(value, "METADATA_RESOLVE_INVALID", "Metadata resuelta invalida.");
  const idTipoListaChequeo = getNumber(record, "idTipoListaChequeo", "IdTipoListaChequeo");
  const idConfiguracionDigitalizacion = getNumber(
    record,
    "idConfiguracionDigitalizacion",
    "IdConfiguracionDigitalizacion",
  );

  if (!idTipoListaChequeo || idTipoListaChequeo <= 0 || !idConfiguracionDigitalizacion) {
    throw createDigitalizacionApiError(
      "METADATA_RESOLVE_REQUIRED_FIELDS",
      "Metadata resolve sin campos obligatorios.",
      "error",
    );
  }

  return {
    idTipoListaChequeo,
    idConfiguracionDigitalizacion,
    obligaListaChequeo: getBoolean(record, "obligaListaChequeo", "ObligaListaChequeo") ?? false,
    esUnico: getBoolean(record, "esUnico", "EsUnico") ?? false,
    unicidadValidada: getBoolean(record, "unicidadValidada", "UnicidadValidada") ?? false,
    trd: normalizeTrd(record.trd ?? record.Trd),
  };
};

export async function resolveDigitalizacionMetadata(
  request: DigitalizacionMetadataResolveRequest,
  options: DigitalizacionApiRequestOptions = {},
) {
  const response = await clienteApi.post(DIGITALIZACION_METADATA_RESOLVE_ENDPOINT, request, {
    ...withSignal(options.signal),
  });

  return unwrapAppResponse<DigitalizacionMetadataResolveResponse>(
    response.data,
    validateMetadataResolve,
    "metadata-resolve",
  );
}
