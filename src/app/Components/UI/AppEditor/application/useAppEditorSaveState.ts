import { normalizeEditorHtml } from "./normalizeEditorHtml";
import type { AppEditorSaveStatus } from "../domain/save-state.types";

type UseAppEditorSaveStateOptions = {
  currentValue: string | null | undefined;
  savedValue: string | null | undefined;
};

type UseAppEditorSaveStateResult = {
  normalizedCurrentValue: string;
  normalizedSavedValue: string;
  isDirty: boolean;
  saveStatus: AppEditorSaveStatus;
};

export function useAppEditorSaveState({
  currentValue,
  savedValue,
}: UseAppEditorSaveStateOptions): UseAppEditorSaveStateResult {
  const normalizedCurrentValue = normalizeEditorHtml(currentValue);
  const normalizedSavedValue = normalizeEditorHtml(savedValue);
  const isDirty = normalizedCurrentValue !== normalizedSavedValue;

  return {
    normalizedCurrentValue,
    normalizedSavedValue,
    isDirty,
    saveStatus: isDirty ? "dirty" : "idle",
  };
}
