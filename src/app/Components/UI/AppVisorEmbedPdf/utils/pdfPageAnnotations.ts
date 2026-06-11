export type AnnotationPagesState = Record<string, unknown>;

export function getAnnotatedPageNumbers(pages: AnnotationPagesState | null | undefined): number[] {
  if (!pages) return [];

  const pageNumbers = new Set<number>();
  for (const [pageIndexRaw, annotationIds] of Object.entries(pages)) {
    if (!Array.isArray(annotationIds) || annotationIds.length === 0) continue;

    const pageIndex = Number.parseInt(pageIndexRaw, 10);
    if (!Number.isFinite(pageIndex) || pageIndex < 0) continue;

    pageNumbers.add(pageIndex + 1);
  }

  return Array.from(pageNumbers).sort((left, right) => left - right);
}
