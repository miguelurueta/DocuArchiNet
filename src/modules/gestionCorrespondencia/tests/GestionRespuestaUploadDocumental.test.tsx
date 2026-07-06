import { render, screen } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { GestionRespuestaUploadDocumental } from "../components/gestionRespuestaMainTab/GestionRespuestaUploadDocumental";

const { appUploadDocumentalSpy, refreshDocumentosSpy, useDocumentosState, useEmpresaActualState } = vi.hoisted(() => ({
  appUploadDocumentalSpy: vi.fn(),
  refreshDocumentosSpy: vi.fn(),
  useDocumentosState: {
    idTareaWf: 933,
    idRutaWf: 9,
    radicado: "2600466700021",
    idRespuestaRadicado: 672,
    nombreGabinete: "CORRESPO",
    gabineteLoading: false,
    gabineteError: undefined as string | undefined,
  },
  useEmpresaActualState: {
    empresa: { IdEmpresa: 2 },
    isLoading: false,
    isError: false,
  },
}));

vi.mock("../hooks/useGestionRespuestaDocumentos", () => ({
  useGestionRespuestaDocumentos: () => ({
    ...useDocumentosState,
    refreshDocumentos: refreshDocumentosSpy,
  }),
}));

vi.mock("../../login/hooks/useEmpresaActual", () => ({
  useEmpresaActual: () => useEmpresaActualState,
}));

vi.mock("../../../app/auth/Infraestructura/ManejadorJWT", () => ({
  obtenerUsuarioIdAutenticado: () => 136,
}));

vi.mock("../../almacenamientoDocumental/components/AppUploadDocumental", () => ({
  AppUploadDocumental: (props: {
    title?: string;
    context: unknown;
    storageOptions?: unknown;
    buildStoreRequest?: unknown;
    onStored?: (result: { rawBackendResult?: unknown }, context: { source: "single" | "batch"; remainingFiles: number }) => void;
    onBatchComplete?: (summary: {
      stored: number;
      failed: number;
      skipped: number;
      cancelled: number;
      remainingFiles: number;
    }) => void;
    onError?: (error: unknown) => void;
  }) => {
    appUploadDocumentalSpy(props);
    return (
      <div data-testid="app-upload-documental-mock">
        <span>{props.title}</span>
        <button
          type="button"
          onClick={() =>
            props.onStored?.(buildStoredResult(), { source: "single", remainingFiles: 0 })
          }
        >
          Simular stored
        </button>
        <button type="button" onClick={() => props.onStored?.(buildStoredResult(), { source: "single", remainingFiles: 1 })}>
          Simular stored con pendientes
        </button>
        <button type="button" onClick={() => props.onStored?.(buildStoredResult(), { source: "batch", remainingFiles: 1 })}>
          Simular batch stored
        </button>
        <button
          type="button"
          onClick={() => props.onBatchComplete?.({ stored: 1, failed: 0, skipped: 0, cancelled: 0, remainingFiles: 0 })}
        >
          Simular batch complete
        </button>
        <button
          type="button"
          onClick={() => props.onBatchComplete?.({ stored: 1, failed: 1, skipped: 0, cancelled: 0, remainingFiles: 1 })}
        >
          Simular batch parcial
        </button>
        <button
          type="button"
          onClick={() => props.onBatchComplete?.({ stored: 1, failed: 0, skipped: 0, cancelled: 0, remainingFiles: 1 })}
        >
          Simular batch con archivo restante
        </button>
        <button
          type="button"
          onClick={() =>
            props.onError?.(
              new Error("No se puede guardar: selecciona la tipologia documental del archivo."),
            )
          }
        >
          Simular error tipologia
        </button>
      </div>
    );
  },
}));

function buildStoredResult() {
  return {
    rawBackendResult: {
      Documento: {
        IdAlmacen: 9967,
        IdRegistroProduccionDocumental: 23040,
        NombreArchivoFinal: "DIG00009967.pdf",
      },
      AnexoRespuesta: {
        IdRespuestaRadicado: 672,
        IdAlmacen: 9967,
        NombreGabinete: "CORRESPO",
        NombreArchivo: "soporte-respuesta.pdf",
        Created: true,
      },
    },
  };
}

describe("[SCRUMCORE-277] GestionRespuestaUploadDocumental", () => {
  beforeEach(() => {
    appUploadDocumentalSpy.mockClear();
    refreshDocumentosSpy.mockClear();
    useDocumentosState.idTareaWf = 933;
    useDocumentosState.idRutaWf = 9;
    useDocumentosState.radicado = "2600466700021";
    useDocumentosState.idRespuestaRadicado = 672;
    useDocumentosState.nombreGabinete = "CORRESPO";
    useDocumentosState.gabineteLoading = false;
    useDocumentosState.gabineteError = undefined;
    useEmpresaActualState.empresa = { IdEmpresa: 2 };
    useEmpresaActualState.isLoading = false;
    useEmpresaActualState.isError = false;
  });

  it("renderiza AppUploadDocumental con contexto, mapper y opciones enterprise", () => {
    render(<GestionRespuestaUploadDocumental />);

    expect(screen.getByTestId("app-upload-documental-mock")).toBeInTheDocument();
    expect(appUploadDocumentalSpy).toHaveBeenCalledWith(
      expect.objectContaining({
        title: "Adjuntos",
        tipologiaObligatoria: true,
        saveAllMode: "inline",
        loadConfig: expect.any(Function),
        buildStoreRequest: expect.any(Function),
        storageOptions: {
          backendPayloadCase: "pascal",
          validateStatusBeforeComplete: true,
          maxChunkSizeBytes: 4 * 1024 * 1024,
        },
        context: expect.objectContaining({
          nombreGabinete: "CORRESPO",
          idTareaWorkflow: 933,
          idRutaWorkflow: 9,
          idRespuesta: 672,
          nameModulo: "2600466700021",
          idUsuarioGestion: 136,
          idEmpresa: 2,
          fechaElaboracion: expect.stringMatching(/^\d{4}-\d{2}-\d{2}$/),
        }),
      }),
    );
  });

  it("refresca documentos cuando el backend confirma AnexoRespuesta.Created", () => {
    const onClose = vi.fn();
    render(<GestionRespuestaUploadDocumental onClose={onClose} />);

    screen.getByRole("button", { name: "Simular stored" }).click();

    expect(refreshDocumentosSpy).toHaveBeenCalledTimes(1);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("no cierra el modal al guardar un archivo individual si quedan archivos pendientes", () => {
    const onClose = vi.fn();
    render(<GestionRespuestaUploadDocumental onClose={onClose} />);

    screen.getByRole("button", { name: "Simular stored con pendientes" }).click();

    expect(refreshDocumentosSpy).toHaveBeenCalledTimes(1);
    expect(onClose).not.toHaveBeenCalled();
  });

  it("no cierra el modal por item almacenado dentro de un lote; cierra al completar el lote", () => {
    const onClose = vi.fn();
    render(<GestionRespuestaUploadDocumental onClose={onClose} />);

    screen.getByRole("button", { name: "Simular batch stored" }).click();

    expect(refreshDocumentosSpy).not.toHaveBeenCalled();
    expect(onClose).not.toHaveBeenCalled();

    screen.getByRole("button", { name: "Simular batch complete" }).click();

    expect(refreshDocumentosSpy).toHaveBeenCalledTimes(1);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it("refresca pero mantiene abierto el modal cuando el lote termina con archivos pendientes", () => {
    const onClose = vi.fn();
    render(<GestionRespuestaUploadDocumental onClose={onClose} />);

    screen.getByRole("button", { name: "Simular batch parcial" }).click();

    expect(refreshDocumentosSpy).toHaveBeenCalledTimes(1);
    expect(onClose).not.toHaveBeenCalled();
  });

  it("no cierra el modal cuando el lote guarda un archivo pero queda otro en cola", () => {
    const onClose = vi.fn();
    render(<GestionRespuestaUploadDocumental onClose={onClose} />);

    screen.getByRole("button", { name: "Simular batch con archivo restante" }).click();

    expect(refreshDocumentosSpy).toHaveBeenCalledTimes(1);
    expect(onClose).not.toHaveBeenCalled();
  });

  it("bloquea la carga documental cuando no existe idRutaWf para tipologias workflow", () => {
    useDocumentosState.idRutaWf = undefined;

    render(<GestionRespuestaUploadDocumental />);

    expect(screen.getByText("No hay ruta workflow disponible para cargar tipologias documentales.")).toBeInTheDocument();
    expect(appUploadDocumentalSpy).not.toHaveBeenCalled();
  });

  it("no muestra alerta superior para errores locales de tipologia requerida", () => {
    render(<GestionRespuestaUploadDocumental />);

    screen.getByRole("button", { name: "Simular error tipologia" }).click();

    expect(
      screen.queryByText("No se puede guardar: selecciona la tipologia documental del archivo."),
    ).not.toBeInTheDocument();
  });
});
