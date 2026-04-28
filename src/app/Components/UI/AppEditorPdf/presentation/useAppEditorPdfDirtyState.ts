import { useMemo } from "react";
import { normalizeEditorHtml, useAppEditorSaveState } from "../../AppEditor";

export type UseAppEditorPdfDirtyStateOptions = {
  currentValue: string;
  savedValue: string;
};

/**
 * FE-17: Dirty state para AppEditorPdf.
 * Normaliza el HTML antes de comparar para evitar falsos positivos por cambios triviales.
 */
export function useAppEditorPdfDirtyState({
  currentValue,
  savedValue,
}: UseAppEditorPdfDirtyStateOptions) {
  const normalizedCurrent = useMemo(
    () => normalizeEditorHtml(currentValue ?? ""),
    [currentValue],
  );
  const normalizedSaved = useMemo(
    () => normalizeEditorHtml(savedValue ?? ""),
    [savedValue],
  );

  const state = useAppEditorSaveState({
    currentValue: normalizedCurrent,
    savedValue: normalizedSaved,
  });

  return {
    ...state,
    isDirty: normalizedCurrent !== normalizedSaved,
  };
}
