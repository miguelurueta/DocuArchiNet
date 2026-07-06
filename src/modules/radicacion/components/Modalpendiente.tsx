import { useState } from "react";
import { Alert, Button, Empty, Modal } from "antd";
import styles from "../style/botonacept.module.css";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faClipboardList } from "@fortawesome/free-solid-svg-icons";

export default function ModalPendiente() {
  const [open, setOpen] = useState(false);

  return (
    <>
      <Button
        type="primary"
        size="large"
        className={styles.btnAcept}
        onClick={() => setOpen(true)}
      >
        <FontAwesomeIcon icon={faClipboardList} />
        Pendientes
      </Button>

      <Modal
        title="Radicados pendientes"
        open={open}
        onCancel={() => setOpen(false)}
        footer={[
          <Button key="close" type="primary" onClick={() => setOpen(false)}>
            Cerrar
          </Button>,
        ]}
        width={720}
      >
        <Alert
          type="info"
          showIcon
          title="Funcionalidad pendiente de integración"
          description="El listado de pendientes estará disponible cuando se implemente el flujo FE-05."
        />
        <Empty
          image={Empty.PRESENTED_IMAGE_SIMPLE}
          description="No hay datos de pendientes disponibles en esta fase."
        />
      </Modal>
    </>
  );
}
