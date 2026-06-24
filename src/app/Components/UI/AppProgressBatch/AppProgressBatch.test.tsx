import { StrictMode } from "react";
import { act, fireEvent, render, screen, waitFor } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppProgressBatch } from "./AppProgressBatch";
import { AppProgressBatch as ExportedAppProgressBatch } from "./index";
import type {
  AppProgressBatchItemContext,
  AppProgressBatchItemResult,
  AppProgressBatchProps,
  AppProgressBatchSummary,
} from "./AppProgressBatch.types";

const success = (): Promise<AppProgressBatchItemResult> =>
  Promise.resolve({ status: "success" });

const renderBatch = (
  overrides?: Partial<AppProgressBatchProps<string>>,
) => {
  const onOpenChange = vi.fn();
  const processItem = vi.fn(success);

  render(
    <AppProgressBatch
      open
      items={["a", "b"]}
      onOpenChange={onOpenChange}
      processItem={processItem}
      {...overrides}
    />,
  );

  return { onOpenChange, processItem };
};

describe("AppProgressBatch [SCRUMCORE-263]", () => {
  it("renderiza controlado con open y usa cierre desde AppModal/AppButton", () => {
    const { onOpenChange } = renderBatch({ items: [], emptyMessage: "Sin elementos" });

    expect(screen.getByText("Proceso por lotes")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Cerrar"));

    expect(onOpenChange).toHaveBeenCalledWith(false);
  });

  it("no ejecuta processItem con lista vacia, muestra mensaje y emite resumen cero", async () => {
    const onComplete = vi.fn();
    const processItem = vi.fn(success);

    render(
      <AppProgressBatch
        open
        items={[]}
        emptyMessage="Nada para procesar"
        onOpenChange={() => undefined}
        processItem={processItem}
        onComplete={onComplete}
      />,
    );

    expect(screen.getByText("Nada para procesar")).toBeInTheDocument();
    expect(processItem).not.toHaveBeenCalled();
    await waitFor(() =>
      expect(onComplete).toHaveBeenCalledWith({
        total: 0,
        processed: 0,
        success: 0,
        warnings: 0,
        skipped: 0,
        controlledErrors: 0,
        fatalErrors: 0,
        cancelled: false,
      }),
    );
  });

  it("autoStart=true inicia una sola vez aun bajo StrictMode", async () => {
    const processItem = vi.fn(success);

    render(
      <StrictMode>
        <AppProgressBatch
          open
          autoStart
          items={["a"]}
          onOpenChange={() => undefined}
          processItem={processItem}
        />
      </StrictMode>,
    );

    await waitFor(() => expect(processItem).toHaveBeenCalledTimes(1));
  });

  it("autoStart=false no inicia automaticamente", () => {
    const { processItem } = renderBatch();

    expect(processItem).not.toHaveBeenCalled();
    expect(screen.getByText("Iniciar")).toBeInTheDocument();
  });

  it("ejecuta items en orden y completa resumen exitoso", async () => {
    const order: string[] = [];
    const onComplete = vi.fn();

    renderBatch({
      items: ["a", "b", "c"],
      onComplete,
      processItem: vi.fn(async (item: string) => {
        order.push(item);
        return { status: "success" };
      }),
    });

    fireEvent.click(screen.getByText("Iniciar"));

    await waitFor(() => expect(onComplete).toHaveBeenCalled());
    expect(order).toEqual(["a", "b", "c"]);
    expect(onComplete).toHaveBeenCalledWith(
      expect.objectContaining({ total: 3, processed: 3, success: 3 }),
    );
  });

  it("registra warning y skipped sin pausar", async () => {
    const onComplete = vi.fn();
    const results: AppProgressBatchItemResult[] = [
      { status: "success" },
      { status: "warning", message: "Advertencia controlada" },
      { status: "skipped", message: "Omitido" },
    ];

    renderBatch({
      items: ["a", "b", "c"],
      onComplete,
      processItem: vi.fn(async () => results.shift() ?? { status: "success" }),
    });

    fireEvent.click(screen.getByText("Iniciar"));

    await waitFor(() =>
      expect(onComplete).toHaveBeenCalledWith(
        expect.objectContaining({
          processed: 3,
          success: 1,
          warnings: 1,
          skipped: 1,
        }),
      ),
    );
  });

  it("pausa con controlled-error y continua con el siguiente item", async () => {
    const onComplete = vi.fn();
    const processItem = vi
      .fn<
        (
          item: string,
          context: AppProgressBatchItemContext,
        ) => Promise<AppProgressBatchItemResult>
      >()
      .mockResolvedValueOnce({ status: "controlled-error", message: "Revisar item" })
      .mockResolvedValueOnce({ status: "success" });

    renderBatch({ items: ["a", "b"], processItem, onComplete });

    fireEvent.click(screen.getByText("Iniciar"));
    expect(await screen.findByText("Revisar item")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Continuar"));

    await waitFor(() =>
      expect(onComplete).toHaveBeenCalledWith(
        expect.objectContaining({ processed: 2, controlledErrors: 1, success: 1 }),
      ),
    );
  });

  it("cancela despues de controlled-error y no procesa pendientes", async () => {
    const onCancel = vi.fn();
    const processItem = vi
      .fn<
        (
          item: string,
          context: AppProgressBatchItemContext,
        ) => Promise<AppProgressBatchItemResult>
      >()
      .mockResolvedValueOnce({ status: "controlled-error", message: "No continuar" })
      .mockResolvedValueOnce({ status: "success" });

    renderBatch({ items: ["a", "b"], processItem, onCancel, confirmOnCancel: false });

    fireEvent.click(screen.getByText("Iniciar"));
    expect(await screen.findByText("No continuar")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Cancelar"));

    await waitFor(() =>
      expect(onCancel).toHaveBeenCalledWith(
        expect.objectContaining({ cancelled: true, processed: 1, controlledErrors: 1 }),
      ),
    );
    expect(processItem).toHaveBeenCalledTimes(1);
  });

  it("fatal-error detiene el proceso y emite onError", async () => {
    const onError = vi.fn();
    const processItem = vi
      .fn<
        (
          item: string,
          context: AppProgressBatchItemContext,
        ) => Promise<AppProgressBatchItemResult>
      >()
      .mockResolvedValueOnce({ status: "fatal-error", message: "Fallo fatal" });

    renderBatch({ items: ["a", "b"], processItem, onError });

    fireEvent.click(screen.getByText("Iniciar"));

    expect(await screen.findByText("Fallo fatal")).toBeInTheDocument();
    expect(onError).toHaveBeenCalled();
    expect(processItem).toHaveBeenCalledTimes(1);
  });

  it("excepcion de processItem detiene el proceso y emite onError", async () => {
    const onError = vi.fn();
    const processItem = vi.fn(async () => {
      throw new Error("Excepcion controlada");
    });

    renderBatch({ processItem, onError });

    fireEvent.click(screen.getByText("Iniciar"));

    expect(await screen.findByText("Excepcion controlada")).toBeInTheDocument();
    expect(onError).toHaveBeenCalled();
  });

  it("cancelacion llama AbortController.abort y no emite onComplete", async () => {
    const onCancel = vi.fn();
    const onComplete = vi.fn();
    let capturedSignal: AbortSignal | null = null;
    let resolveItem: ((value: AppProgressBatchItemResult) => void) | null = null;
    const processItem = vi.fn(
      (_item: string, context: AppProgressBatchItemContext) =>
        new Promise<AppProgressBatchItemResult>((resolve) => {
          capturedSignal = context.signal;
          resolveItem = resolve;
        }),
    );

    renderBatch({ processItem, onCancel, onComplete, confirmOnCancel: false });

    fireEvent.click(screen.getByText("Iniciar"));
    await waitFor(() => expect(capturedSignal).not.toBeNull());
    fireEvent.click(screen.getByText("Cancelar"));

    expect(capturedSignal?.aborted).toBe(true);
    await waitFor(() => expect(onCancel).toHaveBeenCalled());

    await act(async () => {
      resolveItem?.({ status: "success" });
    });

    expect(onComplete).not.toHaveBeenCalled();
  });

  it("normaliza progreso del item y usa getItemLabel", async () => {
    const onComplete = vi.fn();
    let capturedContext: AppProgressBatchItemContext | null = null;
    let resolveItem: ((value: AppProgressBatchItemResult) => void) | null = null;
    const processItem = vi.fn(
      (_item: string, context: AppProgressBatchItemContext) =>
        new Promise<AppProgressBatchItemResult>((resolve) => {
          capturedContext = context;
          resolveItem = resolve;
        }),
    );

    renderBatch({
      items: ["archivo-a"],
      getItemLabel: (item) => `Procesando ${item}`,
      processItem,
      onComplete,
    });

    fireEvent.click(screen.getByText("Iniciar"));

    expect(await screen.findByText("Procesando archivo-a")).toBeInTheDocument();
    await waitFor(() => expect(capturedContext).not.toBeNull());

    act(() => {
      capturedContext?.setItemProgress(-20);
    });
    expect(screen.getAllByRole("progressbar")[1]).toHaveAttribute("aria-valuenow", "0");

    act(() => {
      capturedContext?.setItemProgress(140);
    });
    await waitFor(() =>
      expect(screen.getAllByRole("progressbar")[1]).toHaveAttribute("aria-valuenow", "100"),
    );

    await act(async () => {
      resolveItem?.({ status: "success" });
    });
    await waitFor(() => expect(onComplete).toHaveBeenCalled());
  });

  it("resultado invalido de processItem se trata como fatal", async () => {
    const onError = vi.fn();
    const invalidResult = { status: "unknown" } as unknown as AppProgressBatchItemResult;

    renderBatch({
      processItem: vi.fn(async () => invalidResult),
      onError,
    });

    fireEvent.click(screen.getByText("Iniciar"));

    expect(
      await screen.findByText("processItem retorno un resultado invalido."),
    ).toBeInTheDocument();
    expect(onError).toHaveBeenCalled();
  });

  it("cierre durante ejecucion con confirmOnCancel pide confirmacion", async () => {
    const onCancel = vi.fn();
    let resolveItem: ((value: AppProgressBatchItemResult) => void) | null = null;
    const processItem = vi.fn(
      () =>
        new Promise<AppProgressBatchItemResult>((resolve) => {
          resolveItem = resolve;
        }),
    );

    renderBatch({ processItem, onCancel, confirmOnCancel: true });

    fireEvent.click(screen.getByText("Iniciar"));
    fireEvent.click(screen.getByText("Cancelar"));

    expect(
      screen.getByText("Hay un proceso en curso. Desea cancelarlo?"),
    ).toBeInTheDocument();
    fireEvent.click(screen.getByText("Confirmar"));

    await waitFor(() => expect(onCancel).toHaveBeenCalled());
    await act(async () => {
      resolveItem?.({ status: "success" });
    });
  });

  it("closeOnComplete cierra despues del final exitoso", async () => {
    const onOpenChange = vi.fn();

    renderBatch({ closeOnComplete: true, onOpenChange });

    fireEvent.click(screen.getByText("Iniciar"));

    await waitFor(() => expect(onOpenChange).toHaveBeenCalledWith(false));
  });

  it("exporta componente desde barrel local", () => {
    expect(ExportedAppProgressBatch).toBe(AppProgressBatch);
  });

  it("mantiene resumen final visible en proceso completo de 3 items", async () => {
    renderBatch({ items: ["a", "b", "c"] });

    fireEvent.click(screen.getByText("Iniciar"));

    await waitFor(() => expect(screen.getByLabelText("Resumen del proceso")).toBeInTheDocument());
    expect(screen.getByText("Proceso completado.")).toBeInTheDocument();
    expect(screen.getByText("Exitosos")).toBeInTheDocument();
  });

  it("mantiene summary tipado al completar mezcla de resultados", async () => {
    const onComplete = vi.fn<(summary: AppProgressBatchSummary) => void>();
    const results: AppProgressBatchItemResult[] = [
      { status: "success" },
      { status: "warning", message: "Aviso" },
      { status: "skipped" },
    ];

    renderBatch({
      items: ["a", "b", "c"],
      processItem: vi.fn(async () => results.shift() ?? { status: "success" }),
      onComplete,
    });

    fireEvent.click(screen.getByText("Iniciar"));

    await waitFor(() =>
      expect(onComplete).toHaveBeenCalledWith(
        expect.objectContaining({
          total: 3,
          processed: 3,
          success: 1,
          warnings: 1,
          skipped: 1,
          cancelled: false,
        }),
      ),
    );
  });
});
