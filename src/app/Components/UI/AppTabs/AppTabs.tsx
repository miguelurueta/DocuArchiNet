import { Tabs } from "antd";
import type { TabsProps } from "antd";
import type { ComponentProps, ReactNode } from "react";
import styles from "./AppTabs.module.css";

type AntTabsProps = ComponentProps<typeof Tabs>;

export type AppTabsItem = {
  key: string;
  label: ReactNode;
  children: ReactNode;
  disabled?: boolean;
};

export type AppTabsOrientation = "horizontal" | "vertical";
export type AppTabsVariant = "default" | "card";

export type AppTabsProps = Omit<
  AntTabsProps,
  "items" | "activeKey" | "defaultActiveKey" | "onChange" | "type" | "tabPosition" | "tabPlacement"
> & {
  items: AppTabsItem[];
  activeKey?: string;
  defaultActiveKey?: string;
  onChange?: (activeKey: string) => void;
  orientation?: AppTabsOrientation;
  variant?: AppTabsVariant;
  fullWidth?: boolean;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const orientationToPlacement: Record<AppTabsOrientation, TabsProps["tabPlacement"]> = {
  horizontal: "top",
  vertical: "start",
};

const variantToType: Record<AppTabsVariant, TabsProps["type"]> = {
  default: "line",
  card: "card",
};

export function AppTabs({
  items,
  activeKey,
  defaultActiveKey,
  onChange,
  orientation = "horizontal",
  variant = "default",
  fullWidth = false,
  className,
  ...restProps
}: AppTabsProps) {
  return (
    <Tabs
      {...restProps}
      items={items.map((item) => ({
        key: item.key,
        label: item.label,
        children: <div className={styles.panelContent}>{item.children}</div>,
        disabled: item.disabled,
      }))}
      activeKey={activeKey}
      defaultActiveKey={defaultActiveKey}
      onChange={onChange}
      type={variantToType[variant]}
      tabPlacement={orientationToPlacement[orientation]}
      className={joinClasses(
        styles.root,
        styles[`orientation${orientation === "vertical" ? "Vertical" : "Horizontal"}`],
        styles[`variant${variant === "card" ? "Card" : "Default"}`],
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
  );
}
