import { CheckCircleFilled } from "@ant-design/icons";
import { useEffect, useId, useRef } from "react";
import { AppButton } from "../../../../app/Components/UI/AppButton";
import { AppModal } from "../../../../app/Components/UI/AppModal";
import styles from "./TramiteReasignadoModal.module.css";

export type TramiteReasignadoModalProps = {
  open: boolean;
  usuarioAsignado: string;
  radicado: string;
  onClose: () => void;
};

export function TramiteReasignadoModal({
  open,
  usuarioAsignado,
  radicado,
  onClose,
}: TramiteReasignadoModalProps) {
  const titleId = useId();
  const primaryButtonRef = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    if (!open) return;
    setTimeout(() => primaryButtonRef.current?.focus(), 0);
  }, [open]);

  return (
    <AppModal
      open={open}
      onClose={onClose}
      title={null}
      centered
      width="min(500px, 95vw)"
      className={styles.modal}
      wrapClassName={styles.wrap}
      hideFooter
      destroyOnHidden
    >
      <section className={styles.container} aria-labelledby={titleId}>
        <header className={styles.header}>
          <h3 id={titleId} className={styles.title}>
            Trámite Reasignado
            <span className={styles.successIcon} aria-hidden="true">
              <CheckCircleFilled />
            </span>
          </h3>
        </header>

        <div className={styles.content}>
          <p>
            <strong>Usuario Asignado:</strong> {usuarioAsignado}
          </p>
          <p>
            <strong>Radicado:</strong> {radicado}
          </p>
        </div>

        <div className={styles.actions}>
          <AppButton ref={primaryButtonRef} onClick={onClose} fullWidth>
            Aceptar
          </AppButton>
        </div>
      </section>
    </AppModal>
  );
}
