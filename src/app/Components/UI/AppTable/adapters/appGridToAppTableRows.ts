import type { AppGridRow, AppTableRow } from "../types/dynamicUiTable.types";

export const mapAppGridRowsToAppTableRows = <T extends AppTableRow = AppTableRow>(
  rows: ReadonlyArray<AppGridRow> | null | undefined,
): T[] => {
  if (!rows?.length) {
    return [];
  }

  return rows.map((row) => ({
    id: row.id,
    ...row.data,
  })) as unknown as T[];
};
