import { LeftOutlined, RightOutlined } from "@ant-design/icons";
import type { ReactNode } from "react";
import { useEffect, useId, useRef } from "react";
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
  railButtonLabel?: string;
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
  railButtonLabel,
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
  const overlayRootRef = useRef<HTMLDivElement | null>(null);
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

  useEffect(() => {
    const overlayRoot = overlayRootRef.current;
    if (!overlayRoot) return;

    if (collapsed) {
      const activeElement = typeof document !== "undefined" ? document.activeElement : null;
      if (activeElement instanceof HTMLElement && overlayRoot.contains(activeElement)) {
        activeElement.blur();
      }
      overlayRoot.setAttribute("inert", "");
    } else {
      overlayRoot.removeAttribute("inert");
    }
  }, [collapsed]);

  const overlayNode = (
    <div
      ref={(node) => {
        overlayRootRef.current = node;
      }}
      className={styles.overlayRoot}
      data-collapsed={collapsed}
    >
      {!collapsed ? (
        <div
          className={styles.backdrop}
          role="presentation"
          onClick={onToggle}
        />
      ) : null}
      {panelNode}
    </div>
  );

  return (
    <div
      className={joinClasses(styles.wrapper, className)}
      data-placement={placement}
      data-variant={variant}
    >
      {!isOverlay ? panelNode : null}
      {isOverlay && typeof document !== "undefined"
        ? createPortal(overlayNode, document.body)
        : null}

      <div className={styles.rail} data-collapsed={collapsed} data-placement={placement}>
        {collapsed ? (
          <AppButton
            variant="ghost"
            size="sm"
            className={styles.railButton}
            onClick={onToggle}
            aria-label={railButtonLabel ?? `Mostrar ${title}`}
            title={railButtonLabel ?? `Mostrar ${title}`}
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
