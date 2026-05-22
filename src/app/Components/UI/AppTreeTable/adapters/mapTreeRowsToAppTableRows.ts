import type { AppTableRow } from "../../AppTable/AppTable.types";
import type { AppTreeTableRow } from "../types";
import type { TreeTableRow } from "./flattenTree";

export type AppTreeTableAppTableRow = AppTableRow & {
  __rowId: string;
  id: string;
  __tree: {
    level: number;
    hasChildren: boolean;
    expanded: boolean;
    loadingChildren: boolean;
    node: AppTreeTableRow;
  };
};

export function mapTreeRowsToAppTableRows(input: {
  rows: TreeTableRow[];
  columns: string[];
  loadingChildrenIds: ReadonlySet<string>;
}): AppTreeTableAppTableRow[] {
  const { rows, columns, loadingChildrenIds } = input;

  return rows.map((row) => {
    const values: Record<string, unknown> = {};
    for (const column of columns) {
      values[column] = row.originalNode.values?.[column];
    }

    const loadingChildren = loadingChildrenIds.has(row.id);

    return {
      ...values,
      // Internal stable id used by AppTreeTable/AppTable. It must never be overwritten
      // by backend data fields like `Values.id`.
      __rowId: row.id,
      id: row.id,
      __tree: {
        level: row.level,
        hasChildren: row.hasChildren,
        expanded: row.expanded,
        loadingChildren,
        node: row.originalNode,
      },
    };
  });
}

