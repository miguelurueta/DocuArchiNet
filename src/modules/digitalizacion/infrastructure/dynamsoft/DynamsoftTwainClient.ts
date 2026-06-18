import {
  DYNAMSOFT_ALLOWED_COLOR_MODES,
  DYNAMSOFT_CONTAINER_ID,
  DYNAMSOFT_DEFAULT_RESOURCES_PATH,
  DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
  DYNAMSOFT_MAX_RESOLUTION_DPI,
  DYNAMSOFT_MIN_RESOLUTION_DPI,
} from "./dynamsoft.constants";
import { DynamsoftScannerError } from "./dynamsoft.errors";
import { loadDynamsoftScripts } from "./loadDynamsoftScripts";
import type {
  AutomaticImageProcessingOptions,
  AutomaticImageProcessingResult,
  DigitalizacionScannerClient,
  DynamsoftRuntimeOptions,
  DynamsoftDevice,
  DynamsoftWebTwainObject,
  DynamsoftWindow,
  PdfGenerationResult,
  ScanColorMode,
  ScanOptions,
  ScanPage,
  ScannerDevice,
} from "./dynamsoft.types";

const WEB_TWAIN_ID = "digitalizacion-documental-dwt";
const DYNAMSOFT_OPERATION_TIMEOUT_MS = 15000;
const DYNAMSOFT_WEBTWAIN_READY_TIMEOUT_MS = 5000;
const DYNAMSOFT_WEBTWAIN_READY_POLL_INTERVAL_MS = 200;
const BLANK_PAGE_ANALYSIS_WIDTH = 96;
const BLANK_PAGE_ANALYSIS_HEIGHT = 128;
const BLANK_PAGE_WHITE_THRESHOLD = 245;
const BLANK_PAGE_CONTENT_RATIO_THRESHOLD = 0.003;
const BLANK_PAGE_DARK_RATIO_THRESHOLD = 0.0005;

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

const isDynamsoftCssLoadError = (error: unknown) => {
  if (!error || typeof error !== "object") {
    return false;
  }

  const record = error as { code?: unknown; message?: unknown };
  return (
    record.code === -2804 ||
    (typeof record.message === "string" &&
      record.message.includes("Loading the WebTwain css files failed"))
  );
};

const withDynamsoftTimeout = async <T,>(operation: Promise<T>, operationName: string) =>
  Promise.race([
    operation,
    new Promise<T>((_resolve, reject) => {
      window.setTimeout(() => {
        reject(
          new DynamsoftScannerError({
            code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
            message: `${operationName} no respondio dentro del tiempo esperado.`,
          }),
        );
      }, DYNAMSOFT_OPERATION_TIMEOUT_MS);
    }),
  ]);

const readDiagnosticValue = (target: unknown, keys: string[]) => {
  if (!target || typeof target !== "object") {
    return undefined;
  }

  const record = target as Record<string, unknown>;
  return keys.map((key) => record[key]).find((value) => value !== undefined);
};

const normalizeImageUrl = (url: string | false | undefined) =>
  typeof url === "string" && url.trim().length > 0 ? url : undefined;

const readImageDimension = (
  getter: ((index: number) => number) | undefined,
  index: number,
) => {
  if (!getter) {
    return undefined;
  }

  try {
    const value = getter(index);
    return Number.isFinite(value) && value > 0 ? value : undefined;
  } catch (error) {
    console.warn("PAGE_DIMENSIONS_ERROR", {
      index,
      message: error instanceof Error ? error.message : String(error),
    });
    return undefined;
  }
};

const getPageOrientation = (width?: number, height?: number) => {
  if (!width || !height) {
    return "unknown" as const;
  }

  if (width > height) {
    return "landscape" as const;
  }

  if (height > width) {
    return "portrait" as const;
  }

  return "square" as const;
};

const normalizePageIdSet = (pageIds: string[]) => new Set(pageIds);

const logDevelopmentMetric = (
  label: string,
  startedAt: number,
  metadata?: Record<string, unknown>,
) => {
  if (!import.meta.env.DEV) {
    return;
  }

  console.info(label, {
    durationMs: Math.round(performance.now() - startedAt),
    ...metadata,
  });
};

type ProcessingFeature = keyof AutomaticImageProcessingOptions;

const automaticProcessingFeatures: Array<{
  key: ProcessingFeature;
  timeLog: "DESKEW_TIME" | "AUTOCROP_TIME" | "AUTOROTATE_TIME";
  methods: string[];
}> = [
  {
    key: "deskew",
    timeLog: "DESKEW_TIME",
    methods: ["Deskew", "deskew", "DeskewImage", "AutoDeskew"],
  },
  {
    key: "autoCrop",
    timeLog: "AUTOCROP_TIME",
    methods: ["AutoCrop", "autoCrop", "AutoCropImage"],
  },
  {
    key: "autoRotate",
    timeLog: "AUTOROTATE_TIME",
    methods: ["AutoRotate", "autoRotate", "AutoRotateImage"],
  },
];

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
  private devices: ScannerDevice[] = [];
  private dwtDevices = new Map<string, DynamsoftDevice>();
  private modernDeviceIds = new Set<string>();
  private pageRotationById = new Map<string, number>();
  private originalPageDimensionsById = new Map<string, { width?: number; height?: number }>();

  constructor(options: DynamsoftRuntimeOptions = {}) {
    this.options = {
      scriptSrc: options.scriptSrc ?? "",
      resourcesPath: options.resourcesPath ?? DYNAMSOFT_DEFAULT_RESOURCES_PATH,
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
      resourcesPath: this.options.resourcesPath,
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
    runtime.ResourcesPath = this.options.resourcesPath.replace(/\/+$/, "");
    runtime.Containers = [
      {
        WebTwainId: WEB_TWAIN_ID,
        ContainerId: this.options.containerId,
        Width: "0px",
        Height: "0px",
      },
    ];
    try {
      await Promise.resolve(runtime.Load());
    } catch (error) {
      if (isDynamsoftCssLoadError(error)) {
        throw new DynamsoftScannerError({
          code: "DYNAMSOFT_CSS_LOAD_FAILED",
          message:
            "No fue posible cargar los estilos CSS de Dynamsoft Web TWAIN.",
        });
      }

      throw error;
    }

    const dwt = await this.waitForWebTwain(runtime);
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
    const sourceManagerDevices = this.listDevicesFromSourceManager(dwt);
    if (sourceManagerDevices.length > 0) {
      return sourceManagerDevices;
    }

    if (dwt.GetDevicesAsync) {
      try {
        const sourceDevices = await withDynamsoftTimeout(
          dwt.GetDevicesAsync(undefined, true),
          "GetDevicesAsync",
        );
        if (sourceDevices.length > 0) {
          const devices = sourceDevices.map((device, index) => {
            const name = device.displayName || device.name;
            return {
              id: String(index),
              name,
              index,
            };
          });

          this.devices = devices;
          this.dwtDevices = new Map(
            devices.map((device) => [device.id, sourceDevices[device.index]]),
          );
          this.modernDeviceIds = new Set(devices.map((device) => device.id));
          return devices;
        }
      } catch (error) {
        console.warn("GET_DEVICES_ASYNC_FALLBACK", error);
      }
    }

    return sourceManagerDevices;
  }

  private listDevicesFromSourceManager(dwt: DynamsoftWebTwainObject) {
    this.openSourceManager(dwt);
    const count = dwt.SourceCount ?? 0;
    const devices: ScannerDevice[] = [];

    for (let index = 0; index < count; index += 1) {
      const name = dwt.GetSourceNameItems(index);
      devices.push({
        id: String(index),
        name,
        index,
      });
    }

    this.devices = devices;
    this.modernDeviceIds.clear();
    this.dwtDevices = new Map(
      devices.map((device) => [
        device.id,
        {
          name: device.name,
          displayName: device.name,
        },
      ]),
    );
    return devices;
  }

  async selectDevice(deviceId: string) {
    assertValidDeviceId(deviceId);
    const dwt = this.requireDwt();
    const cachedDevice = this.devices.find((device) => device.id === deviceId);
    const dwtDevice = this.dwtDevices.get(deviceId);
    const deviceIndex = cachedDevice?.index ?? Number(deviceId);
    const sourceCount = dwt.SourceCount ?? 0;

    if (
      !Number.isInteger(deviceIndex) ||
      deviceIndex < 0 ||
      (!cachedDevice && deviceIndex >= sourceCount)
    ) {
      throw new DynamsoftScannerError({
        code: "SCANNER_NOT_FOUND",
        message: "Scanner no encontrado.",
      });
    }

    if (dwtDevice && dwt.SelectDeviceAsync && this.modernDeviceIds.has(deviceId)) {
      try {
        const selectDeviceResult = await withDynamsoftTimeout(
          dwt.SelectDeviceAsync(dwtDevice),
          "SelectDeviceAsync",
        );
        if (!selectDeviceResult) {
          throw new DynamsoftScannerError({
            code: "SCANNER_NOT_FOUND",
            message: "No fue posible seleccionar el scanner.",
          });
        }
      } catch (error) {
        console.error("SELECT_SOURCE_ERROR", error);
        throw error;
      }

      this.selectedDeviceId = deviceId;
      return;
    }

    this.openSourceManager(dwt);
    dwt.GetSourceNameItems(deviceIndex);
    let selectSourceResult = false;
    try {
      selectSourceResult = dwt.SelectSourceByIndex(deviceIndex);
    } catch (error) {
      console.error("SELECT_SOURCE_ERROR", error);
      throw error;
    }

    if (!selectSourceResult) {
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
      const acquireOptions = {
        IfShowUI: options.showScannerUi ?? false,
        PixelType: colorModeToPixelType[options.colorMode ?? "color"],
        Resolution: options.resolutionDpi ?? DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
        IfFeederEnabled: options.feederEnabled ?? true,
        IfDuplexEnabled: options.duplex ?? false,
        IfDisableSourceAfterAcquire: true,
      };
      await new Promise<void>((resolve, reject) => {
        dwt.OpenSource();
        dwt.AcquireImage(
          acquireOptions,
          () => {
            resolve();
          },
          (_code, message) => {
            reject(
              new DynamsoftScannerError({
                code: "SCAN_FAILED",
                message: message || "No fue posible completar el escaneo.",
              }),
            );
          },
        );
      });
      this.ensureNotStale(operationGeneration);

      const nextCount = dwt.HowManyImagesInBuffer ?? previousCount;
      this.pages = this.buildPagesFromBuffer(dwt, nextCount);
      if (options.removeBlankPages) {
        await this.removeDetectedBlankPages(dwt);
      }
      await this.applyAutomaticProcessing(dwt, options.automaticProcessing);

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
    const currentRotation = this.pageRotationById.get(pageId) ?? 0;
    const nextRotation = (currentRotation + degrees) % 360;
    this.pageRotationById.set(pageId, nextRotation);
    this.pages = this.pages.map((page) =>
      page.id === pageId ? this.buildPageFromBuffer(dwt, page.index) : page,
    );
    return [...this.pages];
  }

  async removePage(pageId: string) {
    const dwt = this.requireDwt();
    const pageIndex = this.getPageIndex(pageId);
    dwt.RemoveImage(pageIndex);
    this.pages = this.rebuildPagesAfterBufferRemoval(dwt, this.pages, [pageIndex], new Set([pageId]));
  }

  async reorderPages(pageIds: string[]) {
    const knownPageIds = new Set(this.pages.map((page) => page.id));
    const requestedPageIds = normalizePageIdSet(pageIds);
    const hasSameLength = pageIds.length === this.pages.length;
    const hasKnownPages = pageIds.every((pageId) => knownPageIds.has(pageId));
    const hasAllPages = this.pages.every((page) => requestedPageIds.has(page.id));

    if (!hasSameLength || !hasKnownPages || !hasAllPages) {
      throw new DynamsoftScannerError({
        code: "INVALID_PAGE_ORDER",
        message: "El orden de paginas no coincide con el lote capturado.",
      });
    }

    const byId = new Map(this.pages.map((page) => [page.id, page]));
    this.pages = pageIds.map((pageId) => byId.get(pageId)).filter((page): page is ScanPage => Boolean(page));
    return [...this.pages];
  }

  async clear() {
    const dwt = this.dwt;
    dwt?.RemoveAllImages();
    this.pages = [];
    this.pageRotationById.clear();
    this.originalPageDimensionsById.clear();
  }

  async generatePdf(fileName: string) {
    this.assertNoActiveOperation();
    const dwt = this.requireDwt();
    const pageIndices = this.getPdfPageIndices(dwt);
    const pageCount = pageIndices.length;

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
          pageIndices,
          "application/pdf",
          (nextBlob) => {
            resolve(nextBlob);
          },
          (_code, message) => {
            reject(
              new DynamsoftScannerError({
                code: "PDF_GENERATION_FAILED",
                message: message || "No fue posible generar el PDF.",
              }),
            );
          },
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
    this.devices = [];
    this.dwtDevices.clear();
    this.modernDeviceIds.clear();
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

  private openSourceManager(dwt: DynamsoftWebTwainObject) {
    if (!dwt.OpenSourceManager) {
      return undefined;
    }

    try {
      const opened = dwt.OpenSourceManager();
      return opened;
    } catch (error) {
      console.error("OPEN_SOURCE_MANAGER_ERROR", error);
      throw error;
    }
  }

  private buildPagesFromBuffer(dwt: DynamsoftWebTwainObject, count: number) {
    return Array.from({ length: Math.max(count, 0) }, (_item, index) =>
      this.buildPageFromBuffer(dwt, index),
    );
  }

  private buildPageFromBuffer(dwt: DynamsoftWebTwainObject, index: number): ScanPage {
    const thumbnailUrl = normalizeImageUrl(dwt.GetImageURL?.(index, 160, 220));
    const imageUrl = normalizeImageUrl(dwt.GetImageURL?.(index, -1, -1));
    const width = readImageDimension(dwt.GetImageWidth?.bind(dwt), index);
    const height = readImageDimension(dwt.GetImageHeight?.bind(dwt), index);
    const orientation = getPageOrientation(width, height);
    const pageId = `scan-page-${index + 1}`;
    const originalDimensions = this.originalPageDimensionsById.get(pageId) ?? {
      width,
      height,
    };
    this.originalPageDimensionsById.set(pageId, originalDimensions);
    const rotationDegrees = this.pageRotationById.get(pageId) ?? 0;
    const page: ScanPage = {
      id: pageId,
      index,
      ...(thumbnailUrl ? { thumbnailUrl } : {}),
      ...(imageUrl ? { imageUrl } : {}),
      ...(width ? { width } : {}),
      ...(height ? { height } : {}),
      orientation,
      rotationDegrees,
    };

    return page;
  }

  private async applyAutomaticProcessing(
    dwt: DynamsoftWebTwainObject,
    processing?: AutomaticImageProcessingOptions,
  ) {
    if (!processing) {
      return {};
    }

    const enabledFeatures = automaticProcessingFeatures.filter(
      (feature) => processing[feature.key],
    );
    if (enabledFeatures.length === 0 || this.pages.length === 0) {
      return {};
    }

    const result: AutomaticImageProcessingResult = {};
    for (const feature of enabledFeatures) {
      result[feature.key] = await this.applyAutomaticProcessingFeature(dwt, feature);
    }

    return result;
  }

  private async applyAutomaticProcessingFeature(
    dwt: DynamsoftWebTwainObject,
    feature: (typeof automaticProcessingFeatures)[number],
  ): Promise<NonNullable<AutomaticImageProcessingResult[ProcessingFeature]>> {
    const startedAt = performance.now();
    const method = this.findDwtProcessingMethod(dwt, feature.methods);
    const pageIds = this.pages.map((page) => page.id);

    if (!method) {
      const message = "native-api-unavailable";
      logDevelopmentMetric(feature.timeLog, startedAt, {
        pageCount: this.pages.length,
        status: "unsupported",
        message,
      });
      return {
        status: "unsupported",
        durationMs: Math.round(performance.now() - startedAt),
        message,
      };
    }

    try {
      for (const page of this.pages) {
        const output = method.call(dwt, page.index);
        if (output instanceof Promise) {
          await output;
        }
      }

      this.refreshPagesById(dwt, pageIds);
      const durationMs = Math.round(performance.now() - startedAt);
      logDevelopmentMetric(feature.timeLog, startedAt, {
        pageCount: pageIds.length,
        status: "applied",
        methodName: method.name || "anonymous",
      });
      return {
        status: "applied",
        durationMs,
      };
    } catch (error) {
      const durationMs = Math.round(performance.now() - startedAt);
      const message = error instanceof Error ? error.message : String(error);
      console.warn(feature.timeLog, {
        durationMs,
        pageCount: pageIds.length,
        status: "failed",
        message,
      });
      return {
        status: "failed",
        durationMs,
        message,
      };
    }
  }

  private findDwtProcessingMethod(dwt: DynamsoftWebTwainObject, candidates: string[]) {
    const record = dwt as Record<string, unknown>;
    const method = candidates
      .map((candidate) => record[candidate])
      .find((candidate): candidate is (index: number) => unknown => typeof candidate === "function");

    return method;
  }

  private refreshPagesById(dwt: DynamsoftWebTwainObject, pageIds: string[]) {
    const ids = new Set(pageIds);
    this.pages = this.pages.map((page) => {
      if (!ids.has(page.id)) {
        return page;
      }

      const refreshedPage = this.buildPageFromBuffer(dwt, page.index);
      return {
        ...refreshedPage,
        id: page.id,
        rotationDegrees: this.pageRotationById.get(page.id) ?? page.rotationDegrees ?? 0,
      };
    });
  }

  private async removeDetectedBlankPages(dwt: DynamsoftWebTwainObject) {
    const blankAnalyses = await Promise.all(
      this.pages.map((page) => this.analyzeBlankPageCandidate(page)),
    );
    const blankPages = blankAnalyses.filter((analysis) => analysis.isBlank);

    if (blankPages.length === 0) {
      return;
    }

    const blankPageIds = new Set(blankPages.map((analysis) => analysis.page.id));
    const blankIndexes = blankPages
      .map((analysis) => analysis.page.index)
      .sort((left, right) => right - left);

    blankIndexes.forEach((index) => {
      dwt.RemoveImage(index);
    });

    this.pages = this.rebuildPagesAfterBufferRemoval(dwt, this.pages, blankIndexes, blankPageIds);
  }

  private async analyzeBlankPageCandidate(page: ScanPage) {
    const imageUrl = page.thumbnailUrl ?? page.imageUrl;
    if (!imageUrl) {
      return {
        page,
        isBlank: false,
        contentRatio: 1,
        darkRatio: 1,
        reason: "image-url-unavailable",
      };
    }

    try {
      const image = await this.loadAnalysisImage(imageUrl);
      const canvas = this.options.documentRef.createElement("canvas");
      canvas.width = BLANK_PAGE_ANALYSIS_WIDTH;
      canvas.height = BLANK_PAGE_ANALYSIS_HEIGHT;
      const context = canvas.getContext("2d", { willReadFrequently: true });
      if (!context) {
        return {
          page,
          isBlank: false,
          contentRatio: 1,
          darkRatio: 1,
          reason: "canvas-context-unavailable",
        };
      }

      context.fillStyle = "#ffffff";
      context.fillRect(0, 0, canvas.width, canvas.height);
      context.drawImage(image, 0, 0, canvas.width, canvas.height);
      const pixels = context.getImageData(0, 0, canvas.width, canvas.height).data;
      let contentPixels = 0;
      let darkPixels = 0;
      const totalPixels = pixels.length / 4;

      for (let offset = 0; offset < pixels.length; offset += 4) {
        const red = pixels[offset] ?? 255;
        const green = pixels[offset + 1] ?? 255;
        const blue = pixels[offset + 2] ?? 255;
        const alpha = pixels[offset + 3] ?? 255;
        if (alpha < 12) {
          continue;
        }

        const luminance = 0.2126 * red + 0.7152 * green + 0.0722 * blue;
        if (
          red < BLANK_PAGE_WHITE_THRESHOLD ||
          green < BLANK_PAGE_WHITE_THRESHOLD ||
          blue < BLANK_PAGE_WHITE_THRESHOLD
        ) {
          contentPixels += 1;
        }
        if (luminance < 180) {
          darkPixels += 1;
        }
      }

      const contentRatio = contentPixels / totalPixels;
      const darkRatio = darkPixels / totalPixels;
      const isBlank =
        contentRatio <= BLANK_PAGE_CONTENT_RATIO_THRESHOLD &&
        darkRatio <= BLANK_PAGE_DARK_RATIO_THRESHOLD;

      return {
        page,
        isBlank,
        contentRatio,
        darkRatio,
        reason: isBlank ? "below-content-threshold" : "content-detected",
      };
    } catch (error) {
      console.warn("BLANK_PAGE_ANALYSIS_ERROR", {
        pageId: page.id,
        index: page.index,
        message: error instanceof Error ? error.message : String(error),
      });
      return {
        page,
        isBlank: false,
        contentRatio: 1,
        darkRatio: 1,
        reason: "analysis-failed",
      };
    }
  }

  private loadAnalysisImage(src: string) {
    return new Promise<HTMLImageElement>((resolve, reject) => {
      const ImageConstructor = this.options.windowRef.Image;
      const image = new ImageConstructor();
      image.onload = () => resolve(image);
      image.onerror = () => reject(new Error("No fue posible analizar la imagen escaneada."));
      image.src = src;
    });
  }

  private rebuildPagesAfterBufferRemoval(
    dwt: DynamsoftWebTwainObject,
    previousPages: ScanPage[],
    removedIndexes: number[],
    removedPageIds: Set<string>,
  ) {
    const sortedRemovedIndexes = [...removedIndexes].sort((left, right) => left - right);
    const nextPages = previousPages
      .filter((page) => !removedPageIds.has(page.id))
      .map((page) => {
        const removedBefore = sortedRemovedIndexes.filter((index) => index < page.index).length;
        const nextIndex = page.index - removedBefore;
        const nextPage = this.buildPageFromBuffer(dwt, nextIndex);
        return {
          ...nextPage,
          id: page.id,
          rotationDegrees: this.pageRotationById.get(page.id) ?? page.rotationDegrees ?? 0,
        };
      });

    this.pageRotationById = new Map(
      nextPages.map((page) => [page.id, page.rotationDegrees ?? 0]),
    );
    this.originalPageDimensionsById = new Map(
      nextPages.map((page) => [page.id, { width: page.width, height: page.height }]),
    );

    return nextPages;
  }

  private getPdfPageIndices(dwt: DynamsoftWebTwainObject) {
    const bufferPageCount = dwt.HowManyImagesInBuffer ?? this.pages.length;
    if (this.pages.length === bufferPageCount && this.pages.length > 0) {
      return this.pages.map((page) => page.index);
    }

    return Array.from({ length: bufferPageCount }, (_item, index) => index);
  }

  private async waitForWebTwain(runtime: DynamsoftWebTwainFactory) {
    const startedAt = Date.now();
    let lastDwt: DynamsoftWebTwainObject | null = null;

    while (Date.now() - startedAt <= DYNAMSOFT_WEBTWAIN_READY_TIMEOUT_MS) {
      lastDwt = runtime.GetWebTwain(WEB_TWAIN_ID);

      if (
        lastDwt &&
        readDiagnosticValue(lastDwt, ["_destroy"]) !== true &&
        readDiagnosticValue(lastDwt, ["_bReady"]) !== false
      ) {
        return lastDwt;
      }

      await new Promise((resolve) => {
        window.setTimeout(resolve, DYNAMSOFT_WEBTWAIN_READY_POLL_INTERVAL_MS);
      });
    }
    return null;
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
