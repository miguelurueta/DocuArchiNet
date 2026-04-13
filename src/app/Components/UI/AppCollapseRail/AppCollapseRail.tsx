import { LeftOutlined, RightOutlined } from "@ant-design/icons";
import type { ReactNode } from "react";
import { useId } from "react";
import { AppButton } from "../AppButton";
import styles from "./AppCollapseRail.module.css";

export type AppCollapseRailPlacement = "right" | "left";
export type AppCollapseRailVariant = "inline" | "overlay";

export type AppCollapseRailProps = {
  title: string;
  collapsed: boolean;
  onToggle: () => void;
  children: ReactNode;
  panelId?: string;
  placement?: AppCollapseRailPlacement;
  variant?: AppCollapseRailVariant;
  railLabel?: string;
  railIcon?: ReactNode;
  headerActions?: ReactNode;
  className?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export function AppCollapseRail({
  title,
  collapsed,
  onToggle,
  children,
  panelId,
  placement = "right",
  variant = "inline",
  railLabel,
  railIcon,
  headerActions,
  className,
}: AppCollapseRailProps) {
  const internalId = useId();
  const resolvedId = panelId ?? `${internalId}-panel`;
  const isRight = placement === "right";
  const collapseIcon = isRight ? <RightOutlined /> : <LeftOutlined />;
  const expandIcon = isRight ? <LeftOutlined /> : <RightOutlined />;
  const resolvedRailLabel = railLabel ?? title;

  return (
    <div
      className={joinClasses(styles.wrapper, className)}
      data-placement={placement}
      data-variant={variant}
    >
      <aside
        className={styles.panel}
        data-collapsed={collapsed}
        data-placement={placement}
        data-variant={variant}
        aria-label={title}
      >
        <div className={styles.header}>
          <h5 className={styles.title}>{title}</h5>
          <div className={styles.headerActions}>
            {headerActions}
            <AppButton
              variant="ghost"
              size="sm"
              onClick={onToggle}
              aria-controls={resolvedId}
              aria-expanded={!collapsed}
              aria-label={collapsed ? `Mostrar ${title}` : `Ocultar ${title}`}
              icon={collapseIcon}
              className={styles.toggle}
            />
          </div>
        </div>
        <div id={resolvedId} className={styles.surface}>
          {children}
        </div>
      </aside>

      <div className={styles.rail} data-collapsed={collapsed} data-placement={placement}>
        {collapsed ? (
          <AppButton
            variant="ghost"
            size="sm"
            className={styles.railButton}
            onClick={onToggle}
            aria-label={`Mostrar ${title}`}
            icon={railIcon ?? expandIcon}
          >
            <span className={styles.railLabel}>{resolvedRailLabel}</span>
          </AppButton>
        ) : null}
      </div>
    </div>
  );
}
