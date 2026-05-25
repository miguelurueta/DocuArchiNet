import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useDocumentViewerOrchestrator } from "../useDocumentViewerOrchestrator";

const mocks = vi.hoisted(() => ({
  resolveVisualizacionDocumento: vi.fn(),
  fetchFirmaElectronica: vi.fn(),
}));

vi.mock("../AppDocumentViewerOrchestrator.service", () => ({
  resolveVisualizacionDocumento: (params: unknown) => mocks.resolveVisualizacionDocumento(params),
  fetchFirmaElectronica: (params: unknown) => mocks.fetchFirmaElectronica(params),
}));

describe("useDocumentViewerOrchestrator", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("no PDF => no consulta firma y firmaCheckStatus=not_required", async () => {
    mocks.resolveVisualizacionDocumento.mockResolvedValueOnce({
      IdDocumento: 10,
      NombreGabinete: "G",
      FileName: "x.txt",
      ContentType: "text/plain",
      Origen: "ORIGINAL",
      UrlTemporal: "/tmp/x",
      UrlTemporalAbsoluta: null,
      ExpiresAt: "2026-01-01T00:00:00.000Z",
    });

    const { result } = renderHook(() => useDocumentViewerOrchestrator());

    await act(async () => {
      await result.current.visualizarDocumento({ documentId: 10, nombreGabinete: "G" });
    });

    expect(mocks.fetchFirmaElectronica).not.toHaveBeenCalled();
    expect(result.current.documentoActivo?.isPdf).toBe(false);
    expect(result.current.documentoActivo?.firmaCheckStatus).toBe("not_required");
    expect(result.current.documentoActivo?.fileUrl).toBe("/tmp/x");
  });

  it("PDF => consulta firma y llena isElectronicallySigned", async () => {
    mocks.resolveVisualizacionDocumento.mockResolvedValueOnce({
      IdDocumento: 11,
      NombreGabinete: "G",
      FileName: "x.pdf",
      ContentType: "application/pdf",
      Origen: "ORIGINAL",
      UrlTemporal: "/tmp/x",
      UrlTemporalAbsoluta: "https://cdn/x.pdf",
      ExpiresAt: "2026-01-01T00:00:00.000Z",
    });
    mocks.fetchFirmaElectronica.mockResolvedValueOnce({
      IdArchivo: 11,
      NombreGabinete: "G",
      FirmadoElectronico: true,
      IdCertificado: 1,
    });

    const { result } = renderHook(() => useDocumentViewerOrchestrator());

    await act(async () => {
      await result.current.visualizarDocumento({ documentId: 11, nombreGabinete: "G" });
    });

    expect(mocks.fetchFirmaElectronica).toHaveBeenCalledTimes(1);
    expect(result.current.documentoActivo?.isPdf).toBe(true);
    expect(result.current.documentoActivo?.fileUrl).toBe("https://cdn/x.pdf");
    expect(result.current.documentoActivo?.isElectronicallySigned).toBe(true);
  });

  it("si resolve falla, mantiene el documento previo visible", async () => {
    mocks.resolveVisualizacionDocumento
      .mockResolvedValueOnce({
        IdDocumento: 1,
        NombreGabinete: "G",
        FileName: "x.pdf",
        ContentType: "application/pdf",
        Origen: "ORIGINAL",
        UrlTemporal: "/tmp/1",
        UrlTemporalAbsoluta: "https://cdn/1.pdf",
        ExpiresAt: "2026-01-01T00:00:00.000Z",
      })
      .mockRejectedValueOnce(new Error("boom"));

    mocks.fetchFirmaElectronica.mockResolvedValueOnce({
      IdArchivo: 1,
      NombreGabinete: "G",
      FirmadoElectronico: false,
      IdCertificado: 1,
    });

    const { result } = renderHook(() => useDocumentViewerOrchestrator());

    await act(async () => {
      await result.current.visualizarDocumento({ documentId: 1, nombreGabinete: "G" });
    });

    const previousUrl = result.current.documentoActivo?.fileUrl;
    expect(previousUrl).toBe("https://cdn/1.pdf");

    await act(async () => {
      await result.current.visualizarDocumento({ documentId: 2, nombreGabinete: "G" });
    });

    expect(result.current.documentoActivo?.fileUrl).toBe(previousUrl);
    expect(result.current.documentoActivo?.resolveStatus).toBe("failed");
  });

  it("stale responses se ignoran (out-of-order)", async () => {
    let resolveA!: (value: unknown) => void;
    let resolveB!: (value: unknown) => void;
    const a = new Promise((r) => (resolveA = r));
    const b = new Promise((r) => (resolveB = r));

    mocks.resolveVisualizacionDocumento.mockImplementationOnce(() => a);
    mocks.resolveVisualizacionDocumento.mockImplementationOnce(() => b);

    const { result } = renderHook(() => useDocumentViewerOrchestrator());

    act(() => {
      void result.current.visualizarDocumento({ documentId: 1, nombreGabinete: "G" });
    });
    act(() => {
      void result.current.visualizarDocumento({ documentId: 2, nombreGabinete: "G" });
    });

    resolveB({
      IdDocumento: 2,
      NombreGabinete: "G",
      FileName: "b.pdf",
      ContentType: "application/pdf",
      Origen: "ORIGINAL",
      UrlTemporal: "/tmp/b",
      UrlTemporalAbsoluta: "https://cdn/b.pdf",
      ExpiresAt: "2026-01-01T00:00:00.000Z",
    });

    mocks.fetchFirmaElectronica.mockResolvedValueOnce({
      IdArchivo: 2,
      NombreGabinete: "G",
      FirmadoElectronico: false,
      IdCertificado: 1,
    });

    await waitFor(() => {
      expect(result.current.documentoActivo?.documentId).toBe(2);
      expect(result.current.documentoActivo?.fileUrl).toBe("https://cdn/b.pdf");
    });

    // Respuesta vieja llega tarde: debe ignorarse.
    resolveA({
      IdDocumento: 1,
      NombreGabinete: "G",
      FileName: "a.pdf",
      ContentType: "application/pdf",
      Origen: "ORIGINAL",
      UrlTemporal: "/tmp/a",
      UrlTemporalAbsoluta: "https://cdn/a.pdf",
      ExpiresAt: "2026-01-01T00:00:00.000Z",
    });

    await waitFor(() => {
      expect(result.current.documentoActivo?.documentId).toBe(2);
      expect(result.current.documentoActivo?.fileUrl).toBe("https://cdn/b.pdf");
    });
  });

  it("cancela la request anterior en visualizaciones consecutivas", async () => {
    let firstSignal: AbortSignal | undefined;

    // Dejar la primera promesa colgada para forzar cancelación cuando llega la segunda llamada.
    mocks.resolveVisualizacionDocumento.mockImplementationOnce((params: { signal?: AbortSignal }) => {
      firstSignal = params.signal;
      return new Promise(() => {});
    });

    mocks.resolveVisualizacionDocumento.mockResolvedValueOnce({
      IdDocumento: 2,
      NombreGabinete: "G",
      FileName: "b.pdf",
      ContentType: "application/pdf",
      Origen: "ORIGINAL",
      UrlTemporal: "/tmp/b",
      UrlTemporalAbsoluta: "https://cdn/b.pdf",
      ExpiresAt: "2026-01-01T00:00:00.000Z",
    });

    mocks.fetchFirmaElectronica.mockResolvedValueOnce({
      IdArchivo: 2,
      NombreGabinete: "G",
      FirmadoElectronico: false,
      IdCertificado: 1,
    });

    const { result } = renderHook(() => useDocumentViewerOrchestrator());

    act(() => {
      void result.current.visualizarDocumento({ documentId: 1, nombreGabinete: "G" });
    });

    await waitFor(() => {
      expect(firstSignal).toBeDefined();
    });

    act(() => {
      void result.current.visualizarDocumento({ documentId: 2, nombreGabinete: "G" });
    });

    await waitFor(() => {
      expect(firstSignal?.aborted).toBe(true);
      expect(result.current.documentoActivo?.documentId).toBe(2);
    });
  });
});
