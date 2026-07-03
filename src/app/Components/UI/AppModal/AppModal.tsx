import { Modal } from "antd";
import type { ComponentProps, ReactNode } from "react";
import { AppButton, type AppButtonVariant } from "../AppButton";
import styles from "./AppModal.module.css";

type AntModalProps = ComponentProps<typeof Modal>;

export type AppModalAction = {
  label: ReactNode;
  onClick?: () => void;
  variant?: AppButtonVariant;
  loading?: boolean;
  disabled?: boolean;
};

export type AppModalProps = Omit<
  AntModalProps,
  "title" | "open" | "onOk" | "onCancel" | "footer" | "children"
> & {
  open: boolean;
  title?: ReactNode;
  children?: ReactNode;
  primaryAction?: AppModalAction;
  secondaryAction?: AppModalAction;
  onClose?: () => void;
  closeOnEscape?: boolean;
  hideFooter?: boolean;
};

const buildFooter = ({
  hideFooter,
  secondaryAction,
  primaryAction,
}: Pick<AppModalProps, "hideFooter" | "secondaryAction" | "primaryAction">) => {
  if (hideFooter) {
    return null;
  }

  if (!secondaryAction && !primaryAction) {
    return [];
  }

  return (
    <div className={styles.footer}>
      {secondaryAction ? (
        <AppButton
          variant={secondaryAction.variant ?? "secondary"}
          onClick={secondaryAction.onClick}
          disabled={secondaryAction.disabled}
          loading={secondaryAction.loading}
        >
          {secondaryAction.label}
        </AppButton>
      ) : null}

      {primaryAction ? (
        <AppButton
          variant={primaryAction.variant}
          onClick={primaryAction.onClick}
          disabled={primaryAction.disabled}
          loading={primaryAction.loading}
        >
          {primaryAction.label}
        </AppButton>
      ) : null}
    </div>
  );
};

export function AppModal({
  open,
  title,
  children,
  primaryAction,
  secondaryAction,
  onClose,
  closeOnEscape = true,
  hideFooter = false,
  className,
  maskClosable = false,
  ...restProps
}: AppModalProps) {
  return (
    <Modal
      {...restProps}
      open={open}
      title={<div className={styles.title}>{title}</div>}
      onCancel={onClose}
      keyboard={closeOnEscape}
      maskClosable={maskClosable}
      className={className}
      classNames={{
        mask: styles.mask,
        wrapper: styles.wrapper,
        container: styles.content,
        header: styles.header,
        body: styles.body,
        footer: styles.footerShell,
      }}
      footer={buildFooter({ hideFooter, secondaryAction, primaryAction })}
    >
      <div className={styles.bodyContent}>{children}</div>
    </Modal>
  );
}
