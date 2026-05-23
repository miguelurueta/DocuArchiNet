import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useGestionRespuestaDocumentosTable } from "../hooks/useGestionRespuestaDocumentosTable";
import * as listaDocumentosService from "../services/listaDocumentosRadicados.service";
import * as gabineteService from "../services/solicitaGabineteRadicadoWorkflow.service";
import type { SolicitaGabineteRadicadoWorkflowResponse } from "../types/solicitaGabineteRadicadoWorkflow.types";
import type { ListaDocumentosRadicadosQueryRequest } from "../types/listaDocumentosRadicados.types";

vi.mock("../services/listaDocumentosRadicados.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/listaDocumentosRadicados.service")
  >("../services/listaDocumentosRadicados.service");

  return {
    ...actual,
    queryListaDocumentosRadicados: vi.fn(),
    actionListaDocumentosRadicados: vi.fn(),
    resolveDocumentoVisualizacion: vi.fn(),
  };
});

vi.mock("../services/solicitaGabineteRadicadoWorkflow.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/solicitaGabineteRadicadoWorkflow.service")
  >("../services/solicitaGabineteRadicadoWorkflow.service");
  return {
    ...actual,
    getSolicitaGabinetePorTareaWorkflow: vi.fn(),
  };
});

const buildRows = (ids: string[]) =>
  ids.map((id) => ({
    RowId: id,
    Values: {
      TIPODOCUMENTO: `DOC ${id}`,
      IdDocumento: Number(id.replace(/\D/g, "")) || 1,
    },
    Meta: {
      NodeType: "documento",
      HasChildren: false,
      DocumentId: Number(id.replace(/\D/g, "")) || 1,
      NombreGabinete: "WF_DOCS",
    },
  }));

const buildQueryResponse = (input: {
  ids: string[];
  total?: number;
  totalRecords?: number;
}) => {
  const data: Record<string, unknown> = {
    Rows: buildRows(input.ids),
  };

  if (typeof input.total === "number") {
    data.Total = input.total;
  }
  if (typeof input.totalRecords === "number") {
    data.TotalRecords = input.totalRecords;
  }

  return {
    success: true,
    message: "OK",
    data: data as unknown as import("../types/listaDocumentosRadicados.types").ListaDocumentosRadicadosQueryData,
  };
};

describe("useGestionRespuestaDocumentosTable [SCRUMCORE-224]", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(listaDocumentosService.resolveDocumentoVisualizacion).mockResolvedValue({
      success: true,
      message: "OK",
      data: { fileUrl: "http://example.test/doc.pdf" },
    });
    const gabineteOk: SolicitaGabineteRadicadoWorkflowResponse = {
      success: true,
      message: "OK",
      data: {
        NombreGabinete: "WF_DOCS",
        Radicado: "2025-0001",
        EstadoExistenciaRadicado: "YES",
      },
    };
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValue(gabineteOk);
  });

  it("prioriza Total backend cuando existe", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2"], total: 40 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(40);
    });
  });

  it("usa TotalRecords cuando Total no existe", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2"], totalRecords: 27 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(27);
    });
  });

  it("hace fallback a rows.length cuando backend no entrega total", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2", "r3"] }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(3);
    });
  });

  it("expone Documentos (0) cuando la lista esta vacia", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mockResolvedValueOnce(
      buildQueryResponse({ ids: [] }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(0);
    });
  });

  it("deriva selectedDocumentsCount desde la seleccion actual", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2", "r3"], total: 30 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    act(() => {
      result.current.onSelectionChanged(["r1", "r2"]);
    });

    expect(result.current.selectedDocumentsCount).toBe(2);
  });

  it("recalcula automaticamente el total y la seleccion con mutaciones runtime", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mockResolvedValueOnce(buildQueryResponse({ ids: ["r1", "r2"], total: 99 }))
      .mockResolvedValueOnce(buildQueryResponse({ ids: ["r1", "r2", "r3"], total: 99 }))
      .mockResolvedValueOnce(buildQueryResponse({ ids: ["r1"], total: 99 }));

    vi.mocked(listaDocumentosService.actionListaDocumentosRadicados).mockResolvedValue({
      success: true,
      message: "OK",
      data: {
        RequiresReloadNode: true,
      },
    });

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(99);
    });

    await act(async () => {
      await result.current.onActionTriggered({ actionId: "agregar_item", rowId: "r1" });
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(3);
    });

    act(() => {
      result.current.onSelectionChanged(["r1", "r2", "r3"]);
    });
    expect(result.current.selectedDocumentsCount).toBe(3);

    await act(async () => {
      await result.current.onActionTriggered({ actionId: "eliminar_item", rowId: "r1" });
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(1);
      expect(result.current.selectedDocumentsCount).toBe(1);
    });
  });

  it("no consulta query cuando el radicado es vacio", async () => {
    const gabineteSinRadicado: SolicitaGabineteRadicadoWorkflowResponse = {
      success: true,
      message: "OK",
      data: { NombreGabinete: "WF_DOCS", Radicado: "   ", EstadoExistenciaRadicado: "YES" },
    };
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValueOnce(gabineteSinRadicado);

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable(10));

    const response = await act(async () => result.current.load());

    expect(vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)).not.toHaveBeenCalled();
    expect(response.ok).toBe(false);
    expect(response.message).toMatch(/radicado.*obligatorio/i);
  });

  it("no consulta query cuando EstadoExistenciaRadicado es NO", async () => {
    const gabineteNoExiste: SolicitaGabineteRadicadoWorkflowResponse = {
      success: true,
      message: "OK",
      data: { NombreGabinete: "WF_DOCS", Radicado: "2025-0001", EstadoExistenciaRadicado: "NO" },
    };
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValueOnce(gabineteNoExiste);

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable(10));

    const response = await act(async () => result.current.load());

    expect(vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)).not.toHaveBeenCalled();
    expect(response.ok).toBe(false);
    expect(response.message).toMatch(/no existe/i);
  });

  it("consulta query cuando radicado es valido y lo incluye en el request", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2"], total: 40 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable(10));

    await act(async () => {
      await result.current.load();
    });

    expect(vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)).toHaveBeenCalledTimes(1);
    const req = vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mock
      .calls[0]?.[0] as ListaDocumentosRadicadosQueryRequest;
    expect(req?.CampoRadicado).toBe("ENLASE");
    expect(req?.Radicado).toBe("2025-0001");
  });

  it("ignora respuestas stale cuando cambia idTareaWf (anti-stale)", async () => {
    const deferred = <T,>() => {
      let resolve!: (value: T) => void;
      let reject!: (reason?: unknown) => void;
      const promise = new Promise<T>((res, rej) => {
        resolve = res;
        reject = rej;
      });
      return { promise, resolve, reject };
    };

    const slow = deferred<import("../types/listaDocumentosRadicados.types").ApiResponse<
      import("../types/listaDocumentosRadicados.types").ListaDocumentosRadicadosQueryData
    >>();

    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow)
      .mockResolvedValueOnce({
        success: true,
        message: "OK",
        data: { NombreGabinete: "WF_DOCS", Radicado: "2025-0001", EstadoExistenciaRadicado: "YES" },
      })
      .mockResolvedValueOnce({
        success: true,
        message: "OK",
        data: { NombreGabinete: "WF_DOCS", Radicado: "2025-0002", EstadoExistenciaRadicado: "YES" },
      });

    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mockImplementationOnce(async () => slow.promise)
      .mockResolvedValueOnce(buildQueryResponse({ ids: ["b1"], total: 1 }));

    const { result, rerender } = renderHook(
      ({ id }) => useGestionRespuestaDocumentosTable(id),
      { initialProps: { id: 1 } },
    );

    // start load for A (slow) without awaiting (simulates in-flight request)
    const loadAPromise = result.current.load();

    // switch to B and load (fast)
    rerender({ id: 2 });
    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(1);
    });

    // Resolve A late (should not overwrite)
    slow.resolve(buildQueryResponse({ ids: ["a1", "a2"], total: 2 }));
    await act(async () => {
      await loadAPromise;
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(1);
    });
  });
});

