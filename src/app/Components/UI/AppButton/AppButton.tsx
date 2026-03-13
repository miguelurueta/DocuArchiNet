import { forwardRef } from "react";
import { Button as AntButton, Tooltip } from "antd";
import type { ComponentProps, MouseEventHandler, ReactNode } from "react";
import styles from "./AppButton.module.css";

export type AppButtonVariant =
  | "primary"
  | "secondary"
  | "success"
  | "warning"
  | "danger"
  | "ghost"
  | "link";

export type AppButtonSize = "sm" | "md" | "lg";

type AntButtonProps = ComponentProps<typeof AntButton>;

export type AppButtonProps = Omit<
  AntButtonProps,
  "children" | "size" | "type" | "icon" | "htmlType" | "variant" | "loading"
> & {
  children?: ReactNode;
  variant?: AppButtonVariant;
  size?: AppButtonSize;
  loading?: boolean;
  htmlType?: "button" | "submit" | "reset";
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  icon?: ReactNode;
  fullWidth?: boolean;
  tooltip?: string;
};

const VARIANT_CLASS_MAP: Record<AppButtonVariant, string> = {
  primary: styles.variantPrimary,
  secondary: styles.variantSecondary,
  success: styles.variantSuccess,
  warning: styles.variantWarning,
  danger: styles.variantDanger,
  ghost: styles.variantGhost,
  link: styles.variantLink,
};

const SIZE_CLASS_MAP: Record<AppButtonSize, string> = {
  sm: styles.sizeSm,
  md: styles.sizeMd,
  lg: styles.sizeLg,
};

const ANT_SIZE_MAP: Record<AppButtonSize, AntButtonProps["size"]> = {
  sm: "small",
  md: "middle",
  lg: "large",
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const isNonEmptyString = (value: unknown): value is string =>
  typeof value === "string" && value.trim().length > 0;

const wrapDecorativeIcon = (value: ReactNode) =>
  value ? <span aria-hidden="true">{value}</span> : null;

export const AppButton = forwardRef<HTMLButtonElement, AppButtonProps>(
  function AppButton(
    {
      children,
      variant = "primary",
      size = "md",
      loading = false,
      disabled = false,
      htmlType = "button",
      leftIcon,
      rightIcon,
      icon,
      fullWidth = false,
      tooltip,
      className,
      onClick,
      "aria-label": ariaLabel,
      ...restProps
    },
    ref,
  ) {
    const isIconOnly = !children && Boolean(icon);
    const isBlocked = disabled || loading;

    if (isIconOnly && !isNonEmptyString(ariaLabel)) {
      throw new Error("AppButton icon-only requiere `aria-label`.");
    }

    const handleClick: MouseEventHandler<HTMLElement> = (event) => {
      if (isBlocked) {
        event.preventDefault();
        return;
      }

      onClick?.(event);
    };

    const visualIcon = isIconOnly
      ? wrapDecorativeIcon(icon)
      : wrapDecorativeIcon(leftIcon);

    const button = (
      <AntButton
        {...restProps}
        ref={ref}
        htmlType={htmlType}
        size={ANT_SIZE_MAP[size]}
        icon={visualIcon}
        loading={loading}
        disabled={isBlocked}
        aria-disabled={isBlocked}
        aria-label={ariaLabel}
        onClick={handleClick}
        className={joinClasses(
          styles.button,
          VARIANT_CLASS_MAP[variant],
          SIZE_CLASS_MAP[size],
          fullWidth && styles.fullWidth,
          isIconOnly && styles.iconOnly,
          className,
        )}
      >
        {!isIconOnly && children ? (
          <span className={styles.content}>
            {children}
            {rightIcon ? (
              <span className={styles.rightIcon} aria-hidden="true">
                {rightIcon}
              </span>
            ) : null}
          </span>
        ) : null}
      </AntButton>
    );

    if (!tooltip) {
      return button;
    }

    return (
      <Tooltip title={tooltip}>
        <span className={joinClasses(styles.tooltipWrapper, fullWidth && styles.fullWidth)}>
          {button}
        </span>
      </Tooltip>
    );
  },
);
