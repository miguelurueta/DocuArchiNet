import type { AppPdfSource } from "./pdf.types";

export type AppPdfViewerApi = {
  setSource: (source: AppPdfSource | null) => void;
  setZoom: (zoom: number) => void;
  setRotation: (degrees: number) => void;
  openSidebar: () => void;
  closeSidebar: () => void;
};

