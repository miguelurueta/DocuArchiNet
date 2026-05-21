import type { ColDef } from "ag-grid-community";
import type { AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";
import { mapDynamicUiTableToAppDataTableAgGrid } from "../../../app/Components/UI/AppTable/adapters/dynamicUiToAgGridColumns";
import { mapAppGridColumnsToAppTableColumns } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableColumns";
import type { DynamicUiTableDto } from "../../../app/Components/UI/AppTable/types/dynamicUiTable.types";
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

const toTreeRow = (row: ListaDocumentosRadicadosRowDto): AppTreeTableRow => ({
  id: row.RowId,
  label: inferLabel(row),
  values: row.Values,
  meta: { ...(row.Meta ?? {}) },
  hasChildren: Boolean(row.Meta?.HasChildren),
  children: row.Meta?.HasChildren ? [] : undefined,
});

const pickDynamicUiTable = (data: ListaDocumentosRadicadosQueryData): DynamicUiTableDto | null => {
  const raw = data.Config;
  if (!raw || typeof raw !== "object") return null;
  return raw as DynamicUiTableDto;
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

export const adaptListaDocumentosRadicadosToWorkbenchModel = (
  data: ListaDocumentosRadicadosQueryData,
  options?: { viewMode?: DocumentosWorkbenchViewMode },
): DocumentosWorkbenchTableModel => {
  const rows = (data.Rows ?? []).map(toTreeRow);
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
  const tableColumns = mapAppGridColumnsToAppTableColumns(appGridTable.columns, {
    tableId: appGridTable.tableId,
    userClaims: appGridTable.userClaims,
    menuActions: appGridTable.menuActions,
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
