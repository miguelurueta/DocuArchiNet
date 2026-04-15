import { describe, expect, it } from "vitest";
import type { ColDef } from "ag-grid-community";
import type { AppTableRow } from "../AppTable.types";
import {
  APP_TABLE_EXPORT_FORMATS,
  APP_TABLE_EXPORT_MODES,
  getAvailableAppTableExportModes,
  hasAppTableExportDataSourceCapability,
  isAppTableExportExecutable,
  shouldUseBackendAppTableExport,
  type AppTableExportDataSource,
  type AppTableExportProps,
  type AppTableExportReportMeta,
} from "../AppTableExport.types";

type InboxRow = AppTableRow & {
  id: string;
  radicado: string;
};

describe("[SPEC:APPTABLE-EXPORT-15] AppTable export contracts", () => {
  it("exposes the supported export modes in a stable order", () => {
    expect(APP_TABLE_EXPORT_MODES).toEqual([
      "currentPage",
      "selectedRows",
      "allLoaded",
      "allMatching",
    ]);
  });

  it("exposes the supported export formats", () => {
    expect(APP_TABLE_EXPORT_FORMATS).toEqual(["csv", "xlsx", "pdf"]);
  });

  it("detects datasource capabilities per export mode", () => {
    const dataSource: AppTableExportDataSource<InboxRow> = {
      getCurrentPageRows: () => [{ id: "1", radicado: "RAD-1" }],
      getSelectedRows: () => [{ id: "1", radicado: "RAD-1" }],
      getAllMatchingRows: async () => [{ id: "1", radicado: "RAD-1" }],
    };

    expect(hasAppTableExportDataSourceCapability(dataSource, "currentPage")).toBe(true);
    expect(hasAppTableExportDataSourceCapability(dataSource, "selectedRows")).toBe(true);
    expect(hasAppTableExportDataSourceCapability(dataSource, "allLoaded")).toBe(false);
    expect(hasAppTableExportDataSourceCapability(dataSource, "allMatching")).toBe(true);
  });

  it("returns only enabled modes backed by the datasource", () => {
    const dataSource: AppTableExportDataSource<InboxRow> = {
      getCurrentPageRows: () => [],
      getAllLoadedRows: () => [],
      getBackendExportFile: async () => ({
        blob: new Blob(["export"], { type: "text/plain" }),
      }),
    };

    expect(
      getAvailableAppTableExportModes(dataSource, [
        "currentPage",
        "selectedRows",
        "allLoaded",
        "allMatching",
      ]),
    ).toEqual(["currentPage", "allLoaded", "allMatching"]);
  });

  it("routes currentPage and allMatching to backend when available", () => {
    const dataSource: AppTableExportDataSource<InboxRow> = {
      getCurrentPageRows: () => [{ id: "1", radicado: "RAD-1" }],
      getAllMatchingRows: async () => [{ id: "1", radicado: "RAD-1" }],
      getBackendExportFile: async () => ({
        blob: new Blob(["export"], { type: "text/plain" }),
      }),
    };

    expect(shouldUseBackendAppTableExport(dataSource, "csv", "currentPage")).toBe(true);
    expect(shouldUseBackendAppTableExport(dataSource, "xlsx", "currentPage")).toBe(true);
    expect(shouldUseBackendAppTableExport(dataSource, "xlsx", "allMatching")).toBe(true);
    expect(isAppTableExportExecutable(dataSource, "csv", "currentPage")).toBe(true);
    expect(isAppTableExportExecutable(dataSource, "xlsx", "currentPage")).toBe(true);
    expect(isAppTableExportExecutable(dataSource, "pdf", "allMatching")).toBe(true);
  });

  it("supports report metadata and generic props over AppTable rows", () => {
    const reportMeta: AppTableExportReportMeta = {
      reportName: "Bandeja de entrada",
      generatedBy: "Miguel Angel Urueta Miranda",
      moduleName: "Gestion Correspondencia",
      reportType: "Operativo",
      generatedAt: "2026-04-05T03:00:00.000Z",
      rowCount: 1,
      description: "Exportacion de prueba",
      companyImageAsset: "public/branding/reports/company-report-logo.png",
    };

    const props: AppTableExportProps<InboxRow> = {
      columns: [{ field: "radicado", headerName: "Radicado" }] satisfies ColDef<InboxRow>[],
      dataSource: {
        getCurrentPageRows: () => [{ id: "1", radicado: "RAD-1" }],
      },
      formats: ["xlsx"],
      reportMeta,
      enabledModes: ["currentPage"],
    };

    expect(props.reportMeta.companyImageAsset).toBe(
      "public/branding/reports/company-report-logo.png",
    );
    expect(props.dataSource.getCurrentPageRows()).toHaveLength(1);
  });
});
