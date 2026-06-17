import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { DynamsoftScannerError } from "../infrastructure/dynamsoft";
import { useDigitalizacionScanner } from "../hooks/useDigitalizacionScanner";
import type {
  DigitalizacionScannerClient,
  PdfGenerationResult,
  ScanOptions,
  ScanPage,
  ScannerDevice,
} from "../infrastructure/dynamsoft";

const createDeferred = <T,>() => {
  let resolveValue: (value: T) => void = () => undefined;
  const promise = new Promise<T>((resolve) => {
    resolveValue = resolve;
  });

  return { promise, resolve: resolveValue };
};

const createClient = (): DigitalizacionScannerClient & {
  devices: ScannerDevice[];
  pages: ScanPage[];
  pdf: PdfGenerationResult;
} => {
  const pdfFile = new File(["pdf"], "scan.pdf", { type: "application/pdf" });

  return {
    devices: [{ id: "0", name: "Scanner 1", index: 0 }],
    pages: [
      { id: "page-1", index: 0 },
      { id: "page-2", index: 1 },
    ],
    pdf: { file: pdfFile, pageCount: 1 },
    initialize: vi.fn(async () => undefined),
    listDevices: vi.fn(async function listDevices(this: { devices: ScannerDevice[] }) {
      return this.devices;
    }),
    selectDevice: vi.fn(async () => undefined),
    scan: vi.fn(async function scan(this: { pages: ScanPage[] }, options: ScanOptions) {
      expect(options.deviceId).toBeTypeOf("string");
      return this.pages;
    }),
    rotatePage: vi.fn(async () => undefined),
    removePage: vi.fn(async () => undefined),
    reorderPages: vi.fn(async function reorderPages(this: { pages: ScanPage[] }, pageIds: string[]) {
      this.pages = pageIds
        .map((pageId) => this.pages.find((page) => page.id === pageId))
        .filter((page): page is ScanPage => Boolean(page));
      return this.pages;
    }),
    clear: vi.fn(async () => undefined),
    generatePdf: vi.fn(async function generatePdf(this: { pdf: PdfGenerationResult }) {
      return this.pdf;
    }),
    dispose: vi.fn(async () => undefined),
  };
};

describe("[SPEC:SCRUMCORE-240] useDigitalizacionScanner", () => {
  it("initializes through adapter and exposes ready state", async () => {
    const client = createClient();
    const { result } = renderHook(() => useDigitalizacionScanner({ client }));

    await act(async () => {
      await result.current.initialize();
    });

    expect(result.current.status).toBe("ready");
    expect(result.current.devices).toEqual(client.devices);
    expect(client.initialize).toHaveBeenCalled();
  });

  it("selects scanner, scans pages and generates PDF", async () => {
    const client = createClient();
    const { result } = renderHook(() => useDigitalizacionScanner({ client }));

    await act(async () => {
      await result.current.selectDevice("0");
      await result.current.scan({ deviceId: "0" });
      await result.current.generatePdf("scan.pdf");
    });

    expect(result.current.selectedDeviceId).toBe("0");
    expect(result.current.pages).toEqual(client.pages);
    expect(result.current.pdf).toEqual(client.pdf);
  });

  it("exposes functional errors", async () => {
    const client = createClient();
    vi.mocked(client.initialize).mockRejectedValueOnce(
      new DynamsoftScannerError({
        code: "DYNAMSOFT_RUNTIME_UNAVAILABLE",
        message: "Runtime no disponible",
      }),
    );
    const { result } = renderHook(() => useDigitalizacionScanner({ client }));

    await act(async () => {
      await result.current.initialize();
    });

    expect(result.current.status).toBe("error");
    expect(result.current.error?.code).toBe("DYNAMSOFT_RUNTIME_UNAVAILABLE");
  });

  it("clears and removes pages through adapter", async () => {
    const client = createClient();
    const { result } = renderHook(() => useDigitalizacionScanner({ client }));

    await act(async () => {
      await result.current.scan({ deviceId: "0" });
      await result.current.removePage("page-1");
      await result.current.clear();
    });

    expect(client.removePage).toHaveBeenCalledWith("page-1");
    expect(client.clear).toHaveBeenCalled();
    expect(result.current.pages).toEqual([]);
  });

  it("reorders pages through adapter and clears generated pdf", async () => {
    const client = createClient();
    const { result } = renderHook(() => useDigitalizacionScanner({ client }));

    await act(async () => {
      await result.current.scan({ deviceId: "0" });
      await result.current.generatePdf("scan.pdf");
      await result.current.reorderPages(["page-2", "page-1"]);
    });

    expect(client.reorderPages).toHaveBeenCalledWith(["page-2", "page-1"]);
    expect(result.current.pages.map((page) => page.id)).toEqual(["page-2", "page-1"]);
    expect(result.current.pdf).toBeNull();
  });

  it("ignores stale initialize result after dispose", async () => {
    const client = createClient();
    const deferred = createDeferred<void>();
    vi.mocked(client.initialize).mockReturnValueOnce(deferred.promise);
    const { result } = renderHook(() => useDigitalizacionScanner({ client }));

    void act(() => {
      void result.current.initialize();
    });
    await act(async () => {
      await result.current.dispose();
    });
    await act(async () => {
      deferred.resolve(undefined);
    });

    await waitFor(() => {
      expect(result.current.status).toBe("idle");
    });
  });
});
