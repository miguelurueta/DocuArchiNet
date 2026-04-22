import type { StepsProps } from "antd";
import type { AppStepItem, AppStepsVariant } from "../AppSteps.types";

type NormalizeItemsInput<TData> = {
  items: AppStepItem<TData>[];
  activeIndex: number;
  errorStepIndex: number | null;
  variant: AppStepsVariant;
  titleClassName?: string;
  descriptionClassName?: string;
  timestampClassName?: string;
};

const withCurrentStepSemantics = (
  title: AppStepItem["title"],
  isCurrent: boolean,
  titleClassName?: string,
) => (
  <span aria-current={isCurrent ? "step" : undefined} className={titleClassName}>
    {title}
  </span>
);

export const normalizeItems = <TData,>({
  items,
  activeIndex,
  errorStepIndex,
  variant,
  titleClassName,
  descriptionClassName,
  timestampClassName,
}: NormalizeItemsInput<TData>): StepsProps["items"] =>
  items.map((item, index) => {
    const status = errorStepIndex === index ? "error" : item.status;
    const hasTimelineMeta = variant === "timeline" && Boolean(item.timestamp);

    return {
      key: item.key,
      title: withCurrentStepSemantics(item.title, index === activeIndex, titleClassName),
      description: hasTimelineMeta ? (
        <div className={descriptionClassName}>
          {item.description ? <div>{item.description}</div> : null}
          {item.timestamp ? (
            <time className={timestampClassName} dateTime={item.timestamp}>
              {item.timestamp}
            </time>
          ) : null}
        </div>
      ) : item.description,
      icon: item.icon,
      status,
      disabled: item.disabled,
    };
  });
