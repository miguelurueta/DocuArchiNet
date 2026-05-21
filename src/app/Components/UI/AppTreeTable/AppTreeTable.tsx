import type { ColDef } from "ag-grid-community";
import { useCallback, useEffect, useMemo, useState } from "react";
import AppTable from "../AppTable/AppTable";
import styles from "./AppTreeTable.module.css";
import { mapTreeRowsToAppTableRows } from "./adapters/mapTreeRowsToAppTableRows";
import { resolveTreeIndentation } from "./adapters/resolveTreeIndentation";
import { useTreeExpansionState } from "./hooks/useTreeExpansionState";
import { useTreeVisibleRows } from "./hooks/useTreeVisibleRows";
import type {
  AppTreeTableLoadChildrenResult,
  AppTreeTableLoadResult,
  AppTreeTableProps,
  AppTreeTableRow,
} from "./types";

const formatCellValue = (value: unknown): string => {
  if (value === null || value === undefined) return "";
  if (typeof value === "string") return value;
  if (typeof value === "number") return String(value);
  if (typeof value === "boolean") return value ? "Si" : "No";
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
  const expansion = useTreeExpansionState();
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

  const flattened = useTreeVisibleRows({
    rows: resolvedRows,
    expandedIds: expansion.expandedIds,
  });

  const appTableRows = useMemo(
    () =>
      mapTreeRowsToAppTableRows({
        rows: flattened,
        columns: resolvedColumns,
        loadingChildrenIds: loadingChildren,
      }),
    [flattened, loadingChildren, resolvedColumns],
  );

  const rootClassName = useMemo(
    () => [styles.root, className].filter(Boolean).join(" "),
    [className],
  );

  const tableColumns: ColDef<(typeof appTableRows)[number]>[] = useMemo(() => {
    const columnsToRender = resolvedColumns.length > 0 ? resolvedColumns : ["Label"];

    return columnsToRender.map((column, index) => ({
      headerName: column,
      field: resolvedColumns.length > 0 ? column : "__label",
      flex: index === 0 ? 1 : undefined,
      minWidth: index === 0 ? 220 : 180,
      cellRenderer: (params: { data?: (typeof appTableRows)[number] }) => {
        const row = params.data as (typeof appTableRows)[number] | undefined;
        if (!row) return null;

        if (index !== 0) {
          const raw = resolvedColumns.length > 0 ? row[column] : undefined;
          return formatCellValue(raw);
        }

        const tree = row.__tree;
        const indent = resolveTreeIndentation(tree.level);
        const firstValue =
          resolvedColumns.length > 0 ? formatCellValue(row[resolvedColumns[0]]) : "";
        const labelText = firstValue || tree.node.label;

        return (
          <div className={styles.treeCell} style={{ paddingLeft: indent }}>
            {tree.hasChildren ? (
              <button
                type="button"
                className={styles.treeToggle}
                aria-label={
                  tree.expanded ? `Colapsar ${tree.node.label}` : `Expandir ${tree.node.label}`
                }
                disabled={tree.loadingChildren}
                onClick={(event) => {
                  event.preventDefault();
                  event.stopPropagation();

                  expansion.toggleExpanded(tree.node.id);

                  const needsChildren =
                    !!loadChildren &&
                    (tree.node.children === undefined || tree.node.children.length === 0) &&
                    (tree.node.hasChildren ?? false) &&
                    !loadingChildren.has(tree.node.id);

                  if (!needsChildren) return;

                  setLoadingChildren((prev) => new Set(prev).add(tree.node.id));
                  loadChildren(tree.node)
                    .then((result: AppTreeTableLoadChildrenResult) => {
                      if (!result.ok) return;
                      setState((prevState) => {
                        const baseRows = prevState.status === "ready" ? prevState.rows : resolvedRows;
                        const nextRows = updateRowById(baseRows, tree.node.id, (target) => ({
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
                        next.delete(tree.node.id);
                        return next;
                      });
                    });
                }}
              >
                {tree.loadingChildren ? "..." : tree.expanded ? "-" : "+"}
              </button>
            ) : (
              <span className={styles.togglePlaceholder} aria-hidden="true" />
            )}

            <button
              type="button"
              className={styles.treeLabel}
              onClick={(event) => {
                event.preventDefault();
                event.stopPropagation();
                onSelectRow?.(tree.node.id);
              }}
            >
              {labelText}
            </button>
          </div>
        );
      },
    }));
  }, [expansion, loadChildren, loadingChildren, onSelectRow, resolvedColumns, resolvedRows]);

  if (state.status === "loading") {
    return (
      <div className={rootClassName}>
        <div className={styles.state}>Cargando...</div>
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
    <div className={rootClassName} role="tree" aria-label="Listado en Ã¡rbol">
      <AppTable
        rows={appTableRows}
        columns={tableColumns}
        domLayout="autoHeight"
        layoutMode="content"
        rowSelection="single"
        suppressRowClickSelection
        suppressCellFocus={false}
        rowClickAffordance={false}
        getRowId={(row) => String(row.id)}
        onRowClicked={(row) => onSelectRow?.(row.id)}
        onActionTriggered={(params) => {
          const tree = (params.row as (typeof appTableRows)[number]).__tree;
          if (!tree) return;

          if (params.actionId === "toggle_expand") {
            expansion.toggleExpanded(tree.node.id);
            return;
          }

          if (params.actionId === "select_row") {
            onSelectRow?.(tree.node.id);
          }
        }}
      />
    </div>
  );
}
