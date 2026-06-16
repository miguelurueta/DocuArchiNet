import { Dropdown, Grid } from "antd";
import type { DropdownProps, MenuProps } from "antd";
import {
  cloneElement,
  isValidElement,
  useId,
  useMemo,
  useState,
  type KeyboardEvent,
  type ReactElement,
  type ReactNode,
} from "react";
import styles from "./AppDropdown.module.css";

type AppDropdownTriggerProps = {
  children?: ReactNode;
  title?: string;
  onKeyDown?: (event: KeyboardEvent<HTMLElement>) => void;
  "aria-label"?: string;
  "aria-labelledby"?: string;
  "aria-haspopup"?: "menu";
  "aria-expanded"?: boolean;
  "aria-controls"?: string;
};

export type AppDropdownItem = {
  key: string;
  label?: ReactNode;
  type?: "item" | "divider";
  icon?: ReactNode;
  leftIcon?: ReactNode;
  danger?: boolean;
  disabled?: boolean;
  href?: string;
  onSelect?: () => void;
  children?: AppDropdownItem[];
};

export type AppDropdownPlacement =
  | "bottom"
  | "bottomLeft"
  | "bottomRight"
  | "top"
  | "topLeft"
  | "topRight";

export type AppDropdownProps = {
  trigger: ReactElement;
  items: AppDropdownItem[];
  disabled?: boolean;
  open?: boolean;
  defaultOpen?: boolean;
  onOpenChange?: (open: boolean) => void;
  placement?: AppDropdownPlacement;
  dropdownProps?: Omit<
    DropdownProps,
    "children" | "menu" | "onOpenChange" | "open" | "placement" | "trigger"
  >;
  ariaLabel?: string;
  className?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const isNonEmptyString = (value: unknown): value is string =>
  typeof value === "string" && value.trim().length > 0;

function getTextContent(node: ReactNode): string {
  if (typeof node === "string" || typeof node === "number") {
    return String(node).trim();
  }

  if (Array.isArray(node)) {
    return node.map((child) => getTextContent(child)).join("").trim();
  }

  if (isValidElement(node)) {
    return getTextContent((node.props as AppDropdownTriggerProps).children);
  }

  return "";
}

function hasAccessibleName(trigger: ReactElement<AppDropdownTriggerProps>, ariaLabel?: string) {
  if (isNonEmptyString(ariaLabel)) {
    return true;
  }

  const props = trigger.props;
  const childText = getTextContent(props.children);

  return (
    isNonEmptyString(props["aria-label"]) ||
    isNonEmptyString(props["aria-labelledby"]) ||
    isNonEmptyString(props.title) ||
    childText.length > 0
  );
}

function useMergedOpenState({
  open,
  defaultOpen = false,
  onOpenChange,
}: Pick<AppDropdownProps, "open" | "defaultOpen" | "onOpenChange">) {
  const isControlled = typeof open === "boolean";
  const [uncontrolledOpen, setUncontrolledOpen] = useState(defaultOpen);

  const currentOpen = isControlled ? open : uncontrolledOpen;

  const setOpen = (nextOpen: boolean) => {
    if (!isControlled) {
      setUncontrolledOpen(nextOpen);
    }

    onOpenChange?.(nextOpen);
  };

  return [currentOpen, setOpen] as const;
}

function renderMenuItemLabel(
  item: AppDropdownItem,
  labelClassName: string,
) {
  const itemIcon = item.leftIcon ?? item.icon;

  return item.href ? (
    <a href={item.href} onClick={(event) => event.stopPropagation()}>
      <span className={labelClassName}>
        {itemIcon ? (
          <span className={styles.itemIcon} aria-hidden="true">
            {itemIcon}
          </span>
        ) : null}
        <span>{item.label}</span>
      </span>
    </a>
  ) : (
    <span className={labelClassName}>
      {itemIcon ? (
        <span className={styles.itemIcon} aria-hidden="true">
          {itemIcon}
        </span>
      ) : null}
      <span>{item.label}</span>
    </span>
  );
}

function buildFlatMenuItems(items: AppDropdownItem[]): MenuProps["items"] {
  return items.flatMap((item) => {
    if (item.type === "divider") {
      return [{ type: "divider" as const, key: item.key }];
    }

    const currentItem = {
      key: item.key,
      danger: item.danger,
      disabled: item.disabled || Boolean(item.children?.length),
      label: renderMenuItemLabel(item, styles.childItemLabel),
      onClick:
        item.disabled || item.children?.length
          ? undefined
          : () => {
              item.onSelect?.();
            },
    };

    if (!item.children?.length) {
      return [currentItem];
    }

    return [currentItem, ...(buildFlatMenuItems(item.children) ?? [])];
  });
}

function buildMenuItems(items: AppDropdownItem[], flattenChildren = false): MenuProps["items"] {
  return items.flatMap((item) => {
    if (item.type === "divider") {
      return [{ type: "divider" as const, key: item.key }];
    }

    const currentItem = {
      key: item.key,
      danger: item.danger,
      disabled: item.disabled || (flattenChildren && Boolean(item.children?.length)),
      children:
        item.children && !flattenChildren ? buildMenuItems(item.children, false) : undefined,
      label: renderMenuItemLabel(item, styles.itemLabel),
      onClick:
        item.disabled || item.children?.length
          ? undefined
          : () => {
              item.onSelect?.();
            },
    };

    if (!flattenChildren || !item.children?.length) {
      return [currentItem];
    }

    return [currentItem, ...(buildFlatMenuItems(item.children) ?? [])];
  });
}

export function AppDropdown({
  trigger,
  items,
  disabled = false,
  open,
  defaultOpen = false,
  onOpenChange,
  placement = "bottomLeft",
  dropdownProps,
  ariaLabel,
  className,
}: AppDropdownProps) {
  const screens = Grid.useBreakpoint();
  const isMobile = !screens.md;
  const typedTrigger = trigger as ReactElement<AppDropdownTriggerProps>;
  const menuId = useId();
  const [currentOpen, setCurrentOpen] = useMergedOpenState({ open, defaultOpen, onOpenChange });

  if (!hasAccessibleName(typedTrigger, ariaLabel)) {
    throw new Error("AppDropdown trigger icon-only requiere nombre accesible.");
  }

  const menuItems = useMemo<MenuProps["items"]>(
    () => buildMenuItems(items, isMobile),
    [isMobile, items],
  );

  const triggerProps = typedTrigger.props;
  const originalOnKeyDown = triggerProps.onKeyDown;

  const enhancedTrigger = cloneElement<AppDropdownTriggerProps>(typedTrigger, {
    "aria-label": ariaLabel ?? triggerProps["aria-label"],
    "aria-haspopup": "menu",
    "aria-expanded": currentOpen,
    "aria-controls": menuId,
    onKeyDown: (event: KeyboardEvent<HTMLElement>) => {
      originalOnKeyDown?.(event);

      if (event.defaultPrevented || disabled) {
        return;
      }

      if (event.key === "Enter" || event.key === " ") {
        setCurrentOpen(true);
      }
    },
  });

  if (disabled) {
    return (
      <span
        className={joinClasses(styles.trigger, styles.triggerDisabled, className)}
        aria-disabled="true"
      >
        {enhancedTrigger}
      </span>
    );
  }

  return (
    <Dropdown
      {...dropdownProps}
      menu={{ items: menuItems, id: menuId }}
      trigger={["click"]}
      open={currentOpen}
      onOpenChange={setCurrentOpen}
      placement={placement}
    >
      <span className={joinClasses(styles.trigger, className)}>{enhancedTrigger}</span>
    </Dropdown>
  );
}
