import { AgGridReact } from "ag-grid-react";
import type { ColDef, GridReadyEvent } from "ag-grid-community";
import { useEffect, useMemo, useRef } from "react";
import type { AppTablePaginationMode, AppTableProps, AppTableRow } from "../AppTable.types";
import type { AppTableActionCellRendererParams } from "../types/dynamicUiTableAction.types";
import { useAgGridBaseConfig } from "../hooks/useAgGridBaseConfig";
import { useDeferredLoadingVeil } from "../hooks/useDeferredLoadingVeil";
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
  const isSoftLoading = loading && rows.length > 0;
  const showLoadingVeil = useDeferredLoadingVeil(isSoftLoading);
  const gridOptions = useAgGridBaseConfig<T>({
    rowSelection,
    domLayout,
    layoutMode: resolvedLayoutMode,
    paginationMode,
    clientPaginationPageSize,
    suppressRowClickSelection,
    suppressCellFocus,
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
    onSelectionChanged: (event) => {
      const selectedRows = event.api.getSelectedRows();
      onSelectionChanged?.(selectedRows);
    },
  });

  const columnDefs = useMemo<ColDef<T>[]>(
    () => enrichActionColumns(columns, onActionTriggered),
    [columns, onActionTriggered],
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
        className={joinClasses(
          styles.grid,
          resolvedLayoutMode === "fill" && styles.gridFill,
          "ag-theme-quartz",
          gridClassName,
        )}
        data-overlay={overlayStatus}
        data-testid="app-table-grid"
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
