import { fireEvent, render, screen } from "@testing-library/react";
import { MemoryRouter } from "react-router-dom";
import React, { memo, useState } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import GestionCorrespondencia from "../pages/GestionCorrespondencia";
import type { GestionCorrespondenciaTableResult } from "../hooks/useGestionCorrespondenciaTable";
import * as workflowInboxAutocompleteHook from "../hooks/useWorkflowInboxAutocomplete";

type RenderLogEntry = {
  count: number;
  changedKeys: string[][];
  lastProps?: Record<string, unknown>;
};

const renderLog: Record<string, RenderLogEntry> = {};

const resetRenderLog = () => {
  for (const key of Object.keys(renderLog)) {
    delete renderLog[key];
  }
};

const toComparableProps = (props: Record<string, unknown>) =>
  Object.fromEntries(Object.entries(props).filter(([key]) => key !== "children"));

const recordRender = (name: string, props: Record<string, unknown>) => {
  const comparableProps = toComparableProps(props);
  const previous = renderLog[name];
  const changedKeys =
    previous?.lastProps == null
      ? Object.keys(comparableProps)
      : Object.keys(comparableProps).filter(
          (key) => !Object.is(previous.lastProps?.[key], comparableProps[key]),
        );

  renderLog[name] = {
    count: (previous?.count ?? 0) + 1,
    changedKeys: [...(previous?.changedKeys ?? []), changedKeys],
    lastProps: comparableProps,
  };
};

const getRenderCount = (name: string) => renderLog[name]?.count ?? 0;

const diffFrom = (baseline: Record<string, number>) =>
  Object.fromEntries(
    Object.keys(renderLog).map((key) => [key, getRenderCount(key) - (baseline[key] ?? 0)]),
  );

const snapshotCounts = () =>
  Object.fromEntries(Object.keys(renderLog).map((key) => [key, getRenderCount(key)]));

vi.mock("../hooks/useWorkflowInboxAutocomplete", () => ({
  useWorkflowInboxAutocomplete: vi.fn(),
}));

vi.mock("../../../app/Components/UI/AppToolbar", () => ({
  AppToolbar: memo(function MockAppToolbar(props: {
    className?: string;
    actionContent?: React.ReactNode;
  }) {
    recordRender("AppToolbar", props as Record<string, unknown>);
    return (
      <div data-testid="mock-toolbar" className={props.className}>
        {props.actionContent}
      </div>
    );
  }),
}));

vi.mock("../../../app/Components/UI/AppContent", () => ({
  AppContent: memo(function MockAppContent(props: {
    children?: React.ReactNode;
    className?: string;
    contentClassName?: string;
  }) {
    recordRender("AppContent", props as Record<string, unknown>);
    return (
      <div data-testid="mock-content" className={props.className}>
        <div className={props.contentClassName}>{props.children}</div>
      </div>
    );
  }),
}));

vi.mock("../../../app/Components/UI/AppButton", () => ({
  AppButton: memo(function MockAppButton(props: {
    children?: React.ReactNode;
    onClick?: () => void;
    "aria-label"?: string;
  }) {
    recordRender("AppButton", props as Record<string, unknown>);
    return (
      <button type="button" aria-label={props["aria-label"]} onClick={props.onClick}>
        {props.children}
      </button>
    );
  }),
}));

vi.mock("../../../app/Components/UI/AppInputSearch", () => ({
  AppInputSearch: memo(function MockAppInputSearch(props: {
    value?: string;
    onChange?: (value: string) => void;
    onSearch?: (value: string) => void;
    onClear?: () => void;
    "aria-label"?: string;
  }) {
    recordRender("AppInputSearch", props as Record<string, unknown>);
    return (
      <div>
        <input
          aria-label={props["aria-label"]}
          value={props.value ?? ""}
          onChange={(event) => props.onChange?.(event.target.value)}
          onKeyDown={(event) => {
            if (event.key === "Enter") {
              props.onSearch?.((event.target as HTMLInputElement).value);
            }
          }}
        />
        <button type="button" onClick={() => props.onSearch?.(props.value ?? "")}>
          Buscar
        </button>
        <button type="button" onClick={props.onClear}>
          Limpiar
        </button>
      </div>
    );
  }),
}));

vi.mock("../../../app/Components/UI/AppTable/AppTableQueryWrapper", () => ({
  AppTableQueryWrapper: memo(function MockAppTableQueryWrapper(props: {
    children?: React.ReactNode;
    paginationActions?: React.ReactNode;
  }) {
    recordRender("AppTableQueryWrapper", props as Record<string, unknown>);
    return (
      <div data-testid="mock-query-wrapper">
        {props.paginationActions}
        {props.children}
      </div>
    );
  }),
}));

vi.mock("../../../app/Components/UI/AppTable/AppTableExport", () => ({
  AppTableExport: memo(function MockAppTableExport(props: {
    dataSource: {
      getCurrentPageRows: () => unknown[];
      getSelectedRows: () => unknown[];
      getAllMatchingRows?: () => Promise<unknown[]>;
      getBackendExportFile?: (...args: unknown[]) => Promise<unknown>;
    };
  }) {
    const [open, setOpen] = useState(false);
    recordRender("AppTableExport", {
      ...props,
      open,
      selectedCount: props.dataSource.getSelectedRows().length,
    } as Record<string, unknown>);

    return (
      <div data-testid="mock-export">
        <button type="button" onClick={() => setOpen((prev) => !prev)}>
          Exportar
        </button>
        <span data-testid="mock-export-open">{open ? "open" : "closed"}</span>
      </div>
    );
  }),
}));

vi.mock("../../../app/Components/UI/AppTable/AppTable", () => ({
  default: memo(function MockAppTable(props: {
    onSelectionChanged?: (rows: Array<{ id: string; RADICADO: string }>) => void;
    onActionTriggered?: (input: { actionId: string; row: { id: string; RADICADO: string } }) => void;
    onCellClicked?: (input: { row: { id: string; RADICADO: string }; field?: string }) => void;
  }) {
    recordRender("AppTable", props as Record<string, unknown>);
    return (
      <div data-testid="mock-table">
        <button
          type="button"
          onClick={() =>
            props.onSelectionChanged?.([{ id: "924", RADICADO: "2500456700023" }])
          }
        >
          Seleccionar fila
        </button>
        <button
          type="button"
          onClick={() =>
            props.onActionTriggered?.({
              actionId: "gestionar_tramite_menu",
              row: { id: "924", RADICADO: "2500456700023" },
            })
          }
        >
          Accion fila
        </button>
        <button
          type="button"
          onClick={() =>
            props.onCellClicked?.({
              row: { id: "924", RADICADO: "2500456700023" },
              field: "RADICADO",
            })
          }
        >
          Click celda
        </button>
      </div>
    );
  }),
}));

const createTable = (): GestionCorrespondenciaTableResult => ({
  rows: [{ id: "924", RADICADO: "2500456700023" }],
  columns: [{ field: "RADICADO", headerName: "Radicado" }],
  total: 7,
  page: 1,
  pageSize: 25,
  queryState: {
    page: 1,
    pageSize: 25,
    search: "",
    structuredFilters: [],
    sortField: "fecha_inicio",
    sortDir: "desc",
    searchType: undefined,
  },
  onQueryChange: vi.fn(),
  category: undefined,
  loading: false,
  error: null,
  isEmpty: false,
  hasLoadedOnce: true,
  setCategory: vi.fn(),
  refetch: vi.fn(),
  getAllMatchingRows: vi.fn().mockResolvedValue([
    { id: "924", RADICADO: "2500456700023" },
  ]),
  getBackendExportFile: vi.fn().mockResolvedValue({
    blob: new Blob(["xlsx"], {
      type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    }),
    fileName: "gestion.xlsx",
  }),
});

describe("GestionCorrespondencia profiling evidence", () => {
  beforeEach(() => {
    resetRenderLog();
    vi.clearAllMocks();
    vi.mocked(
      workflowInboxAutocompleteHook.useWorkflowInboxAutocomplete,
    ).mockReturnValue({
      items: [{ value: "RAD-1", label: "Radicado sugerido" }],
      loading: false,
      error: null,
      setSearchText: vi.fn(),
      clear: vi.fn(),
    });
  });

  it("profiles required interactions and classifies rerender impact", () => {
    const table = createTable();
    render(
      <MemoryRouter>
        <GestionCorrespondencia table={table} />
      </MemoryRouter>,
    );

    const input = screen.getByRole("textbox", { name: "Buscar tareas workflow" });

    const typingBaseline = snapshotCounts();
    for (const value of ["r", "ra", "rad", "radi", "radic"]) {
      fireEvent.change(input, { target: { value } });
    }
    const typingDiff = diffFrom(typingBaseline);

    const clearBaseline = snapshotCounts();
    fireEvent.click(screen.getByRole("button", { name: "Limpiar" }));
    const clearDiff = diffFrom(clearBaseline);

    const selectionBaseline = snapshotCounts();
    fireEvent.click(screen.getByRole("button", { name: "Seleccionar fila" }));
    const selectionDiff = diffFrom(selectionBaseline);

    const exportBaseline = snapshotCounts();
    fireEvent.click(screen.getByRole("button", { name: "Exportar" }));
    const exportDiff = diffFrom(exportBaseline);

    const refreshBaseline = snapshotCounts();
    fireEvent.click(screen.getByRole("button", { name: "Actualizar" }));
    const refreshDiff = diffFrom(refreshBaseline);

    expect(typingDiff.AppInputSearch).toBeGreaterThanOrEqual(5);
    expect(typingDiff.AppTableExport).toBe(0);
    expect(typingDiff.AppTable).toBe(0);

    expect(clearDiff.AppInputSearch).toBeGreaterThanOrEqual(1);
    expect(clearDiff.AppTableExport).toBe(0);
    expect(clearDiff.AppTable).toBe(0);

    expect(selectionDiff.AppTableExport).toBeGreaterThanOrEqual(1);
    expect(selectionDiff.AppTable).toBe(0);

    expect(exportDiff.AppTableExport).toBeGreaterThanOrEqual(1);
    expect(exportDiff.AppTable).toBe(0);

    expect(refreshDiff.AppTableExport).toBe(0);
    expect(refreshDiff.AppTable).toBe(0);
    expect(table.refetch).toHaveBeenCalledTimes(1);
  });
});
