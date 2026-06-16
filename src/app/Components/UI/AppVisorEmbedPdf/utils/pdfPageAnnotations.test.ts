import { describe, expect, it } from "vitest";

import { getAnnotatedPageNumbers } from "./pdfPageAnnotations";

describe("getAnnotatedPageNumbers", () => {
  it("convierte indices base 0 a PageNumber base 1, deduplica y ordena", () => {
    expect(
      getAnnotatedPageNumbers({
        "4": ["a"],
        "1": ["b", "c"],
        "0": [],
        "2": ["d"],
        invalid: ["x"],
      }),
    ).toEqual([2, 3, 5]);
  });

  it("retorna lista vacia sin paginas anotadas", () => {
    expect(getAnnotatedPageNumbers(undefined)).toEqual([]);
    expect(getAnnotatedPageNumbers({ "0": [] })).toEqual([]);
  });
});
