import { Tooltip } from "antd";
import { AgGridReact } from "ag-grid-react";
import type { CellKeyDownEvent, ColDef, GridReadyEvent } from "ag-grid-community";
import { useEffect, useMemo, useRef, useState } from "react";
import type { AppTablePaginationMode, AppTableProps, AppTableRow } from "../AppTable.types";
import type { AppTableActionCellRendererParams } from "../types/dynamicUiTableAction.types";
import { useAgGridBaseConfig } from "../hooks/useAgGridBaseConfig";
import { useDeferredLoadingVeil } from "../hooks/useDeferredLoadingVeil";
import {
  isInteractiveElement,
  isNavigableField,
  isRowClickTooltipEnabled,
} from "../utils/navigableAffordance";
import styles from "../AppTable.module.css";

const resolveRowId = <T extends AppTableRow>(
  row: T,
  getRowId?: (row: T) => string,
): string => {
  if (getRowId) {
    return getRowId(row);
  }

  const candidate = row.id;
  if (typeof candidate === "string" || typeof candidate === "number") {
    return String(candidate);
  }

  return JSON.stringify(row);
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const normalizeCellClass = (
  value: string | string[] | null | undefined,
): string | undefined => {
  if (typeof value === "string") {
    return value;
  }

  if (Array.isArray(value)) {
    return value.filter((item): item is string => typeof item === "string" && item.length > 0).join(" ");
  }

  return undefined;
};

const getKeyboardEventKey = (event: Event | null | undefined) => {
  if (!event || typeof event !== "object" || !("key" in event)) {
    return null;
  }

  const key = event.key;
  return typeof key === "string" ? key : null;
};

const isNavigableCellEvent = <T extends AppTableRow>(event: {
  colDef?: { field?: string | null };
  event?: Event | null;
  data?: T | null;
}) => {
  if (!event.data || !isNavigableField(event.colDef?.field ?? null)) {
    return false;
  }

  if (isInteractiveElement(event.event?.target ?? null)) {
    return false;
  }

  return true;
};

const resolveQuickFilterText = (
  paginationMode: AppTablePaginationMode | undefined,
  quickFilterText: string | undefined,
) => {
  if (paginationMode === "server") {
    return undefined;
  }

  return quickFilterText;
};

const enrichActionColumns = <T extends AppTableRow>(
  columns: ColDef<T>[],
  onActionTriggered: AppTableProps<T>["onActionTriggered"],
): ColDef<T>[] => {
  if (!onActionTriggered) {
    return columns;
  }

  return columns.map((column) => {
    const params = column.cellRendererParams as Partial<AppTableActionCellRendererParams> | undefined;
    if (!params?.appGridColumn || !Array.isArray(params.actions)) {
      return column;
    }

    return {
      ...column,
      cellRendererParams: {
        ...params,
        onClientEvent: (input: { actionId: string; row: AppTableRow; columnKey?: string }) => {
          onActionTriggered({
            actionId: input.actionId,
            row: input.row as T,
            columnKey: input.columnKey,
          });
        },
      },
    };
  });
};

type AppTableGridRendererProps<T extends AppTableRow> = AppTableProps<T> & {
  resolvedLayoutMode: "content" | "fill";
};

type GridTooltipAnchor = {
  left: number;
  top: number;
  width: number;
  height: number;
};

const TOOLTIP_MOUSE_DELAY_MS = 350;
const NAVIGABLE_CELL_CLASS = "app-table-navigable-cell";

const getNavigableCellElement = (target: EventTarget | null): HTMLElement | null => {
  if (!(target instanceof HTMLElement)) {
    return null;
  }

  return target.closest(`.ag-cell.${NAVIGABLE_CELL_CLASS}`);
};

export function AppTableGridRenderer<T extends AppTableRow>({
  rows,
  columns,
  loading = false,
  total,
  paginationMode,
  quickFilterText,
  clientPaginationPageSize,
  rowSelection = "multiple",
  suppressRowClickSelection = false,
  suppressCellFocus,
  rowClickAffordance = false,
  rowClickTooltip,
  domLayout = "autoHeight",
  className,
  gridClassName,
  getRowId,
  onRowSelected,
  onCellClicked,
  onRowClicked,
  onActionTriggered,
  onSelectionChanged,
  resolvedLayoutMode,
}: AppTableGridRendererProps<T>) {
  const gridRef = useRef<AgGridReact<T>>(null);
  const tooltipShowTimerRef = useRef<number | null>(null);
  const gridContainerRef = useRef<HTMLDivElement | null>(null);
  const activeTooltipCellRef = useRef<HTMLElement | null>(null);
  const isSoftLoading = loading && rows.length > 0;
  const showLoadingVeil = useDeferredLoadingVeil(isSoftLoading);
  const [tooltipAnchor, setTooltipAnchor] = useState<GridTooltipAnchor | null>(null);
  const isTooltipEnabled = isRowClickTooltipEnabled(rowClickAffordance, rowClickTooltip);
  const gridOptions = useAgGridBaseConfig<T>({
    rowSelection,
    domLayout,
    layoutMode: resolvedLayoutMode,
    paginationMode,
    clientPaginationPageSize,
    suppressRowClickSelection,
    suppressCellFocus: rowClickAffordance ? (suppressCellFocus ?? false) : suppressCellFocus,
    onRowSelected: (event) => {
      if (!event.node.isSelected()) {
        onRowSelected?.(null);
        return;
      }
      onRowSelected?.(event.data ?? null);
    },
    onRowClicked: (event) => {
      if (event.data) {
        onRowClicked?.(event.data);
      }
    },
    onCellClicked: (event) => {
      if (!event.data) return;
      onCellClicked?.({
        row: event.data,
        field: event.colDef?.field ?? null,
        value: event.value,
      });
    },
    onCellKeyDown: (event: CellKeyDownEvent<T>) => {
      if (!rowClickAffordance || getKeyboardEventKey(event.event) !== "Enter") {
        return;
      }

      if (!isNavigableCellEvent(event)) {
        return;
      }

      if (!event.data) {
        return;
      }

      onCellClicked?.({
        row: event.data,
        field: event.colDef?.field ?? null,
        value: event.value,
      });
    },
    onSelectionChanged: (event) => {
      const selectedRows = event.api.getSelectedRows();
      onSelectionChanged?.(selectedRows);
    },
  });

  const columnDefs = useMemo<ColDef<T>[]>(
    () =>
      enrichActionColumns(columns, onActionTriggered).map((column) => {
        if (!rowClickAffordance) {
          return column;
        }

        const existingCellClass = column.cellClass;
        const resolvedField = column.field ?? column.colId ?? null;

        if (!isNavigableField(resolvedField)) {
          return column;
        }

        return {
          ...column,
          cellClass: (params) => {
            const existing = typeof existingCellClass === "function"
              ? existingCellClass(params)
              : existingCellClass;

            return joinClasses(
              normalizeCellClass(existing),
              NAVIGABLE_CELL_CLASS,
              styles.navigableCell,
            );
          },
        };
      }),
    [columns, onActionTriggered, rowClickAffordance],
  );
  const rowData = useMemo<T[]>(() => rows, [rows]);
  const resolvedQuickFilterText = useMemo(
    () => resolveQuickFilterText(paginationMode, quickFilterText),
    [paginationMode, quickFilterText],
  );

  useEffect(() => {
    const api = gridRef.current?.api;
    if (!api) {
      return;
    }

    if (loading) {
      if (rowData.length === 0) {
        api.showLoadingOverlay();
        return;
      }

      api.hideOverlay();
      return;
    }

    if (rowData.length === 0) {
      api.showNoRowsOverlay();
      return;
    }

    api.hideOverlay();
  }, [loading, rowData]);

  const onGridReady = (event: GridReadyEvent<T>) => {
    if (loading) {
      if (rowData.length === 0) {
        event.api.showLoadingOverlay();
        return;
      }

      event.api.hideOverlay();
      return;
    }

    if (rowData.length === 0) {
      event.api.showNoRowsOverlay();
    } else {
      event.api.hideOverlay();
    }
  };

  const overlayStatus = useMemo(() => {
    if (loading) return "loading";
    if (rowData.length === 0) return "empty";
    return "ready";
  }, [loading, rowData.length]);

  const clearTooltipTimer = () => {
    if (tooltipShowTimerRef.current != null) {
      window.clearTimeout(tooltipShowTimerRef.current);
      tooltipShowTimerRef.current = null;
    }
  };

  const hideTooltip = () => {
    clearTooltipTimer();
    activeTooltipCellRef.current = null;
    setTooltipAnchor(null);
  };

  const updateTooltipAnchor = (cell: HTMLElement) => {
    const container = gridContainerRef.current;
    if (!container) {
      return;
    }

    const containerRect = container.getBoundingClientRect();
    const cellRect = cell.getBoundingClientRect();

    setTooltipAnchor({
      left: cellRect.left - containerRect.left,
      top: cellRect.top - containerRect.top,
      width: cellRect.width,
      height: cellRect.height,
    });
  };

  const scheduleTooltipForCell = (cell: HTMLElement) => {
    if (!isTooltipEnabled) {
      return;
    }

    clearTooltipTimer();
    activeTooltipCellRef.current = cell;
    tooltipShowTimerRef.current = window.setTimeout(() => {
      updateTooltipAnchor(cell);
      tooltipShowTimerRef.current = null;
    }, TOOLTIP_MOUSE_DELAY_MS);
  };

  const showTooltipForCell = (cell: HTMLElement) => {
    if (!isTooltipEnabled) {
      return;
    }

    clearTooltipTimer();
    activeTooltipCellRef.current = cell;
    updateTooltipAnchor(cell);
  };

  const tooltipAnchorKey = tooltipAnchor
    ? `${tooltipAnchor.left}-${tooltipAnchor.top}-${tooltipAnchor.width}-${tooltipAnchor.height}`
    : "hidden";

  const resolveValidTooltipCell = (target: EventTarget | null) => {
    if (isInteractiveElement(target)) {
      return null;
    }

    return getNavigableCellElement(target);
  };

  useEffect(() => () => clearTooltipTimer(), []);

  return (
    <div
      className={joinClasses(
        styles.root,
        resolvedLayoutMode === "fill" && styles.rootFill,
        className,
      )}
      data-layout-mode={resolvedLayoutMode}
      data-presentation-mode="table"
      data-typography="inbox"
      data-total={total ?? undefined}
    >
      <div
        ref={gridContainerRef}
        className={joinClasses(
          styles.grid,
          rowClickAffordance && styles.gridAffordance,
          resolvedLayoutMode === "fill" && styles.gridFill,
          "ag-theme-quartz",
          gridClassName,
        )}
        data-overlay={overlayStatus}
        data-testid="app-table-grid"
        onMouseOver={(event) => {
          const cell = resolveValidTooltipCell(event.target);
          if (!cell) {
            hideTooltip();
            return;
          }

          if (activeTooltipCellRef.current === cell) {
            return;
          }

          if (tooltipAnchor) {
            showTooltipForCell(cell);
            return;
          }

          scheduleTooltipForCell(cell);
        }}
        onMouseMove={(event) => {
          const cell = resolveValidTooltipCell(event.target);
          if (!cell) {
            hideTooltip();
            return;
          }

          if (activeTooltipCellRef.current !== cell && tooltipAnchor) {
            showTooltipForCell(cell);
            return;
          }

          if (tooltipAnchor) {
            updateTooltipAnchor(cell);
          }
        }}
        onMouseLeave={() => {
          hideTooltip();
        }}
        onFocusCapture={(event) => {
          const cell = resolveValidTooltipCell(event.target);
          if (!cell) {
            hideTooltip();
            return;
          }

          showTooltipForCell(cell);
        }}
        onBlurCapture={(event) => {
          const nextTarget = event.relatedTarget;
          const nextCell = resolveValidTooltipCell(nextTarget);
          if (nextCell) {
            return;
          }

          hideTooltip();
        }}
      >
        <AgGridReact<T>
          ref={gridRef}
          rowData={rowData}
          columnDefs={columnDefs}
          gridOptions={gridOptions}
          quickFilterText={resolvedQuickFilterText}
          theme="legacy"
          onGridReady={onGridReady}
          getRowId={(params) => resolveRowId(params.data, getRowId)}
        />
        {isTooltipEnabled && tooltipAnchor ? (
          <Tooltip
            key={tooltipAnchorKey}
            title={rowClickTooltip}
            open
            placement="top"
            mouseEnterDelay={0.35}
          >
            <span
              aria-hidden="true"
              className={styles.tooltipAnchor}
              data-testid="app-table-grid-tooltip-anchor"
              style={{
                left: `${tooltipAnchor.left}px`,
                top: `${tooltipAnchor.top}px`,
                width: `${tooltipAnchor.width}px`,
                height: `${tooltipAnchor.height}px`,
              }}
            />
          </Tooltip>
        ) : null}
        {showLoadingVeil ? (
          <div className={styles.loadingVeil} data-testid="app-table-loading-veil">
            <span className={styles.loadingBadge}>
              <span className={styles.loadingBadgeDot} />
              Actualizando datos
            </span>
          </div>
        ) : null}
      </div>
    </div>
  );
}
