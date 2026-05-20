import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppTreeTable } from "./AppTreeTable";
import type { AppTreeTableRow } from "./types";

describe("AppTreeTable", () => {
  it("[SPEC:APP-APPTREETABLE-001] renderiza filas jerárquicas desde rows", () => {
    const rows: AppTreeTableRow[] = [
      { id: "a", label: "A", children: [{ id: "a-1", label: "A1" }] },
    ];

    render(<AppTreeTable rows={rows} />);
    expect(screen.getByText("A")).toBeInTheDocument();
    expect(screen.queryByText("A1")).not.toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /expandir a/i }));
    expect(screen.getByText("A1")).toBeInTheDocument();
  });

  it("[SPEC:APP-APPTREETABLE-002] renderiza loading/empty/error con load() mock", async () => {
    const loadOk = vi.fn().mockResolvedValue({ ok: true, rows: [] });
    const loadFail = vi.fn().mockResolvedValue({ ok: false, message: "Falló" });

    const { rerender } = render(<AppTreeTable load={loadOk} />);
    expect(screen.getByText(/cargando/i)).toBeInTheDocument();

    expect(await screen.findByText(/sin registros/i)).toBeInTheDocument();

    rerender(<AppTreeTable load={loadFail} />);
    expect(await screen.findByText(/error: falló/i)).toBeInTheDocument();
  });

  it("[SPEC:APP-APPTREETABLE-004] permite reintentar cuando load falla", async () => {
    const load = vi
      .fn()
      .mockResolvedValueOnce({ ok: false, message: "Falló" })
      .mockResolvedValueOnce({ ok: true, rows: [] });

    render(<AppTreeTable load={load} />);
    expect(await screen.findByText(/error: falló/i)).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /reintentar/i }));
    expect(await screen.findByText(/sin registros/i)).toBeInTheDocument();
    expect(load).toHaveBeenCalledTimes(2);
  });

  it("[SPEC:APP-APPTREETABLE-003] expand/collapse muestra/oculta hijos correctamente", () => {
    const rows: AppTreeTableRow[] = [
      { id: "a", label: "A", children: [{ id: "a-1", label: "A1" }] },
    ];

    render(<AppTreeTable rows={rows} />);
    fireEvent.click(screen.getByRole("button", { name: /expandir a/i }));
    expect(screen.getByText("A1")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: /colapsar a/i }));
    expect(screen.queryByText("A1")).not.toBeInTheDocument();
  });

  it("[SPEC:APP-APPTREETABLE-005] infiere columnas desde Values y las renderiza", () => {
    const rows: AppTreeTableRow[] = [
      { id: "a", label: "A", values: { ID: 1, TIPODOCUMENTO: "Factura" } },
    ];

    render(<AppTreeTable rows={rows} />);
    expect(screen.getByText("ID")).toBeInTheDocument();
    expect(screen.getByText("TIPODOCUMENTO")).toBeInTheDocument();
    expect(screen.getByText("1")).toBeInTheDocument();
    expect(screen.getByText("Factura")).toBeInTheDocument();
  });
});

