import { blobToArrayBuffer } from "./blobToArrayBuffer";

function toHex(buffer: ArrayBuffer): string {
  return Array.from(new Uint8Array(buffer))
    .map((byte) => byte.toString(16).padStart(2, "0"))
    .join("");
}

export async function calculateBlobSha256(blob: Blob): Promise<string> {
  const subtle = globalThis.crypto?.subtle;
  if (!subtle) {
    throw new Error("SHA-256 no disponible: el navegador no expone crypto.subtle.");
  }

  const buffer = await blobToArrayBuffer(blob);
  const digest = await subtle.digest("SHA-256", new Uint8Array(buffer));
  return toHex(digest);
}
