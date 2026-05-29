import type { FitMode } from "./autoFit.math";
import { computeFitScale } from "./autoFit.math";

type ViewportMetrics = {
  clientWidth: number;
  clientHeight: number;
  scrollWidth?: number;
  scrollHeight?: number;
};

type ViewportScope = {
  getMetrics(): ViewportMetrics;
  scrollTo?(params: { x: number; y: number; behavior?: ScrollBehavior }): void;
};

type ViewportProvides = {
  forDocument(documentId: string): ViewportScope;
};

type ZoomProvides = {
  requestZoom(zoomLevel: number, center?: { vx: number; vy: number }): void;
};

export function applyAutoFitOnce(params: {
  documentId: string;
  fitMode: FitMode;
  rotationSteps?: number;
  zoomLevel: number;
  zoomProvides: ZoomProvides | undefined;
  viewportProvides: ViewportProvides | undefined;
}): { ok: boolean; appliedZoom?: number } {
  const { documentId, fitMode, rotationSteps, zoomLevel, zoomProvides, viewportProvides } = params;
  if (!zoomProvides) return { ok: false };
  if (!viewportProvides) return { ok: false };

  const scope = viewportProvides.forDocument(documentId);
  const m = scope.getMetrics();

  const clientWidth = Number(m.clientWidth);
  const clientHeight = Number(m.clientHeight);
  if (!Number.isFinite(clientWidth) || clientWidth <= 0) return { ok: false };
  if (!Number.isFinite(clientHeight) || clientHeight <= 0) return { ok: false };

  // Estimación determinística del tamaño base (zoom=1):
  // Si el engine expone scrollWidth/scrollHeight del contenido renderizado,
  // se puede normalizar por zoom actual.
  const scrollWidth = typeof m.scrollWidth === "number" ? m.scrollWidth : undefined;
  const scrollHeight = typeof m.scrollHeight === "number" ? m.scrollHeight : undefined;
  if (!scrollWidth || !scrollHeight) return { ok: false };

  const baseZoom = Number.isFinite(zoomLevel) && zoomLevel > 0 ? zoomLevel : 1;
  let baseContentWidth = scrollWidth / baseZoom;
  let baseContentHeight = scrollHeight / baseZoom;

  // Si la rotación metadata es 90/270, el “ancho efectivo” y “alto efectivo”
  // se invierten para calcular fit-to-width/page de forma determinística.
  const steps = typeof rotationSteps === "number" ? rotationSteps : 0;
  const normalizedSteps = ((steps % 4) + 4) % 4;
  if (normalizedSteps === 1 || normalizedSteps === 3) {
    const tmp = baseContentWidth;
    baseContentWidth = baseContentHeight;
    baseContentHeight = tmp;
  }

  const targetZoom = computeFitScale({
    viewport: { width: clientWidth, height: clientHeight },
    content: { width: baseContentWidth, height: baseContentHeight },
    fitMode,
  });

  const center = { vx: clientWidth / 2, vy: clientHeight / 2 };
  zoomProvides.requestZoom(targetZoom, center);

  // Nota: el centering exacto depende del engine; el zoom con centro reduce “anclaje” top/left.
  // No forzamos scrollTo adicional para evitar saltos.
  return { ok: true, appliedZoom: targetZoom };
}
