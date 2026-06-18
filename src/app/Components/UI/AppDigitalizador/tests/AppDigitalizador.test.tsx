import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
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

const panelPreferencesStorageKey = "docuarchi:digitalizacion:panel-preferences";

const installLocalStorageMock = () => {
  const storage = new Map<string, string>();

  Object.defineProperty(window, "localStorage", {
    configurable: true,
    value: {
      getItem: vi.fn((key: string) => storage.get(key) ?? null),
      setItem: vi.fn((key: string, value: string) => {
        storage.set(key, value);
      }),
      removeItem: vi.fn((key: string) => {
        storage.delete(key);
      }),
      clear: vi.fn(() => {
        storage.clear();
      }),
    },
  });
};

const createScannerClient = (
  pages: ScanPage[] = [{ id: "page-1", index: 0 }],
): DigitalizacionScannerClient => ({
  initialize: vi.fn().mockResolvedValue(undefined),
  listDevices: vi.fn().mockResolvedValue([
    { id: "scanner-1", name: "Scanner prueba", index: 0 },
  ]),
  selectDevice: vi.fn().mockResolvedValue(undefined),
  scan: vi.fn(() => Promise.resolve<ScanPage[]>(pages)),
  rotatePage: vi.fn(() => Promise.resolve<ScanPage[]>(pages)),
  removePage: vi.fn().mockResolvedValue(undefined),
  reorderPages: vi.fn(async (pageIds: string[]) =>
    pageIds.map((pageId, index) => ({ id: pageId, index })),
  ),
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
  beforeEach(() => {
    installLocalStorageMock();
    Object.defineProperty(HTMLElement.prototype, "scrollIntoView", {
      configurable: true,
      value: vi.fn(),
    });
  });

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
    const toolbar = screen.getByRole("toolbar", { name: "Herramientas de digitalizacion" });
    expect(toolbar).toBeInTheDocument();
    expect(within(toolbar).getByRole("button", { name: "Escanear" })).toBeInTheDocument();
    expect(within(toolbar).getByRole("button", { name: "Generar PDF" })).toBeInTheDocument();
    expect(within(toolbar).queryByText("Escanear")).toBeNull();
    expect(within(toolbar).queryByText("Generar PDF")).toBeNull();
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
    fireEvent.click(screen.getByLabelText("Eliminar paginas en blanco"));
    fireEvent.click(screen.getByLabelText("Deskew"));
    fireEvent.click(screen.getByLabelText("Auto Crop"));
    fireEvent.click(screen.getByLabelText("Auto Rotate"));
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
        removeBlankPages: true,
        automaticProcessing: {
          deskew: true,
          autoCrop: true,
          autoRotate: true,
        },
      }),
    );
  });

  it("[SPEC:SCRUMCORE-254] contrae paneles laterales y expande el preview sin desmontar el workspace", async () => {
    const scannerClient = createScannerClient();

    render(
      <AppDigitalizador
        context={context}
        scannerClient={scannerClient}
        onCompleted={vi.fn()}
      />,
    );

    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());

    const workspace = screen.getByTestId("digitalizacion-workspace");
    const main = workspace.querySelector("main");

    expect(main).toHaveAttribute("data-thumbnails-collapsed", "false");
    expect(main).toHaveAttribute("data-configuration-collapsed", "false");

    fireEvent.click(screen.getByRole("button", { name: /Ocultar Miniaturas/ }));
    fireEvent.click(screen.getByRole("button", { name: "Ocultar Configuracion de Escaneo" }));

    expect(main).toHaveAttribute("data-thumbnails-collapsed", "true");
    expect(main).toHaveAttribute("data-configuration-collapsed", "true");
    expect(screen.getByRole("region", { name: "Preview digitalizacion" })).toBeInTheDocument();
    expect(document.getElementById("digitalizacion-thumbnails-panel")).toBeInTheDocument();
    expect(document.getElementById("digitalizacion-configuration-panel")).toBeInTheDocument();
    expect(JSON.parse(window.localStorage.getItem(panelPreferencesStorageKey) ?? "{}")).toEqual({
      showThumbnails: false,
      showConfiguration: false,
    });
    expect(scannerClient.initialize).toHaveBeenCalledTimes(1);
    expect(scannerClient.scan).not.toHaveBeenCalled();
    expect(scannerClient.clear).not.toHaveBeenCalled();
    expect(scannerClient.dispose).not.toHaveBeenCalled();
  });

  it("[SPEC:SCRUMCORE-254] restaura paneles colapsados desde localStorage", async () => {
    window.localStorage.setItem(
      panelPreferencesStorageKey,
      JSON.stringify({ showThumbnails: false, showConfiguration: true }),
    );

    render(
      <AppDigitalizador
        context={context}
        scannerClient={createScannerClient()}
        onCompleted={vi.fn()}
      />,
    );

    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());

    const workspace = screen.getByTestId("digitalizacion-workspace");
    const main = workspace.querySelector("main");

    expect(main).toHaveAttribute("data-thumbnails-collapsed", "true");
    expect(main).toHaveAttribute("data-configuration-collapsed", "false");
    expect(screen.getAllByRole("button", { name: /Mostrar Miniaturas/ }).length).toBeGreaterThan(0);
    expect(
      screen.getByRole("button", { name: "Ocultar Configuracion de Escaneo" }),
    ).toBeInTheDocument();
  });

  it("[SPEC:SCRUMCORE-255] navega a una pagina especifica con seleccion, scroll y highlight", async () => {
    const scannerClient = createScannerClient([
      { id: "page-1", index: 0 },
      { id: "page-2", index: 1 },
      { id: "page-3", index: 2 },
      { id: "page-4", index: 3 },
      { id: "page-5", index: 4 },
    ]);

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

    fireEvent.click(screen.getByRole("button", { name: "Escanear" }));

    await waitFor(() => {
      expect(screen.getByRole("heading", { name: "Miniaturas (5)" })).toBeInTheDocument();
    });

    fireEvent.change(screen.getByLabelText("Pagina destino"), {
      target: { value: "5" },
    });
    fireEvent.click(screen.getByRole("button", { name: "Ir a pagina" }));

    const pageFiveButtons = screen.getAllByRole("button").filter((button) =>
      button.textContent?.includes("Pagina 5"),
    );
    const pageFiveThumbnail = pageFiveButtons.find((button) =>
      button.className.includes("thumbnailButton"),
    );

    if (!pageFiveThumbnail) {
      throw new Error("No se encontro la miniatura de Pagina 5.");
    }

    expect(pageFiveThumbnail).toHaveAttribute("data-selected", "true");
    expect(pageFiveThumbnail).toHaveAttribute("data-highlighted", "true");
    expect(HTMLElement.prototype.scrollIntoView).toHaveBeenCalledWith({
      block: "nearest",
      inline: "nearest",
    });
    expect(screen.getAllByText("Pagina 5").length).toBeGreaterThanOrEqual(1);
  });

  it("[SPEC:SCRUMCORE-255] enfoca el control de pagina con Ctrl+G", async () => {
    render(
      <AppDigitalizador
        context={context}
        scannerClient={createScannerClient([
          { id: "page-1", index: 0 },
          { id: "page-2", index: 1 },
        ])}
        onCompleted={vi.fn()}
      />,
    );

    await waitFor(() => expect(screen.getByText("Scanner prueba")).toBeInTheDocument());

    fireEvent.change(screen.getByLabelText("Seleccionar scanner"), {
      target: { value: "scanner-1" },
    });
    await waitFor(() => {
      expect(screen.getByRole("button", { name: "Escanear" })).not.toBeDisabled();
    });
    fireEvent.click(screen.getByRole("button", { name: "Escanear" }));
    await waitFor(() => {
      expect(screen.getByRole("heading", { name: "Miniaturas (2)" })).toBeInTheDocument();
    });

    fireEvent.keyDown(document, { key: "g", ctrlKey: true });

    expect(screen.getByLabelText("Pagina destino")).toHaveFocus();
  });
});
