import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { DigitalizacionDocumentalModal } from "./DigitalizacionDocumentalModal";
import { DigitalizacionDocumentalWorkspace } from "../DigitalizacionDocumentalWorkspace";
import type { DigitalizacionContext } from "../../types/digitalizacion.types";
import {
  DYNAMSOFT_CONTAINER_ID,
  type DigitalizacionScannerClient,
  type ScanPage,
} from "../../infrastructure/dynamsoft";

const baseProps = {
  open: true,
  onClose: vi.fn(),
  onCompleted: vi.fn(),
};

const crearContext: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "Gestion",
  radicado: "RAD-2026",
};

const adjuntarContext: DigitalizacionContext = {
  modo: "adjuntar",
  nombreGabinete: "Gestion",
  radicado: "RAD-2026",
  idDocumentoDestino: 321,
};

const createScannerClient = (): DigitalizacionScannerClient & { pages: ScanPage[] } => ({
  pages: [
    { id: "page-1", index: 0 },
    { id: "page-2", index: 1 },
  ],
  initialize: vi.fn(async () => undefined),
  listDevices: vi.fn(async () => [{ id: "0", name: "Scanner principal", index: 0 }]),
  selectDevice: vi.fn(async () => undefined),
  scan: vi.fn(async function scan(this: { pages: ScanPage[] }) {
    return this.pages;
  }),
  duplicatePage: vi.fn(async function duplicatePage(this: { pages: ScanPage[] }, pageId: string) {
    const sourceIndex = this.pages.findIndex((page) => page.id === pageId);
    const sourcePage = this.pages[sourceIndex];
    if (sourcePage) {
      this.pages.splice(sourceIndex + 1, 0, {
        ...sourcePage,
        id: `${sourcePage.id}-copy`,
        index: this.pages.length,
      });
    }
    return this.pages;
  }),
  rotatePage: vi.fn(async function rotatePage(this: { pages: ScanPage[] }) {
    return this.pages;
  }),
  cropPage: vi.fn(async function cropPage(this: { pages: ScanPage[] }) {
    return this.pages;
  }),
  removePage: vi.fn(async () => undefined),
  reorderPages: vi.fn(async function reorderPages(this: { pages: ScanPage[] }, pageIds: string[]) {
    this.pages = pageIds
      .map((pageId) => this.pages.find((page) => page.id === pageId))
      .filter((page): page is ScanPage => Boolean(page));
    return this.pages;
  }),
  clear: vi.fn(async () => undefined),
  generatePdf: vi.fn(
    async () =>
      ({
        file: new File(["pdf"], "digitalizacion.pdf", { type: "application/pdf" }),
        pageCount: 2,
      }) satisfies Awaited<ReturnType<DigitalizacionScannerClient["generatePdf"]>>,
  ),
  dispose: vi.fn(async () => undefined),
});

describe("[SPEC:SCRUMCORE-239] DigitalizacionDocumentalModal", () => {
  it("renders crear mode", () => {
    render(<DigitalizacionDocumentalModal {...baseProps} context={crearContext} />);

    expect(screen.getByTestId("digitalizacion-workspace")).toBeInTheDocument();
    expect(screen.getByRole("dialog")).toBeInTheDocument();
    expect(screen.getByText("crear")).toBeInTheDocument();
    expect(screen.getAllByText("Guardar documento").length).toBeGreaterThanOrEqual(1);
    expect(screen.getByText("RAD-2026")).toBeInTheDocument();
  });

  it("renders adjuntar mode", () => {
    render(<DigitalizacionDocumentalModal {...baseProps} context={adjuntarContext} />);

    expect(screen.getByText("adjuntar")).toBeInTheDocument();
    expect(screen.getAllByText("Adjuntar digitalizacion").length).toBeGreaterThanOrEqual(1);
    expect(screen.getByText("321")).toBeInTheDocument();
  });

  it("renders workspace inline without modal dialog", () => {
    render(
      <DigitalizacionDocumentalWorkspace
        context={crearContext}
        onCompleted={baseProps.onCompleted}
      />,
    );

    expect(screen.getByTestId("digitalizacion-workspace")).toBeInTheDocument();
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();
    expect(screen.getByText("crear")).toBeInTheDocument();
  });

  it("renders Dynamsoft container before scanner initialization", async () => {
    const scannerClient = createScannerClient();
    vi.mocked(scannerClient.initialize).mockImplementation(async () => {
      expect(document.getElementById(DYNAMSOFT_CONTAINER_ID)).toBeInTheDocument();
    });

    render(
      <DigitalizacionDocumentalWorkspace
        context={crearContext}
        scannerClient={scannerClient}
        onCompleted={baseProps.onCompleted}
      />,
    );

    expect(document.getElementById(DYNAMSOFT_CONTAINER_ID)).toBeInTheDocument();
    await waitFor(() => {
      expect(scannerClient.initialize).toHaveBeenCalled();
    });
  });

  it("shows controlled error for null context", () => {
    const onError = vi.fn();

    render(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={null}
        onError={onError}
      />,
    );

    expect(screen.getByRole("alert")).toHaveTextContent(
      "El contexto documental es obligatorio.",
    );
    expect(onError).toHaveBeenCalledWith(
      expect.objectContaining({ code: "CONTEXT_REQUIRED" }),
    );
  });

  it("shows required idDocumentoDestino for adjuntar", () => {
    render(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={{ modo: "adjuntar", nombreGabinete: "Gestion" }}
      />,
    );

    expect(screen.getByRole("alert")).toHaveTextContent(
      "idDocumentoDestino es obligatorio para modo adjuntar.",
    );
  });

  it("cancel completes with cancelado and closes", () => {
    const onClose = vi.fn();
    const onCompleted = vi.fn();

    render(
      <DigitalizacionDocumentalModal
        open
        context={crearContext}
        onClose={onClose}
        onCompleted={onCompleted}
      />,
    );

    fireEvent.click(screen.getByText("Cancelar"));

    expect(onCompleted).toHaveBeenCalledWith({ accion: "cancelado" });
    expect(onClose).toHaveBeenCalled();
  });

  it("clears previous context data when context changes", () => {
    const { rerender } = render(
      <DigitalizacionDocumentalModal {...baseProps} context={crearContext} />,
    );

    expect(screen.getByText("RAD-2026")).toBeInTheDocument();

    rerender(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={{
          modo: "crear",
          nombreGabinete: "Archivo",
          radicado: "RAD-NEW",
        }}
      />,
    );

    expect(screen.queryByText("RAD-2026")).not.toBeInTheDocument();
    expect(screen.getByText("RAD-NEW")).toBeInTheDocument();
  });

  it("renders scanner devices and captured pages from scanner hook", async () => {
    const scannerClient = createScannerClient();

    render(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={crearContext}
        scannerClient={scannerClient}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Scanner principal")).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText("Seleccionar scanner"), {
      target: { value: "0" },
    });
    await waitFor(() => {
      expect(scannerClient.selectDevice).toHaveBeenCalledWith("0");
      expect(screen.getByRole("button", { name: "Escanear" })).not.toBeDisabled();
    });

    fireEvent.click(screen.getByRole("button", { name: "Escanear" }));

    await waitFor(() => {
      expect(screen.getByRole("heading", { name: "Miniaturas (2)" })).toBeInTheDocument();
    });
    expect(screen.getAllByText("Pagina 1").length).toBeGreaterThanOrEqual(1);
    expect(screen.getByText("Pagina 2")).toBeInTheDocument();
  }, 20000);

  it("[SPEC:SCRUMCORE-264] exposes capture operation toolbar and forwards operation intent", async () => {
    const scannerClient = createScannerClient();

    render(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={crearContext}
        scannerClient={scannerClient}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Scanner principal")).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText("Seleccionar scanner"), {
      target: { value: "0" },
    });
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Escanear" })).not.toBeDisabled();
    });

    fireEvent.click(screen.getByRole("button", { name: "Escanear" }));
    await waitFor(() => {
      expect(screen.getByRole("heading", { name: "Miniaturas (2)" })).toBeInTheDocument();
    });

    fireEvent.click(screen.getByRole("button", { name: "Reemplazar" }));
    await waitFor(() => {
      expect(scannerClient.scan).toHaveBeenCalledWith(
        expect.objectContaining({
          captureOperation: { type: "REPLACE", targetPageId: "page-1" },
        }),
      );
      expect(screen.getByRole("button", { name: "Agregar" })).not.toBeDisabled();
    });

    fireEvent.click(screen.getByRole("button", { name: "Agregar" }));
    await waitFor(() => {
      expect(scannerClient.scan).toHaveBeenCalledWith(
        expect.objectContaining({ captureOperation: { type: "APPEND" } }),
      );
      expect(screen.getByRole("button", { name: "Insertar paginas" })).not.toBeDisabled();
    });

    fireEvent.click(screen.getByRole("button", { name: "Insertar paginas" }));
    fireEvent.click(await screen.findByText("Insertar despues"));

    expect(scannerClient.scan).toHaveBeenCalledWith(
      expect.not.objectContaining({ captureOperation: expect.anything() }),
    );
    expect(scannerClient.scan).toHaveBeenCalledWith(
      expect.objectContaining({
        captureOperation: { type: "INSERT_AFTER", targetPageId: "page-1" },
      }),
    );
  }, 20000);

  it("rotates, removes and generates pdf from selected page", async () => {
    const scannerClient = createScannerClient();

    render(
      <DigitalizacionDocumentalModal
        {...baseProps}
        context={crearContext}
        scannerClient={scannerClient}
      />,
    );

    await waitFor(() => {
      expect(screen.getByText("Scanner principal")).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText("Seleccionar scanner"), {
      target: { value: "0" },
    });
    await waitFor(() => {
      expect(scannerClient.selectDevice).toHaveBeenCalledWith("0");
      expect(screen.getByRole("button", { name: "Escanear" })).not.toBeDisabled();
    });

    fireEvent.click(screen.getByRole("button", { name: "Escanear" }));

    await waitFor(() => {
      expect(screen.getAllByText("Pagina 1").length).toBeGreaterThanOrEqual(1);
    });

    fireEvent.click(screen.getByRole("button", { name: "Rotar derecha" }));
    fireEvent.click(screen.getByRole("button", { name: "Eliminar pagina" }));
    fireEvent.click(screen.getByRole("button", { name: "Generar PDF" }));

    expect(scannerClient.rotatePage).toHaveBeenCalledWith("page-1", 90);
    expect(scannerClient.removePage).toHaveBeenCalledWith("page-1");
    expect(scannerClient.generatePdf).toHaveBeenCalled();
  }, 10000);
});
