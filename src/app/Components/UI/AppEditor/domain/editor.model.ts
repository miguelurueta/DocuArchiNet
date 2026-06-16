import type { Node as ProseMirrorNode } from "@tiptap/pm/model";
import { TextSelection } from "@tiptap/pm/state";

export const APP_EDITOR_EMPTY_DOCUMENT = "<p></p>";

export function normalizeEditorValue(value?: string | null) {
  const trimmed = value?.trim();
  return trimmed && trimmed.length > 0 ? trimmed : APP_EDITOR_EMPTY_DOCUMENT;
}

export function clampSelection(selection: number, max: number) {
  return Math.max(0, Math.min(selection, max));
}

export type TextSelectionRange = {
  from: number;
  to: number;
};

function clampSafe(min: number, max: number, value: number) {
  return Math.max(min, Math.min(value, max));
}

function buildCandidatePositions(
  value: number,
  max: number,
  min = 0,
  radius = 6,
): number[] {
  const candidates = new Set<number>([
    clampSafe(min, max, value),
  ]);
  const orderedCandidates: number[] = [clampSafe(min, max, value)];

  for (let offset = 1; offset <= radius; offset += 1) {
    const forwardCandidate = clampSafe(min, max, value + offset);
    if (!candidates.has(forwardCandidate)) {
      candidates.add(forwardCandidate);
      orderedCandidates.push(forwardCandidate);
    }

    const backwardCandidate = clampSafe(min, max, value - offset);
    if (!candidates.has(backwardCandidate)) {
      candidates.add(backwardCandidate);
      orderedCandidates.push(backwardCandidate);
    }
  }

  [min, max].forEach((fallbackPosition) => {
    const clampedFallback = clampSafe(min, max, fallbackPosition);
    if (!candidates.has(clampedFallback)) {
      candidates.add(clampedFallback);
      orderedCandidates.push(clampedFallback);
    }
  });

  return orderedCandidates;
}

function getSafeInlinePosition(
  doc: ProseMirrorNode,
  position: number,
  max: number,
  min: number,
  candidates: number[],
): number | null {
  const clamped = clampSafe(min, max, position);

  for (const candidate of candidates) {
    try {
      const resolved = doc.resolve(candidate);
      if (resolved.parent.inlineContent) {
        return candidate;
      }
    } catch {
      continue;
    }
  }

  let nearest: { distance: number; position: number } | undefined;

  doc.descendants((node, offset) => {
    if (!node.isTextblock) {
      return;
    }

    const textblockStart = offset + 1;
    const textblockEnd = offset + node.nodeSize - 1;
    const candidate = Math.max(
      textblockStart,
      Math.min(clamped, textblockEnd),
    );

    if (candidate < textblockStart || candidate > textblockEnd) {
      return;
    }

    const distance = Math.abs(clamped - candidate);
    if (nearest == null || distance < nearest.distance) {
      nearest = { distance, position: candidate };
    }

    return false;
  });

  return nearest ? nearest.position : null;
}

function tryCreateTextSelection(
  doc: ProseMirrorNode,
  from: number,
  to: number,
) {
  const min = doc.childCount > 0 ? 1 : 0;
  const max = Math.max(min, doc.content.size - 1);
  const normalizedFrom = clampSafe(min, max, from);
  const normalizedTo = clampSafe(min, max, to);

  try {
    const fromResolved = doc.resolve(normalizedFrom);
    const toResolved = doc.resolve(normalizedTo);
    if (!fromResolved.parent.inlineContent || !toResolved.parent.inlineContent) {
      return null;
    }

    const selection =
      normalizedFrom === normalizedTo
        ? TextSelection.create(doc, normalizedFrom)
        : TextSelection.create(doc, normalizedFrom, normalizedTo);
    return {
      from: selection.from,
      to: selection.to,
    };
  } catch {
    return null;
  }
}

function tryCreateNearSelection(doc: ProseMirrorNode, position: number) {
  const safeMaxPosition = Math.max(0, doc.content.size);
  const clampedPosition = Math.max(0, Math.min(position, safeMaxPosition));

  try {
    const resolved = doc.resolve(clampedPosition);
    const selection = TextSelection.near(resolved, -1);

    return {
      from: selection.from,
      to: selection.to,
    };
  } catch {
    try {
      const resolved = doc.resolve(clampedPosition);
      const selection = TextSelection.near(resolved, 1);

      return {
        from: selection.from,
        to: selection.to,
      };
    } catch {
      try {
        const fallback = TextSelection.create(
          doc,
          Math.max(0, Math.min(clampedPosition, doc.content.size)),
        );

        return {
          from: fallback.from,
          to: fallback.to,
        };
      } catch {
        return null;
      }
    }
  }
}

function resolveSafeSelectionCandidatePair(
  doc: ProseMirrorNode,
  from: number,
  to: number,
  max: number,
  min: number,
) {
  const fromCandidates = buildCandidatePositions(from, max, min);
  const toCandidates = buildCandidatePositions(to, max, min);

  for (let fromOffset = 0; fromOffset < fromCandidates.length; fromOffset += 1) {
    const candidateFrom = fromCandidates[fromOffset];
    for (let toOffset = 0; toOffset < toCandidates.length; toOffset += 1) {
      const candidateTo = toCandidates[toOffset];

      const directSelection = tryCreateTextSelection(doc, candidateFrom, candidateTo);
      if (directSelection) {
        return directSelection;
      }
    }
  }

  return null;
}

function collectTextBlockBoundaryPositions(doc: ProseMirrorNode) {
  const positions = new Set<number>();
  doc.descendants((node, offset) => {
    if (!node.isTextblock) {
      return;
    }

    const textblockStart = offset + 1;
    const textblockEnd = offset + node.nodeSize - 1;

    if (textblockStart <= textblockEnd) {
      positions.add(textblockStart);
      positions.add(textblockEnd);
      positions.add(Math.floor((textblockStart + textblockEnd) / 2));
    }
  });

  return [...positions];
}

export function resolveSafeTextSelectionRange(
  doc: ProseMirrorNode,
  from: number,
  to: number,
): TextSelectionRange | null {
  const hasChildContent = doc.childCount > 0;
  const min = hasChildContent ? 1 : 0;
  const max = Math.max(min, doc.content.size - 1);
  const fromCandidates = buildCandidatePositions(from, max, min);
  const toCandidates = buildCandidatePositions(to, max, min);

  const safeFrom = getSafeInlinePosition(doc, from, max, min, fromCandidates);
  if (safeFrom === null) {
    const textBlockPositions = collectTextBlockBoundaryPositions(doc);
    if (textBlockPositions.length === 0) {
      return null;
    }

    const nearestTextBlock = textBlockPositions.reduce((previous, current) => {
      const currentDistance = Math.abs(current - from);
      const previousDistance = Math.abs(previous - from);
      return currentDistance < previousDistance ? current : previous;
    }, textBlockPositions[0]);

    const fallback = resolveSafeSelectionCandidatePair(
      doc,
      nearestTextBlock,
      to,
      max,
      min,
    );

    if (fallback) {
      return fallback;
    }

    return null;
  }

  const safeTo = getSafeInlinePosition(doc, to, max, min, toCandidates) ?? safeFrom;
  const safeSelection = resolveSafeSelectionCandidatePair(
    doc,
    safeFrom,
    safeTo,
    max,
    min,
  );

  if (safeSelection) {
    return safeSelection;
  }

  const textBlockPositions = collectTextBlockBoundaryPositions(doc);
  if (textBlockPositions.length === 0) {
    return null;
  }

  const nearestTextBlockFrom = textBlockPositions.reduce((previous, current) => {
    const currentDistance = Math.abs(current - safeFrom);
    const previousDistance = Math.abs(previous - safeFrom);
    return currentDistance < previousDistance ? current : previous;
  }, textBlockPositions[0]);
  const nearestTextBlockTo = textBlockPositions.reduce((previous, current) => {
    const currentDistance = Math.abs(current - safeTo);
    const previousDistance = Math.abs(previous - safeTo);
    return currentDistance < previousDistance ? current : previous;
  }, textBlockPositions[0]);

  return (
    resolveSafeSelectionCandidatePair(
      doc,
      nearestTextBlockFrom,
      nearestTextBlockTo,
      max,
      min,
    ) ??
    tryCreateTextSelection(doc, safeFrom, safeFrom) ??
    tryCreateNearSelection(doc, safeFrom) ??
    null
  );
}

export function createSafeTextSelectionFromRange(
  doc: ProseMirrorNode,
  from: number,
  to: number,
) {
  const maxPosition = doc.content.size;
  const safeRange = resolveSafeTextSelectionRange(
    doc,
    Math.max(0, Math.min(from, maxPosition)),
    Math.max(0, Math.min(to, maxPosition)),
  );
  if (!safeRange) {
    return null;
  }

  const safeFrom = safeRange.from;
  const safeTo = safeRange.to;

  const buildSelectionFromRange = (selectionFrom: number, selectionTo: number) => {
    try {
      return safeFrom === selectionTo
        ? TextSelection.create(doc, selectionFrom, selectionTo)
        : TextSelection.create(doc, selectionFrom, selectionTo);
    } catch {
      return null;
    }
  };

  const safeSelection =
    buildSelectionFromRange(safeFrom, safeTo) ??
    buildSelectionFromRange(safeFrom, safeFrom) ??
    buildSelectionFromRange(safeTo, safeTo);

  if (safeSelection) {
    return safeSelection;
  }

  try {
    const nearSelection = TextSelection.near(doc.resolve(safeFrom), -1);
    return TextSelection.create(doc, nearSelection.from, nearSelection.to);
  } catch {
    return null;
  }
}
