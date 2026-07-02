import type {
  AlmacenarDocumentoRequest,
  WorkflowAnexoStorageResult,
} from "../../almacenamientoDocumental/types/almacenamientoDocumental.types";
import type {
  BuildAlmacenarDocumentoRequestInput,
  UploadDocumentalContext,
  UploadDocumentalFileMetadata,
} from "../../almacenamientoDocumental/components/AppUploadDocumental/AppUploadDocumental.types";
import { isRecord, normalizeFileExtension } from "../../almacenamientoDocumental/utils/storageFile.utils";

const PROVIDER_KEY = "RADICACION";
const MODO_RESOLUCION = "RespuestaRadicado";
const TIPO_ADJUNTO_RESPUESTA = "respuesta";

export class GestionRespuestaUploadDocumentalMapperError extends Error {
  public constructor(message: string) {
    super(message);
    this.name = "GestionRespuestaUploadDocumentalMapperError";
  }
}

export function buildGestionRespuestaAlmacenarDocumentoRequest(
  input: BuildAlmacenarDocumentoRequestInput,
): Omit<AlmacenarDocumentoRequest, "rutaTemporalId" | "documentos"> & {
  documento?: Partial<AlmacenarDocumentoRequest["documentos"][number]>;
} {
  const nombreGabinete = requireNonEmpty(input.context.nombreGabinete, "nombreGabinete");
  const idRespuestaRadicado = normalizePositiveNumber(input.context.idRespuesta, "idRespuestaRadicado");
  const fileName = normalizeSafeFileName(input.fileName);
  const tipologia = buildTipologiaOrNull(input.metadata);
  const numeroPaginas = normalizeOptionalPositiveNumber(input.metadata.numeroPaginas);
  const radicado = requireNonEmpty(input.context.nameModulo, "radicado");
  const idUsuarioGestion = normalizePositiveNumber(input.context.idUsuarioGestion, "idUsuarioGestion");
  const idEmpresa = normalizePositiveNumber(input.context.idEmpresa, "idEmpresa");
  const fechaElaboracion = requireDateOnly(input.context.fechaElaboracion, "fechaElaboracion");

  return {
    nombreGabinete,
    nombreDocumento: buildNombreDocumento(input.context, fileName),
    requestId: requireNonEmpty(input.requestId, "requestId"),
    camposIndexacion: buildCamposIndexacion(input),
    inventario: {
      IdUsuarioGestion: idUsuarioGestion,
      IdEmpresa: idEmpresa,
      Radicado: radicado,
      FechaElaboracion: fechaElaboracion,
    },
    trd: tipologia,
    expediente:
      input.context.idExpediente || input.context.idTipoExpediente
        ? {
            idExpediente: input.context.idExpediente ?? null,
            idTipoExpediente: input.context.idTipoExpediente ?? null,
          }
        : null,
    workflow:
      input.context.idTareaWorkflow || input.context.idRutaWorkflow
        ? {
            idTareaWorkflow: input.context.idTareaWorkflow ?? null,
            idRutaWorkflow: input.context.idRutaWorkflow ?? null,
          }
        : null,
    cabinetIndexSeed: {
      sourceModule: PROVIDER_KEY,
      providerKey: PROVIDER_KEY,
      version: "1.0.0",
      payload: {
        modoResolucion: MODO_RESOLUCION,
      },
    },
    anexoRespuesta: {
      idRespuestaRadicado,
      nombreArchivo: fileName,
      tipoAdjunto: TIPO_ADJUNTO_RESPUESTA,
      observacion: "Anexo cargado desde workflow",
    },
    numeroPaginasDeclaradas: numeroPaginas,
    documento: {
      idDocumento: buildDocumentoEntradaId(input.requestId),
      nombreOriginal: fileName,
      extension: normalizeFileExtension(fileName),
      numeroPaginas,
    },
  };
}

export function normalizeWorkflowAnexoStorageResult(rawBackendResult: unknown): WorkflowAnexoStorageResult {
  const envelopeData = isRecord(rawBackendResult) && isRecord(rawBackendResult.data) ? rawBackendResult.data : rawBackendResult;
  const data = requireRecord(envelopeData, "workflow anexo response");
  const documento = requireRecord(readUnknown(data, "documento", "Documento"), "Documento");
  const anexoRespuesta = requireRecord(readUnknown(data, "anexoRespuesta", "AnexoRespuesta"), "AnexoRespuesta");
  const indice = readUnknown(data, "indice", "Indice");
  const workflow = readUnknown(data, "workflow", "Workflow");

  const result: WorkflowAnexoStorageResult = {
    documento: {
      idAlmacen: requirePositiveNumber(documento, "idAlmacen", "IdAlmacen"),
      idRegistroProduccionDocumental: requirePositiveNumber(
        documento,
        "idRegistroProduccionDocumental",
        "IdRegistroProduccionDocumental",
      ),
      nombreArchivoFinal: requireNonEmpty(readString(documento, "nombreArchivoFinal", "NombreArchivoFinal"), "NombreArchivoFinal"),
    },
    anexoRespuesta: {
      idAnexoRespuesta: readNullableNumber(anexoRespuesta, "idAnexoRespuesta", "IdAnexoRespuesta"),
      idRespuestaRadicado: requirePositiveNumber(anexoRespuesta, "idRespuestaRadicado", "IdRespuestaRadicado"),
      idAlmacen: requirePositiveNumber(anexoRespuesta, "idAlmacen", "IdAlmacen"),
      nombreGabinete: requireNonEmpty(readString(anexoRespuesta, "nombreGabinete", "NombreGabinete"), "NombreGabinete"),
      nombreArchivo: requireNonEmpty(readString(anexoRespuesta, "nombreArchivo", "NombreArchivo"), "NombreArchivo"),
      created: requireTrue(anexoRespuesta, "created", "Created"),
    },
    indice: isRecord(indice)
      ? {
          providerKey: readNullableString(indice, "providerKey", "ProviderKey"),
          resolved: readNullableBoolean(indice, "resolved", "Resolved"),
          sourceTrace: readNullableString(indice, "sourceTrace", "SourceTrace"),
        }
      : null,
    workflow: isRecord(workflow)
      ? {
          logInserted: readNullableBoolean(workflow, "logInserted", "LogInserted"),
          idTareaWorkflow: readNullableNumber(workflow, "idTareaWorkflow", "IdTareaWorkflow"),
          idRutaWorkflow: readNullableNumber(workflow, "idRutaWorkflow", "IdRutaWorkflow"),
        }
      : null,
  };

  return result;
}

export function isWorkflowAnexoCreated(rawBackendResult: unknown): boolean {
  try {
    return normalizeWorkflowAnexoStorageResult(rawBackendResult).anexoRespuesta.created;
  } catch {
    return false;
  }
}

function buildCamposIndexacion(input: BuildAlmacenarDocumentoRequestInput): AlmacenarDocumentoRequest["camposIndexacion"] {
  const campos = [
    ...(input.context.camposIndexacion?.map((field) => ({
      nombreCampo: field.nombreCampo,
      valor: field.valor ?? null,
      esObligatorio: field.esObligatorio ?? null,
    })) ?? []),
    ...(input.metadata.fechaCarga
      ? [
          {
            nombreCampo: "fechaCarga",
            valor: input.metadata.fechaCarga,
            esObligatorio: true,
          },
        ]
      : []),
  ];

  return campos.length > 0 ? campos : null;
}

function buildNombreDocumento(context: UploadDocumentalContext, fileName: string): string {
  const suffix = context.nameModulo?.trim() || context.idTareaWorkflow || context.idRespuesta || fileName;
  return `Anexo workflow respuesta ${suffix}`;
}

function buildDocumentoEntradaId(requestId: string): string {
  const normalized = requestId.replace(/[^a-zA-Z0-9_-]/g, "-").slice(0, 80);
  return `wf-anexo-${normalized || Date.now()}`;
}

function buildTipologiaOrNull(metadata: UploadDocumentalFileMetadata): AlmacenarDocumentoRequest["trd"] {
  if (!metadata.idTipoDocumento && !metadata.nombreTipoDocumento) {
    return null;
  }

  const idTipoDocumento = normalizePositiveNumber(metadata.idTipoDocumento, "idTipoDocumento");
  const nombreTipoDocumento = requireNonEmpty(metadata.nombreTipoDocumento, "nombreTipoDocumento");

  return {
    idTipoDocumento,
    nombreTipoDocumento,
  };
}

function normalizeSafeFileName(value: string): string {
  const normalized = requireNonEmpty(value, "fileName").replaceAll("\\", "/").split("/").pop()?.trim() ?? "";
  if (!normalized) {
    throw new GestionRespuestaUploadDocumentalMapperError("fileName must not be a local path");
  }

  return normalized;
}

function normalizePositiveNumber(value: unknown, fieldName: string): number {
  const numeric = typeof value === "string" ? Number(value) : value;
  if (typeof numeric !== "number" || !Number.isFinite(numeric) || numeric <= 0) {
    throw new GestionRespuestaUploadDocumentalMapperError(`${fieldName} must be a positive number`);
  }

  return numeric;
}

function normalizeOptionalPositiveNumber(value: unknown): number | null {
  if (value === undefined || value === null) {
    return null;
  }

  return normalizePositiveNumber(value, "numeroPaginas");
}

function requireNonEmpty(value: unknown, fieldName: string): string {
  if (typeof value !== "string" || value.trim().length === 0) {
    throw new GestionRespuestaUploadDocumentalMapperError(`${fieldName} is required`);
  }

  return value.trim();
}

function requireDateOnly(value: unknown, fieldName: string): string {
  const normalized = requireNonEmpty(value, fieldName);
  if (!/^\d{4}-\d{2}-\d{2}$/.test(normalized)) {
    throw new GestionRespuestaUploadDocumentalMapperError(`${fieldName} must use yyyy-MM-dd format`);
  }

  const date = new Date(`${normalized}T00:00:00.000Z`);
  if (Number.isNaN(date.getTime()) || date.toISOString().slice(0, 10) !== normalized) {
    throw new GestionRespuestaUploadDocumentalMapperError(`${fieldName} must be a valid date`);
  }

  return normalized;
}

function requireRecord(value: unknown, fieldName: string): Record<string, unknown> {
  if (!isRecord(value)) {
    throw new GestionRespuestaUploadDocumentalMapperError(`${fieldName} must be an object`);
  }

  return value;
}

function readUnknown(record: Record<string, unknown>, ...keys: string[]): unknown {
  for (const key of keys) {
    if (key in record) {
      return record[key];
    }
  }

  return undefined;
}

function readString(record: Record<string, unknown>, ...keys: string[]): string | undefined {
  const value = readUnknown(record, ...keys);
  return typeof value === "string" && value.trim() ? value.trim() : undefined;
}

function readNullableString(record: Record<string, unknown>, ...keys: string[]): string | null | undefined {
  const value = readUnknown(record, ...keys);
  if (value === null) return null;
  return typeof value === "string" ? value : undefined;
}

function readNullableNumber(record: Record<string, unknown>, ...keys: string[]): number | null | undefined {
  const value = readUnknown(record, ...keys);
  if (value === null) return null;
  return typeof value === "number" && Number.isFinite(value) ? value : undefined;
}

function readNullableBoolean(record: Record<string, unknown>, ...keys: string[]): boolean | null | undefined {
  const value = readUnknown(record, ...keys);
  if (value === null) return null;
  return typeof value === "boolean" ? value : undefined;
}

function requirePositiveNumber(record: Record<string, unknown>, ...keys: string[]): number {
  const value = readNullableNumber(record, ...keys);
  if (typeof value !== "number" || value <= 0) {
    throw new GestionRespuestaUploadDocumentalMapperError(`${keys.join("/")} must be a positive number`);
  }

  return value;
}

function requireTrue(record: Record<string, unknown>, ...keys: string[]): true {
  const value = readNullableBoolean(record, ...keys);
  if (value !== true) {
    throw new GestionRespuestaUploadDocumentalMapperError(`${keys.join("/")} must be true`);
  }

  return true;
}
