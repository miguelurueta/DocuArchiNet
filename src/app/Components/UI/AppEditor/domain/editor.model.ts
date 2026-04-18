export const APP_EDITOR_EMPTY_DOCUMENT = "<p></p>";

export function normalizeEditorValue(value?: string | null) {
  const trimmed = value?.trim();
  return trimmed && trimmed.length > 0 ? trimmed : APP_EDITOR_EMPTY_DOCUMENT;
}

export function clampSelection(selection: number, max: number) {
  return Math.max(0, Math.min(selection, max));
}
