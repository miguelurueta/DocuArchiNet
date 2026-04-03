import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import AppTableActionCellRenderer from "../renderers/AppTableActionCellRenderer";
import type { AppTableActionCellRendererParams } from "../types/dynamicUiTableAction.types";

const hookState = {
  executeAction: vi.fn(),
  buildActionPayload: vi.fn(),
  evaluateActionAvailability: vi.fn(),
  resolveActionBehavior: vi.fn(),
  resolveActionPresentation: vi.fn(),
  isExecutingAction: false,
  actionError: null,
  lastActionResult: null,
};

vi.mock("../hooks/useDynamicUiTableActions", () => ({
  useDynamicUiTableActions: () => hookState,
}));

const createParams = (
  overrides: Partial<AppTableActionCellRendererParams> = {},
): AppTableActionCellRendererParams =>
  ({
    data: {
      id: "924",
      id_tarea: 924,
      RADICADO: "2500456700023",
    },
    value: "",
    api: {
      getSelectedRows: () => [],
    },
    colDef: {
      field: "acciones",
    },
    appGridColumn: {
      field: "acciones",
      headerName: "Acciones",
      visible: true,
      sortable: false,
      filterable: false,
      isActionColumn: true,
    },
    actions: [
      {
        actionId: "gestionar_tramite",
        label: "Gestionar trámite",
        placement: "row",
        presentation: "icon_button",
        behavior: "client_event",
        request: {
          RowIdField: "id_tarea",
          PayloadFields: {
            id_tarea: "id_tarea",
          },
        },
      },
    ],
    tableId: "workflowInboxgestion",
    userClaims: ["tramites.gestionar"],
    ...overrides,
  }) as AppTableActionCellRendererParams;

describe("[SPEC:ACTUALIZACION-AG-GRID-CELL-ACTION] AppTableActionCellRenderer", () => {
  beforeEach(() => {
    hookState.executeAction.mockReset().mockResolvedValue({
      success: true,
      data: null,
      rawResponse: null,
    });
    hookState.buildActionPayload.mockReset().mockReturnValue({
      id_tarea: 924,
    });
    hookState.evaluateActionAvailability.mockReset().mockReturnValue({
      isVisible: true,
      isEnabled: true,
    });
    hookState.resolveActionBehavior.mockReset().mockReturnValue({
      kind: "client_event",
      rawValue: "client_event",
      isKnown: true,
    });
    hookState.resolveActionPresentation.mockReset().mockReturnValue({
      kind: "icon_button",
      rawValue: "icon_button",
      isKnown: true,
    });
    hookState.isExecutingAction = false;
    hookState.actionError = null;
    hookState.lastActionResult = null;
  });

  it("renders a visible action button for supported icon_button actions", () => {
    render(<AppTableActionCellRenderer {...createParams()} />);

    expect(
      screen.getByRole("button", { name: /Gestionar trámite/i }),
    ).toBeInTheDocument();
  });

  it("does not render actions that are not visible", () => {
    hookState.evaluateActionAvailability.mockReturnValue({
      isVisible: false,
      isEnabled: false,
    });

    const { container } = render(<AppTableActionCellRenderer {...createParams()} />);

    expect(container).toBeEmptyDOMElement();
  });

  it("renders disabled actions without executing them", () => {
    hookState.evaluateActionAvailability.mockReturnValue({
      isVisible: true,
      isEnabled: false,
    });

    render(<AppTableActionCellRenderer {...createParams()} />);

    const button = screen.getByRole("button", { name: /Gestionar trámite/i });
    expect(button).toBeDisabled();

    fireEvent.click(button);

    expect(hookState.executeAction).not.toHaveBeenCalled();
  });

  it("builds payload and executes the action through the shared action layer", async () => {
    render(<AppTableActionCellRenderer {...createParams()} />);

    fireEvent.click(screen.getByRole("button", { name: /Gestionar trámite/i }));

    await waitFor(() => {
      expect(hookState.buildActionPayload).toHaveBeenCalledWith(
        expect.objectContaining({
          row: expect.objectContaining({
            id: "924",
          }),
          columnKey: "acciones",
          userClaims: ["tramites.gestionar"],
        }),
        expect.objectContaining({
          actionId: "gestionar_tramite",
        }),
      );
    });

    expect(hookState.executeAction).toHaveBeenCalledWith({
      tableId: "workflowInboxgestion",
      actionId: "gestionar_tramite",
      rowId: "924",
      columnKey: "acciones",
      selectedRowIds: [],
      payload: {
        id_tarea: 924,
      },
    });
  });

  it("preserves backend order when rendering multiple actions", () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "primera",
              label: "Primera",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
            },
            {
              actionId: "segunda",
              label: "Segunda",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
            },
          ],
        })}
      />,
    );

    expect(
      screen.getAllByRole("button").map((button) => button.getAttribute("aria-label")),
    ).toEqual(["Primera", "Segunda"]);
  });

  it("renders a neutral fallback for unsupported presentations", () => {
    hookState.resolveActionPresentation.mockReturnValue({
      kind: "menu_item",
      rawValue: "menu_item",
      isKnown: true,
    });

    render(<AppTableActionCellRenderer {...createParams()} />);

    expect(screen.getByTestId("app-table-action-fallback")).toBeInTheDocument();
  });
});
