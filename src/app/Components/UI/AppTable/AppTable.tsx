import { AgGridReact } from "ag-grid-react";
import type { ColDef, GridReadyEvent } from "ag-grid-community";
import { useMemo, useRef } from "react";
import type { AppTableProps, AppTableRow } from "./AppTable.types";
import { useAgGridBaseConfig } from "./hooks/useAgGridBaseConfig";
import styles from "./AppTable.module.css";
import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";

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

export default function AppTable<T extends AppTableRow>({
  rows,
  columns,
  loading = false,
  total,
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
}: AppTableProps<T>) {
  const gridRef = useRef<AgGridReact<T>>(null);
  const gridOptions = useAgGridBaseConfig<T>({
    rowSelection,
    domLayout,
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
    <div className={joinClasses(styles.root, className)} data-total={total ?? undefined}>
      <div
        className={joinClasses(styles.grid, "ag-theme-quartz", gridClassName)}
        data-overlay={overlayStatus}
        data-testid="app-table-grid"
      >
        <AgGridReact<T>
          ref={gridRef}
          rowData={rowData}
          columnDefs={columnDefs}
          gridOptions={gridOptions}
          onGridReady={onGridReady}
          getRowId={(params) => resolveRowId(params.data, getRowId)}
        />
      </div>
    </div>
  );
}
