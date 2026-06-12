import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppDigitalizador } from "../AppDigitalizador";
import { AppDigitalizadorProvider } from "../AppDigitalizadorProvider";
import type {
  DigitalizacionContext,
  DigitalizacionScannerClient,
  PdfGenerationResult,
  ScanPage,
} from "../../../../../modules/digitalizacion";

const context: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "DOCUARCHI_TEST",
  radicado: "RAD-1",
};

const createScannerClient = (): DigitalizacionScannerClient => ({
  initialize: vi.fn().mockResolvedValue(undefined),
  listDevices: vi.fn().mockResolvedValue([
    { id: "scanner-1", name: "Scanner prueba", index: 0 },
  ]),
  selectDevice: vi.fn().mockResolvedValue(undefined),
  scan: vi.fn(() => Promise.resolve<ScanPage[]>([{ id: "page-1", index: 0 }])),
  rotatePage: vi.fn().mockResolvedValue(undefined),
  removePage: vi.fn().mockResolvedValue(undefined),
  clear: vi.fn().mockResolvedValue(undefined),
  generatePdf: vi.fn(() =>
    Promise.resolve<PdfGenerationResult>({
      file: new File(["pdf"], "digitalizacion.pdf", { type: "application/pdf" }),
      pageCount: 1,
    }),
  ),
  dispose: vi.fn().mockResolvedValue(undefined),
});

describe("AppDigitalizador", () => {
  it("renderiza inline el workspace sin AppModal", async () => {
    const scannerClient = createScannerClient();

    render(
      <AppDigitalizador
        context={context}
        scannerClient={scannerClient}
        onCompleted={vi.fn()}
      />,
    );

    expect(screen.getByTestId("app-digitalizador")).toBeInTheDocument();
    expect(screen.getByTestId("digitalizacion-workspace")).toBeInTheDocument();
    expect(screen.queryByRole("dialog")).toBeNull();
    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());
  });

  it("inyecta sourceModule desde modulo cuando el contexto no lo trae", async () => {
    render(
      <AppDigitalizador
        context={context}
        modulo="CapDocument"
        scannerClient={createScannerClient()}
        onCompleted={vi.fn()}
      />,
    );

    expect(screen.getByTestId("app-digitalizador")).toHaveAttribute(
      "data-module",
      "CapDocument",
    );
    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());
  });

  it("crea scannerClient desde el provider corporativo", async () => {
    const createScannerClientFromProvider = vi.fn(() => createScannerClient());

    render(
      <AppDigitalizadorProvider
        dynamsoft={{ licenseKey: "license-from-provider" }}
        createScannerClient={createScannerClientFromProvider}
      >
        <AppDigitalizador context={context} onCompleted={vi.fn()} />
      </AppDigitalizadorProvider>,
    );

    expect(createScannerClientFromProvider).toHaveBeenCalledWith({
      licenseKey: "license-from-provider",
    });
    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());
  });

  it("mantiene singleton el scannerClient durante la vida de AppDigitalizador", async () => {
    const createScannerClientFromProvider = vi.fn(() => createScannerClient());
    const { rerender } = render(
      <AppDigitalizadorProvider
        dynamsoft={{ licenseKey: "license-from-provider" }}
        createScannerClient={createScannerClientFromProvider}
      >
        <AppDigitalizador context={context} onCompleted={vi.fn()} />
      </AppDigitalizadorProvider>,
    );

    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());

    rerender(
      <AppDigitalizadorProvider
        dynamsoft={{ licenseKey: "license-from-provider" }}
        createScannerClient={createScannerClientFromProvider}
      >
        <AppDigitalizador
          context={{ ...context, radicado: "RAD-2" }}
          onCompleted={vi.fn()}
        />
      </AppDigitalizadorProvider>,
    );

    expect(createScannerClientFromProvider).toHaveBeenCalledTimes(1);
  });

  it("envia configuracion DocuArchi al escanear", async () => {
    const scannerClient = createScannerClient();

    render(
      <AppDigitalizador
        context={context}
        scannerClient={scannerClient}
        onCompleted={vi.fn()}
      />,
    );

    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());

    fireEvent.change(screen.getByLabelText("Seleccionar scanner"), {
      target: { value: "scanner-1" },
    });
    await waitFor(() =>
      expect(scannerClient.selectDevice).toHaveBeenCalledWith("scanner-1"),
    );
    fireEvent.click(screen.getByLabelText("Duplex activado"));
    fireEvent.change(screen.getByLabelText("Color"), {
      target: { value: "grayscale" },
    });
    fireEvent.change(screen.getByLabelText("Resolucion"), {
      target: { value: "300" },
    });

    fireEvent.click(screen.getByRole("button", { name: "Escanear" }));

    await waitFor(() =>
      expect(scannerClient.scan).toHaveBeenCalledWith({
        deviceId: "scanner-1",
        colorMode: "grayscale",
        duplex: true,
        feederEnabled: true,
        resolutionDpi: 300,
        showScannerUi: false,
      }),
    );
  });
});
