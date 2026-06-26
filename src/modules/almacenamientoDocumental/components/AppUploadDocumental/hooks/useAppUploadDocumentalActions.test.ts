import { act, renderHook, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import type { UploadDocumentalFileItem } from "./useAppUploadDocumentalState";
import { useAppUploadDocumentalActions } from "./useAppUploadDocumentalActions";
import { uploadAndStoreOneDocument } from "../../../services/almacenamientoDocumentalUpload.service";

vi.mock("../../../services/almacenamientoDocumentalUpload.service", () => ({
  uploadAndStoreOneDocument: vi.fn(),
}));

const mockedUploadAndStoreOneDocument = vi.mocked(uploadAndStoreOneDocument);

function createItem(uid: string, name = `${uid}.pdf`): UploadDocumentalFileItem {
  const file = new File(["content"], name, { type: "application/pdf" });
  return {
    uid,
    file,
    name,
    size: file.size,
    extension: ".pdf",
    state: "ready",
    metadata: {
      idTipoDocumento: 1,
      nombreTipoDocumento: "Contrato",
      fechaCarga: "2026-01-10",
    },
  };
}

function createContext(signal = new AbortController().signal) {
  return {
    index: 0,
    total: 1,
    signal,
    setCurrentLabel: vi.fn(),
    setItemProgress: vi.fn(),
    setPhase: vi.fn(),
  };
}

describe("[SPEC:SCRUMCORE-271] useAppUploadDocumentalActions", () => {
  it("prepara lote y procesa items con un request final por archivo", async () => {
    const markFile = vi.fn();
    const onStored = vi.fn();
    const onBatchComplete = vi.fn();
    mockedUploadAndStoreOneDocument.mockResolvedValue({
      temporal: {
        rutaTemporalId: "ruta-1",
        archivoTemporalId: "archivo-1",
        chunkSizeBytes: 2,
        estado: "Completo",
      },
      response: {
        idAlmacen: 1,
        idRegistroProduccionDocumental: 2,
        nombreArchivoFinal: "final.pdf",
        requestId: "req-1",
      },
    });

    const files = [createItem("a", "a.pdf"), createItem("b", "b.pdf")];
    const { result } = renderHook(() =>
      useAppUploadDocumentalActions({
        files,
        config: {
          accept: ".pdf",
          allowedExtensions: [".pdf"],
          maxSizeBytes: 1000,
          multiple: true,
          requiereTipologia: true,
          requiereFechaCarga: true,
        },
        context: { nombreGabinete: "Gestion", idRutaWorkflow: 9 },
        proceso: "workflow",
        operationId: 1,
        validateFileForStore: vi.fn().mockReturnValue(null),
        markFile,
        onStored,
        onBatchComplete,
      }),
    );

    act(() => result.current.saveAll());
    expect(result.current.batchItems).toHaveLength(2);

    let firstResult: Awaited<ReturnType<typeof result.current.processBatchItem>> | undefined;
    let secondResult: Awaited<ReturnType<typeof result.current.processBatchItem>> | undefined;
    await act(async () => {
      firstResult = await result.current.processBatchItem(files[0], createContext());
      secondResult = await result.current.processBatchItem(files[1], createContext());
    });

    expect(firstResult).toEqual({ status: "success" });
    expect(secondResult).toEqual({ status: "success" });

    expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(2);
    expect(mockedUploadAndStoreOneDocument.mock.calls[0][0].request).toMatchObject({
      nombreGabinete: "Gestion",
      nombreDocumento: "a.pdf",
      trd: { idTipoDocumento: 1, nombreTipoDocumento: "Contrato" },
      workflow: { idRutaWorkflow: 9 },
    });
    expect(mockedUploadAndStoreOneDocument.mock.calls[1][0].request).toMatchObject({
      nombreDocumento: "b.pdf",
    });
    expect(onStored).toHaveBeenCalledTimes(2);

    act(() =>
      result.current.handleBatchComplete({
        total: 2,
        processed: 2,
        success: 2,
        warnings: 0,
        skipped: 0,
        controlledErrors: 0,
        fatalErrors: 0,
        cancelled: false,
      }),
    );

    await waitFor(() =>
      expect(onBatchComplete).toHaveBeenCalledWith(
        expect.objectContaining({ total: 2, stored: 2, failed: 0 }),
      ),
    );
  });

  it("marca cancelado cuando el AbortSignal esta abortado durante chunks", async () => {
    const markFile = vi.fn();
    const controller = new AbortController();
    controller.abort();
    mockedUploadAndStoreOneDocument.mockRejectedValue(new DOMException("aborted", "AbortError"));

    const item = createItem("a");
    const { result } = renderHook(() =>
      useAppUploadDocumentalActions({
        files: [item],
        config: {
          accept: ".pdf",
          allowedExtensions: [".pdf"],
          maxSizeBytes: 1000,
          multiple: true,
          requiereTipologia: true,
          requiereFechaCarga: false,
        },
        context: { nombreGabinete: "Gestion" },
        proceso: "radicacion",
        operationId: 1,
        validateFileForStore: vi.fn().mockReturnValue(null),
        markFile,
      }),
    );

    let processResult: Awaited<ReturnType<typeof result.current.processBatchItem>> | undefined;
    await act(async () => {
      processResult = await result.current.processBatchItem(item, createContext(controller.signal));
    });

    expect(processResult).toMatchObject({ status: "skipped" });
    expect(markFile).toHaveBeenCalledWith("a", expect.objectContaining({ state: "cancelled" }));
  });
});
