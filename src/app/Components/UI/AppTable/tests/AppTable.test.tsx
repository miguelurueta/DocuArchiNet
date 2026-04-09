import { act, fireEvent, render, screen } from "@testing-library/react";
import { forwardRef, useImperativeHandle } from "react";
import type { ReactNode } from "react";
import { afterAll, afterEach, beforeAll, describe, expect, test, vi } from "vitest";
import type { CellKeyDownEvent, ColDef } from "ag-grid-community";
import AppTable from "../AppTable";

const agGridReactSpy = vi.fn();
const showLoadingOverlaySpy = vi.fn();
const showNoRowsOverlaySpy = vi.fn();
const hideOverlaySpy = vi.fn();

vi.mock("antd", () => ({
  Tooltip: ({
    title,
    open,
    children,
  }: {
    title?: string;
    open?: boolean;
    children: ReactNode;
  }) => (
    <>
      {children}
      {open && title ? <span data-testid="mock-tooltip">{title}</span> : null}
    </>
  ),
}));

vi.mock("ag-grid-react", () => ({
  AgGridReact: forwardRef((props: unknown, ref) => {
    agGridReactSpy(props);
    useImperativeHandle(ref, () => ({
      api: {
        showLoadingOverlay: showLoadingOverlaySpy,
        showNoRowsOverlay: showNoRowsOverlaySpy,
        hideOverlay: hideOverlaySpy,
      },
    }));

    const typedProps = props as {
      rowData?: Row[];
      columnDefs?: Array<{
        field?: string;
        cellClass?:
          | string
          | ((params: {
              data: Row;
              value: unknown;
              colDef: { field?: string };
            }) => string | string[] | undefined);
      }>;
    };
    const row = typedProps.rowData?.[0];

    return (
      <div data-testid="mocked-grid-root">
        <div>Mocked Grid</div>
        {row && typedProps.columnDefs ? (
          <div className="ag-row">
            {typedProps.columnDefs.map((column, index) => {
              const field = column.field ?? `column-${index}`;
              const value = row[field as keyof Row];
              const resolvedCellClass =
                typeof column.cellClass === "function"
                  ? column.cellClass({
                      data: row,
                      value,
                      colDef: { field },
                    })
                  : column.cellClass;

              return (
                <div
                  key={field}
                  role="gridcell"
                  className={["ag-cell", resolvedCellClass].filter(Boolean).join(" ")}
                  data-field={field}
                  tabIndex={-1}
                >
                  {String(value ?? "")}
                </div>
              );
            })}
          </div>
        ) : null}
      </div>
    );
  }),
}));

vi.mock("../renderers/AppTableActionCellRenderer", () => ({
  default: () => <div data-testid="mock-card-actions">Mocked Card Actions</div>,
}));

let resizeObserverCallback:
  | ((entries: Array<{ contentRect: { width: number } }>) => void)
  | null = null;
const originalResizeObserver = window.ResizeObserver;

class ResizeObserverTestMock {
  constructor(callback: (entries: Array<{ contentRect: { width: number } }>) => void) {
    resizeObserverCallback = callback;
  }

  observe() {}
  unobserve() {}
  disconnect() {}
}

beforeAll(() => {
  window.ResizeObserver = ResizeObserverTestMock as unknown as typeof ResizeObserver;
});

afterAll(() => {
  window.ResizeObserver = originalResizeObserver;
});

type Row = {
  id: string;
  name: string;
};

const createCellKeyDownEvent = (
  field: string,
  target: HTMLElement,
): CellKeyDownEvent<Row> =>
  ({
    event: { key: "Enter", target } as unknown as KeyboardEvent,
    data: { id: "1", name: "Alpha" },
    colDef: { field: field as never },
    column: null,
    node: null,
    rowIndex: 0,
    rowPinned: null,
    api: null,
    context: null,
    type: "cellKeyDown",
    value: "Alpha",
  }) as unknown as CellKeyDownEvent<Row>;

const columns: ColDef<Row>[] = [
  { field: "name", headerName: "Nombre" },
];

describe("[SPEC:CREA-COMPONENTE-TABLE] AppTable", () => {
  beforeAll(() => {
    vi.useFakeTimers();
  });

  afterEach(() => {
    resizeObserverCallback = null;
    agGridReactSpy.mockClear();
    showLoadingOverlaySpy.mockClear();
    showNoRowsOverlaySpy.mockClear();
    hideOverlaySpy.mockClear();
    vi.clearAllTimers();
  });

  afterAll(() => {
    vi.useRealTimers();
  });

  test("preserva compatibilidad hacia atrás sin paginationMode", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: {
        pagination?: boolean;
        paginationPageSize?: number;
        suppressCellFocus?: boolean;
      };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(false);
    expect(lastCall.gridOptions?.paginationPageSize).toBeUndefined();
    expect(lastCall.gridOptions?.suppressCellFocus).toBe(true);
    expect(lastCall.quickFilterText).toBeUndefined();
  });

  test("renderiza el componente base", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "ready");
    expect(wrapper.className).not.toContain("gridAffordance");
    expect(screen.getByText("Mocked Grid")).toBeInTheDocument();
  });

  test("expone estado empty cuando no hay filas", () => {
    render(<AppTable rows={[]} columns={columns} />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "empty");
  });

  test("expone estado loading cuando loading es true", () => {
    render(<AppTable rows={[]} columns={columns} loading />);
    expect(screen.getByTestId("app-table-grid-skeleton")).toBeInTheDocument();
  });

  test("configura client mode con paginación nativa y page size custom", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="client"
        clientPaginationPageSize={50}
        quickFilterText="alpha"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(true);
    expect(lastCall.gridOptions?.paginationPageSize).toBe(50);
    expect(lastCall.quickFilterText).toBe("alpha");
  });

  test("usa page size default 25 en client mode cuando no se informa override", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="client"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
    };

    expect(lastCall.gridOptions?.pagination).toBe(true);
    expect(lastCall.gridOptions?.paginationPageSize).toBe(25);
  });

  test("configura none mode sin paginación y conserva quickFilter local", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="none"
        quickFilterText="alpha"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(false);
    expect(lastCall.gridOptions?.paginationPageSize).toBeUndefined();
    expect(lastCall.quickFilterText).toBe("alpha");
  });

  test("configura server mode sin paginación del grid e ignora quickFilter local", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        paginationMode="server"
        quickFilterText="alpha"
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(false);
    expect(lastCall.gridOptions?.paginationPageSize).toBeUndefined();
    expect(lastCall.quickFilterText).toBeUndefined();
  });

  test("preserva layout content por defecto", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);

    const wrapper = screen.getByTestId("app-table-grid");
    const root = wrapper.parentElement;
    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { domLayout?: string };
    };

    expect(root).toHaveAttribute("data-layout-mode", "content");
    expect(root).toHaveAttribute("data-typography", "inbox");
    expect(lastCall.gridOptions?.domLayout).toBe("autoHeight");
  });

  test("usa layout fill con domLayout normal y conserva altura estable", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        layoutMode="fill"
      />,
    );

    const wrapper = screen.getByTestId("app-table-grid");
    const root = wrapper.parentElement;
    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { domLayout?: string };
    };

    expect(root).toHaveAttribute("data-layout-mode", "fill");
    expect(lastCall.gridOptions?.domLayout).toBe("normal");
  });

  test("usa cards como presentacion alternativa sin romper el contrato base", () => {
    const cardColumns: ColDef<Row & { acciones?: string }>[] = [
      { field: "name", headerName: "Nombre" },
      {
        field: "acciones",
        headerName: "Acciones",
        cellRendererParams: {
          appGridColumn: { field: "acciones", headerName: "Acciones", visible: true, sortable: false, filterable: false },
          actions: [],
        },
      },
    ];

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={cardColumns}
        presentationMode="cards"
      />,
    );

    expect(screen.getByTestId("app-table-cards").parentElement).toHaveAttribute(
      "data-presentation-mode",
      "cards",
    );
    expect(screen.getByTestId("app-table-cards").parentElement).toHaveAttribute(
      "data-typography",
      "inbox",
    );
    expect(screen.getByTestId("app-table-card")).toBeInTheDocument();
    expect(screen.getByText("Nombre")).toBeInTheDocument();
    expect(screen.getByText("Alpha")).toBeInTheDocument();
    expect(screen.getByTestId("mock-card-actions")).toBeInTheDocument();
  });

  test("cards renderiza empty state cuando no hay filas", () => {
    render(<AppTable rows={[]} columns={columns} presentationMode="cards" />);

    expect(screen.getByText("Sin registros")).toBeInTheDocument();
    expect(screen.getByTestId("app-table-cards")).toHaveAttribute("data-overlay", "empty");
  });

  test("cards renderiza skeleton en first load sin filas", () => {
    render(<AppTable rows={[]} columns={columns} presentationMode="cards" loading />);

    expect(screen.getByTestId("app-table-card-skeleton")).toBeInTheDocument();
  });

  test("cards permite restringir y ordenar campos visibles mediante cardFields", () => {
    const cardColumns: ColDef<Row & { status: string }>[] = [
      { field: "name", headerName: "Nombre" },
      { field: "status", headerName: "Estado" },
    ];

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha", status: "Activo" }]}
        columns={cardColumns}
        presentationMode="cards"
        cardFields={["status"]}
      />,
    );

    expect(screen.queryByText("Nombre")).not.toBeInTheDocument();
    expect(screen.getByText("Estado")).toBeInTheDocument();
    expect(screen.getByText("Activo")).toBeInTheDocument();
  });

  test("activa cards automaticamente cuando el contenedor cae por debajo del umbral", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        responsivePresentation={{ enabled: true, cardsBelow: 700 }}
      />,
    );

    act(() => {
      resizeObserverCallback?.([{ contentRect: { width: 640 } }]);
    });

    expect(screen.getByTestId("app-table-cards").parentElement).toHaveAttribute(
      "data-presentation-mode",
      "cards",
    );
  });

  test("mantiene table cuando el contenedor supera el umbral responsive", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        responsivePresentation={{ enabled: true, cardsBelow: 700 }}
      />,
    );

    act(() => {
      resizeObserverCallback?.([{ contentRect: { width: 900 } }]);
    });

    expect(screen.getByTestId("app-table-grid").parentElement).toHaveAttribute(
      "data-presentation-mode",
      "table",
    );
  });

  test("presentationMode manual tiene prioridad sobre el calculo responsive", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        presentationMode="table"
        responsivePresentation={{ enabled: true, cardsBelow: 700 }}
      />,
    );

    act(() => {
      resizeObserverCallback?.([{ contentRect: { width: 320 } }]);
    });

    expect(screen.getByTestId("app-table-grid").parentElement).toHaveAttribute(
      "data-presentation-mode",
      "table",
    );
  });

  test("permite habilitar foco de celda explicitamente", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        suppressCellFocus={false}
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { suppressCellFocus?: boolean };
    };

    expect(lastCall.gridOptions?.suppressCellFocus).toBe(false);
  });

  test("sincroniza overlay cuando loading cambia despues del montaje", () => {
    const { rerender } = render(<AppTable rows={[]} columns={columns} loading />);

    expect(screen.getByTestId("app-table-grid-skeleton")).toBeInTheDocument();

    rerender(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} loading={false} />);

    expect(hideOverlaySpy).toHaveBeenCalled();
  });

  test("mantiene grid visible durante refetch cuando ya existen filas", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} loading />);

    expect(screen.getByTestId("app-table-grid")).toBeInTheDocument();
    expect(screen.queryByTestId("app-table-loading-veil")).not.toBeInTheDocument();

    act(() => {
      vi.advanceTimersByTime(140);
    });

    expect(screen.getByTestId("app-table-loading-veil")).toBeInTheDocument();
    expect(screen.queryByTestId("app-table-grid-skeleton")).not.toBeInTheDocument();
    expect(showLoadingOverlaySpy).not.toHaveBeenCalled();
    expect(hideOverlaySpy).toHaveBeenCalled();
  });

  test("mantiene cards visibles durante refetch y agrega una carga suave", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        presentationMode="cards"
        loading
      />,
    );

    expect(screen.getByTestId("app-table-cards")).toBeInTheDocument();
    expect(screen.queryByTestId("app-table-loading-veil")).not.toBeInTheDocument();

    act(() => {
      vi.advanceTimersByTime(140);
    });

    expect(screen.getByTestId("app-table-loading-veil")).toBeInTheDocument();
    expect(screen.queryByTestId("app-table-card-skeleton")).not.toBeInTheDocument();
  });

  test("ejecuta callbacks de seleccion y click", () => {
    const onRowClicked = vi.fn();
    const onCellClicked = vi.fn();
    const onSelectionChanged = vi.fn();

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        onRowClicked={onRowClicked}
        onCellClicked={onCellClicked}
        onSelectionChanged={onSelectionChanged}
      />,
    );

    expect(onRowClicked).not.toHaveBeenCalled();
    expect(onCellClicked).not.toHaveBeenCalled();
    expect(onSelectionChanged).not.toHaveBeenCalled();
  });

  test("no aplica affordance navegable por defecto", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      columnDefs?: Array<{ cellClass?: string | ((params: unknown) => string | string[] | undefined) }>;
      gridOptions?: { onCellKeyDown?: (event: CellKeyDownEvent<Row>) => void };
    };

    const dataColumn = lastCall.columnDefs?.[0];
    expect(dataColumn?.cellClass).toBeUndefined();

    const onCellKeyDown = lastCall.gridOptions?.onCellKeyDown;
    const onCellClicked = vi.fn();

    onCellKeyDown?.(createCellKeyDownEvent("name", document.createElement("div")));

    expect(onCellClicked).not.toHaveBeenCalled();
  });

  test("aplica affordance navegable solo en celdas de datos cuando rowClickAffordance esta activo", () => {
    const actionColumns = [
      { field: "name", headerName: "Nombre" },
      { field: "ag-Grid-SelectionColumn" as never, headerName: "Seleccion" },
      { field: "acciones" as never, headerName: "Acciones", cellClass: "app-table-action-cell" },
    ] as ColDef<Row>[];

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={actionColumns}
        rowClickAffordance
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      columnDefs?: Array<{ field?: string; cellClass?: string | ((params: unknown) => string | string[] | undefined) }>;
    };

    const dataColumn = lastCall.columnDefs?.find((column) => column.field === "name");
    const selectionColumn = lastCall.columnDefs?.find(
      (column) => column.field === "ag-Grid-SelectionColumn",
    );
    const actionColumn = lastCall.columnDefs?.find((column) => column.field === "acciones");

    expect(typeof dataColumn?.cellClass).toBe("function");
    expect(
      (dataColumn?.cellClass as (params: unknown) => string)({}),
    ).toContain("navigableCell");
    expect(selectionColumn?.cellClass).toBeUndefined();
    expect(actionColumn?.cellClass).toBe("app-table-action-cell");
  });

  test("rowClickAffordance habilita foco de celda por defecto para soportar teclado", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        rowClickAffordance
      />,
    );

    const wrapper = screen.getByTestId("app-table-grid");
    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { suppressCellFocus?: boolean };
    };

    expect(wrapper.className).toContain("gridAffordance");
    expect(lastCall.gridOptions?.suppressCellFocus).toBe(false);
  });

  test("no renderiza tooltip navegable por defecto", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        rowClickAffordance
      />,
    );

    fireEvent.mouseOver(screen.getByRole("gridcell", { name: "Alpha" }));

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.queryByTestId("mock-tooltip")).not.toBeInTheDocument();
  });

  test("tooltip navegable en grid solo se activa con rowClickAffordance y rowClickTooltip", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        rowClickAffordance
        rowClickTooltip="Abrir detalle"
      />,
    );

    fireEvent.mouseOver(screen.getByRole("gridcell", { name: "Alpha" }));

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.getByTestId("mock-tooltip")).toHaveTextContent("Abrir detalle");
    expect(screen.getByTestId("app-table-grid-tooltip-anchor")).toBeInTheDocument();
  });

  test("tooltip navegable no se activa en grid sin rowClickAffordance aunque exista texto", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        rowClickTooltip="Abrir detalle"
      />,
    );

    fireEvent.mouseOver(screen.getByRole("gridcell", { name: "Alpha" }));

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.queryByTestId("mock-tooltip")).not.toBeInTheDocument();
  });

  test("tooltip navegable excluye columnas de acciones y seleccion en grid", () => {
    const mixedColumns = [
      { field: "name", headerName: "Nombre" },
      { field: "ag-Grid-SelectionColumn" as never, headerName: "Seleccion" },
      { field: "acciones" as never, headerName: "Acciones", cellClass: "app-table-action-cell" },
    ] as ColDef<Row>[];

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={mixedColumns}
        rowClickAffordance
        rowClickTooltip="Abrir detalle"
      />,
    );

    const gridRoot = screen.getByTestId("mocked-grid-root");
    const actionCell = gridRoot.querySelector("[data-field='acciones']");
    const selectionCell = gridRoot.querySelector("[data-field='ag-Grid-SelectionColumn']");

    expect(actionCell).not.toBeNull();
    expect(selectionCell).not.toBeNull();

    fireEvent.mouseOver(actionCell as Element);

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.queryByTestId("mock-tooltip")).not.toBeInTheDocument();

    fireEvent.mouseOver(selectionCell as Element);

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.queryByTestId("mock-tooltip")).not.toBeInTheDocument();
  });

  test("tooltip navegable en grid reposiciona la ancla al cambiar de celda", () => {
    const twoColumnRows = [{ id: "1", name: "Alpha", status: "Activo" }] as Array<
      Row & { status: string }
    >;
    const twoColumnDefs: ColDef<Row & { status: string }>[] = [
      { field: "name", headerName: "Nombre" },
      { field: "status", headerName: "Estado" },
    ];

    render(
      <AppTable
        rows={twoColumnRows}
        columns={twoColumnDefs}
        rowClickAffordance
        rowClickTooltip="Abrir detalle"
      />,
    );

    const grid = screen.getByTestId("app-table-grid");
    const firstCell = screen.getByRole("gridcell", { name: "Alpha" });
    const secondCell = screen.getByRole("gridcell", { name: "Activo" });

    Object.defineProperty(grid, "getBoundingClientRect", {
      configurable: true,
      value: () => ({ left: 0, top: 0, width: 500, height: 300 }),
    });
    Object.defineProperty(firstCell, "getBoundingClientRect", {
      configurable: true,
      value: () => ({ left: 10, top: 20, width: 120, height: 42 }),
    });
    Object.defineProperty(secondCell, "getBoundingClientRect", {
      configurable: true,
      value: () => ({ left: 180, top: 62, width: 120, height: 42 }),
    });

    fireEvent.mouseOver(firstCell);

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.getByTestId("app-table-grid-tooltip-anchor")).toHaveStyle({
      left: "10px",
      top: "20px",
    });

    fireEvent.mouseOver(secondCell);

    expect(screen.getByTestId("app-table-grid-tooltip-anchor")).toHaveStyle({
      left: "180px",
      top: "62px",
    });
  });

  test("Enter reutiliza onCellClicked sobre celdas navegables y excluye controles internos", () => {
    const onCellClicked = vi.fn();

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        rowClickAffordance
        onCellClicked={onCellClicked}
      />,
    );

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { onCellKeyDown?: (event: CellKeyDownEvent<Row>) => void };
    };

    const onCellKeyDown = lastCall.gridOptions?.onCellKeyDown;
    const plainTarget = document.createElement("div");
    const buttonTarget = document.createElement("button");

    onCellKeyDown?.(createCellKeyDownEvent("name", plainTarget));

    onCellKeyDown?.(createCellKeyDownEvent("name", buttonTarget));

    onCellKeyDown?.(createCellKeyDownEvent("acciones", plainTarget));

    expect(onCellClicked).toHaveBeenCalledTimes(1);
    expect(onCellClicked).toHaveBeenCalledWith({
      row: { id: "1", name: "Alpha" },
      field: "name",
      value: "Alpha",
    });
  });

  test("cards reutiliza onCellClicked como accion primaria cuando rowClickAffordance esta activo", () => {
    const onCellClicked = vi.fn();

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        presentationMode="cards"
        rowClickAffordance
        onCellClicked={onCellClicked}
      />,
    );

    const card = screen.getByTestId("app-table-card");
    fireEvent.click(card);

    expect(onCellClicked).toHaveBeenCalledTimes(1);
    expect(onCellClicked).toHaveBeenCalledWith({
      row: { id: "1", name: "Alpha" },
      field: "name",
      value: "Alpha",
    });
  });

  test("cards muestran tooltip navegable cuando se configura", () => {
    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        presentationMode="cards"
        rowClickAffordance
        rowClickTooltip="Abrir detalle"
        onCellClicked={vi.fn()}
      />,
    );

    fireEvent.mouseOver(screen.getByTestId("app-table-card"));

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.getByTestId("mock-tooltip")).toHaveTextContent("Abrir detalle");
  });

  test("cards no muestran tooltip al interactuar con acciones", () => {
    const actionColumns: ColDef<Row & { acciones?: string }>[] = [
      { field: "name", headerName: "Nombre" },
      {
        field: "acciones",
        headerName: "Acciones",
        cellRendererParams: {
          appGridColumn: {
            field: "acciones",
            headerName: "Acciones",
            visible: true,
            sortable: false,
            filterable: false,
          },
          actions: [],
        },
      },
    ];

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={actionColumns}
        presentationMode="cards"
        rowClickAffordance
        rowClickTooltip="Abrir detalle"
        onCellClicked={vi.fn()}
      />,
    );

    fireEvent.mouseOver(screen.getByTestId("mock-card-actions"));

    act(() => {
      vi.advanceTimersByTime(400);
    });

    expect(screen.queryByTestId("mock-tooltip")).not.toBeInTheDocument();
  });

  test("cards ejecuta la accion primaria con Enter cuando rowClickAffordance esta activo", () => {
    const onCellClicked = vi.fn();

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={columns}
        presentationMode="cards"
        rowClickAffordance
        onCellClicked={onCellClicked}
      />,
    );

    const card = screen.getByTestId("app-table-card");
    fireEvent.keyDown(card, { key: "Enter" });

    expect(onCellClicked).toHaveBeenCalledTimes(1);
    expect(onCellClicked).toHaveBeenCalledWith({
      row: { id: "1", name: "Alpha" },
      field: "name",
      value: "Alpha",
    });
  });

  test("cards no propaga el click del area de acciones hacia la accion primaria", () => {
    const onCellClicked = vi.fn();
    const actionColumns: ColDef<Row & { acciones?: string }>[] = [
      { field: "name", headerName: "Nombre" },
      {
        field: "acciones",
        headerName: "Acciones",
        cellRendererParams: {
          appGridColumn: {
            field: "acciones",
            headerName: "Acciones",
            visible: true,
            sortable: false,
            filterable: false,
          },
          actions: [],
        },
      },
    ];

    render(
      <AppTable
        rows={[{ id: "1", name: "Alpha" }]}
        columns={actionColumns}
        presentationMode="cards"
        rowClickAffordance
        onCellClicked={onCellClicked}
      />,
    );

    fireEvent.click(screen.getByTestId("mock-card-actions"));

    expect(onCellClicked).not.toHaveBeenCalled();
  });
});
