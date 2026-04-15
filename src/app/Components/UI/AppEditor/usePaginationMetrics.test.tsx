import { describe, expect, it } from "vitest";
import { calculatePaginationMetrics } from "./application/usePaginationMetrics";

describe("usePaginationMetrics [SPEC:IMPLEMENTACION-PAGINACION-APPEDITOR-07-FE]", () => {
  it("calcula altura util y total de paginas estimadas", () => {
    const result = calculatePaginationMetrics({
      contentHeight: 2200,
      pageHeight: 1123,
      pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    });

    expect(result.pageContentHeight).toBe(931);
    expect(result.totalPages).toBe(3);
    expect(result.guideOffsets).toEqual([1027, 1958]);
    expect(result.pageBoundaries).toEqual([931, 1862]);
  });

  it("mantiene una pagina minima y sin guias cuando el contenido es vacio", () => {
    const result = calculatePaginationMetrics({
      contentHeight: 0,
      pageHeight: 1123,
      pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
    });

    expect(result.totalPages).toBe(1);
    expect(result.guideOffsets).toEqual([]);
    expect(result.pageBoundaries).toEqual([]);
  });

  it("trata PageBreak como limite duro y reinicia el calculo de paginas", () => {
    const result = calculatePaginationMetrics({
      contentHeight: 2200,
      pageHeight: 1123,
      pageMargins: { top: 96, right: 72, bottom: 96, left: 72 },
      manualBreakOffsets: [500],
    });

    expect(result.totalPages).toBe(3);
    expect(result.pageBoundaries).toEqual([500, 1431]);
    expect(result.guideOffsets).toEqual([1527]);
  });
});
