import { Badge, Tabs } from "antd";
import type { TabsProps } from "antd";
import type { ComponentProps, ReactNode } from "react";
import { useCallback, useContext, useEffect, useMemo, useRef, useState } from "react";
import {
  UNSAFE_LocationContext as LocationContext,
  UNSAFE_NavigationContext as NavigationContext,
} from "react-router-dom";
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
  "items"
  | "activeKey"
  | "defaultActiveKey"
  | "onChange"
  | "tabPosition"
  | "tabPlacement"
  | "type"
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
  tabPlacement?: TabsProps["tabPlacement"];
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

export function mapToAntdItems(
  items: AppTabItem[],
  options?: { lazy?: boolean; visibleKeys?: Set<string> },
): TabsProps["items"] {
  return items.map((item) => ({
    key: item.key,
    label: buildLabel(item),
    children:
      options?.lazy && options.visibleKeys && !options.visibleKeys.has(item.key)
        ? null
        : <div className={styles.panelContent}>{item.children}</div>,
    disabled: item.disabled,
  }));
}

const orientationToPlacement: Record<AppTabsOrientation, TabsProps["tabPlacement"]> = {
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
  syncWithRouter = false,
  lazy = false,
  onTabVisible,
  tabPosition,
  tabPlacement,
  orientation = "horizontal",
  fullWidth = false,
  className,
  ...restProps
}: AppTabsProps) {
  const isControlled = activeKey !== undefined;
  const locationContext = useContext(LocationContext);
  const navigationContext = useContext(NavigationContext);
  const location = locationContext?.location;
  const navigator = navigationContext?.navigator;
  const hasRouter = Boolean(location && navigator);
  const [internalActiveKey, setInternalActiveKey] = useState(() => {
    if (defaultActiveKey) {
      const defaultItem = items.find((item) => item.key === defaultActiveKey);
      if (defaultItem && !defaultItem.disabled) return defaultActiveKey;
    }
    return getFirstEnabledKey(items);
  });
  const resolveRouterKey = useCallback(() => {
    if (!location) return undefined;
    const params = new URLSearchParams(location.search);
    const queryKey = params.get("tab")?.trim();
    if (queryKey) return queryKey;
    const segments = location.pathname.split("/").filter(Boolean);
    return segments[segments.length - 1];
  }, [location]);
  const routerKeyRaw = syncWithRouter && hasRouter ? resolveRouterKey() : undefined;
  const routerKey = routerKeyRaw && items.some((item) => item.key === routerKeyRaw)
    ? routerKeyRaw
    : undefined;
  const fallbackKey = getFirstEnabledKey(items);
  const effectiveActiveKey =
    syncWithRouter && hasRouter ? routerKey ?? fallbackKey : isControlled ? activeKey : internalActiveKey;
  const wrapperRef = useRef<HTMLDivElement | null>(null);
  const [overflowCount, setOverflowCount] = useState(0);
  const [visibleKeys, setVisibleKeys] = useState<Set<string>>(
    () => new Set(effectiveActiveKey ? [effectiveActiveKey] : []),
  );
  const lastVisibleKeyRef = useRef<string | undefined>(undefined);

  useEffect(() => {
    if (isControlled || (syncWithRouter && hasRouter)) return;
    const current = items.find((item) => item.key === internalActiveKey);
    if (!current || current.disabled) {
      setInternalActiveKey(getFirstEnabledKey(items));
    }
  }, [internalActiveKey, isControlled, items, hasRouter, syncWithRouter]);

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

      if (syncWithRouter && hasRouter && location && navigator) {
        const params = new URLSearchParams(location.search);
        const hasQuery = params.has("tab");
        const segments = location.pathname.split("/").filter(Boolean);
        const lastSegment = segments[segments.length - 1];
        const hasPathKey = items.some((item) => item.key === lastSegment);

        if (hasQuery || !hasPathKey) {
          params.set("tab", nextKey);
          navigator.push(`${location.pathname}?${params.toString()}`);
        } else {
          const nextSegments = [...segments.slice(0, -1), nextKey];
          navigator.push(`/${nextSegments.join("/")}${location.search}`);
        }
      }
    },
    [
      beforeChange,
      effectiveActiveKey,
      isControlled,
      items,
      onChange,
      syncWithRouter,
      hasRouter,
      location,
      navigator,
    ],
  );

  const mappedItems = useMemo(
    () => mapToAntdItems(items, { lazy, visibleKeys }),
    [items, lazy, visibleKeys],
  );

  const resolvedTabPlacement =
    tabPlacement ?? tabPosition ?? orientationToPlacement[orientation];

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

  useEffect(() => {
    if (!syncWithRouter || !hasRouter || !location || !navigator) return;
    if (routerKeyRaw && !routerKey && fallbackKey) {
      const params = new URLSearchParams(location.search);
      const hasQuery = params.has("tab");
      if (hasQuery) {
        params.set("tab", fallbackKey);
        navigator.replace(`${location.pathname}?${params.toString()}`);
        return;
      }
      const segments = location.pathname.split("/").filter(Boolean);
      if (segments.length > 0) {
        const nextSegments = [...segments.slice(0, -1), fallbackKey];
        navigator.replace(`/${nextSegments.join("/")}${location.search}`);
        return;
      }
      params.set("tab", fallbackKey);
      navigator.replace(`${location.pathname}?${params.toString()}`);
    }
  }, [fallbackKey, hasRouter, location, navigator, routerKey, routerKeyRaw, syncWithRouter]);

  useEffect(() => {
    if (!effectiveActiveKey) return;
    if (lazy) {
      setVisibleKeys((prev) => {
        if (prev.has(effectiveActiveKey)) return prev;
        const next = new Set(prev);
        next.add(effectiveActiveKey);
        return next;
      });
    }
    if (lastVisibleKeyRef.current !== effectiveActiveKey) {
      onTabVisible?.(effectiveActiveKey);
      lastVisibleKeyRef.current = effectiveActiveKey;
    }
  }, [effectiveActiveKey, lazy, onTabVisible]);

  return (
    <div ref={wrapperRef} className={styles.wrapper} role="tablist">
      <Tabs
        {...restProps}
        items={mappedItems}
        activeKey={effectiveActiveKey}
        onChange={handleChange}
        type={variantToType[variant]}
        tabPlacement={resolvedTabPlacement}
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
