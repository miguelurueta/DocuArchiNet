export type AppResponses<T> = {
  success: boolean;
  message: string;
  data: T | null;
  meta?: {
    Status?: string;
    RequestId?: string;
  };
  errors?: Array<{
    Type?: string;
    Field?: string;
    Message?: string;
    RequestId?: string;
  }>;
};

export type ReemplazoPaginasPdfAnotadasErrorDetail = NonNullable<AppResponses<unknown>["errors"]>[number];

export class ReemplazoPaginasPdfAnotadasError extends Error {
  readonly field?: string;
  readonly type?: string;
  readonly requestId?: string;
  readonly details: ReemplazoPaginasPdfAnotadasErrorDetail[];

  constructor(message: string, details: ReemplazoPaginasPdfAnotadasErrorDetail[] = [], requestId?: string) {
    super(message);
    this.name = "ReemplazoPaginasPdfAnotadasError";
    this.details = details;
    this.field = details.find((item) => typeof item?.Field === "string" && item.Field.trim())?.Field;
    this.type = details.find((item) => typeof item?.Type === "string" && item.Type.trim())?.Type;
    this.requestId =
      requestId ?? details.find((item) => typeof item?.RequestId === "string" && item.RequestId.trim())?.RequestId;
  }
}

export type StorageUploadEstado = "IN_PROGRESS" | "COMPLETED" | "CANCELLED";

export type StorageUploadInitRequest = {
  NombreOriginal: string;
  TamanoBytes: number;
  Extension: ".PDF";
  HashSha256Esperado?: string | null;
  NumeroChunks: number;
};

export type StorageUploadInitResponseDto = {
  RutaTemporalId: string;
  ArchivoTemporalId: string;
  ChunkSizeBytes: number;
  Estado: StorageUploadEstado;
};

export type StorageUploadChunkResponseDto = {
  chunkIndex: number;
};

export type StorageUploadStatusResponseDto = {
  Estado: StorageUploadEstado;
  ChunksRecibidos: number;
  ChunksPendientes: number;
  TamanoRecibidoBytes: number;
};

export type StorageUploadCompleteResponseDto = {
  Estado: StorageUploadEstado;
};

export type StorageUploadCancelResponseDto = {
  Estado: StorageUploadEstado;
};

export type ReemplazarPaginasPdfAnotadasPageRequest = {
  PageNumber: number;
  RutaTemporalId: string;
  ArchivoTemporalId: string;
  ContentType: "application/pdf";
  HashSha256Esperado?: string | null;
  SourcePageWidth?: number;
  SourcePageHeight?: number;
  SourcePageRotation?: number;
  SourcePageFingerprintSha256?: string;
};

export type ReemplazarPaginasPdfAnotadasRequest = {
  NombreGabinete: string;
  IdDocumento: number;
  RutaTemporalId?: string;
  OriginalPdfPassword?: string;
  SourceDocumentHashSha256?: string;
  SourceDocumentVersion?: string;
  Paginas: ReemplazarPaginasPdfAnotadasPageRequest[];
  Motivo?: string;
  DescOp?: string;
  ModuloRegistro?: "DOCUARCHI" | "PRODUCCION" | "WORKFLOW";
  Radicado?: string;
  IdTareaWorkflow?: number;
  IdRutaWorkflow?: number;
  TipologiaDocumental?: string;
};

export type ReemplazarPaginasPdfAnotadasResponse = {
  IdDocumento: number;
  NombreGabinete: string;
  PaginasReemplazadas: number[];
  RutaArchivoFinal: string;
  RutaRespaldo: string;
  TamanoAnteriorBytes: number;
  TamanoNuevoBytes: number;
  HashAnteriorSha256: string;
  HashNuevoSha256: string;
  RequestId: string;
};
