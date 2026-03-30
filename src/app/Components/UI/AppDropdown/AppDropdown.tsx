import { Dropdown } from "antd";
import type { MenuProps } from "antd";
import {
  Children,
  cloneElement,
  isValidElement,
  useEffect,
  useId,
  useMemo,
  useState,
  type KeyboardEvent,
  type ReactElement,
  type ReactNode,
} from "react";
import styles from "./AppDropdown.module.css";

export type AppDropdownItem = {
  key: string;
  label: ReactNode;
  icon?: ReactNode;
  danger?: boolean;
  disabled?: boolean;
  href?: string;
  onSelect?: () => void;
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
    return getTextContent(node.props.children);
  }

  return "";
}

function hasAccessibleName(trigger: ReactElement, ariaLabel?: string) {
  if (isNonEmptyString(ariaLabel)) {
    return true;
  }

  const props = trigger.props as Record<string, unknown>;
  const childText = getTextContent(props.children);

  return (
    isNonEmptyString(props["aria-label"]) ||
    isNonEmptyString(props["aria-labelledby"]) ||
    isNonEmptyString(props.title) ||
    childText.length > 0
  );
}

function mergeOpenState({
  open,
  defaultOpen = false,
  onOpenChange,
}: Pick<AppDropdownProps, "open" | "defaultOpen" | "onOpenChange">) {
  const isControlled = typeof open === "boolean";
  const [uncontrolledOpen, setUncontrolledOpen] = useState(defaultOpen);

  useEffect(() => {
    if (!isControlled) {
      setUncontrolledOpen(defaultOpen);
    }
  }, [defaultOpen, isControlled]);

  const currentOpen = isControlled ? open : uncontrolledOpen;

  const setOpen = (nextOpen: boolean) => {
    if (!isControlled) {
      setUncontrolledOpen(nextOpen);
    }

    onOpenChange?.(nextOpen);
  };

  return [currentOpen, setOpen] as const;
}

export function AppDropdown({
  trigger,
  items,
  disabled = false,
  open,
  defaultOpen = false,
  onOpenChange,
  placement = "bottomLeft",
  ariaLabel,
  className,
}: AppDropdownProps) {
  const menuId = useId();
  const [currentOpen, setCurrentOpen] = mergeOpenState({ open, defaultOpen, onOpenChange });

  if (!hasAccessibleName(trigger, ariaLabel)) {
    throw new Error("AppDropdown trigger icon-only requiere nombre accesible.");
  }

  const menuItems = useMemo<MenuProps["items"]>(
    () =>
      items.map((item) => ({
        key: item.key,
        danger: item.danger,
        disabled: item.disabled,
        label: item.href ? (
          <a href={item.href} onClick={(event) => event.stopPropagation()}>
            <span className={styles.itemLabel}>
              {item.icon ? (
                <span className={styles.itemIcon} aria-hidden="true">
                  {item.icon}
                </span>
              ) : null}
              <span>{item.label}</span>
            </span>
          </a>
        ) : (
          <span className={styles.itemLabel}>
            {item.icon ? (
              <span className={styles.itemIcon} aria-hidden="true">
                {item.icon}
              </span>
            ) : null}
            <span>{item.label}</span>
          </span>
        ),
        onClick: item.disabled
          ? undefined
          : () => {
              item.onSelect?.();
            },
      })),
    [items],
  );

  const triggerProps = trigger.props as Record<string, unknown>;
  const originalOnKeyDown = triggerProps.onKeyDown as ((event: KeyboardEvent<HTMLElement>) => void) | undefined;

  const enhancedTrigger = cloneElement(trigger, {
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
