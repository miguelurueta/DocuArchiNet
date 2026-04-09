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
  const [overflowCount, setOverflowCount] = useState(0);

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

  const moreLabel = useMemo(
    () => (
      <span className={styles.moreLabel}>
        <span>Mas</span>
        {overflowCount > 0 ? (
          <span className={styles.moreCount}>+{overflowCount}</span>
        ) : null}
      </span>
    ),
    [overflowCount],
  );

  const resolvedMore = useMemo(
    () => ({
      ...more,
      trigger: "hover",
      icon: moreLabel,
    }),
    [more, moreLabel],
  );

  const measureOverflow = useCallback(() => {
    if (orientation === "vertical") {
      setOverflowCount(0);
      return;
    }
    const wrapper = wrapperRef.current;
    if (!wrapper) return;
    const navList = wrapper.querySelector<HTMLElement>(".ant-tabs-nav-list");
    if (!navList) return;
    const rect = navList.getBoundingClientRect();
    if (rect.width === 0) {
      setOverflowCount(0);
      return;
    }
    const tabs = Array.from(navList.querySelectorAll<HTMLElement>(".ant-tabs-tab"));
    if (tabs.length === 0) {
      setOverflowCount(0);
      return;
    }
    const hiddenTabs = tabs.filter((tab) => tab.offsetParent === null);
    setOverflowCount(hiddenTabs.length);
  }, [orientation]);

  useEffect(() => {
    if (typeof window === "undefined") return undefined;
    const frame = window.requestAnimationFrame(() => measureOverflow());
    return () => window.cancelAnimationFrame(frame);
  }, [measureOverflow, mappedItems, size, variant, orientation, fullWidth]);

  useEffect(() => {
    if (typeof ResizeObserver === "undefined") return;
    const wrapper = wrapperRef.current;
    if (!wrapper) return;
    const observer = new ResizeObserver(() => measureOverflow());
    observer.observe(wrapper);
    const navList = wrapper.querySelector<HTMLElement>(".ant-tabs-nav-list");
    if (navList) observer.observe(navList);
    return () => observer.disconnect();
  }, [measureOverflow]);

  return (
    <div ref={wrapperRef} className={styles.wrapper} role="tablist">
      <Tabs
        {...restProps}
        items={mappedItems}
        activeKey={effectiveActiveKey}
        onChange={handleChange}
        type={variantToType[variant]}
        tabPosition={resolvedTabPosition}
        more={resolvedMore}
        className={joinClasses(
          "customTabs",
          styles.root,
          styles.customTabs,
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
