import { GlobalWorkerOptions } from "pdfjs-dist";
import workerUrl from "pdfjs-dist/build/pdf.worker.min.mjs?url";

export function ensurePdfjsWorkerConfigured() {
  if (GlobalWorkerOptions.workerSrc) {
    return;
  }
  GlobalWorkerOptions.workerSrc = workerUrl;
}

