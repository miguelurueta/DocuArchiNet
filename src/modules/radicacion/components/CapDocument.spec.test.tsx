import { fireEvent, render, screen } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import CapDocument from "./CapDocument";
import type { AppTreeTableRow } from "../../../app/Components/UI/AppTreeTable";

vi.mock("../../../app/Components/UI/AppVisorEmbedPdf/AppVisorEmbedPdf", () => ({
  AppVisorEmbedPdf: () => <div data-testid="mock-pdf-viewer">PDF viewer mounted</div>,
}));

vi.mock("../../../app/Components/UI/AppTreeTable", () => ({
  AppTreeTable: ({
    rows,
    activeRowId,
    onSelectRow,
  }: {
    rows: AppTreeTableRow[];
    activeRowId?: string;
    onSelectRow?: (rowId: string) => void;
  }) => (
    <div data-testid="mock-app-tree-table" data-active-row-id={activeRowId ?? ""}>
      {rows.map((row) => (
        <button key={row.id} type="button" onClick={() => onSelectRow?.(row.id)}>
          {row.label}
        </button>
      ))}
    </div>
  ),
}));

describe("CapDocument persistent workspace", () => {
  it("keeps digitalizacion and pdf workspaces mounted while toggling visibility", async () => {
    render(<CapDocument />);

    const digitalizacionWorkspace = screen.getByTestId("digitalizacion-workspace");
    const pdfWorkspace = screen.getByTestId("pdf-viewer-workspace");

    expect(digitalizacionWorkspace).toBeInTheDocument();
    expect(pdfWorkspace).toBeInTheDocument();
    expect(digitalizacionWorkspace).toHaveAttribute("data-active", "true");
    expect(pdfWorkspace).toHaveAttribute("data-active", "false");
    expect(screen.getByTestId("mock-app-tree-table")).toBeInTheDocument();

    fireEvent.click(screen.getByRole("button", { name: "Factura.pdf" }));

    expect(digitalizacionWorkspace).toBeInTheDocument();
    expect(pdfWorkspace).toBeInTheDocument();
    expect(digitalizacionWorkspace).toHaveAttribute("data-active", "false");
    expect(pdfWorkspace).toHaveAttribute("data-active", "true");
    expect(await screen.findByTestId("mock-pdf-viewer")).toBeInTheDocument();
    expect(screen.getByTestId("mock-app-tree-table")).toHaveAttribute(
      "data-active-row-id",
      "factura-1",
    );

    fireEvent.click(screen.getByRole("button", { name: "Cerrar visor" }));

    expect(digitalizacionWorkspace).toBeInTheDocument();
    expect(pdfWorkspace).toBeInTheDocument();
    expect(digitalizacionWorkspace).toHaveAttribute("data-active", "true");
    expect(pdfWorkspace).toHaveAttribute("data-active", "false");
    expect(screen.getByTestId("mock-app-tree-table")).toBeInTheDocument();
  });
});
