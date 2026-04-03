import {
  EditOutlined,
  EllipsisOutlined,
  EyeOutlined,
  DownloadOutlined,
  DeleteOutlined,
} from "@ant-design/icons";
import { useMemo, useState } from "react";
import { AppButton } from "../../AppButton";
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

const humanizeMenuItemLabel = (value: string): string =>
  value
    .split("_")
    .filter((segment) => segment.trim().length > 0)
    .map((segment) => segment.charAt(0).toUpperCase() + segment.slice(1).toLowerCase())
    .join(" ");

const resolveMenuItemIds = (action: AppGridCellAction): string[] => {
  const rawMenuItems = action.behaviorConfig?.menuItems;

  if (!Array.isArray(rawMenuItems)) {
    return [];
  }

  return rawMenuItems
    .filter((item): item is string => typeof item === "string" && item.trim().length > 0)
    .map((item) => item.trim());
};

const buildDropdownItems = (action: AppGridCellAction): AppDropdownItem[] =>
  resolveMenuItemIds(action).map((itemId) => ({
    key: `${action.actionId}:${itemId}`,
    label: humanizeMenuItemLabel(itemId),
    disabled: true,
  }));

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

  const renderedActions = useMemo(
    () =>
      params.actions.map((action) => {
        const availability = evaluateActionAvailability(action, {
          row,
          selectedRows,
          columnKey: params.appGridColumn.field,
          userClaims: params.userClaims,
        });
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
      evaluateActionAvailability,
      params.actions,
      params.appGridColumn.field,
      params.userClaims,
      resolveActionBehavior,
      resolveActionPresentation,
      row,
      selectedRows,
    ],
  );

  const visibleActions = renderedActions.filter(({ availability }) => availability.isVisible);
  const hasUnsupportedVisibleActions = visibleActions.some(
    ({ presentation }) => presentation.kind !== "icon_button",
  );
  const supportedVisibleActions = visibleActions.filter(
    ({ presentation }) => presentation.kind === "icon_button",
  );

  const handleActionClick = async (
    action: AppGridCellAction,
    actionKey: string,
    behaviorKind: string,
  ) => {
    if (!params.tableId) {
      return;
    }

    if (behaviorKind !== "api_call") {
      return;
    }

    const context = {
      row,
      selectedRows,
      columnKey: params.appGridColumn.field,
      userClaims: params.userClaims,
    };
    const payload = buildActionPayload(context, action);

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

  if (supportedVisibleActions.length === 0 && !hasUnsupportedVisibleActions) {
    return null;
  }

  return (
    <div className={styles.root} data-testid="app-table-action-cell">
      {supportedVisibleActions.map(({ action, availability, behavior, key }) => {
        const hasMenuItems = resolveMenuItemIds(action).length > 0;

        if (behavior.kind === "client_event" && hasMenuItems) {
          return (
            <AppDropdown
              key={key}
              ariaLabel={action.label || action.actionId}
              disabled={!availability.isEnabled}
              items={buildDropdownItems(action)}
              trigger={
                <AppButton
                  size="sm"
                  icon={<EllipsisOutlined />}
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
          <AppButton
            key={key}
            size="sm"
            icon={resolveActionIcon(action)}
            variant={resolveButtonVariant(action.tone)}
            aria-label={action.label || action.actionId}
            tooltip={action.label || action.actionId}
            disabled={!availability.isEnabled || !params.tableId}
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
