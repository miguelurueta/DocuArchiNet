import {
  DYNAMSOFT_ALLOWED_COLOR_MODES,
  DYNAMSOFT_CONTAINER_ID,
  DYNAMSOFT_DEFAULT_RESOURCES_PATH,
  DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
  DYNAMSOFT_MAX_RESOLUTION_DPI,
  DYNAMSOFT_MIN_RESOLUTION_DPI,
} from "./dynamsoft.constants";
import { DynamsoftScannerError } from "./dynamsoft.errors";
import { debugDynamsoftLicense } from "./dynamsoftLicenseDebug";
import { loadDynamsoftScripts } from "./loadDynamsoftScripts";
import type {
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
let dynamsoftClientSequence = 0;

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

const debugScannerSelection = (
  stage: string,
  scanner: { scannerName?: string; scannerIndex?: number },
) => {
  console.debug("[DynamsoftScanner]", stage, {
    scannerName: scanner.scannerName ?? "",
    scannerIndex: scanner.scannerIndex ?? -1,
  });
};

const readDiagnosticValue = (target: unknown, keys: string[]) => {
  if (!target || typeof target !== "object") {
    return undefined;
  }

  const record = target as Record<string, unknown>;
  return keys.map((key) => record[key]).find((value) => value !== undefined);
};

const callDiagnosticMethod = (target: unknown, keys: string[], args: unknown[] = []) => {
  if (!target || typeof target !== "object") {
    return undefined;
  }

  const record = target as Record<string, unknown>;
  const method = keys.map((key) => record[key]).find((value) => typeof value === "function");
  if (typeof method !== "function") {
    return undefined;
  }

  try {
    return method.apply(target, args);
  } catch (error) {
    return error instanceof Error ? error.message : String(error);
  }
};

const getDwtLifecycleSnapshot = (dwt: DynamsoftWebTwainObject | null) => ({
  destroy: readDiagnosticValue(dwt, ["_destroy"]),
  ready: readDiagnosticValue(dwt, ["_bReady"]),
  sourceCount: dwt?.SourceCount,
  imageCount: dwt?.HowManyImagesInBuffer,
});

export class DynamsoftTwainClient implements DigitalizacionScannerClient {
  private readonly instanceId = `DynamsoftTwainClient-${++dynamsoftClientSequence}`;

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
  private destroyWatchdogId: number | null = null;
  private lastDestroySnapshot: unknown = undefined;

  constructor(options: DynamsoftRuntimeOptions = {}) {
    this.options = {
      scriptSrc: options.scriptSrc ?? "",
      resourcesPath: options.resourcesPath ?? DYNAMSOFT_DEFAULT_RESOURCES_PATH,
      licenseKey: options.licenseKey,
      containerId: options.containerId ?? DYNAMSOFT_CONTAINER_ID,
      documentRef: options.documentRef ?? document,
      windowRef: options.windowRef ?? window,
    };
    debugDynamsoftLicense(
      "DynamsoftTwainClient.constructor.options.licenseKey",
      this.options.licenseKey,
    );
    console.log("DYNAMSOFT_CLIENT_CREATED", {
      instanceId: this.instanceId,
      containerId: this.options.containerId,
      resourcesPath: this.options.resourcesPath,
    });
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

    const containerExists = Boolean(
      this.options.documentRef.getElementById(this.options.containerId),
    );
    console.log("DYNAMSOFT_CONTAINER_STATUS", {
      containerId: this.options.containerId,
      exists: containerExists,
    });
    console.log("DYNAMSOFT_RUNTIME_DIAGNOSTICS_BEFORE_LOAD", {
      version: readDiagnosticValue(runtime, ["version", "Version", "ProductVersion"]),
      serviceVersion: readDiagnosticValue(runtime, [
        "serviceVersion",
        "ServiceVersion",
        "ServiceInstallerVersion",
      ]),
      twainModuleVersion: readDiagnosticValue(runtime, [
        "twainModuleVersion",
        "TwainModuleVersion",
      ]),
    });

    runtime.ProductKey = this.options.licenseKey;
    debugDynamsoftLicense(
      "DynamsoftTwainClient.runtime.ProductKey before runtime.Load",
      runtime.ProductKey,
    );
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
      console.log("DWT_BEFORE_LOAD", this.dwt);
      await Promise.resolve(runtime.Load());
      console.log("DWT_AFTER_LOAD", this.dwt);
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
    console.log("GET_WEBTWAIN_RESULT", dwt);
    console.log("GET_WEBTWAIN_DESTROY", readDiagnosticValue(dwt, ["_destroy"]));
    console.log("GET_WEBTWAIN_READY", readDiagnosticValue(dwt, ["_bReady"]));
    console.log("DWT_OBJECT_EXIST", callDiagnosticMethod(runtime, ["ObjectExist"], [WEB_TWAIN_ID]));
    if (!dwt) {
      throw new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "No fue posible inicializar Dynamsoft Web TWAIN.",
      });
    }

    this.dwt = dwt;
    console.log("INITIALIZE_DWT_INSTANCE", dwt);
    this.startDestroyWatchdog();
    console.log("DYNAMSOFT_DWT_DIAGNOSTICS_AFTER_INITIALIZE", {
      instanceId: this.instanceId,
      version: readDiagnosticValue(dwt, ["version", "Version", "ProductVersion"]),
      serviceVersion:
        readDiagnosticValue(dwt, ["serviceVersion", "ServiceVersion"]) ??
        callDiagnosticMethod(dwt, ["GetServiceVersion", "getServiceVersion"]),
      twainModuleVersion: readDiagnosticValue(dwt, [
        "twainModuleVersion",
        "TwainModuleVersion",
      ]),
      destroy: readDiagnosticValue(dwt, ["_destroy"]),
      ready: readDiagnosticValue(dwt, ["_bReady"]),
      sourceCount: dwt.SourceCount,
    });
  }

  async listDevices() {
    const dwt = this.requireDwt();
    const sourceManagerDevices = this.listDevicesFromSourceManager(dwt);
    if (sourceManagerDevices.length > 0) {
      return sourceManagerDevices;
    }

    if (dwt.GetDevicesAsync) {
      try {
        console.log("GET_DEVICES_ASYNC_START", {
          deviceType: undefined,
          refresh: true,
        });
        this.logDwtLifecycle("BEFORE_GetDevicesAsync", dwt);
        const sourceDevices = await withDynamsoftTimeout(
          dwt.GetDevicesAsync(undefined, true),
          "GetDevicesAsync",
        );
        console.log("GET_DEVICES_ASYNC_RAW_RESULT", sourceDevices);
        if (Array.isArray(sourceDevices)) {
          sourceDevices.forEach((device, index) => {
            console.log("GET_DEVICES_ASYNC_DEVICE", {
              index,
              device,
              keys: Object.keys(device ?? {}),
              constructor: device?.constructor?.name,
              prototype: Object.getPrototypeOf(device ?? {}),
            });
          });
        }
        this.logDwtLifecycle("AFTER_GetDevicesAsync", dwt);
        console.log("GET_DEVICES_ASYNC_SUCCESS", {
          count: sourceDevices.length,
        });
        if (sourceDevices.length > 0) {
          const devices = sourceDevices.map((device, index) => {
            const name = device.displayName || device.name;
            debugScannerSelection("listDevices.discovered", {
              scannerName: name,
              scannerIndex: index,
            });
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
          devices.forEach((device) => {
            console.log("DWT_DEVICE_CACHE_SET", {
              deviceId: device.id,
              cachedObject: this.dwtDevices.get(device.id),
            });
          });
          return devices;
        }
      } catch (error) {
        console.warn("GET_DEVICES_ASYNC_FALLBACK", error);
      }
    }

    return sourceManagerDevices;
  }

  private listDevicesFromSourceManager(dwt: DynamsoftWebTwainObject) {
    console.log("SOURCE_MANAGER_DISCOVERY_START");
    this.logDwtLifecycle("BEFORE_OpenSourceManager", dwt);
    const opened = this.openSourceManager(dwt);
    this.logDwtLifecycle("AFTER_OpenSourceManager", dwt);
    const count = dwt.SourceCount ?? 0;
    console.log("SOURCE_MANAGER_RESULT", {
      opened,
      sourceCount: count,
    });
    const devices: ScannerDevice[] = [];

    for (let index = 0; index < count; index += 1) {
      this.logDwtLifecycle("BEFORE_GetSourceNameItems", dwt);
      const name = dwt.GetSourceNameItems(index);
      this.logDwtLifecycle("AFTER_GetSourceNameItems", dwt);
      debugScannerSelection("listDevices.discovered", {
        scannerName: name,
        scannerIndex: index,
      });
      devices.push({
        id: String(index),
        name,
        index,
      });
    }

    console.log(
      "TWAIN_SOURCES_AVAILABLE",
      devices.map((device) => ({
        scannerName: device.name,
        scannerIndex: device.index,
      })),
    );
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
    devices.forEach((device) => {
      console.log("DWT_DEVICE_CACHE_SET", {
        deviceId: device.id,
        cachedObject: this.dwtDevices.get(device.id),
      });
    });
    return devices;
  }

  async selectDevice(deviceId: string) {
    console.log("CLIENT_SELECT_DEVICE", deviceId);
    assertValidDeviceId(deviceId);
    const dwt = this.requireDwt();
    const cachedDevice = this.devices.find((device) => device.id === deviceId);
    const dwtDevice = this.dwtDevices.get(deviceId);
    const deviceIndex = cachedDevice?.index ?? Number(deviceId);
    const sourceCount = dwt.SourceCount ?? 0;
    console.log("SCANNER_CACHE", this.devices);
    console.log("SOURCE_COUNT", dwt.SourceCount);
    console.log("DEVICE_OBJECT", dwtDevice ?? null);
    console.log("DWT_DEVICE_CACHE_GET", {
      deviceId,
      dwtDevice,
    });
    debugScannerSelection("selectDevice.request", {
      scannerName: cachedDevice?.name,
      scannerIndex: deviceIndex,
    });

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
      console.log("SELECTION_MODE", {
        mode: "modern",
        deviceId,
        deviceIndex,
      });
      console.log("USING_MODERN_SELECTION");
      console.log("SOURCE_NAME", cachedDevice?.name ?? dwtDevice.displayName ?? dwtDevice.name);
      try {
        console.log("SELECT_DEVICE_START");
        console.log("CURRENT_DWT_INSTANCE", dwt);
        console.log("SELECT_DWT_INSTANCE", dwt);
        console.log("SELECT_DEVICE_INSTANCE_CHECK", {
          dwtExists: Boolean(dwt),
          sameReference: dwt === this.dwt,
          instanceId: this.instanceId,
        });
        console.log("SELECT_DEVICE_ASYNC_EXISTS", typeof dwt.SelectDeviceAsync);
        console.log("DWT_DEVICE_OBJECT", dwtDevice);
        console.log("DWT_DEVICE_JSON", JSON.stringify(dwtDevice, null, 2));
        console.log("DWT_DEVICE_KEYS", Object.keys(dwtDevice ?? {}));
        console.log("DWT_DEVICE_PROTOTYPE", Object.getPrototypeOf(dwtDevice ?? {}));
        console.log("DWT_DEVICE_CONSTRUCTOR", dwtDevice?.constructor?.name);
        this.logDwtLifecycle("BEFORE_SelectDeviceAsync", dwt);
        const selectDeviceResult = await withDynamsoftTimeout(
          dwt.SelectDeviceAsync(dwtDevice),
          "SelectDeviceAsync",
        );
        this.logDwtLifecycle("AFTER_SelectDeviceAsync", dwt);
        console.log("SELECT_DEVICE_SUCCESS");
        console.log("SELECT_SOURCE_RESULT", selectDeviceResult);
        if (!selectDeviceResult) {
          throw new DynamsoftScannerError({
            code: "SCANNER_NOT_FOUND",
            message: "No fue posible seleccionar el scanner.",
          });
        }
      } catch (error) {
        console.error("SELECT_SOURCE_ERROR", error);
        throw error;
      } finally {
        console.log("SELECT_DEVICE_FINALLY");
      }

      this.selectedDeviceId = deviceId;
      console.log("SELECTED_DEVICE_ID", this.selectedDeviceId);
      console.log("SELECT_DEVICE_END");
      return;
    }

    console.log("SELECTION_MODE", {
      mode: "legacy",
      deviceId,
      deviceIndex,
    });
    console.log("USING_LEGACY_SELECTION");
    this.logDwtLifecycle("BEFORE_legacy_OpenSourceManager", dwt);
    this.openSourceManager(dwt);
    this.logDwtLifecycle("AFTER_legacy_OpenSourceManager", dwt);
    this.logDwtLifecycle("BEFORE_legacy_GetSourceNameItems", dwt);
    const sourceName = dwt.GetSourceNameItems(deviceIndex);
    this.logDwtLifecycle("AFTER_legacy_GetSourceNameItems", dwt);
    console.log("SOURCE_NAME", sourceName);
    debugScannerSelection("selectDevice.SelectSourceByIndex", {
      scannerName: cachedDevice?.name ?? sourceName,
      scannerIndex: deviceIndex,
    });
    let selectSourceResult = false;
    try {
      console.log("SELECT_SOURCE_INDEX", deviceIndex);
      this.logDwtLifecycle("BEFORE_SelectSourceByIndex", dwt);
      selectSourceResult = dwt.SelectSourceByIndex(deviceIndex);
      this.logDwtLifecycle("AFTER_SelectSourceByIndex", dwt);
      console.log("SELECT_SOURCE_RESULT", selectSourceResult);
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
    console.log("SELECTED_DEVICE_ID", this.selectedDeviceId);
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
        this.logDwtLifecycle("BEFORE_OpenSource", dwt);
        dwt.OpenSource();
        this.logDwtLifecycle("AFTER_OpenSource", dwt);
        this.logDwtLifecycle("BEFORE_AcquireImage", dwt);
        dwt.AcquireImage(
          {
            IfShowUI: false,
            PixelType: colorModeToPixelType[options.colorMode ?? "color"],
            Resolution: options.resolutionDpi ?? DYNAMSOFT_DEFAULT_RESOLUTION_DPI,
            IfFeederEnabled: true,
            IfDuplexEnabled: options.duplex ?? false,
            IfDisableSourceAfterAcquire: true,
          },
          () => {
            this.logDwtLifecycle("AFTER_AcquireImage_SUCCESS", dwt);
            resolve();
          },
          (_code, message) => {
            this.logDwtLifecycle("AFTER_AcquireImage_FAILURE", dwt);
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
      this.pages = Array.from({ length: Math.max(nextCount, 0) }, (_item, index) => ({
        id: `scan-page-${index + 1}`,
        index,
      }));

      return [...this.pages];
    } finally {
      this.logDwtLifecycle("BEFORE_CloseSource", dwt);
      dwt.CloseSource?.();
      this.logDwtLifecycle("AFTER_CloseSource", dwt);
      this.activeOperation = null;
    }
  }

  async rotatePage(pageId: string, degrees: 90 | 180 | 270) {
    const dwt = this.requireDwt();
    const pageIndex = this.getPageIndex(pageId);
    this.logDwtLifecycle("BEFORE_Rotate", dwt);
    dwt.Rotate(pageIndex, degrees, true);
    this.logDwtLifecycle("AFTER_Rotate", dwt);
  }

  async removePage(pageId: string) {
    const dwt = this.requireDwt();
    const pageIndex = this.getPageIndex(pageId);
    this.logDwtLifecycle("BEFORE_RemoveImage", dwt);
    dwt.RemoveImage(pageIndex);
    this.logDwtLifecycle("AFTER_RemoveImage", dwt);
    this.pages = this.pages
      .filter((page) => page.id !== pageId)
      .map((page, index) => ({ ...page, index }));
  }

  async clear() {
    const dwt = this.dwt;
    this.logDwtLifecycle("BEFORE_RemoveAllImages", dwt);
    dwt?.RemoveAllImages();
    this.logDwtLifecycle("AFTER_RemoveAllImages", dwt);
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
        this.logDwtLifecycle("BEFORE_ConvertToBlob", dwt);
        dwt.ConvertToBlob(
          Array.from({ length: pageCount }, (_item, index) => index),
          "application/pdf",
          (nextBlob) => {
            this.logDwtLifecycle("AFTER_ConvertToBlob_SUCCESS", dwt);
            resolve(nextBlob);
          },
          (_code, message) => {
            this.logDwtLifecycle("AFTER_ConvertToBlob_FAILURE", dwt);
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
    console.log("DYNAMSOFT_CLIENT_DISPOSE_START", {
      instanceId: this.instanceId,
      dwtExists: Boolean(this.dwt),
      disposed: this.disposed,
      stack: new Error("DYNAMSOFT_CLIENT_DISPOSE_STACK").stack,
    });
    this.generation += 1;
    this.disposed = true;
    this.activeOperation = null;
    this.selectedDeviceId = null;
    this.pages = [];
    this.devices = [];
    this.dwtDevices.clear();
    this.modernDeviceIds.clear();
    console.log("DISPOSE_DWT_INSTANCE", this.dwt);
    this.stopDestroyWatchdog();
    this.logDwtLifecycle("BEFORE_dispose_CloseSource", this.dwt);
    this.dwt?.CloseSource?.();
    this.logDwtLifecycle("AFTER_dispose_CloseSource", this.dwt);
    this.dwt = null;
    console.log("BEFORE_DWT_UNLOAD", {
      instanceId: this.instanceId,
    });
    (this.options.windowRef as DynamsoftWindow).Dynamsoft?.DWT?.Unload?.();
    console.log("AFTER_DWT_UNLOAD", {
      instanceId: this.instanceId,
    });
    console.log("DYNAMSOFT_CLIENT_DISPOSE_END", {
      instanceId: this.instanceId,
    });
  }

  private requireDwt() {
    console.log("REQUIRE_DWT", this.dwt);
    this.logDwtLifecycle("REQUIRE_DWT_STATE", this.dwt);
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
      this.logDwtLifecycle("BEFORE_OpenSourceManager_call", dwt);
      const opened = dwt.OpenSourceManager();
      this.logDwtLifecycle("AFTER_OpenSourceManager_call", dwt);
      console.log("OPEN_SOURCE_MANAGER_RESULT", opened);
      return opened;
    } catch (error) {
      console.error("OPEN_SOURCE_MANAGER_ERROR", error);
      throw error;
    }
  }

  private async waitForWebTwain(runtime: DynamsoftWebTwainFactory) {
    const startedAt = Date.now();
    let lastDwt: DynamsoftWebTwainObject | null = null;

    while (Date.now() - startedAt <= DYNAMSOFT_WEBTWAIN_READY_TIMEOUT_MS) {
      lastDwt = runtime.GetWebTwain(WEB_TWAIN_ID);
      console.log("GET_WEBTWAIN_POLL", {
        elapsedMs: Date.now() - startedAt,
        exists: Boolean(lastDwt),
        destroy: readDiagnosticValue(lastDwt, ["_destroy"]),
        ready: readDiagnosticValue(lastDwt, ["_bReady"]),
      });

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

    console.log("GET_WEBTWAIN_TIMEOUT", {
      timeoutMs: DYNAMSOFT_WEBTWAIN_READY_TIMEOUT_MS,
      lastDwt,
      destroy: readDiagnosticValue(lastDwt, ["_destroy"]),
      ready: readDiagnosticValue(lastDwt, ["_bReady"]),
    });
    return null;
  }

  private logDwtLifecycle(label: string, dwt: DynamsoftWebTwainObject | null) {
    const snapshot = getDwtLifecycleSnapshot(dwt);
    console.log("DWT_LIFECYCLE", {
      label,
      instanceId: this.instanceId,
      ...snapshot,
    });
  }

  private startDestroyWatchdog() {
    this.stopDestroyWatchdog();
    this.lastDestroySnapshot = readDiagnosticValue(this.dwt, ["_destroy"]);
    this.destroyWatchdogId = window.setInterval(() => {
      const snapshot = getDwtLifecycleSnapshot(this.dwt);
      if (snapshot.destroy !== this.lastDestroySnapshot) {
        console.warn("DWT_DESTROY_CHANGED", {
          instanceId: this.instanceId,
          previousDestroy: this.lastDestroySnapshot,
          ...snapshot,
          stack: new Error("DWT_DESTROY_CHANGED_STACK").stack,
        });
        this.lastDestroySnapshot = snapshot.destroy;
      }

      console.log("DWT_DESTROY_WATCHDOG", {
        instanceId: this.instanceId,
        ...snapshot,
      });
    }, 1000);
  }

  private stopDestroyWatchdog() {
    if (this.destroyWatchdogId !== null) {
      window.clearInterval(this.destroyWatchdogId);
      this.destroyWatchdogId = null;
    }
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
