import { fireEvent, render, screen } from "@testing-library/react";
import type { ColDef } from "ag-grid-community";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { AppTableExport } from "../AppTableExport";
import type { AppTableExportReportMeta } from "../AppTableExport.types";

type Row = {
  id: string;
  name: string;
  acciones?: string;
};

const reportMeta: AppTableExportReportMeta = {
  reportName: "Bandeja de entrada",
  generatedBy: "Miguel Urueta",
  moduleName: "Gestion Correspondencia",
  reportType: "Operativo",
  generatedAt: "2026-04-05T05:00:00.000Z",
  rowCount: 0,
  description: "Exportacion local",
  companyImageAsset: "public/branding/reports/company-report-logo.png",
};

const columns: ColDef<Row>[] = [
  { field: "name", headerName: "Nombre" },
  {
    field: "acciones",
    headerName: "Acciones",
    cellRendererParams: {
      actions: [{ id: "ver" }],
    },
  },
];

describe("AppTableExport", () => {
  let capturedBlob: Blob | null;
  let createObjectUrlSpy: ReturnType<typeof vi.fn>;
  let revokeObjectUrlSpy: ReturnType<typeof vi.fn>;
  let anchorClickSpy: ReturnType<typeof vi.fn<() => void>>;

  beforeEach(() => {
    capturedBlob = null;
    createObjectUrlSpy = vi.fn((blob: Blob) => {
      capturedBlob = blob;
      return "blob:mock";
    });
    revokeObjectUrlSpy = vi.fn();
    anchorClickSpy = vi.fn<() => void>();

    vi.stubGlobal("URL", {
      createObjectURL: createObjectUrlSpy,
      revokeObjectURL: revokeObjectUrlSpy,
    });
    vi.spyOn(HTMLAnchorElement.prototype, "click").mockImplementation(() => {
      anchorClickSpy();
    });
  });

  afterEach(() => {
    vi.restoreAllMocks();
    vi.unstubAllGlobals();
  });

  it("exporta currentPage a csv usando solo columnas con datos", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha", acciones: "Ver" }],
          getSelectedRows: () => [],
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Página actual"));

    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(anchorClickSpy).toHaveBeenCalledTimes(1);
    expect(revokeObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(capturedBlob).toBeTruthy();
  });

  it("exporta selectedRows cuando la seleccion existe", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
          getSelectedRows: () => [{ id: "2", name: "Beta" }],
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Seleccionados"));

    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(capturedBlob).toBeTruthy();
  });

  it("deshabilita selectedRows cuando no hay seleccion", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
          getSelectedRows: () => [],
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Seleccionados (sin selección)"));

    expect(createObjectUrlSpy).not.toHaveBeenCalled();
  });

  it("solo expone currentPage cuando selectedRows no esta soportado por el datasource", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));

    expect(await screen.findByText("Página actual")).toBeInTheDocument();
    expect(screen.queryByText("Seleccionados")).not.toBeInTheDocument();
  });
});
