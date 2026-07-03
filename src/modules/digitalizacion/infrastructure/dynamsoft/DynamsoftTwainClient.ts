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
  DynamsoftWebTwainFactory,
  DynamsoftWebTwainObject,
  DynamsoftImageType,
  DynamsoftWindow,
  PdfGenerationResult,
  PageCropSelection,
  ScanColorMode,
  ScanOptions,
  ScanPage,
  ScanProgressListener,
  ScannerDevice,
} from "./dynamsoft.types";

const WEB_TWAIN_ID = "digitalizacion-documental-dwt";
const DYNAMSOFT_OPERATION_TIMEOUT_MS = 15000;
const DYNAMSOFT_WEBTWAIN_READY_TIMEOUT_MS = 5000;
const DYNAMSOFT_WEBTWAIN_READY_POLL_INTERVAL_MS = 200;
const BLANK_PAGE_ANALYSIS_WIDTH = 384;
const BLANK_PAGE_ANALYSIS_HEIGHT = 512;
const BLANK_PAGE_WHITE_THRESHOLD = 245;
const BLANK_PAGE_CONTENT_RATIO_THRESHOLD = 0.003;
const BLANK_PAGE_DARK_PIXEL_THRESHOLD = 12;
const BLANK_PAGE_BORDER_FRACTION_TO_IGNORE = 0.06;
const BLANK_PAGE_DARK_RATIO_THRESHOLD = 0.002;
const BLANK_PAGE_LOW_CONTRAST_LUMINANCE_THRESHOLD = 230;
const BLANK_PAGE_LOW_CONTRAST_VARIANCE_THRESHOLD = 30;
const BLANK_PAGE_LOW_CONTRAST_CONTENT_RATIO_MULTIPLIER = 1.2;
const BLANK_PAGE_EDGE_DEVIATION_THRESHOLD = 28;
const BLANK_PAGE_EDGE_RATIO_THRESHOLD = 0.002;
const BLANK_PAGE_WHITE_PERCENTILE = 0.95;
const BLANK_PAGE_DYNAMIC_WHITE_THRESHOLD_FLOOR = 225;
const BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_THRESHOLD = 220;
const BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_MAX_STDDEV = 28;
const BLANK_PAGE_ASYNC_MIN_BLOCK_HEIGHT = 20;
const BLANK_PAGE_ASYNC_MAX_BLOCK_HEIGHT = 30;

type BlankPageAnalysis = {
  page: ScanPage;
  isBlank: boolean;
  contentRatio: number;
  darkPixels: number;
  clusteredDarkPixels: number;
  darkRatio: number;
  reason: string;
  imageSource: "original" | "thumbnail" | "unavailable";
};

type BlankPageRemovalResult = {
  analyses: BlankPageAnalysis[];
  detected: BlankPageAnalysis[];
  removedPageIds: Set<string>;
  requestedIndexes: number[];
  removedIndexes: number[];
  survivedIndexes: number[];
};

type DynamsoftBlankPageConfig = {
  ifAutoDiscardBlankpagesKey?: string;
  ifAutoDiscardBlankpages: unknown;
  blankImageThreshold?: number;
  blankImageMaxStdDev?: number;
  hasAnyConfig: boolean;
};

const logBlankPageDiagnostic = (label: string, payload: Record<string, unknown>) => {
  console.info(label, payload);
};

const countClusteredDarkPixels = (darkMask: boolean[], width: number, height: number) => {
  let clusteredDarkPixels = 0;

  for (let y = 0; y < height; y += 1) {
    for (let x = 0; x < width; x += 1) {
      const index = y * width + x;
      if (!darkMask[index]) {
        continue;
      }

      let hasDarkNeighbor = false;
      for (let dy = -1; dy <= 1 && !hasDarkNeighbor; dy += 1) {
        for (let dx = -1; dx <= 1; dx += 1) {
          if (dx === 0 && dy === 0) {
            continue;
          }

          const neighborX = x + dx;
          const neighborY = y + dy;
          if (
            neighborX >= 0 &&
            neighborX < width &&
            neighborY >= 0 &&
            neighborY < height &&
            darkMask[neighborY * width + neighborX]
          ) {
            hasDarkNeighbor = true;
            break;
          }
        }
      }

      if (hasDarkNeighbor) {
        clusteredDarkPixels += 1;
      }
    }
  }

  return clusteredDarkPixels;
};

const getLuminancePercentile = (
  histogram: Int32Array,
  totalCount: number,
  percentile: number,
) => {
  if (totalCount <= 0) {
    return 0;
  }

  const targetCount = Math.ceil(totalCount * percentile);
  let cumulativeCount = 0;

  for (let index = 0; index < histogram.length; index += 1) {
    cumulativeCount += histogram[index];
    if (cumulativeCount >= targetCount) {
      return index;
    }
  }

  return histogram.length - 1;
};

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

const assertValidCropSelection = (selection: PageCropSelection) => {
  const values = [selection.x, selection.y, selection.width, selection.height];
  if (values.some((value) => !Number.isFinite(value)) || selection.width <= 0 || selection.height <= 0) {
    throw new DynamsoftScannerError({
      code: "INVALID_SCAN_OPTIONS",
      message: "El area de recorte no es valida.",
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
  progressStage: "applyingDeskew" | "applyingAutoCrop" | "applyingAutoRotate";
  progressLabel: string;
  timeLog: "DESKEW_TIME" | "AUTOCROP_TIME" | "AUTOROTATE_TIME";
  methods: string[];
}> = [
  {
    key: "deskew",
    progressStage: "applyingDeskew",
    progressLabel: "Aplicando Deskew",
    timeLog: "DESKEW_TIME",
    methods: ["Deskew", "deskew", "DeskewImage", "AutoDeskew"],
  },
  {
    key: "autoCrop",
    progressStage: "applyingAutoCrop",
    progressLabel: "Aplicando Auto Crop",
    timeLog: "AUTOCROP_TIME",
    methods: ["AutoCrop", "autoCrop", "AutoCropImage"],
  },
  {
    key: "autoRotate",
    progressStage: "applyingAutoRotate",
    progressLabel: "Aplicando Auto Rotate",
    timeLog: "AUTOROTATE_TIME",
    methods: ["AutoRotate", "autoRotate", "AutoRotateImage"],
  },
];

type ScanPipelinePerfRecord = {
  scanId: string;
  scanStartedAt: number;
  stages: {
    acquireImageMs?: number;
    buildPagesFromBufferMs?: number;
    blankDetectionMs?: number;
    deskewMs?: number;
    autoCropMs?: number;
    autoRotateMs?: number;
    reactFirstRenderMs?: number;
  };
};

type ScanPipelinePerfWindow = Window & {
  __docuarchiScanPipelinePerf?: ScanPipelinePerfRecord;
};

const readScanPipelinePerfRecord = () =>
  typeof window === "undefined"
    ? null
    : ((window as ScanPipelinePerfWindow).__docuarchiScanPipelinePerf ?? null);

const initScanPipelinePerfRecord = () => {
  if (typeof window === "undefined") {
    return null;
  }

  const scanId = `scan-${Date.now()}-${Math.random().toString(36).slice(2, 10)}`;
  const record: ScanPipelinePerfRecord = {
    scanId,
    scanStartedAt: performance.now(),
    stages: {},
  };

  (window as ScanPipelinePerfWindow).__docuarchiScanPipelinePerf = record;
  console.info("[SCAN PERF] scan started", {
    scanId,
    startedAt: record.scanStartedAt,
  });

  return record;
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
        console.error(error);
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
      console.error(error);
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
    const previousPages = [...this.pages];
    this.activeOperation = "scan";
    let blankPageRuntimeConfig = this.configureDynamsoftBlankPageDetection(dwt);
    const pipelinePerf = initScanPipelinePerfRecord();

    try {
      const acquireImageStart = performance.now();
      const previousCount = dwt.HowManyImagesInBuffer ?? 0;
      this.reportScanProgress(options.onProgress, {
        stage: "acquiring",
        label: "Escaneando documentos",
        detail: options.showScannerUi
          ? "El avance nativo depende del dialogo PaperStream."
          : "Esperando paginas desde Dynamsoft Web TWAIN.",
        cancellable: true,
      });
      const acquireOptions: Record<string, unknown> & {
        IfShowUI: boolean;
        PixelType: number;
        Resolution: number;
        IfFeederEnabled: boolean;
        IfDuplexEnabled: boolean;
        IfDisableSourceAfterAcquire: boolean;
      } = {
        IfShowUI: options.showScannerUi ?? false,
        PixelType: colorModeToPixelType[options.colorMode ?? "color"],
        Resolution: options.resolutionDpi ?? DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
        IfFeederEnabled: options.feederEnabled ?? true,
        IfDuplexEnabled: options.duplex ?? false,
        IfDisableSourceAfterAcquire: true,
        IfAutoDiscardBlankpages: options.removeBlankPages ?? false,
      };
      await new Promise<void>((resolve, reject) => {
        dwt.OpenSource();
        blankPageRuntimeConfig =
          this.configureDynamsoftBlankPageDetection(dwt) ?? blankPageRuntimeConfig;
        this.applyDynamsoftBlankPageDetection(dwt, options, blankPageRuntimeConfig);
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
      const acquireImageDuration = performance.now() - acquireImageStart;
      if (pipelinePerf) {
        pipelinePerf.stages.acquireImageMs = acquireImageDuration;
      }
      if (pipelinePerf) {
        console.info("[SCAN PERF] AcquireImage", {
          scanId: pipelinePerf.scanId,
          durationMs: Math.round(acquireImageDuration),
        });
      }
      this.ensureNotStale(operationGeneration);

      const buildPagesFromBufferStart = performance.now();
      const nextCount = dwt.HowManyImagesInBuffer ?? previousCount;
      this.reportScanProgress(options.onProgress, {
        stage: "processingImages",
        label: "Procesando imagenes",
        detail: `${Math.max(nextCount - previousCount, 0)} paginas recibidas.`,
        currentPage: Math.max(nextCount - previousCount, 0),
        totalPages: Math.max(nextCount - previousCount, 0) || undefined,
        progress: 35,
        cancellable: false,
      });
      const shouldReusePages = options.captureOperation?.type === "APPEND";
      this.pages = this.buildPagesFromBuffer(dwt, nextCount, previousPages, {
        reusePreviousPages: shouldReusePages,
      });
      const buildPagesFromBufferDuration = performance.now() - buildPagesFromBufferStart;
      if (pipelinePerf) {
        pipelinePerf.stages.buildPagesFromBufferMs = buildPagesFromBufferDuration;
      }
      if (pipelinePerf) {
        console.info("[SCAN PERF] buildPagesFromBuffer", {
          scanId: pipelinePerf.scanId,
          durationMs: Math.round(buildPagesFromBufferDuration),
        });
      }
      if (options.removeBlankPages) {
        logBlankPageDiagnostic("BLANK_PAGE_FINAL_STATE", {
          stage: "buildPagesFromBuffer",
          pageCount: this.pages.length,
          bufferCount: dwt.HowManyImagesInBuffer ?? null,
          pages: this.summarizePagesForDiagnostics(this.pages),
        });
      }
      let blankRemovalResult: BlankPageRemovalResult | null = null;
      let blankDetectionDuration = 0;
      if (options.removeBlankPages) {
        const blankDetectionStart = performance.now();
        this.reportScanProgress(options.onProgress, {
          stage: "removingBlankPages",
          label: "Eliminando paginas en blanco",
          detail: `${this.pages.length} paginas por analizar.`,
          currentPage: 0,
          totalPages: this.pages.length,
          progress: 45,
          cancellable: false,
        });
        const nativeBlankRemovalResult = await this.removeDetectedBlankPagesWithDynamsoft(dwt, {
          startIndex: previousCount,
          endIndex: nextCount,
        });
        if (nativeBlankRemovalResult) {
          blankRemovalResult = nativeBlankRemovalResult;
        } else {
          blankRemovalResult = await this.removeDetectedBlankPages(dwt);
        }
        blankDetectionDuration = performance.now() - blankDetectionStart;
        if (pipelinePerf) {
          console.info("[SCAN PERF] Blank Detection", {
            scanId: pipelinePerf.scanId,
            durationMs: Math.round(blankDetectionDuration),
          });
        }
      } else {
        blankDetectionDuration = 0;
        if (pipelinePerf) {
          console.info("[SCAN PERF] Blank Detection", {
            scanId: pipelinePerf.scanId,
            durationMs: 0,
            status: "disabled",
          });
        }
      }
      if (pipelinePerf) {
        pipelinePerf.stages.blankDetectionMs = blankDetectionDuration;
      }
      await this.applyAutomaticProcessing(dwt, options.automaticProcessing, options.onProgress);
      const scannedPages = this.pages;
      const shouldFilterPreviousPagesForOperation =
        options.captureOperation &&
        options.captureOperation.type !== "NEW" &&
        options.removeBlankPages;
      const previousPagesForOperation = shouldFilterPreviousPagesForOperation
        ? (() => {
            const scannedPageIds = new Set(scannedPages.map((scannedPage) => scannedPage.id));
            return previousPages.filter((page) => scannedPageIds.has(page.id));
          })()
        : previousPages;

      this.pages = this.resolveCaptureOperationPages({
        operation: options.captureOperation,
        previousPages: previousPagesForOperation,
        scannedPages,
      });
      this.reportScanProgress(options.onProgress, {
        stage: "preparingDocument",
        label: "Preparando documento",
        detail: `${this.pages.length} paginas listas para preview.`,
        currentPage: this.pages.length,
        totalPages: this.pages.length,
        progress: 95,
        cancellable: false,
      });
      if (options.removeBlankPages) {
        this.logBlankPageReinsertions("afterAutomaticProcessing", blankRemovalResult);
        logBlankPageDiagnostic("BLANK_PAGE_FINAL_STATE", {
          stage: "scannerClientReturn",
          pageCount: this.pages.length,
          bufferCount: dwt.HowManyImagesInBuffer ?? null,
          pages: this.summarizePagesForDiagnostics(this.pages),
        });
      }
      return [...this.pages];
    } finally {
      console.info("scan(): entered finally");
      this.restoreDynamsoftBlankPageDetection(dwt, blankPageRuntimeConfig);
      dwt.CloseSource?.();
      this.activeOperation = null;
    }
  }

  private configureDynamsoftBlankPageDetection(dwt: DynamsoftWebTwainObject) {
    const dwtObject = dwt as Record<string, unknown>;
    const ifAutoDiscardBlankpages = "IfAutoDiscardBlankpages";
    const config: DynamsoftBlankPageConfig = {
      ifAutoDiscardBlankpagesKey: ifAutoDiscardBlankpages,
      ifAutoDiscardBlankpages: dwtObject[ifAutoDiscardBlankpages],
      blankImageThreshold:
        typeof dwtObject.BlankImageThreshold === "number"
          ? (dwtObject.BlankImageThreshold as number)
          : undefined,
      blankImageMaxStdDev:
        typeof dwtObject.BlankImageMaxStdDev === "number"
          ? (dwtObject.BlankImageMaxStdDev as number)
          : undefined,
      hasAnyConfig: false,
    };

    if (ifAutoDiscardBlankpages in dwtObject) {
      config.hasAnyConfig = true;
    }
    if ("BlankImageThreshold" in dwtObject) {
      config.hasAnyConfig = true;
    }
    if ("BlankImageMaxStdDev" in dwtObject) {
      config.hasAnyConfig = true;
    }

    return config.hasAnyConfig ? config : null;
  }

  private applyDynamsoftBlankPageDetection(
    dwt: DynamsoftWebTwainObject,
    options: ScanOptions,
    blankPageRuntimeConfig: DynamsoftBlankPageConfig | null,
  ) {
    if (!options.removeBlankPages || !blankPageRuntimeConfig) {
      return;
    }

    const dwtObject = dwt as Record<string, unknown>;

    if (blankPageRuntimeConfig.ifAutoDiscardBlankpagesKey) {
      dwtObject[blankPageRuntimeConfig.ifAutoDiscardBlankpagesKey] = true;
    }

    if ("BlankImageThreshold" in dwtObject) {
      dwtObject.BlankImageThreshold = BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_THRESHOLD;
    }

    if ("BlankImageMaxStdDev" in dwtObject) {
      dwtObject.BlankImageMaxStdDev = BLANK_PAGE_DYNAMSOFT_BLANK_IMAGE_MAX_STDDEV;
    }
  }

  private restoreDynamsoftBlankPageDetection(
    dwt: DynamsoftWebTwainObject,
    blankPageRuntimeConfig: DynamsoftBlankPageConfig | null,
  ) {
    console.info("restoreDynamsoftBlankPageDetection(): entered");
    if (!blankPageRuntimeConfig || !blankPageRuntimeConfig.hasAnyConfig) {
      console.info("restoreDynamsoftBlankPageDetection(): skipped no config");
      return;
    }

    const dwtObject = dwt as Record<string, unknown>;

    if (blankPageRuntimeConfig.ifAutoDiscardBlankpagesKey) {
      const property = blankPageRuntimeConfig.ifAutoDiscardBlankpagesKey;
      const previousValue = dwtObject[property];
      const nextValue = blankPageRuntimeConfig.ifAutoDiscardBlankpages;
      console.info("restore", property, "previous", previousValue, "attempt", nextValue);
      try {
        dwtObject[property] = nextValue;
        console.info("restore", property, "OK");
      } catch (error) {
        console.error(error);
        console.error(error instanceof Error ? error.stack : "NO_STACK");
      }
    }

    if (
      "BlankImageThreshold" in dwtObject &&
      blankPageRuntimeConfig.blankImageThreshold !== undefined
    ) {
      const property = "BlankImageThreshold";
      const previousValue = dwtObject.BlankImageThreshold;
      const nextValue = blankPageRuntimeConfig.blankImageThreshold;
      console.info("restore", property, "previous", previousValue, "attempt", nextValue);
      try {
        dwtObject.BlankImageThreshold = nextValue;
        console.info("restore", property, "OK");
      } catch (error) {
        console.error(error);
        console.error(error instanceof Error ? error.stack : "NO_STACK");
      }
    }

    if (
      "BlankImageMaxStdDev" in dwtObject &&
      blankPageRuntimeConfig.blankImageMaxStdDev !== undefined
    ) {
      const property = "BlankImageMaxStdDev";
      const previousValue = dwtObject.BlankImageMaxStdDev;
      const nextValue = blankPageRuntimeConfig.blankImageMaxStdDev;
      console.info("restore", property, "previous", previousValue, "attempt", nextValue);
      try {
        dwtObject.BlankImageMaxStdDev = nextValue;
        console.info("restore", property, "OK");
      } catch (error) {
        console.error(error);
        console.error(error instanceof Error ? error.stack : "NO_STACK");
      }
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

  async deskewPage(pageId: string) {
    const dwt = this.requireDwt();
    const page = this.pages.find((currentPage) => currentPage.id === pageId);
    if (!page) {
      throw new DynamsoftScannerError({
        code: "PDF_EMPTY",
        message: "Pagina no encontrada.",
      });
    }

    const deskewFeature = automaticProcessingFeatures.find(
      (feature) => feature.key === "deskew",
    );
    if (!deskewFeature) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "Deskew manual no esta disponible.",
      });
    }

    const previousPages = this.pages;
    this.pages = [page];
    try {
      await this.applyAutomaticProcessingFeature(dwt, deskewFeature);
    } finally {
      const processedPage = this.pages.find((currentPage) => currentPage.id === pageId) ?? page;
      this.pages = previousPages.map((currentPage) =>
        currentPage.id === pageId ? processedPage : currentPage,
      );
    }

    return [...this.pages];
  }

  async cropPage(pageId: string, selection: PageCropSelection) {
    assertValidCropSelection(selection);
    const dwt = this.requireDwt();
    if (!dwt.Crop) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "El recorte manual no esta disponible en este runtime de Dynamsoft.",
      });
    }

    const pageIndex = this.getPageIndex(pageId);
    const left = Math.max(0, Math.floor(selection.x));
    const top = Math.max(0, Math.floor(selection.y));
    const right = Math.ceil(selection.x + selection.width);
    const bottom = Math.ceil(selection.y + selection.height);
    const cropResult = dwt.Crop(pageIndex, left, top, right, bottom);
    if (cropResult === false) {
      throw new DynamsoftScannerError({
        code: "INVALID_SCAN_OPTIONS",
        message: "No fue posible aplicar el recorte manual.",
      });
    }

    this.pages = this.pages.map((page) =>
      page.id === pageId
        ? {
            ...this.buildPageFromBuffer(dwt, page.index),
            id: page.id,
            rotationDegrees: this.pageRotationById.get(page.id) ?? page.rotationDegrees ?? 0,
          }
        : page,
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

  async duplicatePage(pageId: string) {
    const dwt = this.requireDwt();
    const sourceVisualIndex = this.pages.findIndex((page) => page.id === pageId);
    const sourcePage = this.pages[sourceVisualIndex];

    if (!sourcePage) {
      throw new DynamsoftScannerError({
        code: "PDF_EMPTY",
        message: "Pagina no encontrada.",
      });
    }

    if (!dwt.CopyToClipboard || !dwt.LoadDibFromClipboard) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "La duplicacion de paginas no esta disponible en este runtime de Dynamsoft.",
      });
    }

    const previousCount = dwt.HowManyImagesInBuffer ?? this.pages.length;
    const copied = dwt.CopyToClipboard(sourcePage.index);
    if (copied === false) {
      throw new DynamsoftScannerError({
        code: "INVALID_SCAN_OPTIONS",
        message: "No fue posible copiar la pagina seleccionada.",
      });
    }

    const loaded = dwt.LoadDibFromClipboard();
    const nextCount = dwt.HowManyImagesInBuffer ?? previousCount + 1;
    if (loaded === false || nextCount <= previousCount) {
      throw new DynamsoftScannerError({
        code: "INVALID_SCAN_OPTIONS",
        message: "No fue posible insertar la pagina duplicada.",
      });
    }

    const duplicateIndex = nextCount - 1;
    const duplicatePage = {
      ...this.buildPageFromBuffer(dwt, duplicateIndex),
      rotationDegrees: this.pageRotationById.get(sourcePage.id) ?? sourcePage.rotationDegrees ?? 0,
    };

    this.pageRotationById.set(duplicatePage.id, duplicatePage.rotationDegrees ?? 0);
    this.originalPageDimensionsById.set(duplicatePage.id, {
      width: sourcePage.width,
      height: sourcePage.height,
    });
    this.pages = [
      ...this.pages.slice(0, sourceVisualIndex + 1),
      duplicatePage,
      ...this.pages.slice(sourceVisualIndex + 1),
    ];

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
      const convertToBlob = async (
        targetPageIndices: number[],
        type: DynamsoftImageType,
      ): Promise<Blob> =>
        new Promise<Blob>((resolve, reject) => {
          dwt.ConvertToBlob(
            targetPageIndices,
            type,
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

      const runtimePdfType = (dwt as unknown as {
        EnumDWT_ImageType?: { IT_PDF?: DynamsoftImageType };
      }).EnumDWT_ImageType?.IT_PDF;
      const globalPdfType = (
        (this.options.windowRef as DynamsoftWindow).Dynamsoft?.DWT as unknown as {
          EnumDWT_ImageType?: { IT_PDF?: DynamsoftImageType };
        }
      )?.EnumDWT_ImageType?.IT_PDF;
      const pdfImageTypes: DynamsoftImageType[] = [
        runtimePdfType,
        globalPdfType,
        "application/pdf",
      ].filter((value): value is DynamsoftImageType => value !== undefined);

      const pdfImageTypesSet = Array.from(new Set(pdfImageTypes));
      let lastError: unknown = null;
      let blob: Blob | null = null;

      for (const imageType of pdfImageTypesSet) {
        try {
          blob = await convertToBlob(pageIndices, imageType);
          break;
        } catch (error) {
          lastError = error;
          if (typeof console !== "undefined") {
            console.error("[generatePdf][attemptError]", imageType, error);
          }
          if (!(error instanceof DynamsoftScannerError)) {
            throw error;
          }
          if (!error.message.toLowerCase().includes("image type is not supported")) {
            throw error;
          }
          continue;
        }
      }

      if (!blob) {
        try {
          const pageBlobs = await Promise.all(
            pageIndices.map(async (pageIndex) => {
              try {
                return await convertToBlob([pageIndex], "image/png" as DynamsoftImageType);
              } catch {
                return await convertToBlob([pageIndex], "image/jpeg" as DynamsoftImageType);
              }
            }),
          );
          const { PDFDocument } = await import("pdf-lib");
          const pdfDoc = await PDFDocument.create();

          for (const pageBlob of pageBlobs) {
            const imageBytes = new Uint8Array(await pageBlob.arrayBuffer());
            let embeddedPage;
            try {
              embeddedPage = await pdfDoc.embedPng(imageBytes);
            } catch {
              embeddedPage = await pdfDoc.embedJpg(imageBytes);
            }
            const page = pdfDoc.addPage([embeddedPage.width, embeddedPage.height]);
            page.drawImage(embeddedPage, {
              x: 0,
              y: 0,
              width: embeddedPage.width,
              height: embeddedPage.height,
            });
          }

          const pdfBytes = await pdfDoc.save();
          const pdfBuffer = new ArrayBuffer(pdfBytes.byteLength);
          new Uint8Array(pdfBuffer).set(pdfBytes);
          blob = new Blob([pdfBuffer], { type: "application/pdf" });
          lastError = null;
        } catch (error) {
          lastError = error;
        }
      }

      if (!blob) {
        throw lastError ?? new DynamsoftScannerError({
          code: "PDF_GENERATION_FAILED",
          message: "No fue posible generar el PDF.",
        });
      }

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

  private buildPagesFromBuffer(
    dwt: DynamsoftWebTwainObject,
    count: number,
    previousPages: ScanPage[] = this.pages,
    options: {
      reusePreviousPages?: boolean;
    } = {},
  ) {
    if (options.reusePreviousPages) {
      const stablePages = previousPages.slice(0, Math.min(previousPages.length, count)).map(
        (page, index) => ({
          ...page,
          index,
        }),
      );

      if (count <= previousPages.length) {
        return stablePages;
      }

      return [
        ...stablePages,
        ...Array.from(
          { length: Math.max(count - previousPages.length, 0) },
          (_item, localIndex) =>
            this.buildPageFromBuffer(dwt, previousPages.length + localIndex),
        ),
      ];
    }

    return Array.from({ length: Math.max(count, 0) }, (_item, index) =>
      this.buildPageFromBuffer(dwt, index, previousPages[index]?.id),
    );
  }

  private buildPageFromBuffer(
    dwt: DynamsoftWebTwainObject,
    index: number,
    pageId?: string,
  ): ScanPage {
    const thumbnailUrl = normalizeImageUrl(dwt.GetImageURL?.(index, 160, 220));
    const imageUrl = normalizeImageUrl(dwt.GetImageURL?.(index, -1, -1));
    const fallbackImageUrl = normalizeImageUrl(dwt.GetImageURL?.(index));
    const finalThumbnailUrl = thumbnailUrl ?? fallbackImageUrl;
    const finalImageUrl = imageUrl ?? fallbackImageUrl;
    const width = readImageDimension(dwt.GetImageWidth?.bind(dwt), index);
    const height = readImageDimension(dwt.GetImageHeight?.bind(dwt), index);
    const orientation = getPageOrientation(width, height);
    const stablePageId = pageId ?? `scan-page-${index + 1}`;
    const originalDimensions = this.originalPageDimensionsById.get(stablePageId) ?? {
      width,
      height,
    };
    this.originalPageDimensionsById.set(stablePageId, originalDimensions);
    const rotationDegrees = this.pageRotationById.get(stablePageId) ?? 0;
    const page: ScanPage = {
      id: stablePageId,
      index,
      ...(finalThumbnailUrl ? { thumbnailUrl: finalThumbnailUrl } : {}),
      ...(finalImageUrl ? { imageUrl: finalImageUrl } : {}),
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
    onProgress?: ScanProgressListener,
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
      const featureStart = performance.now();
      this.reportScanProgress(onProgress, {
        stage: feature.progressStage,
        label: feature.progressLabel,
        detail: `${this.pages.length} paginas en procesamiento.`,
        currentPage: 0,
        totalPages: this.pages.length,
        progress:
          feature.key === "deskew"
            ? 55
            : feature.key === "autoCrop"
              ? 65
              : 75,
        cancellable: false,
      });
      result[feature.key] = await this.applyAutomaticProcessingFeature(dwt, feature);
      const featureDuration = performance.now() - featureStart;
      const sharedPerf = readScanPipelinePerfRecord();
      if (sharedPerf) {
        if (feature.key === "deskew") {
          sharedPerf.stages.deskewMs = featureDuration;
        } else if (feature.key === "autoCrop") {
          sharedPerf.stages.autoCropMs = featureDuration;
        } else if (feature.key === "autoRotate") {
          sharedPerf.stages.autoRotateMs = featureDuration;
        }
      }
      const stageLabel =
        feature.key === "deskew"
          ? "Deskew"
          : feature.key === "autoCrop"
            ? "AutoCrop"
            : "AutoRotate";
      console.info(`[SCAN PERF] ${stageLabel}`, {
        scanId: readScanPipelinePerfRecord()?.scanId,
        durationMs: Math.round(featureDuration),
      });
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

  private reportScanProgress(
    onProgress: ScanProgressListener | undefined,
    progress: Parameters<ScanProgressListener>[0],
  ) {
    onProgress?.(progress);
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

  private resolveCaptureOperationPages({
    operation,
    previousPages,
    scannedPages,
  }: {
    operation: ScanOptions["captureOperation"];
    previousPages: ScanPage[];
    scannedPages: ScanPage[];
  }) {
    const operationType = operation?.type ?? "NEW";
    if (operationType === "NEW" || previousPages.length === 0) {
      return scannedPages;
    }

    const previousPageIds = new Set(previousPages.map((page) => page.id));
    const capturedPages = scannedPages.filter((page) => !previousPageIds.has(page.id));
    if (capturedPages.length === 0) {
      return previousPages;
    }

    if (operationType === "APPEND") {
      return [...previousPages, ...capturedPages];
    }

    const targetPageId = operation?.targetPageId;
    const targetIndex = previousPages.findIndex((page) => page.id === targetPageId);
    if (targetIndex < 0) {
      return [...previousPages, ...capturedPages];
    }

    if (operationType === "REPLACE") {
      return [
        ...previousPages.slice(0, targetIndex),
        ...capturedPages,
        ...previousPages.slice(targetIndex + 1),
      ];
    }

    if (operationType === "INSERT_BEFORE") {
      return [
        ...previousPages.slice(0, targetIndex),
        ...capturedPages,
        ...previousPages.slice(targetIndex),
      ];
    }

    return [
      ...previousPages.slice(0, targetIndex + 1),
      ...capturedPages,
      ...previousPages.slice(targetIndex + 1),
    ];
  }

  private async removeDetectedBlankPages(
    dwt: DynamsoftWebTwainObject,
  ): Promise<BlankPageRemovalResult> {
    const blankAnalyses = await Promise.all(
      this.pages.map((page) => this.analyzeBlankPageCandidate(page)),
    );
    const blankPages = blankAnalyses.filter((analysis) => analysis.isBlank);
    const keptPages = blankAnalyses.filter((analysis) => !analysis.isBlank);

    keptPages.forEach((analysis) => {
      logBlankPageDiagnostic("BLANK_PAGE_KEPT", {
        pageId: analysis.page.id,
        index: analysis.page.index,
        pageNumber: analysis.page.index + 1,
        reason: analysis.reason,
        contentPercentage: Number((analysis.contentRatio * 100).toFixed(4)),
        darkPixels: analysis.darkPixels,
        clusteredDarkPixels: analysis.clusteredDarkPixels,
        imageSource: analysis.imageSource,
      });
      logBlankPageDiagnostic("BLANK_PAGE_SURVIVED", {
        stage: "analysis",
        pageId: analysis.page.id,
        pageIndex: analysis.page.index,
        pageNumber: analysis.page.index + 1,
        reason: analysis.reason,
        contentPercentage: Number((analysis.contentRatio * 100).toFixed(4)),
        darkPixels: analysis.darkPixels,
        clusteredDarkPixels: analysis.clusteredDarkPixels,
        imageSource: analysis.imageSource,
      });
    });

    if (blankPages.length === 0) {
      return {
        analyses: blankAnalyses,
        detected: [],
        removedPageIds: new Set(),
        requestedIndexes: [],
        removedIndexes: [],
        survivedIndexes: [],
      };
    }

    const blankPageIds = new Set(blankPages.map((analysis) => analysis.page.id));
    const blankIndexes = blankPages
      .map((analysis) => analysis.page.index)
      .sort((left, right) => right - left);
    const removedIndexes: number[] = [];
    const survivedIndexes: number[] = [];

    blankPages.forEach((analysis) => {
      logBlankPageDiagnostic("BLANK_PAGE_DETECTED", {
        pageId: analysis.page.id,
        pageIndex: analysis.page.index,
        pageNumber: analysis.page.index + 1,
        reason: analysis.reason,
        contentPercentage: Number((analysis.contentRatio * 100).toFixed(4)),
        darkPixels: analysis.darkPixels,
        clusteredDarkPixels: analysis.clusteredDarkPixels,
        whiteThreshold: BLANK_PAGE_WHITE_THRESHOLD,
        contentThreshold: BLANK_PAGE_CONTENT_RATIO_THRESHOLD,
        darkPixelThreshold: BLANK_PAGE_DARK_PIXEL_THRESHOLD,
        imageSource: analysis.imageSource,
      });
    });

    blankIndexes.forEach((index) => {
      const beforeCount = dwt.HowManyImagesInBuffer;
      const removeResult = dwt.RemoveImage(index);
      const afterCount = dwt.HowManyImagesInBuffer;
      const removedFromBuffer =
        beforeCount === undefined ||
        afterCount === undefined ||
        afterCount < beforeCount ||
        removeResult === true;

      if (removedFromBuffer) {
        removedIndexes.push(index);
        return;
      }

      survivedIndexes.push(index);
    });

    blankPages.forEach((analysis) => {
      const removedFromBuffer = removedIndexes.includes(analysis.page.index);
      const label = removedFromBuffer ? "BLANK_PAGE_REMOVED" : "BLANK_PAGE_SURVIVED";
      logBlankPageDiagnostic(label, {
        stage: "removeImage",
        pageId: analysis.page.id,
        pageIndex: analysis.page.index,
        pageNumber: analysis.page.index + 1,
        reason: analysis.reason,
        removedFromBuffer,
        contentPercentage: Number((analysis.contentRatio * 100).toFixed(4)),
        darkPixels: analysis.darkPixels,
        clusteredDarkPixels: analysis.clusteredDarkPixels,
        imageSource: analysis.imageSource,
      });
    });

    this.pages = this.rebuildPagesAfterBufferRemoval(dwt, this.pages, removedIndexes, blankPageIds);
    this.logBlankPageReinsertions("afterBlankRemoval", {
      analyses: blankAnalyses,
      detected: blankPages,
      removedPageIds: blankPageIds,
      requestedIndexes: blankIndexes,
      removedIndexes,
      survivedIndexes,
    });
    logBlankPageDiagnostic("BLANK_PAGE_FINAL_STATE", {
      stage: "afterBlankRemoval",
      pageCount: this.pages.length,
      bufferCount: dwt.HowManyImagesInBuffer ?? null,
      detectedPageIds: blankPages.map((analysis) => analysis.page.id),
      requestedIndexes: blankIndexes,
      removedIndexes,
      survivedIndexes,
      pages: this.summarizePagesForDiagnostics(this.pages),
    });

    return {
      analyses: blankAnalyses,
      detected: blankPages,
      removedPageIds: blankPageIds,
      requestedIndexes: blankIndexes,
      removedIndexes,
      survivedIndexes,
    };
  }

  private async removeDetectedBlankPagesWithDynamsoft(
    dwt: DynamsoftWebTwainObject,
    pageRange: { startIndex: number; endIndex: number },
  ): Promise<BlankPageRemovalResult | null> {
    const dwtObject = dwt as Record<string, unknown>;
    const isBlankImageAsync = dwtObject.IsBlankImageAsync;
    const isBlankImageExpress = dwtObject.IsBlankImageExpress;
    const useAsyncBlankDetection = typeof isBlankImageAsync === "function";

    if (!useAsyncBlankDetection && typeof isBlankImageExpress !== "function") {
      return null;
    }

    const { startIndex, endIndex } = pageRange;
    if (startIndex < 0 || endIndex <= startIndex) {
      return {
        analyses: [],
        detected: [],
        removedPageIds: new Set(),
        requestedIndexes: [],
        removedIndexes: [],
        survivedIndexes: [],
      };
    }

    const candidateIndexes = Array.from(
      { length: Math.max(endIndex - startIndex, 0) },
      (_item, offset) => startIndex + offset,
    );
    const checkedIndexes = candidateIndexes.filter(
      (index) => index >= 0 && index < this.pages.length,
    );
    const blankIndexes = (
      await Promise.all(
        checkedIndexes.map(async (index) => {
          try {
            if (useAsyncBlankDetection) {
              const isBlank = await (
                isBlankImageAsync as (
                  index: number,
                  options?: {
                    minBlockHeight?: number;
                    maxBlockHeight?: number;
                  },
                ) => Promise<boolean>
              ).call(dwt, index, {
                minBlockHeight: BLANK_PAGE_ASYNC_MIN_BLOCK_HEIGHT,
                maxBlockHeight: BLANK_PAGE_ASYNC_MAX_BLOCK_HEIGHT,
              });
              return isBlank ? index : null;
            }

            const isBlank = (isBlankImageExpress as (index: number) => boolean).call(dwt, index);
            return isBlank ? index : null;
          } catch {
            return null;
          }
        }),
      )
    )
      .filter((index): index is number => index !== null && index !== undefined);

    const blankPages = blankIndexes
      .map((index) => this.pages[index])
      .filter((page): page is ScanPage => Boolean(page));

    if (blankPages.length === 0) {
      return {
        analyses: [],
        detected: [],
        removedPageIds: new Set(),
        requestedIndexes: blankIndexes,
        removedIndexes: [],
        survivedIndexes: [],
      };
    }

    const blankPageAnalyses: BlankPageAnalysis[] = useAsyncBlankDetection
      ? blankPages.map((page) => ({
          page,
          isBlank: true,
          contentRatio: 0,
          darkPixels: 0,
          clusteredDarkPixels: 0,
          darkRatio: 0,
          reason: "isBlankImageAsync",
          imageSource: "unavailable",
        }))
      : blankPages.map((page) => {
          const imageSource: BlankPageAnalysis["imageSource"] = page.imageUrl
            ? "original"
            : page.thumbnailUrl
              ? "thumbnail"
              : "unavailable";
          return {
            page,
            isBlank: true,
            contentRatio: 0,
            darkPixels: 0,
            clusteredDarkPixels: 0,
            darkRatio: 0,
            reason: "isBlankImageExpress",
            imageSource,
          };
        });
    const confirmedBlankPages = blankPageAnalyses.filter((analysis) => analysis.isBlank);
    const confirmedBlankIndexes = confirmedBlankPages
      .map((analysis) => analysis.page.index)
      .sort((left, right) => right - left);

    const blankPageIds = new Set(confirmedBlankPages.map((analysis) => analysis.page.id));
    const removedIndexes: number[] = [];

    blankPageAnalyses.forEach((analysis) => {
      const label = analysis.isBlank ? "BLANK_PAGE_DETECTED" : "BLANK_PAGE_SURVIVED";
      logBlankPageDiagnostic(label, {
        stage: "removeDetectedBlankPagesWithDynamsoft",
        pageId: analysis.page.id,
        pageIndex: analysis.page.index,
        pageNumber: analysis.page.index + 1,
        reason: analysis.reason,
        contentPercentage: Number((analysis.contentRatio * 100).toFixed(4)),
        darkPixels: analysis.darkPixels,
        clusteredDarkPixels: analysis.clusteredDarkPixels,
        whiteThreshold: BLANK_PAGE_WHITE_THRESHOLD,
        contentThreshold: BLANK_PAGE_CONTENT_RATIO_THRESHOLD,
        darkPixelThreshold: BLANK_PAGE_DARK_PIXEL_THRESHOLD,
        imageSource: analysis.imageSource,
      });
    });

    confirmedBlankIndexes.forEach((index) => {
      const beforeCount = dwt.HowManyImagesInBuffer;
      const removeResult = dwt.RemoveImage(index);
      const afterCount = dwt.HowManyImagesInBuffer;
      const removedFromBuffer =
        beforeCount === undefined ||
        afterCount === undefined ||
        afterCount < beforeCount ||
        removeResult === true;

      if (removedFromBuffer) {
        removedIndexes.push(index);
      }
    });

    const removedIndexSet = new Set(removedIndexes);
    const survivedIndexes = blankIndexes.filter((index) => !removedIndexSet.has(index));
    const confirmedBlankPagesForResult = confirmedBlankPages.map((analysis) => analysis.page);

    confirmedBlankPagesForResult.forEach((page) => {
      const removedFromBuffer = removedIndexes.includes(page.index);
      const analysis = blankPageAnalyses.find((candidate) => candidate.page.id === page.id);
      const label = removedFromBuffer ? "BLANK_PAGE_REMOVED" : "BLANK_PAGE_SURVIVED";
      logBlankPageDiagnostic(label, {
        stage: "removeImage",
        pageId: page.id,
        pageIndex: page.index,
        pageNumber: page.index + 1,
        reason: analysis?.reason ?? "analysis-failed",
        removedFromBuffer,
        contentPercentage: analysis ? Number((analysis.contentRatio * 100).toFixed(4)) : undefined,
        darkPixels: analysis?.darkPixels,
        clusteredDarkPixels: analysis?.clusteredDarkPixels,
        imageSource: analysis?.imageSource ?? "unavailable",
      });
    });

    this.pages = this.rebuildPagesAfterBufferRemoval(dwt, this.pages, removedIndexes, blankPageIds);
    this.logBlankPageReinsertions("afterBlankRemoval", {
      analyses: blankPageAnalyses,
      detected: confirmedBlankPages,
      removedPageIds: blankPageIds,
      requestedIndexes: blankIndexes,
      removedIndexes,
      survivedIndexes,
    });
    logBlankPageDiagnostic("BLANK_PAGE_FINAL_STATE", {
      stage: "afterBlankRemoval",
      pageCount: this.pages.length,
      bufferCount: dwt.HowManyImagesInBuffer ?? null,
      detectedPageIds: confirmedBlankPagesForResult.map((page) => page.id),
      requestedIndexes: blankIndexes,
      removedIndexes,
      survivedIndexes,
      pages: this.summarizePagesForDiagnostics(this.pages),
    });

    return {
      analyses: blankPageAnalyses,
      detected: confirmedBlankPages,
      removedPageIds: blankPageIds,
      requestedIndexes: blankIndexes,
      removedIndexes,
      survivedIndexes,
    };
  }

  private async analyzeBlankPageCandidate(page: ScanPage): Promise<BlankPageAnalysis> {
    const imageUrl = page.imageUrl ?? page.thumbnailUrl;
    const imageSource = page.imageUrl ? "original" : page.thumbnailUrl ? "thumbnail" : "unavailable";
    if (!imageUrl) {
      return {
        page,
        isBlank: false,
        contentRatio: 1,
        darkPixels: Number.POSITIVE_INFINITY,
        clusteredDarkPixels: Number.POSITIVE_INFINITY,
        darkRatio: 1,
        reason: "image-url-unavailable",
        imageSource,
      };
    }

    try {
      const loadCandidates = [
        {
          src: page.imageUrl,
          source: "original" as const,
        },
        {
          src: page.thumbnailUrl,
          source: "thumbnail" as const,
        },
      ].filter((candidate): candidate is { src: string; source: "original" | "thumbnail" } =>
        Boolean(candidate.src),
      );

      let image: HTMLImageElement | null = null;
      let analyzedImageSource: BlankPageAnalysis["imageSource"] = imageSource;
      for (const candidate of loadCandidates) {
        try {
          image = await this.loadAnalysisImage(candidate.src);
          analyzedImageSource = candidate.source;
          break;
        } catch {
          continue;
        }
      }

      if (!image) {
        return {
          page,
          isBlank: false,
          contentRatio: 1,
          darkPixels: Number.POSITIVE_INFINITY,
          clusteredDarkPixels: Number.POSITIVE_INFINITY,
          darkRatio: 1,
          reason: "analysis-failed",
          imageSource,
        };
      }

      logBlankPageDiagnostic("BLANK_PAGE_ANALYSIS_START", {
        pageId: page.id,
        index: page.index,
        pageNumber: page.index + 1,
        imageSource: analyzedImageSource,
        analysisWidth: BLANK_PAGE_ANALYSIS_WIDTH,
        analysisHeight: BLANK_PAGE_ANALYSIS_HEIGHT,
        whiteThreshold: BLANK_PAGE_WHITE_THRESHOLD,
        contentThreshold: BLANK_PAGE_CONTENT_RATIO_THRESHOLD,
        darkPixelThreshold: BLANK_PAGE_DARK_PIXEL_THRESHOLD,
      });
      const canvas = this.options.documentRef.createElement("canvas");
      canvas.width = BLANK_PAGE_ANALYSIS_WIDTH;
      canvas.height = BLANK_PAGE_ANALYSIS_HEIGHT;
      const context = canvas.getContext("2d", { willReadFrequently: true });
      if (!context) {
        return {
          page,
          isBlank: false,
          contentRatio: 1,
          darkPixels: Number.POSITIVE_INFINITY,
          clusteredDarkPixels: Number.POSITIVE_INFINITY,
          darkRatio: 1,
          reason: "canvas-context-unavailable",
          imageSource: analyzedImageSource,
        };
      }

      context.fillStyle = "#ffffff";
      context.fillRect(0, 0, canvas.width, canvas.height);
      context.drawImage(image, 0, 0, canvas.width, canvas.height);
      const pixels = context.getImageData(0, 0, canvas.width, canvas.height).data;
      let contentPixels = 0;
      let interiorContentPixels = 0;
      let darkPixels = 0;
      const totalPixels = pixels.length / 4;
      const luminanceHistogram = new Int32Array(256);
      const opaquePixels = new Uint8Array(totalPixels);
      const totalHorizontalTransitions = Math.max(1, BLANK_PAGE_ANALYSIS_WIDTH * (BLANK_PAGE_ANALYSIS_HEIGHT - 1));
      const totalVerticalTransitions = Math.max(1, (BLANK_PAGE_ANALYSIS_WIDTH - 1) * BLANK_PAGE_ANALYSIS_HEIGHT);
      let luminanceSum = 0;
      let luminanceSqSum = 0;
      let edgeTransitions = 0;
      const interiorXMargin = Math.floor(BLANK_PAGE_ANALYSIS_WIDTH * BLANK_PAGE_BORDER_FRACTION_TO_IGNORE);
      const interiorYMargin = Math.floor(BLANK_PAGE_ANALYSIS_HEIGHT * BLANK_PAGE_BORDER_FRACTION_TO_IGNORE);
      const interiorWidth = Math.max(1, BLANK_PAGE_ANALYSIS_WIDTH - interiorXMargin * 2);
      const interiorHeight = Math.max(1, BLANK_PAGE_ANALYSIS_HEIGHT - interiorYMargin * 2);
      const interiorTotalPixels = interiorWidth * interiorHeight;
      const darkMask = Array.from({ length: totalPixels }, () => false);
      const luminanceValues = new Float32Array(totalPixels);

      for (let offset = 0; offset < pixels.length; offset += 4) {
        const pixelIndex = offset / 4;
        const red = pixels[offset] ?? 255;
        const green = pixels[offset + 1] ?? 255;
        const blue = pixels[offset + 2] ?? 255;
        const alpha = pixels[offset + 3] ?? 255;
        if (alpha < 12) {
          continue;
        }

        opaquePixels[pixelIndex] = 1;

        const luminance = 0.2126 * red + 0.7152 * green + 0.0722 * blue;
        if (luminance < 180) {
          darkPixels += 1;
          darkMask[offset / 4] = true;
        }
        luminanceSum += luminance;
        luminanceSqSum += luminance * luminance;
        luminanceValues[pixelIndex] = luminance;
        const roundedLuminance = Math.max(0, Math.min(255, Math.round(luminance)));
        luminanceHistogram[roundedLuminance] += 1;
      }

      const adaptiveWhiteThreshold = Math.max(
        BLANK_PAGE_DYNAMIC_WHITE_THRESHOLD_FLOOR,
        Math.min(
          BLANK_PAGE_WHITE_THRESHOLD,
          getLuminancePercentile(
            luminanceHistogram,
            totalPixels,
            BLANK_PAGE_WHITE_PERCENTILE,
          ),
        ),
      );
      for (let pixelIndex = 0; pixelIndex < totalPixels; pixelIndex += 1) {
        if (!opaquePixels[pixelIndex]) {
          continue;
        }

        const pixelX = pixelIndex % BLANK_PAGE_ANALYSIS_WIDTH;
        const pixelY = Math.floor(pixelIndex / BLANK_PAGE_ANALYSIS_WIDTH);
        const red = pixels[pixelIndex * 4] ?? 255;
        const green = pixels[pixelIndex * 4 + 1] ?? 255;
        const blue = pixels[pixelIndex * 4 + 2] ?? 255;

        if (red < adaptiveWhiteThreshold || green < adaptiveWhiteThreshold || blue < adaptiveWhiteThreshold) {
          contentPixels += 1;
          if (
            pixelX >= interiorXMargin &&
            pixelX < BLANK_PAGE_ANALYSIS_WIDTH - interiorXMargin &&
            pixelY >= interiorYMargin &&
            pixelY < BLANK_PAGE_ANALYSIS_HEIGHT - interiorYMargin
          ) {
            interiorContentPixels += 1;
          }
        }
      }
      contentPixels = Math.min(contentPixels, totalPixels);

      for (let y = 0; y < BLANK_PAGE_ANALYSIS_HEIGHT; y += 1) {
        const rowOffset = y * BLANK_PAGE_ANALYSIS_WIDTH;
        for (let x = 0; x < BLANK_PAGE_ANALYSIS_WIDTH - 1; x += 1) {
          const left = luminanceValues[rowOffset + x] ?? 0;
          const right = luminanceValues[rowOffset + x + 1] ?? 0;
          if (Math.abs(left - right) > BLANK_PAGE_EDGE_DEVIATION_THRESHOLD) {
            edgeTransitions += 1;
          }
        }
      }

      for (let y = 0; y < BLANK_PAGE_ANALYSIS_HEIGHT - 1; y += 1) {
        const rowOffset = y * BLANK_PAGE_ANALYSIS_WIDTH;
        const nextRowOffset = (y + 1) * BLANK_PAGE_ANALYSIS_WIDTH;
        for (let x = 0; x < BLANK_PAGE_ANALYSIS_WIDTH; x += 1) {
          const top = luminanceValues[rowOffset + x] ?? 0;
          const bottom = luminanceValues[nextRowOffset + x] ?? 0;
          if (Math.abs(top - bottom) > BLANK_PAGE_EDGE_DEVIATION_THRESHOLD) {
            edgeTransitions += 1;
          }
        }
      }

      const contentRatio = contentPixels / totalPixels;
      const interiorContentRatio = interiorContentPixels / interiorTotalPixels;
      const darkRatio = darkPixels / totalPixels;
      const clusteredDarkPixels = countClusteredDarkPixels(darkMask, canvas.width, canvas.height);
      const edgeTransitionRatio =
        edgeTransitions / (totalHorizontalTransitions + totalVerticalTransitions);
      const averageLuminance = luminanceSum / totalPixels;
      const luminanceVariance = luminanceSqSum / totalPixels - averageLuminance * averageLuminance;
      const isLowContrastWhitePage =
        averageLuminance >= BLANK_PAGE_LOW_CONTRAST_LUMINANCE_THRESHOLD &&
        luminanceVariance <= BLANK_PAGE_LOW_CONTRAST_VARIANCE_THRESHOLD &&
        interiorContentRatio <=
          BLANK_PAGE_CONTENT_RATIO_THRESHOLD * BLANK_PAGE_LOW_CONTRAST_CONTENT_RATIO_MULTIPLIER;
      const contentDetected =
        contentRatio > BLANK_PAGE_CONTENT_RATIO_THRESHOLD &&
        interiorContentRatio > BLANK_PAGE_CONTENT_RATIO_THRESHOLD;
      const darkContentDetected =
        darkRatio > BLANK_PAGE_DARK_RATIO_THRESHOLD &&
        clusteredDarkPixels > BLANK_PAGE_DARK_PIXEL_THRESHOLD;
      const contentDetectedByEdges = edgeTransitionRatio > BLANK_PAGE_EDGE_RATIO_THRESHOLD;
      const isBlank =
        (!contentDetected && !darkContentDetected && !contentDetectedByEdges) ||
        isLowContrastWhitePage;
      const contentPercentage = Number((contentRatio * 100).toFixed(4));
      const reason = isBlank
        ? "below-content-threshold"
        : darkContentDetected
          ? "clustered-dark-pixels-detected"
          : "content-detected";

      logBlankPageDiagnostic("BLANK_PAGE_CONTENT_PERCENTAGE", {
        pageId: page.id,
        index: page.index,
        pageNumber: page.index + 1,
        contentPercentage,
        contentPixels,
        totalPixels,
      });
      logBlankPageDiagnostic("BLANK_PAGE_DARK_PIXELS", {
        pageId: page.id,
        index: page.index,
        pageNumber: page.index + 1,
        darkPixels,
        clusteredDarkPixels,
        darkRatio: Number(darkRatio.toFixed(6)),
        darkPixelThreshold: BLANK_PAGE_DARK_PIXEL_THRESHOLD,
        adaptiveWhiteThreshold,
      });
      logBlankPageDiagnostic("BLANK_PAGE_ANALYSIS_RESULT", {
        pageId: page.id,
        index: page.index,
        pageNumber: page.index + 1,
        isBlank,
        reason,
        contentPercentage,
        darkPixels,
        clusteredDarkPixels,
        darkRatio: Number(darkRatio.toFixed(6)),
        imageSource: analyzedImageSource,
      });

      return {
        page,
        isBlank,
        contentRatio,
        darkPixels,
        clusteredDarkPixels,
        darkRatio,
        reason,
        imageSource: analyzedImageSource,
      };
    } catch (error) {
      console.warn("BLANK_PAGE_ANALYSIS_ERROR", {
        pageId: page.id,
        index: page.index,
        pageNumber: page.index + 1,
        message: error instanceof Error ? error.message : String(error),
      });
      return {
        page,
        isBlank: false,
        contentRatio: 1,
        darkPixels: Number.POSITIVE_INFINITY,
        clusteredDarkPixels: Number.POSITIVE_INFINITY,
        darkRatio: 1,
        reason: "analysis-failed",
        imageSource,
      };
    }
  }

  private logBlankPageReinsertions(
    stage: string,
    removalResult: BlankPageRemovalResult | null,
  ) {
    if (!removalResult || removalResult.detected.length === 0) {
      return;
    }

    const currentPageIds = new Set(this.pages.map((page) => page.id));
    removalResult.detected.forEach((analysis) => {
      if (!analysis || !analysis.page) {
        logBlankPageDiagnostic("BLANK_PAGE_REINSERTION_SKIP", {
          stage,
          reason: "missing-analysis-page",
          requestedIndexes: removalResult.requestedIndexes,
          removedIndexes: removalResult.removedIndexes,
          survivedIndexes: removalResult.survivedIndexes,
        });
        return;
      }

      if (!currentPageIds.has(analysis.page.id)) {
        return;
      }

      logBlankPageDiagnostic("BLANK_PAGE_REINSERTED", {
        stage,
        pageId: analysis.page.id,
        pageIndex: analysis.page.index,
        pageNumber: analysis.page.index + 1,
        requestedIndexes: removalResult.requestedIndexes,
        removedIndexes: removalResult.removedIndexes,
        survivedIndexes: removalResult.survivedIndexes,
      });
    });
  }

  private summarizePagesForDiagnostics(pages: ScanPage[]) {
    return pages.map((page) => ({
      pageId: page.id,
      pageIndex: page.index,
      pageNumber: page.index + 1,
      thumbnailUrl: page.thumbnailUrl,
      imageUrl: page.imageUrl,
    }));
  }

  private loadAnalysisImage(src: string) {
    return new Promise<HTMLImageElement>((resolve, reject) => {
      const ImageConstructor = (this.options.windowRef as DynamsoftWindow).Image ?? Image;
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
    if (this.pages.length > 0) {
      return this.pages.map((page) => page.index);
    }

    const bufferPageCount = dwt.HowManyImagesInBuffer ?? this.pages.length;
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
    const page = this.pages.find((currentPage) => currentPage.id === pageId);
    if (!page) {
      throw new DynamsoftScannerError({
        code: "PDF_EMPTY",
        message: "Pagina no encontrada.",
      });
    }

    return page.index;
  }
}
