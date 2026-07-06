import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import type { UploadDocumentalFileItem } from "./useAppUploadDocumentalState";
import { useAppUploadDocumentalActions } from "./useAppUploadDocumentalActions";
import { uploadAndStoreOneDocument } from "../../../services/almacenamientoDocumentalUpload.service";

vi.mock("../../../services/almacenamientoDocumentalUpload.service", () => ({
  uploadAndStoreOneDocument: vi.fn(),
}));

const mockedUploadAndStoreOneDocument = vi.mocked(uploadAndStoreOneDocument);

function createItem(
  uid: string,
  name = `${uid}.pdf`,
  overrides: Partial<UploadDocumentalFileItem> = {},
): UploadDocumentalFileItem {
  const file = new File(["content"], name, { type: "application/pdf" });
  return {
    uid,
    file: overrides.file ?? file,
    name: overrides.name ?? name,
    size: file.size,
    extension: ".pdf",
    state: "ready",
    metadata: {
      idTipoDocumento: 1,
      nombreTipoDocumento: "Contrato",
      fechaCarga: "2026-01-10",
    },
    ...overrides,
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
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("prepara lote y procesa items con un request final por archivo", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
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
        setFiles,
        onStored,
        onBatchComplete,
      }),
    );

    await act(async () => {
      await result.current.saveAll();
    });
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
    expect(onStored).toHaveBeenNthCalledWith(
      1,
      expect.objectContaining({ fileUid: "a" }),
      { source: "batch", remainingFiles: 1 },
    );

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
    const setFiles = vi.fn();
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
        setFiles,
      }),
    );

    let processResult: Awaited<ReturnType<typeof result.current.processBatchItem>> | undefined;
    await act(async () => {
      processResult = await result.current.processBatchItem(item, createContext(controller.signal));
    });

    expect(processResult).toMatchObject({ status: "skipped" });
    expect(markFile).toHaveBeenCalledWith("a", expect.objectContaining({ state: "cancelled" }));
  });

  it("cancela un archivo activo desde cancelFile", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onError = vi.fn();
    mockedUploadAndStoreOneDocument.mockImplementationOnce(
      (input) =>
        new Promise((_, reject) => {
          input.signal?.addEventListener("abort", () => reject(new DOMException("aborted", "AbortError")));
        }),
    );

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
        setFiles,
        onError,
      }),
    );

    let savePromise!: Promise<void>;
    await act(async () => {
      savePromise = result.current.saveOne("a");
    });
    await waitFor(() => expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1));

    await act(async () => {
      result.current.cancelFile("a");
      await savePromise;
    });

    expect(markFile).toHaveBeenCalledWith("a", expect.objectContaining({ state: "cancelled" }));
    expect(onError).not.toHaveBeenCalled();
  });

  it("ignora un segundo guardar archivo mientras el primero sigue activo", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    let resolveStore!: (value: Awaited<ReturnType<typeof uploadAndStoreOneDocument>>) => void;
    mockedUploadAndStoreOneDocument.mockImplementationOnce(
      () =>
        new Promise((resolve) => {
          resolveStore = resolve;
        }),
    );

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
        setFiles,
      }),
    );

    let firstSave!: Promise<void>;
    await act(async () => {
      firstSave = result.current.saveOne("a");
      await result.current.saveOne("a");
    });

    expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1);

    await act(async () => {
      resolveStore({
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
      await firstSave;
    });
  });

  it("cancela la carga global inline y no procesa archivos pendientes", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onBatchComplete = vi.fn();
    const onError = vi.fn();
    const first = createItem("a", "a.pdf");
    const second = createItem("b", "b.pdf");
    mockedUploadAndStoreOneDocument.mockImplementationOnce(
      (input) =>
        new Promise((_, reject) => {
          input.signal?.addEventListener("abort", () => reject(new DOMException("aborted", "AbortError")));
        }),
    );

    const { result } = renderHook(() =>
      useAppUploadDocumentalActions({
        files: [first, second],
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
        setFiles,
        saveAllMode: "inline",
        onBatchComplete,
        onError,
      }),
    );

    let savePromise!: Promise<void>;
    await act(async () => {
      savePromise = result.current.saveAll();
    });
    await waitFor(() => expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1));

    await act(async () => {
      result.current.cancelAll();
      await savePromise;
    });

    expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1);
    expect(markFile).toHaveBeenCalledWith("a", expect.objectContaining({ state: "cancelled" }));
    expect(onBatchComplete).toHaveBeenCalledWith(
      expect.objectContaining({ stored: 0, skipped: 2, cancelled: 1 }),
    );
    expect(onError).not.toHaveBeenCalled();
  });

  it("permite reintentar con guardar todo cuando solo queda un archivo cancelado", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onBatchComplete = vi.fn();
    const item = createItem("a", "grande.pdf", { state: "cancelled" });
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
        setFiles,
        saveAllMode: "inline",
        onBatchComplete,
      }),
    );

    expect(result.current.canSaveAll).toBe(true);

    await act(async () => {
      await result.current.saveAll();
    });

    expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1);
    expect(onBatchComplete).toHaveBeenCalledWith(
      expect.objectContaining({ stored: 1, failed: 0, remainingFiles: 0 }),
    );
  });

  it("no abre el lote ni llama backend si algun archivo falla validacion previa", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onError = vi.fn();
    const item = createItem("a");
    const validationError = "No se puede guardar: selecciona la tipologia documental del archivo.";
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
        validateFileForStore: vi.fn().mockReturnValue(validationError),
        markFile,
        setFiles,
        onError,
      }),
    );

    await act(async () => {
      await result.current.saveAll();
    });

    expect(result.current.batchOpen).toBe(false);
    expect(result.current.batchItems).toEqual([]);
    expect(mockedUploadAndStoreOneDocument).not.toHaveBeenCalled();
    expect(markFile).toHaveBeenCalledWith(
      "a",
      expect.objectContaining({
        state: "error",
        error: undefined,
        metadata: { error: validationError },
      }),
    );
    expect(onError).not.toHaveBeenCalled();
  });

  it("procesa archivos validos y conserva en cola los que fallan tipologia en guardar todo", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onBatchComplete = vi.fn();
    const validationError = "No se puede guardar: selecciona la tipologia documental del archivo.";
    const valid = createItem("a", "a.pdf");
    const invalid = createItem("b", "b.pdf", { metadata: {} });
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

    const { result } = renderHook(() =>
      useAppUploadDocumentalActions({
        files: [valid, invalid],
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
        validateFileForStore: vi.fn((uid: string) => (uid === "b" ? validationError : null)),
        markFile,
        setFiles,
        onBatchComplete,
      }),
    );

    await act(async () => {
      await result.current.saveAll();
    });

    expect(result.current.batchOpen).toBe(true);
    expect(result.current.batchItems).toEqual([valid]);
    expect(markFile).toHaveBeenCalledWith(
      "b",
      expect.objectContaining({
        state: "error",
        error: undefined,
        metadata: { error: validationError },
      }),
    );

    await act(async () => {
      await result.current.processBatchItem(valid, createContext());
    });

    act(() =>
      result.current.handleBatchComplete({
        total: 1,
        processed: 1,
        success: 1,
        warnings: 0,
        skipped: 0,
        controlledErrors: 0,
        fatalErrors: 0,
        cancelled: false,
      }),
    );

    await waitFor(() =>
      expect(onBatchComplete).toHaveBeenCalledWith(
        expect.objectContaining({ total: 2, stored: 1, failed: 1 }),
      ),
    );
    expect(setFiles).toHaveBeenCalledWith(expect.any(Function));
    const cleanup = setFiles.mock.calls.at(-1)?.[0] as (current: UploadDocumentalFileItem[]) => UploadDocumentalFileItem[];
    expect(cleanup([valid, invalid])).toEqual([invalid]);
  });

  it("guarda todo inline sin abrir modal de progreso cuando saveAllMode es inline", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onBatchComplete = vi.fn();
    const valid = createItem("a", "a.pdf");
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

    const { result } = renderHook(() =>
      useAppUploadDocumentalActions({
        files: [valid],
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
        setFiles,
        saveAllMode: "inline",
        onBatchComplete,
      }),
    );

    await act(async () => {
      await result.current.saveAll();
    });

    expect(result.current.batchOpen).toBe(false);
    expect(result.current.batchItems).toEqual([]);
    expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1);
    expect(onBatchComplete).toHaveBeenCalledWith(
      expect.objectContaining({ total: 1, stored: 1, failed: 0 }),
    );
    expect(setFiles).toHaveBeenCalledWith(expect.any(Function));
  });

  it("mantiene el lote como parcial si aun existe un archivo en error sin tipologia", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onBatchComplete = vi.fn();
    const validationError = "No se puede guardar: selecciona la tipologia documental del archivo.";
    const valid = createItem("a", "nuevo.pdf");
    const pending = {
      ...createItem("b", "pendiente.pdf"),
      state: "error" as const,
      error: undefined,
      metadata: { error: validationError },
    };
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

    const { result } = renderHook(() =>
      useAppUploadDocumentalActions({
        files: [valid, pending],
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
        validateFileForStore: vi.fn((uid: string) => (uid === "b" ? validationError : null)),
        markFile,
        setFiles,
        saveAllMode: "inline",
        onBatchComplete,
      }),
    );

    await act(async () => {
      await result.current.saveAll();
    });

    expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1);
    expect(onBatchComplete).toHaveBeenCalledWith(
      expect.objectContaining({ total: 2, stored: 1, failed: 1 }),
    );
    expect(markFile).toHaveBeenCalledWith(
      "b",
      expect.objectContaining({
        state: "error",
        error: undefined,
        metadata: { error: validationError },
      }),
    );
  });

  it("no notifica error global cuando guardar un archivo falla por validacion local", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const onError = vi.fn();
    const item = createItem("a");
    const validationError = "No se puede guardar: selecciona la tipologia documental del archivo.";
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
        validateFileForStore: vi.fn().mockReturnValue(validationError),
        markFile,
        setFiles,
        onError,
      }),
    );

    await act(async () => {
      await result.current.saveOne("a");
    });

    expect(mockedUploadAndStoreOneDocument).not.toHaveBeenCalled();
    expect(markFile).toHaveBeenCalledWith(
      "a",
      expect.objectContaining({
        state: "error",
        error: undefined,
        metadata: { error: validationError },
      }),
    );
    expect(onError).not.toHaveBeenCalled();
  });

  it("remueve de la cola el archivo guardado individualmente y conserva los pendientes", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const storedItem = createItem("a", "guardado.pdf");
    const pendingItem = {
      ...createItem("b", "pendiente.pdf"),
      state: "error" as const,
      metadata: { error: "No se puede guardar: selecciona la tipologia documental del archivo." },
    };
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

    const { result } = renderHook(() =>
      useAppUploadDocumentalActions({
        files: [storedItem, pendingItem],
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
        setFiles,
      }),
    );

    await act(async () => {
      await result.current.saveOne("a");
    });

    expect(setFiles).toHaveBeenCalledWith(expect.any(Function));
    const cleanup = setFiles.mock.calls.at(-1)?.[0] as (current: UploadDocumentalFileItem[]) => UploadDocumentalFileItem[];
    expect(cleanup([storedItem, pendingItem])).toEqual([pendingItem]);
  });

  it("mantiene el progreso alineado con completar, almacenar y actualizar documentos", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const item = createItem("a");
    mockedUploadAndStoreOneDocument.mockImplementationOnce(async (input) => {
      input.onProgress?.({
        fileUid: "a",
        phase: "uploading",
        percent: 100,
      });
      input.onProgress?.({
        fileUid: "a",
        phase: "completing",
        percent: 100,
      });
      input.onProgress?.({
        fileUid: "a",
        phase: "storing",
        percent: 100,
      });

      return {
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
      };
    });

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
        setFiles,
      }),
    );

    const progressContext = createContext();
    await act(async () => {
      await result.current.processBatchItem(item, progressContext);
    });

    expect(progressContext.setItemProgress).toHaveBeenCalledWith(82);
    expect(progressContext.setItemProgress).toHaveBeenCalledWith(92);
    expect(progressContext.setItemProgress).toHaveBeenCalledWith(98);
    expect(progressContext.setItemProgress).toHaveBeenCalledWith(99);
    expect(progressContext.setPhase).toHaveBeenCalledWith("Actualizando documentos");
    expect(progressContext.setPhase).toHaveBeenCalledWith("Guardado");
  });

  it("muestra mensaje funcional cuando falla el registro final", async () => {
    const markFile = vi.fn();
    const setFiles = vi.fn();
    const item = createItem("a");
    mockedUploadAndStoreOneDocument.mockRejectedValue({
      code: "storage_store_error",
      message: "request field is required",
    });

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
        setFiles,
      }),
    );

    let processResult: Awaited<ReturnType<typeof result.current.processBatchItem>> | undefined;
    await act(async () => {
      processResult = await result.current.processBatchItem(item, createContext());
    });

    expect(processResult).toMatchObject({
      status: "controlled-error",
      message:
        "El archivo se cargo, pero no fue posible registrar el documento. Revisa los datos e intenta nuevamente.",
    });
    expect(markFile).toHaveBeenCalledWith(
      "a",
      expect.objectContaining({
        state: "error",
        error:
          "El archivo se cargo, pero no fue posible registrar el documento. Revisa los datos e intenta nuevamente.",
        metadata: {
          error:
            "El archivo se cargo, pero no fue posible registrar el documento. Revisa los datos e intenta nuevamente.",
        },
      }),
    );
  });
});
