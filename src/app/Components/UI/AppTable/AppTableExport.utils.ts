import type { ColDef } from "ag-grid-community";
import type {
  AppTableExportFormat,
  AppTableExportMode,
  AppTableExportReportMeta,
} from "./AppTableExport.types";
import type { AppTableRow } from "./AppTable.types";

export type AppTableExportColumn<T extends AppTableRow = AppTableRow> = {
  field: Extract<keyof T, string> | string;
  headerName: string;
};

type AppTableExportRequest<T extends AppTableRow> = {
  columns: AppTableExportColumn<T>[];
  rows: T[];
  format: AppTableExportFormat;
  mode: AppTableExportMode;
  reportMeta: AppTableExportReportMeta;
  fileName?: string;
};

const ACTION_FIELD_PATTERN = /accion/i;

const sanitizeSegment = (value: string) =>
  value
    .normalize("NFD")
    .replace(/[\u0300-\u036f]/g, "")
    .replace(/[^a-zA-Z0-9]+/g, "-")
    .replace(/^-+|-+$/g, "")
    .toLowerCase();

const escapeCsvValue = (value: unknown) => {
  const normalized =
    value === null || value === undefined
      ? ""
      : value instanceof Date
        ? value.toISOString()
        : String(value);

  if (/[",\n]/.test(normalized)) {
    return `"${normalized.replace(/"/g, '""')}"`;
  }

  return normalized;
};

export const getAppTableExportableColumns = <T extends AppTableRow>(
  columns: ColDef<T>[],
): AppTableExportColumn<T>[] =>
  columns.flatMap((column) => {
    const field = column.field;
    if (typeof field !== "string" || field.trim().length === 0) {
      return [];
    }

    if (column.hide) {
      return [];
    }

    const actionLikeColumn =
      ACTION_FIELD_PATTERN.test(field) ||
      ACTION_FIELD_PATTERN.test(column.headerName ?? "") ||
      Boolean((column.cellRendererParams as { actions?: unknown } | undefined)?.actions) ||
      Boolean(
        (
          column.cellRendererParams as
            | { appGridColumn?: { actions?: unknown[] } }
            | undefined
        )?.appGridColumn?.actions,
      );

    if (actionLikeColumn) {
      return [];
    }

    return [
      {
        field,
        headerName: column.headerName?.trim() || field,
      },
    ];
  });

export const getAppTableExportRows = <T extends AppTableRow>({
  mode,
  getCurrentPageRows,
  getSelectedRows,
  getAllLoadedRows,
}: {
  mode: Extract<AppTableExportMode, "currentPage" | "selectedRows" | "allLoaded">;
  getCurrentPageRows: () => T[];
  getSelectedRows?: () => T[];
  getAllLoadedRows?: () => T[];
}): T[] => {
  if (mode === "selectedRows") {
    return getSelectedRows?.() ?? [];
  }

  if (mode === "allLoaded") {
    return getAllLoadedRows?.() ?? [];
  }

  return getCurrentPageRows();
};

export const buildAppTableExportFileName = ({
  reportMeta,
  mode,
  format,
  fileName,
}: {
  reportMeta: AppTableExportReportMeta;
  mode: AppTableExportMode;
  format: AppTableExportFormat;
  fileName?: string;
}) => {
  if (fileName?.trim()) {
    return fileName.trim();
  }

  const reportName = sanitizeSegment(reportMeta.reportName || "reporte");
  const generatedAt = sanitizeSegment(reportMeta.generatedAt || new Date().toISOString());
  return `${reportName}-${mode}-${generatedAt}.${format}`;
};

export const serializeAppTableExportToCsv = <T extends AppTableRow>({
  columns,
  rows,
}: Pick<AppTableExportRequest<T>, "columns" | "rows">) => {
  const headerRow = columns.map((column) => escapeCsvValue(column.headerName)).join(",");
  const dataRows = rows.map((row) =>
    columns.map((column) => escapeCsvValue(row[column.field as keyof T])).join(","),
  );

  return [headerRow, ...dataRows].join("\n");
};

export const downloadAppTableExportFile = <T extends AppTableRow>({
  columns,
  rows,
  format,
  mode,
  reportMeta,
  fileName,
}: AppTableExportRequest<T>) => {
  if (format !== "csv") {
    throw new Error(`Formato de exportacion no soportado en esta fase: ${format}`);
  }

  const csv = serializeAppTableExportToCsv({ columns, rows });
  const finalFileName = buildAppTableExportFileName({
    reportMeta,
    mode,
    format,
    fileName,
  });
  const blob = new Blob([csv], { type: "text/csv;charset=utf-8;" });
  const blobUrl = window.URL.createObjectURL(blob);
  const anchor = document.createElement("a");
  anchor.href = blobUrl;
  anchor.download = finalFileName;
  anchor.click();
  window.URL.revokeObjectURL(blobUrl);
};
