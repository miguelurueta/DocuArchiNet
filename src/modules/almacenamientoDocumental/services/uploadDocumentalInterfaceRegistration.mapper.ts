import type {
  AlmacenarDocumentoStoredResult,
  AppUploadDocumentalModoDocumento,
  UploadDocumentalContext,
  UploadDocumentalFileMetadata,
  UploadDocumentalInterfaceRegistration,
  UploadDocumentalProcessKey,
} from "../components/AppUploadDocumental/AppUploadDocumental.types";
import { isRecord } from "../utils/storageFile.utils";

export type BuildInterfaceRegistrationInput = {
  stored: AlmacenarDocumentoStoredResult;
  rawBackendResult?: unknown;
  context: UploadDocumentalContext;
  metadata: UploadDocumentalFileMetadata;
  proceso: UploadDocumentalProcessKey;
  modoDocumento?: AppUploadDocumentalModoDocumento;
};

export function buildUploadDocumentalInterfaceRegistration(
  input: BuildInterfaceRegistrationInput,
): UploadDocumentalInterfaceRegistration[] {
  const events: UploadDocumentalInterfaceRegistration[] = [];
  const raw = isRecord(input.rawBackendResult) ? input.rawBackendResult : undefined;

  const mappedFromRaw = mapKnownRawEvents(raw);
  events.push(...mappedFromRaw);

  if (events.length === 0) {
    const contextual = buildContextualEvent(input);
    if (contextual) {
      events.push(contextual);
    }
  }

  if (events.length === 0 && input.rawBackendResult !== undefined) {
    events.push({ kind: "raw", raw: input.rawBackendResult });
  }

  return events;
}

function buildContextualEvent(
  input: BuildInterfaceRegistrationInput,
): UploadDocumentalInterfaceRegistration | null {
  if (input.modoDocumento === "relacionado-radicado") {
    return {
      kind: "related-document-row",
      nombreGabinete: input.context.nombreGabinete,
      idImagen: input.context.idImagen,
      tipoDocumental: input.metadata.nombreTipoDocumento,
      nombreTipoDocumental: input.metadata.nombreTipoDocumento,
    };
  }

  if (input.context.idTareaWorkflow || input.modoDocumento === "documento-libre-respuesta") {
    return {
      kind: "workflow-document-row",
      nombreGabinete: input.context.nombreGabinete,
      idImagen: input.context.idImagen,
      idTareaWorkflow: input.context.idTareaWorkflow,
      tipoDocumental: input.metadata.nombreTipoDocumento,
      nombreTipoDocumental: input.metadata.nombreTipoDocumento,
    };
  }

  return {
    kind: "production-document-row",
    idRegistro: input.stored.idRegistroProduccionDocumental,
    idImagen: input.context.idImagen,
    nombreArchivo: input.stored.nombreArchivoFinal,
    fecha: input.metadata.fechaCarga,
    tipoDocumental: input.metadata.nombreTipoDocumento,
    nombreGabinete: input.context.nombreGabinete,
  };
}

function mapKnownRawEvents(raw: Record<string, unknown> | undefined): UploadDocumentalInterfaceRegistration[] {
  if (!raw) {
    return [];
  }

  const events: UploadDocumentalInterfaceRegistration[] = [];
  const migrationUrl = getString(raw, "url", "Url", "urlPreview", "UrlPreview");
  if (migrationUrl) {
    events.push({
      kind: "migration-preview",
      url: migrationUrl,
      idRegistro: getNumber(raw, "idRegistro", "IdRegistro", "idRegistroProduccionDocumental"),
    });
  }

  const contadorPaginas = getNumber(raw, "contadorPaginas", "ContadorPaginas", "numeroPaginas", "NumeroPaginas");
  if (contadorPaginas !== undefined) {
    events.push({ kind: "page-counter", contadorPaginas });
  }

  const semaforoUrl = getString(raw, "urlImagenSemaforo", "UrlImagenSemaforo");
  if (semaforoUrl) {
    events.push({ kind: "traffic-light", urlImagenSemaforo: semaforoUrl });
  }

  const dropdownText = getString(raw, "dropdownText", "text", "Text");
  const dropdownValue = getPrimitive(raw, "dropdownValue", "value", "Value");
  if (dropdownText && dropdownValue !== undefined) {
    events.push({
      kind: "dropdown-option",
      text: dropdownText,
      value: dropdownValue,
      target: getDropdownTarget(raw),
    });
  }

  const versionId = getNumber(raw, "idVersionDocumento", "IdVersionDocumento");
  if (versionId !== undefined) {
    events.push({
      kind: "document-version-row",
      idImagen: getNumber(raw, "idImagen", "IdImagen"),
      idVersionDocumento: versionId,
      idRegistroVersion: getNumber(raw, "idRegistroVersion", "IdRegistroVersion"),
      tipoDocumento: getString(raw, "tipoDocumento", "TipoDocumento"),
      estadoFirmaDigital: getString(raw, "estadoFirmaDigital", "EstadoFirmaDigital"),
      iconName: getString(raw, "iconName", "IconName"),
      dbt: getNumber(raw, "dbt", "Dbt"),
      fechaRegistroVersion: getString(raw, "fechaRegistroVersion", "FechaRegistroVersion"),
    });
  }

  const rowTable = getUnknown(raw, "rowTable", "RowTable");
  const fieldTable = getUnknown(raw, "fieldTable", "FieldTable");
  const source = getString(raw, "source", "Source");
  if (rowTable !== undefined && fieldTable !== undefined && (source === "rue-sii" || source === "virtual-sii")) {
    events.push({ kind: "table-import-result", rowTable, fieldTable, source });
  }

  return events;
}

function getUnknown(record: Record<string, unknown>, ...keys: string[]): unknown {
  for (const key of keys) {
    if (key in record) {
      return record[key];
    }
  }

  return undefined;
}

function getString(record: Record<string, unknown>, ...keys: string[]): string | undefined {
  const value = getUnknown(record, ...keys);
  return typeof value === "string" && value.trim() ? value : undefined;
}

function getNumber(record: Record<string, unknown>, ...keys: string[]): number | undefined {
  const value = getUnknown(record, ...keys);
  return typeof value === "number" && Number.isFinite(value) ? value : undefined;
}

function getPrimitive(record: Record<string, unknown>, ...keys: string[]): string | number | undefined {
  const value = getUnknown(record, ...keys);
  return typeof value === "string" || typeof value === "number" ? value : undefined;
}

function getDropdownTarget(record: Record<string, unknown>): "respuesta" | "pqrs" | "anexo" | undefined {
  const value = getString(record, "target", "Target");
  return value === "respuesta" || value === "pqrs" || value === "anexo" ? value : undefined;
}
