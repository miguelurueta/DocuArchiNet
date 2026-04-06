import type { ColDef } from "ag-grid-community";
import type { ReactNode } from "react";
import type { AppTableRow } from "./AppTable.types";

export type AppTableExportMode =
  | "currentPage"
  | "selectedRows"
  | "allLoaded"
  | "allMatching";

export type AppTableExportFormat = "csv" | "xlsx" | "pdf";

export type AppTableExportColumn<T extends AppTableRow = AppTableRow> = {
  field: Extract<keyof T, string> | string;
  headerName: string;
};

export type AppTableBackendExportRequest<T extends AppTableRow = AppTableRow> = {
  columns: AppTableExportColumn<T>[];
  format: AppTableExportFormat;
  mode: AppTableExportMode;
  reportMeta: AppTableExportReportMeta;
  fileName?: string;
};

export type AppTableBackendExportFile = {
  blob: Blob;
  fileName?: string;
  contentType?: string;
};

export type AppTableExportDataSource<T extends AppTableRow = AppTableRow> = {
  getCurrentPageRows: () => T[];
  getSelectedRows?: () => T[];
  getAllLoadedRows?: () => T[];
  getAllMatchingRows?: () => Promise<T[]>;
  getBackendExportFile?: (
    request: AppTableBackendExportRequest<T>,
  ) => Promise<AppTableBackendExportFile>;
};

export type AppTableExportReportMeta = {
  reportName: string;
  generatedBy: string;
  moduleName: string;
  reportType: string;
  generatedAt: string;
  rowCount: number;
  description: string;
  companyImageAsset: string;
};

export type AppTableExportProps<T extends AppTableRow = AppTableRow> = {
  columns: ColDef<T>[];
  dataSource: AppTableExportDataSource<T>;
  formats: AppTableExportFormat[];
  reportMeta: AppTableExportReportMeta;
  enabledModes?: AppTableExportMode[];
  fileName?: string;
  triggerLabel?: ReactNode;
  disabled?: boolean;
};

export const APP_TABLE_EXPORT_MODES: readonly AppTableExportMode[] = [
  "currentPage",
  "selectedRows",
  "allLoaded",
  "allMatching",
] as const;

export const APP_TABLE_EXPORT_FORMATS: readonly AppTableExportFormat[] = [
  "csv",
  "xlsx",
  "pdf",
] as const;

export const hasAppTableExportDataSourceCapability = <T extends AppTableRow>(
  dataSource: AppTableExportDataSource<T>,
  mode: AppTableExportMode,
): boolean => {
  switch (mode) {
    case "currentPage":
      return typeof dataSource.getCurrentPageRows === "function";
    case "selectedRows":
      return typeof dataSource.getSelectedRows === "function";
    case "allLoaded":
      return typeof dataSource.getAllLoadedRows === "function";
    case "allMatching":
      return (
        typeof dataSource.getAllMatchingRows === "function" ||
        typeof dataSource.getBackendExportFile === "function"
      );
    default:
      return false;
  }
};

export const getAvailableAppTableExportModes = <T extends AppTableRow>(
  dataSource: AppTableExportDataSource<T>,
  enabledModes: readonly AppTableExportMode[] = APP_TABLE_EXPORT_MODES,
): AppTableExportMode[] =>
  enabledModes.filter((mode) => hasAppTableExportDataSourceCapability(dataSource, mode));

export const shouldUseBackendAppTableExport = <T extends AppTableRow>(
  dataSource: AppTableExportDataSource<T>,
  format: AppTableExportFormat,
  mode: AppTableExportMode,
): boolean =>
  typeof dataSource.getBackendExportFile === "function" &&
  mode === "allMatching" &&
  (format === "csv" || format === "xlsx" || format === "pdf");

export const isAppTableExportExecutable = <T extends AppTableRow>(
  dataSource: AppTableExportDataSource<T>,
  format: AppTableExportFormat,
  mode: AppTableExportMode,
): boolean => {
  if (!hasAppTableExportDataSourceCapability(dataSource, mode)) {
    return false;
  }

  if (shouldUseBackendAppTableExport(dataSource, format, mode)) {
    return true;
  }

  return format === "csv";
};
