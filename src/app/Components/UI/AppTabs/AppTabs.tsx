import { Badge, Tabs } from "antd";
import type { TabsProps } from "antd";
import type { ComponentProps, ReactNode } from "react";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import styles from "./AppTabs.module.css";

type AntTabsProps = ComponentProps<typeof Tabs>;

type AppTabsSize = "sm" | "md" | "lg";

type AppTabsVariant = "default" | "card" | "underline" | "pills";

type AppTabsOrientation = "horizontal" | "vertical";

export type AppTabItem = {
  key: string;
  label: ReactNode;
  children: ReactNode;
  icon?: ReactNode;
  badge?: number;
  disabled?: boolean;
};

export type AppTabsProps = Omit<
  AntTabsProps,
  "items" | "activeKey" | "defaultActiveKey" | "onChange" | "tabPosition" | "type"
> & {
  items: AppTabItem[];
  activeKey?: string;
  defaultActiveKey?: string;
  beforeChange?: (nextKey: string, currentKey?: string) => boolean | Promise<boolean>;
  variant?: AppTabsVariant;
  size?: AppTabsSize;
  more?: TabsProps["more"];
  syncWithRouter?: boolean;
  lazy?: boolean;
  onTabVisible?: (key: string) => void;
  onChange?: (activeKey: string) => void;
  tabPosition?: TabsProps["tabPosition"];
  orientation?: AppTabsOrientation;
  fullWidth?: boolean;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const getFirstEnabledKey = (items: AppTabItem[]) =>
  items.find((item) => !item.disabled)?.key ?? items[0]?.key;

const buildLabel = (item: AppTabItem) => (
  <span className={styles.label}>
    {item.icon ? <span className={styles.labelIcon}>{item.icon}</span> : null}
    <span className={styles.labelText}>{item.label}</span>
    {typeof item.badge === "number" ? (
      <Badge className={styles.labelBadge} count={item.badge} size="small" />
    ) : null}
  </span>
);

export function mapToAntdItems(items: AppTabItem[]): TabsProps["items"] {
  return items.map((item) => ({
    key: item.key,
    label: buildLabel(item),
    children: <div className={styles.panelContent}>{item.children}</div>,
    disabled: item.disabled,
  }));
}

const orientationToPlacement: Record<AppTabsOrientation, TabsProps["tabPosition"]> = {
  horizontal: "top",
  vertical: "left",
};

const variantToType: Record<AppTabsVariant, TabsProps["type"]> = {
  default: "line",
  card: "card",
  underline: "line",
  pills: "line",
};

export function AppTabs({
  items,
  activeKey,
  defaultActiveKey,
  onChange,
  beforeChange,
  variant = "default",
  size = "md",
  more,
  tabPosition,
  orientation = "horizontal",
  fullWidth = false,
  className,
  ...restProps
}: AppTabsProps) {
  const isControlled = activeKey !== undefined;
  const [internalActiveKey, setInternalActiveKey] = useState(() => {
    if (defaultActiveKey) {
      const defaultItem = items.find((item) => item.key === defaultActiveKey);
      if (defaultItem && !defaultItem.disabled) return defaultActiveKey;
    }
    return getFirstEnabledKey(items);
  });
  const effectiveActiveKey = isControlled ? activeKey : internalActiveKey;
  const wrapperRef = useRef<HTMLDivElement | null>(null);

  useEffect(() => {
    if (isControlled) return;
    const current = items.find((item) => item.key === internalActiveKey);
    if (!current || current.disabled) {
      setInternalActiveKey(getFirstEnabledKey(items));
    }
  }, [internalActiveKey, isControlled, items]);

  useEffect(() => {
    if (!effectiveActiveKey) return;
    const current = items.find((item) => item.key === effectiveActiveKey);
    if (current?.disabled) return;
    const tab = wrapperRef.current?.querySelector<HTMLElement>(
      `[data-node-key="${effectiveActiveKey}"] .ant-tabs-tab-btn`,
    );
    tab?.focus();
  }, [effectiveActiveKey, items]);

  const handleChange = useCallback(
    async (nextKey: string) => {
      const nextItem = items.find((item) => item.key === nextKey);
      if (nextItem?.disabled) return;

      const allowed = await Promise.resolve(
        beforeChange ? beforeChange(nextKey, effectiveActiveKey) : true,
      );
      if (!allowed) return;

      if (!isControlled) {
        setInternalActiveKey(nextKey);
      }
      onChange?.(nextKey);
    },
    [beforeChange, effectiveActiveKey, isControlled, items, onChange],
  );

  const mappedItems = useMemo(() => mapToAntdItems(items), [items]);

  const resolvedTabPosition = tabPosition ?? orientationToPlacement[orientation];

  return (
    <div ref={wrapperRef} className={styles.wrapper} role="tablist">
      <Tabs
        {...restProps}
        items={mappedItems}
        activeKey={effectiveActiveKey}
        onChange={handleChange}
        type={variantToType[variant]}
        tabPosition={resolvedTabPosition}
        more={more}
        className={joinClasses(
          styles.root,
          styles[`orientation${orientation === "vertical" ? "Vertical" : "Horizontal"}`],
          styles[`variant${variant.charAt(0).toUpperCase()}${variant.slice(1)}`],
          styles[`size${size.toUpperCase()}`],
          fullWidth && styles.fullWidth,
          className,
        )}
        classNames={{
          root: styles.semanticRoot,
          header: styles.header,
          item: styles.item,
          content: styles.content,
        }}
      />
    </div>
  );
}
