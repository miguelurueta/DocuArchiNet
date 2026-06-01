export type FitMode = "width" | "page";

export type ViewportSize = {
  width: number;
  height: number;
};

export type ContentSize = {
  width: number;
  height: number;
};

export function computeFitScale(params: {
  viewport: ViewportSize;
  content: ContentSize;
  fitMode: FitMode;
}): number {
  const { viewport, content, fitMode } = params;

  if (!Number.isFinite(viewport.width) || viewport.width <= 0) return 1;
  if (!Number.isFinite(viewport.height) || viewport.height <= 0) return 1;
  if (!Number.isFinite(content.width) || content.width <= 0) return 1;
  if (!Number.isFinite(content.height) || content.height <= 0) return 1;

  const scaleW = viewport.width / content.width;
  const scaleH = viewport.height / content.height;

  if (fitMode === "page") {
    const scale = Math.min(scaleW, scaleH);
    return clampScale(scale);
  }

  return clampScale(scaleW);
}

function clampScale(scale: number): number {
  if (!Number.isFinite(scale) || scale <= 0) return 1;
  // Guardrail: evitar valores extremos; el zoom plugin ya limita por config,
  // pero esto previene NaN/Infinity y casos patológicos.
  return Math.min(Math.max(scale, 0.1), 4);
}

