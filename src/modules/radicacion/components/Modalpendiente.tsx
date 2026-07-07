import { useState } from "react";
import { Button } from "antd";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faClipboardList } from "@fortawesome/free-solid-svg-icons";
import styles from "../style/botonacept.module.css";
import { RadicacionPendientesModal } from "./RadicacionPendientesModal";

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

      <RadicacionPendientesModal open={open} onClose={() => setOpen(false)} />
    </>
  );
}
