const DEFAULT_DEMO_PDF = "/demo/20260410DiagnosticoCCV.pdf";

export function useDemoPdfUrl(): string {
  const configured =
    (import.meta.env.VITE_EMBEDPDF_DEMO_PDF as string | undefined) ?? "";
  return configured.trim() ? configured.trim() : DEFAULT_DEMO_PDF;
}
