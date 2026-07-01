import { fireEvent, render, screen, waitFor, within } from "@testing-library/react";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { AppUploadBatchView } from "./AppUploadBatchView";
import { AppUploadBatchView as ExportedAppUploadBatchView } from "./index";
import type { AppUploadBatchFileItem, AppUploadBatchViewProps } from "./AppUploadBatchView.types";

const createFile = (name: string, type = "application/pdf") =>
  new File(["content"], name, { type, lastModified: 1_700_000_000_000 });

const createItem = (
  uid: string,
  overrides: Partial<AppUploadBatchFileItem<{ type?: string }>> = {},
): AppUploadBatchFileItem<{ type?: string }> => {
  const file = overrides.file ?? createFile(overrides.name ?? `${uid}.pdf`);
  const name = overrides.name ?? file.name;
  return {
    uid,
    file,
    name,
    size: overrides.size ?? file.size,
    extension: overrides.extension ?? `.${name.split(".").pop() ?? "pdf"}`,
    state: overrides.state ?? "ready",
    ...overrides,
  };
};

const renderView = (overrides: Partial<AppUploadBatchViewProps<{ type?: string }>> = {}) => {
  const props: AppUploadBatchViewProps<{ type?: string }> = {
    files: [createItem("a"), createItem("b", { name: "very-long-document-name.pdf" })],
    selectedUid: "a",
    onFilesSelected: vi.fn(),
    onSelectFile: vi.fn(),
    onPreviewFile: vi.fn(),
    onRemoveFile: vi.fn(),
    onSaveFile: vi.fn(),
    onSaveAll: vi.fn(),
    onClearAll: vi.fn(),
    onClosePreview: vi.fn(),
    ...overrides,
  };

  const result = render(<AppUploadBatchView {...props} />);
  return { ...result, props };
};

describe("AppUploadBatchView [SCRUMCORE-270]", () => {
  beforeEach(() => {
    Object.defineProperty(window, "localStorage", {
      value: {
        getItem: vi.fn(),
        setItem: vi.fn(),
        removeItem: vi.fn(),
        clear: vi.fn(),
      },
      configurable: true,
    });
    vi.spyOn(URL, "createObjectURL").mockImplementation((value) => {
      const file = value as File;
      return `blob:test/${file.name}`;
    });
    vi.spyOn(URL, "revokeObjectURL").mockImplementation(() => undefined);
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("exporta el componente desde su index", () => {
    expect(ExportedAppUploadBatchView).toBe(AppUploadBatchView);
  });

  it("renderiza lista vacia con contador y mensaje", () => {
    renderView({ files: [], emptyMessage: "Sin archivos pendientes" });

    expect(screen.getByText("Sin archivos pendientes")).toBeInTheDocument();
    expect(screen.getAllByText("0 archivo(s)").length).toBeGreaterThan(0);
  });

  it("renderiza archivos con nombre, tamano, estado y fila activa", () => {
    renderView({
      files: [
        createItem("a", { size: 2048, state: "uploading", progress: 45, phaseLabel: "Enviando" }),
        createItem("b", { name: "factura-final.pdf", state: "done" }),
      ],
      selectedUid: "b",
    });

    expect(screen.getByText("a.pdf")).toBeInTheDocument();
    expect(screen.getAllByText(/7 B/).length).toBeGreaterThan(0);
    expect(screen.getByText("Cargando")).toBeInTheDocument();
    expect(screen.getByText("Enviando")).toBeInTheDocument();
    const activeFileButton = screen
      .getAllByRole("button", { name: /factura-final.pdf/i })
      .find((button) => button.getAttribute("aria-pressed") === "true");

    expect(activeFileButton).toHaveAttribute(
      "aria-pressed",
      "true",
    );
  });

  it("ejecuta callbacks de acciones globales y por archivo", async () => {
    const { props } = renderView({ canSaveOne: true });

    fireEvent.click(screen.getByText("Guardar todo"));
    fireEvent.click(screen.getByText("Limpiar todo"));
    fireEvent.click(screen.getByLabelText("Ver a.pdf"));
    fireEvent.click(screen.getByLabelText("Guardar a.pdf"));
    fireEvent.click(screen.getByLabelText("Eliminar a.pdf"));

    expect(props.onSaveAll).toHaveBeenCalledTimes(1);
    expect(props.onClearAll).toHaveBeenCalledTimes(1);
    expect(props.onSelectFile).toHaveBeenCalledWith("a");
    expect(props.onPreviewFile).toHaveBeenCalledWith("a");
    expect(props.onSaveFile).toHaveBeenCalledWith("a");
    await waitFor(() => expect(props.onRemoveFile).toHaveBeenCalledWith("a"));
  });

  it("compone AppUpload como selector de archivos", () => {
    const { container } = renderView({ drag: false });

    expect(container.querySelector('input[type="file"]')).not.toBeNull();
    expect(screen.getByText("Cargar archivos")).toBeInTheDocument();
  });

  it("respeta disabled, loading, can* e item.disabled", () => {
    renderView({
      loading: true,
      canSaveOne: true,
      files: [createItem("a", { disabled: true })],
    });

    expect(screen.getByText("Guardar todo").closest("button")).toBeDisabled();
    expect(screen.getByText("Limpiar todo").closest("button")).toBeDisabled();
    expect(screen.getByLabelText("Ver a.pdf")).toBeDisabled();
    expect(screen.getByLabelText("Guardar a.pdf")).toBeDisabled();
    expect(screen.getByLabelText("Eliminar a.pdf")).toBeDisabled();
  });

  it("renderiza slots de metadata, preview, nombre y footer", () => {
    renderView({
      files: [createItem("a", { metadata: { type: "Contrato" } })],
      renderFileName: (item) => <span>Nombre custom {item.name}</span>,
      renderMetadata: ({ item }) => <span>Tipo {item.metadata?.type}</span>,
      renderPreview: ({ item }) => <span>Preview custom {item.name}</span>,
      renderFooterExtra: (summary) => <span>Extra {summary.total}</span>,
    });

    expect(screen.getByText("Nombre custom a.pdf")).toBeInTheDocument();
    expect(screen.getByText("Tipo Contrato")).toBeInTheDocument();
    fireEvent.click(screen.getByLabelText("Ver a.pdf"));
    expect(screen.getByText("Preview custom a.pdf")).toBeInTheDocument();
    expect(screen.getByText("Extra 1")).toBeInTheDocument();
  });

  it("muestra error y advertencia por archivo", () => {
    renderView({
      files: [
        createItem("a", { state: "warning", warning: "Requiere revision" }),
        createItem("b", { state: "error", error: "No se pudo procesar" }),
      ],
    });

    expect(screen.getByText("Advertencia")).toBeInTheDocument();
    expect(screen.getByText("Requiere revision")).toBeInTheDocument();
    expect(screen.getByText("Error")).toBeInTheDocument();
    expect(screen.getByText("No se pudo procesar")).toBeInTheDocument();
  });

  it("renderiza preview default PDF, imagen y fallback", () => {
    const { rerender, props } = renderView({
      files: [createItem("pdf", { file: createFile("doc.pdf", "application/pdf") })],
      selectedUid: "pdf",
    });

    fireEvent.click(screen.getByLabelText("Ver doc.pdf"));
    expect(screen.getByTitle("Vista previa de doc.pdf")).toBeInTheDocument();

    rerender(
      <AppUploadBatchView
        {...props}
        files={[
          createItem("img", {
            file: createFile("foto.png", "image/png"),
            name: "foto.png",
            extension: ".png",
          }),
        ]}
        selectedUid="img"
      />,
    );

    fireEvent.click(screen.getByLabelText("Ver foto.png"));
    expect(screen.getByAltText("foto.png")).toBeInTheDocument();

    rerender(
      <AppUploadBatchView
        {...props}
        files={[
          createItem("txt", {
            file: createFile("notas.txt", "text/plain"),
            name: "notas.txt",
            extension: ".txt",
          }),
        ]}
        selectedUid="txt"
      />,
    );

    fireEvent.click(screen.getByLabelText("Ver notas.txt"));
    const preview = screen.getByLabelText("Vista previa del archivo activo");
    expect(within(preview).getAllByText("notas.txt").length).toBeGreaterThan(0);
    expect(within(preview).getByText(/TXT/)).toBeInTheDocument();
  });

  it("permite cerrar la vista previa aunque no exista archivo seleccionado", () => {
    const { rerender, props } = renderView({
      files: [createItem("a", { file: createFile("a.pdf", "application/pdf") })],
      selectedUid: "a",
    });

    fireEvent.click(screen.getByLabelText("Ver a.pdf"));

    rerender(<AppUploadBatchView {...props} files={[]} selectedUid={undefined} />);

    const closePreviewButton = screen.getByLabelText("Cerrar vista previa");
    expect(closePreviewButton).toBeEnabled();

    fireEvent.click(closePreviewButton);

    expect(props.onClosePreview).toHaveBeenCalledTimes(1);
  });

  it("revoca object URL al cambiar archivo y desmontar", () => {
    const { rerender, unmount, props } = renderView({
      files: [createItem("a", { file: createFile("a.pdf", "application/pdf") })],
      selectedUid: "a",
    });

    fireEvent.click(screen.getByLabelText("Ver a.pdf"));
    rerender(
      <AppUploadBatchView
        {...props}
        files={[createItem("b", { file: createFile("b.pdf", "application/pdf") })]}
        selectedUid="b"
      />,
    );
    unmount();

    expect(URL.revokeObjectURL).toHaveBeenCalledWith("blob:test/a.pdf");
    expect(URL.revokeObjectURL).toHaveBeenCalledWith("blob:test/b.pdf");
  });
});
