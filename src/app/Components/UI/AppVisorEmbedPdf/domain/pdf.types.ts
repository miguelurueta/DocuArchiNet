export type AppPdfSource =
  | { kind: "url"; url: string; filename?: string }
  | { kind: "bytes"; bytes: ArrayBuffer; filename?: string };

export type AppPdfLoadState =
  | "idle"
  | "loading"
  | "ready"
  | "password_required"
  | "error";

export type AppPdfCapabilities = {
  zoom?: boolean;
  rotate?: boolean;
  search?: boolean;
  print?: boolean;
  download?: boolean;
  annotations?: boolean;
  signatures?: boolean;
  thumbnails?: boolean;
  password?: boolean;
};

