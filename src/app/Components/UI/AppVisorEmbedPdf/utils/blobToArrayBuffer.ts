export function blobToArrayBuffer(blob: Blob): Promise<ArrayBuffer> {
  if (typeof blob.arrayBuffer === "function") return blob.arrayBuffer();

  return new Promise<ArrayBuffer>((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      if (reader.result instanceof ArrayBuffer) {
        resolve(reader.result);
        return;
      }

      reject(new Error("No fue posible leer el Blob como ArrayBuffer."));
    };
    reader.onerror = () => reject(reader.error ?? new Error("No fue posible leer el Blob."));
    reader.readAsArrayBuffer(blob);
  });
}
