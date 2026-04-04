import type { ColDef } from "ag-grid-community";
import AppTableActionCellRenderer from "./AppTableActionCellRenderer";
import type { AppGridCellAction, AppGridColumn, AppTableRow } from "../types/dynamicUiTable.types";
import type { AppTableProps } from "../AppTable.types";
import type { AppTableActionCellRendererParams } from "../types/dynamicUiTableAction.types";
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
  } as AppTableActionCellRendererParams;
};

type AppTableCardRendererProps<T extends AppTableRow> = Pick<
  AppTableProps<T>,
  "rows" | "columns" | "cardFields" | "loading" | "total" | "className" | "onRowClicked"
> & {
  resolvedLayoutMode: "content" | "fill";
};

const isActionColumn = <T extends AppTableRow>(column: ColDef<T>): boolean => {
  const params = column.cellRendererParams as Partial<AppTableActionCellRendererParams> | undefined;
  return Boolean(params?.appGridColumn && Array.isArray(params.actions));
};

export function AppTableCardRenderer<T extends AppTableRow>({
  rows,
  columns,
  cardFields,
  loading = false,
  total,
  className,
  onRowClicked,
  resolvedLayoutMode,
}: AppTableCardRendererProps<T>) {
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
          rows.map((row, rowIndex) => (
            <article
              key={String(row.id ?? rowIndex)}
              className={styles.card}
              data-testid="app-table-card"
              onClick={() => onRowClicked?.(row)}
            >
              <div className={styles.cardBody}>
                {valueColumns.map((column) => {
                  const field = column.field;
                  const value = field ? row[field] : undefined;
                  const formattedValue = formatCardValue(value);

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
                <div className={styles.cardActions}>
                  {actionColumns.map((column) => {
                    const actionParams = resolveActionRendererParams(column, row);
                    if (!actionParams) {
                      return null;
                    }

                    return (
                      <AppTableActionCellRenderer
                        key={column.colId ?? column.field ?? "actions"}
                        {...actionParams}
                      />
                    );
                  })}
                </div>
              ) : null}
            </article>
          ))
        )}
      </div>
    </div>
  );
}
