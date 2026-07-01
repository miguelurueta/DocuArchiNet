import { InboxOutlined } from "@ant-design/icons";
import { useCallback, useState } from "react";
import { AppModal } from "../../../../app/Components/UI/AppModal";
import { GestionRespuestaUploadDocumental } from "./GestionRespuestaUploadDocumental";
import styles from "./GestionRespuestaMainTabContent.module.css";

export function GestionRespuestaUploadDocumentalModal() {
  const [open, setOpen] = useState(false);

  const handleOpen = useCallback(() => {
    setOpen(true);
  }, []);

  const handleClose = useCallback(() => {
    setOpen(false);
  }, []);

  return (
    <>
      <button
        type="button"
        className={styles.uploadModalTrigger}
        onClick={handleOpen}
        aria-label="Abrir carga de documentos adjuntos"
      >
        <span className={styles.uploadModalIcon} aria-hidden="true">
          <InboxOutlined />
        </span>
        <span className={styles.uploadModalCopy}>
          <strong>Adjuntar documentos</strong>
          <span>Haz click para cargar o arrastrar archivos en el modal.</span>
        </span>
      </button>

      <AppModal
        open={open}
        title="Adjuntar documentos"
        width="min(1040px, calc(100vw - 28px))"
        centered
        className={styles.uploadDocumentalModal}
        hideFooter
        destroyOnHidden
        onClose={handleClose}
      >
        <div className={styles.uploadDocumentalModalBody}>
          {open ? (
            <GestionRespuestaUploadDocumental embedded={false} open={open} onClose={handleClose} />
          ) : null}
        </div>
      </AppModal>
    </>
  );
}
