import type { ComponentPropsWithoutRef, ElementType, ReactNode } from "react";
import styles from "./AppContent.module.css";

type AppContentAs = "section" | "article" | "div" | "main";
export type AppContentWidth = "default" | "wide" | "full";
export type AppContentDensity = "comfortable" | "compact";

type AppContentElementProps<TAs extends ElementType> = Omit<
  ComponentPropsWithoutRef<TAs>,
  "children" | "className"
>;

export type AppContentProps<TAs extends ElementType = "section"> = {
  as?: TAs;
  children: ReactNode;
  header?: ReactNode;
  footer?: ReactNode;
  className?: string;
  contentClassName?: string;
  width?: AppContentWidth;
  density?: AppContentDensity;
} & AppContentElementProps<TAs>;

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

export function AppContent<TAs extends AppContentAs = "section">({
  as,
  children,
  header,
  footer,
  className,
  contentClassName,
  width = "default",
  density = "comfortable",
  ...restProps
}: AppContentProps<TAs>) {
  const Component = (as ?? "section") as ElementType;

  return (
    <Component
      {...restProps}
      className={joinClasses(
        styles.root,
        styles[`width${width === "wide" ? "Wide" : width === "full" ? "Full" : "Default"}`],
        styles[`density${density === "compact" ? "Compact" : "Comfortable"}`],
        className,
      )}
    >
      {header ? <div className={styles.header}>{header}</div> : null}
      <div className={joinClasses(styles.body, contentClassName)}>{children}</div>
      {footer ? <div className={styles.footer}>{footer}</div> : null}
    </Component>
  );
}
