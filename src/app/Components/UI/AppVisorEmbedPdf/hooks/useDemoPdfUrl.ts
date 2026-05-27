const DEFAULT_DEMO_PDF = "/demo/20260410DiagnosticoCCV.pdf";

const readBooleanEnv = (value: unknown, fallback: boolean) => {
  if (value === undefined || value === null || value === "") return fallback;
  return String(value).toLowerCase() !== "false";
};

export function useDemoPdfUrl(): string | null {
  // Demo PDF should be opt-in to avoid masking integration issues.
  const enabled = readBooleanEnv(import.meta.env.VITE_EMBEDPDF_SHOW_DEMO_PDF, false);
  if (!enabled) return null;

  const configured = (import.meta.env.VITE_EMBEDPDF_DEMO_PDF as string | undefined) ?? "";
  const resolved = configured.trim() ? configured.trim() : DEFAULT_DEMO_PDF;
  return resolved;
}
