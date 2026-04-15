import type { ReactNode } from "react";
import { useEffect, useId, useState } from "react";
import { Link } from "react-router-dom";
import { AppButton } from "../AppButton";
import type { AppButtonProps, AppButtonSize, AppButtonVariant } from "../AppButton";
import { AppDropdown } from "../AppDropdown";
import styles from "./AppToolbar.module.css";

export type AppToolbarBreakpoint = "sm" | "md";

export type AppToolbarBreadcrumbItem = {
  key?: string;
  label: ReactNode;
  to?: string;
  href?: string;
  onClick?: () => void;
  current?: boolean;
};

export type AppToolbarAction = {
  key: string;
  label: ReactNode;
  onClick?: AppButtonProps["onClick"];
  href?: string;
  variant?: AppButtonVariant;
  size?: AppButtonSize;
  icon?: ReactNode;
  disabled?: boolean;
  loading?: boolean;
  tooltip?: string;
  ariaLabel?: string;
};

export type AppToolbarProps = {
  title?: ReactNode;
  subtitle?: ReactNode;
  description?: ReactNode;
  breadcrumbs?: AppToolbarBreadcrumbItem[];
  primaryAction?: AppToolbarAction;
  actions?: AppToolbarAction[];
  secondaryActions?: AppToolbarAction[];
  extra?: ReactNode;
  children?: ReactNode;
  actionContent?: ReactNode;
  className?: string;
  collapseBreakpoint?: AppToolbarBreakpoint;
  maxVisibleSecondaryActions?: number;
  overflowLabel?: string;
  sticky?: boolean;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const MEDIA_QUERY_MAP: Record<AppToolbarBreakpoint, string> = {
  sm: "(max-width: 640px)",
  md: "(max-width: 768px)",
};

function useMediaQuery(query: string) {
  const getMatches = () =>
    typeof window !== "undefined" ? window.matchMedia(query).matches : false;

  const [matches, setMatches] = useState(getMatches);

  useEffect(() => {
    const mediaQueryList = window.matchMedia(query);
    const update = (event: MediaQueryListEvent) => {
      setMatches(event.matches);
    };

    setMatches(mediaQueryList.matches);
    mediaQueryList.addEventListener("change", update);

    return () => {
      mediaQueryList.removeEventListener("change", update);
    };
  }, [query]);

  return matches;
}

function renderBreadcrumb(item: AppToolbarBreadcrumbItem) {
  if (item.to) {
    return (
      <Link to={item.to} onClick={item.onClick} className={styles.breadcrumbLink}>
        {item.label}
      </Link>
    );
  }

  if (item.href) {
    return (
      <a href={item.href} onClick={item.onClick} className={styles.breadcrumbLink}>
        {item.label}
      </a>
    );
  }

  return (
    <button type="button" onClick={item.onClick} className={styles.breadcrumbButton}>
      {item.label}
    </button>
  );
}

function renderAction(action: AppToolbarAction, size: AppButtonSize = "md") {
  return (
    <AppButton
      key={action.key}
      onClick={action.onClick}
      href={action.href}
      variant={action.variant ?? "secondary"}
      size={action.size ?? size}
      leftIcon={action.icon}
      disabled={action.disabled}
      loading={action.loading}
      tooltip={action.tooltip}
      aria-label={action.ariaLabel}
    >
      {action.label}
    </AppButton>
  );
}

export function AppToolbar({
  title,
  subtitle,
  description,
  breadcrumbs = [],
  primaryAction,
  actions = [],
  secondaryActions = [],
  extra,
  children,
  actionContent,
  className,
  collapseBreakpoint = "md",
  maxVisibleSecondaryActions = 2,
  overflowLabel = "More actions",
  sticky = false,
}: AppToolbarProps) {
  const titleId = useId();
  const isCompact = useMediaQuery(MEDIA_QUERY_MAP[collapseBreakpoint]);
  const visibleSecondaryActions = isCompact
    ? []
    : secondaryActions.slice(0, maxVisibleSecondaryActions);
  const overflowActions = isCompact
    ? secondaryActions
    : secondaryActions.slice(maxVisibleSecondaryActions);
  const hasContext =
    Boolean(title) ||
    Boolean(subtitle) ||
    Boolean(description) ||
    breadcrumbs.length > 0 ||
    Boolean(children) ||
    Boolean(extra);

  return (
    <section
      aria-labelledby={titleId}
      className={joinClasses(
        styles.toolbar,
        isCompact && styles.compact,
        !hasContext && styles.contextless,
        sticky && styles.sticky,
        className,
      )}
    >
      {hasContext ? (
        <div className={styles.context}>
          {breadcrumbs.length > 0 ? (
            <nav aria-label="Breadcrumb" className={styles.breadcrumbs}>
              <ol className={styles.breadcrumbList}>
                {breadcrumbs.map((item, index) => (
                  <li
                    key={item.key ?? `${String(item.label)}-${index}`}
                    className={styles.breadcrumbItem}
                    aria-current={item.current ? "page" : undefined}
                  >
                    {item.current ? (
                      <span className={styles.breadcrumbCurrent}>{item.label}</span>
                    ) : (
                      renderBreadcrumb(item)
                    )}
                  </li>
                ))}
              </ol>
            </nav>
          ) : null}

          {title || subtitle || description ? (
            <div className={styles.headingBlock}>
              {subtitle ? <p className={styles.subtitle}>{subtitle}</p> : null}
              {title ? (
                <h2 id={titleId} className={styles.title}>
                  {title}
                </h2>
              ) : null}
              {description ? <p className={styles.description}>{description}</p> : null}
            </div>
          ) : null}

          {children || extra ? (
            <div className={styles.extra}>
              {children}
              {extra}
            </div>
          ) : null}
        </div>
      ) : null}

      <div className={styles.actions} data-compact={isCompact}>
        {actionContent ? <>{actionContent}</> : null}
        {actions.length > 0 ? (
          <div className={styles.actionGroup}>{actions.map((action) => renderAction(action))}</div>
        ) : null}

        {visibleSecondaryActions.length > 0 ? (
          <div className={styles.secondaryActionGroup}>
            {visibleSecondaryActions.map((action) => renderAction(action))}
          </div>
        ) : null}

        {overflowActions.length > 0 ? (
          <AppDropdown
            ariaLabel={overflowLabel}
            items={overflowActions.map((action) => ({
              key: action.key,
              label: action.label,
              icon: action.icon,
              danger: action.variant === "danger",
              disabled: action.disabled || action.loading,
              href: action.href,
              onSelect: action.onClick ? () => action.onClick?.({} as never) : undefined,
            }))}
            trigger={
              <AppButton
                aria-label={overflowLabel}
                variant="ghost"
                size="md"
                icon={<span aria-hidden="true">...</span>}
              />
            }
          />
        ) : null}

        {primaryAction ? (
          <div className={styles.primaryAction}>{renderAction(primaryAction, "md")}</div>
        ) : null}
      </div>
    </section>
  );
}
