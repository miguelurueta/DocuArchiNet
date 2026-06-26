import clienteApi from "../../../api/Clienteaxios";
import type {
  AlmacenarDocumentoRequest,
  AlmacenarDocumentoResponse,
  AlmacenamientoDocumentalUploadErrorCode,
  DocumentoEntrada,
  StorageUploadCancelResponse,
  StorageUploadCompleteResponse,
  StorageUploadInitRequest,
  StorageUploadInitResponse,
  StorageUploadStatusResponse,
  UploadOneDocumentInput,
  UploadOneDocumentResult,
  UploadStoragePhase,
  UploadStorageProgress,
} from "../types/almacenamientoDocumental.types";
import { AlmacenamientoDocumentalUploadError } from "../types/almacenamientoDocumental.types";
import {
  DEFAULT_STORAGE_CHUNK_SIZE_BYTES,
  DEFAULT_STORAGE_CONTENT_TYPE,
  calculateTotalChunks,
  clampPercent,
  getBooleanField,
  getNumberField,
  getStringField,
  isRecord,
  normalizeFileExtension,
  sliceFileChunk,
} from "../utils/storageFile.utils";

const STORAGE_BASE_ENDPOINT = "/api/gestor-documental/almacenamiento";
const TEMPORARY_UPLOAD_ENDPOINT = `${STORAGE_BASE_ENDPOINT}/upload-temporal`;

type HttpConfig = {
  signal?: AbortSignal;
  headers?: Record<string, string | number>;
};

type EnvelopeResult<T> = {
  data: T;
  rawBackendResult?: unknown;
  requestId?: string;
};

type PhaseContext = UploadStoragePhase | "status" | "cancel";

export const ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS = {
  init: `${TEMPORARY_UPLOAD_ENDPOINT}/init`,
  chunk: (rutaTemporalId: string, archivoTemporalId: string, chunkIndex: number) =>
    `${TEMPORARY_UPLOAD_ENDPOINT}/${encodeSegment(rutaTemporalId)}/${encodeSegment(
      archivoTemporalId,
    )}/chunk/${chunkIndex}`,
  status: (rutaTemporalId: string, archivoTemporalId: string) =>
    `${TEMPORARY_UPLOAD_ENDPOINT}/${encodeSegment(rutaTemporalId)}/${encodeSegment(
      archivoTemporalId,
    )}/status`,
  complete: (rutaTemporalId: string, archivoTemporalId: string) =>
    `${TEMPORARY_UPLOAD_ENDPOINT}/${encodeSegment(rutaTemporalId)}/${encodeSegment(
      archivoTemporalId,
    )}/complete`,
  cancel: (rutaTemporalId: string, archivoTemporalId: string) =>
    `${TEMPORARY_UPLOAD_ENDPOINT}/${encodeSegment(rutaTemporalId)}/${encodeSegment(archivoTemporalId)}`,
  almacenar: STORAGE_BASE_ENDPOINT,
} as const;

export async function initTemporaryUpload(
  request: StorageUploadInitRequest,
  signal?: AbortSignal,
): Promise<StorageUploadInitResponse> {
  try {
    ensureNotAborted(signal);
    validateInitRequest(request);

    const response = await clienteApi.post(ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.init, request, withSignal(signal));
    return unwrapStorageResponse(response.data, validateStorageUploadInitResponse, {
      code: "storage_init_error",
      phase: "initializing",
      operation: "initTemporaryUpload",
    }).data;
  } catch (error) {
    throw toStorageError(error, "storage_init_error", "initializing");
  }
}

export async function uploadTemporaryChunk(input: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  chunkIndex: number;
  totalChunks: number;
  chunk: Blob;
  signal?: AbortSignal;
}): Promise<void> {
  try {
    ensureNotAborted(input.signal);
    validateTemporaryIds(input.rutaTemporalId, input.archivoTemporalId);
    validateChunkInput(input.chunkIndex, input.totalChunks, input.chunk);

    const response = await clienteApi.put(
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.chunk(
        input.rutaTemporalId,
        input.archivoTemporalId,
        input.chunkIndex,
      ),
      input.chunk,
      {
        signal: input.signal,
        headers: {
          "Content-Type": DEFAULT_STORAGE_CONTENT_TYPE,
          "X-Total-Chunks": input.totalChunks,
        },
      },
    );

    validateEmptyOrSuccessfulResponse(response.data, "storage_chunk_error", "uploading");
  } catch (error) {
    throw toStorageError(error, "storage_chunk_error", "uploading");
  }
}

export async function getTemporaryUploadStatus(input: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  signal?: AbortSignal;
}): Promise<StorageUploadStatusResponse> {
  try {
    ensureNotAborted(input.signal);
    validateTemporaryIds(input.rutaTemporalId, input.archivoTemporalId);

    const response = await clienteApi.get(
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.status(input.rutaTemporalId, input.archivoTemporalId),
      withSignal(input.signal),
    );

    return unwrapStorageResponse(response.data, validateStorageUploadStatusResponse, {
      code: "storage_status_error",
      phase: "status",
      operation: "getTemporaryUploadStatus",
    }).data;
  } catch (error) {
    throw toStorageError(error, "storage_status_error", "status");
  }
}

export async function completeTemporaryUpload(input: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  signal?: AbortSignal;
}): Promise<void> {
  try {
    ensureNotAborted(input.signal);
    validateTemporaryIds(input.rutaTemporalId, input.archivoTemporalId);

    const response = await clienteApi.post(
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.complete(input.rutaTemporalId, input.archivoTemporalId),
      undefined,
      withSignal(input.signal),
    );

    validateEmptyOrSuccessfulResponse(response.data, "storage_complete_error", "completing");
  } catch (error) {
    throw toStorageError(error, "storage_complete_error", "completing");
  }
}

export async function cancelTemporaryUpload(input: {
  rutaTemporalId: string;
  archivoTemporalId: string;
  signal?: AbortSignal;
}): Promise<void> {
  try {
    validateTemporaryIds(input.rutaTemporalId, input.archivoTemporalId);

    const response = await clienteApi.delete(
      ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.cancel(input.rutaTemporalId, input.archivoTemporalId),
      withSignal(input.signal),
    );

    validateEmptyOrSuccessfulResponse(response.data, "storage_cancel_error", "cancel");
  } catch (error) {
    throw toStorageError(error, "storage_cancel_error", "cancel");
  }
}

export async function almacenarDocumento(
  request: AlmacenarDocumentoRequest,
  signal?: AbortSignal,
): Promise<AlmacenarDocumentoResponse> {
  return (await almacenarDocumentoInternal(request, signal)).response;
}

export async function uploadAndStoreOneDocument(input: UploadOneDocumentInput): Promise<UploadOneDocumentResult> {
  let temporal: StorageUploadInitResponse | undefined;

  try {
    validateUploadOneDocumentInput(input);
    ensureNotAborted(input.signal);

    const initialChunkSizeBytes = input.initialChunkSizeBytes ?? DEFAULT_STORAGE_CHUNK_SIZE_BYTES;
    const initialTotalChunks = calculateTotalChunks(input.file.size, initialChunkSizeBytes);
    emitProgress(input, { phase: "initializing", percent: 0, totalBytes: input.file.size });

    temporal = await initTemporaryUpload(
      {
        nombreOriginal: input.file.name,
        tamanoBytes: input.file.size,
        extension: normalizeFileExtension(input.file.name),
        hashSha256Esperado: null,
        numeroChunks: initialTotalChunks,
      },
      input.signal,
    );

    const backendChunkSizeBytes = temporal.chunkSizeBytes;
    const totalChunks = calculateTotalChunks(input.file.size, backendChunkSizeBytes);

    for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex += 1) {
      ensureNotAborted(input.signal);

      const chunk = sliceFileChunk(input.file, chunkIndex, backendChunkSizeBytes);
      await uploadTemporaryChunk({
        rutaTemporalId: temporal.rutaTemporalId,
        archivoTemporalId: temporal.archivoTemporalId,
        chunkIndex,
        totalChunks,
        chunk,
        signal: input.signal,
      });

      emitProgress(input, {
        phase: "uploading",
        chunkIndex,
        totalChunks,
        loadedBytes: Math.min((chunkIndex + 1) * backendChunkSizeBytes, input.file.size),
        totalBytes: input.file.size,
        percent: ((chunkIndex + 1) / totalChunks) * 100,
      });
    }

    ensureNotAborted(input.signal);
    emitProgress(input, { phase: "completing", percent: 0, totalBytes: input.file.size });
    await completeTemporaryUpload({
      rutaTemporalId: temporal.rutaTemporalId,
      archivoTemporalId: temporal.archivoTemporalId,
      signal: input.signal,
    });
    emitProgress(input, { phase: "completing", percent: 100, totalBytes: input.file.size });

    ensureNotAborted(input.signal);
    emitProgress(input, { phase: "storing", percent: 0, totalBytes: input.file.size });
    const storeResult = await almacenarDocumentoInternal(buildStoreRequest(input, temporal), input.signal);
    emitProgress(input, { phase: "storing", percent: 100, totalBytes: input.file.size });

    return {
      temporal,
      response: storeResult.response,
      rawBackendResult: storeResult.rawBackendResult,
    };
  } catch (error) {
    const storageError = toStorageError(error, "storage_store_error", "storing");

    if (temporal && storageError.code === "storage_aborted") {
      try {
        await cancelTemporaryUpload({
          rutaTemporalId: temporal.rutaTemporalId,
          archivoTemporalId: temporal.archivoTemporalId,
        });
      } catch (cancelError) {
        throw new AlmacenamientoDocumentalUploadError({
          code: "storage_aborted",
          phase: storageError.phase,
          message: storageError.message,
          cause: storageError,
          details: {
            cancelWarning: cancelError,
            rutaTemporalId: temporal.rutaTemporalId,
            archivoTemporalId: temporal.archivoTemporalId,
          },
        });
      }
    }

    throw storageError;
  }
}

async function almacenarDocumentoInternal(
  request: AlmacenarDocumentoRequest,
  signal?: AbortSignal,
): Promise<{ response: AlmacenarDocumentoResponse; rawBackendResult?: unknown }> {
  try {
    ensureNotAborted(signal);
    validateStoreRequest(request);

    const response = await clienteApi.post(ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.almacenar, request, withSignal(signal));
    const result = unwrapStorageResponse(response.data, validateAlmacenarDocumentoResponse, {
      code: "storage_store_error",
      phase: "storing",
      operation: "almacenarDocumento",
    });

    return {
      response: result.data,
      rawBackendResult: result.rawBackendResult,
    };
  } catch (error) {
    throw toStorageError(error, "storage_store_error", "storing");
  }
}

export function unwrapStorageResponse<T>(
  payload: unknown,
  validateData: (data: unknown) => T,
  context: {
    code: AlmacenamientoDocumentalUploadErrorCode;
    phase?: PhaseContext;
    operation: string;
  },
): EnvelopeResult<T> {
  if (!isRecord(payload)) {
    throw contractError(context, "Backend response must be an object", payload);
  }

  const success = readBoolean(payload, "success", "Success");
  const data = readUnknown(payload, "data", "Data");
  const requestId = readRequestId(payload);
  const hasEnvelope = success !== undefined || data !== undefined;

  if (success === false) {
    throw new AlmacenamientoDocumentalUploadError({
      code: context.code,
      phase: context.phase,
      message: readMessage(payload) ?? `${context.operation} failed`,
      requestId,
      details: readUnknown(payload, "errors", "Errors") ?? payload,
    });
  }

  if (!hasEnvelope) {
    return {
      data: validateData(payload),
      rawBackendResult: payload,
      requestId,
    };
  }

  if (data === undefined) {
    throw contractError(context, "Backend envelope data is missing", payload);
  }

  return {
    data: validateData(data),
    rawBackendResult: data,
    requestId,
  };
}

function validateStorageUploadInitResponse(data: unknown): StorageUploadInitResponse {
  const record = requireRecord(data, "StorageUploadInitResponse");
  return {
    rutaTemporalId: requireNonEmptyStringField(record, "rutaTemporalId", "RutaTemporalId"),
    archivoTemporalId: requireNonEmptyStringField(record, "archivoTemporalId", "ArchivoTemporalId"),
    chunkSizeBytes: requirePositiveNumberField(record, "chunkSizeBytes", "ChunkSizeBytes"),
    estado: requireNonEmptyStringField(record, "estado", "Estado"),
  };
}

function validateStorageUploadStatusResponse(data: unknown): StorageUploadStatusResponse {
  const record = requireRecord(data, "StorageUploadStatusResponse");
  return {
    rutaTemporalId: requireNonEmptyStringField(record, "rutaTemporalId", "RutaTemporalId"),
    archivoTemporalId: requireNonEmptyStringField(record, "archivoTemporalId", "ArchivoTemporalId"),
    estado: requireNonEmptyStringField(record, "estado", "Estado"),
    chunksRecibidos: getNullableNumber(record, "chunksRecibidos", "ChunksRecibidos"),
    totalChunks: getNullableNumber(record, "totalChunks", "TotalChunks"),
    completado: getNullableBoolean(record, "completado", "Completado"),
  };
}

function validateAlmacenarDocumentoResponse(data: unknown): AlmacenarDocumentoResponse {
  const record = requireRecord(data, "AlmacenarDocumentoResponse");
  return {
    idAlmacen: requirePositiveNumberField(record, "idAlmacen", "IdAlmacen"),
    idRegistroProduccionDocumental: requirePositiveNumberField(
      record,
      "idRegistroProduccionDocumental",
      "IdRegistroProduccionDocumental",
    ),
    nombreArchivoFinal: requireNonEmptyStringField(record, "nombreArchivoFinal", "NombreArchivoFinal"),
    requestId: requireNonEmptyStringField(record, "requestId", "RequestId"),
  };
}

function validateEmptyOrSuccessfulResponse(
  payload: unknown,
  code: AlmacenamientoDocumentalUploadErrorCode,
  phase: PhaseContext,
): void {
  if (payload === undefined || payload === null || payload === "") {
    return;
  }

  unwrapStorageResponse<StorageUploadCompleteResponse | StorageUploadCancelResponse>(
    payload,
    (data) => {
      if (!isRecord(data)) {
        return {};
      }

      return data;
    },
    { code, phase, operation: phase },
  );
}

function buildStoreRequest(input: UploadOneDocumentInput, temporal: StorageUploadInitResponse): AlmacenarDocumentoRequest {
  const { documento, ...storeRequest } = input.request;
  const documentoEntrada: DocumentoEntrada = {
    archivoTemporalId: temporal.archivoTemporalId,
    nombreOriginal: input.file.name,
    extension: normalizeFileExtension(input.file.name),
    ...documento,
  };

  return {
    ...storeRequest,
    rutaTemporalId: temporal.rutaTemporalId,
    documentos: [documentoEntrada],
  };
}

function emitProgress(
  input: UploadOneDocumentInput,
  progress: Omit<UploadStorageProgress, "fileUid" | "percent"> & { percent: number },
): void {
  input.onProgress?.({
    ...progress,
    fileUid: input.fileUid,
    percent: clampPercent(progress.percent),
  });
}

function withSignal(signal?: AbortSignal): HttpConfig | undefined {
  return signal ? { signal } : undefined;
}

function encodeSegment(value: string): string {
  return encodeURIComponent(value);
}

function ensureNotAborted(signal?: AbortSignal): void {
  if (signal?.aborted) {
    throw new AlmacenamientoDocumentalUploadError({
      code: "storage_aborted",
      message: "Storage upload was aborted",
    });
  }
}

function toStorageError(
  error: unknown,
  fallbackCode: AlmacenamientoDocumentalUploadErrorCode,
  phase: PhaseContext,
): AlmacenamientoDocumentalUploadError {
  if (error instanceof AlmacenamientoDocumentalUploadError) {
    return error;
  }

  if (isAbortError(error)) {
    return new AlmacenamientoDocumentalUploadError({
      code: "storage_aborted",
      phase,
      message: "Storage upload was aborted",
      cause: error,
    });
  }

  return new AlmacenamientoDocumentalUploadError({
    code: fallbackCode,
    phase,
    message: error instanceof Error ? error.message : "Storage upload request failed",
    cause: error,
  });
}

function isAbortError(error: unknown): boolean {
  if (error instanceof DOMException && error.name === "AbortError") {
    return true;
  }

  if (!isRecord(error)) {
    return false;
  }

  return (
    error.name === "AbortError" ||
    error.name === "CanceledError" ||
    error.code === "ERR_CANCELED" ||
    error.message === "canceled"
  );
}

function validateInitRequest(request: StorageUploadInitRequest): void {
  if (!isRecord(request)) {
    throw new TypeError("StorageUploadInitRequest must be an object");
  }

  requireNonEmptyStringField(request, "nombreOriginal");
  requirePositiveNumberField(request, "tamanoBytes");
  if (typeof request.extension !== "string") {
    throw new TypeError("extension must be a string");
  }
  requirePositiveNumberField(request, "numeroChunks");
}

function validateTemporaryIds(rutaTemporalId: string, archivoTemporalId: string): void {
  if (!rutaTemporalId.trim() || !archivoTemporalId.trim()) {
    throw new TypeError("Temporary upload ids are required");
  }
}

function validateChunkInput(chunkIndex: number, totalChunks: number, chunk: Blob): void {
  if (!Number.isInteger(chunkIndex) || chunkIndex < 0) {
    throw new TypeError("chunkIndex must be a zero-based non-negative integer");
  }

  if (!Number.isInteger(totalChunks) || totalChunks <= 0) {
    throw new TypeError("totalChunks must be a positive integer");
  }

  if (chunkIndex >= totalChunks) {
    throw new RangeError("chunkIndex must be lower than totalChunks");
  }

  if (!(chunk instanceof Blob)) {
    throw new TypeError("chunk must be a Blob");
  }
}

function validateUploadOneDocumentInput(input: UploadOneDocumentInput): void {
  if (!input.fileUid.trim()) {
    throw new TypeError("fileUid is required");
  }

  if (!(input.file instanceof File)) {
    throw new TypeError("file must be a File");
  }

  validateStoreRequest({
    ...input.request,
    rutaTemporalId: "pending",
    documentos: [
      {
        archivoTemporalId: "pending",
        nombreOriginal: input.file.name,
        extension: normalizeFileExtension(input.file.name),
      },
    ],
  });
}

function validateStoreRequest(request: AlmacenarDocumentoRequest): void {
  if (!isRecord(request)) {
    throw new TypeError("AlmacenarDocumentoRequest must be an object");
  }

  requireNonEmptyStringField(request, "nombreGabinete");
  requireNonEmptyStringField(request, "rutaTemporalId");
  requireNonEmptyStringField(request, "nombreDocumento");
  requireNonEmptyStringField(request, "requestId");

  if (!Array.isArray(request.documentos) || request.documentos.length === 0) {
    throw new TypeError("documentos must contain at least one document");
  }
}

function contractError(
  context: {
    code: AlmacenamientoDocumentalUploadErrorCode;
    phase?: PhaseContext;
    operation: string;
  },
  message: string,
  details: unknown,
): AlmacenamientoDocumentalUploadError {
  return new AlmacenamientoDocumentalUploadError({
    code: "storage_contract_error",
    phase: context.phase,
    message: `${context.operation}: ${message}`,
    details,
  });
}

function requireRecord(value: unknown, typeName: string): Record<string, unknown> {
  if (!isRecord(value)) {
    throw new AlmacenamientoDocumentalUploadError({
      code: "storage_contract_error",
      message: `${typeName} must be an object`,
      details: value,
    });
  }

  return value;
}

function requireNonEmptyStringField(record: Record<string, unknown>, ...keys: string[]): string {
  const value = getStringField(record, ...keys);
  if (!value || value.trim().length === 0) {
    throw new AlmacenamientoDocumentalUploadError({
      code: "storage_contract_error",
      message: `${keys.join("/")} must be a non-empty string`,
      details: record,
    });
  }

  return value;
}

function requirePositiveNumberField(record: Record<string, unknown>, ...keys: string[]): number {
  const value = getNumberField(record, ...keys);
  if (value === undefined || value <= 0) {
    throw new AlmacenamientoDocumentalUploadError({
      code: "storage_contract_error",
      message: `${keys.join("/")} must be a positive number`,
      details: record,
    });
  }

  return value;
}

function getNullableNumber(record: Record<string, unknown>, ...keys: string[]): number | null | undefined {
  for (const key of keys) {
    if (record[key] === null) {
      return null;
    }
  }

  return getNumberField(record, ...keys);
}

function getNullableBoolean(record: Record<string, unknown>, ...keys: string[]): boolean | null | undefined {
  for (const key of keys) {
    if (record[key] === null) {
      return null;
    }
  }

  return getBooleanField(record, ...keys);
}

function readBoolean(record: Record<string, unknown>, ...keys: string[]): boolean | undefined {
  return getBooleanField(record, ...keys);
}

function readUnknown(record: Record<string, unknown>, ...keys: string[]): unknown {
  for (const key of keys) {
    if (key in record) {
      return record[key];
    }
  }

  return undefined;
}

function readMessage(record: Record<string, unknown>): string | undefined {
  return getStringField(record, "message", "Message", "title", "Title", "detail", "Detail");
}

function readRequestId(record: Record<string, unknown>): string | undefined {
  const directRequestId = getStringField(record, "requestId", "RequestId");
  if (directRequestId) {
    return directRequestId;
  }

  const data = readUnknown(record, "data", "Data");
  if (isRecord(data)) {
    const dataRequestId = getStringField(data, "requestId", "RequestId");
    if (dataRequestId) {
      return dataRequestId;
    }
  }

  const meta = readUnknown(record, "meta", "Meta");
  if (isRecord(meta)) {
    const metaRequestId = getStringField(meta, "requestId", "RequestId", "correlationId", "CorrelationId");
    if (metaRequestId) {
      return metaRequestId;
    }
  }

  const errors = readUnknown(record, "errors", "Errors");
  if (isRecord(errors)) {
    return getStringField(errors, "requestId", "RequestId", "correlationId", "CorrelationId");
  }

  return undefined;
}
