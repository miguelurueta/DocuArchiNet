import { useEffect, useMemo, useState } from "react";
import styles from "./AppTreeTable.module.css";
import type { AppTreeTableLoadResult, AppTreeTableProps, AppTreeTableRow } from "./types";

type FlattenedRow = {
  row: AppTreeTableRow;
  depth: number;
  hasChildren: boolean;
};

function flattenRows(
  rows: AppTreeTableRow[],
  expanded: ReadonlySet<string>,
  depth = 0,
): FlattenedRow[] {
  const result: FlattenedRow[] = [];
  for (const row of rows) {
    const children = row.children ?? [];
    const hasChildren = children.length > 0;
    result.push({ row, depth, hasChildren });
    if (hasChildren && expanded.has(row.id)) {
      result.push(...flattenRows(children, expanded, depth + 1));
    }
  }
  return result;
}

export function AppTreeTable({
  rows,
  load,
  onSelectRow,
  emptyMessage = "Sin registros.",
  className,
}: AppTreeTableProps) {
  const [expanded, setExpanded] = useState<Set<string>>(() => new Set());
  const [state, setState] = useState<
    | { status: "idle" }
    | { status: "loading" }
    | { status: "error"; message: string }
    | { status: "ready"; rows: AppTreeTableRow[] }
  >({ status: "idle" });

  useEffect(() => {
    let cancelled = false;
    if (!load) return;

    setState({ status: "loading" });
    load()
      .then((result: AppTreeTableLoadResult) => {
        if (cancelled) return;
        if (result.ok) setState({ status: "ready", rows: result.rows });
        else setState({ status: "error", message: result.message });
      })
      .catch(() => {
        if (cancelled) return;
        setState({ status: "error", message: "No fue posible cargar el listado." });
      });

    return () => {
      cancelled = true;
    };
  }, [load]);

  const resolvedRows = state.status === "ready" ? state.rows : (rows ?? []);
  const flattened = useMemo(
    () => flattenRows(resolvedRows, expanded),
    [resolvedRows, expanded],
  );

  const rootClassName = useMemo(
    () => [styles.root, className].filter(Boolean).join(" "),
    [className],
  );

  if (state.status === "loading") {
    return <div className={rootClassName}><div className={styles.state}>Cargando…</div></div>;
  }

  if (state.status === "error") {
    return (
      <div className={rootClassName}>
        <div className={styles.state}>Error: {state.message}</div>
      </div>
    );
  }

  if (flattened.length === 0) {
    return (
      <div className={rootClassName}>
        <div className={styles.state}>{emptyMessage}</div>
      </div>
    );
  }

  return (
    <div className={rootClassName} role="tree" aria-label="Listado en árbol">
      {flattened.map(({ row, depth, hasChildren }) => {
        const isExpanded = expanded.has(row.id);
        return (
          <div
            key={row.id}
            className={styles.row}
            role="treeitem"
            aria-level={depth + 1}
            aria-expanded={hasChildren ? isExpanded : undefined}
            style={{ paddingLeft: 8 + depth * 16 }}
          >
            {hasChildren ? (
              <button
                type="button"
                className={styles.toggle}
                aria-label={isExpanded ? `Colapsar ${row.label}` : `Expandir ${row.label}`}
                onClick={() => {
                  setExpanded((prev) => {
                    const next = new Set(prev);
                    if (next.has(row.id)) next.delete(row.id);
                    else next.add(row.id);
                    return next;
                  });
                }}
              >
                {isExpanded ? "–" : "+"}
              </button>
            ) : (
              <span className={styles.togglePlaceholder} aria-hidden="true" />
            )}
            <button
              type="button"
              className={styles.rowButton}
              onClick={() => onSelectRow?.(row.id)}
            >
              {row.label}
            </button>
          </div>
        );
      })}
    </div>
  );
}

