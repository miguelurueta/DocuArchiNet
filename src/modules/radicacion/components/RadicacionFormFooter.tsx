import React from "react";
import { Button } from "antd";
import { DeleteFilled, FileFilled, OpenAIFilled } from "@ant-design/icons";
import styles from "../style/FormRadicacion.module.css";

type RadicacionFormFooterProps = {
  onDocumentosIa?: () => void;
  onClear: () => void;
  onSubmit: () => void;
  submitLoading?: boolean;
};

const RadicacionFormFooter: React.FC<RadicacionFormFooterProps> = ({
  onDocumentosIa,
  onClear,
  onSubmit,
  submitLoading = false,
}) => (
  <div className={styles.footer}>
    <Button icon={<OpenAIFilled />} className={styles.btnRad} onClick={onDocumentosIa}>
      Documentos IA
    </Button>

    <div className={styles.rightGroup}>
      <Button icon={<DeleteFilled />} className={styles.btnClear} onClick={onClear}>
        Limpiar
      </Button>

      <Button
        icon={<FileFilled />}
        className={styles.btnRad}
        loading={submitLoading}
        onClick={onSubmit}
      >
        Radicar
      </Button>
    </div>
  </div>
);

export default RadicacionFormFooter;
