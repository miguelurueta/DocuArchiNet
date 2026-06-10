import {
  DYNAMSOFT_ALLOWED_COLOR_MODES,
  DYNAMSOFT_CONTAINER_ID,
  DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
  DYNAMSOFT_MAX_RESOLUTION_DPI,
  DYNAMSOFT_MIN_RESOLUTION_DPI,
} from "./dynamsoft.constants";
import { DynamsoftScannerError } from "./dynamsoft.errors";
import { loadDynamsoftScripts } from "./loadDynamsoftScripts";
import type {
  DigitalizacionScannerClient,
  DynamsoftRuntimeOptions,
  DynamsoftWebTwainObject,
  DynamsoftWindow,
  PdfGenerationResult,
  ScanColorMode,
  ScanOptions,
  ScanPage,
  ScannerDevice,
} from "./dynamsoft.types";

const WEB_TWAIN_ID = "digitalizacion-documental-dwt";

const colorModeToPixelType: Record<ScanColorMode, number> = {
  blackWhite: 0,
  grayscale: 1,
  color: 2,
};

const isAllowedColorMode = (value: unknown): value is ScanColorMode =>
  typeof value === "string" &&
  DYNAMSOFT_ALLOWED_COLOR_MODES.includes(value as ScanColorMode);

const assertValidDeviceId = (deviceId: string) => {
  if (deviceId.trim().length === 0) {
    throw new DynamsoftScannerError({
      code: "INVALID_DEVICE_ID",
      message: "deviceId es obligatorio.",
    });
  }
};

const assertValidScanOptions = (options: ScanOptions) => {
  assertValidDeviceId(options.deviceId);

  if (
    options.resolutionDpi !== undefined &&
    (!Number.isInteger(options.resolutionDpi) ||
      options.resolutionDpi < DYNAMSOFT_MIN_RESOLUTION_DPI ||
      options.resolutionDpi > DYNAMSOFT_MAX_RESOLUTION_DPI)
  ) {
    throw new DynamsoftScannerError({
      code: "INVALID_SCAN_OPTIONS",
      message: "resolutionDpi no es valido.",
    });
  }

  if (options.colorMode !== undefined && !isAllowedColorMode(options.colorMode)) {
    throw new DynamsoftScannerError({
      code: "INVALID_SCAN_OPTIONS",
      message: "colorMode no es valido.",
    });
  }

  if (options.duplex !== undefined && typeof options.duplex !== "boolean") {
    throw new DynamsoftScannerError({
      code: "INVALID_SCAN_OPTIONS",
      message: "duplex no es valido.",
    });
  }
};

const assertPdfResult = (file: File, pageCount: number) => {
  if (pageCount <= 0) {
    throw new DynamsoftScannerError({
      code: "PDF_EMPTY",
      message: "No hay paginas para generar PDF.",
    });
  }

  if (
    file.size <= 0 ||
    file.type !== "application/pdf" ||
    !file.name.toLowerCase().endsWith(".pdf")
  ) {
    throw new DynamsoftScannerError({
      code: "PDF_GENERATION_FAILED",
      message: "El PDF generado no cumple el contrato requerido.",
    });
  }
};

export class DynamsoftTwainClient implements DigitalizacionScannerClient {
  private readonly options: Required<Omit<DynamsoftRuntimeOptions, "licenseKey">> & {
    licenseKey?: string;
  };

  private dwt: DynamsoftWebTwainObject | null = null;
  private selectedDeviceId: string | null = null;
  private disposed = false;
  private activeOperation: "scan" | "generatePdf" | null = null;
  private generation = 0;
  private pages: ScanPage[] = [];

  constructor(options: DynamsoftRuntimeOptions = {}) {
    this.options = {
      scriptSrc: options.scriptSrc ?? "",
      licenseKey: options.licenseKey,
      containerId: options.containerId ?? DYNAMSOFT_CONTAINER_ID,
      documentRef: options.documentRef ?? document,
      windowRef: options.windowRef ?? window,
    };
  }

  async initialize() {
    this.disposed = false;
    const operationGeneration = this.generation;
    await loadDynamsoftScripts({
      scriptSrc: this.options.scriptSrc || undefined,
      documentRef: this.options.documentRef,
    });
    this.ensureNotStale(operationGeneration);

    const runtime = (this.options.windowRef as DynamsoftWindow).Dynamsoft?.DWT;
    if (!runtime) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "Dynamsoft runtime no esta disponible.",
      });
    }

    if (!this.options.licenseKey?.trim()) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_LICENSE_INVALID",
        message: "Licencia Dynamsoft no configurada.",
        recoverable: false,
      });
    }

    runtime.ProductKey = this.options.licenseKey;
    runtime.Containers = [
      {
        WebTwainId: WEB_TWAIN_ID,
        ContainerId: this.options.containerId,
        Width: "0px",
        Height: "0px",
      },
    ];
    runtime.Load();

    const dwt = runtime.GetWebTwain(WEB_TWAIN_ID);
    if (!dwt) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "No fue posible inicializar Dynamsoft Web TWAIN.",
      });
    }

    this.dwt = dwt;
  }

  async listDevices() {
    const dwt = this.requireDwt();
    const count = dwt.SourceCount ?? 0;
    const devices: ScannerDevice[] = [];

    for (let index = 0; index < count; index += 1) {
      devices.push({
        id: String(index),
        name: dwt.GetSourceNameItems(index),
      });
    }

    return devices;
  }

  async selectDevice(deviceId: string) {
    assertValidDeviceId(deviceId);
    const dwt = this.requireDwt();
    const deviceIndex = Number(deviceId);
    const sourceCount = dwt.SourceCount ?? 0;

    if (!Number.isInteger(deviceIndex) || deviceIndex < 0 || deviceIndex >= sourceCount) {
      throw new DynamsoftScannerError({
        code: "SCANNER_NOT_FOUND",
        message: "Scanner no encontrado.",
      });
    }

    if (!dwt.SelectSourceByIndex(deviceIndex)) {
      throw new DynamsoftScannerError({
        code: "SCANNER_NOT_FOUND",
        message: "No fue posible seleccionar el scanner.",
      });
    }

    this.selectedDeviceId = deviceId;
  }

  async scan(options: ScanOptions) {
    this.assertNoActiveOperation();
    assertValidScanOptions(options);

    if (this.selectedDeviceId !== options.deviceId) {
      throw new DynamsoftScannerError({
        code: "SCANNER_NOT_SELECTED",
        message: "Seleccione un scanner antes de escanear.",
      });
    }

    const dwt = this.requireDwt();
    const operationGeneration = this.generation;
    this.activeOperation = "scan";

    try {
      const previousCount = dwt.HowManyImagesInBuffer ?? 0;
      await new Promise<void>((resolve, reject) => {
        dwt.OpenSource();
        dwt.AcquireImage(
          {
            IfShowUI: false,
            PixelType: colorModeToPixelType[options.colorMode ?? "color"],
            Resolution: options.resolutionDpi ?? DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
            IfFeederEnabled: true,
            IfDuplexEnabled: options.duplex ?? false,
            IfDisableSourceAfterAcquire: true,
          },
          resolve,
          (_code, message) =>
            reject(
              new DynamsoftScannerError({
                code: "SCAN_FAILED",
                message: message || "No fue posible completar el escaneo.",
              }),
            ),
        );
      });
      this.ensureNotStale(operationGeneration);

      const nextCount = dwt.HowManyImagesInBuffer ?? previousCount;
      this.pages = Array.from({ length: Math.max(nextCount, 0) }, (_item, index) => ({
        id: `scan-page-${index + 1}`,
        index,
      }));

      return [...this.pages];
    } finally {
      dwt.CloseSource?.();
      this.activeOperation = null;
    }
  }

  async rotatePage(pageId: string, degrees: 90 | 180 | 270) {
    const dwt = this.requireDwt();
    const pageIndex = this.getPageIndex(pageId);
    dwt.Rotate(pageIndex, degrees, true);
  }

  async removePage(pageId: string) {
    const dwt = this.requireDwt();
    const pageIndex = this.getPageIndex(pageId);
    dwt.RemoveImage(pageIndex);
    this.pages = this.pages
      .filter((page) => page.id !== pageId)
      .map((page, index) => ({ ...page, index }));
  }

  async clear() {
    const dwt = this.dwt;
    dwt?.RemoveAllImages();
    this.pages = [];
  }

  async generatePdf(fileName: string) {
    this.assertNoActiveOperation();
    const dwt = this.requireDwt();
    const pageCount = dwt.HowManyImagesInBuffer ?? this.pages.length;

    if (pageCount <= 0) {
      throw new DynamsoftScannerError({
        code: "PDF_EMPTY",
        message: "No hay paginas para generar PDF.",
      });
    }

    const operationGeneration = this.generation;
    this.activeOperation = "generatePdf";

    try {
      const blob = await new Promise<Blob>((resolve, reject) => {
        dwt.ConvertToBlob(
          Array.from({ length: pageCount }, (_item, index) => index),
          "application/pdf",
          resolve,
          (_code, message) =>
            reject(
              new DynamsoftScannerError({
                code: "PDF_GENERATION_FAILED",
                message: message || "No fue posible generar el PDF.",
              }),
            ),
        );
      });
      this.ensureNotStale(operationGeneration);

      const normalizedFileName = fileName.toLowerCase().endsWith(".pdf")
        ? fileName
        : `${fileName}.pdf`;
      const file = new File([blob], normalizedFileName, { type: "application/pdf" });
      const result: PdfGenerationResult = { file, pageCount };
      assertPdfResult(result.file, result.pageCount);

      return result;
    } finally {
      this.activeOperation = null;
    }
  }

  async dispose() {
    this.generation += 1;
    this.disposed = true;
    this.activeOperation = null;
    this.selectedDeviceId = null;
    this.pages = [];
    this.dwt?.CloseSource?.();
    this.dwt = null;
    (this.options.windowRef as DynamsoftWindow).Dynamsoft?.DWT?.Unload?.();
  }

  private requireDwt() {
    if (this.disposed || !this.dwt) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "Dynamsoft runtime no esta disponible.",
      });
    }

    return this.dwt;
  }

  private assertNoActiveOperation() {
    if (this.activeOperation) {
      throw new DynamsoftScannerError({
        code: "SCAN_IN_PROGRESS",
        message: "Ya existe una operacion de scanner en curso.",
      });
    }
  }

  private ensureNotStale(operationGeneration: number) {
    if (this.disposed || operationGeneration !== this.generation) {
      throw new DynamsoftScannerError({
        code: "STALE_OPERATION_IGNORED",
        message: "Operacion scanner obsoleta ignorada.",
      });
    }
  }

  private getPageIndex(pageId: string) {
    const pageIndex = this.pages.findIndex((page) => page.id === pageId);
    if (pageIndex === -1) {
      throw new DynamsoftScannerError({
        code: "PDF_EMPTY",
        message: "Pagina no encontrada.",
      });
    }

    return pageIndex;
  }
}
