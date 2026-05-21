import { useMemo } from "react";
import type { AppTreeTableRow } from "../types";
import { flattenTree } from "../adapters/flattenTree";

export function useTreeVisibleRows(input: {
  rows: AppTreeTableRow[];
  expandedIds: ReadonlySet<string>;
}) {
  const { rows, expandedIds } = input;

  return useMemo(() => flattenTree(rows, expandedIds), [rows, expandedIds]);
}

