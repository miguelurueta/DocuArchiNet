import type { AppTreeTableRow } from "../types";

export type TreeTableRow = {
  id: string;
  label: string;
  level: number;
  parentId: string | null;
  expanded: boolean;
  hasChildren: boolean;
  selectable: boolean;
  originalNode: AppTreeTableRow;
};

export function flattenTree(
  rows: AppTreeTableRow[],
  expandedIds: ReadonlySet<string>,
  level = 0,
  parentId: string | null = null,
): TreeTableRow[] {
  const result: TreeTableRow[] = [];

  for (const row of rows) {
    const children = row.children ?? [];
    const hasChildren = row.hasChildren ?? children.length > 0;
    const id = row.id;

    const isExpanded = hasChildren && expandedIds.has(id);

    result.push({
      id,
      label: row.label,
      level,
      parentId,
      expanded: isExpanded,
      hasChildren,
      selectable: true,
      originalNode: row,
    });

    if (isExpanded && children.length > 0) {
      result.push(...flattenTree(children, expandedIds, level + 1, id));
    }
  }

  return result;
}

