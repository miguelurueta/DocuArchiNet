import { DataGrid } from "@mui/x-data-grid";
import type {
  DataGridProps,
  GridColDef,
  GridPaginationModel,
  GridRowId,
  GridRowSelectionModel,
  GridRowsProp,
} from "@mui/x-data-grid";
import type { ComponentProps, ReactNode } from "react";
import styles from "./AppDataTableMui.module.css";

type MuiDataGridProps = ComponentProps<typeof DataGrid>;

export type AppDataTableMuiColumn = GridColDef;
export type AppDataTableMuiRowId = GridRowId;
export type AppDataTableMuiRowSelectionModel = GridRowSelectionModel;

export type AppDataTableMuiProps = Omit<
  MuiDataGridProps,
  | "rows"
  | "columns"
  | "loading"
  | "pageSizeOptions"
  | "paginationModel"
  | "initialState"
  | "onRowSelectionModelChange"
  | "rowSelectionModel"
> & {
  rows: GridRowsProp;
  columns: AppDataTableMuiColumn[];
  loading?: boolean;
  emptyMessage?: ReactNode;
  label?: string;
  initialPageSize?: number;
  pageSizeOptions?: number[];
  rowSelectionModel?: AppDataTableMuiRowSelectionModel;
  onRowSelectionModelChange?: DataGridProps["onRowSelectionModelChange"];
};

const DEFAULT_PAGE_SIZE = 10;

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

function EmptyOverlay({ message }: { message: ReactNode }) {
  return (
    <div className={styles.emptyState} role="status">
      <div className={styles.emptyTitle}>Sin resultados</div>
      <div className={styles.emptyMessage}>{message}</div>
    </div>
  );
}

export function AppDataTableMui({
  rows,
  columns,
  loading = false,
  emptyMessage = "No hay registros para mostrar.",
  label,
  initialPageSize = DEFAULT_PAGE_SIZE,
  pageSizeOptions = [5, 10, 20, 50],
  className,
  rowSelectionModel,
  onRowSelectionModelChange,
  autoHeight = true,
  ...restProps
}: AppDataTableMuiProps) {
  const paginationModel: GridPaginationModel = {
    page: 0,
    pageSize: initialPageSize,
  };

  return (
    <div className={joinClasses(styles.shell, className)}>
      <DataGrid
        {...restProps}
        rows={rows}
        columns={columns}
        loading={loading}
        autoHeight={autoHeight}
        pageSizeOptions={pageSizeOptions}
        initialState={{
          pagination: {
            paginationModel,
          },
        }}
        rowSelectionModel={rowSelectionModel}
        onRowSelectionModelChange={onRowSelectionModelChange}
        aria-label={label}
        slots={{
          noRowsOverlay: () => <EmptyOverlay message={emptyMessage} />,
        }}
        sx={{
          border: 0,
          minHeight: 320,
          "& .MuiDataGrid-columnHeaders": {
            backgroundColor: "#f6f9fc",
            borderBottom: "1px solid #dde6f1",
            color: "#17324b",
            fontWeight: 700,
          },
          "& .MuiDataGrid-cell": {
            borderBottom: "1px solid #edf2f7",
            color: "#314559",
          },
          "& .MuiDataGrid-footerContainer": {
            borderTop: "1px solid #dde6f1",
          },
          "& .MuiDataGrid-overlayWrapper": {
            minHeight: 220,
          },
        }}
      />
    </div>
  );
}
