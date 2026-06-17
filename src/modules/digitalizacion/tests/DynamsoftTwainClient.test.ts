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

const createDwt = (): DynamsoftWebTwainObject => {
  const state = { imageCount: 0 };

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
      state.imageCount = 2;
      onSuccess();
    }),
    RemoveImage: vi.fn(() => {
      state.imageCount -= 1;
    }),
    RemoveAllImages: vi.fn(() => {
      state.imageCount = 0;
    }),
    Rotate: vi.fn(() => true),
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
