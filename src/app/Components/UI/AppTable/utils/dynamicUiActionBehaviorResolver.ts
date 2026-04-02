import type { DynamicUiUnknownRecord, UiActionDto } from "../types/dynamicUiTable.types";
import type {
  DynamicUiExecutableAction,
  DynamicUiResolvedBehavior,
} from "../types/dynamicUiTableAction.types";

const KNOWN_BEHAVIORS = new Set([
  "api_call",
  "navigate",
  "modal",
  "download",
  "emit",
  "custom",
  "client_event",
]);

const pickString = (...values: Array<string | null | undefined>): string | undefined =>
  values.find((value) => typeof value === "string" && value.trim().length > 0)?.trim();

const pickRecord = (...values: Array<DynamicUiUnknownRecord | null | undefined>): DynamicUiUnknownRecord | undefined => {
  const match = values.find((value) => value != null);
  return match ? { ...match } : undefined;
};

const isUiActionDto = (action: DynamicUiExecutableAction): action is UiActionDto =>
  "Behavior" in action || "behavior" in action || "BehaviorConfig" in action || "behaviorConfig" in action;

export const resolveDynamicUiActionBehavior = (
  action: DynamicUiExecutableAction,
): DynamicUiResolvedBehavior => {
  const rawValue =
    pickString(
      isUiActionDto(action) ? action.behavior : action.behavior,
      isUiActionDto(action) ? action.Behavior : undefined,
    ) ?? "noop";

  const kind = rawValue.toLowerCase();

  return {
    kind,
    rawValue,
    isKnown: KNOWN_BEHAVIORS.has(kind),
    config: pickRecord(
      isUiActionDto(action) ? action.behaviorConfig : action.behaviorConfig,
      isUiActionDto(action) ? action.BehaviorConfig : undefined,
    ),
  };
};
