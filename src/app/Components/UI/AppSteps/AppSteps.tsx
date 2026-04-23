import { Progress, Steps } from "antd";
import type { StepsProps } from "antd";
import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import type { AppStepItem, AppStepsProps } from "./AppSteps.types";
import { guardStepChange } from "./helpers/guardStepChange";
import { normalizeItems } from "./helpers/normalizeItems";
import { resolveIsControlled } from "./helpers/resolveIsControlled";
import styles from "./AppSteps.module.css";

const joinClasses = (...values: Array<string | false | null | undefined>) =>
  values.filter(Boolean).join(" ");

const clampPercent = (value: number) => Math.max(0, Math.min(100, value));

const resolveValidIndex = <TData,>(items: AppStepItem<TData>[], candidate: number) => {
  if (items.length === 0) return 0;

  const bounded = Math.max(0, Math.min(candidate, items.length - 1));
  if (!items[bounded]?.disabled) return bounded;

  const firstEnabled = items.findIndex((item) => !item.disabled);
  return firstEnabled >= 0 ? firstEnabled : bounded;
};

const sizeToAntSize: Record<NonNullable<AppStepsProps["size"]>, StepsProps["size"]> = {
  sm: "small",
  md: "default",
  lg: "default",
};

const useIsNarrowViewport = (enabled: boolean, maxWidthPx: number) => {
  const [isNarrow, setIsNarrow] = useState(false);

  useEffect(() => {
    if (!enabled || typeof window === "undefined" || typeof window.matchMedia !== "function") {
      setIsNarrow(false);
      return;
    }

    const media = window.matchMedia(`(max-width: ${maxWidthPx}px)`);
    const onChange = (event: MediaQueryListEvent) => {
      setIsNarrow(event.matches);
    };

    setIsNarrow(media.matches);
    media.addEventListener("change", onChange);
    return () => {
      media.removeEventListener("change", onChange);
    };
  }, [enabled, maxWidthPx]);

  return isNarrow;
};

const resolveOrientation = (
  variant: NonNullable<AppStepsProps["variant"]>,
  direction: AppStepsProps["direction"],
) => {
  if (variant === "timeline") return "vertical";
  return direction ?? "horizontal";
};

export function AppSteps<TData = unknown>({
  items,
  current,
  defaultCurrent = 0,
  variant = "default",
  direction,
  size = "md",
  responsive = true,
  validateStep,
  progressPercent,
  onChange,
  className,
}: AppStepsProps<TData>) {
  const isControlled = resolveIsControlled(current);
  const [internalCurrent, setInternalCurrent] = useState(() =>
    resolveValidIndex(items, defaultCurrent),
  );
  const [errorStepIndex, setErrorStepIndex] = useState<number | null>(null);
  const guardRunRef = useRef(0);

  const resolvedCurrent = isControlled
    ? resolveValidIndex(items, current ?? 0)
    : internalCurrent;

  useEffect(() => {
    if (isControlled) return;
    setInternalCurrent((previous) => resolveValidIndex(items, previous));
  }, [isControlled, items]);

  const antOrientation = resolveOrientation(variant, direction);
  const isNarrowViewport = useIsNarrowViewport(
    responsive && variant !== "timeline" && antOrientation === "horizontal",
    900,
  );
  const effectiveOrientation =
    responsive && antOrientation === "horizontal" && isNarrowViewport
      ? "vertical"
      : antOrientation;
  const antSize = sizeToAntSize[size];

  const antItems = useMemo(
    () =>
      normalizeItems({
        items,
        activeIndex: resolvedCurrent,
        errorStepIndex,
        variant,
        titleClassName: styles.stepTitle,
        descriptionClassName: styles.stepDescription,
        timestampClassName: styles.stepTimestamp,
        srOnlyClassName: styles.srOnly,
      }),
    [errorStepIndex, items, resolvedCurrent, variant],
  );

  const handleChange = useCallback(
    async (targetIndex: number) => {
      const targetItem = items[targetIndex];
      if (!targetItem) return;

      const runId = ++guardRunRef.current;
      const guard = await guardStepChange({
        currentIndex: resolvedCurrent,
        targetIndex,
        targetDisabled: targetItem.disabled,
        variant,
        validateStep,
      });

      if (runId !== guardRunRef.current) return;

      if (!guard.canMove) {
        if (guard.showErrorOnCurrent) {
          setErrorStepIndex(resolvedCurrent);
        }
        return;
      }

      setErrorStepIndex(null);
      if (!isControlled) {
        setInternalCurrent(targetIndex);
      }
      onChange?.(targetIndex);
    },
    [isControlled, items, onChange, resolvedCurrent, validateStep, variant],
  );

  return (
    <section
      className={joinClasses(
        styles.root,
        styles[`variant${variant.charAt(0).toUpperCase()}${variant.slice(1)}`],
        styles[`size${size.toUpperCase()}`],
        className,
      )}
    >
      {variant === "progress" && typeof progressPercent === "number" ? (
        <div className={styles.progress}>
          <Progress percent={clampPercent(progressPercent)} size="small" />
        </div>
      ) : null}

      <Steps
        className={styles.steps}
        current={resolvedCurrent}
        orientation={effectiveOrientation}
        size={antSize}
        responsive={responsive}
        items={antItems}
        onChange={(targetIndex) => {
          void handleChange(targetIndex);
        }}
      />
    </section>
  );
}
