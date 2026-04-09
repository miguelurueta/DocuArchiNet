export const ACTION_COLUMN_FIELD = "acciones";
export const SELECTION_COLUMN_FIELD = "ag-Grid-SelectionColumn";

export const isInteractiveElement = (target: EventTarget | null): target is HTMLElement => {
  if (!(target instanceof HTMLElement)) {
    return false;
  }

  return Boolean(
    target.closest("button, a, input, textarea, select, [role=\"button\"], [role=\"menuitem\"]"),
  );
};

export const isNavigableField = (field?: string | null) =>
  Boolean(field && field !== ACTION_COLUMN_FIELD && field !== SELECTION_COLUMN_FIELD);

export const isRowClickTooltipEnabled = (
  rowClickAffordance: boolean,
  rowClickTooltip: string | undefined,
) => rowClickAffordance && typeof rowClickTooltip === "string" && rowClickTooltip.trim().length > 0;
