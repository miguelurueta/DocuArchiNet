export type AppVisorPdfInput =
  | { kind: "url"; url: string }
  | { kind: "bytes"; bytes: Uint8Array; fileName?: string };

export type AppVisorPdfTool =
  | "pan"
  | "select"
  | "freehand"
  | "text"
  | "rect"
  | "arrow"
  | "stamp_grafo";

export type AppVisorPdfExportFormat = "original" | "annotated";

export type AppVisorPdfError = { message: string; code?: string };

export type AppVisorPdfProps = {
  input: AppVisorPdfInput | null;
  documentId?: string;
  readOnly?: boolean;

  /**
   * Estados controlables para integrar con flujos externos (por ejemplo: carga de URL firmada).
   * Nota: aunque el Ticket 01 mockea el engine, el contrato existe desde el inicio.
   */
  loading?: boolean;
  error?: AppVisorPdfError | null;
  onRetry?: () => void;

  page?: number;
  defaultPage?: number;
  onPageChange?: (page: number) => void;

  zoom?: number;
  defaultZoom?: number;
  onZoomChange?: (zoom: number) => void;

  tool?: AppVisorPdfTool;
  defaultTool?: AppVisorPdfTool;
  onToolChange?: (tool: AppVisorPdfTool) => void;

  onRequestSaveAnnotations?: () => Promise<void>;
  onRequestExport?: (format: AppVisorPdfExportFormat) => Promise<void>;

  className?: string;
  "aria-label"?: string;
};

