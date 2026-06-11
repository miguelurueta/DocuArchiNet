import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppDigitalizador } from "./AppDigitalizador";
import type { DigitalizacionContext } from "../../../../modules/digitalizacion/types/digitalizacion.types";
import type {
  DigitalizacionScannerClient,
  ScanPage,
} from "../../../../modules/digitalizacion/infrastructure/dynamsoft";

const crearContext: DigitalizacionContext = {
  modo: "crear",
  nombreGabinete: "Gestion",
  radicado: "RAD-2026",
};

const createScannerClient = (): DigitalizacionScannerClient & { pages: ScanPage[] } => ({
  pages: [
    { id: "page-1", index: 0 },
    { id: "page-2", index: 1 },
  ],
  initialize: vi.fn(async () => undefined),
  listDevices: vi.fn(async () => [{ id: "0", name: "Scanner principal" }]),
  selectDevice: vi.fn(async () => undefined),
  scan: vi.fn(async function scan(this: { pages: ScanPage[] }) {
    return this.pages;
  }),
  rotatePage: vi.fn(async () => undefined),
  removePage: vi.fn(async () => undefined),
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

describe("[SPEC:SCRUMCORE-244] AppDigitalizador", () => {
  it("renders the reusable scanner surface without AppModal", async () => {
    const scannerClient = createScannerClient();

    render(
      <AppDigitalizador
        context={crearContext}
        scannerClient={scannerClient}
        onCompleted={vi.fn()}
      />,
    );

    expect(screen.getByTestId("app-digitalizador")).toBeInTheDocument();
    expect(screen.queryByRole("dialog")).not.toBeInTheDocument();

    await waitFor(() => {
      expect(screen.getByText("Scanner principal")).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText("Seleccionar scanner"), {
      target: { value: "0" },
    });
    await waitFor(() => {
      expect(scannerClient.selectDevice).toHaveBeenCalledWith("0");
    });

    fireEvent.click(screen.getByText("Escanear"));

    await waitFor(() => {
      expect(
        screen.getByText((_, element) => element?.textContent === "Miniaturas (2)"),
      ).toBeInTheDocument();
    });
  });

  it("stays mounted while inactive without initializing scanner", () => {
    const scannerClient = createScannerClient();

    render(
      <AppDigitalizador
        active={false}
        context={crearContext}
        scannerClient={scannerClient}
        onCompleted={vi.fn()}
      />,
    );

    expect(screen.getByTestId("app-digitalizador")).toHaveAttribute("data-active", "false");
    expect(scannerClient.initialize).not.toHaveBeenCalled();
  });
});
