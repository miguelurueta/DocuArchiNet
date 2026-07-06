import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { AppTreeTable } from "./AppTreeTable";
import type { AppTreeTableRow } from "./types";

const appTableSpy = vi.fn();

vi.mock("../AppTable/AppTable", () => ({
  default: (props: {
    rows: Array<Record<string, unknown>>;
    columns: Array<{
      headerName?: string;
      field?: string;
      cellRenderer?: (params: { data?: Record<string, unknown> }) => unknown;
    }>;
    onRowClicked?: (row: Record<string, unknown>) => void;
    onCellClicked?: (input: { row: Record<string, unknown>; columnKey?: string }) => void;
    onActionTriggered?: (input: { actionId: string; row: Record<string, unknown>; columnKey?: string }) => void;
    domLayout?: string;
    layoutMode?: string;
  }) => {
    appTableSpy(props);
    return (
      <div data-testid="mock-apptable">
        <div>
          {props.columns.map((col, index) => (
            <span key={col.field ?? col.headerName ?? String(index)}>
              {col.headerName ?? col.field}
            </span>
          ))}
        </div>

        {props.rows.map((row, rowIndex) => (
          <div key={String(row.id ?? rowIndex)}>
            {props.columns.map((col, colIndex) => {
              const key = col.field ?? col.headerName ?? String(colIndex);
              const rendered = col.cellRenderer
                ? col.cellRenderer({ data: row })
                : col.field
                  ? row[col.field]
                  : null;

              return (
                <span
                  key={String(key)}
                  onClick={() => props.onRowClicked?.(row)}
                  onDoubleClick={() =>
                    props.onCellClicked?.({ row, columnKey: col.field ?? col.headerName })
                  }
                  role="button"
                  tabIndex={0}
                >
                  {rendered as any}
                </span>
              );
            })}
          </div>
        ))}

        <button
          type="button"
          onClick={() => props.onActionTriggered?.({ actionId: "ver_documento", row: props.rows[0] })}
        >
          trigger-action
        </button>
      </div>
    );
  },
}));

describe("AppTreeTable", () => {
  it("permite configurar layout del AppTable interno", () => {
    const rows: AppTreeTableRow[] = [{ id: "a", label: "A" }];
    appTableSpy.mockClear();

    render(<AppTreeTable rows={rows} tableLayoutMode="fill" tableDomLayout="normal" />);

    expect(appTableSpy).toHaveBeenCalledWith(
      expect.objectContaining({ layoutMode: "fill", domLayout: "normal" }),
    );
  });

  it("[SPEC:APP-APPTREETABLE-001] renderiza filas jerarquicas desde rows", () => {
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
    const loadFail = vi.fn().mockResolvedValue({ ok: false, message: "Fallo" });

    const { rerender } = render(<AppTreeTable load={loadOk} />);
    expect(screen.getByText(/cargando/i)).toBeInTheDocument();

    expect(await screen.findByText(/sin registros/i)).toBeInTheDocument();

    rerender(<AppTreeTable load={loadFail} />);
    expect(await screen.findByText(/error: fallo/i)).toBeInTheDocument();
  });

  it("[SPEC:APP-APPTREETABLE-004] permite reintentar cuando load falla", async () => {
    const load = vi
      .fn()
      .mockResolvedValueOnce({ ok: false, message: "Fallo" })
      .mockResolvedValueOnce({ ok: true, rows: [] });

    render(<AppTreeTable load={load} />);
    expect(await screen.findByText(/error: fallo/i)).toBeInTheDocument();

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

describe("[SPEC:APPTREETABLE-216] AppTreeTable wrapper sobre AppTable", () => {
  it("mantiene compatibilidad sin afectar consumidores", () => {
    const rows: AppTreeTableRow[] = [{ id: "a", label: "A" }];
    render(<AppTreeTable rows={rows} />);
    expect(screen.getByText("A")).toBeInTheDocument();
  });

  it("[SPEC:APPTREETABLE-217] reexpone onCellClicked y onActionTriggered sin romper wrapper", () => {
    const rows: AppTreeTableRow[] = [{ id: "a", label: "A", values: { TIPODOCUMENTO: "DOC 1" } }];
    const onCellClicked = vi.fn();
    const onActionTriggered = vi.fn();

    render(<AppTreeTable rows={rows} onCellClicked={onCellClicked} onActionTriggered={onActionTriggered} />);

    const candidates = screen.getAllByRole("button", { name: "DOC 1" });
    const labelButton = candidates.find((el) => el.tagName.toLowerCase() === "button") ?? candidates[0];
    fireEvent.doubleClick(labelButton);
    expect(onCellClicked).toHaveBeenCalled();

    fireEvent.click(screen.getByRole("button", { name: /trigger-action/i }));
    expect(onActionTriggered).toHaveBeenCalledWith(expect.objectContaining({ actionId: "ver_documento", rowId: "a" }));
  });
});
