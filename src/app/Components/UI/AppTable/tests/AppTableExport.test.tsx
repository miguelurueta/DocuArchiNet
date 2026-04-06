import { fireEvent, render, screen, waitFor } from "@testing-library/react";
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

describe("AppTableExport [SPEC:APPTABLE-EXPORT-17] [SPEC:APPTABLE-EXPORT-18] [SPEC:APPTABLE-EXPORT-20] [SPEC:22-FE-INTEGRAR-APPTABLEEXPORT-CON-API-APPTABLE-EXPORT-MD]", () => {
  let capturedBlob: Blob | null;
  let createObjectUrlSpy: ReturnType<typeof vi.fn>;
  let revokeObjectUrlSpy: ReturnType<typeof vi.fn>;
  let anchorClickSpy: () => void;

  beforeEach(() => {
    capturedBlob = null;
    createObjectUrlSpy = vi.fn((blob: Blob) => {
      capturedBlob = blob;
      return "blob:mock";
    });
    revokeObjectUrlSpy = vi.fn();
    anchorClickSpy = vi.fn();

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

  it("exporta allLoaded cuando el datasource lo soporta", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
          getAllLoadedRows: () => [
            { id: "1", name: "Alpha" },
            { id: "2", name: "Beta" },
          ],
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
        enabledModes={["currentPage", "allLoaded"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Todos los cargados"));

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

  it("no expone allLoaded cuando el datasource no implementa getAllLoadedRows", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
        enabledModes={["currentPage", "allLoaded"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));

    expect(await screen.findByText("Página actual")).toBeInTheDocument();
    expect(screen.queryByText("Todos los cargados")).not.toBeInTheDocument();
  });

  it("mantiene formatos ejecutivos como no ejecutables fuera de allMatching", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
          getAllMatchingRows: async () => [{ id: "1", name: "Alpha" }],
          getBackendExportFile: vi.fn().mockResolvedValue({
            blob: new Blob(["xlsx"], {
              type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            }),
            fileName: "gestion.xlsx",
          }),
        }}
        formats={["xlsx"]}
        reportMeta={reportMeta}
        enabledModes={["currentPage", "allMatching"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));

    expect(await screen.findByText("Exportar en Excel")).toBeInTheDocument();
    expect(screen.getByText("Página actual")).toBeInTheDocument();
    expect(screen.getByText("Todos los resultados")).toBeInTheDocument();

    fireEvent.click(screen.getByText("Página actual"));
    expect(createObjectUrlSpy).not.toHaveBeenCalled();
  });

  it("expone allMatching solo cuando el datasource async lo soporta y lo diferencia de allLoaded", async () => {
    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
          getAllLoadedRows: () => [{ id: "1", name: "Alpha" }],
          getAllMatchingRows: async () => [
            { id: "1", name: "Alpha" },
            { id: "2", name: "Beta" },
          ],
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
        enabledModes={["currentPage", "allLoaded", "allMatching"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));

    expect(await screen.findByText("Todos los cargados")).toBeInTheDocument();
    expect(screen.getByText("Todos los resultados")).toBeInTheDocument();
  });

  it("usa el datasource async remoto para exportar allMatching", async () => {
    const getAllMatchingRows = vi.fn().mockResolvedValue([
      { id: "1", name: "Alpha" },
      { id: "2", name: "Beta" },
    ]);

    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "9", name: "Visible" }],
          getAllMatchingRows,
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
        enabledModes={["allMatching"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Todos los resultados"));

    await waitFor(() => {
      expect(getAllMatchingRows).toHaveBeenCalledTimes(1);
    });
    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(capturedBlob).toBeTruthy();
  });

  it("recupera el estado interactivo cuando allMatching falla sin activar descarga", async () => {
    const errorSpy = vi.spyOn(console, "error").mockImplementation(() => undefined);
    const getAllMatchingRows = vi.fn()
      .mockRejectedValueOnce(new Error("remote export failed"))
      .mockResolvedValueOnce([{ id: "2", name: "Beta" }]);

    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Alpha" }],
          getAllMatchingRows,
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
        enabledModes={["allMatching"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Todos los resultados"));

    await waitFor(() => {
      expect(errorSpy).toHaveBeenCalledWith(
        "AppTable export failed",
        expect.any(Error),
      );
    });
    expect(createObjectUrlSpy).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Todos los resultados"));

    await waitFor(() => {
      expect(getAllMatchingRows).toHaveBeenCalledTimes(2);
      expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    });
  });

  it("usa la estrategia backend para descargar allMatching en formatos ejecutivos", async () => {
    const getBackendExportFile = vi.fn().mockResolvedValue({
      blob: new Blob(["xlsx"], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      }),
      fileName: "gestion.xlsx",
    });

    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Visible" }],
          getAllMatchingRows: async () => [{ id: "1", name: "Visible" }],
          getBackendExportFile,
        }}
        formats={["xlsx"]}
        reportMeta={reportMeta}
        enabledModes={["allMatching"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Todos los resultados"));

    await waitFor(() => {
      expect(getBackendExportFile).toHaveBeenCalledWith(
        expect.objectContaining({
          format: "xlsx",
          mode: "allMatching",
          reportMeta,
        }),
      );
    });
    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(anchorClickSpy).toHaveBeenCalledTimes(1);
  });

  it("recupera el estado interactivo cuando la exportacion backend falla", async () => {
    const errorSpy = vi.spyOn(console, "error").mockImplementation(() => undefined);
    const getBackendExportFile = vi.fn()
      .mockRejectedValueOnce(new Error("server export failed"))
      .mockResolvedValueOnce({
        blob: new Blob(["csv"], { type: "text/csv;charset=utf-8;" }),
        fileName: "gestion.csv",
      });

    render(
      <AppTableExport
        columns={columns}
        dataSource={{
          getCurrentPageRows: () => [{ id: "1", name: "Visible" }],
          getAllMatchingRows: async () => [{ id: "1", name: "Visible" }],
          getBackendExportFile,
        }}
        formats={["csv"]}
        reportMeta={reportMeta}
        enabledModes={["allMatching"]}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Todos los resultados"));

    await waitFor(() => {
      expect(errorSpy).toHaveBeenCalledWith(
        "AppTable export failed",
        expect.any(Error),
      );
    });
    expect(createObjectUrlSpy).not.toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    fireEvent.click(await screen.findByText("Todos los resultados"));

    await waitFor(() => {
      expect(getBackendExportFile).toHaveBeenCalledTimes(2);
      expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    });
  });
});
