import { act, renderHook, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { GestionRespuestaDocumentosProvider } from "../context/GestionRespuestaDocumentosContext";
import { useGestionRespuestaDocumentos } from "../hooks/useGestionRespuestaDocumentos";
import { useListaDocumentosRadicadosTreeTable } from "../hooks/useListaDocumentosRadicadosTreeTable";
import * as listaDocumentosService from "../services/listaDocumentosRadicados.service";
import * as gabineteService from "../services/solicitaGabineteRadicadoWorkflow.service";
import type { SolicitaGabineteRadicadoWorkflowResponse } from "../types/solicitaGabineteRadicadoWorkflow.types";
import type {
  ApiResponse,
  ListaDocumentosRadicadosQueryData,
  ListaDocumentosRadicadosRowDto,
} from "../types/listaDocumentosRadicados.types";

vi.mock("../services/listaDocumentosRadicados.service", () => {
  return {
    queryListaDocumentosRadicados: vi.fn(),
    actionListaDocumentosRadicados: vi.fn(),
    resolveDocumentoVisualizacion: vi.fn(),
  };
});

vi.mock("../services/solicitaGabineteRadicadoWorkflow.service", () => {
  return {
    getSolicitaGabinetePorTareaWorkflow: vi.fn(),
  };
});

const buildProviderWrapper =
  (props: { idTareaWf?: number; radicado?: string }) =>
  ({ children }: { children: ReactNode }) => (
    <GestionRespuestaDocumentosProvider {...props}>{children}</GestionRespuestaDocumentosProvider>
  );

const buildRows = (): ListaDocumentosRadicadosRowDto[] => [
  {
    RowId: "r1",
    Meta: {
      NodeType: "documento",
      DocumentId: 111,
      HasChildren: false,
    },
    Values: {
      Documento: "DOC-111",
    },
  },
];

const buildQueryResponse = (rows: ListaDocumentosRadicadosRowDto[]): ApiResponse<ListaDocumentosRadicadosQueryData> => ({
  success: true,
  message: "OK",
  data: { Rows: rows },
});

const buildGabineteResponse = (
  overrides: Partial<SolicitaGabineteRadicadoWorkflowResponse> = {},
): SolicitaGabineteRadicadoWorkflowResponse => ({
  success: true,
  message: "OK",
  data: {
    NombreGabinete: "WF_DOCS",
    Radicado: "2025-0001",
    EstadoExistenciaRadicado: "YES",
  },
  ...overrides,
});

describe("[SPEC:SCRUMCORE-221] useListaDocumentosRadicadosTreeTable", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValue(
      buildGabineteResponse(),
    );
  });

  it("consume nombreGabinete desde contexto transversal y lo usa en query y accion", async () => {
    vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mockResolvedValue(
      buildQueryResponse(buildRows()),
    );
    vi.mocked(listaDocumentosService.actionListaDocumentosRadicados).mockResolvedValue({
      success: true,
      message: "OK",
    });

    const { result } = renderHook(
      () => ({
        table: useListaDocumentosRadicadosTreeTable(10),
        context: useGestionRespuestaDocumentos(),
      }),
      {
        wrapper: buildProviderWrapper({ idTareaWf: 10, radicado: "2025-0001" }),
      },
    );

    await waitFor(() => {
      expect(result.current.context.nombreGabinete).toBe("WF_DOCS");
      expect(result.current.context.gabineteLoading).toBe(false);
    });

    await act(async () => {
      await result.current.table.load();
    });

    expect(vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)).toHaveBeenCalledTimes(1);
    expect(
      vi.mocked(listaDocumentosService.queryListaDocumentosRadicados).mock.calls[0]?.[0].NombreGabinete,
    ).toBe("WF_DOCS");

    await act(async () => {
      await result.current.table.onSelectRow("r1");
    });

    expect(vi.mocked(listaDocumentosService.actionListaDocumentosRadicados)).toHaveBeenCalledWith(
      expect.objectContaining({
        Payload: expect.objectContaining({ NombreGabinete: "WF_DOCS", IdDocumento: 111 }),
      }),
    );
  });

  it("no ejecuta query mientras el contexto reporta carga de gabinete", async () => {
    const deferred = <T,>() => {
      let resolve!: (value: T) => void;
      const promise = new Promise<T>((res) => {
        resolve = res;
      });
      return { promise, resolve };
    };
    const gabineteLoading = deferred<SolicitaGabineteRadicadoWorkflowResponse>();
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockImplementationOnce(
      async () => gabineteLoading.promise,
    );

    const { result } = renderHook(
      () => ({
        table: useListaDocumentosRadicadosTreeTable(10),
        context: useGestionRespuestaDocumentos(),
      }),
      {
        wrapper: buildProviderWrapper({ idTareaWf: 10, radicado: "2025-0001" }),
      },
    );

    await waitFor(() => {
      expect(result.current.context.gabineteLoading).toBe(true);
    });

    const response = await act(async () => result.current.table.load());

    expect(response.ok).toBe(false);
    expect(response.message).toBe("Cargando informacion del gabinete. Intenta nuevamente.");
    expect(vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)).not.toHaveBeenCalled();
  });

  it("falla con error funcional si no hay nombreGabinete resuelto", async () => {
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValue({
      success: true,
      message: "OK",
      data: {
        NombreGabinete: undefined as unknown as string,
        Radicado: "2025-0001",
        EstadoExistenciaRadicado: "YES",
      },
    });

    const { result } = renderHook(
      () => ({
        table: useListaDocumentosRadicadosTreeTable(10),
        context: useGestionRespuestaDocumentos(),
      }),
      {
        wrapper: buildProviderWrapper({ idTareaWf: 10, radicado: "2025-0001" }),
      },
    );

    await waitFor(() => {
      expect(result.current.context.gabineteLoading).toBe(false);
    });

    expect(await act(async () => result.current.table.load())).toEqual(
      expect.objectContaining({ ok: false, message: "NombreGabinete requerido" }),
    );
    expect(vi.mocked(listaDocumentosService.queryListaDocumentosRadicados)).not.toHaveBeenCalled();
  });

  it("evita que ver_documento se ejecute sin nombreGabinete", async () => {
    vi.mocked(gabineteService.getSolicitaGabinetePorTareaWorkflow).mockResolvedValue({
      success: true,
      message: "OK",
      data: {
        NombreGabinete: undefined as unknown as string,
        Radicado: "2025-0001",
        EstadoExistenciaRadicado: "YES",
      },
    });

    const { result } = renderHook(
      () => ({
        table: useListaDocumentosRadicadosTreeTable(10),
        context: useGestionRespuestaDocumentos(),
      }),
      {
        wrapper: buildProviderWrapper({ idTareaWf: 10, radicado: "2025-0001" }),
      },
    );

    await waitFor(() => {
      expect(result.current.context.gabineteLoading).toBe(false);
    });

    await expect(() =>
      act(async () => {
        await result.current.table.onSelectRow("r1");
      }),
    ).rejects.toThrow("NombreGabinete requerido");
  });
});
