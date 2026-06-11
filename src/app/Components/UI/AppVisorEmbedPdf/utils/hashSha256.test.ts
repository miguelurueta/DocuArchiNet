import { describe, expect, it } from "vitest";

import { calculateBlobSha256 } from "./hashSha256";

describe("calculateBlobSha256", () => {
  it("calcula SHA-256 hexadecimal de un Blob", async () => {
    const hash = await calculateBlobSha256(new Blob(["abc"]));

    expect(hash).toBe("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad");
  });
});
