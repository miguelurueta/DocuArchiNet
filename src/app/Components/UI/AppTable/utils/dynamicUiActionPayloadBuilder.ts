import type { DynamicUiUnknownRecord, UiActionDto } from "../types/dynamicUiTable.types";
import type {
  DynamicUiActionContext,
  DynamicUiExecutableAction,
} from "../types/dynamicUiTableAction.types";

type PayloadFieldsMap = Record<string, string>;

const pickString = (...values: Array<string | null | undefined>): string | undefined =>
  values.find((value) => typeof value === "string" && value.trim().length > 0)?.trim();

const pickRecord = (...values: Array<DynamicUiUnknownRecord | null | undefined>): DynamicUiUnknownRecord => {
  const match = values.find((value) => value != null);
  return match ? { ...match } : {};
};

const isUiActionDto = (action: DynamicUiExecutableAction): action is UiActionDto =>
  "ActionId" in action ||
  "actionId" in action ||
  "Behavior" in action ||
  "behavior" in action ||
  "Request" in action ||
  "request" in action;

const getActionRequest = (action: DynamicUiExecutableAction): DynamicUiUnknownRecord =>
  isUiActionDto(action)
    ? pickRecord(action.request, action.Request)
    : pickRecord(action.request);

const getActionPayload = (action: DynamicUiExecutableAction): DynamicUiUnknownRecord =>
  isUiActionDto(action)
    ? pickRecord(action.payload, action.Payload)
    : pickRecord(action.payload);

const getRequestFieldMap = (request: DynamicUiUnknownRecord): PayloadFieldsMap => {
  const raw = request.PayloadFields ?? request.payloadFields;
  if (!raw || typeof raw !== "object" || Array.isArray(raw)) {
    return {};
  }

  return Object.entries(raw as Record<string, unknown>).reduce<PayloadFieldsMap>((acc, [key, value]) => {
    if (typeof value === "string" && value.trim().length > 0) {
      acc[key] = value.trim();
    }
    return acc;
  }, {});
};

const getRequestRowIdField = (request: DynamicUiUnknownRecord): string | undefined =>
  pickString(
    typeof request.RowIdField === "string" ? request.RowIdField : undefined,
    typeof request.rowIdField === "string" ? request.rowIdField : undefined,
  );

const omitControlKeys = (request: DynamicUiUnknownRecord): DynamicUiUnknownRecord => {
  const clone = { ...request };
  delete clone.RowIdField;
  delete clone.rowIdField;
  delete clone.PayloadFields;
  delete clone.payloadFields;
  return clone;
};

const pickFromRow = (fieldName: string, context: DynamicUiActionContext): unknown => {
  const directRowValue = context.row?.data[fieldName];
  if (directRowValue !== undefined) {
    return directRowValue;
  }

  const selectedRows = context.selectedRows ?? [];
  const values = selectedRows
    .map((row) => row.data[fieldName])
    .filter((value) => value !== undefined);

  if (values.length === 0) {
    return undefined;
  }

  return values.length === 1 ? values[0] : values;
};

export const buildDynamicUiActionPayload = (
  action: DynamicUiExecutableAction,
  context: DynamicUiActionContext,
  manualPayload?: Record<string, unknown>,
): Record<string, unknown> => {
  const request = getActionRequest(action);
  const payloadFields = getRequestFieldMap(request);
  const rowIdField = getRequestRowIdField(request);
  const selectedRows = context.selectedRows ?? [];

  const derivedPayload = Object.entries(payloadFields).reduce<Record<string, unknown>>((acc, [targetKey, sourceField]) => {
    const value = pickFromRow(sourceField, context);
    if (value !== undefined) {
      acc[targetKey] = value;
    }
    return acc;
  }, {});

  if (rowIdField) {
    const rowIdValue = pickFromRow(rowIdField, context);
    if (rowIdValue !== undefined) {
      derivedPayload.rowId = rowIdValue;
    }
  } else if (context.row?.id) {
    derivedPayload.rowId = context.row.id;
  }

  if (selectedRows.length > 0) {
    derivedPayload.selectedRowIds = selectedRows.map((row) => row.id);
  }

  return {
    ...derivedPayload,
    ...omitControlKeys(request),
    ...getActionPayload(action),
    ...(manualPayload ?? {}),
  };
};
