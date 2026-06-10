import { beforeEach, describe, expect, it, vi } from "vitest";
import { DynamsoftTwainClient } from "../infrastructure/dynamsoft";
import { DYNAMSOFT_SCRIPT_ID } from "../infrastructure/dynamsoft";
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

const createDwt = (): DynamsoftWebTwainObject => {
  const state = { imageCount: 0 };

  return {
    SourceCount: 2,
    get HowManyImagesInBuffer() {
      return state.imageCount;
    },
    SelectSourceByIndex: vi.fn(() => true),
    GetSourceNameItems: vi.fn((index: number) => `Scanner ${index + 1}`),
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
  });

  it("initializes and lists devices", async () => {
    const dwt = createDwt();
    const runtime = createRuntime(dwt);
    const client = createClient(runtime);

    await client.initialize();

    await expect(client.listDevices()).resolves.toEqual([
      { id: "0", name: "Scanner 1" },
      { id: "1", name: "Scanner 2" },
    ]);
    expect(runtime.Load).toHaveBeenCalled();
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

    expect(pages).toHaveLength(2);
    expect(pdf.pageCount).toBe(2);
    expect(pdf.file.name).toBe("digitalizacion.pdf");
    expect(pdf.file.type).toBe("application/pdf");
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
