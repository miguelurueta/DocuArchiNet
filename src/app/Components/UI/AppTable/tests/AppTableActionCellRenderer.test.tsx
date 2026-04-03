import { fireEvent, render, screen, waitFor } from "@testing-library/react";
import type { ReactElement, ReactNode } from "react";
import { beforeEach, describe, expect, it, vi } from "vitest";
import AppTableActionCellRenderer from "../renderers/AppTableActionCellRenderer";
import type { AppDropdownItem } from "../../AppDropdown";
import type { AppGridCellAction } from "../types/dynamicUiTable.types";
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

const renderItems = (items: AppDropdownItem[]) =>
  items.map((item) => {
    if (item.type === "divider") {
      return <li key={item.key} data-testid="mock-app-dropdown-divider" />;
    }

    return (
      <li key={item.key}>
        <button
          type="button"
          data-testid="mock-app-dropdown-item"
          data-disabled={item.disabled ? "true" : "false"}
          onClick={() => item.onSelect?.()}
        >
          {item.label}
        </button>
        {item.children?.length ? <ul>{renderItems(item.children)}</ul> : null}
      </li>
    );
  });

vi.mock("../../AppDropdown", () => ({
  AppDropdown: ({
    trigger,
    items,
    disabled,
    ariaLabel,
  }: {
    trigger: ReactElement;
    items: AppDropdownItem[];
    disabled?: boolean;
    ariaLabel?: string;
  }) => (
    <div data-testid="mock-app-dropdown" data-disabled={disabled ? "true" : "false"}>
      {trigger}
      <div data-testid="mock-app-dropdown-label">{ariaLabel}</div>
      <ul>{renderItems(items)}</ul>
    </div>
  ),
}));

const createMenuAction = (overrides: Partial<AppGridCellAction> = {}): AppGridCellAction => ({
  actionId: "reasignar_tramite",
  label: "Reasignar trámite",
  placement: "row",
  presentation: "menu_item",
  behavior: "api_call",
  request: {
    RowIdField: "id_tarea",
    PayloadFields: {
      id_tarea: "id_tarea",
    },
  },
  ...overrides,
});

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
    menuActions: [],
    tableId: "workflowInboxgestion",
    userClaims: ["tramites.gestionar"],
    ...overrides,
  }) as AppTableActionCellRendererParams;

describe("[SPEC:ACTUALIZACION-AG-GRID-CELL-ACTION-MENU-CHILDREN] AppTableActionCellRenderer", () => {
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
    hookState.resolveActionBehavior.mockReset().mockImplementation((action: AppGridCellAction) => ({
      kind: action.behavior,
      rawValue: action.behavior,
      isKnown: true,
    }));
    hookState.resolveActionPresentation.mockReset().mockImplementation((action: AppGridCellAction) => ({
      kind: action.presentation,
      rawValue: action.presentation,
      isKnown: true,
    }));
    hookState.isExecutingAction = false;
    hookState.actionError = null;
    hookState.lastActionResult = null;
  });

  it("renders a visible action button for supported icon_button actions", () => {
    render(<AppTableActionCellRenderer {...createParams()} />);

    expect(screen.getByRole("button", { name: /Gestionar trámite/i })).toBeInTheDocument();
  });

  it("does not render actions that are not visible", () => {
    hookState.evaluateActionAvailability.mockReturnValue({
      isVisible: false,
      isEnabled: false,
    });

    const { container } = render(<AppTableActionCellRenderer {...createParams()} />);

    expect(container).toBeEmptyDOMElement();
  });

  it("builds payload and executes api_call actions through the shared action layer", async () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "gestionar_tramite",
              label: "Gestionar trámite",
              placement: "row",
              presentation: "icon_button",
              behavior: "api_call",
              request: {
                RowIdField: "id_tarea",
                PayloadFields: {
                  id_tarea: "id_tarea",
                },
              },
            },
          ],
        })}
      />,
    );

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

  it("resolves menuItems against MenuActions and renders backend labels", () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "gestionar_tramite",
              label: "Gestionar trámite",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
              behaviorConfig: {
                menuItems: ["reasignar_tramite", "archivar_tramite"],
              },
            },
          ],
          menuActions: [
            createMenuAction(),
            createMenuAction({
              actionId: "archivar_tramite",
              label: "Archivar trámite",
            }),
          ],
        })}
      />,
    );

    expect(screen.getByTestId("mock-app-dropdown")).toBeInTheDocument();
    expect(screen.getByText("Reasignar trámite")).toBeInTheDocument();
    expect(screen.getByText("Archivar trámite")).toBeInTheDocument();
  });

  it("ignores unresolved menu ids without breaking the dropdown", () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "gestionar_tramite",
              label: "Gestionar trámite",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
              behaviorConfig: {
                menuItems: ["reasignar_tramite", "missing_action"],
              },
            },
          ],
          menuActions: [createMenuAction()],
        })}
      />,
    );

    expect(screen.getAllByTestId("mock-app-dropdown-item")).toHaveLength(1);
    expect(screen.getByText("Reasignar trámite")).toBeInTheDocument();
  });

  it("renders children recursively as nested dropdown items", () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "gestionar_tramite",
              label: "Gestionar trámite",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
              behaviorConfig: {
                menuItems: ["menu_padre"],
              },
            },
          ],
          menuActions: [
            createMenuAction({
              actionId: "menu_padre",
              label: "Más acciones",
              children: [
                createMenuAction({
                  actionId: "reasignar_tramite",
                  label: "Reasignar trámite",
                }),
              ],
            }),
          ],
        })}
      />,
    );

    expect(screen.getByText("Más acciones")).toBeInTheDocument();
    expect(screen.getByText("Reasignar trámite")).toBeInTheDocument();
  });

  it("renders divider items as non-executable separators", async () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "gestionar_tramite",
              label: "Gestionar trámite",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
              behaviorConfig: {
                menuItems: ["reasignar_tramite", "divider_1", "archivar_tramite"],
              },
            },
          ],
          menuActions: [
            createMenuAction(),
            {
              actionId: "divider_1",
              label: "",
              placement: "row",
              presentation: "menu_item",
              behavior: "noop",
              isDivider: true,
            },
            createMenuAction({
              actionId: "archivar_tramite",
              label: "Archivar trámite",
            }),
          ],
        })}
      />,
    );

    expect(screen.getByTestId("mock-app-dropdown-divider")).toBeInTheDocument();
    fireEvent.click(screen.getByText("Reasignar trámite"));
    await waitFor(() => {
      expect(hookState.executeAction).toHaveBeenCalledTimes(1);
    });
  });

  it("keeps the direct action fallback when menuItems has no valid resolutions", () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "gestionar_tramite",
              label: "Gestionar trámite",
              placement: "row",
              presentation: "icon_button",
              behavior: "api_call",
              behaviorConfig: {
                menuItems: ["missing_action"],
              },
            },
          ],
        })}
      />,
    );

    expect(screen.queryByTestId("mock-app-dropdown")).not.toBeInTheDocument();
    expect(screen.getByRole("button", { name: /Gestionar trámite/i })).toBeInTheDocument();
  });

  it("does not execute divider or invalid menu items", () => {
    render(
      <AppTableActionCellRenderer
        {...createParams({
          actions: [
            {
              actionId: "gestionar_tramite",
              label: "Gestionar trámite",
              placement: "row",
              presentation: "icon_button",
              behavior: "client_event",
              behaviorConfig: {
                menuItems: ["divider_1"],
              },
            },
          ],
          menuActions: [
            {
              actionId: "divider_1",
              label: "",
              placement: "row",
              presentation: "menu_item",
              behavior: "noop",
              isDivider: true,
            },
          ],
        })}
      />,
    );

    expect(screen.queryAllByTestId("mock-app-dropdown-item")).toHaveLength(0);
    expect(hookState.executeAction).not.toHaveBeenCalled();
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
