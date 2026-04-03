import type {
  AppGridCellAction,
  DynamicUiUnknownRecord,
  UiActionDto,
  UiCellActionDto,
} from "../types/dynamicUiTable.types";

const pickString = (...values: Array<string | null | undefined>): string | undefined =>
  values.find((value) => typeof value === "string" && value.trim().length > 0)?.trim();

const pickBoolean = (...values: Array<boolean | null | undefined>): boolean | undefined =>
  values.find((value) => typeof value === "boolean");

const pickStringArray = (...values: Array<string[] | null | undefined>): string[] | undefined => {
  const match = values.find((value) => Array.isArray(value));
  return match ? [...match] : undefined;
};

const pickRecord = (
  ...values: Array<DynamicUiUnknownRecord | null | undefined>
): DynamicUiUnknownRecord | undefined => {
  const match = values.find((value) => value != null);
  return match ? { ...match } : undefined;
};

const resolveActionId = (action: UiActionDto | UiCellActionDto, index: number): string =>
  pickString(action.actionId, action.ActionId, action.label, action.Label) ?? `action-${index}`;

const unwrapAction = (action: UiActionDto | UiCellActionDto): UiActionDto | UiCellActionDto =>
  action.action ?? action.Action ?? action;

const mapDynamicUiAction = (
  rawAction: UiActionDto | UiCellActionDto,
  index: number,
): AppGridCellAction => {
  const action = unwrapAction(rawAction);
  const children = action.children ?? action.Children;

  return {
    actionId: resolveActionId(action, index),
    label: pickString(action.label, action.Label) ?? "Sin etiqueta",
    placement: pickString(action.placement, action.Placement) ?? "row",
    presentation: pickString(action.presentation, action.Presentation) ?? "default",
    behavior: pickString(action.behavior, action.Behavior) ?? "noop",
    isDivider: pickBoolean(action.isDivider, action.IsDivider) ?? false,
    behaviorConfig: pickRecord(action.behaviorConfig, action.BehaviorConfig),
    request: pickRecord(action.request, action.Request),
    icon: pickString(action.icon, action.Icon),
    tone: pickString(action.tone, action.Tone),
    requiresConfirm: pickBoolean(action.requiresConfirm, action.RequiresConfirm),
    confirmTitle: pickString(action.confirmTitle, action.ConfirmTitle),
    confirmMessage: pickString(action.confirmMessage, action.ConfirmMessage),
    requiredClaimsAny: pickStringArray(action.requiredClaimsAny, action.RequiredClaimsAny),
    requiredClaimsAll: pickStringArray(action.requiredClaimsAll, action.RequiredClaimsAll),
    claimKey: pickString(action.claimKey, action.ClaimKey),
    rules: pickRecord(action.rules, action.Rules),
    payload: pickRecord(action.payload, action.Payload),
    metadata: pickRecord(action.metadata, action.Metadata),
    children: Array.isArray(children)
      ? children.map((child, childIndex) => mapDynamicUiAction(child, childIndex))
      : undefined,
  };
};

export const mapDynamicUiActions = (
  actions: ReadonlyArray<UiActionDto | UiCellActionDto> | null | undefined,
): AppGridCellAction[] => {
  if (!actions?.length) {
    return [];
  }

  return actions.map((rawAction, index) => mapDynamicUiAction(rawAction, index));
};

export const groupCellActionsByColumnKey = (
  actions: ReadonlyArray<UiCellActionDto> | null | undefined,
): Record<string, AppGridCellAction[]> => {
  if (!actions?.length) {
    return {};
  }

  return actions.reduce<Record<string, AppGridCellAction[]>>((acc, action) => {
    const columnKey = pickString(action.columnKey, action.ColumnKey);
    if (!columnKey) {
      return acc;
    }

    const mappedAction = mapDynamicUiActions([action])[0];
    if (!mappedAction) {
      return acc;
    }

    const current = acc[columnKey] ?? [];
    acc[columnKey] = [...current, mappedAction];
    return acc;
  }, {});
};
