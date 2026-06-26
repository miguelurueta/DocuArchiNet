import { describe, expect, it } from "vitest";
import {
  DEFAULT_STORAGE_CONTENT_TYPE,
  calculateTotalChunks,
  createStorageRequestId,
  getFileContentType,
  normalizeFileExtension,
  sliceFileChunk,
} from "./storageFile.utils";

function readBlobText(blob: Blob): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onerror = () => reject(reader.error);
    reader.onload = () => resolve(String(reader.result));
    reader.readAsText(blob);
  });
}

describe("[SPEC:SCRUMCORE-272] storageFile utils", () => {
  it("normalizes file extensions", () => {
    expect(normalizeFileExtension("Documento.PDF")).toBe(".pdf");
    expect(normalizeFileExtension("anexo.final.TIFF")).toBe(".tiff");
  });

  it("returns an empty extension for files without extension", () => {
    expect(normalizeFileExtension("README")).toBe("");
    expect(normalizeFileExtension(".env")).toBe("");
    expect(normalizeFileExtension("archivo.")).toBe("");
  });

  it("calculates total chunks with positive sizes", () => {
    expect(calculateTotalChunks(0, 1024)).toBe(1);
    expect(calculateTotalChunks(6, 3)).toBe(2);
    expect(calculateTotalChunks(7, 3)).toBe(3);
  });

  it("rejects invalid chunk size inputs", () => {
    expect(() => calculateTotalChunks(-1, 3)).toThrow("sizeBytes");
    expect(() => calculateTotalChunks(1, 0)).toThrow("chunkSizeBytes");
    expect(() => calculateTotalChunks(1, Number.NaN)).toThrow("chunkSizeBytes");
  });

  it("slices chunks with Blob.slice without loading the full file", async () => {
    const file = new File(["abcdef"], "scan.pdf", { type: "application/pdf" });
    const chunk = sliceFileChunk(file, 1, 3);

    expect(chunk).toBeInstanceOf(Blob);
    expect(chunk.size).toBe(3);
    await expect(readBlobText(chunk)).resolves.toBe("def");
  });

  it("rejects slice bounds outside the file", () => {
    const file = new File(["abcdef"], "scan.pdf", { type: "application/pdf" });
    expect(() => sliceFileChunk(file, 2, 3)).toThrow("outside the file bounds");
  });

  it("returns content type fallback when the Blob has no type", () => {
    expect(getFileContentType(new Blob(["abc"]))).toBe(DEFAULT_STORAGE_CONTENT_TYPE);
  });

  it("creates traceable storage request ids", () => {
    expect(createStorageRequestId("doc")).toMatch(/^doc-/);
    expect(createStorageRequestId("")).toMatch(/^storage-/);
  });
});
