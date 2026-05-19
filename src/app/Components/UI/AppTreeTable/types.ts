export type AppTreeTableRowId = string;

export type AppTreeTableRow = {
  id: AppTreeTableRowId;
  label: string;
  children?: AppTreeTableRow[];
};

export type AppTreeTableLoadResult =
  | { ok: true; rows: AppTreeTableRow[] }
  | { ok: false; message: string };

export type AppTreeTableProps = {
  rows?: AppTreeTableRow[];
  load?: () => Promise<AppTreeTableLoadResult>;
  onSelectRow?: (rowId: AppTreeTableRowId) => void;
  emptyMessage?: string;
  className?: string;
};

