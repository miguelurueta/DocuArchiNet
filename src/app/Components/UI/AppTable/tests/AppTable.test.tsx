import { act, render, screen } from "@testing-library/react";
import { forwardRef, useImperativeHandle } from "react";
import { afterAll, afterEach, beforeAll, describe, expect, test, vi } from "vitest";
import type { ColDef } from "ag-grid-community";
import AppTable from "../AppTable";

const agGridReactSpy = vi.fn();
const showLoadingOverlaySpy = vi.fn();
const showNoRowsOverlaySpy = vi.fn();
const hideOverlaySpy = vi.fn();

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
    return <div>Mocked Grid</div>;
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

const columns: ColDef<Row>[] = [
  { field: "name", headerName: "Nombre" },
];

describe("[SPEC:CREA-COMPONENTE-TABLE] AppTable", () => {
  afterEach(() => {
    resizeObserverCallback = null;
    agGridReactSpy.mockClear();
    showLoadingOverlaySpy.mockClear();
    showNoRowsOverlaySpy.mockClear();
    hideOverlaySpy.mockClear();
  });

  test("preserva compatibilidad hacia atrás sin paginationMode", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);

    const lastCall = agGridReactSpy.mock.calls.at(-1)?.[0] as {
      gridOptions?: { pagination?: boolean; paginationPageSize?: number };
      quickFilterText?: string;
    };

    expect(lastCall.gridOptions?.pagination).toBe(false);
    expect(lastCall.gridOptions?.paginationPageSize).toBeUndefined();
    expect(lastCall.quickFilterText).toBeUndefined();
  });

  test("renderiza el componente base", () => {
    render(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "ready");
    expect(screen.getByText("Mocked Grid")).toBeInTheDocument();
  });

  test("expone estado empty cuando no hay filas", () => {
    render(<AppTable rows={[]} columns={columns} />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "empty");
  });

  test("expone estado loading cuando loading es true", () => {
    render(<AppTable rows={[]} columns={columns} loading />);
    const wrapper = screen.getByTestId("app-table-grid");
    expect(wrapper).toHaveAttribute("data-overlay", "loading");
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
    const cardColumns: ColDef<Row>[] = [
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

  test("sincroniza overlay cuando loading cambia despues del montaje", () => {
    const { rerender } = render(<AppTable rows={[]} columns={columns} loading />);

    expect(showLoadingOverlaySpy).toHaveBeenCalled();

    rerender(<AppTable rows={[{ id: "1", name: "Alpha" }]} columns={columns} loading={false} />);

    expect(hideOverlaySpy).toHaveBeenCalled();
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
});
