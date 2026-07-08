import { render, screen, fireEvent } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import RadicacionFormFooter from "./RadicacionFormFooter";

describe("RadicacionFormFooter", () => {
  it("[SPEC:TD-FE-03] invoca callbacks recibidos por props sin implementar logica propia", () => {
    const onDocumentosIa = vi.fn();
    const onClear = vi.fn();
    const onSubmit = vi.fn();

    render(
      <RadicacionFormFooter
        onDocumentosIa={onDocumentosIa}
        onClear={onClear}
        onSubmit={onSubmit}
      />,
    );

    fireEvent.click(screen.getByRole("button", { name: /documentos ia/i }));
    fireEvent.click(screen.getByRole("button", { name: /limpiar/i }));
    fireEvent.click(screen.getByRole("button", { name: /radicar/i }));

    expect(onDocumentosIa).toHaveBeenCalledTimes(1);
    expect(onClear).toHaveBeenCalledTimes(1);
    expect(onSubmit).toHaveBeenCalledTimes(1);
  });

  it("[SPEC:TD-FE-03] Documentos IA puede quedar sin callback y no dispara limpiar", () => {
    const onClear = vi.fn();

    render(<RadicacionFormFooter onClear={onClear} onSubmit={vi.fn()} />);

    expect(() =>
      fireEvent.click(screen.getByRole("button", { name: /documentos ia/i })),
    ).not.toThrow();
    expect(onClear).not.toHaveBeenCalled();
  });
});
