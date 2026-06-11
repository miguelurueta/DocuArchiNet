import { render, screen, waitFor } from "@testing-library/react";
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
  listDevices: vi.fn().mockResolvedValue([{ id: "scanner-1", name: "Scanner prueba" }]),
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
});
