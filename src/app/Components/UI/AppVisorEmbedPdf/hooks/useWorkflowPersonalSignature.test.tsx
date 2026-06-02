import { act, renderHook, waitFor } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";

import { useWorkflowPersonalSignature } from "./useWorkflowPersonalSignature";

const clienteApiGetMock = vi.fn();

vi.mock("../../../../../api/Clienteaxios", () => ({
  default: {
    defaults: { baseURL: "https://localhost:7101" },
    get: (...args: unknown[]) => clienteApiGetMock(...args),
  },
}));

const metadataResponse = (token: string) => ({
  data: {
    success: true,
    message: "YES",
    data: {
      IdUsuarioWorkflow: 141,
      FileName: `firma-${token}.png`,
      ContentType: "image/png",
      RelativePath: `signatures/firma-${token}.png`,
      UrlTemporal: `/api/workflow/usuarios/firma-temporal/download/${token}`,
      ExpiresAt: "2026-05-15T00:00:00Z",
    },
    errors: [],
  },
});

const pngBlob = () =>
  ({
    type: "image/png",
    arrayBuffer: vi.fn().mockResolvedValue(new Uint8Array([112, 110, 103]).buffer),
  }) as unknown as Blob;

describe("useWorkflowPersonalSignature", () => {
  let createObjectUrlSpy: ReturnType<typeof vi.spyOn>;
  let revokeObjectUrlSpy: ReturnType<typeof vi.spyOn>;

  beforeEach(() => {
    clienteApiGetMock.mockReset();
    createObjectUrlSpy = vi
      .spyOn(URL, "createObjectURL")
      .mockImplementation(() => "blob:http://localhost/personal-sig");
    revokeObjectUrlSpy = vi.spyOn(URL, "revokeObjectURL").mockImplementation(() => undefined);
  });

  afterEach(() => {
    createObjectUrlSpy.mockRestore();
    revokeObjectUrlSpy.mockRestore();
  });

  it("[SPEC:SCRUMCORE-211] carga metadata, descarga blob y expone firma personal lista", async () => {
    clienteApiGetMock.mockImplementation((url: string) => {
      if (url === "/api/workflow/usuarios/firma-temporal") {
        return Promise.resolve(metadataResponse("tok-1"));
      }

      if (String(url).includes("/api/workflow/usuarios/firma-temporal/download/tok-1")) {
        return Promise.resolve({ data: pngBlob() });
      }

      throw new Error(`Unexpected GET: ${url}`);
    });

    const { result } = renderHook(() => useWorkflowPersonalSignature());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.errorMessage).toBeNull();
      expect(result.current.status).toBe("ready");
    });

    expect(clienteApiGetMock).toHaveBeenCalledWith("/api/workflow/usuarios/firma-temporal");
    expect(clienteApiGetMock).toHaveBeenCalledWith(
      "https://localhost:7101/api/workflow/usuarios/firma-temporal/download/tok-1",
      { responseType: "blob" },
    );
    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(result.current.blobUrl).toBe("blob:http://localhost/personal-sig");
    expect(result.current.imageData).toBeInstanceOf(ArrayBuffer);
    expect(result.current.meta).toMatchObject({
      fileName: "firma-tok-1.png",
      contentType: "image/png",
    });
    expect(result.current.errorMessage).toBeNull();
  });

  it("[SPEC:SCRUMCORE-211] si descarga retorna 404 reintenta metadata y descarga una vez mas", async () => {
    let metaCall = 0;
    let downloadCall = 0;

    clienteApiGetMock.mockImplementation((url: string) => {
      if (url === "/api/workflow/usuarios/firma-temporal") {
        metaCall += 1;
        return Promise.resolve(metadataResponse(metaCall === 1 ? "tok-1" : "tok-2"));
      }

      if (String(url).includes("/api/workflow/usuarios/firma-temporal/download/")) {
        downloadCall += 1;
        if (downloadCall === 1) {
          return Promise.reject({ response: { status: 404 } });
        }
        return Promise.resolve({ data: pngBlob() });
      }

      throw new Error(`Unexpected GET: ${url}`);
    });

    const { result } = renderHook(() => useWorkflowPersonalSignature());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.errorMessage).toBeNull();
      expect(result.current.status).toBe("ready");
    });

    expect(metaCall).toBe(2);
    expect(downloadCall).toBe(2);
    expect(createObjectUrlSpy).toHaveBeenCalledTimes(1);
    expect(result.current.meta).toMatchObject({
      fileName: "firma-tok-2.png",
      urlTemporal: "/api/workflow/usuarios/firma-temporal/download/tok-2",
    });
  });

  it("[SPEC:SCRUMCORE-211] clear revoca objectURL y vuelve a estado idle", async () => {
    clienteApiGetMock.mockImplementation((url: string) => {
      if (url === "/api/workflow/usuarios/firma-temporal") {
        return Promise.resolve(metadataResponse("tok-1"));
      }

      if (String(url).includes("/api/workflow/usuarios/firma-temporal/download/tok-1")) {
        return Promise.resolve({ data: pngBlob() });
      }

      throw new Error(`Unexpected GET: ${url}`);
    });

    const { result } = renderHook(() => useWorkflowPersonalSignature());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.errorMessage).toBeNull();
      expect(result.current.status).toBe("ready");
    });

    act(() => {
      result.current.clear();
    });

    expect(revokeObjectUrlSpy).toHaveBeenCalledWith("blob:http://localhost/personal-sig");
    expect(result.current.status).toBe("idle");
    expect(result.current.blobUrl).toBeNull();
    expect(result.current.imageData).toBeNull();
    expect(result.current.meta).toBeNull();
  });
});
