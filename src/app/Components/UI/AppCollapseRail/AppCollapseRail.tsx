import { LeftOutlined, RightOutlined } from "@ant-design/icons";
import type { ReactNode } from "react";
import { useId } from "react";
import { createPortal } from "react-dom";
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
  const toggleIcon = collapsed ? expandIcon : collapseIcon;
  const resolvedRailLabel = railLabel ?? title;
  const isOverlay = variant === "overlay";
  const panelNode = (
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
            icon={toggleIcon}
            className={styles.toggle}
          />
        </div>
      </div>
      <div id={resolvedId} className={styles.surface}>
        {children}
      </div>
    </aside>
  );

  return (
    <div
      className={joinClasses(styles.wrapper, className)}
      data-placement={placement}
      data-variant={variant}
    >
      {!isOverlay ? panelNode : null}
      {isOverlay && typeof document !== "undefined" ? createPortal(panelNode, document.body) : null}

      <div className={styles.rail} data-collapsed={collapsed} data-placement={placement}>
        {collapsed ? (
          <AppButton
            variant="ghost"
            size="sm"
            className={styles.railButton}
            onClick={onToggle}
            aria-label={`Mostrar ${title}`}
            icon={railIcon ?? (!isOverlay ? expandIcon : undefined)}
          >
            {isOverlay ? (
              <span className={styles.railLabel}>
                {railIcon ? <span className={styles.railLabelIcon}>{railIcon}</span> : null}
                {resolvedRailLabel}
              </span>
            ) : null}
          </AppButton>
        ) : null}
      </div>
    </div>
  );
}
