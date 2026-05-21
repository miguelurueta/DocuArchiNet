import { useCallback, useEffect, useMemo, useState } from "react";
import styles from "./AppTreeTable.module.css";
import type {
  AppTreeTableLoadChildrenResult,
  AppTreeTableLoadResult,
  AppTreeTableProps,
  AppTreeTableRow,
} from "./types";

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
    const hasChildren = row.hasChildren ?? children.length > 0;
    result.push({ row, depth, hasChildren });
    if (hasChildren && expanded.has(row.id)) {
      result.push(...flattenRows(children, expanded, depth + 1));
    }
  }
  return result;
}

const formatCellValue = (value: unknown): string => {
  if (value === null || value === undefined) return "";
  if (typeof value === "string") return value;
  if (typeof value === "number") return String(value);
  if (typeof value === "boolean") return value ? "Sí" : "No";
  return String(value);
};

const inferColumnsFromRows = (rows: AppTreeTableRow[]): string[] => {
  const first = rows.find((row) => row.values && Object.keys(row.values).length > 0);
  if (!first?.values) return [];
  return Object.keys(first.values);
};

const updateRowById = (
  rows: AppTreeTableRow[],
  rowId: string,
  updater: (row: AppTreeTableRow) => AppTreeTableRow,
): AppTreeTableRow[] => {
  let touched = false;
  const next = rows.map((row) => {
    if (row.id === rowId) {
      touched = true;
      return updater(row);
    }

    const children = row.children ?? [];
    if (children.length === 0) return row;

    const nextChildren = updateRowById(children, rowId, updater);
    if (nextChildren === children) return row;
    touched = true;
    return { ...row, children: nextChildren };
  });

  return touched ? next : rows;
};

export function AppTreeTable({
  rows,
  load,
  loadChildren,
  onSelectRow,
  columns,
  emptyMessage = "Sin registros.",
  isRetryEnabled = true,
  className,
}: AppTreeTableProps) {
  const [expanded, setExpanded] = useState<Set<string>>(() => new Set());
  const [loadingChildren, setLoadingChildren] = useState<Set<string>>(() => new Set());
  const [state, setState] = useState<
    | { status: "idle" }
    | { status: "loading" }
    | { status: "error"; message: string }
    | { status: "ready"; rows: AppTreeTableRow[] }
  >({ status: "idle" });

  const retryLoad = useCallback(async () => {
    if (!load) return;
    setState({ status: "loading" });
    try {
      const result: AppTreeTableLoadResult = await load();
      if (result.ok) setState({ status: "ready", rows: result.rows });
      else setState({ status: "error", message: result.message });
    } catch {
      setState({ status: "error", message: "No fue posible cargar el listado." });
    }
  }, [load]);

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
  const resolvedColumns = useMemo(
    () => (columns && columns.length > 0 ? columns : inferColumnsFromRows(resolvedRows)),
    [columns, resolvedRows],
  );
  const flattened = useMemo(
    () => flattenRows(resolvedRows, expanded),
    [resolvedRows, expanded],
  );

  const rootClassName = useMemo(
    () => [styles.root, className].filter(Boolean).join(" "),
    [className],
  );

  if (state.status === "loading") {
    return (
      <div className={rootClassName}>
        <div className={styles.state}>Cargando…</div>
      </div>
    );
  }

  if (state.status === "error") {
    return (
      <div className={rootClassName}>
        <div className={styles.state}>
          <div>Error: {state.message}</div>
          {load && isRetryEnabled ? (
            <button type="button" className={styles.retry} onClick={retryLoad}>
              Reintentar
            </button>
          ) : null}
        </div>
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
      {resolvedColumns.length > 0 ? (
        <div className={styles.header} aria-hidden="true">
          <span className={styles.togglePlaceholder} />
          <div className={styles.columnsRow}>
            {resolvedColumns.map((column) => (
              <div key={column} className={styles.cell} title={column}>
                {column}
              </div>
            ))}
          </div>
        </div>
      ) : null}

      {flattened.map(({ row, depth, hasChildren }) => {
        const isExpanded = expanded.has(row.id);
        const isLoadingChildren = loadingChildren.has(row.id);
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

                  const needsChildren =
                    !!loadChildren &&
                    (row.children === undefined || row.children.length === 0) &&
                    (row.hasChildren ?? false) &&
                    !loadingChildren.has(row.id);

                  if (!needsChildren) return;

                  setLoadingChildren((prev) => new Set(prev).add(row.id));
                  loadChildren(row)
                    .then((result: AppTreeTableLoadChildrenResult) => {
                      if (!result.ok) return;
                      setState((prevState) => {
                        const baseRows =
                          prevState.status === "ready" ? prevState.rows : resolvedRows;
                        const nextRows = updateRowById(baseRows, row.id, (target) => ({
                          ...target,
                          children: result.rows,
                          hasChildren: result.rows.length > 0,
                        }));
                        return { status: "ready", rows: nextRows };
                      });
                    })
                    .finally(() => {
                      setLoadingChildren((prev) => {
                        const next = new Set(prev);
                        next.delete(row.id);
                        return next;
                      });
                    });
                }}
              >
                {isLoadingChildren ? "…" : isExpanded ? "–" : "+"}
              </button>
            ) : (
              <span className={styles.togglePlaceholder} aria-hidden="true" />
            )}

            {resolvedColumns.length > 0 ? (
              <div className={styles.columnsRow}>
                {resolvedColumns.map((column, index) => {
                  const raw = row.values?.[column];
                  const text = formatCellValue(raw);
                  if (index === 0) {
                    return (
                      <div key={column} className={styles.cell} title={text || row.label}>
                        <span className={styles.labelCell}>
                          <button
                            type="button"
                            className={styles.rowButton}
                            onClick={() => onSelectRow?.(row.id)}
                          >
                            {text || row.label}
                          </button>
                        </span>
                      </div>
                    );
                  }

                  return (
                    <div key={column} className={styles.cell} title={text}>
                      {text}
                    </div>
                  );
                })}
              </div>
            ) : (
              <button
                type="button"
                className={styles.rowButton}
                onClick={() => onSelectRow?.(row.id)}
              >
                {row.label}
              </button>
            )}
          </div>
        );
      })}
    </div>
  );
}

