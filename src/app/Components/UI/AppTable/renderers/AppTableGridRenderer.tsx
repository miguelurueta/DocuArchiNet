import { AgGridReact } from "ag-grid-react";
import type { ColDef, GridReadyEvent } from "ag-grid-community";
import { useEffect, useMemo, useRef } from "react";
import type { AppTablePaginationMode, AppTableProps, AppTableRow } from "../AppTable.types";
import { useAgGridBaseConfig } from "../hooks/useAgGridBaseConfig";
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
  domLayout = "autoHeight",
  className,
  gridClassName,
  getRowId,
  onRowSelected,
  onCellClicked,
  onRowClicked,
  onSelectionChanged,
  resolvedLayoutMode,
}: AppTableGridRendererProps<T>) {
  const gridRef = useRef<AgGridReact<T>>(null);
  const gridOptions = useAgGridBaseConfig<T>({
    rowSelection,
    domLayout,
    layoutMode: resolvedLayoutMode,
    paginationMode,
    clientPaginationPageSize,
    suppressRowClickSelection,
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

  const columnDefs = useMemo<ColDef<T>[]>(() => columns, [columns]);
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
      api.showLoadingOverlay();
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
      event.api.showLoadingOverlay();
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
      </div>
    </div>
  );
}
