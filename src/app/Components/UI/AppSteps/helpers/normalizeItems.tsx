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
  srOnlyClassName?: string;
};

const withCurrentStepSemantics = (
  title: AppStepItem["title"],
  isCurrent: boolean,
  status: AppStepItem["status"] | undefined,
  titleClassName?: string,
  srOnlyClassName?: string,
) => (
  <span aria-current={isCurrent ? "step" : undefined} className={titleClassName}>
    {title}
    {status ? (
      <span className={srOnlyClassName}>
        {` estado ${status}`}
      </span>
    ) : null}
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
  srOnlyClassName,
}: NormalizeItemsInput<TData>): StepsProps["items"] =>
  items.map((item, index) => {
    const status = errorStepIndex === index ? "error" : item.status;
    const hasTimelineMeta = variant === "timeline" && Boolean(item.timestamp);
    const content = hasTimelineMeta ? (
      <div className={descriptionClassName}>
        {item.description ? <div>{item.description}</div> : null}
        {item.timestamp ? (
          <time className={timestampClassName} dateTime={item.timestamp}>
            {item.timestamp}
          </time>
        ) : null}
      </div>
    ) : item.description ? (
      <div className={descriptionClassName}>{item.description}</div>
    ) : undefined;

    return {
      key: item.key,
      title: withCurrentStepSemantics(
        item.title,
        index === activeIndex,
        status,
        titleClassName,
        srOnlyClassName,
      ),
      content,
      icon: item.icon,
      status,
      disabled: item.disabled,
    };
  });
