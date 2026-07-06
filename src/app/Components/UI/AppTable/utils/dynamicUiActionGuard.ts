import type { DynamicUiUnknownRecord, UiActionDto } from "../types/dynamicUiTable.types";
import type {
  DynamicUiActionAvailabilityResult,
  DynamicUiActionContext,
  DynamicUiExecutableAction,
} from "../types/dynamicUiTableAction.types";

const pickString = (...values: Array<string | null | undefined>): string | undefined =>
  values.find((value) => typeof value === "string" && value.trim().length > 0)?.trim();

const pickStringArray = (...values: Array<string[] | null | undefined>): string[] => {
  const match = values.find((value) => Array.isArray(value));
  return match ? [...match] : [];
};

const pickRecord = (...values: Array<DynamicUiUnknownRecord | null | undefined>): DynamicUiUnknownRecord | undefined => {
  const match = values.find((value) => value != null);
  return match ? { ...match } : undefined;
};

const isUiActionDto = (action: DynamicUiExecutableAction): action is UiActionDto =>
  "RequiredClaimsAny" in action ||
  "requiredClaimsAny" in action ||
  "ClaimKey" in action ||
  "claimKey" in action ||
  "Rules" in action ||
  "rules" in action;

const getRequiredClaimsAny = (action: DynamicUiExecutableAction): string[] =>
  pickStringArray(
    isUiActionDto(action) ? action.requiredClaimsAny : action.requiredClaimsAny,
    isUiActionDto(action) ? action.RequiredClaimsAny : undefined,
  );

const getRequiredClaimsAll = (action: DynamicUiExecutableAction): string[] =>
  pickStringArray(
    isUiActionDto(action) ? action.requiredClaimsAll : action.requiredClaimsAll,
    isUiActionDto(action) ? action.RequiredClaimsAll : undefined,
  );

const getClaimKey = (action: DynamicUiExecutableAction): string | undefined =>
  pickString(
    isUiActionDto(action) ? action.claimKey : action.claimKey,
    isUiActionDto(action) ? action.ClaimKey : undefined,
  );

const getRules = (action: DynamicUiExecutableAction): DynamicUiUnknownRecord | undefined =>
  pickRecord(
    isUiActionDto(action) ? action.rules : action.rules,
    isUiActionDto(action) ? action.Rules : undefined,
  );

const evaluateSafeRules = (
  rules: DynamicUiUnknownRecord | undefined,
): Pick<DynamicUiActionAvailabilityResult, "isVisible" | "isEnabled" | "reasons"> => {
  if (!rules) {
    return {
      isVisible: true,
      isEnabled: true,
    };
  }

  const reasons: string[] = [];
  let isVisible = true;
  let isEnabled = true;

  const visible = rules.visible ?? rules.isVisible;
  if (typeof visible === "boolean") {
    isVisible = visible;
  }

  const enabled = rules.enabled ?? rules.isEnabled;
  if (typeof enabled === "boolean") {
    isEnabled = enabled;
  }

  const safeKeys = new Set(["visible", "isVisible", "enabled", "isEnabled"]);
  const unsupportedKeys = Object.keys(rules).filter((key) => !safeKeys.has(key));
  if (unsupportedKeys.length > 0) {
    reasons.push(`Rules not safely evaluated in frontend: ${unsupportedKeys.join(", ")}`);
  }

  return {
    isVisible,
    isEnabled,
    reasons,
  };
};

const isDeleteAction = (action: DynamicUiExecutableAction): boolean => {
  const actionId = "actionId" in action ? action.actionId : action.ActionId;

  return typeof actionId === "string" && actionId.trim().toLowerCase() === "eliminar_item";
};

export const evaluateDynamicUiActionAvailability = (
  action: DynamicUiExecutableAction,
  context: DynamicUiActionContext,
): DynamicUiActionAvailabilityResult => {
  const reasons: string[] = [];
  const userClaims = new Set(context.userClaims ?? []);
  const rowCanDelete = context.row?.meta?.CanDelete;

  const requiredAny = getRequiredClaimsAny(action);
  if (requiredAny.length > 0 && !requiredAny.some((claim) => userClaims.has(claim))) {
    reasons.push(`Missing any required claim: ${requiredAny.join(", ")}`);
  }

  const requiredAll = getRequiredClaimsAll(action);
  const missingAll = requiredAll.filter((claim) => !userClaims.has(claim));
  if (missingAll.length > 0) {
    reasons.push(`Missing required claims: ${missingAll.join(", ")}`);
  }

  const claimKey = getClaimKey(action);
  if (claimKey && !userClaims.has(claimKey)) {
    reasons.push(`Missing claim key: ${claimKey}`);
  }

  const ruleResult = evaluateSafeRules(getRules(action));
  reasons.push(...(ruleResult.reasons ?? []));

  const claimsSatisfied =
    requiredAny.length === 0 || requiredAny.some((claim) => userClaims.has(claim));
  const allClaimsSatisfied = missingAll.length === 0;
  const claimKeySatisfied = !claimKey || userClaims.has(claimKey);

  const deleteAllowed = !isDeleteAction(action) || rowCanDelete !== false;
  if (!deleteAllowed) {
    reasons.push("Delete action disabled by row metadata CanDelete=false");
  }

  const visible = claimsSatisfied && allClaimsSatisfied && claimKeySatisfied && ruleResult.isVisible && deleteAllowed;
  const enabled = visible && ruleResult.isEnabled;

  return {
    isVisible: visible,
    isEnabled: enabled,
    reasons: reasons.length > 0 ? reasons : undefined,
  };
};
