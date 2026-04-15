import { afterEach, describe, expect, it, vi } from "vitest";
import { generateLocalImageId } from "./application/localImageIds";

describe("localImageIds", () => {
  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("genera ids con el prefijo esperado", () => {
    vi.spyOn(crypto, "randomUUID").mockReturnValue(
      "abc-123-def-456-ghi" as `${string}-${string}-${string}-${string}-${string}`,
    );

    expect(generateLocalImageId()).toBe("img_local_abc-123-def-456-ghi");
  });
});
