import type { ColDef } from "ag-grid-community";
import type { AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";
import { mapDynamicUiTableToAppDataTableAgGrid } from "../../../app/Components/UI/AppTable/adapters/dynamicUiToAgGridColumns";
import { mapAppGridColumnsToAppTableColumns } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableColumns";
import type {
  AppGridCellAction,
  DynamicUiTableDto,
} from "../../../app/Components/UI/AppTable/types/dynamicUiTable.types";
import type { ListaDocumentosRadicadosQueryData, ListaDocumentosRadicadosRowDto } from "../types/listaDocumentosRadicados.types";

export type DocumentosWorkbenchViewMode = "hierarchical" | "flatDocuments";

const inferLabel = (row: ListaDocumentosRadicadosRowDto): string => {
  const values = row.Values ?? {};

  const tipodocumento = values.TIPODOCUMENTO;
  if (typeof tipodocumento === "string" && tipodocumento.trim().length > 0) {
    // SCRUM-209: el backend ya formaliza TIPODOCUMENTO con fallback `DOC {ID}`.
    // El frontend no recalcula el fallback; solo lo muestra.
    return tipodocumento.trim();
  }

  const firstKey = Object.keys(values)[0];
  const firstValue = firstKey ? values[firstKey] : undefined;
  if (typeof firstValue === "string" && firstValue.trim().length > 0) return firstValue.trim();
  return String(firstValue ?? row.RowId);
};

const readRowIdCandidate = (value: unknown): string | null => {
  if (typeof value === "string" && value.trim().length > 0) return value.trim();
  if (typeof value === "number" && Number.isFinite(value)) return String(value);
  return null;
};

export const resolveDocumentWorkbenchRowId = (
  row: ListaDocumentosRadicadosRowDto,
  index: number,
): string => {
  const fromRowId = readRowIdCandidate(row.RowId);
  if (fromRowId) return fromRowId;

  const values = row.Values ?? {};
  const fallbackFromValues =
    readRowIdCandidate(values.RowId) ??
    readRowIdCandidate(values.ROWID) ??
    readRowIdCandidate(values.IdDocumento) ??
    readRowIdCandidate(values.IDDOCUMENTO) ??
    readRowIdCandidate(values.DocumentId) ??
    readRowIdCandidate(values.DOCUMENTID) ??
    readRowIdCandidate(values.ID) ??
    readRowIdCandidate(values.Id);

  if (fallbackFromValues) return fallbackFromValues;

  const firstKey = Object.keys(values)[0];
  const firstValue = firstKey ? values[firstKey] : undefined;
  const fallbackToken = readRowIdCandidate(firstValue);
  if (fallbackToken) {
    const normalized = fallbackToken.replace(/\s+/g, "-");
    return `row-${index}-${normalized}`;
  }

  return `row-${index}`;
};

const toTreeRow = (row: ListaDocumentosRadicadosRowDto, index: number): AppTreeTableRow => ({
  id: resolveDocumentWorkbenchRowId(row, index),
  label: inferLabel(row),
  values: row.Values,
  meta: { ...(row.Meta ?? {}) },
  hasChildren: Boolean(row.Meta?.HasChildren),
  children: row.Meta?.HasChildren ? [] : undefined,
});

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

const hasDynamicTableShape = (value: unknown): value is DynamicUiTableDto => {
  if (!isRecord(value)) return false;

  const candidate = value as Record<string, unknown>;
  const tableId = candidate.TableId ?? candidate.tableId;
  const rawColumns = candidate.Columns ?? candidate.columns;
  const rawActions = [
    candidate.CellActions,
    candidate.cellActions,
    candidate.MenuActions,
    candidate.menuActions,
    candidate.RowActions,
    candidate.rowActions,
  ];

  const hasActionCollections = rawActions.some((item) => Array.isArray(item));
  const hasObjectColumns =
    Array.isArray(rawColumns) &&
    rawColumns.some((column) => typeof column === "object" && column !== null);
  const hasTableId = typeof tableId === "string" && tableId.trim().length > 0;

  return hasActionCollections || hasObjectColumns || hasTableId;
};

const pickDynamicUiTable = (data: ListaDocumentosRadicadosQueryData): DynamicUiTableDto | null => {
  if (isRecord(data.Config)) {
    return data.Config as DynamicUiTableDto;
  }

  return hasDynamicTableShape(data) ? (data as unknown as DynamicUiTableDto) : null;
};

export type DocumentosWorkbenchTableModel = {
  rows: AppTreeTableRow[];
  columns?: string[];
  tableColumns?: ColDef<Record<string, unknown>>[];
  tableId?: string;
};

const pickColumnsKeys = (data: ListaDocumentosRadicadosQueryData): string[] | undefined => {
  const raw = data.Columns;
  if (!raw) return undefined;

  if (Array.isArray(raw) && raw.every((item) => typeof item === "string")) {
    return raw as string[];
  }

  if (Array.isArray(raw) && raw.every((item) => item && typeof item === "object")) {
    const keys = (raw as Array<Record<string, unknown>>)
      .map((item) => {
        const key =
          (typeof item.ColumnKey === "string" ? item.ColumnKey : undefined) ??
          (typeof item.columnKey === "string" ? item.columnKey : undefined) ??
          (typeof item.Field === "string" ? item.Field : undefined) ??
          (typeof item.field === "string" ? item.field : undefined) ??
          (typeof item.DataIndex === "string" ? item.DataIndex : undefined) ??
          (typeof item.dataIndex === "string" ? item.dataIndex : undefined);
        return key?.trim();
      })
      .filter((value): value is string => !!value && value.length > 0);

    return keys.length > 0 ? keys : undefined;
  }

  return undefined;
};

const forceWorkbenchClientEvent = (action: AppGridCellAction): AppGridCellAction => ({
  ...action,
  behavior: "client_event",
  children: action.children?.map(forceWorkbenchClientEvent),
});

const forceWorkbenchActionColumnsClientEvent = (
  columns: ReturnType<typeof mapDynamicUiTableToAppDataTableAgGrid>["columns"],
): ReturnType<typeof mapDynamicUiTableToAppDataTableAgGrid>["columns"] =>
  columns.map((column) =>
    column.isActionColumn
      ? {
          ...column,
          actions: column.actions?.map(forceWorkbenchClientEvent),
        }
      : column,
  );

export const adaptListaDocumentosRadicadosToWorkbenchModel = (
  data: ListaDocumentosRadicadosQueryData,
  options?: { viewMode?: DocumentosWorkbenchViewMode },
): DocumentosWorkbenchTableModel => {
  const rows = (data.Rows ?? []).map((row, index) => toTreeRow(row, index));
  let columns = pickColumnsKeys(data);

  if (options?.viewMode === "flatDocuments") {
    // SCRUM-209: vista simplificada (label principal + acciones). Evita depender
    // de columnas legacy removidas incluso si backend las envía en `Columns`.
    const canUseTipodocumento = rows.some((row) => typeof row.values?.TIPODOCUMENTO === "string");
    columns = canUseTipodocumento ? ["TIPODOCUMENTO"] : columns?.slice(0, 1);
  }

  const dynamicUiTable = pickDynamicUiTable(data);
  if (!dynamicUiTable) {
    return { rows, columns };
  }

  const appGridTable = mapDynamicUiTableToAppDataTableAgGrid(dynamicUiTable);
  const workbenchColumns = forceWorkbenchActionColumnsClientEvent(appGridTable.columns);
  const workbenchMenuActions = (appGridTable.menuActions ?? []).map(forceWorkbenchClientEvent);
  const tableColumns = mapAppGridColumnsToAppTableColumns(workbenchColumns, {
    tableId: appGridTable.tableId,
    userClaims: appGridTable.userClaims,
    menuActions: workbenchMenuActions,
    onClientEvent: ({ actionId, row, columnKey }) => {
      // AppTable emite client_event via onActionTriggered; este callback queda
      // configurado pero el manejo final se realiza en DocumentosWorkbench via
      // `AppTreeTable.onActionTriggered`.
      void actionId;
      void row;
      void columnKey;
    },
  });

  return {
    rows,
    columns,
    tableColumns: tableColumns as ColDef<Record<string, unknown>>[],
    tableId: appGridTable.tableId,
  };
};
