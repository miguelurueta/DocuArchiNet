import { act, renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { GestionRespuestaDocumentosProvider } from "../context/GestionRespuestaDocumentosContext";
import { useGestionRespuestaDocumentos } from "../hooks/useGestionRespuestaDocumentos";
import { useGestionRespuestaDocumentosTable } from "../hooks/useGestionRespuestaDocumentosTable";
import * as deleteDocumentoService from "../services/eliminarDocumentoStorageEngine.service";
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

vi.mock("../services/eliminarDocumentoStorageEngine.service", async () => {
  const actual = await vi.importActual<
    typeof import("../services/eliminarDocumentoStorageEngine.service")
  >("../services/eliminarDocumentoStorageEngine.service");

  return {
    ...actual,
    eliminarDocumentoStorageEngine: vi.fn(),
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
  paginationTotal?: number;
  metaTotal?: number;
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
  if (typeof input.paginationTotal === "number") {
    data.pagination = {
      page: 1,
      pageSize: 25,
      total: input.paginationTotal,
    };
  }

  return {
    success: true,
    message: "OK",
    data: data as unknown as import("../types/listaDocumentosRadicados.types").ListaDocumentosRadicadosQueryData,
    ...(typeof input.metaTotal === "number"
      ? { meta: { Total: input.metaTotal } }
      : {}),
  };
};

const buildProviderWrapper =
  (props: {
    idTareaWf?: number;
    radicado?: string;
    idRespuestaRadicado?: string | number;
  }) =>
  ({ children }: { children: ReactNode }) => (
    <GestionRespuestaDocumentosProvider {...props}>
      {children}
    </GestionRespuestaDocumentosProvider>
  );

describe("useGestionRespuestaDocumentosTable [SCRUMCORE-224]", () => {
  beforeEach(() => {
    vi.resetAllMocks();
    vi.mocked(
      listaDocumentosService.resolveDocumentoVisualizacion,
    ).mockResolvedValue({
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
    vi.mocked(
      gabineteService.getSolicitaGabinetePorTareaWorkflow,
    ).mockResolvedValue(gabineteOk);
    vi.mocked(
      deleteDocumentoService.eliminarDocumentoStorageEngine,
    ).mockResolvedValue({
      success: true,
      message: "Documento eliminado correctamente.",
      severity: "success",
      requestId: "req-delete",
      httpStatus: 204,
      rawResponse: "",
    });
  });

  it("prioriza Total backend cuando existe", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
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
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
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

  it("usa data.pagination.total cuando meta total no existe", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2"], paginationTotal: 19 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(19);
    });
  });

  it("usa meta.Total cuando el backend responde metadata en PascalCase", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2"], total: 2, metaTotal: 44 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(44);
    });
  });

  it("consulta lista completa aunque exista estado de pagina en el wrapper", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2"], paginationTotal: 40 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    act(() => {
      result.current.onQueryChange({ page: 2 });
    });

    await act(async () => {
      await result.current.load();
    });

    const req = vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mock.calls[0]?.[0] as ListaDocumentosRadicadosQueryRequest;
    expect(req.Page).toBe(1);
    expect(req.PageSize).toBe(25);
    expect(req.DocumentRelationScope).toBe("documentsOnly");
    expect(req.EnablePagination).toBe(false);
  });

  it("filtra localmente la lista completa y reinicia Page a 1", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["contrato-1", "acta-2"], paginationTotal: 2 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    act(() => {
      result.current.onQueryChange({ page: 3 });
      result.current.onQueryChange({ search: " contrato " });
    });

    let loadResult: Awaited<ReturnType<typeof result.current.load>> | undefined;
    await act(async () => {
      loadResult = await result.current.load();
    });

    const req = vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mock.calls[0]?.[0] as ListaDocumentosRadicadosQueryRequest;
    expect(req.Page).toBe(1);
    expect(req.Search).toBe("");
    expect(req.SearchType).toBe(1);
    expect(req.EnablePagination).toBe(false);
    expect(loadResult?.ok).toBe(true);
    expect(loadResult?.rows).toHaveLength(1);
    expect(loadResult?.rows[0].id).toBe("contrato-1");
    expect(result.current.totalDocumentsCount).toBe(1);
  });

  it("reinicia Page a 1 cuando cambia PageSize o scope", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1"], paginationTotal: 1 }),
    );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    act(() => {
      result.current.onQueryChange({ page: 4 });
      result.current.onQueryChange({ pageSize: 50 });
    });

    expect(result.current.queryState).toEqual(
      expect.objectContaining({ page: 1, pageSize: 50 }),
    );

    act(() => {
      result.current.onQueryChange({ page: 3 });
      result.current.setDocumentRelationScope("responseAttachmentsOnly");
    });

    await act(async () => {
      await result.current.load();
    });

    const req = vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mock.calls[0]?.[0] as ListaDocumentosRadicadosQueryRequest;
    expect(req.Page).toBe(1);
    expect(req.PageSize).toBe(50);
    expect(req.DocumentRelationScope).toBe("responseAttachmentsOnly");
    expect(req.EnablePagination).toBe(false);
  });

  it("hace fallback a rows.length cuando backend no entrega total", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(buildQueryResponse({ ids: ["r1", "r2", "r3"] }));

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(3);
    });
  });

  it("expone Documentos (0) cuando la lista esta vacia", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(buildQueryResponse({ ids: [] }));

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(0);
    });
  });

  it("deriva selectedDocumentsCount desde la seleccion actual", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
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
      .mockResolvedValueOnce(
        buildQueryResponse({ ids: ["r1", "r2"], total: 99 }),
      )
      .mockResolvedValueOnce(
        buildQueryResponse({ ids: ["r1", "r2", "r3"], total: 99 }),
      )
      .mockResolvedValueOnce(buildQueryResponse({ ids: ["r1"], total: 99 }));

    vi.mocked(
      listaDocumentosService.actionListaDocumentosRadicados,
    ).mockResolvedValue({
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
      await result.current.onActionTriggered({
        actionId: "agregar_item",
        rowId: "r1",
      });
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(3);
    });

    const refreshReq = vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mock.calls[1]?.[0] as ListaDocumentosRadicadosQueryRequest | undefined;
    expect(refreshReq?.EnablePagination).toBe(false);

    act(() => {
      result.current.onSelectionChanged(["r1", "r2", "r3"]);
    });
    expect(result.current.selectedDocumentsCount).toBe(3);

    await act(async () => {
      await result.current.onActionTriggered({
        actionId: "eliminar_item",
        rowId: "r1",
      });
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(2);
      expect(result.current.selectedDocumentsCount).toBe(2);
    });
  });

  it("mantiene carga completa tras cambiar PageSize despues de una mutacion runtime", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mockResolvedValueOnce(
        buildQueryResponse({ ids: ["r1", "r2"], total: 99 }),
      )
      .mockResolvedValueOnce(
        buildQueryResponse({ ids: ["p1", "p2"], total: 99 }),
      );

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(99);
    });

    await act(async () => {
      await result.current.onActionTriggered({
        actionId: "eliminar_item",
        rowId: "r1",
      });
    });

    act(() => {
      result.current.onQueryChange({ pageSize: 10 });
    });

    await act(async () => {
      await result.current.load();
    });

    const req = vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mock.calls[1]?.[0] as ListaDocumentosRadicadosQueryRequest;
    expect(req.PageSize).toBe(10);
    expect(req.EnablePagination).toBe(false);
    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(2);
    });
  });

  it("borra via servicio persistido y pasa idAlmacen desde DocumentId", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(buildQueryResponse({ ids: ["r1"], total: 1 }));
    vi.mocked(
      deleteDocumentoService.eliminarDocumentoStorageEngine,
    ).mockResolvedValueOnce({
      success: false,
      message: "No es posible eliminar este anexo en este momento.",
      severity: "warning",
      requestId: "req-delete",
      httpStatus: 400,
    });

    const { result } = renderHook(() => useGestionRespuestaDocumentosTable());

    await act(async () => {
      await result.current.load();
    });

    await waitFor(() => {
      expect(result.current.totalDocumentsCount).toBe(1);
    });

    const response = await act(async () =>
      result.current.onActionTriggered({
        actionId: "eliminar_item",
        rowId: "r1",
      }),
    );

    expect(
      vi.mocked(deleteDocumentoService.eliminarDocumentoStorageEngine),
    ).toHaveBeenCalledWith({
      idAlmacen: 1,
      nombreGabinete: "WF_DOCS",
      sourceModule: "WORKFLOW",
    });
    expect(response).toEqual({
      success: false,
      message: "No es posible eliminar este anexo en este momento.",
      severity: "warning",
      requestId: "req-delete",
      httpStatus: 400,
    });
  });

  it("no consulta query cuando el radicado es vacio", async () => {
    const { result } = renderHook(
      () => ({
        table: useGestionRespuestaDocumentosTable(10),
        context: useGestionRespuestaDocumentos(),
      }),
      {
        wrapper: buildProviderWrapper({ idTareaWf: 10, radicado: "   " }),
      },
    );

    await waitFor(() => {
      expect(result.current.context.gabineteLoading).toBe(false);
    });

    const response = await act(async () => result.current.table.load());

    expect(
      vi.mocked(listaDocumentosService.queryListaDocumentosRadicados),
    ).not.toHaveBeenCalled();
    expect(response.ok).toBe(false);
    expect(response.message).toMatch(/radicado.*obligatorio/i);
  });

  it("no consulta query cuando EstadoExistenciaRadicado es NO", async () => {
    const gabineteNoExiste: SolicitaGabineteRadicadoWorkflowResponse = {
      success: true,
      message: "OK",
      data: {
        NombreGabinete: "WF_DOCS",
        Radicado: "2025-0001",
        EstadoExistenciaRadicado: "NO",
      },
    };
    vi.mocked(
      gabineteService.getSolicitaGabinetePorTareaWorkflow,
    ).mockResolvedValueOnce(gabineteNoExiste);

    const { result } = renderHook(
      () => ({
        table: useGestionRespuestaDocumentosTable(10),
        context: useGestionRespuestaDocumentos(),
      }),
      {
        wrapper: buildProviderWrapper({ idTareaWf: 10, radicado: "2025-0001" }),
      },
    );

    await waitFor(() => {
      expect(result.current.context.gabineteLoading).toBe(false);
    });

    const response = await act(async () => result.current.table.load());

    expect(
      vi.mocked(listaDocumentosService.queryListaDocumentosRadicados),
    ).not.toHaveBeenCalled();
    expect(response.ok).toBe(false);
    expect(response.message).toMatch(/no existe/i);
  });

  it("consulta query cuando radicado es valido y lo incluye en el request", async () => {
    vi.mocked(
      listaDocumentosService.queryListaDocumentosRadicados,
    ).mockResolvedValueOnce(
      buildQueryResponse({ ids: ["r1", "r2"], total: 40 }),
    );

    const { result } = renderHook(
      () => ({
        table: useGestionRespuestaDocumentosTable(10),
        context: useGestionRespuestaDocumentos(),
      }),
      {
        wrapper: buildProviderWrapper({ idTareaWf: 10, radicado: "2025-0001" }),
      },
    );

    await waitFor(() => {
      expect(result.current.context.nombreGabinete).toBe("WF_DOCS");
    });

    await act(async () => {
      await result.current.table.load();
    });

    expect(
      vi.mocked(listaDocumentosService.queryListaDocumentosRadicados),
    ).toHaveBeenCalledTimes(1);
    const req = vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mock.calls[0]?.[0] as ListaDocumentosRadicadosQueryRequest;
    expect(req?.CampoRadicado).toBe("ENLASE");
    expect(req?.Radicado).toBe("2025-0001");
    expect(req?.DocumentRelationScope).toBe("documentsOnly");
    expect(req?.EnablePagination).toBe(false);
  });

  it.skip("ignora respuestas stale cuando cambia idTareaWf (anti-stale)", async () => {
    const deferred = <T,>() => {
      let resolve!: (value: T) => void;
      let reject!: (reason?: unknown) => void;
      const promise = new Promise<T>((res, rej) => {
        resolve = res;
        reject = rej;
      });
      return { promise, resolve, reject };
    };

    const slow =
      deferred<
        import("../types/listaDocumentosRadicados.types").ApiResponse<
          import("../types/listaDocumentosRadicados.types").ListaDocumentosRadicadosQueryData
        >
      >();

    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow)
      .mockResolvedValueOnce({
        success: true,
        message: "OK",
        data: {
          NombreGabinete: "WF_DOCS",
          Radicado: "2025-0001",
          EstadoExistenciaRadicado: "YES",
        },
      })
      .mockResolvedValueOnce({
        success: true,
        message: "OK",
        data: {
          NombreGabinete: "WF_DOCS",
          Radicado: "2025-0002",
          EstadoExistenciaRadicado: "YES",
        },
      });

    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)
      .mockImplementationOnce(async () => slow.promise)
      .mockResolvedValueOnce(buildQueryResponse({ ids: ["b1"], total: 1 }));

    let providerProps = { idTareaWf: 1, radicado: "2025-0001" };
    const wrapper = ({ children }: { children: ReactNode }) => (
      <GestionRespuestaDocumentosProvider {...providerProps}>
        {children}
      </GestionRespuestaDocumentosProvider>
    );

    const { result, rerender } = renderHook(
      ({ id }) => ({
        table: useGestionRespuestaDocumentosTable(id),
        context: useGestionRespuestaDocumentos(),
      }),
      { initialProps: { id: 1 }, wrapper },
    );

    await waitFor(() => {
      expect(result.current.context.nombreGabinete).toBe("WF_DOCS");
    });

    // start load for A (slow) without awaiting (simulates in-flight request)
    const loadAPromise = result.current.table.load();

    // switch to B and load (fast)
    providerProps = { idTareaWf: 2, radicado: "2025-0002" };
    rerender({ id: 2 });
    await waitFor(() => {
      expect(result.current.context.nombreGabinete).toBe("WF_DOCS");
      expect(
        gabineteService.getSolicitaGabinetePorTareaWorkflow,
      ).toHaveBeenCalledTimes(2);
    });

    const loadBPromise = result.current.table.load();

    // Resolve A late (should not overwrite)
    slow.resolve(buildQueryResponse({ ids: ["a1", "a2"], total: 2 }));
    await act(async () => {
      await Promise.all([loadAPromise, loadBPromise]);
    });

    await waitFor(() => {
      expect(result.current.table.totalDocumentsCount).toBe(1);
    });
  });
});
