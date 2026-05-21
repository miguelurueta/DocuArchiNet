import type { AppTableRow } from "../../AppTable/AppTable.types";
import type { AppTreeTableRow } from "../types";
import type { TreeTableRow } from "./flattenTree";

export type AppTreeTableAppTableRow = AppTableRow & {
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
      id: row.id,
      ...values,
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

