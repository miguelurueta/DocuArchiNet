export const DEFAULT_STORAGE_CHUNK_SIZE_BYTES = 4 * 1024 * 1024;
export const DEFAULT_STORAGE_CONTENT_TYPE = "application/octet-stream";

export function normalizeFileExtension(fileName: string): string {
  const trimmedName = fileName.trim();
  const lastDotIndex = trimmedName.lastIndexOf(".");

  if (lastDotIndex <= 0 || lastDotIndex === trimmedName.length - 1) {
    return "";
  }

  return trimmedName.slice(lastDotIndex).toLowerCase();
}

export function getFileContentType(file: Blob, fallback = DEFAULT_STORAGE_CONTENT_TYPE): string {
  return file.type.trim() || fallback;
}

export function calculateTotalChunks(sizeBytes: number, chunkSizeBytes: number): number {
  assertNonNegativeFiniteNumber(sizeBytes, "sizeBytes");
  assertPositiveFiniteNumber(chunkSizeBytes, "chunkSizeBytes");

  return Math.max(1, Math.ceil(sizeBytes / chunkSizeBytes));
}

export function sliceFileChunk(file: Blob, chunkIndex: number, chunkSizeBytes: number): Blob {
  assertNonNegativeInteger(chunkIndex, "chunkIndex");
  assertPositiveFiniteNumber(chunkSizeBytes, "chunkSizeBytes");

  const start = chunkIndex * chunkSizeBytes;
  if (start >= file.size && file.size > 0) {
    throw new RangeError("chunkIndex is outside the file bounds");
  }

  const end = Math.min(start + chunkSizeBytes, file.size);
  return file.slice(start, end, getFileContentType(file));
}

export function createStorageRequestId(prefix = "storage"): string {
  const normalizedPrefix = prefix.trim() || "storage";
  const cryptoApi = globalThis.crypto;

  if (cryptoApi && typeof cryptoApi.randomUUID === "function") {
    return `${normalizedPrefix}-${cryptoApi.randomUUID()}`;
  }

  const randomPart = Math.random().toString(36).slice(2, 10);
  return `${normalizedPrefix}-${Date.now().toString(36)}-${randomPart}`;
}

export function isRecord(value: unknown): value is Record<string, unknown> {
  return typeof value === "object" && value !== null && !Array.isArray(value);
}

export function getStringField(record: Record<string, unknown>, ...keys: string[]): string | undefined {
  for (const key of keys) {
    const value = record[key];
    if (typeof value === "string") {
      return value;
    }
  }

  return undefined;
}

export function getNumberField(record: Record<string, unknown>, ...keys: string[]): number | undefined {
  for (const key of keys) {
    const value = record[key];
    if (typeof value === "number" && Number.isFinite(value)) {
      return value;
    }
  }

  return undefined;
}

export function getBooleanField(record: Record<string, unknown>, ...keys: string[]): boolean | undefined {
  for (const key of keys) {
    const value = record[key];
    if (typeof value === "boolean") {
      return value;
    }
  }

  return undefined;
}

export function assertNonEmptyString(value: unknown, fieldName: string): string {
  if (typeof value !== "string" || value.trim().length === 0) {
    throw new TypeError(`${fieldName} must be a non-empty string`);
  }

  return value;
}

export function assertPositiveFiniteNumber(value: unknown, fieldName: string): number {
  if (typeof value !== "number" || !Number.isFinite(value) || value <= 0) {
    throw new TypeError(`${fieldName} must be a positive number`);
  }

  return value;
}

export function assertNonNegativeFiniteNumber(value: unknown, fieldName: string): number {
  if (typeof value !== "number" || !Number.isFinite(value) || value < 0) {
    throw new TypeError(`${fieldName} must be a non-negative number`);
  }

  return value;
}

export function assertNonNegativeInteger(value: unknown, fieldName: string): number {
  if (typeof value !== "number" || !Number.isInteger(value) || value < 0) {
    throw new TypeError(`${fieldName} must be a non-negative integer`);
  }

  return value;
}

export function clampPercent(percent: number): number {
  if (!Number.isFinite(percent)) {
    return 0;
  }

  return Math.min(100, Math.max(0, Math.round(percent)));
}
