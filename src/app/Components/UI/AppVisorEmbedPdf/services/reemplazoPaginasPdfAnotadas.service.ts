import clienteApi from "../../../../../api/Clienteaxios";
import type {
  AppResponses,
  ReemplazarPaginasPdfAnotadasRequest,
  ReemplazarPaginasPdfAnotadasResponse,
  StorageUploadCancelResponseDto,
  StorageUploadChunkResponseDto,
  StorageUploadCompleteResponseDto,
  StorageUploadInitRequest,
  StorageUploadInitResponseDto,
  StorageUploadStatusResponseDto,
} from "./reemplazoPaginasPdfAnotadas.types";
import { ReemplazoPaginasPdfAnotadasError } from "./reemplazoPaginasPdfAnotadas.types";

const BASE_ENDPOINT = "/api/gestor-documental/documentos/reemplazopdf";

export const REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS = {
  init: `${BASE_ENDPOINT}/upload-temporal/init`,
  chunk: (rutaTemporalId: string, archivoTemporalId: string, chunkIndex: number) =>
    `${BASE_ENDPOINT}/upload-temporal/${encodeURIComponent(rutaTemporalId)}/${encodeURIComponent(
      archivoTemporalId,
    )}/chunk/${chunkIndex}`,
  status: (rutaTemporalId: string, archivoTemporalId: string) =>
    `${BASE_ENDPOINT}/upload-temporal/${encodeURIComponent(rutaTemporalId)}/${encodeURIComponent(
      archivoTemporalId,
    )}/status`,
  complete: (rutaTemporalId: string, archivoTemporalId: string) =>
    `${BASE_ENDPOINT}/upload-temporal/${encodeURIComponent(rutaTemporalId)}/${encodeURIComponent(
      archivoTemporalId,
    )}/complete`,
  cancel: (rutaTemporalId: string, archivoTemporalId: string) =>
    `${BASE_ENDPOINT}/upload-temporal/${encodeURIComponent(rutaTemporalId)}/${encodeURIComponent(
      archivoTemporalId,
    )}`,
  reemplazar: `${BASE_ENDPOINT}/paginas-anotadas`,
} as const;

function firstMeaningfulMessage<T>(envelope: AppResponses<T> | null | undefined): string | undefined {
  const errorMessage = envelope?.errors?.find((item) => typeof item?.Message === "string" && item.Message.trim())
    ?.Message;
  if (typeof errorMessage === "string" && errorMessage.trim()) return errorMessage.trim();
  if (typeof envelope?.message === "string" && envelope.message.trim()) return envelope.message.trim();
  return undefined;
}

function readRequestId<T>(envelope: AppResponses<T> | null | undefined): string | undefined {
  const metaRequestId = envelope?.meta?.RequestId;
  if (typeof metaRequestId === "string" && metaRequestId.trim()) return metaRequestId.trim();

  const errorRequestId = envelope?.errors?.find((item) => typeof item?.RequestId === "string" && item.RequestId.trim())
    ?.RequestId;
  if (typeof errorRequestId === "string" && errorRequestId.trim()) return errorRequestId.trim();

  const dataRequestId = (envelope?.data as { RequestId?: unknown } | null | undefined)?.RequestId;
  if (typeof dataRequestId === "string" && dataRequestId.trim()) return dataRequestId.trim();

  return undefined;
}

export function unwrapAppResponse<T>(
  envelope: AppResponses<T>,
  options: { operation: string; requireData?: boolean },
): T {
  const { operation, requireData = true } = options;
  const details = Array.isArray(envelope?.errors) ? envelope.errors : [];

  if (!envelope?.success) {
    throw new ReemplazoPaginasPdfAnotadasError(
      firstMeaningfulMessage(envelope) ?? `${operation}: respuesta no exitosa.`,
      details,
      readRequestId(envelope),
    );
  }

  if (requireData && envelope.data == null) {
    throw new ReemplazoPaginasPdfAnotadasError(
      `${operation}: contrato invalido, data requerido.`,
      details,
      readRequestId(envelope),
    );
  }

  return envelope.data as T;
}

export async function initUploadTemporalPdfAnotado(
  request: StorageUploadInitRequest,
  options?: { signal?: AbortSignal },
): Promise<StorageUploadInitResponseDto> {
  const response = await clienteApi.post<AppResponses<StorageUploadInitResponseDto>>(
    REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.init,
    request,
    options?.signal ? { signal: options.signal } : undefined,
  );

  return unwrapAppResponse(response.data, { operation: "init upload temporal PDF anotado" });
}

export async function uploadTemporalChunk(
  params: {
    rutaTemporalId: string;
    archivoTemporalId: string;
    chunkIndex: number;
    totalChunks: number;
    chunk: Blob | ArrayBuffer;
  },
  options?: { signal?: AbortSignal },
): Promise<StorageUploadChunkResponseDto> {
  const { rutaTemporalId, archivoTemporalId, chunkIndex, totalChunks, chunk } = params;
  const response = await clienteApi.put<AppResponses<StorageUploadChunkResponseDto>>(
    REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.chunk(rutaTemporalId, archivoTemporalId, chunkIndex),
    chunk,
    {
      ...(options?.signal ? { signal: options.signal } : {}),
      headers: {
        "Content-Type": "application/octet-stream",
        "X-Total-Chunks": String(totalChunks),
      },
    },
  );

  return unwrapAppResponse(response.data, { operation: "upload chunk PDF anotado" });
}

export async function statusUploadTemporal(
  params: { rutaTemporalId: string; archivoTemporalId: string },
  options?: { signal?: AbortSignal },
): Promise<StorageUploadStatusResponseDto> {
  const { rutaTemporalId, archivoTemporalId } = params;
  const response = await clienteApi.get<AppResponses<StorageUploadStatusResponseDto>>(
    REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.status(rutaTemporalId, archivoTemporalId),
    options?.signal ? { signal: options.signal } : undefined,
  );

  return unwrapAppResponse(response.data, { operation: "status upload temporal PDF anotado" });
}

export async function completeUploadTemporal(
  params: { rutaTemporalId: string; archivoTemporalId: string },
  options?: { signal?: AbortSignal },
): Promise<StorageUploadCompleteResponseDto> {
  const { rutaTemporalId, archivoTemporalId } = params;
  const response = await clienteApi.post<AppResponses<StorageUploadCompleteResponseDto>>(
    REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.complete(rutaTemporalId, archivoTemporalId),
    {},
    options?.signal ? { signal: options.signal } : undefined,
  );

  return unwrapAppResponse(response.data, { operation: "complete upload temporal PDF anotado" });
}

export async function cancelUploadTemporal(
  params: { rutaTemporalId: string; archivoTemporalId: string },
  options?: { signal?: AbortSignal },
): Promise<StorageUploadCancelResponseDto> {
  const { rutaTemporalId, archivoTemporalId } = params;
  const response = await clienteApi.delete<AppResponses<StorageUploadCancelResponseDto>>(
    REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.cancel(rutaTemporalId, archivoTemporalId),
    options?.signal ? { signal: options.signal } : undefined,
  );

  return unwrapAppResponse(response.data, { operation: "cancel upload temporal PDF anotado" });
}

export async function reemplazarPaginasPdfAnotadas(
  request: ReemplazarPaginasPdfAnotadasRequest,
  options?: { signal?: AbortSignal },
): Promise<ReemplazarPaginasPdfAnotadasResponse> {
  const response = await clienteApi.post<AppResponses<ReemplazarPaginasPdfAnotadasResponse>>(
    REEMPLAZO_PAGINAS_PDF_ANOTADAS_ENDPOINTS.reemplazar,
    request,
    options?.signal ? { signal: options.signal } : undefined,
  );

  return unwrapAppResponse(response.data, { operation: "reemplazar paginas PDF anotadas" });
}
