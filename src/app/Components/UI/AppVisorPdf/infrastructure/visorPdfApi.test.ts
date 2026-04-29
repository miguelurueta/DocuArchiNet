import { beforeEach, describe, expect, it, vi } from "vitest";
import type { ApiResponse } from "../../../../../api/ApiResponse";
import type { VisorPdfAnnotationsPayloadV1 } from "../domain/annotations.types";
import type { VisorPdfStampConfig } from "../domain/visorPdfApi.types";

const getMock = vi.fn();
const putMock = vi.fn();

vi.mock("../../../../../api/Clienteaxios", () => ({
  default: {
    get: getMock,
    put: putMock,
  },
}));

describe("createAppVisorPdfApi [SPEC:SCRUMCORE-193]", () => {
  beforeEach(() => {
    getMock.mockReset();
    putMock.mockReset();
  });

  it("getPdfUrl retorna ApiResponse con url", async () => {
    const { createAppVisorPdfApi } = await import("./visorPdfApi");
    const api = createAppVisorPdfApi({ basePath: "/x" });

    const payload: ApiResponse<{ url: string; expiresAtIso?: string }> = {
      success: true,
      message: "ok",
      data: { url: "https://example.com/file.pdf", expiresAtIso: "2026-01-01" },
    };
    getMock.mockResolvedValueOnce({ data: payload });

    await expect(api.getPdfUrl("doc-1")).resolves.toEqual(payload);
    expect(getMock).toHaveBeenCalledTimes(1);
  });

  it("getAnnotations retorna ApiResponse<VisorPdfAnnotationsPayloadV1>", async () => {
    const { createAppVisorPdfApi } = await import("./visorPdfApi");
    const api = createAppVisorPdfApi({ basePath: "/x" });

    const annotations: VisorPdfAnnotationsPayloadV1 = {
      version: 1,
      pages: [{ pageNumber: 1, objects: [] }],
    };
    const payload: ApiResponse<VisorPdfAnnotationsPayloadV1> = {
      success: true,
      message: "ok",
      data: annotations,
    };
    getMock.mockResolvedValueOnce({ data: payload });

    await expect(api.getAnnotations("doc-1")).resolves.toEqual(payload);
    expect(getMock).toHaveBeenCalledTimes(1);
  });

  it("saveAnnotations envia payload correcto", async () => {
    const { createAppVisorPdfApi } = await import("./visorPdfApi");
    const api = createAppVisorPdfApi({ basePath: "/x" });

    const annotations: VisorPdfAnnotationsPayloadV1 = {
      version: 1,
      pages: [{ pageNumber: 1, objects: [{ kind: "rect" }] }],
    };

    const payload: ApiResponse<{ savedAtIso: string }> = {
      success: true,
      message: "ok",
      data: { savedAtIso: "2026-01-01" },
    };
    putMock.mockResolvedValueOnce({ data: payload });

    await expect(api.saveAnnotations("doc-1", annotations)).resolves.toEqual(payload);
    expect(putMock).toHaveBeenCalledTimes(1);
    expect(putMock.mock.calls[0]?.[1]).toEqual(annotations);
  });

  it("getStampConfig retorna ApiResponse<VisorPdfStampConfig>", async () => {
    const { createAppVisorPdfApi } = await import("./visorPdfApi");
    const api = createAppVisorPdfApi({ basePath: "/x" });

    const config: VisorPdfStampConfig = {
      enabled: true,
      opacity: 0.5,
      scale: 1,
      rotationDeg: 0,
    };

    const payload: ApiResponse<VisorPdfStampConfig> = {
      success: true,
      message: "ok",
      data: config,
    };
    getMock.mockResolvedValueOnce({ data: payload });

    await expect(api.getStampConfig()).resolves.toEqual(payload);
    expect(getMock).toHaveBeenCalledTimes(1);
  });

  it("saveStampConfig envia payload correcto", async () => {
    const { createAppVisorPdfApi } = await import("./visorPdfApi");
    const api = createAppVisorPdfApi({ basePath: "/x" });

    const config: VisorPdfStampConfig = {
      enabled: false,
      opacity: 0.8,
      scale: 0.75,
      rotationDeg: 15,
    };

    const payload: ApiResponse<{ savedAtIso: string }> = {
      success: true,
      message: "ok",
      data: { savedAtIso: "2026-01-01" },
    };
    putMock.mockResolvedValueOnce({ data: payload });

    await expect(api.saveStampConfig(config)).resolves.toEqual(payload);
    expect(putMock).toHaveBeenCalledTimes(1);
    expect(putMock.mock.calls[0]?.[1]).toEqual(config);
  });

  it("propaga errores 401/403 (reject) sin normalizar", async () => {
    const { createAppVisorPdfApi } = await import("./visorPdfApi");
    const api = createAppVisorPdfApi({ basePath: "/x" });

    const error401 = Object.assign(new Error("Unauthorized"), {
      response: { status: 401 },
    });
    getMock.mockRejectedValueOnce(error401);
    await expect(api.getAnnotations("doc-1")).rejects.toBe(error401);

    const error403 = Object.assign(new Error("Forbidden"), {
      response: { status: 403 },
    });
    getMock.mockRejectedValueOnce(error403);
    await expect(api.getAnnotations("doc-1")).rejects.toBe(error403);
  });
});
