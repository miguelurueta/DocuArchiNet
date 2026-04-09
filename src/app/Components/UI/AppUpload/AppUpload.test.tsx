import { act, fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppUpload } from "./AppUpload";
import type { AppUploadFile, AppUploadHandle } from "./AppUpload";
import styles from "./AppUpload.module.css";

const sampleFile = (overrides?: Partial<AppUploadFile>): AppUploadFile => ({
  uid: "file-1",
  name: "documento.pdf",
  size: 1200,
  type: "application/pdf",
  status: "queued",
  percent: 0,
  ...overrides,
});

const createFile = (name: string, type: string, size = 1000) =>
  new File(["x".repeat(size)], name, { type });

describe("AppUpload [SPEC:APP-UPLOAD-001]", () => {
  it("renderiza la lista controlada y respeta el orden", () => {
    render(
      <AppUpload
        value={[sampleFile(), sampleFile({ uid: "file-2", name: "foto.png" })]}
        onChange={() => undefined}
      />,
    );

    expect(screen.getByTitle("documento.pdf")).toBeInTheDocument();
    expect(screen.getByTitle("foto.png")).toBeInTheDocument();
  });

  it("oculta el boton de carga al alcanzar maxCount", () => {
    render(
      <AppUpload value={[sampleFile()]} maxCount={1} onChange={() => undefined} />,
    );

    expect(screen.queryByText("Cargar archivos")).toBeNull();
  });

  it("dispara onRemove y onChange al eliminar un archivo", () => {
    const handleChange = vi.fn();
    const handleRemove = vi.fn();

    render(
      <AppUpload
        value={[sampleFile()]}
        onChange={handleChange}
        onRemove={handleRemove}
      />,
    );

    const removeButton = screen.getByLabelText("Eliminar documento.pdf");
    fireEvent.click(removeButton);

    expect(handleRemove).toHaveBeenCalledTimes(1);
    expect(handleChange).toHaveBeenCalledTimes(1);
  });

  it("valida accept y maxSize antes de cargar", () => {
    const handleError = vi.fn();
    const { container } = render(
      <AppUpload
        accept="image/png"
        maxSize={10}
        onChange={() => undefined}
        onError={handleError}
      />,
    );

    const input = container.querySelector("input[type=\"file\"]") as HTMLInputElement;
    const file = createFile("doc.pdf", "application/pdf", 20);

    fireEvent.change(input, { target: { files: [file] } });

    expect(handleError).toHaveBeenCalled();
  });

  it("respeta validateFile custom", async () => {
    const handleError = vi.fn();
    const validateFile = vi.fn().mockResolvedValue(false);
    const { container } = render(
      <AppUpload
        validateFile={validateFile}
        onChange={() => undefined}
        onError={handleError}
      />,
    );

    const input = container.querySelector("input[type=\"file\"]") as HTMLInputElement;
    const file = createFile("doc.pdf", "application/pdf", 5);

    await act(async () => {
      fireEvent.change(input, { target: { files: [file] } });
    });

    expect(validateFile).toHaveBeenCalled();
    expect(handleError).toHaveBeenCalled();
  });

  it("emite onProgress 0-100 y onSuccess en customRequest", async () => {
    const handleProgress = vi.fn();
    const handleSuccess = vi.fn();
    const { container } = render(
      <AppUpload
        strategy="auto"
        onChange={() => undefined}
        onProgress={handleProgress}
        onSuccess={handleSuccess}
        customRequest={async (_file, helpers) => {
          helpers.onProgress(120);
          helpers.onSuccess();
        }}
      />,
    );

    const input = container.querySelector("input[type=\"file\"]") as HTMLInputElement;
    const file = createFile("foto.png", "image/png", 5);

    await act(async () => {
      fireEvent.change(input, { target: { files: [file] } });
    });

    expect(handleProgress).toHaveBeenCalledWith(expect.any(Object), 100);
    expect(handleSuccess).toHaveBeenCalled();
  });

  it("permite retry y abort via ref", async () => {
    const handleChange = vi.fn();
    const handleError = vi.fn();
    const ref = { current: null as AppUploadHandle | null };
    let latestFile: AppUploadFile | undefined;

    const { container } = render(
      <AppUpload
        ref={(instance) => {
          ref.current = instance;
        }}
        strategy="auto"
        onChange={(files) => {
          latestFile = files[0];
          handleChange(files);
        }}
        onError={handleError}
        customRequest={async (_file, helpers) => {
          helpers.onError(new Error("fallo"));
        }}
      />,
    );

    const input = container.querySelector("input[type=\"file\"]") as HTMLInputElement;
    const file = createFile("foto.png", "image/png", 5);

    await act(async () => {
      fireEvent.change(input, { target: { files: [file] } });
    });

    expect(handleError).toHaveBeenCalled();

    await act(async () => {
      if (!latestFile) return;
      ref.current?.retry({ ...latestFile, status: "error" });
      ref.current?.abort({ ...latestFile, status: "uploading" });
    });

    expect(handleChange).toHaveBeenCalled();
  });

  it("muestra estado drag valid/invalid", () => {
    const { container } = render(
      <AppUpload drag accept="application/pdf" onChange={() => undefined} />,
    );
    const root = container.firstElementChild as HTMLElement;

    const dataTransfer = {
      items: [
        {
          type: "application/pdf",
          getAsFile: () => createFile("doc.pdf", "application/pdf", 5),
        },
      ],
    };

    fireEvent.dragOver(root, { dataTransfer });
    expect(root.querySelector(`.${styles.dragValid}`)).toBeTruthy();

    const invalidTransfer = {
      items: [
        {
          type: "application/zip",
          getAsFile: () => createFile("file.zip", "application/zip", 5),
        },
      ],
    };

    fireEvent.dragOver(root, { dataTransfer: invalidTransfer });
    expect(root.querySelector(`.${styles.dragInvalid}`)).toBeTruthy();
  });

  it("permite acciones por teclado", () => {
    const handleRemove = vi.fn();
    const handlePreview = vi.fn();
    render(
      <AppUpload
        value={[sampleFile()]}
        onChange={() => undefined}
        onRemove={handleRemove}
        onPreview={handlePreview}
      />,
    );

    const card = screen.getByTitle("documento.pdf").closest("[data-status]");
    fireEvent.keyDown(card as Element, { key: "Enter" });
    fireEvent.keyDown(card as Element, { key: "Delete" });

    expect(handlePreview).toHaveBeenCalled();
    expect(handleRemove).toHaveBeenCalled();
  });

  it("emite telemetry en select y upload_success", async () => {
    const handleTelemetry = vi.fn();
    const { container } = render(
      <AppUpload
        strategy="auto"
        onChange={() => undefined}
        onTelemetry={handleTelemetry}
        customRequest={async (_file, helpers) => {
          helpers.onSuccess();
        }}
      />,
    );

    const input = container.querySelector("input[type=\"file\"]") as HTMLInputElement;
    const file = createFile("foto.png", "image/png", 5);

    await act(async () => {
      fireEvent.change(input, { target: { files: [file] } });
    });

    expect(handleTelemetry).toHaveBeenCalledWith(
      expect.objectContaining({ type: "select" }),
    );
    expect(handleTelemetry).toHaveBeenCalledWith(
      expect.objectContaining({ type: "upload_success" }),
    );
  });
});
