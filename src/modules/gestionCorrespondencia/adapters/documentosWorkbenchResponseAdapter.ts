import type { ColDef } from "ag-grid-community";
import type { AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";
import { mapDynamicUiTableToAppDataTableAgGrid } from "../../../app/Components/UI/AppTable/adapters/dynamicUiToAgGridColumns";
import { mapAppGridColumnsToAppTableColumns } from "../../../app/Components/UI/AppTable/adapters/appGridToAppTableColumns";
import type { DynamicUiTableDto } from "../../../app/Components/UI/AppTable/types/dynamicUiTable.types";
import type { ListaDocumentosRadicadosQueryData, ListaDocumentosRadicadosRowDto } from "../types/listaDocumentosRadicados.types";

const inferLabel = (row: ListaDocumentosRadicadosRowDto): string => {
  const values = row.Values ?? {};
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
  tableColumns?: ColDef<Record<string, unknown>>[];
};

export const adaptListaDocumentosRadicadosToWorkbenchModel = (
  data: ListaDocumentosRadicadosQueryData,
): DocumentosWorkbenchTableModel => {
  const rows = (data.Rows ?? []).map(toTreeRow);

  const dynamicUiTable = pickDynamicUiTable(data);
  if (!dynamicUiTable) {
    return { rows };
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
    tableColumns: tableColumns as ColDef<Record<string, unknown>>[],
  };
};

