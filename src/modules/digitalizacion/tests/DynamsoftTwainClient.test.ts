import { beforeEach, describe, expect, it, vi } from "vitest";
import {
  DYNAMSOFT_CSS_ID_PREFIX,
  DYNAMSOFT_DEFAULT_RESOURCES_PATH,
  DYNAMSOFT_SCRIPT_ID,
  DynamsoftTwainClient,
} from "../infrastructure/dynamsoft";
import { resetDynamsoftScriptLoaderForTests } from "../infrastructure/dynamsoft/loadDynamsoftScripts";
import type {
  DynamsoftWebTwainFactory,
  DynamsoftWebTwainObject,
  DynamsoftWindow,
} from "../infrastructure/dynamsoft";

const addLoadedScript = () => {
  const script = document.createElement("script");
  script.id = DYNAMSOFT_SCRIPT_ID;
  script.setAttribute("data-loaded", "true");
  document.head.appendChild(script);
};

const addLoadedCss = () => {
  [0, 1].forEach((index) => {
    const link = document.createElement("link");
    link.id = `${DYNAMSOFT_CSS_ID_PREFIX}-${index}`;
    link.rel = "stylesheet";
    link.setAttribute("data-loaded", "true");
    document.head.appendChild(link);
  });
};

const createDwt = (
  acquiredImageCount = 2,
  options: { removeImageUpdatesBuffer?: boolean } = {},
): DynamsoftWebTwainObject => {
  const state = { imageCount: 0 };
  const removeImageUpdatesBuffer = options.removeImageUpdatesBuffer ?? true;

  return {
    SourceCount: 2,
    get HowManyImagesInBuffer() {
      return state.imageCount;
    },
    SelectSourceByIndex: vi.fn(() => true),
    GetSourceNameItems: vi.fn((index: number) => `Scanner ${index + 1}`),
    GetImageURL: vi.fn((index: number, width?: number, height?: number) =>
      `dwt://image-${index}-${width ?? -1}-${height ?? -1}`,
    ),
    GetImageWidth: vi.fn(() => 1700),
    GetImageHeight: vi.fn(() => 2200),
    OpenSource: vi.fn(),
    CloseSource: vi.fn(),
    AcquireImage: vi.fn((_options, onSuccess) => {
      state.imageCount = acquiredImageCount;
      onSuccess();
    }),
    RemoveImage: vi.fn(() => {
      if (removeImageUpdatesBuffer) {
        state.imageCount -= 1;
      }
    }),
    RemoveAllImages: vi.fn(() => {
      state.imageCount = 0;
    }),
    Rotate: vi.fn(() => true),
    Crop: vi.fn(() => true),
    ConvertToBlob: vi.fn((_indices, _type, onSuccess) => {
      onSuccess(new Blob(["pdf"], { type: "application/pdf" }));
    }),
  };
};

const createRuntime = (dwt: DynamsoftWebTwainObject): DynamsoftWebTwainFactory => ({
  Load: vi.fn(),
  Unload: vi.fn(),
  GetWebTwain: vi.fn(() => dwt),
});

const createClient = (runtime: DynamsoftWebTwainFactory) =>
  new DynamsoftTwainClient({
    licenseKey: "license",
    windowRef: { Dynamsoft: { DWT: runtime } } as DynamsoftWindow,
    documentRef: document,
  });

type PixelPattern = (width: number, height: number) => Uint8ClampedArray;

const createPattern = (
  base: number,
  marks: Array<{
    x: number;
    y: number;
    width: number;
    height: number;
    value: number;
  }> = [],
): PixelPattern => (width, height) => {
  const pixels = new Uint8ClampedArray(width * height * 4);
  for (let offset = 0; offset < pixels.length; offset += 4) {
    pixels[offset] = base;
    pixels[offset + 1] = base;
    pixels[offset + 2] = base;
    pixels[offset + 3] = 255;
  }

  marks.forEach((mark) => {
    const maxY = Math.min(mark.y + mark.height, height);
    const maxX = Math.min(mark.x + mark.width, width);
    for (let y = mark.y; y < maxY; y += 1) {
      for (let x = mark.x; x < maxX; x += 1) {
        const offset = (y * width + x) * 4;
        pixels[offset] = mark.value;
        pixels[offset + 1] = mark.value;
        pixels[offset + 2] = mark.value;
        pixels[offset + 3] = 255;
      }
    }
  });

  return pixels;
};

const createSparseNoisePattern = (noisePixels: number, value = 240): PixelPattern => (width, height) => {
  const pixels = createPattern(255)(width, height);
  for (let index = 0; index < noisePixels; index += 1) {
    const x = (index * 37) % width;
    const y = (index * 53) % height;
    const offset = (y * width + x) * 4;
    pixels[offset] = value;
    pixels[offset + 1] = value;
    pixels[offset + 2] = value;
  }

  return pixels;
};

const installAnalysisCanvasMock = (patterns: Map<string, PixelPattern>) => {
  const createElement = document.createElement.bind(document);

  return vi.spyOn(document, "createElement").mockImplementation((tagName: string) => {
    if (tagName !== "canvas") {
      return createElement(tagName);
    }

    let imageSource = "";
    const canvas = {
      width: 0,
      height: 0,
      getContext: vi.fn(() => ({
        fillStyle: "",
        fillRect: vi.fn(),
        drawImage: vi.fn((image: { src: string }) => {
          imageSource = image.src;
        }),
        getImageData: vi.fn((_x: number, _y: number, width: number, height: number) => ({
          data: (patterns.get(imageSource) ?? createPattern(255))(width, height),
        })),
      })),
    };

    return canvas as unknown as HTMLCanvasElement;
  });
};

const createAnalysisWindow = (
  runtime: DynamsoftWebTwainFactory,
  loadedSources: string[],
): DynamsoftWindow => {
  class TestImage {
    onload: (() => void) | null = null;
    onerror: (() => void) | null = null;
    private currentSrc = "";

    set src(value: string) {
      this.currentSrc = value;
      loadedSources.push(value);
      queueMicrotask(() => {
        this.onload?.();
      });
    }

    get src() {
      return this.currentSrc;
    }
  }

  return {
    Dynamsoft: { DWT: runtime },
    Image: TestImage,
  } as unknown as DynamsoftWindow;
};

describe("[SPEC:SCRUMCORE-240] DynamsoftTwainClient", () => {
  beforeEach(() => {
    resetDynamsoftScriptLoaderForTests();
    document.head.innerHTML = "";
    addLoadedScript();
    addLoadedCss();
  });

  it("initializes and lists devices", async () => {
    const dwt = createDwt();
    const runtime = createRuntime(dwt);
    const client = createClient(runtime);

    await client.initialize();

    await expect(client.listDevices()).resolves.toEqual([
      { id: "0", name: "Scanner 1", index: 0 },
      { id: "1", name: "Scanner 2", index: 1 },
    ]);
    expect(runtime.Load).toHaveBeenCalled();
    expect(runtime.ResourcesPath).toBe(DYNAMSOFT_DEFAULT_RESOURCES_PATH);
  });

  it("maps DWT css load failure to a functional error", async () => {
    const runtime = createRuntime(createDwt());
    vi.mocked(runtime.Load).mockImplementation(() => {
      throw { code: -2804, message: "Loading the WebTwain css files failed." };
    });
    const client = createClient(runtime);

    await expect(client.initialize()).rejects.toMatchObject({
      code: "DYNAMSOFT_CSS_LOAD_FAILED",
      message: "No fue posible cargar los estilos CSS de Dynamsoft Web TWAIN.",
    });
  });

  it("fails when runtime is unavailable", async () => {
    const client = new DynamsoftTwainClient({
      licenseKey: "license",
      windowRef: {} as DynamsoftWindow,
      documentRef: document,
    });

    await expect(client.initialize()).rejects.toMatchObject({
      code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
    });
  });

  it("fails when license is missing", async () => {
    const client = new DynamsoftTwainClient({
      windowRef: { Dynamsoft: { DWT: createRuntime(createDwt()) } } as DynamsoftWindow,
      documentRef: document,
    });

    await expect(client.initialize()).rejects.toMatchObject({
      code: "DYNAMSOFT_LICENSE_INVALID",
    });
  });

  it("selects scanner, scans pages and generates PDF only", async () => {
    const dwt = createDwt();
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.selectDevice("0");
    const pages = await client.scan({ deviceId: "0", colorMode: "color" });
    const pdf = await client.generatePdf("digitalizacion");

    expect(pages).toEqual([
      {
        id: "scan-page-1",
        index: 0,
        thumbnailUrl: "dwt://image-0-160-220",
        imageUrl: "dwt://image-0--1--1",
        width: 1700,
        height: 2200,
        orientation: "portrait",
        rotationDegrees: 0,
      },
      {
        id: "scan-page-2",
        index: 1,
        thumbnailUrl: "dwt://image-1-160-220",
        imageUrl: "dwt://image-1--1--1",
        width: 1700,
        height: 2200,
        orientation: "portrait",
        rotationDegrees: 0,
      },
    ]);
    expect(pdf.pageCount).toBe(2);
    expect(pdf.file.name).toBe("digitalizacion.pdf");
    expect(pdf.file.type).toBe("application/pdf");
    expect(dwt.GetImageURL).toHaveBeenCalledWith(0, 160, 220);
    expect(dwt.GetImageURL).toHaveBeenCalledWith(0, -1, -1);
    expect(dwt.GetImageWidth).toHaveBeenCalledWith(0);
    expect(dwt.GetImageHeight).toHaveBeenCalledWith(0);
  });

  it("keeps scan successful when automatic processing APIs are unavailable", async () => {
    const dwt = createDwt();
    const client = createClient(createRuntime(dwt));
    const infoSpy = vi.spyOn(console, "info").mockImplementation(() => undefined);

    await client.initialize();
    await client.selectDevice("0");
    const pages = await client.scan({
      deviceId: "0",
      automaticProcessing: {
        deskew: true,
        autoCrop: true,
        autoRotate: true,
      },
    });

    expect(pages).toHaveLength(2);
    expect(infoSpy).toHaveBeenCalledWith(
      "DESKEW_TIME",
      expect.objectContaining({ status: "unsupported" }),
    );
    expect(infoSpy).toHaveBeenCalledWith(
      "AUTOCROP_TIME",
      expect.objectContaining({ status: "unsupported" }),
    );
    expect(infoSpy).toHaveBeenCalledWith(
      "AUTOROTATE_TIME",
      expect.objectContaining({ status: "unsupported" }),
    );

    infoSpy.mockRestore();
  });

  it("uses native automatic processing methods when the DWT runtime exposes them", async () => {
    const dwt = createDwt();
    dwt.Deskew = vi.fn(() => true);
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.selectDevice("0");
    await client.scan({
      deviceId: "0",
      automaticProcessing: {
        deskew: true,
      },
    });

    expect(dwt.Deskew).toHaveBeenCalledWith(0);
    expect(dwt.Deskew).toHaveBeenCalledWith(1);
    expect(dwt.GetImageURL).toHaveBeenCalledWith(0, 160, 220);
    expect(dwt.GetImageURL).toHaveBeenCalledWith(1, 160, 220);
  });

  it("[SPEC:SCRUMCORE-257] crops one page using real page coordinates", async () => {
    const dwt = createDwt();
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.selectDevice("0");
    const pages = await client.scan({ deviceId: "0" });
    const croppedPages = await client.cropPage(pages[0].id, {
      x: 120.4,
      y: 240.6,
      width: 500.2,
      height: 700.1,
    });

    expect(dwt.Crop).toHaveBeenCalledWith(0, 120, 240, 621, 941);
    expect(croppedPages).toHaveLength(2);
    expect(croppedPages[0].id).toBe(pages[0].id);
    expect(dwt.RemoveImage).not.toHaveBeenCalled();
    expect(dwt.ConvertToBlob).not.toHaveBeenCalled();
  });

  it("[SPEC:SCRUMCORE-257] crops the original buffer page after reorder", async () => {
    const dwt = createDwt();
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.selectDevice("0");
    const pages = await client.scan({ deviceId: "0" });
    await client.reorderPages([pages[1].id, pages[0].id]);
    await client.cropPage(pages[1].id, {
      x: 10,
      y: 20,
      width: 30,
      height: 40,
    });

    expect(dwt.Crop).toHaveBeenCalledWith(1, 10, 20, 40, 60);
  });

  it("analyzes the original image for blank-page removal and keeps valid low-contrast pages", async () => {
    const dwt = createDwt(6);
    const patterns = new Map<string, PixelPattern>([
      ["dwt://image-0--1--1", createPattern(255)],
      ["dwt://image-1--1--1", createPattern(255, [{ x: 24, y: 24, width: 4, height: 4, value: 0 }])],
      ["dwt://image-2--1--1", createPattern(255, [{ x: 32, y: 64, width: 40, height: 20, value: 240 }])],
      ["dwt://image-3--1--1", createPattern(255, [{ x: 40, y: 100, width: 80, height: 12, value: 165 }])],
      ["dwt://image-4--1--1", createPattern(255, [{ x: 48, y: 140, width: 80, height: 12, value: 242 }])],
      ["dwt://image-5--1--1", createSparseNoisePattern(100, 0)],
      ["dwt://image-0-160-220", createPattern(255)],
      ["dwt://image-1-160-220", createPattern(255)],
      ["dwt://image-2-160-220", createPattern(255)],
      ["dwt://image-3-160-220", createPattern(255)],
      ["dwt://image-4-160-220", createPattern(255)],
      ["dwt://image-5-160-220", createPattern(255)],
    ]);
    const loadedSources: string[] = [];
    const runtime = createRuntime(dwt);
    const createElementSpy = installAnalysisCanvasMock(patterns);
    const client = new DynamsoftTwainClient({
      licenseKey: "license",
      windowRef: createAnalysisWindow(runtime, loadedSources),
      documentRef: document,
    });
    const infoSpy = vi.spyOn(console, "info").mockImplementation(() => undefined);

    try {
      await client.initialize();
      await client.selectDevice("0");
      const pages = await client.scan({ deviceId: "0", removeBlankPages: true });

      expect(loadedSources).toEqual([
        "dwt://image-0--1--1",
        "dwt://image-1--1--1",
        "dwt://image-2--1--1",
        "dwt://image-3--1--1",
        "dwt://image-4--1--1",
        "dwt://image-5--1--1",
      ]);
      expect(dwt.RemoveImage).toHaveBeenCalledTimes(2);
      expect(dwt.RemoveImage).toHaveBeenNthCalledWith(1, 5);
      expect(dwt.RemoveImage).toHaveBeenNthCalledWith(2, 0);
      expect(pages.map((page) => page.id)).toEqual([
        "scan-page-2",
        "scan-page-3",
        "scan-page-4",
        "scan-page-5",
      ]);
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_ANALYSIS_START",
        expect.objectContaining({
          imageSource: "original",
          analysisWidth: 384,
          analysisHeight: 512,
          whiteThreshold: 245,
          contentThreshold: 0.003,
          darkPixelThreshold: 12,
        }),
      );
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_DARK_PIXELS",
        expect.objectContaining({
          pageId: "scan-page-6",
          darkPixels: 100,
          clusteredDarkPixels: 0,
          darkPixelThreshold: 12,
        }),
      );
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_REMOVED",
        expect.objectContaining({ pageId: "scan-page-1" }),
      );
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_REMOVED",
        expect.objectContaining({ pageId: "scan-page-6" }),
      );
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_KEPT",
        expect.objectContaining({ pageId: "scan-page-2" }),
      );
    } finally {
      infoSpy.mockRestore();
      createElementSpy.mockRestore();
    }
  });

  it("keeps blank pages out of scanner pages and PDF when DWT RemoveImage does not update the buffer", async () => {
    const dwt = createDwt(3, { removeImageUpdatesBuffer: false });
    const patterns = new Map<string, PixelPattern>([
      ["dwt://image-0--1--1", createPattern(255)],
      ["dwt://image-1--1--1", createPattern(255, [{ x: 32, y: 64, width: 40, height: 20, value: 0 }])],
      ["dwt://image-2--1--1", createPattern(255, [{ x: 48, y: 128, width: 80, height: 18, value: 165 }])],
    ]);
    const loadedSources: string[] = [];
    const runtime = createRuntime(dwt);
    const createElementSpy = installAnalysisCanvasMock(patterns);
    const client = new DynamsoftTwainClient({
      licenseKey: "license",
      windowRef: createAnalysisWindow(runtime, loadedSources),
      documentRef: document,
    });
    const infoSpy = vi.spyOn(console, "info").mockImplementation(() => undefined);

    try {
      await client.initialize();
      await client.selectDevice("0");
      const pages = await client.scan({ deviceId: "0", removeBlankPages: true });
      await client.generatePdf("sin-blancas");

      expect(dwt.RemoveImage).toHaveBeenCalledWith(0);
      expect(dwt.HowManyImagesInBuffer).toBe(3);
      expect(pages.map((page) => page.id)).toEqual(["scan-page-2", "scan-page-3"]);
      expect(pages.map((page) => page.index)).toEqual([1, 2]);
      expect(dwt.ConvertToBlob).toHaveBeenCalledWith(
        [1, 2],
        "application/pdf",
        expect.any(Function),
        expect.any(Function),
      );
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_DETECTED",
        expect.objectContaining({ pageId: "scan-page-1", pageIndex: 0, pageNumber: 1 }),
      );
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_SURVIVED",
        expect.objectContaining({
          stage: "removeImage",
          pageId: "scan-page-1",
          removedFromBuffer: false,
        }),
      );
      expect(infoSpy).toHaveBeenCalledWith(
        "BLANK_PAGE_FINAL_STATE",
        expect.objectContaining({
          stage: "afterBlankRemoval",
          pageCount: 2,
          bufferCount: 3,
          survivedIndexes: [0],
        }),
      );
    } finally {
      infoSpy.mockRestore();
      createElementSpy.mockRestore();
    }
  });

  it("selects cached SourceCount scanner through legacy source index when SourceCount changes later", async () => {
    const dwt = createDwt();
    dwt.SelectDeviceAsync = vi.fn(async () => true);
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.listDevices();
    dwt.SourceCount = 0;
    await client.selectDevice("1");

    expect(dwt.SelectDeviceAsync).not.toHaveBeenCalled();
    expect(dwt.SelectSourceByIndex).toHaveBeenCalledWith(1);
  });

  it("uses DWT 19 device API when available instead of legacy source index", async () => {
    const dwt = createDwt();
    dwt.SourceCount = 0;
    dwt.GetDevicesAsync = vi.fn(async () => [
      { name: "paperstream", displayName: "PaperStream IP fi-7160 #2" },
      { name: "wiatwain", displayName: "WIATWAIN-fi-7160 #2" },
    ]);
    dwt.SelectDeviceAsync = vi.fn(async () => true);
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    const devices = await client.listDevices();
    await client.selectDevice("1");

    expect(devices).toEqual([
      { id: "0", name: "PaperStream IP fi-7160 #2", index: 0 },
      { id: "1", name: "WIATWAIN-fi-7160 #2", index: 1 },
    ]);
    expect(dwt.SelectDeviceAsync).toHaveBeenCalledWith({
      name: "wiatwain",
      displayName: "WIATWAIN-fi-7160 #2",
    });
    expect(dwt.SelectSourceByIndex).not.toHaveBeenCalled();
  });

  it("prefers SourceCount discovery and legacy selection over DWT 19 discovery", async () => {
    const dwt = createDwt();
    dwt.GetDevicesAsync = vi.fn(async () => {
      throw new Error("GetDevicesAsync timeout");
    });
    dwt.SelectDeviceAsync = vi.fn(async () => true);
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    const devices = await client.listDevices();
    await client.selectDevice("1");

    expect(devices).toEqual([
      { id: "0", name: "Scanner 1", index: 0 },
      { id: "1", name: "Scanner 2", index: 1 },
    ]);
    expect(dwt.GetDevicesAsync).not.toHaveBeenCalled();
    expect(dwt.SelectDeviceAsync).not.toHaveBeenCalled();
    expect(dwt.SelectSourceByIndex).toHaveBeenCalledWith(1);
  });

  it("selects cached SourceCount scanner through legacy source index when SourceCount changes later", async () => {
    const dwt = createDwt();
    dwt.SelectDeviceAsync = vi.fn(async () => true);
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.listDevices();
    dwt.SourceCount = 0;
    await client.selectDevice("1");

    expect(dwt.SelectDeviceAsync).not.toHaveBeenCalled();
    expect(dwt.SelectSourceByIndex).toHaveBeenCalledWith(1);
  });

  it("uses DWT 19 device API when available instead of legacy source index", async () => {
    const dwt = createDwt();
    dwt.SourceCount = 0;
    dwt.GetDevicesAsync = vi.fn(async () => [
      { name: "paperstream", displayName: "PaperStream IP fi-7160 #2" },
      { name: "wiatwain", displayName: "WIATWAIN-fi-7160 #2" },
    ]);
    dwt.SelectDeviceAsync = vi.fn(async () => true);
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    const devices = await client.listDevices();
    await client.selectDevice("1");

    expect(devices).toEqual([
      { id: "0", name: "PaperStream IP fi-7160 #2", index: 0 },
      { id: "1", name: "WIATWAIN-fi-7160 #2", index: 1 },
    ]);
    expect(dwt.SelectDeviceAsync).toHaveBeenCalledWith({
      name: "wiatwain",
      displayName: "WIATWAIN-fi-7160 #2",
    });
    expect(dwt.SelectSourceByIndex).not.toHaveBeenCalled();
  });

  it("prefers SourceCount discovery and legacy selection over DWT 19 discovery", async () => {
    const dwt = createDwt();
    dwt.GetDevicesAsync = vi.fn(async () => {
      throw new Error("GetDevicesAsync timeout");
    });
    dwt.SelectDeviceAsync = vi.fn(async () => true);
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    const devices = await client.listDevices();
    await client.selectDevice("1");

    expect(devices).toEqual([
      { id: "0", name: "Scanner 1", index: 0 },
      { id: "1", name: "Scanner 2", index: 1 },
    ]);
    expect(dwt.GetDevicesAsync).not.toHaveBeenCalled();
    expect(dwt.SelectDeviceAsync).not.toHaveBeenCalled();
    expect(dwt.SelectSourceByIndex).toHaveBeenCalledWith(1);
  });

  it("blocks scan without selected scanner", async () => {
    const client = createClient(createRuntime(createDwt()));

    await client.initialize();

    await expect(client.scan({ deviceId: "0" })).rejects.toMatchObject({
      code: "SCANNER_NOT_SELECTED",
    });
  });

  it("validates scan options", async () => {
    const client = createClient(createRuntime(createDwt()));

    await client.initialize();

    await expect(
      client.scan({ deviceId: "", resolutionDpi: 999 }),
    ).rejects.toMatchObject({ code: "INVALID_DEVICE_ID" });
  });

  it("removes, rotates and clears pages by stable page id", async () => {
    const dwt = createDwt();
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.selectDevice("0");
    const pages = await client.scan({ deviceId: "0" });
    await client.rotatePage(pages[0].id, 90);
    await client.removePage(pages[0].id);
    await client.clear();

    expect(dwt.Rotate).toHaveBeenCalledWith(0, 90, true);
    expect(dwt.RemoveImage).toHaveBeenCalledWith(0);
    expect(dwt.RemoveAllImages).toHaveBeenCalled();
  });

  it("generates PDF using reordered page indices", async () => {
    const dwt = createDwt();
    const client = createClient(createRuntime(dwt));

    await client.initialize();
    await client.selectDevice("0");
    const pages = await client.scan({ deviceId: "0" });
    const reorderedPages = await client.reorderPages([pages[1].id, pages[0].id]);
    await client.generatePdf("reordered.pdf");

    expect(reorderedPages.map((page) => page.id)).toEqual([pages[1].id, pages[0].id]);
    expect(dwt.ConvertToBlob).toHaveBeenCalledWith(
      [1, 0],
      "application/pdf",
      expect.any(Function),
      expect.any(Function),
    );
  });

  it("fails PDF generation with no pages", async () => {
    const client = createClient(createRuntime(createDwt()));

    await client.initialize();

    await expect(client.generatePdf("empty.pdf")).rejects.toMatchObject({
      code: "PDF_EMPTY",
    });
  });

  it("disposes runtime and ignores later use", async () => {
    const runtime = createRuntime(createDwt());
    const client = createClient(runtime);

    await client.initialize();
    await client.dispose();

    expect(runtime.Unload).toHaveBeenCalled();
    await expect(client.listDevices()).rejects.toMatchObject({
      code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
    });
  });
});
