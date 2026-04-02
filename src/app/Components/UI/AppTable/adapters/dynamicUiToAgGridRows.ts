import type { AppGridRow, DynamicUiUnknownRecord, UiRowDto } from "../types/dynamicUiTable.types";

const pickRecord = (row: UiRowDto): DynamicUiUnknownRecord => {
  const values = row.values ?? row.Values;
  return values ? { ...values } : {};
};

const pickMeta = (row: UiRowDto): DynamicUiUnknownRecord | undefined => {
  const meta = row.meta ?? row.Meta;
  return meta ? { ...meta } : undefined;
};

const resolveRowId = (row: UiRowDto, index: number): string => {
  const candidate = row.id ?? row.Id ?? row.key ?? row.Key;
  if (candidate == null || candidate === "") {
    return `row-${index}`;
  }
  return String(candidate);
};

export const mapDynamicUiRowsToAppGridRows = (
  rows: ReadonlyArray<UiRowDto> | null | undefined,
): AppGridRow[] => {
  if (!rows?.length) {
    return [];
  }

  return rows.map((row, index) => ({
    id: resolveRowId(row, index),
    data: pickRecord(row),
    meta: pickMeta(row),
  }));
};
