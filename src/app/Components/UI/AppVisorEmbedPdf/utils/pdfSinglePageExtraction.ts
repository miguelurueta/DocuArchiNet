import { blobToArrayBuffer } from "./blobToArrayBuffer";

export type ExtractedSinglePagePdf = {
  pageNumber: number;
  blob: Blob;
};

function assertPageNumber(pageNumber: number) {
  if (!Number.isInteger(pageNumber) || pageNumber <= 0) {
    throw new Error("PageNumber debe ser un entero positivo 1-based.");
  }
}

function uint8ArrayToBlobPart(bytes: Uint8Array<ArrayBufferLike>): BlobPart {
  const copy = new Uint8Array(bytes.byteLength);
  copy.set(bytes);
  return copy.buffer;
}

export async function extractSinglePagePdf(sourcePdf: Blob, pageNumber: number): Promise<Blob> {
  assertPageNumber(pageNumber);

  const { PDFDocument } = await import("pdf-lib");
  const sourceBytes = await blobToArrayBuffer(sourcePdf);
  const sourceDocument = await PDFDocument.load(sourceBytes);
  const pageCount = sourceDocument.getPageCount();
  if (pageNumber > pageCount) {
    throw new Error(`PageNumber ${pageNumber} esta fuera del rango del PDF (${pageCount}).`);
  }

  const targetDocument = await PDFDocument.create();
  const [page] = await targetDocument.copyPages(sourceDocument, [pageNumber - 1]);
  targetDocument.addPage(page);

  const bytes = await targetDocument.save();
  return new Blob([uint8ArrayToBlobPart(bytes)], { type: "application/pdf" });
}

export async function extractSinglePagePdfs(
  sourcePdf: Blob,
  pageNumbers: number[],
): Promise<ExtractedSinglePagePdf[]> {
  const uniquePageNumbers = Array.from(new Set(pageNumbers)).sort((left, right) => left - right);

  const extracted: ExtractedSinglePagePdf[] = [];
  for (const pageNumber of uniquePageNumbers) {
    extracted.push({
      pageNumber,
      blob: await extractSinglePagePdf(sourcePdf, pageNumber),
    });
  }

  return extracted;
}
