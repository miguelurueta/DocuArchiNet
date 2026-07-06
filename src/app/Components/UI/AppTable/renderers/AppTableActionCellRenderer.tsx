import {
  DeleteOutlined,
  DownloadOutlined,
  EditOutlined,
  EllipsisOutlined,
  EyeOutlined,
  MoreOutlined,
} from "@ant-design/icons";
import { useMemo, useState } from "react";
import { AppIconActionButton } from "../../AppButton";
import { AppDropdown, type AppDropdownItem } from "../../AppDropdown";
import { useDynamicUiTableActions } from "../hooks/useDynamicUiTableActions";
import type {
  AppGridCellAction,
  AppGridRow,
  AppTableRow,
  DynamicUiUnknownRecord,
} from "../types/dynamicUiTable.types";
import type { AppTableActionCellRendererParams } from "../types/dynamicUiTableAction.types";
import styles from "./AppTableActionCellRenderer.module.css";

const ACTION_ICON_MAP = {
  view: <EyeOutlined />,
  edit: <EditOutlined />,
  download: <DownloadOutlined />,
  delete: <DeleteOutlined />,
} as const;

type RenderedAction = {
  action: AppGridCellAction;
  availability: { isVisible: boolean; isEnabled: boolean };
  behavior: { kind: string; rawValue: string };
  presentation: { kind: string; rawValue: string };
  key: string;
};

type DropdownBuilderDependencies = {
  evaluateActionAvailability: ReturnType<typeof useDynamicUiTableActions>["evaluateActionAvailability"];
  resolveActionBehavior: ReturnType<typeof useDynamicUiTableActions>["resolveActionBehavior"];
  resolveActionPresentation: ReturnType<typeof useDynamicUiTableActions>["resolveActionPresentation"];
  onSelectAction: (action: AppGridCellAction, actionKey: string, behaviorKind: string) => void;
  context: {
    row?: AppGridRow;
    selectedRows: AppGridRow[];
    columnKey: string;
    userClaims?: string[];
  };
  tableId?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const isRecord = (value: unknown): value is DynamicUiUnknownRecord =>
  typeof value === "object" && value !== null && !Array.isArray(value);

const normalizeRowId = (value: unknown): string | undefined => {
  if (typeof value === "string" || typeof value === "number") {
    return String(value);
  }

  return undefined;
};

const toAppGridRow = (row: AppTableRow | undefined): AppGridRow | undefined => {
  if (!row) {
    return undefined;
  }

  const normalizedId = normalizeRowId(row.id);
  if (!normalizedId) {
    return undefined;
  }

  return {
    id: normalizedId,
    data: { ...row },
  };
};

const toSelectedAppGridRows = (selectedRows: unknown[]): AppGridRow[] =>
  selectedRows
    .filter(isRecord)
    .map((row) => toAppGridRow(row as AppTableRow))
    .filter((row): row is AppGridRow => Boolean(row));

const toSelectedRowIds = (selectedRows: AppGridRow[]): string[] =>
  selectedRows.map((row) => row.id);

const resolveMenuItemIds = (action: AppGridCellAction): string[] => {
  const rawMenuItems = action.behaviorConfig?.menuItems;

  if (!Array.isArray(rawMenuItems)) {
    return [];
  }

  return rawMenuItems
    .filter((item): item is string => typeof item === "string" && item.trim().length > 0)
    .map((item) => item.trim());
};

const resolveButtonVariant = (tone?: string) => {
  switch (tone) {
    case "danger":
      return "danger";
    case "warning":
      return "warning";
    case "success":
      return "success";
    case "secondary":
      return "secondary";
    default:
      return "ghost";
  }
};

const resolveActionIcon = (action: AppGridCellAction) => {
  const iconKey = action.icon?.trim().toLowerCase();

  if (iconKey && iconKey in ACTION_ICON_MAP) {
    return ACTION_ICON_MAP[iconKey as keyof typeof ACTION_ICON_MAP];
  }

  return <EllipsisOutlined />;
};

const buildActionKey = (actionId: string, rowId?: string) => `${actionId}:${rowId ?? "no-row"}`;

const buildMenuActionLookup = (menuActions: AppGridCellAction[] | undefined): Map<string, AppGridCellAction> =>
  new Map(
    (menuActions ?? [])
      .filter((action) => action.actionId.trim().length > 0)
      .map((action) => [action.actionId, action] as const),
  );

const resolveCatalogMenuActions = (
  action: AppGridCellAction,
  menuActionLookup: Map<string, AppGridCellAction>,
): AppGridCellAction[] =>
  resolveMenuItemIds(action)
    .map((itemId) => menuActionLookup.get(itemId))
    .filter((item): item is AppGridCellAction => Boolean(item));

const mapActionToDropdownItem = (
  action: AppGridCellAction,
  path: string[],
  dependencies: DropdownBuilderDependencies,
): AppDropdownItem | null => {
  if (action.isDivider) {
    return {
      key: [...path, "divider"].join(":"),
      type: "divider",
    };
  }

  const availability = dependencies.evaluateActionAvailability(action, dependencies.context);
  if (!availability.isVisible) {
    return null;
  }

  const presentation = dependencies.resolveActionPresentation(action);
  const behavior = dependencies.resolveActionBehavior(action);
  const itemKey = [...path, action.actionId].join(":");
  const children = (action.children ?? [])
    .map((child, index) =>
      mapActionToDropdownItem(child, [...path, action.actionId, String(index)], dependencies),
    )
    .filter((item): item is AppDropdownItem => Boolean(item));

  if (presentation.kind !== "menu_item" && children.length === 0) {
    return null;
  }

  return {
    key: itemKey,
    label: action.label,
    disabled:
      !availability.isEnabled ||
      (behavior.kind === "api_call" && !dependencies.tableId && children.length === 0),
    onSelect:
      !availability.isEnabled || children.length > 0
        ? undefined
        : () => {
            dependencies.onSelectAction(action, itemKey, behavior.kind);
          },
    children: children.length > 0 ? children : undefined,
  };
};

export default function AppTableActionCellRenderer(
  params: AppTableActionCellRendererParams,
) {
  const [activeActionKey, setActiveActionKey] = useState<string | null>(null);
  const {
    buildActionPayload,
    evaluateActionAvailability,
    executeAction,
    isExecutingAction,
    resolveActionBehavior,
    resolveActionPresentation,
  } = useDynamicUiTableActions();

  const row = toAppGridRow(params.data);
  const selectedRows = toSelectedAppGridRows(params.api?.getSelectedRows?.() ?? []);
  const actionContext = useMemo(
    () => ({
      row,
      selectedRows,
      columnKey: params.appGridColumn.field,
      userClaims: params.userClaims,
    }),
    [params.appGridColumn.field, params.userClaims, row, selectedRows],
  );

  const menuActionLookup = useMemo(
    () => buildMenuActionLookup(params.menuActions),
    [params.menuActions],
  );

  const handleActionClick = async (
    action: AppGridCellAction,
    actionKey: string,
    behaviorKind: string,
  ) => {
    if (behaviorKind === "client_event") {
      if (params.data) {
        params.onClientEvent?.({
          actionId: action.actionId,
          row: params.data,
          columnKey: params.appGridColumn.field,
        });
      }
      return;
    }

    if (!params.tableId || behaviorKind !== "api_call") {
      return;
    }

    const payload = buildActionPayload(actionContext, action);

    setActiveActionKey(actionKey);

    try {
      await executeAction({
        tableId: params.tableId,
        actionId: action.actionId,
        rowId: row?.id,
        columnKey: params.appGridColumn.field,
        selectedRowIds: toSelectedRowIds(selectedRows),
        payload,
      });
    } finally {
      setActiveActionKey(null);
    }
  };

  const renderedActions = useMemo<RenderedAction[]>(
    () =>
      params.actions.map((action) => {
        const availability = evaluateActionAvailability(action, actionContext);
        const presentation = resolveActionPresentation(action);
        const behavior = resolveActionBehavior(action);
        const key = buildActionKey(action.actionId, row?.id);

        return {
          action,
          availability,
          behavior,
          presentation,
          key,
        };
      }),
    [
      actionContext,
      evaluateActionAvailability,
      params.actions,
      resolveActionBehavior,
      resolveActionPresentation,
      row?.id,
    ],
  );

  const visibleActions = renderedActions.filter(({ availability }) => availability.isVisible);
  const hasUnsupportedVisibleActions = visibleActions.some(
    ({ presentation }) => presentation.kind !== "icon_button",
  );
  const supportedVisibleActions = visibleActions.filter(
    ({ presentation }) => presentation.kind === "icon_button",
  );

  if (supportedVisibleActions.length === 0 && !hasUnsupportedVisibleActions) {
    return null;
  }

  return (
    <div className={styles.root} data-testid="app-table-action-cell">
      {supportedVisibleActions.map(({ action, availability, behavior, key }) => {
        const resolvedMenuActions = resolveCatalogMenuActions(action, menuActionLookup);
        const dropdownItems = resolvedMenuActions
          .map((menuAction, index) =>
            mapActionToDropdownItem(menuAction, [action.actionId, String(index)], {
              evaluateActionAvailability,
              resolveActionBehavior,
              resolveActionPresentation,
              onSelectAction: (menuAction, actionKey, behaviorKind) => {
                void handleActionClick(menuAction, actionKey, behaviorKind);
              },
              context: actionContext,
              tableId: params.tableId,
            }),
          )
          .filter((item): item is AppDropdownItem => Boolean(item));

        if (behavior.kind === "client_event" && dropdownItems.length > 0) {
          return (
            <AppDropdown
              key={key}
              ariaLabel={action.label || action.actionId}
              disabled={!availability.isEnabled}
              items={dropdownItems}
              trigger={
              <AppIconActionButton
                size="sm"
                icon={<MoreOutlined />}
                variant={resolveButtonVariant(action.tone)}
                aria-label={action.label || action.actionId}
                tooltip={action.label || action.actionId}
                disabled={!availability.isEnabled}
                data-action-id={action.actionId}
                data-action-behavior={behavior.rawValue}
              />
            }
          />
        );
      }

        return (
          <AppIconActionButton
            key={key}
            size="sm"
            icon={resolveActionIcon(action)}
            variant={resolveButtonVariant(action.tone)}
            aria-label={action.label || action.actionId}
            tooltip={action.label || action.actionId}
            disabled={
              !availability.isEnabled ||
              (behavior.kind === "api_call" && !params.tableId)
            }
            loading={isExecutingAction && activeActionKey === key}
            data-action-id={action.actionId}
            data-action-behavior={behavior.rawValue}
            onClick={(event) => {
              event.preventDefault();
              event.stopPropagation();
              if (!availability.isEnabled) {
                return;
              }
              void handleActionClick(action, key, behavior.kind);
            }}
          />
        );
      })}
      {supportedVisibleActions.length === 0 && hasUnsupportedVisibleActions ? (
        <span
          className={joinClasses(styles.fallback)}
          data-testid="app-table-action-fallback"
          aria-hidden="true"
        >
          ...
        </span>
      ) : null}
    </div>
  );
}
