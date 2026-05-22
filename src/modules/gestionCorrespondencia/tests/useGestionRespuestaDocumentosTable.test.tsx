import { act, renderHook, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { useGestionRespuestaDocumentosTable } from "../hooks/useGestionRespuestaDocumentosTable";
import * as listaDocumentosService from "../services/listaDocumentosRadicados.service";

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
});

