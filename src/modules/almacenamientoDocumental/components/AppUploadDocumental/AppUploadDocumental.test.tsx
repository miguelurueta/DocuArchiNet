import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import type { ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import { AppUploadDocumental } from "./AppUploadDocumental";
import type { AppUploadDocumentalProps } from "./AppUploadDocumental.types";
import type {
  AppUploadBatchFileItem,
  AppUploadBatchSummary,
  AppUploadBatchViewProps,
} from "../../../../app/Components/UI/AppUploadBatchView";
import { uploadAndStoreOneDocument } from "../../services/almacenamientoDocumentalUpload.service";

vi.mock("../../services/almacenamientoDocumentalUpload.service", () => ({
  uploadAndStoreOneDocument: vi.fn(),
}));

vi.mock("../../../../app/Components/UI/AppUploadBatchView", () => ({
  AppUploadBatchView: (props: AppUploadBatchViewProps) => (
    <section aria-label={props.title}>
      <h2>{props.title}</h2>
      <button
        type="button"
        disabled={props.disabled}
        onClick={() => props.onFilesSelected?.([new File(["bad"], "mal.exe")])}
      >
        add-invalid
      </button>
      <button
        type="button"
        disabled={props.disabled}
        onClick={() =>
          props.onFilesSelected?.([
            new File(["ok"], "contrato_arrendamiento.pdf", { type: "application/pdf" }),
          ])
        }
      >
        add-valid
      </button>
      <button type="button" disabled={!props.canSaveAll} onClick={props.onSaveAll}>
        Guardar todo
      </button>
      <div>{props.files.length === 0 ? props.emptyMessage : null}</div>
      {props.files.map((item) => (
        <article key={item.uid}>
          <span>{item.name}</span>
          {item.error ? <span>{item.error}</span> : null}
          {props.renderMetadata?.({
            item: item as AppUploadBatchFileItem<unknown>,
            disabled: Boolean(item.disabled),
          }) as ReactNode}
          <button type="button" onClick={() => props.onSaveFile?.(item.uid)}>
            Guardar {item.name}
          </button>
        </article>
      ))}
      {props.renderFooterExtra?.(
        props.summary ??
          ({
            total: props.files.length,
            queued: 0,
            ready: props.files.length,
            uploading: 0,
            done: 0,
            warning: 0,
            error: 0,
            cancelled: 0,
          } satisfies AppUploadBatchSummary),
      )}
    </section>
  ),
}));

vi.mock("../../../../app/Components/UI/AppProgressBatch", () => ({
  AppProgressBatch: () => null,
}));

const mockedUploadAndStoreOneDocument = vi.mocked(uploadAndStoreOneDocument);

const baseProps = (overrides: Partial<AppUploadDocumentalProps> = {}): AppUploadDocumentalProps => ({
  proceso: "radicacion",
  context: { nombreGabinete: "Gestion", idExpediente: 10 },
  loadConfig: vi.fn().mockResolvedValue({
    accept: ".pdf,.png",
    allowedExtensions: [".pdf", ".png"],
    maxSizeBytes: 10_000,
    multiple: true,
    requiereTipologia: true,
    requiereFechaCarga: true,
    fechaCargaObligatoria: true,
    validationMode: "queue-with-error",
    preferredChunkSizeBytes: 2,
  }),
  loadTiposDocumentales: vi.fn().mockResolvedValue([
    { idTipoDocumento: 1, nombreTipoDocumento: "Contrato arrendamiento" },
    { idTipoDocumento: 2, nombreTipoDocumento: "Factura venta" },
  ]),
  ...overrides,
});

describe("[SPEC:SCRUMCORE-271] AppUploadDocumental", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it("carga config/tipologias y renderiza la vista documental", async () => {
    const props = baseProps();
    render(<AppUploadDocumental {...props} />);

    await waitFor(() =>
      expect(props.loadConfig).toHaveBeenCalledWith({
        proceso: "radicacion",
        context: props.context,
        modoDocumento: undefined,
      }),
    );
    expect(props.loadTiposDocumentales).toHaveBeenCalledWith({
      proceso: "radicacion",
      context: props.context,
    });
    expect(screen.getByText("Adjuntar documentos")).toBeInTheDocument();
    expect(screen.getByText("No hay documentos en la cola.")).toBeInTheDocument();
  });

  it("deshabilita seleccion si falla config", async () => {
    render(
      <AppUploadDocumental
        {...baseProps({
          loadConfig: vi.fn().mockRejectedValue(new Error("config failed")),
          onError: vi.fn(),
        })}
      />,
    );

    expect(await screen.findByText("No fue posible cargar la configuracion documental.")).toBeInTheDocument();
    expect(screen.getByText("add-valid")).toBeDisabled();
  });

  it("encola archivo invalido con error en queue-with-error", async () => {
    render(<AppUploadDocumental {...baseProps()} />);
    await screen.findByText("Adjuntar documentos");

    fireEvent.click(screen.getByText("add-invalid"));

    expect(await screen.findByText("mal.exe")).toBeInTheDocument();
    expect(screen.getAllByText(/Extension no permitida/).length).toBeGreaterThan(0);
  });

  it("guarda un archivo individual con metadata, callbacks tipados y mapper", async () => {
    const onStored = vi.fn();
    const onInterfaceRegistration = vi.fn();
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
        nombreArchivoFinal: "contrato.pdf",
        requestId: "req-1",
      },
      rawBackendResult: { contadorPaginas: 3 },
    });

    render(<AppUploadDocumental {...baseProps({ onStored, onInterfaceRegistration })} />);
    await screen.findByText("Adjuntar documentos");

    fireEvent.click(screen.getByText("add-valid"));

    expect(await screen.findByText("contrato_arrendamiento.pdf")).toBeInTheDocument();
    fireEvent.change(screen.getByLabelText(/Fecha documental/i), { target: { value: "2026-01-10" } });
    fireEvent.click(screen.getByText(/Guardar contrato_arrendamiento.pdf/i));

    await waitFor(() => expect(mockedUploadAndStoreOneDocument).toHaveBeenCalledTimes(1));
    expect(mockedUploadAndStoreOneDocument.mock.calls[0][0].request).toMatchObject({
      nombreGabinete: "Gestion",
      trd: {
        idTipoDocumento: 1,
        nombreTipoDocumento: "Contrato arrendamiento",
      },
      expediente: {
        idExpediente: 10,
      },
    });
    expect(onStored).toHaveBeenCalledWith(
      expect.objectContaining({
        fileUid: expect.any(String),
        fileName: "contrato_arrendamiento.pdf",
        idAlmacen: 1,
        interfaceRegistration: expect.any(Array),
      }),
    );
    expect(onInterfaceRegistration).toHaveBeenCalledWith(
      expect.arrayContaining([{ kind: "page-counter", contadorPaginas: 3 }]),
    );
  });
});
