import clienteApi from "../../../api/Clienteaxios";
import type {
  UploadTemporalCompleteResponse,
  UploadTemporalInitRequest,
  UploadTemporalInitResponse,
  UploadTemporalPdfOptions,
  UploadTemporalReferencia,
} from "../types/digitalizacionApi.types";
import {
  assertNonEmptyString,
  assertPdfFile,
  assertPositiveNumber,
  assertRecord,
  createDigitalizacionApiError,
  getBoolean,
  getNumber,
  getString,
  unwrapAppResponse,
  withSignal,
} from "./digitalizacionApiClient";

export const UPLOAD_TEMPORAL_INIT_ENDPOINT =
  "/api/gestor-documental/almacenamiento/upload-temporal/init";

export const getUploadTemporalChunkEndpoint = (
  rutaTemporalId: string,
  archivoTemporalId: string,
  chunkIndex: number,
) =>
  `/api/gestor-documental/almacenamiento/upload-temporal/${rutaTemporalId}/${archivoTemporalId}/chunk/${chunkIndex}`;

export const getUploadTemporalCompleteEndpoint = (
  rutaTemporalId: string,
  archivoTemporalId: string,
) =>
  `/api/gestor-documental/almacenamiento/upload-temporal/${rutaTemporalId}/${archivoTemporalId}/complete`;

export const DEFAULT_UPLOAD_TEMPORAL_CHUNK_SIZE_BYTES = 4 * 1024 * 1024;

const validateInitResponse = (value: unknown): UploadTemporalInitResponse => {
  const record = assertRecord(value, "UPLOAD_INIT_INVALID", "Upload init invalido.");
  const rutaTemporalId = assertNonEmptyString(
    getString(record, "rutaTemporalId", "RutaTemporalId"),
    "RUTA_TEMPORAL_REQUIRED",
    "RutaTemporalId es obligatorio.",
  );
  const archivoTemporalId = assertNonEmptyString(
    getString(record, "archivoTemporalId", "ArchivoTemporalId"),
    "ARCHIVO_TEMPORAL_REQUIRED",
    "ArchivoTemporalId es obligatorio.",
  );

  return {
    rutaTemporalId,
    archivoTemporalId,
    chunkSizeBytes:
      getNumber(record, "chunkSizeBytes", "ChunkSizeBytes") ??
      DEFAULT_UPLOAD_TEMPORAL_CHUNK_SIZE_BYTES,
    totalChunks: getNumber(record, "totalChunks", "TotalChunks") ?? 1,
  };
};

const validateCompleteResponse = (value: unknown): UploadTemporalCompleteResponse => {
  const record = assertRecord(value, "UPLOAD_COMPLETE_INVALID", "Upload complete invalido.");
  const completado = getBoolean(record, "completado", "Completado") ?? false;
  if (!completado) {
    throw createDigitalizacionApiError(
      "UPLOAD_COMPLETE_NOT_CONFIRMED",
      "El backend no confirmo el upload temporal.",
      "error",
    );
  }

  return {
    rutaTemporalId: assertNonEmptyString(
      getString(record, "rutaTemporalId", "RutaTemporalId"),
      "RUTA_TEMPORAL_REQUIRED",
      "RutaTemporalId es obligatorio.",
    ),
    archivoTemporalId: assertNonEmptyString(
      getString(record, "archivoTemporalId", "ArchivoTemporalId"),
      "ARCHIVO_TEMPORAL_REQUIRED",
      "ArchivoTemporalId es obligatorio.",
    ),
    completado,
  };
};

export async function initUploadTemporalPdf(
  request: UploadTemporalInitRequest,
  options: UploadTemporalPdfOptions = {},
) {
  const response = await clienteApi.post(UPLOAD_TEMPORAL_INIT_ENDPOINT, request, {
    ...withSignal(options.signal),
  });

  return unwrapAppResponse<UploadTemporalInitResponse>(
    response.data,
    validateInitResponse,
    "upload-init",
  );
}

export async function uploadTemporalPdfChunk(
  reference: UploadTemporalReferencia,
  chunkIndex: number,
  chunk: Blob,
  options: UploadTemporalPdfOptions = {},
) {
  assertNonEmptyString(reference.rutaTemporalId, "RUTA_TEMPORAL_REQUIRED", "RutaTemporalId es obligatorio.");
  assertNonEmptyString(
    reference.archivoTemporalId,
    "ARCHIVO_TEMPORAL_REQUIRED",
    "ArchivoTemporalId es obligatorio.",
  );
  assertPositiveNumber(chunkIndex + 1, "CHUNK_INDEX_INVALID", "chunkIndex invalido.");

  const response = await clienteApi.put(
    getUploadTemporalChunkEndpoint(reference.rutaTemporalId, reference.archivoTemporalId, chunkIndex),
    chunk,
    {
      headers: { "Content-Type": "application/octet-stream" },
      ...withSignal(options.signal),
    },
  );

  if (response.data && typeof response.data === "object" && "success" in response.data) {
    unwrapAppResponse(response.data, (data) => data, "upload-chunk");
  }
}

export async function completeUploadTemporalPdf(
  reference: UploadTemporalReferencia,
  request: { SizeBytes: number; TotalChunks: number; RequestId?: string },
  options: UploadTemporalPdfOptions = {},
) {
  const response = await clienteApi.post(
    getUploadTemporalCompleteEndpoint(reference.rutaTemporalId, reference.archivoTemporalId),
    request,
    {
      ...withSignal(options.signal),
    },
  );

  return unwrapAppResponse<UploadTemporalCompleteResponse>(
    response.data,
    validateCompleteResponse,
    "upload-complete",
  );
}

export async function uploadPdfTemporal(
  fileInput: File,
  options: UploadTemporalPdfOptions = {},
): Promise<UploadTemporalReferencia> {
  const file = assertPdfFile(fileInput);
  const chunkSizeBytes = options.chunkSizeBytes ?? DEFAULT_UPLOAD_TEMPORAL_CHUNK_SIZE_BYTES;
  const totalChunks = Math.max(1, Math.ceil(file.size / chunkSizeBytes));
  const init = await initUploadTemporalPdf(
    {
      NombreArchivo: file.name,
      ContentType: file.type || "application/pdf",
      SizeBytes: file.size,
      ChunkSizeBytes: chunkSizeBytes,
      TotalChunks: totalChunks,
      ModuloRegistro: "DIGITALIZACION",
      RequestId: options.requestId,
    },
    options,
  );
  const reference = {
    rutaTemporalId: init.rutaTemporalId,
    archivoTemporalId: init.archivoTemporalId,
  };

  for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex += 1) {
    if (options.signal?.aborted) {
      throw createDigitalizacionApiError("REQUEST_ABORTED", "Upload cancelado.", "aborted");
    }
    const start = chunkIndex * chunkSizeBytes;
    const end = Math.min(file.size, start + chunkSizeBytes);
    await uploadTemporalPdfChunk(reference, chunkIndex, file.slice(start, end), options);
    options.onProgress?.({
      uploadedChunks: chunkIndex + 1,
      totalChunks,
      progress: Math.round(((chunkIndex + 1) / totalChunks) * 100),
    });
  }

  await completeUploadTemporalPdf(
    reference,
    {
      SizeBytes: file.size,
      TotalChunks: totalChunks,
      RequestId: options.requestId,
    },
    options,
  );

  return reference;
}
