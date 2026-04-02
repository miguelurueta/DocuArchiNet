import type { DynamicUiUnknownRecord, UiActionDto } from "../types/dynamicUiTable.types";
import type {
  DynamicUiExecutableAction,
  DynamicUiResolvedPresentation,
} from "../types/dynamicUiTableAction.types";

const KNOWN_PRESENTATIONS = new Set([
  "button",
  "menu_item",
  "icon",
  "icon_button",
]);

const pickString = (...values: Array<string | null | undefined>): string | undefined =>
  values.find((value) => typeof value === "string" && value.trim().length > 0)?.trim();

const pickRecord = (...values: Array<DynamicUiUnknownRecord | null | undefined>): DynamicUiUnknownRecord | undefined => {
  const match = values.find((value) => value != null);
  return match ? { ...match } : undefined;
};

const isUiActionDto = (action: DynamicUiExecutableAction): action is UiActionDto =>
  "Presentation" in action || "presentation" in action || "Metadata" in action || "metadata" in action;

export const resolveDynamicUiActionPresentation = (
  action: DynamicUiExecutableAction,
): DynamicUiResolvedPresentation => {
  const rawValue =
    pickString(
      isUiActionDto(action) ? action.presentation : action.presentation,
      isUiActionDto(action) ? action.Presentation : undefined,
    ) ?? "default";

  const kind = rawValue.toLowerCase();

  return {
    kind,
    rawValue,
    isKnown: KNOWN_PRESENTATIONS.has(kind),
    config: pickRecord(
      isUiActionDto(action) ? action.metadata : action.metadata,
      isUiActionDto(action) ? action.Metadata : undefined,
    ),
  };
};
