import { describe, expect, it } from "vitest";
import { isPdfFromContentType, pickResolvedFileUrl } from "../AppDocumentViewerOrchestrator.adapter";

describe("AppDocumentViewerOrchestrator.adapter", () => {
  it("prioriza UrlTemporalAbsoluta sobre UrlTemporal", () => {
    expect(
      pickResolvedFileUrl({ UrlTemporalAbsoluta: "https://x/y.pdf", UrlTemporal: "/tmp/1" }),
    ).toBe("https://x/y.pdf");
  });

  it("hace fallback a UrlTemporal si UrlTemporalAbsoluta es null/vacia", () => {
    expect(pickResolvedFileUrl({ UrlTemporalAbsoluta: null, UrlTemporal: "/tmp/1" })).toBe("/tmp/1");
    expect(pickResolvedFileUrl({ UrlTemporalAbsoluta: "   ", UrlTemporal: "/tmp/1" })).toBe("/tmp/1");
  });

  it("detecta PDF por ContentType y fallback por FileName", () => {
    expect(isPdfFromContentType("application/pdf", "x.bin")).toBe(true);
    expect(isPdfFromContentType("APPLICATION/PDF", "x.bin")).toBe(true);
    expect(isPdfFromContentType("application/octet-stream", "x.pdf")).toBe(true);
    expect(isPdfFromContentType("text/plain", "x.txt")).toBe(false);
  });
});

