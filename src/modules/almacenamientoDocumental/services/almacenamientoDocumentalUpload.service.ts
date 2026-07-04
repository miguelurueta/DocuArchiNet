import clienteApi from "../../../api/Clienteaxios";
import type {
  AlmacenarDocumentoRequest,
  AlmacenarDocumentoResponse,
  AlmacenamientoDocumentalUploadErrorCode,
  BackendAlmacenarDocumentoRequest,
  BackendStorageUploadInitRequest,
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
  assertPositiveFiniteNumber,
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
const DEBUG_STORAGE_UPLOAD =
  typeof import.meta !== "undefined" &&
  Boolean(import.meta.env?.DEV) &&
  import.meta.env?.MODE !== "test";
const STORAGE_CHUNK_TRANSIENT_RETRY_DELAYS_MS = [300, 900] as const;

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
  return initTemporaryUploadWithPayload(request, request, signal);
}

async function initTemporaryUploadWithPayload(
  request: StorageUploadInitRequest,
  payload: StorageUploadInitRequest | BackendStorageUploadInitRequest,
  signal?: AbortSignal,
): Promise<StorageUploadInitResponse> {
  try {
    ensureNotAborted(signal);
    validateInitRequest(request);
    debugStorageUpload("init request", {
      nombreOriginal: request.nombreOriginal,
      tamanoBytes: request.tamanoBytes,
      tamanoMb: bytesToMb(request.tamanoBytes),
      extension: request.extension,
      numeroChunks: request.numeroChunks,
    });

    const response = await clienteApi.post(ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.init, payload, withSignal(signal));
    const result = unwrapStorageResponse(response.data, validateStorageUploadInitResponse, {
      code: "storage_init_error",
      phase: "initializing",
      operation: "initTemporaryUpload",
    }).data;

    debugStorageUpload("init response", {
      rutaTemporalId: result.rutaTemporalId,
      archivoTemporalId: result.archivoTemporalId,
      chunkSizeBytes: result.chunkSizeBytes,
      chunkSizeMb: bytesToMb(result.chunkSizeBytes),
      estado: result.estado,
    });

    return result;
  } catch (error) {
    debugStorageUpload("init error", readStorageDebugError(error));
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
  ensureNotAborted(input.signal);
  validateTemporaryIds(input.rutaTemporalId, input.archivoTemporalId);
  validateChunkInput(input.chunkIndex, input.totalChunks, input.chunk);

  for (let attempt = 0; attempt <= STORAGE_CHUNK_TRANSIENT_RETRY_DELAYS_MS.length; attempt += 1) {
    try {
      ensureNotAborted(input.signal);
      debugStorageUpload("chunk request", {
        rutaTemporalId: input.rutaTemporalId,
        archivoTemporalId: input.archivoTemporalId,
        chunkIndex: input.chunkIndex,
        totalChunks: input.totalChunks,
        chunkSizeBytes: input.chunk.size,
        chunkSizeMb: bytesToMb(input.chunk.size),
        attempt: attempt + 1,
      });

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
      debugStorageUpload("chunk response", {
        rutaTemporalId: input.rutaTemporalId,
        archivoTemporalId: input.archivoTemporalId,
        chunkIndex: input.chunkIndex,
        status: "ok",
        attempt: attempt + 1,
      });
      return;
    } catch (error) {
      const retryDelayMs = STORAGE_CHUNK_TRANSIENT_RETRY_DELAYS_MS[attempt];
      const canRetry = retryDelayMs !== undefined && isTransientChunkUploadError(error, input.signal);

      debugStorageUpload(canRetry ? "chunk retry" : "chunk error", {
        ...readStorageDebugError(error),
        rutaTemporalId: input.rutaTemporalId,
        archivoTemporalId: input.archivoTemporalId,
        chunkIndex: input.chunkIndex,
        totalChunks: input.totalChunks,
        chunkSizeBytes: input.chunk instanceof Blob ? input.chunk.size : undefined,
        chunkSizeMb: input.chunk instanceof Blob ? bytesToMb(input.chunk.size) : undefined,
        attempt: attempt + 1,
        retryDelayMs: canRetry ? retryDelayMs : undefined,
      });

      if (!canRetry) {
        throw toStorageError(error, "storage_chunk_error", "uploading");
      }

      await waitForRetry(retryDelayMs, input.signal);
    }
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

    const result = unwrapStorageResponse(response.data, validateStorageUploadStatusResponse, {
      code: "storage_status_error",
      phase: "status",
      operation: "getTemporaryUploadStatus",
    }).data;

    debugStorageUpload("status response", {
      rutaTemporalId: result.rutaTemporalId,
      archivoTemporalId: result.archivoTemporalId,
      estado: result.estado,
      totalChunks: result.totalChunks,
      tamanoRecibidoBytes: result.tamanoRecibidoBytes,
      tamanoRecibidoMb:
        typeof result.tamanoRecibidoBytes === "number" ? bytesToMb(result.tamanoRecibidoBytes) : undefined,
      chunksPendientes: result.chunksPendientes,
      completado: result.completado,
    });

    return result;
  } catch (error) {
    debugStorageUpload("status error", readStorageDebugError(error));
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
    debugStorageUpload("complete response", {
      rutaTemporalId: input.rutaTemporalId,
      archivoTemporalId: input.archivoTemporalId,
      status: "ok",
    });
  } catch (error) {
    debugStorageUpload("complete error", readStorageDebugError(error));
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
    debugStorageUpload("uploadAndStore start", {
      fileUid: input.fileUid,
      fileName: input.file.name,
      fileSizeBytes: input.file.size,
      fileSizeMb: bytesToMb(input.file.size),
      initialChunkSizeBytes,
      initialChunkSizeMb: bytesToMb(initialChunkSizeBytes),
      initialTotalChunks,
      validateStatusBeforeComplete: Boolean(input.validateStatusBeforeComplete),
    });
    const initRequest: StorageUploadInitRequest = {
      nombreOriginal: input.file.name,
      tamanoBytes: input.file.size,
      extension: normalizeFileExtension(input.file.name),
      hashSha256Esperado: null,
      numeroChunks: initialTotalChunks,
    };
    emitProgress(input, { phase: "initializing", percent: 0, totalBytes: input.file.size });

    temporal = await initTemporaryUploadWithPayload(
      initRequest,
      input.backendPayloadCase === "pascal" ? toBackendStorageUploadInitRequest(initRequest) : initRequest,
      input.signal,
    );

    const backendChunkSizeBytes = temporal.chunkSizeBytes;
    const effectiveChunkSizeBytes = resolveEffectiveChunkSizeBytes(backendChunkSizeBytes, input.maxChunkSizeBytes);
    const totalChunks = calculateTotalChunks(input.file.size, effectiveChunkSizeBytes);
    debugStorageUpload("uploadAndStore chunk plan", {
      fileUid: input.fileUid,
      fileName: input.file.name,
      fileSizeBytes: input.file.size,
      fileSizeMb: bytesToMb(input.file.size),
      backendChunkSizeBytes,
      backendChunkSizeMb: bytesToMb(backendChunkSizeBytes),
      maxChunkSizeBytes: input.maxChunkSizeBytes,
      maxChunkSizeMb: input.maxChunkSizeBytes ? bytesToMb(input.maxChunkSizeBytes) : undefined,
      effectiveChunkSizeBytes,
      effectiveChunkSizeMb: bytesToMb(effectiveChunkSizeBytes),
      totalChunks,
    });

    for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex += 1) {
      ensureNotAborted(input.signal);

      const chunk = sliceFileChunk(input.file, chunkIndex, effectiveChunkSizeBytes);
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
        loadedBytes: Math.min((chunkIndex + 1) * effectiveChunkSizeBytes, input.file.size),
        totalBytes: input.file.size,
        percent: ((chunkIndex + 1) / totalChunks) * 100,
      });
    }

    ensureNotAborted(input.signal);
    emitProgress(input, { phase: "completing", percent: 0, totalBytes: input.file.size });
    if (input.validateStatusBeforeComplete) {
      const status = await getTemporaryUploadStatus({
        rutaTemporalId: temporal.rutaTemporalId,
        archivoTemporalId: temporal.archivoTemporalId,
        signal: input.signal,
      });
      validateReadyToComplete(status, input.file.size);
    }

    await completeTemporaryUpload({
      rutaTemporalId: temporal.rutaTemporalId,
      archivoTemporalId: temporal.archivoTemporalId,
      signal: input.signal,
    });
    emitProgress(input, { phase: "completing", percent: 100, totalBytes: input.file.size });

    ensureNotAborted(input.signal);
    emitProgress(input, { phase: "storing", percent: 0, totalBytes: input.file.size });
    const storeRequest = buildStoreRequest(input, temporal);
    const storePayload =
      input.backendPayloadCase === "pascal" ? toBackendAlmacenarDocumentoRequest(storeRequest) : storeRequest;
    debugStorageUpload("store request", {
      requestId: storeRequest.requestId,
      nombreGabinete: storeRequest.nombreGabinete,
      rutaTemporalId: storeRequest.rutaTemporalId,
      documentos: storeRequest.documentos.map((documento) => ({
        archivoTemporalId: documento.archivoTemporalId,
        nombreOriginal: documento.nombreOriginal,
        extension: documento.extension,
      })),
    });
    const storeResult = await almacenarDocumentoInternal(storeRequest, input.signal, storePayload);
    emitProgress(input, { phase: "storing", percent: 100, totalBytes: input.file.size });
    debugStorageUpload("uploadAndStore success", {
      fileUid: input.fileUid,
      fileName: input.file.name,
      idAlmacen: storeResult.response.idAlmacen,
      idRegistroProduccionDocumental: storeResult.response.idRegistroProduccionDocumental,
      requestId: storeResult.response.requestId,
    });

    return {
      temporal,
      response: storeResult.response,
      rawBackendResult: storeResult.rawBackendResult,
    };
  } catch (error) {
    debugStorageUpload("uploadAndStore error", readStorageDebugError(error));
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
  payload: AlmacenarDocumentoRequest | BackendAlmacenarDocumentoRequest = request,
): Promise<{ response: AlmacenarDocumentoResponse; rawBackendResult?: unknown }> {
  try {
    ensureNotAborted(signal);
    validateStoreRequest(request);

    const response = await clienteApi.post(ALMACENAMIENTO_DOCUMENTAL_ENDPOINTS.almacenar, payload, withSignal(signal));
    const result = unwrapStorageResponse(response.data, validateAlmacenarDocumentoResponse, {
      code: "storage_store_error",
      phase: "storing",
      operation: "almacenarDocumento",
    });
    debugStorageUpload("store response", {
      idAlmacen: result.data.idAlmacen,
      idRegistroProduccionDocumental: result.data.idRegistroProduccionDocumental,
      nombreArchivoFinal: result.data.nombreArchivoFinal,
      requestId: result.data.requestId,
    });

    return {
      response: result.data,
      rawBackendResult: result.rawBackendResult,
    };
  } catch (error) {
    debugStorageUpload("store error", readStorageDebugError(error));
    throw toStorageError(error, "storage_store_error", "storing");
  }
}

function resolveEffectiveChunkSizeBytes(backendChunkSizeBytes: number, maxChunkSizeBytes?: number): number {
  if (maxChunkSizeBytes === undefined) {
    return backendChunkSizeBytes;
  }

  return Math.min(backendChunkSizeBytes, assertPositiveFiniteNumber(maxChunkSizeBytes, "maxChunkSizeBytes"));
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
  const meta = readUnknown(payload, "meta", "Meta");
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
    data: validateData(mergeEnvelopeMetaForValidation(data, meta)),
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
    rutaTemporalId: getStringField(record, "rutaTemporalId", "RutaTemporalId") ?? "",
    archivoTemporalId: getStringField(record, "archivoTemporalId", "ArchivoTemporalId") ?? "",
    estado: requireNonEmptyStringField(record, "estado", "Estado"),
    chunksRecibidos: getNullableNumberArrayOrNumber(record, "chunksRecibidos", "ChunksRecibidos"),
    chunksPendientes: getNullableNumberArrayOrNumber(record, "chunksPendientes", "ChunksPendientes"),
    totalChunks: getNullableNumber(record, "totalChunks", "TotalChunks"),
    tamanoRecibidoBytes: getNullableNumber(record, "tamanoRecibidoBytes", "TamanoRecibidoBytes"),
    completado: getNullableBoolean(record, "completado", "Completado"),
  };
}

function validateAlmacenarDocumentoResponse(data: unknown): AlmacenarDocumentoResponse {
  const record = requireRecord(data, "AlmacenarDocumentoResponse");
  const documentRecord = isRecord(record.Documento) ? record.Documento : isRecord(record.documento) ? record.documento : record;
  const metaRecord = isRecord(record.meta) ? record.meta : isRecord(record.Meta) ? record.Meta : undefined;
  const anexoRecord = isRecord(record.AnexoRespuesta)
    ? record.AnexoRespuesta
    : isRecord(record.anexoRespuesta)
      ? record.anexoRespuesta
      : undefined;

  if (anexoRecord) {
    const created = getBooleanField(anexoRecord, "created", "Created");
    if (created !== true) {
      throw new AlmacenamientoDocumentalUploadError({
        code: "storage_contract_error",
        message: "AnexoRespuesta.Created must be true",
        details: record,
      });
    }
  }

  return {
    idAlmacen: requirePositiveNumberField(documentRecord, "idAlmacen", "IdAlmacen"),
    idRegistroProduccionDocumental: requirePositiveNumberField(
      documentRecord,
      "idRegistroProduccionDocumental",
      "IdRegistroProduccionDocumental",
    ),
    nombreArchivoFinal: requireNonEmptyStringField(documentRecord, "nombreArchivoFinal", "NombreArchivoFinal"),
    requestId:
      getStringField(documentRecord, "requestId", "RequestId") ??
      getStringField(record, "requestId", "RequestId") ??
      (metaRecord ? getStringField(metaRecord, "requestId", "RequestId") : undefined) ??
      requireNonEmptyStringField(documentRecord, "requestId", "RequestId"),
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

export function toBackendStorageUploadInitRequest(request: StorageUploadInitRequest): BackendStorageUploadInitRequest {
  return {
    NombreOriginal: request.nombreOriginal,
    TamanoBytes: request.tamanoBytes,
    Extension: request.extension,
    HashSha256Esperado: request.hashSha256Esperado ?? null,
    NumeroChunks: request.numeroChunks,
  };
}

export function toBackendAlmacenarDocumentoRequest(
  request: AlmacenarDocumentoRequest,
): BackendAlmacenarDocumentoRequest {
  const payload: BackendAlmacenarDocumentoRequest = {
    NombreGabinete: request.nombreGabinete,
    RutaTemporalId: request.rutaTemporalId,
    NombreDocumento: request.nombreDocumento,
    RequestId: request.requestId,
    Documentos: request.documentos.map((documento) => ({
      IdDocumento: documento.idDocumento ?? null,
      ArchivoTemporalId: documento.archivoTemporalId,
      NombreOriginal: documento.nombreOriginal,
      Extension: documento.extension,
      NumeroPaginas: documento.numeroPaginas ?? undefined,
    })).map(removeUndefinedFields),
    CamposIndexacion: request.camposIndexacion?.length
      ? request.camposIndexacion.map((campo) => ({
        NombreCampo: campo.nombreCampo,
        Valor: campo.valor ?? null,
        EsObligatorio: campo.esObligatorio ?? null,
        }))
      : undefined,
    Inventario: request.inventario,
    Trd: request.trd
      ? {
          IdTipoDocumento: request.trd.idTipoDocumento ?? null,
          NombreTipoDocumento: request.trd.nombreTipoDocumento ?? null,
        }
      : undefined,
    Expediente: request.expediente
      ? {
          IdExpediente: request.expediente.idExpediente ?? null,
          IdTipoExpediente: request.expediente.idTipoExpediente ?? null,
        }
      : undefined,
    Workflow: request.workflow
      ? {
          IdTareaWorkflow: request.workflow.idTareaWorkflow ?? null,
          IdRutaWorkflow: request.workflow.idRutaWorkflow ?? null,
        }
      : undefined,
    CabinetIndexSeed: request.cabinetIndexSeed
      ? {
          SourceModule: request.cabinetIndexSeed.sourceModule,
          ProviderKey: request.cabinetIndexSeed.providerKey,
          Version: request.cabinetIndexSeed.version,
          Payload: {
            ModoResolucion: request.cabinetIndexSeed.payload.modoResolucion,
            ProveedorExterno: request.cabinetIndexSeed.payload.proveedorExterno ?? null,
            RadicadoExterno: request.cabinetIndexSeed.payload.radicadoExterno ?? null,
            MatriculaSII: request.cabinetIndexSeed.payload.matriculaSII ?? null,
          },
        }
      : undefined,
    AnexoRespuesta: request.anexoRespuesta
      ? {
          IdRespuestaRadicado: request.anexoRespuesta.idRespuestaRadicado,
          NombreArchivo: request.anexoRespuesta.nombreArchivo,
          TipoAdjunto: request.anexoRespuesta.tipoAdjunto,
          Observacion: request.anexoRespuesta.observacion ?? null,
        }
      : undefined,
    FullText: request.fullText ?? undefined,
    NumeroPaginasDeclaradas: request.numeroPaginasDeclaradas ?? undefined,
  };

  return removeUndefinedFields(payload) as BackendAlmacenarDocumentoRequest;
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
    message: readHttpErrorMessage(error) ?? (error instanceof Error ? error.message : "Storage upload request failed"),
    details: readHttpErrorDetails(error),
    cause: error,
  });
}

function readHttpErrorMessage(error: unknown): string | undefined {
  const details = readHttpErrorDetails(error);
  if (!isRecord(details)) {
    return undefined;
  }

  const userMessage = readFirstValidationMessage(details);
  if (userMessage) {
    return userMessage;
  }

  const title = getStringField(details, "title", "Title");
  const message = getStringField(details, "message", "Message");
  return message ?? title;
}

function readHttpErrorDetails(error: unknown): unknown {
  if (!isRecord(error)) {
    return undefined;
  }

  const response = error.response;
  if (!isRecord(response)) {
    return undefined;
  }

  return response.data;
}

function readFirstValidationMessage(details: Record<string, unknown>): string | undefined {
  const errors = readUnknown(details, "errors", "Errors");
  if (!isRecord(errors)) {
    return undefined;
  }

  for (const [field, value] of Object.entries(errors)) {
    if (Array.isArray(value)) {
      const first = value.find((item): item is string => typeof item === "string" && item.trim().length > 0);
      if (first) {
        return `${field}: ${first}`;
      }
    }
    if (typeof value === "string" && value.trim().length > 0) {
      return `${field}: ${value.trim()}`;
    }
  }

  return undefined;
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

  if (input.maxChunkSizeBytes !== undefined) {
    assertPositiveFiniteNumber(input.maxChunkSizeBytes, "maxChunkSizeBytes");
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

function validateReadyToComplete(status: StorageUploadStatusResponse, fileSizeBytes: number): void {
  if (hasPendingChunks(status.chunksPendientes)) {
    throw new AlmacenamientoDocumentalUploadError({
      code: "storage_status_error",
      phase: "status",
      message: "Temporary upload has pending chunks",
      details: status,
    });
  }

  if (
    typeof status.tamanoRecibidoBytes === "number" &&
    Number.isFinite(status.tamanoRecibidoBytes) &&
    status.tamanoRecibidoBytes !== fileSizeBytes
  ) {
    throw new AlmacenamientoDocumentalUploadError({
      code: "storage_status_error",
      phase: "status",
      message: "Temporary upload received size does not match file size",
      details: status,
    });
  }
}

function hasPendingChunks(value: StorageUploadStatusResponse["chunksPendientes"]): boolean {
  if (Array.isArray(value)) {
    return value.length > 0;
  }

  if (typeof value === "number") {
    return value > 0;
  }

  return false;
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

function getNullableNumberArrayOrNumber(
  record: Record<string, unknown>,
  ...keys: string[]
): number[] | number | null | undefined {
  for (const key of keys) {
    const value = record[key];
    if (value === null) {
      return null;
    }
    if (Array.isArray(value)) {
      return value.filter((item): item is number => typeof item === "number" && Number.isFinite(item));
    }
    if (typeof value === "number" && Number.isFinite(value)) {
      return value;
    }
  }

  return undefined;
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

function mergeEnvelopeMetaForValidation(data: unknown, meta: unknown): unknown {
  if (!isRecord(data) || meta === undefined) {
    return data;
  }

  return {
    ...data,
    meta,
    Meta: meta,
  };
}

function removeUndefinedFields<T extends Record<string, unknown>>(value: T): T {
  return Object.fromEntries(Object.entries(value).filter(([, fieldValue]) => fieldValue !== undefined)) as T;
}

function debugStorageUpload(message: string, payload?: Record<string, unknown>): void {
  if (!DEBUG_STORAGE_UPLOAD) {
    return;
  }

  console.info(`[almacenamientoDocumentalUpload][debug] ${message}`, payload ?? {});
}

function bytesToMb(value: number): number {
  return Number((value / 1024 / 1024).toFixed(2));
}

function readStorageDebugError(error: unknown): Record<string, unknown> {
  if (!isRecord(error)) {
    return {
      error,
    };
  }

  const response = isRecord(error.response) ? error.response : undefined;
  const data = response ? response.data : undefined;

  return {
    name: error.name,
    code: error.code,
    message: error.message,
    status: response?.status,
    statusText: response?.statusText,
    data,
  };
}

function isTransientChunkUploadError(error: unknown, signal?: AbortSignal): boolean {
  if (signal?.aborted || isAbortLikeError(error)) {
    return false;
  }

  if (!isRecord(error)) {
    return false;
  }

  if (isRecord(error.response)) {
    return false;
  }

  const code = typeof error.code === "string" ? error.code : undefined;
  const message = typeof error.message === "string" ? error.message.toLowerCase() : "";

  return (
    code === "ERR_NETWORK" ||
    code === "ECONNABORTED" ||
    code === "ETIMEDOUT" ||
    message.includes("network error") ||
    message.includes("timeout")
  );
}

function isAbortLikeError(error: unknown): boolean {
  return Boolean(
    error &&
      typeof error === "object" &&
      ((error as { name?: unknown }).name === "AbortError" || (error as { code?: unknown }).code === "ERR_CANCELED"),
  );
}

function waitForRetry(delayMs: number, signal?: AbortSignal): Promise<void> {
  if (delayMs <= 0) {
    ensureNotAborted(signal);
    return Promise.resolve();
  }

  return new Promise((resolve, reject) => {
    if (signal?.aborted) {
      reject(
        new AlmacenamientoDocumentalUploadError({
          code: "storage_aborted",
          message: "Storage upload was aborted",
          phase: "uploading",
        }),
      );
      return;
    }

    const timeoutId = window.setTimeout(() => {
      signal?.removeEventListener("abort", handleAbort);
      resolve();
    }, delayMs);

    const handleAbort = () => {
      window.clearTimeout(timeoutId);
      reject(
        new AlmacenamientoDocumentalUploadError({
          code: "storage_aborted",
          message: "Storage upload was aborted",
          phase: "uploading",
        }),
      );
    };

    signal?.addEventListener("abort", handleAbort, { once: true });
  });
}
