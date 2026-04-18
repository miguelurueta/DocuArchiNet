import {
  CheckCircleFilled,
  CloseOutlined,
  DownloadOutlined,
  MailOutlined,
} from "@ant-design/icons";
import { useEffect, useId, useMemo, useRef } from "react";
import { AppButton } from "../../../../../app/Components/UI/AppButton";
import { AppModal } from "../../../../../app/Components/UI/AppModal";
import styles from "./ConfirmacionEnvioModal.module.css";

export type ConfirmacionEnvioModalProps = {
  open: boolean;
  onClose: () => void;
  radicado: string;
  fechaEnvio: string;
  destinatario: string;
  correoEnviado: string;
};

const downloadTextFile = (filename: string, content: string) => {
  const blob = new Blob([content], { type: "text/plain;charset=utf-8" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = filename;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(url);
};

export function ConfirmacionEnvioModal({
  open,
  onClose,
  radicado,
  fechaEnvio,
  destinatario,
  correoEnviado,
}: ConfirmacionEnvioModalProps) {
  const titleId = useId();
  const primaryButtonRef = useRef<HTMLButtonElement | null>(null);

  useEffect(() => {
    if (!open) return;
    // Keep focus inside the modal and land on the primary action.
    setTimeout(() => primaryButtonRef.current?.focus(), 0);
  }, [open]);

  const certificateText = useMemo(() => {
    const lines = [
      "CERTIFICADO DE ENVIO",
      "",
      `Radicado: ${radicado}`,
      `Fecha de envio: ${fechaEnvio}`,
      `Destinatario: ${destinatario}`,
      `Correo enviado: ${correoEnviado}`,
      "",
      "Generado por DocuArchiCore (UI).",
    ];
    return lines.join("\n");
  }, [correoEnviado, destinatario, fechaEnvio, radicado]);

  return (
    <AppModal
      open={open}
      onClose={onClose}
      title={<span id={titleId}>Respuesta enviada correctamente</span>}
      centered
      width="min(640px, 92vw)"
      className={styles.modal}
      wrapClassName={styles.wrap}
      hideFooter
      destroyOnHidden
    >
      <section className={styles.shell} aria-labelledby={titleId}>
        <header className={styles.header}>
          <div className={styles.iconWrap} aria-hidden="true">
            <div className={styles.iconStack}>
              <MailOutlined style={{ fontSize: 46 }} />
              <span className={styles.iconCheck}>
                <CheckCircleFilled style={{ fontSize: 22 }} />
              </span>
            </div>
          </div>
          <p className={styles.lead}>
            El envio fue registrado y el comprobante esta disponible para descarga.
          </p>
        </header>

        <div className={styles.divider} role="separator" />

        <div className={styles.list} aria-label="Resumen de envío">
          <div className={styles.row}>
            <span className={styles.label}>Radicado</span>
            <span className={styles.value}>{radicado}</span>
          </div>
          <div className={styles.row}>
            <span className={styles.label}>Fecha de envío</span>
            <span className={styles.value}>{fechaEnvio}</span>
          </div>
          <div className={styles.row}>
            <span className={styles.label}>Destinatario</span>
            <span className={styles.value}>{destinatario}</span>
          </div>
          <div className={styles.row}>
            <span className={styles.label}>Correo enviado</span>
            <span className={styles.value}>{correoEnviado}</span>
          </div>
        </div>

        <div className={styles.divider} role="separator" />

        <div className={styles.actions} aria-label="Acciones">
          <AppButton
            ref={primaryButtonRef}
            leftIcon={<DownloadOutlined />}
            fullWidth
            onClick={() => {
              downloadTextFile(
                `certificado-envio-${radicado || "sin-radicado"}.txt`,
                certificateText,
              );
            }}
          >
            Descargar certificado de envío
          </AppButton>
          <AppButton variant="secondary" leftIcon={<CloseOutlined />} fullWidth onClick={onClose}>
            Cerrar
          </AppButton>
        </div>
      </section>
    </AppModal>
  );
}
