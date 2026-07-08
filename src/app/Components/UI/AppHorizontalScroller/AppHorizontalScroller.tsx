import type { CSSProperties, ReactNode } from "react";
import styles from "./AppHorizontalScroller.module.css";

export type AppHorizontalScrollerDensity = "compact" | "comfortable";
export type AppHorizontalScrollerGap = "xs" | "sm" | "md" | "lg";
export type AppHorizontalScrollerSnap = "none" | "start" | "center";

export interface AppHorizontalScrollerProps {
  children: ReactNode;
  ariaLabel: string;
  className?: string;
  viewportClassName?: string;
  contentClassName?: string;
  density?: AppHorizontalScrollerDensity;
  gap?: AppHorizontalScrollerGap;
  itemMinWidth?: number | string;
  itemMaxWidth?: number | string;
  scrollSnap?: AppHorizontalScrollerSnap;
  edgeFade?: boolean;
  testId?: string;
}

type AppHorizontalScrollerStyle = CSSProperties & {
  "--app-horizontal-scroller-item-min-width"?: string;
  "--app-horizontal-scroller-item-max-width"?: string;
};

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const toCssDimension = (value: number | string | undefined): string | undefined => {
  if (typeof value === "number") {
    return Number.isFinite(value) && value > 0 ? `${value}px` : undefined;
  }

  const normalized = value?.trim();
  if (!normalized || normalized.startsWith("-")) {
    return undefined;
  }

  return normalized ? normalized : undefined;
};

const createDimensionStyle = (
  itemMinWidth: number | string | undefined,
  itemMaxWidth: number | string | undefined,
): AppHorizontalScrollerStyle => {
  const style: AppHorizontalScrollerStyle = {};
  const minWidth = toCssDimension(itemMinWidth);
  const maxWidth = toCssDimension(itemMaxWidth);

  if (minWidth) {
    style["--app-horizontal-scroller-item-min-width"] = minWidth;
  }

  if (maxWidth) {
    style["--app-horizontal-scroller-item-max-width"] = maxWidth;
  }

  return style;
};

export function AppHorizontalScroller({
  children,
  ariaLabel,
  className,
  viewportClassName,
  contentClassName,
  density = "comfortable",
  gap = "md",
  itemMinWidth,
  itemMaxWidth,
  scrollSnap = "none",
  edgeFade = false,
  testId,
}: AppHorizontalScrollerProps) {
  return (
    <div
      className={joinClasses(styles.root, edgeFade && styles.edgeFade, className)}
    >
      <div
        role="region"
        aria-label={ariaLabel}
        data-testid={testId}
        className={joinClasses(
          styles.viewport,
          styles[`density${density === "compact" ? "Compact" : "Comfortable"}`],
          viewportClassName,
        )}
      >
        <div
          className={joinClasses(
            styles.content,
            styles[`gap${gap.toUpperCase()}`],
            scrollSnap !== "none" && styles.snap,
            scrollSnap === "start" && styles.snapStart,
            scrollSnap === "center" && styles.snapCenter,
            contentClassName,
          )}
          style={createDimensionStyle(itemMinWidth, itemMaxWidth)}
        >
          {children}
        </div>
      </div>
    </div>
  );
}
