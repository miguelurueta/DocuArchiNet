import { Tooltip } from "antd";
import type { ColDef } from "ag-grid-community";
import { useEffect, useRef, useState } from "react";
import { useDeferredLoadingVeil } from "../hooks/useDeferredLoadingVeil";
import AppTableActionCellRenderer from "./AppTableActionCellRenderer";
import type { AppGridCellAction, AppGridColumn, AppTableRow } from "../types/dynamicUiTable.types";
import type { AppTableProps } from "../AppTable.types";
import type { AppTableActionCellRendererParams } from "../types/dynamicUiTableAction.types";
import { isInteractiveElement, isRowClickTooltipEnabled } from "../utils/navigableAffordance";
import styles from "../AppTable.module.css";

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

const formatCardValue = (value: unknown): string => {
  if (value == null) {
    return "";
  }

  if (typeof value === "string" || typeof value === "number" || typeof value === "boolean") {
    return String(value);
  }

  if (Array.isArray(value)) {
    return value.map((item) => formatCardValue(item)).filter(Boolean).join(", ");
  }

  if (value instanceof Date) {
    return value.toISOString();
  }

  if (isRecord(value)) {
    return JSON.stringify(value);
  }

  return String(value);
};

const formatColumnCardValue = <T extends AppTableRow>(
  column: ColDef<T>,
  row: T,
  value: unknown,
): string => {
  if (typeof column.valueFormatter === "function") {
    const formattedValue = column.valueFormatter({
      value,
      data: row,
      colDef: column,
      column: null,
      node: null,
      api: null,
      context: null,
    } as never);

    return formatCardValue(formattedValue);
  }

  return formatCardValue(value);
};

const resolveActionRendererParams = <T extends AppTableRow>(
  column: ColDef<T>,
  row: T,
): AppTableActionCellRendererParams | null => {
  const params = column.cellRendererParams as Partial<AppTableActionCellRendererParams> | undefined;
  if (!params?.appGridColumn || !Array.isArray(params.actions)) {
    return null;
  }

  return {
    api: {
      getSelectedRows: () => [row],
    } as AppTableActionCellRendererParams["api"],
    data: row,
    value: "",
    appGridColumn: params.appGridColumn as AppGridColumn,
    actions: [...(params.actions as AppGridCellAction[])],
    menuActions: [...(params.menuActions ?? [])],
    tableId: params.tableId,
    userClaims: [...(params.userClaims ?? [])],
    onClientEvent: params.onClientEvent,
  } as AppTableActionCellRendererParams;
};

type AppTableCardRendererProps<T extends AppTableRow> = Pick<
  AppTableProps<T>,
  | "rows"
  | "columns"
  | "cardFields"
  | "loading"
  | "total"
  | "className"
  | "onRowClicked"
  | "onActionTriggered"
  | "rowClickAffordance"
  | "rowClickTooltip"
> & {
  resolvedLayoutMode: "content" | "fill";
};

const TOOLTIP_MOUSE_DELAY_MS = 350;

const isActionColumn = <T extends AppTableRow>(column: ColDef<T>): boolean => {
  const params = column.cellRendererParams as Partial<AppTableActionCellRendererParams> | undefined;
  return Boolean(params?.appGridColumn && Array.isArray(params.actions));
};

const isTooltipExcludedTarget = (target: EventTarget | null, actionClassName: string) => {
  if (isInteractiveElement(target)) {
    return true;
  }

  return target instanceof HTMLElement && Boolean(target.closest(`.${actionClassName}`));
};

export function AppTableCardRenderer<T extends AppTableRow>({
  rows,
  columns,
  cardFields,
  loading = false,
  total,
  className,
  onRowClicked,
  onActionTriggered,
  rowClickAffordance = false,
  rowClickTooltip,
  resolvedLayoutMode,
}: AppTableCardRendererProps<T>) {
  const tooltipShowTimerRef = useRef<number | null>(null);
  const isSoftLoading = loading && rows.length > 0;
  const showLoadingVeil = useDeferredLoadingVeil(isSoftLoading);
  const [activeTooltipCardKey, setActiveTooltipCardKey] = useState<string | null>(null);
  const isTooltipEnabled =
    typeof onRowClicked === "function" && isRowClickTooltipEnabled(rowClickAffordance, rowClickTooltip);
  const valueColumns = columns.filter((column) => {
    if (column.hide || isActionColumn(column)) {
      return false;
    }

    if (!cardFields?.length) {
      return true;
    }

    const field = column.field ?? column.colId;
    return Boolean(field && cardFields.includes(field));
  });
  const actionColumns = columns.filter((column) => !column.hide && isActionColumn(column));
  const overlayStatus = loading ? "loading" : rows.length === 0 ? "empty" : "ready";

  const clearTooltipTimer = () => {
    if (tooltipShowTimerRef.current != null) {
      window.clearTimeout(tooltipShowTimerRef.current);
      tooltipShowTimerRef.current = null;
    }
  };

  const hideTooltip = () => {
    clearTooltipTimer();
    setActiveTooltipCardKey(null);
  };

  const scheduleTooltip = (cardKey: string) => {
    if (!isTooltipEnabled) {
      return;
    }

    clearTooltipTimer();
    tooltipShowTimerRef.current = window.setTimeout(() => {
      setActiveTooltipCardKey(cardKey);
      tooltipShowTimerRef.current = null;
    }, TOOLTIP_MOUSE_DELAY_MS);
  };

  const showTooltip = (cardKey: string) => {
    if (!isTooltipEnabled) {
      return;
    }

    clearTooltipTimer();
    setActiveTooltipCardKey(cardKey);
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
      data-presentation-mode="cards"
      data-typography="inbox"
      data-total={total ?? undefined}
    >
      <div
        className={joinClasses(
          styles.cards,
          resolvedLayoutMode === "fill" && styles.cardsFill,
        )}
        data-overlay={overlayStatus}
        data-testid="app-table-cards"
      >
        {rows.length === 0 ? (
          <div className={styles.cardEmpty}>Sin registros</div>
        ) : (
          rows.map((row, rowIndex) => {
            const cardKey = String(row.id ?? rowIndex);

            return (
              <Tooltip
                key={cardKey}
                title={rowClickTooltip}
                open={isTooltipEnabled && activeTooltipCardKey === cardKey}
                placement="top"
                mouseEnterDelay={0.35}
              >
                <article
                  className={joinClasses(
                    styles.card,
                    rowClickAffordance && typeof onRowClicked === "function" && styles.cardNavigable,
                  )}
                  data-testid="app-table-card"
                  tabIndex={rowClickAffordance && typeof onRowClicked === "function" ? 0 : undefined}
                  onClick={() => onRowClicked?.(row)}
                  onKeyDown={(event) => {
                    if (event.key !== "Enter") {
                      return;
                    }

                    event.preventDefault();
                    onRowClicked?.(row);
                  }}
                  onMouseOver={(event) => {
                    if (isTooltipExcludedTarget(event.target, styles.cardActions)) {
                      hideTooltip();
                      return;
                    }

                    scheduleTooltip(cardKey);
                  }}
                  onMouseMove={(event) => {
                    if (isTooltipExcludedTarget(event.target, styles.cardActions)) {
                      hideTooltip();
                    }
                  }}
                  onMouseLeave={() => {
                    hideTooltip();
                  }}
                  onFocusCapture={(event) => {
                    if (isTooltipExcludedTarget(event.target, styles.cardActions)) {
                      hideTooltip();
                      return;
                    }

                    showTooltip(cardKey);
                  }}
                  onBlurCapture={(event) => {
                    const nextTarget = event.relatedTarget;
                    if (nextTarget instanceof HTMLElement && event.currentTarget.contains(nextTarget)) {
                      return;
                    }

                    hideTooltip();
                  }}
                >
                  <div className={styles.cardBody}>
                    {valueColumns.map((column) => {
                      const field = column.field;
                      const value = field ? row[field] : undefined;
                      const formattedValue = formatColumnCardValue(column, row, value);

                      if (!field || formattedValue.length === 0) {
                        return null;
                      }

                      return (
                        <div key={column.colId ?? field} className={styles.cardField}>
                          <span className={styles.cardLabel}>{column.headerName ?? field}</span>
                          <span className={styles.cardValue}>{formattedValue}</span>
                        </div>
                      );
                    })}
                  </div>

                  {actionColumns.length > 0 ? (
                    <div
                      className={styles.cardActions}
                      onClick={(event) => event.stopPropagation()}
                      onKeyDown={(event) => event.stopPropagation()}
                    >
                      {actionColumns.map((column) => {
                        const actionParams = resolveActionRendererParams(column, row);
                        if (!actionParams) {
                          return null;
                        }

                        return (
                          <AppTableActionCellRenderer
                            key={column.colId ?? column.field ?? "actions"}
                            {...actionParams}
                            onClientEvent={(input) => {
                              onActionTriggered?.({
                                actionId: input.actionId,
                                row: input.row as T,
                                columnKey: input.columnKey,
                              });
                            }}
                          />
                        );
                      })}
                    </div>
                  ) : null}
                </article>
              </Tooltip>
            );
          })
        )}
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
