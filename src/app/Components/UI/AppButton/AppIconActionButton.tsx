import { AppButton, type AppButtonProps } from "./AppButton";

export type AppIconActionButtonProps = Omit<
  AppButtonProps,
  "children" | "leftIcon" | "rightIcon" | "ref"
> & {
  icon: NonNullable<AppButtonProps["icon"]>;
  "aria-label": string;
};

export function AppIconActionButton({
  icon,
  variant = "ghost",
  size = "md",
  tooltip,
  ...restProps
}: AppIconActionButtonProps) {
  return (
    <AppButton
      {...restProps}
      variant={variant}
      size={size}
      icon={icon}
      tooltip={tooltip}
    />
  );
}
