import { describe, expect, it } from "vitest";
import { PDFDocument } from "pdf-lib";

import { blobToArrayBuffer } from "./blobToArrayBuffer";
import { extractSinglePagePdf, extractSinglePagePdfs } from "./pdfSinglePageExtraction";

async function createPdfBlob(pageCount: number): Promise<Blob> {
  const document = await PDFDocument.create();
  for (let index = 0; index < pageCount; index += 1) {
    document.addPage([612, 792]);
  }

  const bytes = await document.save();
  return new Blob([bytes], { type: "application/pdf" });
}

async function countPages(blob: Blob): Promise<number> {
  const document = await PDFDocument.load(await blobToArrayBuffer(blob));
  return document.getPageCount();
}

describe("pdfSinglePageExtraction", () => {
  it("extrae un PDF real de una sola pagina usando PageNumber 1-based", async () => {
    const source = await createPdfBlob(3);

    const extracted = await extractSinglePagePdf(source, 2);

    expect(extracted.type).toBe("application/pdf");
    expect(extracted.size).toBeGreaterThan(0);
    await expect(countPages(extracted)).resolves.toBe(1);
  });

  it("deduplica y ordena multiples paginas", async () => {
    const source = await createPdfBlob(4);

    const extracted = await extractSinglePagePdfs(source, [3, 1, 3]);

    expect(extracted.map((item) => item.pageNumber)).toEqual([1, 3]);
    await expect(Promise.all(extracted.map((item) => countPages(item.blob)))).resolves.toEqual([1, 1]);
  });

  it("rechaza PageNumber fuera de rango", async () => {
    const source = await createPdfBlob(1);

    await expect(extractSinglePagePdf(source, 2)).rejects.toThrow(/fuera del rango/i);
  });
});
